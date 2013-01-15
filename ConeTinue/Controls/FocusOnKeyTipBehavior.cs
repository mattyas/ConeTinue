using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Windows.Controls;

namespace ConeTinue.Controls
{
	public class FocusOnKeyTipBehavior : Behavior<UIElement>
	{
		protected override void OnAttached()
		{
			KeyTipService.AddKeyTipAccessedHandler(AssociatedObject, (sender, args) => AssociatedObject.Focus());
			base.OnAttached();
		}
	}
}