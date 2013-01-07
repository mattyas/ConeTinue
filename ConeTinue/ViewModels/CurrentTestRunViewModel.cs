using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class CurrentTestRunViewModel : PropertyChangedBase, IHandle<NewTestsLoaded>, IHandle<StartingTestRun>, IHandle<TestRunDone>
	{
		private readonly IEventAggregator eventAggregator;
		private TestItemHolder tests = new TestItemHolder();
		private readonly Stopwatch stopwatch = new Stopwatch();
		private string status = "Select test and press run";

		public CurrentTestRunViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			eventAggregator.Subscribe(this);
		}

		public void Handle(NewTestsLoaded message)
		{
			if (tests != null)
				tests.CurrentTestRun.PropertyChanged -= TestsChanged;
			tests = message.Tests;
			tests.CurrentTestRun.PropertyChanged += TestsChanged;
		}

		private void TestsChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == "Status")
			{
				NotifyOfPropertyChange(() => RunStatus);
				eventAggregator.Publish(new UpdateTestRunStatus(RunStatus));
			}

			if (propertyChangedEventArgs.PropertyName == "CountFinished")
			{
				NotifyOfPropertyChange(() => CountFinished);
			}
			if (propertyChangedEventArgs.PropertyName == "Count")
			{
				NotifyOfPropertyChange(() => Count);
			}
			if (propertyChangedEventArgs.PropertyName == "StatusString")
			{
				NotifyOfPropertyChange(() => TestStatus);
			}
		}

		public TestRunStatus RunStatus
		{
			get { return tests.CurrentTestRun.Status; }
		}

		public int CountFinished
		{
			get { return TestRun.CountFinished; } set{}
		}

		public int Count
		{
			get { return TestRun.Count; }
		}

		public void Handle(StartingTestRun message)
		{
			
			if (message.TestRunType != TestRunType.RunTests)
				return;
			stopwatch.Restart();
			Status = "Running tests";
		}

		public string TestStatus { get { return tests.CurrentTestRun.StatusString; } }
		public string Status
		{
			get { return status; }
			set
			{
				if (value == status) return;
				status = value;
				NotifyOfPropertyChange(() => Status);
			}
		}

		public TestRun TestRun {
			get { return tests.CurrentTestRun; }
		}

		public void Handle(TestRunDone message)
		{
			if (message.TestRunType == TestRunType.FindTests)
				return;
			stopwatch.Stop();
			if (message.TestRunType == TestRunType.RunTests)
			{
				Status = string.Format("Done in {0} - Test time {1}", stopwatch.Elapsed.ToSecondsString(), tests.CurrentTestRun.TotalTestTime.ToSecondsString());
				tests.CurrentTestRun.Done();
			}
			if (message.TestRunType == TestRunType.Aborted)
			{
				Status = string.Format("Aborted after {0}", stopwatch.Elapsed.ToSecondsString());
				tests.CurrentTestRun.Aborted();
			}
		}

	}

}