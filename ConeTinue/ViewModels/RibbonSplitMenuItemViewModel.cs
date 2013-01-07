using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonSplitMenuItemViewModel : Screen, ICanExecute
	{
		private readonly System.Action execute;
		private bool canExecute;
		public BindableCollection<RibbonSplitMenuItemViewModel> Items { get; private set; }

		public RibbonSplitMenuItemViewModel(string displayName, System.Action execute, Icon icon, string keyTip, IEnumerable<RibbonSplitMenuItemViewModel> items = null, bool canExecute = true)
		{
			Items = new BindableCollection<RibbonSplitMenuItemViewModel>(items ?? new RibbonSplitMenuItemViewModel[0]);
			this.canExecute = canExecute;
			this.execute = execute;
			Icon = icon;
			KeyTip = keyTip;
			base.DisplayName = displayName;
		}

		public void Execute(RoutedEventArgs	sender)
		{
			execute();
			sender.Handled = true;
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

		public Icon Icon { get; private set; }
		public string KeyTip { get; private set; }
	}
}