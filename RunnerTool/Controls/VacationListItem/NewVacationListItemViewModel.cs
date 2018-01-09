using RunnerTool.Core;
using System.Windows;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for the new vaction list item
	/// </summary>
	public class NewVacationListItemViewModel : VacationListItemViewModel
	{
		#region public properties

		public override string Tooltip => "createVacation";

		#endregion

		#region constructor

		public NewVacationListItemViewModel () : base(null) { }

		#endregion

		#region command methods

		/// <summary>
		/// Resets the input GUI in order to create a new sender
		/// </summary>
		protected override void LeftClickCommandMethod ()
		{
			IoC.CreateVacationViewModel.BeginCreation();
		}

		#endregion
	}
}