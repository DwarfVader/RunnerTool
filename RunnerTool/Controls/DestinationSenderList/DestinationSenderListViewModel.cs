using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class DestinationSenderListViewModel : BaseViewModel
	{
		#region public properties

		/// <summary>
		/// The model
		/// </summary>
		public Trip Trip { get; private set; }

		/// <summary>
		/// A destination of the trip
		/// </summary>
		public Destination Destination { get; private set; }

		/// <summary>
		/// The trip destinations for the given destination of the trip
		/// </summary>
		public ObservableCollection<ViewTripDestinationItemViewModel> TripDestinationVMs { get; set; }

		public Brush Background => Destination.IsAvailable(Trip.Date) ?
			(Brush)new DestinationTypeToBackgroundConverter().Convert(Destination, typeof(Brush), null, null) : Brushes.Gray;

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public DestinationSenderListViewModel (Trip trip, Destination destination)
		{
			TripDestinationVMs = new ObservableCollection<ViewTripDestinationItemViewModel>();

			SetValues(trip, destination);
		}

		#endregion

		#region public methods

		public void SetValues (Trip trip, Destination destination)
		{
			Trip = trip;
			Destination = destination;

			//filter senders (trip destinations) for the given trip/destination combination
			TripDestinationVMs.Clear();
			Trip.CalcCombinedTripDestinations().Where(td => td.Destination.ShortName.Equals(Destination.ShortName)).
					GroupBy(td => td.Sender).Select(g => g.First()).
					OrderBy(td => DB.Instance.Senders.IndexOf(td.Sender)).ToList().ForEach(
					td => TripDestinationVMs.Add(new ViewTripDestinationItemViewModel(td, true)));
		}

		#endregion
	}
}