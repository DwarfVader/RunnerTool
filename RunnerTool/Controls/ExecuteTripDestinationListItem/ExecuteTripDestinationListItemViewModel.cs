using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RunnerTool
{
	public enum ExecuteTripDestinationState
	{
		/// <summary>
		/// Destination is not available at this date. Entry still in list to show it is pending
		/// </summary>
		NotAvailable,
		/// <summary>
		/// Not visited yet
		/// </summary>
		Unhandled,
		/// <summary>
		/// Parts of the senders' visit purposes to this destination have been fulfilled
		/// </summary>
		PartsCompleted,
		/// <summary>
		/// All visit purposes to this destination have been fulfilled
		/// </summary>
		Completed,
		/// <summary>
		/// Destination was unexpectedly not available. All of the visit purposes are abandoned
		/// </summary>
		Abandoned
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteTripDestinationListItemViewModel : BaseViewModel
	{
		#region private members

		ExecuteTripDestinationState state = ExecuteTripDestinationState.Unhandled;

		#endregion

		#region public properties

		/// <summary>
		/// The current trip
		/// </summary>
		public Trip Trip { get; set; }

		/// <summary>
		/// The destination of interest
		/// </summary>
		public Destination Destination { get; set; }

		public string TitleText => Destination.Name + ", " + Trip.Date.ToShortDateString();

		public ExecuteTripDestinationState State
		{
			get => state;
			set
			{
				state = value;

				//IoC.ExecuteTripViewModel.CheckTripState();
			}
		}

		public int SenderCount { get; private set; }

		public bool IsAbandonButtonVisible => !Trip.IsFinished && State != ExecuteTripDestinationState.NotAvailable;

		#endregion

		#region commands

		public ICommand ShowExecuteDestinationViewCommand { get; private set; }
		public ICommand AbandonDestinationCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExecuteTripDestinationListItemViewModel (Trip trip, Destination destination, int senderCount)
		{
			Trip = trip;
			Destination = destination;
			SenderCount = senderCount;

			//determine availability of destination for current date
			if (!Destination.IsAvailable(DateTime.Now))
				State = ExecuteTripDestinationState.NotAvailable;

			//commands
			ShowExecuteDestinationViewCommand = new RelayCommand(ShowExecuteDestinationViewCommandMethod);
			AbandonDestinationCommand = new RelayCommand(AbandonDestinationCommandMethod);
		}

		#endregion

		#region command methods

		void ShowExecuteDestinationViewCommandMethod ()
		{
			//if destination is not available at this date, do nothing
			if (State == ExecuteTripDestinationState.NotAvailable)
				return;

			FrameHistory.OpenFrame(IoC.ExecuteTripDestinationViewModel);
			IoC.ExecuteTripDestinationViewModel.SetValues(Trip, IoC.ExecuteTripViewModel.DestinationVMMapping[Destination]);
		}

		void AbandonDestinationCommandMethod ()
		{
			//if destination is not available at this date, do nothing
			if (State == ExecuteTripDestinationState.NotAvailable)
				return;

			IoC.ExecuteTripViewModel.AbandonDestination(Destination);
		}

		#endregion
	}
}