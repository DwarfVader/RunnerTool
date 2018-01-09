using System.Collections.Generic;
using System.Linq;

namespace RunnerTool.Core
{
	public class VisitPurpose : BaseObservable
	{
		public VisitPurposeType Type { get; private set; }

		//need to keep this virtual as GUI will extend this class and override state
		public virtual VisitPurposeState State { get; set; }

		public VisitPurpose (VisitPurposeType type, VisitPurposeState state = VisitPurposeState.Unhandled)
		{
			Type = type;
			State = state;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class TripDestination : BaseObservableContent
	{
		#region public properties

		/// <summary>
		/// The trip, this information belongs to
		/// </summary>
		public Trip Trip { get; set; }

		/// <summary>
		/// The sender, corresponding to the given visit purposes for the destination
		/// </summary>
		public Sender Sender { get; private set; }

		/// <summary>
		/// The destination of the trip
		/// </summary>
		public Destination Destination { get; private set; }

		/// <summary>
		/// The visit purposes to the destination for the trip
		/// </summary>
		public List<VisitPurpose> VisitPurposes { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public TripDestination (Trip trip, Sender sender, Destination destination, List<VisitPurposeSelection> visitPurposeSelections = null)
		{
			Trip = trip;
			Sender = sender;
			Destination = destination;

			VisitPurposes = new List<VisitPurpose>();
			if (visitPurposeSelections != null)
			{
				AddVisitPurposes(visitPurposeSelections);
			}
		}

		#endregion

		#region public methods

		public void AddVisitPurpose (VisitPurposeType type, VisitPurposeState state)
		{
			VisitPurposes.Add(new VisitPurpose(type, state));
		}
		public void AddVisitPurposes (List<VisitPurposeSelection> visitPurposeSelections)
		{
			visitPurposeSelections.Where(vp => vp.IsSelected).ToList().ForEach(vp => AddVisitPurpose(vp.Type, vp.State));
		}

		#endregion
	}
}