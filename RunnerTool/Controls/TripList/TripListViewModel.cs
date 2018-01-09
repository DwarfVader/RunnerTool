using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using PropertyChanged;

namespace RunnerTool
{
	/// <summary>
	/// The mode defining how filter criterions are going to be applied
	/// </summary>
	public enum TripFilterMode
	{
		/// <summary>
		/// One criterion has to be fulfilled by the overall trip
		/// </summary>
		Union,
		/// <summary>
		/// All criterions have to be fulfilled by the overall trip
		/// </summary>
		Intersection,
		/// <summary>
		/// All criterions have to be fulfilled by one trip destination
		/// </summary>
		InnerIntersection
	}

	/// <summary>
	/// 
	/// </summary>
	public class TripListViewModel : BaseVisibleViewModel
	{
		#region private members

		bool isVisible;
		Sender filterSender;
		Destination filterDestination;
		TripFilterMode filterMode;

		#endregion

		#region public properties

		public ObservableCollection<TripListItemViewModel> TripVMs { get; private set; }
		public ObservableCollection<TripListItemViewModel> FilteredTripVMs { get; private set; }

		public override bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				if (isVisible)
				{
					//TODO: better?
					InitTripVMs();
				}
			}
		}

		public bool IsOverlayVisible { get; private set; }

		/// <summary>
		/// Sender, for which trips are going to be filtered
		/// </summary>
		[DoNotCheckEquality]
		public Sender FilterSender
		{
			get => filterSender;
			set
			{
				filterSender = value;

				RecalcFilteredTrips();
			}
		}

		/// <summary>
		/// Destination, for which trips are going to be filtered
		/// </summary>
		[DoNotCheckEquality]
		public Destination FilterDestination
		{
			get => filterDestination;
			set
			{
				filterDestination = value;

				RecalcFilteredTrips();
			}
		}

		/// <summary>
		/// The filter mode defining how filter criterions shall be applied
		/// </summary>
		public TripFilterMode FilterMode
		{
			get => filterMode;
			set
			{
				filterMode = value;

				RecalcFilteredTrips();
			}
		}

		#endregion

		#region commands

		public ICommand ShowSenderSelectionCommand { get; private set; }
		public ICommand ShowDestinationSelectionCommand { get; private set; }
		public ICommand ResetSenderSelectionCommand { get; private set; }
		public ICommand ResetDestinationSelectionCommand { get; private set; }
		public ICommand SwitchFilterTypeCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public TripListViewModel ()
		{
			TripVMs = new ObservableCollection<TripListItemViewModel>();
			FilteredTripVMs = new ObservableCollection<TripListItemViewModel>();
			InitTripVMs();
			FilterMode = TripFilterMode.Union;

			//commands
			ShowSenderSelectionCommand = new RelayCommand(ShowSenderSelectionCommandMethod);
			ShowDestinationSelectionCommand = new RelayCommand(ShowDestinationSelectionCommandMethod);
			ResetSenderSelectionCommand = new RelayCommand(ResetSenderSelectionCommandMethod);
			ResetDestinationSelectionCommand = new RelayCommand(ResetDestinationSelectionCommandMethod);
			SwitchFilterTypeCommand = new RelayCommand(SwitchFilterTypeCommandMethod);
		}

		#endregion

		#region commands methods

		protected override void CloseCommandMethod ()
		{
			if (!IsOverlayVisible)
				FrameHistory.CloseFrame();
		}

		void ShowSenderSelectionCommandMethod ()
		{
			//set callback method, such that clicking on an item sets sender here
			IoC.SenderSelectionViewModel.ItemLeftClickCallback = ((sender) => FilterSender = sender);
			IoC.SenderSelectionViewModel.IsVisible = true;
		}

		void ShowDestinationSelectionCommandMethod ()
		{
			//set callback method, such that clicking on an item sets destination here
			IoC.DestinationSelectionViewModel.ItemLeftClickCallback = ((destination) => FilterDestination = destination);
			IoC.DestinationSelectionViewModel.IsVisible = true;
		}

		void ResetSenderSelectionCommandMethod ()
		{
			FilterSender = null;
		}

		void ResetDestinationSelectionCommandMethod ()
		{
			FilterDestination = null;
		}

		void SwitchFilterTypeCommandMethod ()
		{
			//switch to next value
			FilterMode = EnumHelpers.NextValue(FilterMode);

			//TODO: color lerp to indicate wildcard if nothing is selected for filtering
			//TODO: rethink behavior: begin editing trip, then clicking create trip => cancel edit? 
			//		keep on editing? disable title buttons?
		}

		#endregion

		#region public methods

		public void RecalcOverlayVisibility ()
		{
			//returns a combined visibility value for additional controls
			IsOverlayVisible = IoC.SenderSelectionViewModel.IsVisible || IoC.DestinationSelectionViewModel.IsVisible;
		}

		#endregion

		#region private methods

		void InitTripVMs ()
		{
			//create list of view models for each trip
			TripVMs.Clear();
			foreach (Trip trip in DB.Instance.Trips)
			{
				TripVMs.Add(new TripListItemViewModel(trip));
			}

			//initalize filtered list with all entries
			FilteredTripVMs.Clear();
			foreach (TripListItemViewModel item in TripVMs)
			{
				FilteredTripVMs.Add(item);
			}

			RecalcFilteredTrips();
		}

		void RecalcFilteredTrips ()
		{
			//get rid of previous items
			FilteredTripVMs.Clear();

			//loop over each trip...
			foreach (TripListItemViewModel tripVM in TripVMs)
			{
				//filter trips according to filter type based on sender/destination criterions
				switch (FilterMode)
				{
				case TripFilterMode.Union:
					//add trip if it contains trip destinations fulfilling at least one constraint
					if ((FilterSender == null || tripVM.Trip.TripDestinations.Any(td => td.Sender == FilterSender)) ||
						(FilterDestination == null || tripVM.Trip.TripDestinations.Any(td => td.Destination.Name.Equals(FilterDestination.Name))))
					{
						//add filtered trip view model to list
						FilteredTripVMs.Add(tripVM);
					}
					break;
				case TripFilterMode.Intersection:
					//add trip if all constraints are fulfilled by (multiple) trip destinations
					if ((FilterSender == null || tripVM.Trip.TripDestinations.Any(td => td.Sender == FilterSender)) &&
						(FilterDestination == null || tripVM.Trip.TripDestinations.Any(td => td.Destination.Name.Equals(FilterDestination.Name))))
					{
						//add filtered trip view model to list
						FilteredTripVMs.Add(tripVM);
					}
					break;
				case TripFilterMode.InnerIntersection:
					//add trip if all constraints are fulfilled by the same trip destination
					if (tripVM.Trip.TripDestinations.Any(td => (FilterSender == null || td.Sender == FilterSender) &&
						(FilterDestination == null || td.Destination.Name.Equals(FilterDestination.Name))))
					{
						//add filtered trip view model to list
						FilteredTripVMs.Add(tripVM);
					}
					break;
				}
			}
		}

		#endregion
	}
}