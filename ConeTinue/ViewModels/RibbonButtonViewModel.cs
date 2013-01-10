using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonButtonViewModel : Screen, ICanExecute
	{
		private readonly System.Action execute;
		private bool canExecute;

		public RibbonButtonViewModel(string displayName, System.Action execute, Icon icon, string keyTip, bool canExecute = true, bool showToolTip = false)
		{
			this.canExecute = canExecute;
			this.execute = execute;
			LargeIcon = icon;
			SmallIcon = icon;
			KeyTip = keyTip;
			if (showToolTip)
				ToolTip = displayName;
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

		public object ToolTip { get; private set; }
		public Icon LargeIcon { get; private set; }
		public Icon SmallIcon { get; private set; }
		public string KeyTip { get; private set; }
	}
}