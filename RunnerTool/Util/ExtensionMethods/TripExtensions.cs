using RunnerTool.Core;
using System.Collections.Generic;
using System.Linq;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public static class TripExtensions
	{
		public static List<TripDestination> CalcCombinedTripDestinations (this Trip trip)
		{
			//create list to hold distinct trip destinations
			List<TripDestination> tripDestinations = new List<TripDestination>();

			//iterate over regular trip destinations
			trip.TripDestinations.ToList().ForEach(curTD =>
			{
				//seek trip destination in abandoned list with same sender/destination combination as in current trip destination
				TripDestination td = trip.PreviouslyAbandonedTripDestinations.FirstOrDefault(aTD =>
					aTD.Sender == curTD.Sender && aTD.Destination == curTD.Destination);

				//if there is an previously abandoned trip destination...
				if (td != null)
					//create a combined trip destination 
					tripDestinations.Add(new ExecuteTripTripDestinationVM(curTD, td));
				else
					//simply insert current trip destination
					tripDestinations.Add(curTD);
			});

			//add abandoned trip destinations which are not yet in list / which are not contained in regular trip destinations
			tripDestinations.AddRange(trip.PreviouslyAbandonedTripDestinations.Where(aTD => !tripDestinations.Any(td =>
				td.Sender == aTD.Sender && td.Destination == aTD.Destination)));

			return tripDestinations;
		}
	}
}