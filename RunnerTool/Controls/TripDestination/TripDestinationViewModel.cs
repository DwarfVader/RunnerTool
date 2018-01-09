using PropertyChanged;
using RunnerTool.Core;
using System.Collections.Generic;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class TripDestinationViewModel : BaseVisibleViewModel
	{
		#region private members

		bool isVisible;
		TripDestination tripDestination;

		#endregion

		#region public properties

		/// <summary>
		/// The trip
		/// </summary>
		[DoNotCheckEquality]
		public TripDestination TripDestination
		{
			get => tripDestination;
			set
			{
				tripDestination = value;

				IsVisible = true;
			}
		}

		/// <summary>
		/// Whether this control is visible
		/// </summary>
		public override bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				IoC.ViewTripViewModel.RecalcOverlayVisibility();
			}
		}

		#endregion

		#region commands

		public ICommand EditCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public TripDestinationViewModel ()
		{
			//commands
			EditCommand = new RelayCommand(EditCommandMethod);
		}

		#endregion

		#region command methods

		protected override void CloseCommandMethod ()
		{
			IsVisible = false;
		}

		void EditCommandMethod ()
		{

		}

		#endregion
	}
}