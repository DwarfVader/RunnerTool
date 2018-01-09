using System.Windows.Controls;

namespace RunnerTool {
	/// <summary>
	/// 
	/// </summary>
	public partial class VisitPurposeSelectionListControl : UserControl {
		public VisitPurposeSelectionListControl () {
			InitializeComponent();

			DataContext = IoC.VisitPurposeSelectionListViewModel;
		}
	}
}