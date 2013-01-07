using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Converters
{
	public class TestRunStatusIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var testRunStatus = (TestRunStatus)value;
			switch (testRunStatus)
			{
				case TestRunStatus.Failed:
					return CreateImageFromPath("01-onebit_10.png");
				case TestRunStatus.Running:
					return CreateImageFromPath("01-onebit_05.png");
				case TestRunStatus.Aborted:
					return CreateImageFromPath("01-onebit_09.png");
				case TestRunStatus.Success:
					return CreateImageFromPath("01-onebit_06.png");
				case TestRunStatus.NotStarted:
					return null;
			}
			return null;
		}

		public static ImageSource CreateImageFromPath(string imageName)
		{
			return BitmapFrame.Create(new Uri("pack://application:,,,/ConeTinue;component/Images/" + imageName, UriKind.RelativeOrAbsolute));
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}