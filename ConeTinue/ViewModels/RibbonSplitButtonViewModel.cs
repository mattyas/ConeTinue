using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonSplitButtonViewModel : Screen, ICanExecute
	{
		private readonly System.Action execute;
		private bool canExecute;
		public BindableCollection<RibbonSplitMenuItemViewModel> Items { get; private set; }

		public RibbonSplitButtonViewModel(string displayName, System.Action execute, Icon icon, string keyTip, string headerKeyTip, ISubMenuProvider subMenuProvider = null, IEnumerable<RibbonSplitMenuItemViewModel> items = null, bool canExecute = true, bool showToolTip = false)
		{
			Items = new BindableCollection<RibbonSplitMenuItemViewModel>(items ?? new RibbonSplitMenuItemViewModel[0]);
			if (subMenuProvider != null)
			{
				subMenuProvider.PropertyChanged += (sender, args) =>
					{ if (args.PropertyName == "Items") ReloadSubMenu(subMenuProvider); };
				ReloadSubMenu(subMenuProvider);
			}
			this.canExecute = canExecute;
			this.execute = execute;
			LargeIcon = icon;
			SmallIcon = icon;
			KeyTip = keyTip;
			HeaderKeyTip = headerKeyTip;
			if (showToolTip)
				ToolTip = displayName;

			base.DisplayName = displayName;
		}

		private void ReloadSubMenu(ISubMenuProvider subMenuProvider)
		{
			Items.Clear();
			Items.AddRange(subMenuProvider.Items);
			NotifyOfPropertyChange(() =>Items);
		}

		public void Execute()
		{
			execute();
		}

		public bool CanExecute
		{
			get { return canExecute; }
			set
			{
				if (value.Equals(canExecute)) return;
				canExecute = value;
				NotifyOfPropertyChange(() => CanExecute);
			}
		}

		public string ToolTip { get; private set; }
		public Icon LargeIcon { get; private set; }
		public Icon SmallIcon { get; private set; }
		public string KeyTip { get; private set; }
		public string HeaderKeyTip { get; private set; }
	}
}