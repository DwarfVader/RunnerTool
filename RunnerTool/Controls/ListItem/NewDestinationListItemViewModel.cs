using RunnerTool.Core;
using System.Windows;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for the new destination list item
	/// </summary>
	public class NewDestinationListItemViewModel : DestinationListItemViewModel
	{
		public override string Name => "createDestination";

		public override string ShortName => "+";

		public override Brush Background => (Brush)Application.Current.Resources["VeryDarkGrayBrush"];

		#region constructor

		public NewDestinationListItemViewModel () : base(null)
		{
			Foreground = Brushes.White;
			BorderColor = Brushes.LightYellow;
			BorderThickness = 3;
		}

		#endregion

		#region command methods

		/// <summary>
		/// Resets the input GUI in order to create a new destination
		/// </summary>
		protected override void LeftClickCommandMethod ()
		{
			IoC.CreateDestinationViewModel.BeginCreation();
		}

		#endregion
	}
}