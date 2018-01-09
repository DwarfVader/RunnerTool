using RunnerTool.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// Converts a destination type to a background color brush
	/// </summary>
	public class DestinationTypeToBackgroundConverter : BaseValueConverter<DestinationTypeToBackgroundConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Brushes.White;
			//return Application.Current.Resources["VeryDarkGrayBrush"];

			switch (((Destination)value).Type)
			{
			case DestinationType.Dr:
				return Application.Current.Resources["DestinationDrBrush"];
			case DestinationType.Shop:
				return Application.Current.Resources["DestinationShopBrush"];
			case DestinationType.Other:
				return Application.Current.Resources["DestinationOtherBrush"];
			default:
				Debugger.Break();
				return null;
			}
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IsAbandonedColorConverter : BaseValueConverter<IsAbandonedColorConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value ? Brushes.Red : (Brush)Application.Current.Resources["VeryDarkGrayBrush"]);
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class VisitPurposeStateForegroundConverter : BaseValueConverter<VisitPurposeStateForegroundConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((VisitPurposeState)value)
			{
			case VisitPurposeState.Unhandled:
				return Application.Current.Resources["VeryDarkGrayBrush"];
			case VisitPurposeState.Finished:
				return Brushes.Green;
			case VisitPurposeState.Abandoned:
				return Brushes.DarkRed;
			}

			//dummy
			return Brushes.Transparent;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ExecuteTripDestinationStateColorConverter : BaseValueConverter<ExecuteTripDestinationStateColorConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((ExecuteTripDestinationState)value)
			{
			case ExecuteTripDestinationState.NotAvailable:
				return Brushes.Gray;
			case ExecuteTripDestinationState.Unhandled:
				return Brushes.LightGray;
			case ExecuteTripDestinationState.PartsCompleted:
				return Brushes.LightYellow;
			case ExecuteTripDestinationState.Completed:
				return Brushes.LightGreen;
			case ExecuteTripDestinationState.Abandoned:
				return Brushes.LightCoral;
			}

			//dummy
			return Brushes.Transparent;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ExecuteTripSenderStateColorConverter : BaseValueConverter<ExecuteTripSenderStateColorConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((ExecuteTripSenderState)value)
			{
			case ExecuteTripSenderState.Unhandled:
				return Brushes.LightGray;
			case ExecuteTripSenderState.PartsCompleted:
				return Brushes.LightYellow;
			case ExecuteTripSenderState.Completed:
				return Brushes.LightGreen;
			case ExecuteTripSenderState.Abandoned:
				return Brushes.LightCoral;
			}

			//dummy
			return Brushes.Transparent;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}