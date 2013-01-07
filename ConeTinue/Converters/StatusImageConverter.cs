using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ConeTinue.Domain;

namespace ConeTinue.Converters
{
	public class StatusImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var iconType = (TestStatus)value;
			switch (iconType)
			{
				case TestStatus.Failed:
					return CreateImageFromPath("01-onebit_10.png");
				case TestStatus.InQueue:
					return CreateImageFromPath("01-onebit_05.png");
				case TestStatus.NotSet:
					return null;
				case TestStatus.Pending:
					return CreateImageFromPath("01-onebit_07.png");
				case TestStatus.Running:
					return CreateImageFromPath("01-onebit_05.png");
				case TestStatus.Skipped:
					return CreateImageFromPath("01-onebit_09.png");
				case TestStatus.Success:
					return CreateImageFromPath("01-onebit_06.png");
				case TestStatus.Unknown:
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