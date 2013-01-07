using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
			using (new RemotingServer(myId, eventAggregator))
			{
				foreach (var testAssembly in testAssemblies)
				{
					eventAggregator.Publish(new StatusMessage("Finding tests in " + testAssembly.AssemblyFileName));

					var domainHolder = LoadDomain(testAssembly);
					var tests = domainHolder.Proxy.FindTests();
					foreach (var test in tests)
						allTests.Add(test.WithAssembly(testAssembly));
					eventAggregator.Publish(new StatusMessage("Done loading " + testAssembly.AssemblyFileName));

				}
			}
			testItemHolder = TestItemHolder.CreateFrom(allTests, settings.DefaultAllExpanded);
			eventAggregator.Publish(new NewTestsLoaded(testItemHolder));
			eventAggregator.Publish(new StatusMessage("Done loading tests"));
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


		public bool RunTests()
		{
			try
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
			catch (Exception ex)
			{
				eventAggregator.Publish(new ErrorMessage(ex.ToString()));
				return false;
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