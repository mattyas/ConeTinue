using Caliburn.Micro;
using ConeTinue.Domain.TestFilters;

namespace ConeTinue.ViewModels
{
	public class FilterViewModel : PropertyChangedBase
	{
		private readonly IEventAggregator eventAggregator;

		public FilterViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
		}

		private string filter = string.Empty;
		private bool showWhenMatching = true;

		public string Filter
		{
			get { return filter; }
			set
			{
				if (value == filter) return;
				filter = value;
				NotifyOfPropertyChange(() => Filter);
				NotifyOfPropertyChange(() => CanNewFilterTests);
				NotifyOfPropertyChange(() => CanContinueFilterTests);
			}
		}

		public bool ShowWhenMatching
		{
			get { return showWhenMatching; }
			set
			{
				if (value.Equals(showWhenMatching)) return;
				showWhenMatching = value;
				NotifyOfPropertyChange(() => ShowWhenMatching);
			}
		}

		public bool CanContinueFilterTests
		{
			get { return filter.Length != 0; }
		}

		public bool CanNewFilterTests
		{
			get { return filter.Length != 0; }
		}


		public void ContinueFilterTests()
		{
			FilterTests();
		}

		public void FilterTests()
		{
			eventAggregator.Publish(new FilterOnTextWithReplaceUnderscore(new FilterOnText(Filter, ShowWhenMatching)));
			Filter = string.Empty;
		}

		public void NewFilterTests()
		{
			ClearFilter();
			FilterTests();
		}

		public void ClearFilter()
		{
			eventAggregator.Publish(new ClearFilters());
		}
	}
}