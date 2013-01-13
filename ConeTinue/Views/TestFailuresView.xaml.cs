using System.Windows.Controls;
using ConeTinue.Domain;
using ConeTinue.Domain.CrossDomain;

namespace ConeTinue.Views
{
	/// <summary>
	/// Interaction logic for TestFailuresView.xaml
	/// </summary>
	public partial class TestFailuresView
	{
		public TestFailuresView()
		{
			InitializeComponent();
		}

		private void Failures_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var failure = Failures.SelectedItem as TestFailure;
			Browser.NavigateToString(FailureToHtml.GetHtml(failure));
		}
	}
}
