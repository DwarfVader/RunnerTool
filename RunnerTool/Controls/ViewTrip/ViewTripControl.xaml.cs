using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für ViewTripControl.xaml
	/// </summary>
	public partial class ViewTripControl : UserControl {
		public ViewTripControl () {
			InitializeComponent();

			DataContext = IoC.ViewTripViewModel;
		}
	}
}