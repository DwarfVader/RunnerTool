using RunnerTool.Core;
using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für ExecuteTripDestinationControl.xaml
	/// </summary>
	public partial class ExecuteTripDestinationControl : UserControl
	{
		public ExecuteTripDestinationControl ()
		{
			InitializeComponent();

			DataContext = IoC.ExecuteTripDestinationViewModel;
		}
	}
}