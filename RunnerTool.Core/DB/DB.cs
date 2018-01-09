using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class DB
	{
		#region singleton

		static DB instance;
		static readonly object syncRoot = new object();
		public static DB Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new DB();
							instance.Init();
						}
					}
				}

				return instance;
			}
		}

		#endregion

		#region delegates

		public delegate void DestinationDelegate ();
		public event DestinationDelegate OnDestinationsChanged;

		public delegate void SenderDelegate ();
		public event SenderDelegate OnSendersChanged;

		public delegate void TripDelegate ();
		public event TripDelegate OnTripsChanged;

		public delegate void VacationDelegate ();
		public event VacationDelegate OnVacationsChanged;

		#endregion

		#region private members

		List<Destination> destinations;
		List<Sender> senders;
		List<SenderType> senderTypes;
		List<Trip> trips;
		List<Vacation> vacations;

		#endregion

		#region public properties

		public Dictionary<DestinationType, List<VisitPurposeSelection>> DestinationVisitPurposeMapping { get; private set; }

		public List<SenderType> SenderTypes
		{
			get
			{
				senderTypes.Sort(Comparators.SenderTypeCompare);
				return senderTypes;
			}
			private set => senderTypes = value;
		}

		public List<Sender> Senders
		{
			get
			{
				senders.Sort(Comparators.SenderCompare);
				return senders;
			}
			private set => senders = value;
		}

		public List<Destination> Destinations
		{
			get
			{
				destinations.Sort(Comparators.DestinationCompare);
				return destinations;
			}
			private set => destinations = value;
		}

		public List<Trip> Trips
		{
			get
			{
				trips.Sort(Comparators.TripCompare);
				return trips;
			}
			private set => trips = value;
		}

		public List<TripDestination> AbandonedTripDestinations { get; private set; }

		public List<Vacation> Vacations
		{
			get
			{
				vacations.Sort(Comparators.VacationCompare);
				return vacations;
			}
			private set => vacations = value;
		}

		#endregion

		#region constructor

		/// <summary>
		/// Private constructor
		/// </summary>
		DB ()
		{
			//register events
			DayChangedNotifier.DayChanged += CheckVacationStates;
		}

		~DB ()
		{
			//unregister events
			DayChangedNotifier.DayChanged -= CheckVacationStates;
		}

		#endregion

		#region public methods

		public void Init ()
		{
			AbandonedTripDestinations = new List<TripDestination>();

			InitDestinationVisitPurposeMapping();
			InitSenderTypes();
			InitSenders();
			InitDestinations();
			InitTrips();
			InitVacations();
		}

		public void AddDestination (Destination destination)
		{
			Destinations.Add(destination);

			CallOnDestinationsChanged();
		}

		public void AddSender (Sender sender)
		{
			Senders.Add(sender);

			CallOnSendersChanged();
		}

		public void AddTrip (Trip trip)
		{
			Trips.Add(trip);

			CallOnTripsChanged();
		}

		public void AddVacation (Vacation vacation)
		{
			Vacations.Add(vacation);

			CallOnVacationsChanged();
		}

		public void CallOnDestinationsChanged ()
		{
			OnDestinationsChanged?.Invoke();
		}

		public void CallOnSendersChanged ()
		{
			OnSendersChanged?.Invoke();
		}

		public void CallOnTripsChanged ()
		{
			OnTripsChanged?.Invoke();
		}

		public void CallOnVacationsChanged ()
		{
			OnVacationsChanged?.Invoke();
		}

		public void AddToAbandonedTripDestination (TripDestination tripDestination)
		{
			//add/combine an entry for each visit purpose
			tripDestination.VisitPurposes.ForEach(vp => AddToAbandonedTripDestination(
				tripDestination.Sender, tripDestination.Destination, vp.Type));
		}

		public void AddToAbandonedTripDestination (Sender sender, Destination destination, VisitPurposeType visitPurposeType)
		{
			//retrieve trip destination if already in list
			TripDestination td = AbandonedTripDestinations.FirstOrDefault(aTD =>
				aTD.Sender == sender && aTD.Destination == destination);

			//not in list yet
			if (td == null)
			{
				//create new trip destination...
				td = new TripDestination(null, sender, destination);

				//and add to abandoned list
				AbandonedTripDestinations.Add(td);
			}

			//check if visit purpose is not alreay in list
			if (!td.VisitPurposes.Any(vp => vp.Type == visitPurposeType))
			{
				//add visit purpose; need to set state unhandled as it is going to be added to a new trip as fresh entry
				td.AddVisitPurpose(visitPurposeType, VisitPurposeState.Unhandled);
			}
		}

		public void RemoveFromAbandonedTripDestination (Sender sender, Destination destination, VisitPurposeType visitPurposeType)
		{
			//retrieve trip destination
			TripDestination td = AbandonedTripDestinations.FirstOrDefault(aTD =>
				aTD.Sender == sender && aTD.Destination == destination);

			//not in list...
			if (td == null)
				throw new Exception("Abandoned list should contain trip destination with given sender and destination");

			//remove visit purpose from trip destination
			td.VisitPurposes.RemoveAll(vp => vp.Type == visitPurposeType);

			//if no visit purpose is left in trip destination...
			if (td.VisitPurposes.Count == 0)
				//remove trip destination from abandoned list
				AbandonedTripDestinations.Remove(td);
		}

		#endregion

		#region private methods

		void CheckVacationStates ()
		{
			Vacations.ForEach(v => v.CheckFinished());
		}

		void InitDestinationVisitPurposeMapping ()
		{
			DestinationVisitPurposeMapping = new Dictionary<DestinationType, List<VisitPurposeSelection>> {
				{ DestinationType.Dr,
					new List<VisitPurposeSelection> {
						new VisitPurposeSelection(VisitPurposeType.Recipe, true),
						new VisitPurposeSelection(VisitPurposeType.Sign, false),
						new VisitPurposeSelection(VisitPurposeType.Other, false)
					}
				},

				{ DestinationType.Shop,
					new List<VisitPurposeSelection> {
						new VisitPurposeSelection(VisitPurposeType.Buy, true),
						new VisitPurposeSelection(VisitPurposeType.Other, false)
					}
				},

				{ DestinationType.Other,
					new List<VisitPurposeSelection> {
						new VisitPurposeSelection(VisitPurposeType.Other, true)
					}
				},
			};
		}

		void InitSenderTypes ()
		{
			SenderTypes = new List<SenderType>
			{
				new SenderType(0, "NBS"),
				new SenderType(1, "AB"),
				new SenderType(2, "NB"),
				new SenderType(3, "APP"),
				//administration: porter, secretary, accounting
				new SenderType(4, "ADMIN"),
			};
		}

		void InitSenders ()
		{
			Senders = new List<Sender>
			{
				new Sender (SenderType.GetByOrder(0), "Neubau Süd", "NBS"),
				new Sender (SenderType.GetByOrder(1), "Altbau 1", "AB1"),
				new Sender (SenderType.GetByOrder(1), "Altbau 2", "AB2"),
				new Sender (SenderType.GetByOrder(1), "Altbau 3+4", "AB34"),
				new Sender (SenderType.GetByOrder(2), "Neubau 1", "NB1"),
				new Sender (SenderType.GetByOrder(2), "Neubau 2", "NB2"),
				new Sender (SenderType.GetByOrder(2), "Neubau 3+4", "NB34"),
				new Sender (SenderType.GetByOrder(3), "Appartements", "APP"),
				new Sender (SenderType.GetByOrder(4), "Portier", "POR"),
				new Sender (SenderType.GetByOrder(4), "Sekretariat", "SEK"),
				new Sender (SenderType.GetByOrder(4), "Buchhaltung", "BUCH"),
			};
		}

		void InitDestinations ()
		{
			Destinations = new List<Destination>
			{
				new Destination("Dr. Mair", "MAI", DestinationType.Dr),
				new Destination("Dr. Zanier", "ZAN", DestinationType.Dr,
				new List<DateConstraint>
				{
					new DayOfWeekDateConstraint(DayOfWeek.Thursday),
				}),
				new Destination("Dr. Faes", "FAES", DestinationType.Dr),
				new Destination("Dr. Schneider", "SCH", DestinationType.Dr,
				new List<DateConstraint>
				{
					new DayOfWeekDateConstraint(DayOfWeek.Wednesday)
				}),
				new Destination("Dr. Brandstätter", "BRA", DestinationType.Dr, null, true),
				new Destination("Dr. Weiler", "WEI", DestinationType.Dr,
				new List<DateConstraint>
				{
					new DayOfWeekDateConstraint(DayOfWeek.Wednesday)
				}),
				new Destination("Dr. Wieser", "WIE", DestinationType.Dr),
				new Destination("Dr. Lassnig-Sitte", "SIT", DestinationType.Dr,
				new List<DateConstraint>
				{
					new DayOfWeekDateConstraint(DayOfWeek.Wednesday)
				}),
				new Destination("Dr. Seibald", "SEI", DestinationType.Dr,
				new List<DateConstraint>
				{
					new DayOfWeekDateConstraint(DayOfWeek.Tuesday, DayOfWeek.Thursday)
				}, true),

				new Destination("Sanibed", "SAN", DestinationType.Shop),
				new Destination("Papier Geiger", "GEI", DestinationType.Shop),
				new Destination("Tyrolia", "TYR", DestinationType.Shop),
				new Destination("Spar", "SPAR", DestinationType.Shop),
				new Destination("Osttiroler Bote", "OB", DestinationType.Shop),

				new Destination("Metagil", "MET", DestinationType.Other),
				new Destination("Stadtgemeinde", "LIE", DestinationType.Other),
				new Destination("Meldeamt", "MEL", DestinationType.Other),
				new Destination("Dolomitenbank", "DOL", DestinationType.Other),
				new Destination("Raika", "RAI", DestinationType.Other),
				new Destination("Sparkasse", "SPK", DestinationType.Other),
				new Destination("Neuroth", "NEU", DestinationType.Other),
				new Destination("Postamt", "POST", DestinationType.Other),
			};

		}

		void InitTrips ()
		{
			Trips = new List<Trip>();
			Trip trip;

			trip = new Trip();
			trip.AddTripDestination(new TripDestination(trip, Senders[0], Destinations[0],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[0].VisitPurposeSelectionTemplates[0].Type, true),
					new VisitPurposeSelection(Destinations[0].VisitPurposeSelectionTemplates[1].Type, true)
				}));
			trip.AddTripDestination(new TripDestination(trip, Senders[0], Destinations[1],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[1].VisitPurposeSelectionTemplates[0].Type, true),
					new VisitPurposeSelection(Destinations[1].VisitPurposeSelectionTemplates[1].Type, true)
				}));
			trip.AddTripDestination(new TripDestination(trip, Senders[0], Destinations[2],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[2].VisitPurposeSelectionTemplates[0].Type, true),
				}));
			trip.AddTripDestination(new TripDestination(trip, Senders[0], Destinations[5],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[5].VisitPurposeSelectionTemplates[0].Type, true),
				}));
			trip.AddTripDestination(new TripDestination(trip, Senders[2], Destinations[0],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[0].VisitPurposeSelectionTemplates[0].Type, true),
				}));
			trip.AddTripDestination(new TripDestination(trip, Senders[9], Destinations[1],
				new List<VisitPurposeSelection> {
					new VisitPurposeSelection(Destinations[1].VisitPurposeSelectionTemplates[0].Type, true),
				}));
			trip.FinishCreation();
			Trips.Add(trip);

			trip = new Trip();
			trip.AddTripDestination(new TripDestination(trip, Senders[8], Destinations[1],
				new List<VisitPurposeSelection> {
					Destinations[1].VisitPurposeSelectionTemplates[0]
				}));
			trip.Date = DateTime.UtcNow.AddDays(2);
			trip.FinishCreation();
			Trips.Add(trip);
		}

		void InitVacations ()
		{
			Vacations = new List<Vacation>
			{
				new Vacation(Destinations[1], DateTime.UtcNow.AddDays(-4), DateTime.UtcNow.AddDays(-20)),
				new Vacation(Destinations[0], DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(2)),
				new Vacation(Destinations[2], DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3)),
			};
		}

		#endregion
	}
}