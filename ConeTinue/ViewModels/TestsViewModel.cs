using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.TestFilters;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class TestsViewModel : PropertyChangedBase, IHandle<NewTestsLoaded>, IHandle<ModifyTests>, IHandle<FilterTests>, IHandle<RemoveFilter>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly SettingsStrategy settings;

		public TestsViewModel(IEventAggregator eventAggregator, SettingsStrategy settings)
		{
			this.eventAggregator = eventAggregator;
			this.settings = settings;
			eventAggregator.Subscribe(this);
		}

		private TestItemHolder tests = new TestItemHolder();
		public TestItem[] Tests
		{
			get { return new TestItem[] { tests }; }
		}

		private void UpdateAllTest(TestItem item, Action<TestItem> update)
		{
			update(item);
			foreach (var testItem in item.Tests)
			{
				UpdateAllTest(testItem, update);
			}
		}

		public void Handle(NewTestsLoaded testItems)
		{
			var oldTestItemHolder = tests;
			tests = testItems.Tests;
			tests.ReApplySettings(oldTestItemHolder);
			NotifyOfPropertyChange(() => Tests);
			if (settings.RunTestsOnReload)
				if (tests.Tests.Any(x => x.ShouldAnyRun))
					eventAggregator.Publish(new RunTests());


		}

		public void Handle(ModifyTests message)
		{
			Task.Factory.StartNew(() =>
				{
					Action<TestItem> update = _ => { };
					switch (message)
					{
						case ModifyTests.ExpandAll:
							update = x => x.IsExpanded = true;
							break;
						case ModifyTests.ExpandOnlyTestsToRun:
							update = x => x.IsExpanded = x.ShouldAnyRun;
							break;
						case ModifyTests.UnCheckAll:
							update = x => x.ShouldRun = false;
							break;
						case ModifyTests.CheckAll:
							update = x => x.ShouldRun = x.IsTest;
							break;
						case ModifyTests.CheckFailed:
							update = x => x.ShouldRun = x.IsTest && x.Status == TestStatus.Failed;
							break;
						case ModifyTests.CollapseAll:
							update = x => x.IsExpanded = false;
							break;
					}
					UpdateAllTest(tests, update);
				});

		}

		public void Handle(FilterTests message)
		{
			tests.Filter(message);
			NotifyOfPropertyChange(() => Tests);
		}

		public void Handle(RemoveFilter message)
		{
			tests.RemoveFilter(message.Filter);
			NotifyOfPropertyChange(() => Tests);
		}
	}
}
