using Caliburn.Micro;

namespace ConeTinue.ViewModels
{
	public class RibbonGroupViewModel : Screen
	{
		public BindableCollection<IRibbonControlViewModel> Items { get; private set; }

		public RibbonGroupViewModel()
		{
			Items = new BindableCollection<IRibbonControlViewModel>();
		}

		public void SetProvider(IRibbonControlViewProvider provider)
		{
			this.provider = provider;
			ReloadItems();
			provider.PropertyChanged += (sender, args) => { if (args.PropertyName == "Items") ReloadItems(); };

		}

		private IRibbonControlViewProvider provider;
		private void ReloadItems()
		{
			Items.Clear();
			Items.AddRange(provider.Items);
			NotifyOfPropertyChange(() => Items);
		}

	}
}