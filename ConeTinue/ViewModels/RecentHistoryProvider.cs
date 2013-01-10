using System.Collections.Generic;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class RecentHistoryProvider : PropertyChangedBase, ISubMenuProvider
	{
		private readonly SettingsStrategy settings;
		private readonly IEventAggregator eventAggregator;
		private readonly List<RibbonSplitMenuItemViewModel> items = new List<RibbonSplitMenuItemViewModel>();

		public RecentHistoryProvider(SettingsStrategy settings, IEventAggregator eventAggregator)
		{
			this.settings = settings;
			this.eventAggregator = eventAggregator;
			ReloadMenu();
			settings.PropertyChanged += (sender, args) => { if (args.PropertyName == "Recent") ReloadMenu(); };
		}

		private void ReloadMenu()
		{
			items.Clear();
			int i = 0;
			foreach (var path in settings.Recent)
			{
				string localPath = path;
				items.Add(new RibbonSplitMenuItemViewModel(path,
				                                           () => eventAggregator.Publish(new AddTestAssemblies(localPath)),
				                                           Icon.AddTestAssembly, keyTip: (i++).ToString()));
			}
			NotifyOfPropertyChange(() => Items);
		}

		public IEnumerable<RibbonSplitMenuItemViewModel> Items
		{
			get { return items; }
		}
	}
}