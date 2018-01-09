using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class ExecuteTripDestinationViewModel : BaseVisibleViewModel
	{
		#region public properties

		/// <summary>
		/// The current trip
		/// </summary>
		public Trip Trip { get; set; }

		/// <summary>
		/// The destination of interest
		/// </summary>
		public ExecuteTripDestinationVM DestinationVM { get; set; }

		/// <summary>
		/// The trip destinations that lead to the destination
		/// </summary>
		public ObservableCollection<ExecuteTripSenderListItemViewModel> TripSenderVMs { get; private set; }

		public string TitleText => DestinationVM?.Destination.Name + ", " + Trip?.Date.ToShortDateString();

		#endregion

		#region commands

		public ICommand ResetCommand { get; private set; }
		public ICommand FinishCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExecuteTripDestinationViewModel ()
		{
			TripSenderVMs = new ObservableCollection<ExecuteTripSenderListItemViewModel>();

			//commands
			ResetCommand = new RelayCommand(ResetCommandMethod);
			FinishCommand = new RelayCommand(FinishCommandMethod);
		}

		#endregion

		#region command methods

		void ResetCommandMethod ()
		{
			//reset states to unhandled
			ApplyToVisitPurposeSelectionVMs(vp => vp.ResetState());

			IoC.ExecuteTripViewModel.UpdateStateOfDestination(DestinationVM.Destination, true);

			FrameHistory.CloseFrame();
		}

		void FinishCommandMethod ()
		{
			//set states based on their temporal values
			ApplyToVisitPurposeSelectionVMs(vp => vp.ApplyState());

			IoC.ExecuteTripViewModel.UpdateStateOfDestination(DestinationVM.Destination);

			FrameHistory.CloseFrame();

			//if there are further destinations to proceed with, re-open this frame
			IoC.ExecuteTripViewModel.ProceedWithNextDestination();
		}

		#endregion

		#region public methods

		public void SetValues (Trip trip, ExecuteTripDestinationVM destinationVM)
		{
			Trip = trip;
			DestinationVM = destinationVM;

			CalcTripDestinations();
		}

		#endregion

		#region private methods

		protected void CalcTripDestinations ()
		{
			if (Trip == null || DestinationVM == null)
				return;

			TripSenderVMs.Clear();

			DestinationVM.VisitPurposeSelectionVMs.Keys.ToList().ForEach(td =>
			{
				TripSenderVMs.Add(new ExecuteTripSenderListItemViewModel(td, DestinationVM.VisitPurposeSelectionVMs[td]));
			});
		}

		void ApplyToVisitPurposeSelectionVMs (Action<ExecuteTripVisitPurposeSelectionVM> method)
		{
			DestinationVM.VisitPurposeSelectionVMs.Values.ToList().ForEach(list =>
			{
				list.ForEach(vp => method(vp));
			});
		}

		#endregion
	}
}