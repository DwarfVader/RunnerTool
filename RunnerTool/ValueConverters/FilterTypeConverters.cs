using RunnerTool.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// Converts a filter mode to an icon
	/// </summary>
	public class FilterModeToSymbolConverter : BaseValueConverter<FilterModeToSymbolConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((TripFilterMode)value)
			{
			case TripFilterMode.Union:
				return Application.Current.Resources["UnionIcon"];
			case TripFilterMode.Intersection:
				return Application.Current.Resources["IntersectionIcon"];
			case TripFilterMode.InnerIntersection:
				return Application.Current.Resources["IntersectionIcon"];
			}

			throw new Exception("Illegal argument");
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Converts a filter mode to a string
	/// </summary>
	public class FilterModeToTooltipConverter : BaseValueConverter<FilterModeToTooltipConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((TripFilterMode)value)
			{
			case TripFilterMode.Union:
				return "Union (whole trip)";
			case TripFilterMode.Intersection:
				return "Intersection (whole trip)";
			case TripFilterMode.InnerIntersection:
				return "Intersection (single trip destination)";
			}

			throw new Exception("Illegal argument");
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Converts a filter mode to a background color
	/// </summary>
	public class FilterModeToBackgroundConverter : BaseValueConverter<FilterModeToBackgroundConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((TripFilterMode)value)
			{
			case TripFilterMode.Union:
			case TripFilterMode.Intersection:
				return Brushes.LimeGreen;
			case TripFilterMode.InnerIntersection:
				return Brushes.Red;
			}

			throw new Exception("Illegal argument");
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}