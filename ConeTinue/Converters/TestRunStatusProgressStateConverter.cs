using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Converters
{
	public class TestRunStatusProgressStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var testRunStatus = (TestRunStatus)value;
			switch (testRunStatus)
			{
				case TestRunStatus.Aborted:
				case TestRunStatus.Success:
				case TestRunStatus.NotStarted:
				case TestRunStatus.Failed:
					return System.Windows.Shell.TaskbarItemProgressState.None;
				case TestRunStatus.Running:
					return System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
