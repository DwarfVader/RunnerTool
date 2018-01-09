using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für DestinationSelectionControl.xaml
	/// </summary>
	public partial class DestinationSelectionControl : UserControl
	{
		public DestinationSelectionControl ()
		{
			InitializeComponent();

			DataContext = IoC.DestinationSelectionViewModel;
		}
	}
}