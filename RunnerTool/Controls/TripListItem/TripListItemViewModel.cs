using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class TripListItemViewModel : BaseViewModel
	{
		#region public properties

		public Trip Trip { get; private set; }

		public string DateFormatted => GetDateFormatted();

		public string Tooltip { get; private set; }

		#endregion

		#region commands

		public ICommand ViewTripCommand { get; private set; }
		public ICommand ExecuteTripCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public TripListItemViewModel (Trip trip)
		{
			Trip = trip;
			UpdateTooltip();

			//commands
			ViewTripCommand = new RelayCommand(ViewTripCommandMethod);
			ExecuteTripCommand = new RelayCommand(ExecuteTripCommandMethod);

			//register events
			Languages.LanguageChanged += UpdateTooltip;
		}

		~TripListItemViewModel ()
		{
			//unregister events
			Languages.LanguageChanged -= UpdateTooltip;
		}

		#endregion

		#region command methods

		void ViewTripCommandMethod ()
		{
			IoC.ViewTripViewModel.Trip = Trip;
			FrameHistory.OpenFrame(IoC.ViewTripViewModel);
		}

		void ExecuteTripCommandMethod ()
		{
			//do not execute not fully created trips
			if (!Trip.IsCreated)
				return;

			//another trip in execution
			if (IoC.ExecuteTripViewModel.IsExecuting && IoC.ExecuteTripViewModel.Trip != Trip)
				return;

			//trip date does not match current date
			if (Trip.Date.Date.CompareTo(DateTime.UtcNow.Date) != 0)
				return;

			//show trip execution frame; need to do this BEFORE trip gets set
			FrameHistory.OpenFrame(IoC.ExecuteTripViewModel);

			//not already executing a trip...
			if (!IoC.ExecuteTripViewModel.IsExecuting)
				//set trip to be executed
				IoC.ExecuteTripViewModel.Trip = Trip;
		}

		#endregion

		#region private methods

		string GetDateFormatted ()
		{
			return Trip.Date.ToShortDateString();
		}

		void UpdateTooltip ()
		{
			string tt = "";

			//dates / finished
			tt += Languages.Get("tripDate") + ": " + Trip.Date.ToShortDateString();
			tt += "\n" + Languages.Get((Trip.IsFinished ? "finished" : "notFinishedYet"));

			//senders
			tt += "\n\n" + Languages.Get("senders") + ":\n";
			List<Sender> senders = new List<Sender>();
			foreach (var td in Trip.TripDestinations)
			{
				if (!senders.Contains(td.Sender))
				{
					senders.Add(td.Sender);
				}
			}
			foreach (var sender in senders)
			{
				tt += sender.ShortName + " ";
			}

			//destinations
			tt += "\n\n" + Languages.Get("destinations") + ":\n";
			List<Destination> destinations = new List<Destination>();
			foreach (var td in Trip.TripDestinations)
			{
				if (!destinations.Contains(td.Destination))
				{
					destinations.Add(td.Destination);
				}
			}
			foreach (var dest in destinations)
			{
				tt += dest.ShortName + " ";
			}

			Tooltip = tt;
		}

		#endregion
	}
}