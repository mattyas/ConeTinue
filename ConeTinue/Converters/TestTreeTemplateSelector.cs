using System.Windows;
using System.Windows.Controls;
using ConeTinue.Domain;

namespace ConeTinue.Converters
{
	public class TestTreeTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TestTemplate { get; set; }
		public DataTemplate TreeTemplate { get; set; }
		public DataTemplate RootTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var testItem = (TestItem) item;
			if (testItem.IsTest)
				return TestTemplate;
			if (testItem is TestItemHolder)
				return RootTemplate;
			return TreeTemplate;
		}
	}
}