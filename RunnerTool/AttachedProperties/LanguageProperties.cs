using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// Takes a token string and sets the text property to the 
	/// corresponding string in selected language
	/// </summary>
	public class LToken : BaseAttachedProperty<LToken, object>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			string token = e.NewValue.ToString();
			if (e.NewValue.GetType().IsEnum)
				token = token.ToLowerFirst();

			UpdateString(sender, token);

			//register framework element: method avoids duplicate entries, no need to check here
			Languages.AddFrameworkElementTokenMapping(sender, token);

			(sender as FrameworkElement).Unloaded += (ss, ee) =>
			{
				//unregister framework element
				Languages.RemoveFrameworkElementTokenMapping(sender);
			};
		}

		public static void UpdateString (DependencyObject sender, string token)
		{
			if (sender is TextBlock)
				(sender as TextBlock).Text = Languages.Get(token);
			else if (sender is Button)
				(sender as Button).Content = Languages.Get(token);
			else
				//a type is missing above
				Debugger.Break();
		}
	}

	/// <summary>
	/// Takes a token string and sets the tooltip property to the 
	/// corresponding string in selected language
	/// </summary>
	public class LTokenTooltip : BaseAttachedProperty<LTokenTooltip, object>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			string token = e.NewValue.ToString();
			if (e.NewValue.GetType().IsEnum)
				token = token.ToLowerFirst();

			UpdateString(sender, token);

			//register framework element: method avoids duplicate entries, no need to check here
			Languages.AddFrameworkElementTooltipTokenMapping(sender, token);

			(sender as FrameworkElement).Unloaded += (ss, ee) =>
			{
				//unregister framework element
				Languages.RemoveFrameworkElementTooltipTokenMapping(sender);
			};
		}

		public static void UpdateString (DependencyObject sender, string token)
		{
			(sender as FrameworkElement).ToolTip = Languages.Get(token);
		}
	}
}