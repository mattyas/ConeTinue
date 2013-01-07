using System.Collections.Generic;
using System.ComponentModel;

namespace ConeTinue.ViewModels
{
	public interface ISubMenuProvider : INotifyPropertyChanged
	{
		IEnumerable<RibbonSplitMenuItemViewModel> Items { get; }
	}
}