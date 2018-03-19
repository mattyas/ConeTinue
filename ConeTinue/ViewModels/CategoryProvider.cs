using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.TestFilters;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class CategoryProvider : PropertyChangedBase, IRibbonControlViewProvider, IHandle<NewTestsLoaded>, IChangeValue<bool>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly SettingsStrategy settingsStrategy;

		public CategoryProvider(IEventAggregator eventAggregator, SettingsStrategy settingsStrategy)
		{
			this.eventAggregator = eventAggregator;
			this.settingsStrategy = settingsStrategy;
			settingsStrategy.PropertyChanged += (sender, args) => { if (args.PropertyName == "ExcludeCategories") Filter();};
			eventAggregator.Subscribe(this);
		}

		private List<TestCategory> categories = new List<TestCategory>();
		private readonly HashSet<string> notSelectedCategories = new HashSet<string>();
		public void Handle(NewTestsLoaded message)
		{
			categories = message.Tests.AllCategories.ToList();
			ReloadItems();
		}

		private readonly List<RibbonCheckboxViewModel> items = new List<RibbonCheckboxViewModel>();

		private void ReloadItems()
		{
			items.Clear();
			int i = 0;
			foreach (var category in categories)
			{
				
				items.Add(new RibbonCheckboxViewModel(category.ToString(),
				                                           this,
														   category.Name, keyTip: (i++).ToString(), smallIcon: Icon.Category));
			}
			NotifyOfPropertyChange(() => Items);
			Filter();
		}

		public IEnumerable<IRibbonControlViewModel> Items
		{
			get { return items; }
		}

		public bool GetValue(string propertyName)
		{
			return !notSelectedCategories.Contains(propertyName);
		}

		public void SetValue(string propertyName, bool value)
		{
			if (value)
				notSelectedCategories.RemoveWhere(x => x == propertyName);
			else
				notSelectedCategories.Add(propertyName);
			Filter();
		}

		private void Filter()
		{
			if (settingsStrategy.ExcludeCategories)
				eventAggregator.Publish(new ExcludeCategoriesFilter(categories.Where(x => notSelectedCategories.Contains(x.Name)).ToList()));
			else
				eventAggregator.Publish(new IncludeCategoriesFilter(categories.Where(x => !notSelectedCategories.Contains(x.Name)).ToList()));
		}
	}
}