using System.Collections.Generic;
using System.ComponentModel;

namespace ConeTinue.ViewModels
{
	public interface IRibbonControlViewProvider : INotifyPropertyChanged
	{
		IEnumerable<IRibbonControlViewModel> Items { get; }
	}
}