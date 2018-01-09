using RunnerTool.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RunnerTool
{
	public enum ExecuteTripSenderState
	{
		Unhandled,
		PartsCompleted,
		Completed,
		Abandoned
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteTripSenderListItemViewModel : BaseViewModel
	{
		#region public properties

		public TripDestination TripDestination { get; private set; }

		/// <summary>
		/// Allows to mark visit purposes that have not been fulfilled
		/// </summary>
		public List<ExecuteTripVisitPurposeSelectionVM> VisitPurposeVMs { get; private set; }

		public ExecuteTripSenderState State { get; private set; } = ExecuteTripSenderState.Unhandled;

		#endregion

		#region commands

		public ICommand AbandonCommand { get; private set; }

		#endregion

		#region constructor

		public ExecuteTripSenderListItemViewModel (TripDestination tripDestination, List<ExecuteTripVisitPurposeSelectionVM> visitPurposeVMs)
		{
			TripDestination = tripDestination;
			VisitPurposeVMs = visitPurposeVMs;

			//iterate over corresponding visit purpose view models to set the callback method
			VisitPurposeVMs?.ForEach(vp => vp.OnIsAbandonedChangedHandler = CalcState);

			CalcState();

			//commands
			AbandonCommand = new RelayCommand(AbandonCommandMethod);
		}

		#endregion

		#region command methods

		void AbandonCommandMethod ()
		{
			//abandon each visit purpose
			VisitPurposeVMs.ForEach(vp => vp.Abandon());

			//set state of sender list item
			State = ExecuteTripSenderState.Abandoned;
		}

		#endregion

		#region private methods

		void CalcState ()
		{
			//TODO: can get rid of those null checks?
			if (VisitPurposeVMs == null)
				return;

			//get number of abandoned visit purposes
			int abandonedCount = VisitPurposeVMs.Count(vp => vp.StateTemp == VisitPurposeState.Abandoned);

			//get number of unhandled visit purposes
			int unhandledCount = VisitPurposeVMs.Count(vp => vp.StateTemp == VisitPurposeState.Unhandled);

			//set state of sender to this destination according to counts
			State = (unhandledCount != 0 && abandonedCount == 0 ? ExecuteTripSenderState.Unhandled : (abandonedCount == VisitPurposeVMs.Count ?
				ExecuteTripSenderState.Abandoned : (abandonedCount == 0 ?
				ExecuteTripSenderState.Completed : ExecuteTripSenderState.PartsCompleted)));
		}

		#endregion

		#region designer helper methods

		/// <summary>
		/// NOTE: Only used to make designer work
		/// </summary>
		public void Designer_SetVisitPurposeVMs ()
		{
			VisitPurposeVMs = new List<ExecuteTripVisitPurposeSelectionVM>
			{
				new ExecuteTripVisitPurposeSelectionVM(null, new VisitPurpose(VisitPurposeType.Recipe)),
				new ExecuteTripVisitPurposeSelectionVM(null, new VisitPurpose(VisitPurposeType.Sign)),
				new ExecuteTripVisitPurposeSelectionVM(null, new VisitPurpose(VisitPurposeType.Other)),
			};
		}

		#endregion
	}
}