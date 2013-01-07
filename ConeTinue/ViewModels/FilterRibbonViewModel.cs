using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.TestFilters;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class FilterRibbonViewModel : PropertyChangedBase, IHandle<NewTestsLoaded>, IRibbonControlViewModel
	{
		private readonly IEventAggregator eventAggregator;
		private TestItemHolder tests;

		public FilterRibbonViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			eventAggregator.Subscribe(this);
		}

		public void RemoveFilter(FilterTests filterToRemove)
		{
			eventAggregator.Publish(new RemoveFilter(filterToRemove));
		}

		public void Handle(NewTestsLoaded message)
		{
			Tests = message.Tests;
		}

		public TestItemHolder Tests
		{
			get { return tests; }
			set
			{
				if (Equals(value, tests)) return;
				tests = value;
				NotifyOfPropertyChange(() => Tests);
			}
		}

	}
}