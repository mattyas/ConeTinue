using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain.CrossDomain
{
	public class CrossDomainTestRunner : IDisposable
	{
		private readonly IEventAggregator eventAggregator;
		private readonly Guid myId;
		private readonly SettingsStrategy settings;

		public CrossDomainTestRunner(IEventAggregator eventAggregator, Guid myId, SettingsStrategy settings)
		{
			this.eventAggregator = eventAggregator;
			this.myId = myId;
			this.settings = settings;
		}

		public void FindTests(IEnumerable<TestAssembly> testAssemblies)
		{
			UnloadDomains();
			var allTests = new SynchronizedCollection<TestInfoWithAssembly>();
			var failedAssemblies = new List<TestAssembly>();
			using (new RemotingServer(myId, eventAggregator))
			{
				foreach (var testAssembly in testAssemblies)
				{
					try
					{

						eventAggregator.Publish(new StatusMessage("Finding tests in " + testAssembly.AssemblyFileName));
						var domainHolder = LoadDomain(testAssembly);
						var tests = domainHolder.Proxy.FindTests();
						foreach (var test in tests)
							allTests.Add(test.WithAssembly(testAssembly));
						eventAggregator.Publish(new StatusMessage("Done loading " + testAssembly.AssemblyFileName));
					}
					catch (Exception ex)
					{
						failedAssemblies.Add(testAssembly);
						eventAggregator.Publish(new ErrorMessage("Failed loading " +testAssembly.AssemblyFileName,"Wrong Cone version?"));
						Debug.Write(ex);
					}


				}
				testItemHolder = TestItemHolderFactory.CreateFrom(allTests, settings.DefaultAllExpanded);
				eventAggregator.Publish(new NewTestsLoaded(testItemHolder));
				if (failedAssemblies.Any())
					eventAggregator.Publish(new ErrorMessage("Failed loading tests" , "Errors in " + string.Join(", ", failedAssemblies.Select(x => x.AssemblyFileName))));
				else
					eventAggregator.Publish(new StatusMessage("Done loading tests"));
			}
		}

		private void UnloadDomains()
		{
			foreach (var domain in domains.Values)
			{
				domain.Dispose();
			}
			domains.Clear();
		}

		private AppDomainHolder LoadDomain(TestAssembly testAssembly)
		{
			AppDomainHolder domainHolder;
			if (domains.TryGetValue(testAssembly, out domainHolder))
				domainHolder.Dispose();
			domainHolder = new AppDomainHolder(testAssembly, myId);
			domains[testAssembly] = domainHolder;
			return domainHolder;
		}


		public bool RunTests(bool runFast)
		{
			try
			{
				if (runFast)
					return RunTestsFast();
				return RunTestsWithFullReporting();
			}
			catch (Exception ex)
			{
				eventAggregator.Publish(new ErrorMessage("Failed running tests", ex.ToString()));
				return false;
			}
		}

		private bool RunTestsFast()
		{
			testItemHolder.MarkTestToRun();
			var reports = new List<FastTestReport>();
			foreach (var domain in domains.Values)
			{
				var testsToRun = new HashSet<string>(testItemHolder.GetTestToRun(domain.Proxy.TestAssembly).Select(x => x.FullName));
				if (testsToRun.Count == 0)
					continue;
				var report = domain.Proxy.RunTestsFast(testsToRun, settings.OutputDebugAndError);
				reports.Add(report);
			}
			foreach (var report in reports)
			{
				foreach (var statuses in report.TestStatuses)
				{
					TestItem item;
					if (testItemHolder.TryGetTest(statuses.Key, out item))
						item.Status = statuses.Value;
				}
				if (report.HasError)
					eventAggregator.Publish(new ErrorMessage("Failed running tests",report.Error));
				foreach (var testTime in report.TestTimes)
				{
					TestItem item;
					if (testItemHolder.TryGetTest(testTime.Key, out item))
						item.RunTime = testTime.Value;
				}
				eventAggregator.Publish(new InfoMessage(report.Output));
				eventAggregator.Publish(new ReportFailures(report.Failures.ToArray()));
			}
			if (reports.Count == 0)
				return true;
			return reports.All(r => r.IsSuccess);
		}

		private bool RunTestsWithFullReporting()
		{
			using (new RemotingServer(myId, eventAggregator, testItemHolder))
			{
				testItemHolder.MarkTestToRun();
				foreach (var domain in domains.Values)
				{
					var testsToRun = new HashSet<string>(testItemHolder.GetTestToRun(domain.Proxy.TestAssembly).Select(x => x.FullName));
					if (testsToRun.Count == 0)
						continue;
					if (!domain.Proxy.RunTests(testsToRun, settings.OutputDebugAndError))
						return false;
				}
				return true;
			}
		}

		public void AbortTests()
		{
			foreach (var domain in domains.Values)
			{
				domain.Proxy.ShouldRunTests = false;
			}			
		}

		private readonly ConcurrentDictionary<TestAssembly, AppDomainHolder> domains = new ConcurrentDictionary<TestAssembly, AppDomainHolder>();
		private TestItemHolder testItemHolder = new TestItemHolder();

		public void Dispose()
		{
			foreach (var appDomain in domains.Values)
			{
				appDomain.Dispose();
			}
		}
	}
}