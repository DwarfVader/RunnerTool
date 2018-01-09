using System;
using System.Globalization;

namespace RunnerTool
{
	/// <summary>
	/// Converts a <see cref="SenderType"/> to a name string
	/// </summary>
	public class DateToShortDateConverter : BaseValueConverter<DateToShortDateConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime)value).ToShortDateString();
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}