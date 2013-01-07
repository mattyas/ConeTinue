using System.Collections.Generic;
using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonMenuItemViewModel : Screen, ICanExecute
	{
		private readonly System.Action execute;
		private bool canExecute;
		public BindableCollection<RibbonMenuItemViewModel> Items { get; private set; }

		public RibbonMenuItemViewModel(string displayName, System.Action execute, Icon icon, string keyTip, IEnumerable<RibbonMenuItemViewModel> items = null, bool canExecute = true)
		{
			Items = new BindableCollection<RibbonMenuItemViewModel>(items ?? new RibbonMenuItemViewModel[0]);
			this.canExecute = canExecute;
			this.execute = execute;
			Icon = icon;
			KeyTip = keyTip;
			base.DisplayName = displayName;
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

		public Icon Icon { get; private set; }
		public string KeyTip { get; private set; }
	}
}