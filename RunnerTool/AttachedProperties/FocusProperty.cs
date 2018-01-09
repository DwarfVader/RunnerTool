using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class FocusProperty : BaseAttachedProperty<FocusProperty, bool>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			//set keyboard focus if element gets visible
			(sender as FrameworkElement).IsVisibleChanged += (ss, ee) =>
			{
				FrameworkElement fe = ss as FrameworkElement;
				if (fe.IsVisible)
				{
					fe.Focusable = true;
					fe.Focus();
				}
			};
		}
	}
}