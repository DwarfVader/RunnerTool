using RunnerTool.Core;
using System.ComponentModel;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewTripDestinationItemViewModel : BaseListItemViewModel
	{
		#region public properties

		public TripDestination TripDestination { get; private set; }

		public override string Name
		{
			get => (InnerIsSender ? TripDestination.Sender.Name : TripDestination.Destination.Name);
			protected set { }
		}
		public override string ShortName
		{
			get => (InnerIsSender ? TripDestination.Sender.ShortName : TripDestination.Destination.ShortName);
			protected set { }
		}
		public override Brush Background
		{
			get => (TripDestination.Destination.IsAvailable(TripDestination.Trip.Date) || InnerIsSender ? (InnerIsSender ?
					(Brush)(new SenderToBackgroundConverter().Convert(TripDestination.Sender, typeof(Brush), null, null)) :
					(Brush)(new DestinationTypeToBackgroundConverter().Convert(TripDestination.Destination, typeof(Brush), null, null))) :
					Brushes.Gray);
			protected set { }
		}

		/// <summary>
		/// A shortened version of the date of the trip
		/// </summary>
		public string TripDateShort => TripDestination.Trip.Date.ToShortDateString();

		public bool InnerIsSender { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ViewTripDestinationItemViewModel (TripDestination tripDestination, bool innerIsSender)
		{
			InnerIsSender = innerIsSender;
			TripDestination = tripDestination;
		}

		#endregion

		#region command methods

		protected override void LeftClickCommandMethod ()
		{
			IoC.TripDestinationViewModel.TripDestination = TripDestination;
		}

		#endregion
	}
}