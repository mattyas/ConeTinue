using System;
using System.Linq.Expressions;
using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
	public class RibbonCheckboxViewModel : PropertyChangedBase, IHaveDisplayName, IRibbonControlViewModel
	{
		private readonly IChangeValue settingsStrategy;
		private readonly string propertyName;
		private bool GetValue()
		{
			return settingsStrategy.GetValue(propertyName);
		}
		public string DisplayName { get; set; }
		public string KeyTip { get; private set; }
		public Icon SmallIcon { get; set; }

		public RibbonCheckboxViewModel(string displayName, IChangeValue settingsStrategy, Expression<Func<bool>> property, string keyTip, Icon smallIcon) : this(displayName,settingsStrategy,property.GetMemberInfo().Name, keyTip, smallIcon)
		{
					
		}
		public RibbonCheckboxViewModel(string displayName, IChangeValue settingsStrategy, string propertyName, string keyTip, Icon smallIcon)
		{
			this.propertyName = propertyName;
			this.settingsStrategy = settingsStrategy;
			settingsStrategy.PropertyChanged += (sender, args) => { if (args.PropertyName == propertyName) NotifyOfPropertyChange(propertyName); };
			DisplayName = displayName;
			KeyTip = keyTip;
			SmallIcon = smallIcon;
		}

		public bool IsChecked
		{
			get { return GetValue(); }
			set
			{
				if (value.Equals(GetValue())) return;
				settingsStrategy.SetValue(propertyName, value);
			}
		}
	}
}