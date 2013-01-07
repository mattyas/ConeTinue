using System;
using System.Collections.Generic;
using System.Linq;

namespace ConeTinue.Domain
{
	public class TestItem : DelayedPropertyChangedBase
	{
		public TestItem()
		{
			TestRun = new TestRun();
		}
		private bool shouldRun;
		private TestStatus status = TestStatus.Unknown;
		private bool isInLatestRun;
		private bool isSelected;
		private bool isExpanded;
		private bool isVisible = true;
		private readonly List<TestItem> tests = new List<TestItem>();


		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				if (value.Equals(isExpanded)) return;
				isExpanded = value;
				NotifyOfPropertyChange(() => IsExpanded);
			}
		}

		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (value.Equals(isSelected)) return;
				isSelected = value;
				NotifyOfPropertyChange(() => IsSelected);
			}
		}

		public virtual string Name { get; set; }
		public TestKey TestKey{ get; set; }
		public IEnumerable<TestItem> Tests
		{
			get { return tests.Where(x => x.IsVisible); }
		}

		public bool IsTest { get; set; }
		public virtual bool IsVisible
		{
			get { return isVisible; }
			set { isVisible = value;
				if (value && Parent != null)
					Parent.IsVisible = true;
			}
		}

		public bool ShouldRun
		{
			get
			{
				return shouldRun;
			}
			set
			{
				if (value.Equals(shouldRun)) return;
				shouldRun = value;
				NotifyOfPropertyChange(() => ShouldRun);
			}
		}

		public bool EffectiveShouldRun { get
		{
			if (Parent != null && Parent.EffectiveShouldRun)
				return true;
			return shouldRun;

		} }

		public event EventHandler<StatusChangedEventArgs> StatusChanged;
		private void OnStatusChanged(TestStatus oldStatus, TestStatus newStatus)
		{
			if (StatusChanged != null)
				StatusChanged(this, new StatusChangedEventArgs(oldStatus, newStatus, this));
		}
		public TestStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (value == status) return;
				var oldValue = status;
				status = value;
				NotifyOfPropertyChange(() => Status);
				OnStatusChanged(oldValue, value);
			}
		}
		public TestRun TestRun { get; private set; }
		
		private TimeSpan runTime;


		public TestItem Parent { get; set; }

		public bool IsInLatestRun
		{
			get { return isInLatestRun; }
			set
			{
				if (value.Equals(isInLatestRun)) return;
				isInLatestRun = value;
				NotifyOfPropertyChange(() => IsInLatestRun);
				if (Parent != null && value)
					Parent.IsInLatestRun = true;
			}
		}

		public bool ShouldAnyRun
		{
			get
			{
				if (EffectiveShouldRun)
					return true;
				if (Tests.Any(x => x.ShouldAnyRun))
					return true;
				return false;
			}
		}

		public List<string> Categories { get; set; }

		public string RunTimeInSeconds
		{
			get
			{
				if (RunTime == TimeSpan.Zero)
					return string.Empty;
				return string.Format("{0}s", RunTime.ToSecondsString());
			}
		}

		public TimeSpan RunTime
		{
			get { return runTime; }
			set
			{
				if (value.Equals(runTime)) return;
				runTime = value;
				NotifyOfPropertyChange(() => RunTime);
				NotifyOfPropertyChange(() => RunTimeInSeconds);
			}
		}

		public void AddTest(TestItem newTestItem)
		{
			tests.Add(newTestItem);
		}

		public void SortTests(Comparison<TestItem> comparison)
		{
			tests.Sort(comparison);
		}

		public void DisconnectFromParent()
		{
			if (Parent != null)
				Parent.tests.Remove(this);
			Parent = null;
		}
	}
}