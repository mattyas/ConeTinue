using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain
{

	public class TestRun : DelayedPropertyChangedBase
	{
		public void Reset()
		{
			foreach (var test in tests)
			{
				test.StatusChanged -= TestOnStatusChanged;
			}
			tests.Clear();
			countsPerStatus.Clear();
			Status = TestRunStatus.Running;
			NotifyOfPropertyChange(() => CountFinished);
			NotifyOfPropertyChange(() => Count);

		}
		private readonly List<TestItem> tests = new List<TestItem>();

		public bool HasFailures
		{
			get { return countsPerStatus.Any(x => x.Key == TestStatus.Failed && x.Value > 0); }
		}

		public int Count { get { return tests.Count; } }

		public int CountFinished
		{
			get { return countsPerStatus.Where(x => x.Key != TestStatus.InQueue).Sum(x => x.Value); }
		}
	
		public TimeSpan TotalTestTime { get { return tests.Select(x => x.RunTime).Aggregate(TimeSpan.Zero, (a, b) => a + b); } }
		public string StatusString
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				var statusSummary = string.Join(", ", countsPerStatus.Where(x => x.Value != 0).Select(x => string.Format("{0} {1}", x.Value, x.Key)));
				if (statusSummary.Length == 0)
					stringBuilder.Append("None");
				stringBuilder.Append(statusSummary);
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
		}

		public void AddTest(TestItem test)
		{
			tests.Add(test);
			if (test.Status != TestStatus.Unknown)
				countsPerStatus.AddOrUpdate(test.Status, 1, (status, i) => i + 1);
			test.StatusChanged += TestOnStatusChanged;
			NotifyOfPropertyChange(() => Count);
		}

		private void TestOnStatusChanged(object sender, StatusChangedEventArgs args)
		{
			countsPerStatus.AddOrUpdate(args.NewStatus, 1, (status, i) => i + 1);
			if (args.OldStatus != TestStatus.Unknown)
				countsPerStatus.AddOrUpdate(args.OldStatus, -1, (status, i) => i - 1);
			NotifyOfPropertyChange(() => StatusString);
			if (Status != TestRunStatus.Aborted)
				if (args.NewStatus == TestStatus.Failed || args.OldStatus == TestStatus.Failed)
					Status = HasFailures ? TestRunStatus.Failed : Status;
			NotifyOfPropertyChange(() => CountFinished);
		}

		private readonly ConcurrentDictionary<TestStatus, int> countsPerStatus = new ConcurrentDictionary<TestStatus, int>();
		private TestRunStatus status;

		public TestRunStatus Status
		{
			get { return status; }
			set
			{
				if (value == status) return;
				status = value;
				NotifyOfPropertyChange("Status");
			}
		}

		public void Aborted()
		{
			Status = TestRunStatus.Aborted;
		}

		public void Done()
		{
			Status = Status == TestRunStatus.Running ? TestRunStatus.Success : Status;
		}
	}
}