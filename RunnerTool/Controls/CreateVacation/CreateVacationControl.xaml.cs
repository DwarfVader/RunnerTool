using System.Windows.Controls;

namespace RunnerTool
{
	/// <summary>
	/// Interaktionslogik für CreateVacationControl.xaml
	/// </summary>
	public partial class CreateVacationControl : UserControl
	{
		public CreateVacationControl ()
		{
			InitializeComponent();

			DataContext = IoC.CreateVacationViewModel;
		}
	}
}