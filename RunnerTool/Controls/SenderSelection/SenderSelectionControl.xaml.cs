using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für SenderSelectionControl.xaml
	/// </summary>
	public partial class SenderSelectionControl : UserControl {
		public SenderSelectionControl () {
			InitializeComponent();

			DataContext = IoC.SenderSelectionViewModel;
		}
	}
}