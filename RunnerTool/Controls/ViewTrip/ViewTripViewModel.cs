using PropertyChanged;
using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewTripViewModel : BaseVisibleViewModel
	{
		#region private members

		Trip trip;

		#endregion

		#region public properties

		/// <summary>
		/// The model to this view model
		/// </summary>
		[DoNotCheckEquality]
		public Trip Trip
		{
			get => trip;
			set
			{
				trip = value;

				RecalcLists();
			}
		}

		/// <summary>
		/// A shortened date string
		/// </summary>
		public string TitleText => Languages.Get("viewTrip") + ": " + Trip?.Date.ToShortDateString();

		/// <summary>
		/// The destinations of the trip
		/// </summary>
		public ObservableCollection<DestinationSenderListViewModel> DestinationVMs { get; private set; }

		/// <summary>
		/// The senders of the trip
		/// </summary>
		public ObservableCollection<SenderDestinationListViewModel> SenderVMs { get; private set; }

		/// <summary>
		/// Specifies, whether the view trip destination control is visible
		/// </summary>
		public bool IsViewTripDestinationControlVisible { get; private set; }

		/// <summary>
		/// Specifies, whether the view trip sender control is visible
		/// </summary>
		public bool IsViewTripSenderControlVisible { get; private set; } = true;

		public bool IsOverlayVisible { get; private set; }

		public override bool IsVisible { get; set; } = false;

		/// <summary>
		/// Can only edit a trip if it is not finished yet and not currently being executed
		/// </summary>
		public bool IsEditButtonVisible => Trip != null && !Trip.IsFinished && IoC.ExecuteTripViewModel.Trip != Trip;

		#endregion

		#region commands

		public ICommand GroupByDestinationsCommand { get; private set; }
		public ICommand GroupBySendersCommand { get; private set; }
		public ICommand EditTripCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ViewTripViewModel ()
		{
			DestinationVMs = new ObservableCollection<DestinationSenderListViewModel>();
			SenderVMs = new ObservableCollection<SenderDestinationListViewModel>();

			//commands
			GroupByDestinationsCommand = new RelayCommand(GroupByDestinationsCommandMethod);
			GroupBySendersCommand = new RelayCommand(GroupBySendersCommandMethod);
			EditTripCommand = new RelayCommand(EditTripCommandMethod);

			//as the trip updates, this view model's event fires by the update chain and recalcs
			PropertyChanged += (s, e) => RecalcLists();
		}

		#endregion

		#region command methods

		void GroupByDestinationsCommandMethod ()
		{
			IsViewTripDestinationControlVisible = true;
			IsViewTripSenderControlVisible = false;
		}

		void GroupBySendersCommandMethod ()
		{
			IsViewTripSenderControlVisible = true;
			IsViewTripDestinationControlVisible = false;
		}

		void EditTripCommandMethod ()
		{
			IoC.CreateTripViewModel.Trip = Trip;
			FrameHistory.OpenFrame(IoC.CreateTripViewModel);
		}

		#endregion

		#region public methods

		public void RecalcLists ()
		{
			if (Trip == null)
				return;

			/* TODO: is it worth the effort to update lists (and their inner lists) by their changes? 
			 * or just keep creating new lists on each change? */

			//calculate combined trip destinations based on regular and previously abandoned ones
			List<TripDestination> tripDestinations = Trip.CalcCombinedTripDestinations();

			//calc destination list
			DestinationVMs.Clear();
			tripDestinations.GroupBy(td => td.Destination.ShortName).Select(g => g.First()).
					OrderByDescending(td => td.Destination.IsAvailable(td.Trip.Date)).
					ThenBy(td => DB.Instance.Destinations.IndexOf(td.Destination)).ToList().ForEach(
					td => DestinationVMs.Add(new DestinationSenderListViewModel(Trip, td.Destination)));

			//calc sender list
			SenderVMs.Clear();
			tripDestinations.GroupBy(td => td.Sender).Select(g => g.First()).
					OrderBy(td => DB.Instance.Senders.IndexOf(td.Sender)).ToList().ForEach(
					td => SenderVMs.Add(new SenderDestinationListViewModel(Trip, td.Sender)));
		}

		public void RecalcOverlayVisibility ()
		{
			//returns a combined visibility value for additional controls
			IsOverlayVisible = IoC.TripDestinationViewModel.IsVisible;
		}

		#endregion

		#region private methods

		void TripChangedCallback (object s, PropertyChangedEventArgs e)
		{
			RecalcLists();
		}

		#endregion
	}
}