using System;
using System.Globalization;
using System.Windows;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
	{
		public override object Convert (object value, Type targetType, object invertValue, CultureInfo culture)
		{
			return ((bool)value ^ (invertValue != null && bool.Parse(invertValue.ToString()))) ? Visibility.Visible : Visibility.Hidden;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BooleanToVisibilityCollapsedConverter : BaseValueConverter<BooleanToVisibilityCollapsedConverter>
	{
		public override object Convert (object value, Type targetType, object invertValue, CultureInfo culture)
		{
			return ((bool)value ^ (invertValue != null && bool.Parse(invertValue.ToString()))) ? Visibility.Visible : Visibility.Collapsed;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}