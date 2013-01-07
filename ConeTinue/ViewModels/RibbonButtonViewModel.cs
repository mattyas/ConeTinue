using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonButtonViewModel : Screen, IRibbonControlViewModel, ICanExecute
	{
		private readonly System.Action execute;
		private bool canExecute;

		public RibbonButtonViewModel(string displayName, System.Action execute, Icon icon, string keyTip, bool canExecute = true)
		{
			this.canExecute = canExecute;
			this.execute = execute;
			LargeIcon = icon;
			SmallIcon = icon;
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

		public Icon LargeIcon { get; private set; }
		public Icon SmallIcon { get; private set; }
		public string KeyTip { get; private set; }
	}
}