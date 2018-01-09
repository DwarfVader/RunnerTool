using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für BaseInputControl.xaml
	/// </summary>
	public partial class BaseInputControl : UserControl
	{
		public object ChildContent
		{
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ChildContentProperty =
			DependencyProperty.Register("ChildContent", typeof(object), typeof(BaseInputControl), new PropertyMetadata(null));

		public BaseInputControl ()
		{
			InitializeComponent();
		}
	}
}