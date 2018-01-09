using RunnerTool.Core;
using System.Collections.ObjectModel;
using System.Linq;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class SenderDestinationListViewModel : BaseViewModel
	{
		#region public properties

		/// <summary>
		/// The model
		/// </summary>
		public Trip Trip { get; private set; }

		/// <summary>
		/// The sender to which the trip destinations belong
		/// </summary>
		public Sender Sender { get; private set; }

		/// <summary>
		/// All trip destinations of a trip for the given sender
		/// </summary>
		public ObservableCollection<ViewTripDestinationItemViewModel> TripDestinationVMs { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SenderDestinationListViewModel (Trip trip, Sender sender)
		{
			TripDestinationVMs = new ObservableCollection<ViewTripDestinationItemViewModel>();

			SetValues(trip, sender);
		}

		#endregion

		#region public methods

		public void SetValues (Trip trip, Sender sender)
		{
			Trip = trip;
			Sender = sender;

			//filter destinations (trip destinations) for the given trip/sender combination
			TripDestinationVMs.Clear();
			Trip.CalcCombinedTripDestinations().Where(td => td.Sender == Sender).
					GroupBy(td => td.Destination.ShortName).Select(g => g.First()).
					OrderByDescending(td => td.Destination.IsAvailable(td.Trip.Date)).
					ThenBy(td => DB.Instance.Destinations.IndexOf(td.Destination)).ToList().ForEach(
					td => TripDestinationVMs.Add(new ViewTripDestinationItemViewModel(td, false)));
		}

		#endregion

	}
}