using System;
using System.Globalization;

namespace ConeTinue.Domain
{
	public static class TimeSpanExtensions
	{
		private static readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };
		public static string ToSecondsString(this TimeSpan time)
		{
			return time.TotalSeconds.ToString("F3", numberFormatInfo);
		}
	}
}