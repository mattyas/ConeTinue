using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ConeTinue.Domain;

namespace ConeTinue.Converters
{
	public class IconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var iconType = (Icon)value;
			switch (iconType)
			{
				case Icon.Run:
					return CreateImageFromPath("onebit_08.png");
				case Icon.AddTestAssembly:
					return CreateImageFromPath("060.png");
				case Icon.ClearTestSession:
					return CreateImageFromPath("059.png");
				case Icon.CheckAll:
					return CreateImageFromPath("030.png");
				case Icon.CheckFailed:
					return CreateImageFromPath("028.png");
				case Icon.UnCheckAll:
					return CreateImageFromPath("029.png");
				case Icon.AllVisible:
					return CreateImageFromPath("020.png");
				case Icon.ExpandOnlyTestsToRun:
					return CreateImageFromPath("021.png");
				case Icon.CollapseAll:
					return CreateImageFromPath("019.png");
				case Icon.ClearFilters:
					return CreateImageFromPath("069.png");
				case Icon.ShowOnlyTestsToRun:
					return CreateImageFromPath("071.png");
				case Icon.ReloadTestSession:
					return CreateImageFromPath("052.png");
				case Icon.AbortTestRun:
					return CreateImageFromPath("034.png");
				case Icon.Category:
					return CreateImageFromPath("040.png");
				case Icon.Info:
					return CreateImageFromPath("onebit_47.png");
				case Icon.Error:
					return CreateImageFromPath("onebit_49.png");
				case Icon.VisualStudio:
					return CreateImageFromPath("onebit_02.png");
				case Icon.RunFast:
					return CreateImageFromPath("bonus48x48_20.png");
				case Icon.LoadTestSessionFromFailedTests:
					return CreateImageFromPath("027.png");
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