using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für CreateSenderControl.xaml
	/// </summary>
	public partial class CreateSenderControl : UserControl
	{
		public CreateSenderControl ()
		{
			InitializeComponent();

			DataContext = IoC.CreateSenderViewModel;
		}
	}
}