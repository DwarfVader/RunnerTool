using RunnerTool.Core;
using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für CreateTripControl.xaml
	/// </summary>
	public partial class CreateTripControl : UserControl {
		public CreateTripControl () {
			InitializeComponent();

			DataContext = IoC.CreateTripViewModel;
		}
	}
}