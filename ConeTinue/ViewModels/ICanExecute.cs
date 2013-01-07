namespace ConeTinue.ViewModels
{
	public interface ICanExecute : IRibbonControlViewModel
	{
		bool CanExecute { get; set; }
	}
}