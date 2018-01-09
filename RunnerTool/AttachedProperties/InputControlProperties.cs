using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class AlignInputControlsProperty : BaseAttachedProperty<AlignInputControlsProperty, bool>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(sender is StackPanel sp))
				return;

			//SetSizes(sp);

			RoutedEventHandler onLoaded = null;
			onLoaded = (ss, ee) =>
			{
				sp.Loaded -= onLoaded;

				SetSizes(sp);
			};
			sp.Loaded += onLoaded;

			Languages.LanguageChanged += () => SetSizes(sp);
		}

		void SetSizes (StackPanel sp)
		{
			double maxWidth = 0;
			double maxHeight = 0;

			//get largest child's width
			foreach (var item in sp.Children)
			{
				if (item is BaseInputControl input)
				{
					//need this to calculate the real text size
					FormattedText ft = new FormattedText(input.LabelText.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
						new Typeface(input.LabelText.FontFamily.ToString()), input.LabelText.FontSize, Brushes.White,
						VisualTreeHelper.GetDpi(Application.Current.MainWindow).DpiScaleX);

					maxWidth = Math.Max(maxWidth, ft.Width);
					maxHeight = Math.Max(maxHeight, input.ActualHeight);
				}
			}

			//set each element's width
			foreach (var item in sp.Children)
			{
				if (item is BaseInputControl input)
				{
					input.LabelText.Width = maxWidth;
					input.Height = maxHeight;
				}
			}
		}
	}
}