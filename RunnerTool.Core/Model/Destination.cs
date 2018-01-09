using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Destination : BaseObservableContent
	{
		#region public properties

		/// <summary>
		/// The name of the destination
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// A shortened name / ID
		/// </summary>
		public string ShortName { get; set; }

		/// <summary>
		/// The type of the destination
		/// </summary>
		public DestinationType Type { get; set; }

		/// <summary>
		/// Indicates if this destination grants goodies sometimes
		/// </summary>
		public bool IsGoodieGiver { get; set; }

		/// <summary>
		/// Specifies the possible purposes for the visit
		/// </summary>
		public List<VisitPurposeSelection> VisitPurposeSelectionTemplates { get; private set; }

		/// <summary>
		/// Specifies a list of date constraints to calculate availability for a given date
		/// </summary>
		public List<DateConstraint> NotAvailableConstraints { get; private set; }

		/// <summary>
		/// Delivers the <see cref="DayOfWeekDateConstraint"/> for closed days of the destination
		/// </summary>
		public DayOfWeekDateConstraint ClosedDaysConstraint { get; private set; }

		#endregion

		#region constructor

		public Destination (string name, string shortName, DestinationType type, List<DateConstraint> notAvailableConstraints = null,
			bool isGoodieGiver = false)
		{
			Name = name;
			ShortName = shortName;
			Type = type;
			IsGoodieGiver = isGoodieGiver;
			InitNotAvailableConstraints(notAvailableConstraints);
			InitVisitPurposeTemplates();
		}

		#endregion

		#region public methods

		public void AddVisitPurposeTemplate (VisitPurposeType visitPurposeType, bool isPreSelected = false)
		{
			VisitPurposeSelectionTemplates.Add(new VisitPurposeSelection(visitPurposeType, VisitPurposeState.Unhandled, isPreSelected));
		}

		public void AddNotAvailableConstraint (DateConstraint dateConstraint)
		{
			NotAvailableConstraints.Add(dateConstraint);
		}

		public void RemoveVacationConstraint (Vacation vacation)
		{
			//removes all constraints based on given vacation
			NotAvailableConstraints.RemoveAll(c => c is VacationDateConstraint && ((VacationDateConstraint)c).Vacation == vacation);
		}

		/// <summary>
		/// Checks whether this destination is available/open for the given date
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public bool IsAvailable (DateTime date)
		{
			return !NotAvailableConstraints.Any(c => c.IsSatisfying(date));
		}

		#endregion

		#region private methods

		void InitVisitPurposeTemplates ()
		{
			VisitPurposeSelectionTemplates = new List<VisitPurposeSelection>();

			foreach (VisitPurposeSelection vp in DB.Instance.DestinationVisitPurposeMapping[Type])
			{
				VisitPurposeSelectionTemplates.Add(new VisitPurposeSelection(vp));
			}
		}

		void InitNotAvailableConstraints (List<DateConstraint> notAvailableConstraints)
		{
			//create new list
			NotAvailableConstraints = new List<DateConstraint>();

			//create day of week constraint and add to list
			ClosedDaysConstraint = new DayOfWeekDateConstraint();
			NotAvailableConstraints.Add(ClosedDaysConstraint);

			//handle input constraints
			if (notAvailableConstraints != null)
			{
				foreach (var constraint in notAvailableConstraints)
				{
					if (constraint is DayOfWeekDateConstraint)
					{
						//manipulate existing constraint
						ClosedDaysConstraint.SetDays(((DayOfWeekDateConstraint)constraint).Days);
					}
					else
					{
						//add new constraint
						NotAvailableConstraints.Add(constraint);
					}
				}
			}
		}

		#endregion
	}
}