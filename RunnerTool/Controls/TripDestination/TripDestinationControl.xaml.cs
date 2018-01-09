using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für TripDestinationControl.xaml
	/// </summary>
	public partial class TripDestinationControl : UserControl {
		public TripDestinationControl () {
			InitializeComponent();

			DataContext = IoC.TripDestinationViewModel;
		}
	}
}