using RunnerTool.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// Converts a <see cref="SenderType"/> to a background color
	/// </summary>
	public class SenderToBackgroundConverter : BaseValueConverter<SenderToBackgroundConverter>
	{
		public override object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Brushes.White;

			switch (((Sender)value).Type.Order)
			{
			case 0:
				return Brushes.LightGreen;
			case 1:
				return Brushes.LightSkyBlue;
			case 2:
				return Brushes.Yellow;
			case 3:
				return Brushes.LightCoral;
			case 4:
				return Brushes.Violet;
			}

			Debugger.Break();
			return null;
		}

		public override object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}