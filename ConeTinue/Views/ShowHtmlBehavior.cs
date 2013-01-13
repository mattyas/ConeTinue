using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ConeTinue.Views
{
	public class ShowHtmlBehavior : Behavior<WebBrowser>
	{


		public string HtmlText
		{
			get { return (string)GetValue(HtmlTextProperty); }
			set { SetValue(HtmlTextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for HtmlText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HtmlTextProperty =
			DependencyProperty.Register("HtmlText", typeof(string), typeof(ShowHtmlBehavior), new PropertyMetadata(TextChanged));

				

		private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var behavior = d as ShowHtmlBehavior;
			if (behavior == null)
				return;
			if (behavior.AssociatedObject == null)
				return;
			behavior.AssociatedObject.NavigateToString((string)e.NewValue);
		}
	}
}