using RunnerTool.Core;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace RunnerTool
{
	public enum LanguageCode
	{
		DE,
		EN
	}

	/// <summary>
	/// The class to provide multi-language support. Provides strings for corresponding 
	/// tokens, based on the selected language code. 
	/// </summary>
	public static class Languages
	{
		#region delegates

		public delegate void LanguageDelegate ();
		public static event LanguageDelegate LanguageChanged;

		#endregion

		#region private members

		static Dictionary<LanguageCode, Dictionary<string, string>> languageCodeMapping;
		static LanguageCode selectedLanguage;

		static Dictionary<DependencyObject, string> frameworkElementTokenMapping;
		static Dictionary<DependencyObject, string> frameworkElementTooltipTokenMapping;

		#endregion

		#region public properties

		public static LanguageCode SelectedLanguage
		{
			get => selectedLanguage;
			set
			{
				if (value == selectedLanguage)
					return;

				selectedLanguage = value;

				UpdateRegisteredFrameworkElements();
				LanguageChanged?.Invoke();
			}
		}

		#endregion

		#region constructor

		static Languages ()
		{
			languageCodeMapping = new Dictionary<LanguageCode, Dictionary<string, string>>();
			frameworkElementTokenMapping = new Dictionary<DependencyObject, string>();
			frameworkElementTooltipTokenMapping = new Dictionary<DependencyObject, string>();

			SelectedLanguage = LanguageCode.DE;

			InitGermanStrings();
			InitEnglishStrings();
		}

		#endregion

		#region public methods

		/// <summary>
		/// Delivers a string for a given token, based on the selected language code
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static string Get (string token)
		{
			//delivers the corresponding string for given token, if existing
			if (languageCodeMapping[SelectedLanguage].ContainsKey(token))
				return languageCodeMapping[SelectedLanguage][token];
			//token not in mapping: return token to signalize entry is missing
			else
				return token;
		}

		public static void AddFrameworkElementTokenMapping (DependencyObject sender, string token)
		{
			//remove previous entry, if any, to avoid duplicates
			RemoveFrameworkElementTokenMapping(sender);

			frameworkElementTokenMapping.Add(sender, token);
		}

		public static void RemoveFrameworkElementTokenMapping (DependencyObject sender)
		{
			if (frameworkElementTokenMapping.ContainsKey(sender))
				frameworkElementTokenMapping.Remove(sender);
		}

		public static void AddFrameworkElementTooltipTokenMapping (DependencyObject sender, string token)
		{
			//remove previous entry, if any, to avoid duplicates
			RemoveFrameworkElementTooltipTokenMapping(sender);

			frameworkElementTooltipTokenMapping.Add(sender, token);
		}

		public static void RemoveFrameworkElementTooltipTokenMapping (DependencyObject sender)
		{
			if (frameworkElementTooltipTokenMapping.ContainsKey(sender))
				frameworkElementTooltipTokenMapping.Remove(sender);
		}

		#endregion

		#region private methods

		static void UpdateRegisteredFrameworkElements ()
		{
			frameworkElementTokenMapping.ToList().ForEach(entry =>
			{
				LToken.UpdateString(entry.Key, entry.Value);
			});

			frameworkElementTooltipTokenMapping.ToList().ForEach(entry =>
			{
				LTokenTooltip.UpdateString(entry.Key, entry.Value);
			});
		}

		static void InitGermanStrings ()
		{
			Dictionary<string, string> mapping = new Dictionary<string, string>
			{
				{ "runnersFinest", "Des Botens Feinstes" },
				{ "newTrip", "Neuer Trip" },
				{ "trips", "Trips" },
				{ "senders", "Sender" },
				{ "destinations", "Ziele" },
				{ "vacations", "Urlaube" },

				{ "close", "Schließen" },
				{ "cancel", "Abbrechen" },
				{ "create", "Erstellen" },
				{ "finish", "Bestätigen" },
				{ "select", "Auswählen" },
				{ "reset", "Rücksetzen" },

				{ "name", "Name" },
				{ "shortName", "Kürzel" },
				{ "type", "Typ" },

				{ "recipe", "Rezept" },
				{ "sign", "Unterschrift" },
				{ "buy", "Einkauf" },
				{ "other", "Sonstiges" },

				{ "createTrip", "Neuen Trip erstellen" },
				{ "executeTrip", "Trip ausführen" },
				{ "viewTrip", "Trip ansehen" },
				{ "editTrip", "Trip editieren" },

				{ "manageDestinations", "Ziele verwalten" },
				{ "createDestination", "Neues Ziel erstellen" },
				{ "editDestination", "Ziel editieren" },
				{ "newDestination", "Neues Ziel" },
				{ "goodieGiver", "Gibt Leckerli" },
				{ "goodieGiverSmiley", "Leckerli :)" },
				{ "closed", "Geschlossen" },
				{ "mondayShort", "MO" },
				{ "tuesdayShort", "DI" },
				{ "wednesdayShort", "MI" },
				{ "thursdayShort", "DO" },
				{ "fridayShort", "FR" },
				{ "saturdayShort", "SA" },
				{ "sundayShort", "SO" },

				{ "manageSenders", "Sender verwalten" },
				{ "createSender", "Neuen Sender erstellen" },
				{ "editSender", "Sender editieren" },
				{ "newSender", "Neuer Sender" },

				{ "manageVacations", "Urlaube verwalten" },
				{ "createVacation", "Neuen Urlaub erstellen" },
				{ "editVacation", "Urlaub editieren" },
				{ "newVacation", "Neuer Urlaub" },
				{ "begin", "Beginn" },
				{ "end", "Ende" },

				{ "destinationSelection", "Zielauswahl" },
				{ "senderSelection", "Senderauswahl" },

				{ "tripList", "Trip-Liste" },
				{ "filterTrips", "Trips filtern" },
				{ "union", "Vereinigung (ganzer Trip)" },
				{ "intersection", "Schnittmenge (ganzer Trip)" },
				{ "innerIntersection", "Schnittmenge (Sender-Ziel-Paar)" },
				{ "tripDate", "Trip Datum" },
				{ "finished", "Beendet" },
				{ "notFinishedYet", "Noch nicht beendet" },

				{ "groupBySenders", "Nach Sendern gruppieren" },
				{ "groupByDestinations", "Nach Zielen gruppieren" },
			};

			languageCodeMapping.Add(LanguageCode.DE, mapping);
		}

		static void InitEnglishStrings ()
		{
			Dictionary<string, string> mapping = new Dictionary<string, string>
			{
				{ "runnersFinest", "Runner's Finest" },
				{ "newTrip", "New trip" },
				{ "trips", "Trips" },
				{ "senders", "Senders" },
				{ "destinations", "Destinations" },
				{ "vacations", "Vacations" },

				{ "close", "Close" },
				{ "cancel", "Cancel" },
				{ "create", "Create" },
				{ "finish", "Finish" },
				{ "select", "Select" },
				{ "reset", "Reset" },

				{ "name", "Name" },
				{ "shortName", "Short name" },
				{ "type", "Type" },

				{ "recipe", "Recipe" },
				{ "sign", "Signature" },
				{ "buy", "Purchase" },
				{ "other", "Other" },

				{ "createTrip", "Create new trip" },
				{ "executeTrip", "Execute trip" },
				{ "viewTrip", "View trip" },
				{ "editTrip", "Edit trip" },

				{ "manageDestinations", "Manage destinations" },
				{ "createDestination", "Create new destination" },
				{ "editDestination", "Edit destination" },
				{ "newDestination", "New destination" },
				{ "goodieGiver", "Gives goodie" },
				{ "goodieGiverSmiley", "Goodie giver :)" },
				{ "closed", "Closed" },
				{ "mondayShort", "MO" },
				{ "tuesdayShort", "TU" },
				{ "wednesdayShort", "WE" },
				{ "thursdayShort", "TH" },
				{ "fridayShort", "FR" },
				{ "saturdayShort", "SA" },
				{ "sundayShort", "SU" },

				{ "manageSenders", "Manage senders" },
				{ "createSender", "Create new sender" },
				{ "editSender", "Edit sender" },
				{ "newSender", "New sender" },

				{ "manageVacations", "Manage vacations" },
				{ "createVacation", "Create new vacation" },
				{ "editVacation", "Edit vacation" },
				{ "newVacation", "New vacation" },
				{ "begin", "Begin" },
				{ "end", "End" },

				{ "destinationSelection", "Destination selection" },
				{ "senderSelection", "Sender selection" },

				{ "tripList", "Trip list" },
				{ "filterTrips", "Filter trips" },
				{ "union", "Unioin (whole trip)" },
				{ "intersection", "Intersection (whole trip)" },
				{ "innerIntersection", "Intersection (sender-destination pair)" },
				{ "tripDate", "Trip date" },
				{ "finished", "Finished" },
				{ "notFinishedYet", "Not finished yet" },

				{ "groupBySenders", "Group by senders" },
				{ "groupByDestinations", "Group by destinations" },
			};

			languageCodeMapping.Add(LanguageCode.EN, mapping);
		}

		#endregion
	}
}