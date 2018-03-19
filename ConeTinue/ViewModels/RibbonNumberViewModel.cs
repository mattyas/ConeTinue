using System;
using System.Linq.Expressions;
using Caliburn.Micro;
using ConeTinue.Domain;

namespace ConeTinue.ViewModels
{
    public class RibbonNumberViewModel : PropertyChangedBase, IHaveDisplayName, IRibbonControlViewModel
    {
        private readonly IChangeValue<int> settingsStrategy;
        private readonly string propertyName;
        private int GetValue()
        {
            return settingsStrategy.GetValue(propertyName);
        }
        public string DisplayName { get; set; }
        public string KeyTip { get; private set; }
        public Icon SmallIcon { get; set; }

        public RibbonNumberViewModel(string displayName, IChangeValue<int> settingsStrategy, Expression<Func<int>> property, string keyTip, Icon smallIcon) : this(displayName, settingsStrategy, property.GetMemberInfo().Name, keyTip, smallIcon)
        {

        }
        public RibbonNumberViewModel(string displayName, IChangeValue<int> settingsStrategy, string propertyName, string keyTip, Icon smallIcon)
        {
            this.propertyName = propertyName;
            this.settingsStrategy = settingsStrategy;
            settingsStrategy.PropertyChanged += (sender, args) => { if (args.PropertyName == propertyName) NotifyOfPropertyChange(propertyName); };
            DisplayName = displayName;
            KeyTip = keyTip;
            SmallIcon = smallIcon;
        }

        public int Value
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