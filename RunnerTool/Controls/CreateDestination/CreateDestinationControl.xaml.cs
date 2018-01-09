using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für CreateDestinationControl.xaml
	/// </summary>
	public partial class CreateDestinationControl : UserControl
	{
		public CreateDestinationControl ()
		{
			InitializeComponent();

			DataContext = IoC.CreateDestinationViewModel;
		}
	}
}