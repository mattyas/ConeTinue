using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ConeTinue.Domain;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Converters
{
	public class TestRunStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var testRunStatus = (TestRunStatus) value;
			switch (testRunStatus)
			{
					case TestRunStatus.Aborted:
					return Brushes.DarkOrchid;
					case TestRunStatus.Failed:
					return Brushes.Red;
					case TestRunStatus.Success:
					return Brushes.LawnGreen;
			}
			return Brushes.LightSkyBlue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}