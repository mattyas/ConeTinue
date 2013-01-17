using System.Windows.Controls;
using System.Windows.Interactivity;
using Microsoft.Windows.Controls;

namespace ConeTinue.Controls
{
	public class SetIsSelectedOnKeyTipBehavior : Behavior<ListBoxItem>
	{
		protected override void OnAttached()
		{
			KeyTipService.AddKeyTipAccessedHandler(AssociatedObject, (sender, args) => AssociatedObject.IsSelected = true);
			base.OnAttached();
		}
	}
}