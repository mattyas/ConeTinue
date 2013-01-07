using System.Collections.ObjectModel;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class TestSessionViewModel : IRibbonControlViewModel
	{
		public ObservableCollection<TestAssembly> TestAssemblies { get; private set; }

		public TestSessionViewModel(ObservableCollection<TestAssembly> testAssemblies) 
		{
			TestAssemblies = testAssemblies;
		}
	}
}