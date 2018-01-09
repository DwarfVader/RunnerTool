using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// A view model for a vacation list entry
	/// </summary>
	public class VacationListItemViewModel : BaseViewModel
	{
		#region delegates

		public Action<Vacation> LeftClickCallback { get; set; }
		PropertyChangedEventHandler onVacationChangedHandler;

		#endregion

		#region public properties

		/// <summary>
		/// The model
		/// </summary>
		public Vacation Vacation { get; private set; }

		/// <summary>
		/// A shortened begin date string
		/// </summary>
		public string DateString => GetDateString();

		/// <summary>
		/// A tooltip, providing information about the vacation
		/// </summary>
		public virtual string Tooltip { get; private set; }

		#endregion

		#region commands

		public ICommand LeftClickCommand { get; private set; }
		public ICommand RightClickCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VacationListItemViewModel (Vacation vacation)
		{
			Vacation = vacation;
			UpdateTooltip();

			//commands
			LeftClickCommand = new RelayCommand(LeftClickCommandMethod);
			RightClickCommand = new RelayCommand(RightClickCommandMethod);

			//register events
			Languages.LanguageChanged += UpdateTooltip;
			if (Vacation != null)
			{
				onVacationChangedHandler = (s, e) => UpdateTooltip();
				Vacation.PropertyChanged += onVacationChangedHandler;
			}
		}

		~VacationListItemViewModel ()
		{
			//unregister events
			Languages.LanguageChanged -= UpdateTooltip;
			if (Vacation != null)
				Vacation.PropertyChanged -= onVacationChangedHandler;
		}

		#endregion

		#region command methods

		protected virtual void LeftClickCommandMethod ()
		{
			LeftClickCallback?.Invoke(Vacation);
		}

		protected virtual void RightClickCommandMethod () { }

		#endregion

		#region private methods

		void UpdateTooltip ()
		{
			if (Vacation == null)
			{
				Tooltip = "";
				return;
			}

			string tt = "";

			//destination
			tt += Vacation.Destination.Name;

			//dates / finished
			tt += "\n" + Languages.Get("begin") + ": " + Vacation.BeginDate.ToShortDateString();
			tt += "\n" + Languages.Get("end") + ": " + Vacation.EndDate.ToShortDateString();
			tt += (Vacation.IsFinished ? "\n" + Languages.Get("finished") : "");

			Tooltip = tt;
		}

		string GetDateString ()
		{
			string date = "";

			//only a single day
			if (Vacation.BeginDate.CompareTo(Vacation.EndDate) == 0)
			{
				date = Vacation.BeginDate.ToString("dd.MM.yy");
			}
			//vacation does not cross year
			else if (Vacation.BeginDate.Year == Vacation.EndDate.Year)
			{
				date = Vacation.BeginDate.ToString("dd.MM.") + " - " + Vacation.EndDate.ToString("dd.MM.yy");
			}
			//vacation crosses year
			else
			{
				date = Vacation.BeginDate.ToString("dd.MM.yy") + " - " + Vacation.EndDate.ToString("dd.MM.yy");
			}

			return date;
		}

		#endregion
	}
}