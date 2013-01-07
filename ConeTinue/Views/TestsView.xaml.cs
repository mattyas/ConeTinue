using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConeTinue.Views
{
	/// <summary>
	/// Interaction logic for Tests.xaml
	/// </summary>
	public partial class TestsView : UserControl
	{
		public TestsView()
		{
			InitializeComponent();
		}

		private void TreeViewSelectedItemChanged(object sender, RoutedEventArgs e)
		{
			var item = sender as TreeViewItem;
			if (item == null) return;
			item.BringIntoView();
			e.Handled = true;
		}

	}
}
