using System.ComponentModel;

namespace ConeTinue.Domain
{
	public interface IChangeValue : INotifyPropertyChanged
	{
		bool GetValue(string propertyName);
		void SetValue(string propertyName, bool value);
	}
}