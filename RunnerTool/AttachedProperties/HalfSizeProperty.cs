using System.Windows;
using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class HalfSizeProperty : BaseAttachedProperty<HalfSizeProperty, int>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			//resize content width on loaded
			(sender as WrapPanel).Loaded += (ss, ee) =>
			{
				WrapPanel wp = ss as WrapPanel;
				wp.ItemWidth = wp.ActualWidth/(int)e.NewValue;
			};

			//resize content width on parent size changed as well
			(sender as WrapPanel).SizeChanged += (ss, ee) =>
			{
				WrapPanel wp = ss as WrapPanel;
				wp.ItemWidth = wp.ActualWidth/(int)e.NewValue;
			};
		}
	}
}