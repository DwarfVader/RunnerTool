using RunnerTool.Core;
using System.Windows;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for the new sender list item
	/// </summary>
	public class NewSenderListItemViewModel : SenderListItemViewModel
	{
		public override string Name => "createSender";

		public override string ShortName => "+";

		public override Brush Background => (Brush)Application.Current.Resources["VeryDarkGrayBrush"];

		#region constructor

		public NewSenderListItemViewModel () : base(null)
		{
			Foreground = Brushes.White;
			BorderColor = Brushes.LightYellow;
			BorderThickness = 3;
		}

		#endregion

		#region command methods

		/// <summary>
		/// Resets the input GUI in order to create a new sender
		/// </summary>
		protected override void LeftClickCommandMethod ()
		{
			IoC.CreateSenderViewModel.BeginCreation();
		}

		#endregion
	}
}