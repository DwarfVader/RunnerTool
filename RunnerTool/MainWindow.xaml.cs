using System.Windows;

namespace RunnerTool {
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow () {
			InitializeComponent();

			DataContext = new WindowViewModel(this);
		}
	}
}