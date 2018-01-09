using System;

namespace RunnerTool.Core
{
	/// <summary>
	/// Stores information about closed dates for a destination
	/// </summary>
	public class Vacation : BaseObservableContent
	{
		#region public properties

		/// <summary>
		/// The destination which is on vacation / has closed
		/// </summary>
		public Destination Destination { get; private set; }

		/// <summary>
		/// First day of vacation
		/// </summary>
		public DateTime BeginDate { get; private set; }

		/// <summary>
		/// Last day of vacation
		/// </summary>
		public DateTime EndDate { get; private set; }

		/// <summary>
		/// Whether this vacation is over
		/// </summary>
		public bool IsFinished { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Vacation (Destination destination, DateTime beginDate, DateTime endDate)
		{
			SetValues(destination, beginDate, endDate);

			ApplyToDestination();
		}

		#endregion

		#region public methods

		public void SetValues (Destination destination, DateTime beginDate, DateTime endDate)
		{
			Destination = destination;
			BeginDate = beginDate.Date;
			EndDate = endDate.Date;

			//swap dates in case of nonsensical begin and end
			if (BeginDate > EndDate)
			{
				DateTime temp = BeginDate;
				BeginDate = EndDate;
				EndDate = temp;
			}

			ApplyToDestination();

			//check whether vacation is over, based on dates
			CheckFinished();
		}

		/// <summary>
		/// Checks whether vacation is over and calls finish method in case
		/// </summary>
		public void CheckFinished ()
		{
			//end date has been surpassed
			if (EndDate < DateTime.UtcNow.Date)
				Finish();
		}

		/// <summary>
		/// Marks this vacation as over and removes date constraint from destination
		/// </summary>
		public void Finish ()
		{
			IsFinished = true;

			//remove constraint for this vacation from destination
			Destination.RemoveVacationConstraint(this);
		}

		#endregion

		#region private methods

		/// <summary>
		/// Adds a <see cref="VacationDateConstraint"/> to the destination if vacation is 
		/// not already over
		/// </summary>
		void ApplyToDestination ()
		{

			//TODO: adapt to changes: if constraint already in destination, do update instead of new entry

			//check if vacation is not already over
			if (DateTime.UtcNow.Date <= EndDate)
			{
				IsFinished = false;

				//add not available constraint to destination
				Destination.AddNotAvailableConstraint(new VacationDateConstraint(this));
			}
		}

		#endregion
	}
}