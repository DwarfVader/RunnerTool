using System.Collections.Generic;
using System.Collections.ObjectModel;
using RunnerTool.Core;
using System.Windows.Input;
using PropertyChanged;
using System.Linq;
using System.ComponentModel;
using System;

namespace RunnerTool
{
	public class SenderDestinationSelection : BaseObservableContent
	{
		public Sender Sender { get; private set; }

		public List<DestinationVisitPurposeSelection> Destinations { get; private set; }
		public ObservableCollection<DestinationVisitPurposeSelectionListItemViewModel> FilteredDestinationVMs { get; private set; }

		public List<DestinationVisitPurposeSelection> SelectedDestinations { get; private set; }
		public ObservableCollection<DestinationVisitPurposeSelectionListItemViewModel> FilteredSelectedDestinationVMs { get; private set; }

		public SenderDestinationSelection (Sender sender)
		{
			Sender = sender;

			Destinations = new List<DestinationVisitPurposeSelection>();
			FilteredDestinationVMs = new ObservableCollection<DestinationVisitPurposeSelectionListItemViewModel>();
			InitDestinations();

			SelectedDestinations = new List<DestinationVisitPurposeSelection>();
			FilteredSelectedDestinationVMs = new ObservableCollection<DestinationVisitPurposeSelectionListItemViewModel>();
		}

		void InitDestinations ()
		{
			foreach (var destination in DB.Instance.Destinations)
			{
				Destinations.Add(new DestinationVisitPurposeSelection(destination));
			}
		}

		/// <summary>
		/// Check for changes in the database
		/// </summary>
		public void UpdateDestinations ()
		{
			//store currently selected destinations
			List<Destination> selected = SelectedDestinations.Select(s => s.Destination).ToList();

			//get rid of old entries
			Destinations.Clear();
			SelectedDestinations.Clear();

			//re-initialize destination list
			InitDestinations();

			//restore previous selections
			Destinations.Where(d => selected.Contains(d.Destination)).ToList().ForEach(d => SelectDestination(d));

			//TODO: improve this messy updating approach...
			//update filtered list
			IoC.CreateTripViewModel.RecalcFilteredDestinations();

			//TODO: currently, deleting destinations from DB is not planned
		}

		public void SelectDestination (DestinationVisitPurposeSelection destination)
		{
			//move destination from/to list
			SelectedDestinations.Add(destination);
			Destinations.Remove(destination);

			//re-sort lists
			SortDestinationList(Destinations);
			SortDestinationList(SelectedDestinations);
		}

		public void UnselectDestination (DestinationVisitPurposeSelection destination)
		{
			//move destination from/to list
			Destinations.Add(destination);
			SelectedDestinations.Remove(destination);

			//re-sort lists
			SortDestinationList(Destinations);
			SortDestinationList(SelectedDestinations);
		}

		void SortDestinationList (List<DestinationVisitPurposeSelection> list)
		{
			list.Sort(Comparators.DestinationVisitSelectionCompare);
		}
	}

	public class DestinationVisitPurposeSelection : BaseObservableContent
	{
		public Destination Destination { get; private set; }
		public List<VisitPurposeSelection> VisitPurposes { get; private set; }

		public DestinationVisitPurposeSelection (Destination destination)
		{
			Destination = destination;

			//initialize visit purposes with template values
			VisitPurposes = new List<VisitPurposeSelection>();
			foreach (var visitPurpose in destination.VisitPurposeSelectionTemplates)
			{
				VisitPurposes.Add(new VisitPurposeSelection(visitPurpose));
			}
		}
	}

	/// <summary>
	/// The view model for the list of destinations to create a new trip
	/// </summary>
	public class CreateTripViewModel : BaseVisibleViewModel
	{
		#region private members

		Trip trip;
		Sender curSender;
		string searchText;

		#endregion

		#region public properties

		/// <summary>
		/// The trip that is treated: If null, a new trip will be created, otherwise, 
		/// given trip will be edited. 
		/// </summary>
		[DoNotCheckEquality]
		public Trip Trip
		{
			get => trip;
			set
			{
				trip = value;

				if (trip == null)
				{
					TitleText = "createTrip";
				}
				else
				{
					SetupGUI();
				}
			}
		}

		public Dictionary<Sender, SenderDestinationSelection> SenderDestinationSelections { get; private set; }
		public SenderDestinationSelection CurSenderDestinationSelection => GetCurSenderDestinationSelection();

		/// <summary>
		/// Title text, depending on whether a trip is set or not.
		/// </summary>
		public string TitleText { get; private set; }

		/// <summary>
		/// Search text to filter destinations
		/// </summary>
		public string SearchText
		{
			get => searchText;
			set
			{
				searchText = value;
				RecalcFilteredDestinations();
			}
		}

		/// <summary>
		/// The sender/client which needs something from a destination
		/// </summary>
		[DoNotCheckEquality]
		public Sender CurSender
		{
			get => curSender;
			set
			{
				curSender = value;
				CurSenderChanged(curSender);
				RecalcFilteredDestinations();
			}
		}

		public bool IsCreateButtonVisible => Trip == null;

		public bool IsFinishEditButtonVisible => !IsCreateButtonVisible;

		/// <summary>
		/// Determines, whether the overlay panel is visible
		/// </summary>
		public bool IsOverlayVisible { get; private set; }

		public override bool IsVisible { get; set; }

		public DatePickerInputViewModel TripDateVM { get; private set; }

		#endregion

		#region commands

		public ICommand CloseAndResetCommand { get; private set; }
		public ICommand CreateTripCommand { get; private set; }
		public ICommand FinishEditCommand { get; private set; }
		public ICommand ShowSenderSelectionCommand { get; private set; }

		#endregion

		#region constructor

		public CreateTripViewModel ()
		{
			Trip = null;

			SenderDestinationSelections = new Dictionary<Sender, SenderDestinationSelection>();

			//default to first entry
			CurSender = DB.Instance.Senders[0];
			//TODO: improvable?
			/* Need to explicitly call this method as it is not getting called from within 
			 * the setter property. Because of Fody, setters are not executed if the value 
			 * does not change. Therefore, when (initially) setting a property to its default 
			 * value, methods from within the setter have to be called manually. */
			CurSenderChanged(CurSender);

			SearchText = "";
			TripDateVM = new DatePickerInputViewModel("Trip date");

			//commands
			CloseAndResetCommand = new RelayCommand(CloseAndResetCommandMethod);
			CreateTripCommand = new RelayCommand(CreateTripCommandMethod);
			FinishEditCommand = new RelayCommand(FinishEditCommandMethod);
			ShowSenderSelectionCommand = new RelayCommand(ShowSenderSelectionCommandMethod);

			//register events
			DB.Instance.OnDestinationsChanged += DestinationsChangedCallback;
		}

		~CreateTripViewModel ()
		{
			//unregister events
			DB.Instance.OnDestinationsChanged -= DestinationsChangedCallback;
		}

		#endregion

		#region command methods

		protected override void CloseCommandMethod ()
		{
			if (!IsOverlayVisible)
			{
				//edit mode: closing also resets input
				if (Trip != null)
					ResetInput();

				FrameHistory.CloseFrame();
			}
		}

		void CloseAndResetCommandMethod ()
		{
			//reset values
			ResetInput();

			//close frame
			CloseCommandMethod();
		}

		void CreateTripCommandMethod ()
		{
			Trip trip = new Trip();

			//iterate over sender destination selections of all sender types
			foreach (var sds in SenderDestinationSelections.Values)
			{
				//add trip information for each 
				foreach (var dest in sds.SelectedDestinations)
				{
					trip.AddTripDestination(new TripDestination(trip, sds.Sender, dest.Destination, dest.VisitPurposes));
				}
			}

			//set trip date
			trip.Date = TripDateVM.Date;

			//there is no trip in execution at the moment
			if (!IoC.ExecuteTripViewModel.IsExecuting)
			{
				trip.FinishCreation();
			}

			//add trip to database
			DB.Instance.Trips.Add(trip);

			//reset and close frame as soon as input was handled
			CloseAndResetCommandMethod();
		}

		void FinishEditCommandMethod ()
		{
			//reset previous trip destinations
			Trip.TripDestinations.Clear();

			//iterate over sender destination selections of all sender types
			foreach (var sds in SenderDestinationSelections.Values)
			{
				//add trip information for each 
				foreach (var dest in sds.SelectedDestinations)
				{
					Trip.AddTripDestination(new TripDestination(Trip, sds.Sender, dest.Destination, dest.VisitPurposes));
				}
			}

			//set trip date
			Trip.Date = TripDateVM.Date;

			//trip creation has not finished yet and no trip in execution
			if (!IoC.ExecuteTripViewModel.IsExecuting && !Trip.IsCreated)
			{
				Trip.FinishCreation();
			}

			//close/reset this frame
			CloseAndResetCommandMethod();
		}

		void ShowSenderSelectionCommandMethod ()
		{
			//set callback method, such that clicking on an item sets sender here
			IoC.SenderSelectionViewModel.ItemLeftClickCallback = ((sender) => SelectSender(sender));
			IoC.SenderSelectionViewModel.IsVisible = true;
		}

		#endregion

		#region public methods

		public void SelectDestination (DestinationVisitPurposeSelection destination)
		{
			var sds = SenderDestinationSelections[CurSender];

			if (sds.Destinations.Contains(destination))
			{
				//move destination from unselected to selected list
				sds.SelectDestination(destination);

				RecalcFilteredDestinations();
			}
		}

		public void UnselectDestination (DestinationVisitPurposeSelection destination)
		{
			var sds = SenderDestinationSelections[CurSender];

			if (sds.SelectedDestinations.Contains(destination))
			{
				//move destination from selected to unselected list
				sds.UnselectDestination(destination);

				//update filtered lists as well
				RecalcFilteredDestinations();
			}
		}

		public void RecalcFilteredDestinations ()
		{
			CalcFilteredDestinations();
			CalcFilteredSelectedDestinations();
		}

		public void SelectSender (Sender sender)
		{
			CurSender = sender;
		}

		public void RecalcOverlayVisibility ()
		{
			//returns a combined visibility value for additional controls
			IsOverlayVisible = IoC.SenderSelectionViewModel.IsVisible || IoC.VisitPurposeSelectionListViewModel.IsVisible;
		}

		/// <summary>
		/// Resets all user inputs
		/// </summary>
		public void ResetInput ()
		{
			//reset trip (if any)
			Trip = null;

			//reset search text
			SearchText = string.Empty;

			//reset sender/destination selections
			SenderDestinationSelections.Clear();

			//reset date of trip
			TripDateVM.Date = DateTime.UtcNow.Date;

			//reset current sender type
			/* NOTE: Need to do this AFTER sender destination selections has been cleared,
			 * as setting sender type will update the cur selection in the GUI. */
			CurSender = DB.Instance.Senders[0];
		}

		#endregion

		#region private methods

		void CalcFilteredDestinations ()
		{
			var sds = SenderDestinationSelections[CurSender];

			//get rid of previous elements
			sds.FilteredDestinationVMs.Clear();

			//filter matching destinations
			foreach (var item in sds.Destinations)
			{
				//search text (if any) matches item
				if (string.IsNullOrEmpty(SearchText) ||
						item.Destination.Name.ToLower().Contains(SearchText.ToLower()) ||
						item.Destination.ShortName.ToLower().Contains(SearchText.ToLower()))
				{
					sds.FilteredDestinationVMs.Add(new DestinationVisitPurposeSelectionListItemViewModel(item));
				}
			}
		}

		void CalcFilteredSelectedDestinations ()
		{
			var sds = SenderDestinationSelections[CurSender];

			//get rid of previous elements
			sds.FilteredSelectedDestinationVMs.Clear();

			//filter matching destinations
			foreach (var item in sds.SelectedDestinations)
			{
				//search text matches item
				if (item.Destination.Name.ToLower().Contains(SearchText) ||
						item.Destination.ShortName.ToLower().Contains(SearchText))
				{
					sds.FilteredSelectedDestinationVMs.Add(new DestinationVisitPurposeSelectionListItemViewModel(item, true));
				}
			}
		}

		void CurSenderChanged (Sender senderType)
		{
			CreateSenderDestinationSelection(senderType);
		}

		void CreateSenderDestinationSelection (Sender senderType)
		{
			//if sender type is not contained in list...
			if (!SenderDestinationSelections.ContainsKey(senderType))
			{
				//add new entry for sender type to list
				SenderDestinationSelections.Add(senderType, new SenderDestinationSelection(senderType));
			}
		}

		SenderDestinationSelection GetCurSenderDestinationSelection ()
		{
			//if current sender type is not contained in list...
			if (!SenderDestinationSelections.ContainsKey(CurSender))
			{
				//add new entry for current sender type to list
				SenderDestinationSelections.Add(CurSender, new SenderDestinationSelection(CurSender));
			}

			//return destination selection list for current sender
			return SenderDestinationSelections[CurSender];
		}

		/// <summary>
		/// Set up the GUI with values coming from existing trip (if any) in edit mode.
		/// </summary>
		void SetupGUI ()
		{
			if (Trip == null)
				return;

			SenderDestinationSelections.Clear();

			//set title text
			TitleText = "Edit trip: " + Trip.Date.ToShortDateString();

			//initialize all sender destination selections
			foreach (var sender in DB.Instance.Senders)
			{
				//create entry with default values
				CreateSenderDestinationSelection(sender);
			}

			//iterate over all trip destinations
			foreach (var td in Trip.TripDestinations)
			{
				//sender of current trip destination
				var sender = td.Sender;

				//get sender destination selection for current sender
				var sds = SenderDestinationSelections[sender];

				//get visit purpose selection for given destination and sender
				var vps = sds.Destinations.First(v => v.Destination.Name.Equals(td.Destination.Name));

				//iterate over all visit purposes in visit purpose seletion...
				foreach (var vp in vps.VisitPurposes)
				{
					//and replace default selections with actual selection in trip destination
					//visit purposes occuring in trip destination need to be selected
					VisitPurpose curVP = td.VisitPurposes.FirstOrDefault(vp1 => vp1.Type == vp.Type);
					vp.IsSelected = curVP != null;
					vp.State = curVP != null ? curVP.State : VisitPurposeState.Unhandled;
				}

				//move visit purpose selection to selected destination list
				sds.Destinations.Remove(vps);
				sds.SelectedDestinations.Add(vps);
			}

			//set trip date
			TripDateVM.Date = Trip.Date;

			//reset current sender type and force GUI to update
			CurSender = DB.Instance.Senders[0];
		}

		void DestinationsChangedCallback ()
		{
			SenderDestinationSelections.Values.ToList().ForEach(sds => sds.UpdateDestinations());
		}

		#endregion
	}
}