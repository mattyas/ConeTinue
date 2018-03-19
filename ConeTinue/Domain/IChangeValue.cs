using System.ComponentModel;

namespace ConeTinue.Domain
{
	public interface IChangeValue<T> : INotifyPropertyChanged
	{
		T GetValue(string propertyName);
		void SetValue(string propertyName, T value);
	}
}