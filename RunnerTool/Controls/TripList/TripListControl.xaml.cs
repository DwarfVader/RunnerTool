using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für TripListControl.xaml
	/// </summary>
	public partial class TripListControl : UserControl {
		public TripListControl () {
			InitializeComponent();

			DataContext = IoC.TripListViewModel;
		}
	}
}