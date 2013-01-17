using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using Microsoft.Windows.Controls;

namespace ConeTinue.Controls
{
	public class ClickOnKeyTipBehavior : Behavior<ButtonBase>
	{
		protected override void OnAttached()
		{
			KeyTipService.AddKeyTipAccessedHandler(AssociatedObject, (sender, args) => AssociatedObject.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, AssociatedObject)));
			base.OnAttached();
		}
	}
}