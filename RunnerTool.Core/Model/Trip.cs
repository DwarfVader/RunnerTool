using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// Contains information about a specific trip
	/// </summary>
	public class Trip : BaseObservableContent
	{
		#region private members

		DateTime date;

		#endregion

		#region public properties

		/// <summary>
		/// The date on which this trip took place
		/// </summary>
		public DateTime Date
		{
			get => date;
			set
			{
				date = value.Date;
			}
		}

		/// <summary>
		/// Information about the destinations and visit purposes of this trip
		/// </summary>
		public ObservableCollection<TripDestination> TripDestinations { get; private set; }

		/// <summary>
		/// Trip destinations which have been abandoned in previous trips. Those get 
		/// added automatically and cannot be modified in edit mode. Those trip destinations/
		/// their visit purposes are only visible in trip execution as a combined value of 
		/// <see cref="TripDestinations"/> and this list. 
		/// </summary>
		public List<TripDestination> PreviouslyAbandonedTripDestinations { get; private set; }

		/// <summary>
		/// Determines, whether the creation of this trip has been completed. 
		/// Previously abandoned trip destinations are going to be added as soon as this flag 
		/// gets true. 
		/// </summary>
		public bool IsCreated { get; private set; }

		/// <summary>
		/// Has this trip been finished? 
		/// </summary>
		public bool IsFinished { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Trip ()
		{
			Date = DateTime.UtcNow.Date; //dummy date
			TripDestinations = new ObservableCollection<TripDestination>();
			TripDestinations.CollectionChanged += (s, e) => OnPropertyChanged("TripDestinations");
			PreviouslyAbandonedTripDestinations = new List<TripDestination>();
			IsFinished = false;
		}

		#endregion

		#region public methods

		public void AddTripDestination (TripDestination tripDestination)
		{
			TripDestinations.Add(tripDestination);
		}

		public void AddPreviouslyAbandonedTripDestination (TripDestination tripDestination)
		{
			if (!IsCreated)
				throw new Exception("IsCreated flag has not been set yet!");

			tripDestination.Trip = this;

			PreviouslyAbandonedTripDestinations.Add(tripDestination);
		}

		public void Finish ()
		{
			IsFinished = true;

			//add trips to unavailable destinations to abandoned list in database
			TripDestinations.Where(td => !td.Destination.IsAvailable(Date)).ToList().ForEach(td =>
			{
				DB.Instance.AddToAbandonedTripDestination(td);
			});

			//consider previously abandoned ones as well
			PreviouslyAbandonedTripDestinations.Where(td => !td.Destination.IsAvailable(Date)).ToList().ForEach(td =>
			{
				DB.Instance.AddToAbandonedTripDestination(td);
			});
		}

		/// <summary>
		/// Marks trip as created and adds previously abandoned trip destinations from database
		/// </summary>
		public void FinishCreation ()
		{
			//mark trip creation as finished
			IsCreated = true;

			/*add previously abandoned trip destinations; those not available at trip's date will be blind entries 
			 * just to display that something is pending */
			DB.Instance.AbandonedTripDestinations.ForEach(td => AddPreviouslyAbandonedTripDestination(td));

			//remove those entries from abandoned list in database which are available at trip's date
			//DB.Instance.AbandonedTripDestinations.RemoveAll(td => td.Destination.IsAvailable(Date));

			//clear list of abandoned trip destinations
			DB.Instance.AbandonedTripDestinations.Clear();
		}

		#endregion
	}
}