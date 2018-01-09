using Ninject;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public static class IoC
	{
		#region private properties

		/// <summary>
		/// The kernel of the IoC
		/// </summary>
		public static IKernel Kernel { get; private set; } = new StandardKernel();

		#endregion

		#region public properties

		public static VisitPurposeSelectionListViewModel VisitPurposeSelectionListViewModel => Get<VisitPurposeSelectionListViewModel>();
		public static CreateTripViewModel CreateTripViewModel => Get<CreateTripViewModel>();
		public static CreateDestinationViewModel CreateDestinationViewModel => Get<CreateDestinationViewModel>();
		public static CreateSenderViewModel CreateSenderViewModel => Get<CreateSenderViewModel>();
		public static CreateVacationViewModel CreateVacationViewModel => Get<CreateVacationViewModel>();
		public static SenderSelectionViewModel SenderSelectionViewModel => Get<SenderSelectionViewModel>();
		public static DestinationSelectionViewModel DestinationSelectionViewModel => Get<DestinationSelectionViewModel>();
		public static TripListViewModel TripListViewModel => Get<TripListViewModel>();
		public static ViewTripViewModel ViewTripViewModel => Get<ViewTripViewModel>();
		public static TripDestinationViewModel TripDestinationViewModel => Get<TripDestinationViewModel>();
		public static ExecuteTripViewModel ExecuteTripViewModel => Get<ExecuteTripViewModel>();
		public static ExecuteTripDestinationViewModel ExecuteTripDestinationViewModel => Get<ExecuteTripDestinationViewModel>();

		#endregion

		#region construction

		public static void Setup ()
		{
			BindViewModels();
		}

		private static void BindViewModels ()
		{
			Kernel.Bind<VisitPurposeSelectionListViewModel>().ToConstant(new VisitPurposeSelectionListViewModel());
			Kernel.Bind<CreateTripViewModel>().ToConstant(new CreateTripViewModel());
			Kernel.Bind<CreateDestinationViewModel>().ToConstant(new CreateDestinationViewModel());
			Kernel.Bind<CreateSenderViewModel>().ToConstant(new CreateSenderViewModel());
			Kernel.Bind<CreateVacationViewModel>().ToConstant(new CreateVacationViewModel());
			Kernel.Bind<SenderSelectionViewModel>().ToConstant(new SenderSelectionViewModel());
			Kernel.Bind<DestinationSelectionViewModel>().ToConstant(new DestinationSelectionViewModel());
			Kernel.Bind<TripListViewModel>().ToConstant(new TripListViewModel());
			Kernel.Bind<ViewTripViewModel>().ToConstant(new ViewTripViewModel());
			Kernel.Bind<TripDestinationViewModel>().ToConstant(new TripDestinationViewModel());
			Kernel.Bind<ExecuteTripViewModel>().ToConstant(new ExecuteTripViewModel());
			Kernel.Bind<ExecuteTripDestinationViewModel>().ToConstant(new ExecuteTripDestinationViewModel());
		}

		#endregion

		#region public methods

		public static T Get<T> ()
		{
			return Kernel.Get<T>();
		}

		#endregion
	}
}