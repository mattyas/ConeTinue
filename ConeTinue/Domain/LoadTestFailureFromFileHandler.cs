using System;
using System.Linq;
using Caliburn.Micro;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.ViewModels;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain
{
	public class LoadTestFailureFromFileHandler : IHandle<LoadTestAssemblyFromFailedTests>, IHandle<TestRunDone>, IHandle<NewTestsLoaded>
	{
		private readonly IEventAggregator eventAggregator;

		public LoadTestFailureFromFileHandler(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			eventAggregator.Subscribe(this);
		}

		public void Handle(LoadTestAssemblyFromFailedTests message)
		{
			try
			{
				testsToSelectOnNextReload = FindFailedTests.FindFailures(message.Path);
				eventAggregator.Publish(new ClearTestSession());
				eventAggregator.Publish(new TestFilters.ClearFilters());
				foreach (var path in testsToSelectOnNextReload.Select(x => x.TestKey.TestAssembly.AssemblyPath).Distinct())
				{
					eventAggregator.Publish(new AddTestAssembly(path));
				}
			}
			catch (Exception ex)
			{
				eventAggregator.Publish(new ErrorMessage(ex.ToString()));
			}

		}

		private TestFailure[] testsToSelectOnNextReload;


		public void Handle(TestRunDone message)
		{
			if (testsToSelectOnNextReload == null)
				return;
			if (message.TestRunType != TestRunType.FindTests)
				return;
			foreach (var testFailure in testsToSelectOnNextReload)
			{
				TestItem item;
				if (tests.TryGetTest(testFailure.TestKey, out item))
					item.Status = TestStatus.Failed;
				eventAggregator.Publish(testFailure);
			}
			eventAggregator.Publish(ModifyTests.CheckFailed);
			eventAggregator.Publish(new TestFilters.ShowOnlyTestsToRun());
			testsToSelectOnNextReload = null;
		}

		private TestItemHolder tests = new TestItemHolder();
		public void Handle(NewTestsLoaded message)
		{
			tests = message.Tests;
		}

	}
}