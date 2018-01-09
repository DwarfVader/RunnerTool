using RunnerTool.Core;
using System.Collections.Generic;

namespace RunnerTool
{
	/// <summary>
	/// A class that provides design time models for several view models
	/// </summary>
	public static class DesignModels
	{
		public static CreateDestinationViewModel CreateDestinationDesignModel = new CreateDestinationViewModel
		{
			IsVisible = true,
			CurDestination = DB.Instance.Destinations[0]
		};

		public static CreateSenderViewModel CreateSenderDesignModel = new CreateSenderViewModel
		{
			IsVisible = true,
			CurSender = DB.Instance.Senders[0]
		};

		public static CreateTripViewModel CreateTripDesignModel = new CreateTripViewModel
		{
			IsVisible = true,
			Trip = null
		};

		public static CreateVacationViewModel CreateVacationDesignModel = new CreateVacationViewModel
		{
			IsVisible = true,
			CurVacation = DB.Instance.Vacations[0]
		};

		public static DestinationSelectionViewModel DestinationSelectionDesignModel = new DestinationSelectionViewModel
		{
			IsVisible = true
		};

		public static DestinationSenderListViewModel DestinationSenderListDesignModel = new DestinationSenderListViewModel(
			DB.Instance.Trips[0], DB.Instance.Trips[0].TripDestinations[0].Destination);

		/// <summary>
		/// As base input view model is abstract, use a <see cref="StringInputViewModel"/> as design model
		/// </summary>
		public static BaseInputViewModel BaseInputDesignModel = new StringInputViewModel("Name");

		/// <summary>
		/// As base list view model is abstract, use a <see cref="DestinationListItemViewModel"/> as design model
		/// </summary>
		public static BaseListItemViewModel ListItemDesignModel = new DestinationListItemViewModel(DB.Instance.Destinations[0]);

		public static SenderDestinationListViewModel SenderDestinationListDesignModel = new SenderDestinationListViewModel(
			DB.Instance.Trips[0], DB.Instance.Trips[0].TripDestinations[0].Sender);

		public static SenderSelectionViewModel SenderSelectionDesignModel = new SenderSelectionViewModel
		{
			IsVisible = true
		};

		public static TripDestinationViewModel TripDestinationDesignModel = new TripDestinationViewModel
		{
			TripDestination = DB.Instance.Trips[0].TripDestinations[0]
		};

		public static TripListViewModel TripListDesignModel = new TripListViewModel
		{
			IsVisible = true
		};

		public static TripListItemViewModel TripListItemDesignModel = new TripListItemViewModel(DB.Instance.Trips[0]);

		public static VacationListItemViewModel VacationListItemDesignModel = new VacationListItemViewModel(DB.Instance.Vacations[0]);

		public static ViewTripViewModel ViewTripDesignModel = new ViewTripViewModel
		{
			IsVisible = true,
			Trip = DB.Instance.Trips[0]
		};

		public static VisitPurposeSelectionViewModel VisitPurposeSelectionDesignModel = new VisitPurposeSelectionViewModel(
			null, DB.Instance.DestinationVisitPurposeMapping[DestinationType.Shop][0]);

		public static VisitPurposeSelectionListViewModel VisitPurposeSelectionListDesignModel = new VisitPurposeSelectionListViewModel
		{
			IsVisible = true,
			Model = new DestinationVisitPurposeSelection(DB.Instance.Destinations[0])
		};

		public static ExecuteTripViewModel ExecuteTripDesignModel = new ExecuteTripViewModel
		{
			IsVisible = true,
			Trip = DB.Instance.Trips[0]
		};

		public static ExecuteTripDestinationViewModel ExecuteTripDestinationDesignModel = new ExecuteTripDestinationViewModel
		{
			IsVisible = true
		};

		public static ExecuteTripDestinationListItemViewModel ExecuteTripDestinationListItemDesignModel =
			new ExecuteTripDestinationListItemViewModel(DB.Instance.Trips[0], DB.Instance.Destinations[0], 3);

		public static ExecuteTripSenderListItemViewModel ExecuteTripSenderListItemDesignModel =
			new ExecuteTripSenderListItemViewModel(DB.Instance.Trips[0].TripDestinations[0],
				new List<ExecuteTripVisitPurposeSelectionVM>
			{
				new ExecuteTripVisitPurposeSelectionVM(DB.Instance.Trips[0].TripDestinations[0], DB.Instance.Trips[0].TripDestinations[0].VisitPurposes[0]),
				new ExecuteTripVisitPurposeSelectionVM(DB.Instance.Trips[0].TripDestinations[0], DB.Instance.Trips[0].TripDestinations[0].VisitPurposes[1]),
			});

		static DesignModels ()
		{
			//initialize stuff that didn't work via constructor
			var destinationVMs = new ExecuteTripDestinationVM(DB.Instance.Destinations[0]);
			destinationVMs.VisitPurposeSelectionVMs.Add(DB.Instance.Trips[0].TripDestinations[0], new List<ExecuteTripVisitPurposeSelectionVM>
			{
				new ExecuteTripVisitPurposeSelectionVM(DB.Instance.Trips[0].TripDestinations[0], new VisitPurpose(VisitPurposeType.Recipe)),
				new ExecuteTripVisitPurposeSelectionVM(DB.Instance.Trips[0].TripDestinations[0], new VisitPurpose(VisitPurposeType.Sign)),
			});
			ExecuteTripDestinationDesignModel.SetValues(DB.Instance.Trips[0], destinationVMs);

			ExecuteTripSenderListItemDesignModel.Designer_SetVisitPurposeVMs();
		}
	}
}