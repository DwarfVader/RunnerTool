using RunnerTool.Core;
using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für ExecuteTripControl.xaml
	/// </summary>
	public partial class ExecuteTripControl : UserControl
	{
		public ExecuteTripControl ()
		{
			InitializeComponent();

			DataContext = IoC.ExecuteTripViewModel;
		}
	}
}