using Caliburn.Micro;

namespace ConeTinue.ViewModels
{
	public class RibbonTabViewModel : Screen
	{
		public string KeyTip { get; private set; }
		public BindableCollection<RibbonGroupViewModel> Groups { get; private set; }

		public RibbonTabViewModel(string keyTip)
		{
			KeyTip = keyTip;
			Groups = new BindableCollection<RibbonGroupViewModel>();
		}
	}
}