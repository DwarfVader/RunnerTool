using PropertyChanged;
using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// An extension to <see cref="TripDestination"/> which allows to combine two trip destinations 
	/// to one and further use combined visit purposes. 
	/// </summary>
	public class ExecuteTripTripDestinationVM : TripDestination
	{
		public TripDestination TripDestination { get; private set; }
		public TripDestination PreviouslyAbandonedTripDestination { get; private set; }

		public ExecuteTripTripDestinationVM (TripDestination tripDestination, TripDestination previouslyAbandonedTripDestination) :
			base(tripDestination.Trip, tripDestination.Sender, tripDestination.Destination)
		{
			TripDestination = tripDestination;
			PreviouslyAbandonedTripDestination = previouslyAbandonedTripDestination;

			//create combined visit purposes based on trip destinations
			TripDestination.VisitPurposes.ForEach(vp => AddCombinedVisitPurpose(vp));
			PreviouslyAbandonedTripDestination.VisitPurposes.ForEach(vp => AddCombinedVisitPurpose(vp));
		}

		void AddCombinedVisitPurpose (VisitPurpose visitPurpose)
		{
			//add new combined visit purpose to list; checking type should suffice
			if (!VisitPurposes.Any(vp => vp.Type == visitPurpose.Type))
			{
				VisitPurposes.Add(new ExecuteTripCombinedVisitPurpose(visitPurpose.Type));

				//keep visit purposes sorted
				VisitPurposes.Sort(Core.Comparators.VisitPurposeTypeCompare);
			}

			//retrieve combined visit purpose
			ExecuteTripCombinedVisitPurpose combinedVisitPurpose =
				VisitPurposes.First(vp => vp.Type == visitPurpose.Type) as ExecuteTripCombinedVisitPurpose;

			//add visit purpose to combined visit purpose
			combinedVisitPurpose.AddVisitPurpose(visitPurpose);
		}
	}

	/// <summary>
	/// An extension to <see cref="VisitPurpose"/> which allows to combine multiple visit purposes 
	/// to one without losing information about each single contributor. Also propagates state 
	/// changes down to each single visit purpose. 
	/// </summary>
	public class ExecuteTripCombinedVisitPurpose : VisitPurpose
	{
		VisitPurposeState state;

		/// <summary>
		/// A list of visit purposes which need to be combined, such that setting the state 
		/// gets propagated down to each visit purpose
		/// </summary>
		public List<VisitPurpose> VisitPurposes { get; private set; }

		public override VisitPurposeState State
		{
			get => state;
			set
			{
				state = value;

				//update state of each visit purpose that contributes to the combined value
				PropagateStateToVisitPurposes();
			}
		}

		public ExecuteTripCombinedVisitPurpose (VisitPurposeType type) : base(type, VisitPurposeState.Unhandled)
		{
			VisitPurposes = new List<VisitPurpose>();
		}

		public void AddVisitPurpose (VisitPurpose visitPurpose)
		{
			VisitPurposes.Add(visitPurpose);

			//assume that each visit purpose to be added has the same state
			State = visitPurpose.State;
		}

		void PropagateStateToVisitPurposes ()
		{
			//set states of individual visit purposes
			VisitPurposes.ForEach(vp => vp.State = State);
		}
	}

	public class ExecuteTripDestinationVM : BaseViewModel
	{
		public Destination Destination { get; private set; }
		public Dictionary<TripDestination, List<ExecuteTripVisitPurposeSelectionVM>> VisitPurposeSelectionVMs { get; private set; }

		public ExecuteTripDestinationVM (Destination destination)
		{
			Destination = destination;
			VisitPurposeSelectionVMs = new Dictionary<TripDestination, List<ExecuteTripVisitPurposeSelectionVM>>();
		}
	}

	public class ExecuteTripVisitPurposeSelectionVM : BaseViewModel
	{
		#region delegates

		/// <summary>
		/// The method to be called when the temporal abandoned value changes
		/// </summary>
		public Action OnIsAbandonedChangedHandler;

		#endregion

		#region private members

		VisitPurposeState stateTemp;

		#endregion

		#region public properties

		public TripDestination TripDestination { get; private set; }

		public VisitPurpose VisitPurpose { get; set; }
		public VisitPurposeState StateTemp
		{
			get => stateTemp;
			set
			{
				stateTemp = value;

				//invoke callbacks (if any)
				OnIsAbandonedChangedHandler?.Invoke();
			}
		}

		#endregion

		#region commands

		public ICommand ToggleStateCommand { get; private set; }

		#endregion

		#region constructor

		public ExecuteTripVisitPurposeSelectionVM (TripDestination tripDestination, VisitPurpose visitPurpose)
		{
			TripDestination = tripDestination;
			VisitPurpose = visitPurpose;
			StateTemp = VisitPurpose.State;

			//commands
			ToggleStateCommand = new RelayCommand(ToggleStateCommandMethod);
		}

		#endregion

		#region command methods

		/// <summary>
		/// Toggle the temporal state
		/// </summary>
		void ToggleStateCommandMethod ()
		{
			//finished trip getting re-viewed...
			if (TripDestination.Trip.IsFinished)
				//do not manipulate state
				return;

			switch (StateTemp)
			{
			case VisitPurposeState.Unhandled:
			case VisitPurposeState.Finished:
				StateTemp = VisitPurposeState.Abandoned;
				break;
			case VisitPurposeState.Abandoned:
				StateTemp = VisitPurposeState.Finished;
				break;
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Apply the temporal state to the actual state
		/// </summary>
		public void ApplyState ()
		{
			//mark unhandled states as finished ones
			if (StateTemp == VisitPurposeState.Unhandled)
				StateTemp = VisitPurposeState.Finished;

			SetVisitPurposeState(StateTemp);
		}

		/// <summary>
		/// Resets the temporal state to the actual state
		/// </summary>
		public void RestoreStateTemp ()
		{
			StateTemp = VisitPurpose.State;
		}

		/// <summary>
		/// Resets the actual state to be unhandled and update its temporal state
		/// </summary>
		public void ResetState ()
		{
			SetVisitPurposeState(VisitPurposeState.Unhandled);
			StateTemp = VisitPurpose.State;
		}

		/// <summary>
		/// Set the actual state to be abandoned and update its temporal state
		/// </summary>
		public void Abandon ()
		{
			SetVisitPurposeState(VisitPurposeState.Abandoned);
			StateTemp = VisitPurpose.State;
		}

		#endregion

		#region private methods

		void SetVisitPurposeState (VisitPurposeState state)
		{
			//do nothing if state does not change
			if (VisitPurpose.State == state)
				return;

			//check if needed to be removed from abandoned list in database
			if (VisitPurpose.State == VisitPurposeState.Abandoned)
				DB.Instance.RemoveFromAbandonedTripDestination(TripDestination.Sender, TripDestination.Destination, VisitPurpose.Type);

			VisitPurpose.State = state;

			//check if needed to be added to abandoned list in database
			if (VisitPurpose.State == VisitPurposeState.Abandoned)
				DB.Instance.AddToAbandonedTripDestination(TripDestination.Sender, TripDestination.Destination, VisitPurpose.Type);
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteTripViewModel : BaseVisibleViewModel
	{
		#region private members

		Trip trip;

		#endregion

		#region public properties

		public override bool IsVisible { get; set; }
		[DoNotCheckEquality]
		public Trip Trip
		{
			get => trip;
			set
			{
				trip = value;

				TripChanged();
			}
		}
		public Dictionary<Destination, ExecuteTripDestinationVM> DestinationVMMapping { get; private set; }
		public ObservableCollection<ExecuteTripDestinationListItemViewModel> DestinationListItemVMs { get; private set; }
		public string TitleText { get; private set; }

		/// <summary>
		/// Determines, whether there is a trip in execution at the moment. 
		/// Avoids setting another trip to be executed while the previous one has not been finished
		/// </summary>
		public bool IsExecuting { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExecuteTripViewModel ()
		{
			DestinationVMMapping = new Dictionary<Destination, ExecuteTripDestinationVM>();
			DestinationListItemVMs = new ObservableCollection<ExecuteTripDestinationListItemViewModel>();
			UpdateTitleText();

			//register events
			DB.Instance.OnVacationsChanged += RecalcAvailablity;
			Languages.LanguageChanged += UpdateTitleText;
		}

		~ExecuteTripViewModel ()
		{
			//unregister events
			DB.Instance.OnVacationsChanged -= RecalcAvailablity;
			Languages.LanguageChanged -= UpdateTitleText;
		}

		#endregion

		#region command methods

		protected override void CloseCommandMethod ()
		{
			base.CloseCommandMethod();

			//calculate the finished state of the trip as execute frame gets closed; finished cannot be re-executed
			CheckTripState();
		}

		#endregion

		#region public methods

		public void UpdateStateOfDestination (Destination destination, bool reset = false)
		{
			//retrieve destination view model
			var destinationVM = DestinationListItemVMs.First(d => d.Destination == destination);

			//check availability of destination for trip execution date
			if (!destination.IsAvailable(DateTime.UtcNow))
			{
				//set state in case of vacation changes during execution
				destinationVM.State = ExecuteTripDestinationState.NotAvailable;
				return;
			}

			//reset state of destination view model
			if (reset)
			{
				destinationVM.State = ExecuteTripDestinationState.Unhandled;

				//get all trip destinations to given destination
				DestinationVMMapping[destination].VisitPurposeSelectionVMs.Values.ToList().ForEach(list =>
				{
					list.ForEach(vp => vp.ResetState());
				});

				return;
			}

			int abandonedCount = 0;
			int unhandledCount = 0;
			int totalCount = 0;
			DestinationVMMapping[destination].VisitPurposeSelectionVMs.Values.ToList().ForEach(list =>
			{
				//number of abandoned visit purposes for this trip destination to the given destination
				abandonedCount += list.Count(vp => vp.VisitPurpose.State == VisitPurposeState.Abandoned);

				//number of unhandled visit purposes for this trip destination to the given destination
				unhandledCount += list.Count(vp => vp.VisitPurpose.State == VisitPurposeState.Unhandled);

				//total number of visit purposes
				totalCount += list.Count;
			});

			//update state based on abandoned count
			destinationVM.State = (unhandledCount != 0 ? ExecuteTripDestinationState.Unhandled :
				(abandonedCount == 0 ? ExecuteTripDestinationState.Completed : (abandonedCount == totalCount ?
				ExecuteTripDestinationState.Abandoned : ExecuteTripDestinationState.PartsCompleted)));
		}

		public void AbandonDestination (Destination destination)
		{
			//iterate over each list (of visit purposes) belonging to a trip destination to the destination
			DestinationVMMapping[destination].VisitPurposeSelectionVMs.Values.ToList().ForEach(list =>
			{
				//set each visit purpose of the list to abandoned
				list.ForEach(vp => vp.Abandon());
			});

			//set state of whole destination list item
			DestinationListItemVMs.First(d => d.Destination == destination).State = ExecuteTripDestinationState.Abandoned;
		}

		/// <summary>
		/// Chooses the next destination of the trip based on some order
		/// </summary>
		public void ProceedWithNextDestination ()
		{
			//get next destination list item based on order
			var destItem = DestinationListItemVMs.FirstOrDefault(d => d.State == ExecuteTripDestinationState.Unhandled);

			//if a destination was found...
			if (destItem != null)
			{
				//show GUI for destination's execution
				FrameHistory.OpenFrame(IoC.ExecuteTripDestinationViewModel);
				IoC.ExecuteTripDestinationViewModel.SetValues(Trip, DestinationVMMapping[destItem.Destination]);
			}
		}

		/// <summary>
		/// Checks, whether each destination list item has been handled and updates the state of the 
		/// trip accordingly
		/// </summary>
		public void CheckTripState ()
		{
			//no unhandled destinations left; finish trip
			if (!DestinationListItemVMs.Any(d => d.State == ExecuteTripDestinationState.Unhandled))
			{
				Trip.Finish();

				//trip execution done
				IsExecuting = false;
			}
		}

		#endregion

		#region private methods

		void TripChanged ()
		{
			//get rid of old content
			DestinationVMMapping.Clear();
			DestinationListItemVMs.Clear();

			//no trip, no cry
			if (Trip == null)
				return;

			//update title text TODO: get rid of that here?
			UpdateTitleText();

			//trip not in view mode
			if (!Trip.IsFinished)
				//trip execution begins
				IsExecuting = true;

			/* calculate list of distinct/combined trip destinations based on regular and previously abandoned ones and
			 * create visit purpose selection view models for each trip destination in order to be able to mark abandoned purposes */
			Trip.CalcCombinedTripDestinations().ForEach(td =>
			{
				//create a new list of visit purpose selections for the current trip destination
				List<ExecuteTripVisitPurposeSelectionVM> vms = new List<ExecuteTripVisitPurposeSelectionVM>();

				//iterate over visit purposes to populate list
				foreach (var vp in td.VisitPurposes)
				{
					//create a visit purpose selection view model for the current visit purpose
					vms.Add(new ExecuteTripVisitPurposeSelectionVM(td, vp));
				}

				//(create and) retrieve destination view model which stores visit purpose view models
				ExecuteTripDestinationVM destinationVM;
				if (!DestinationVMMapping.ContainsKey(td.Destination))
				{
					DestinationVMMapping.Add(td.Destination, new ExecuteTripDestinationVM(td.Destination));
				}
				destinationVM = DestinationVMMapping[td.Destination];

				//add trip destination / view model list mapping to destination view model
				destinationVM.VisitPurposeSelectionVMs.Add(td, vms);
			});

			//iterate over destinations
			DestinationVMMapping.Keys.ToList().ForEach(destination =>
			{
				//add new destination list item view models for each destination
				DestinationListItemVMs.Add(new ExecuteTripDestinationListItemViewModel(Trip, destination,
					DestinationVMMapping[destination].VisitPurposeSelectionVMs.Keys.Count));

				//calculate state of view model for given destination
				UpdateStateOfDestination(destination);
			});

			//order destinations by name
			DestinationListItemVMs = new ObservableCollection<ExecuteTripDestinationListItemViewModel>(
				DestinationListItemVMs.OrderByDescending(d => d.Destination.IsAvailable(DateTime.UtcNow)).
				ThenBy(d => d.Destination.Name));

			//begin execution with first destination 
			ProceedWithNextDestination();
		}

		void RecalcAvailablity ()
		{
			DestinationListItemVMs.ToList().ForEach(d =>
			{
				//availability changed to unavailable
				if (!d.Destination.IsAvailable(DateTime.UtcNow) && d.State != ExecuteTripDestinationState.NotAvailable)
				{
					d.State = ExecuteTripDestinationState.NotAvailable;

					//get all trip destinations to given destination
					DestinationVMMapping[d.Destination].VisitPurposeSelectionVMs.Values.ToList().ForEach(list =>
					{
						//reset each visit purpose's state
						list.ForEach(vp => vp.ResetState());
					});
				}
				//availability changed to available
				else if (d.Destination.IsAvailable(DateTime.UtcNow) && d.State == ExecuteTripDestinationState.NotAvailable)
				{
					d.State = ExecuteTripDestinationState.Unhandled;
				}
			});
		}

		void UpdateTitleText ()
		{
			TitleText = Languages.Get("executeTrip") + ": " + Trip?.Date.ToShortDateString();
		}

		#endregion
	}
}