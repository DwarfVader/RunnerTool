using System;
using System.Collections.Generic;
using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// Base class for date constraints 
	/// </summary>
	public abstract class DateConstraint
	{
		/// <summary>
		/// Checks whether the given date satisfies the constraint
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public abstract bool IsSatisfying (DateTime date);
	}

	/// <summary>
	/// Stores a list of <see cref="DayOfWeek"/> to check the day of week 
	/// of a <see cref="DateTime"/> against
	/// </summary>
	public class DayOfWeekDateConstraint : DateConstraint
	{
		/// <summary>
		/// The list of days that satisfy the constraint
		/// </summary>
		public List<DayOfWeek> Days { get; private set; }

		public DayOfWeekDateConstraint (params DayOfWeek[] days) : this(days.ToList()) { }

		public DayOfWeekDateConstraint (List<DayOfWeek> days)
		{
			Days = new List<DayOfWeek>(days);
		}

		public void SetDays (List<DayOfWeek> days)
		{
			SetDays(days.ToArray());
		}
		public void SetDays (params DayOfWeek[] days)
		{
			Days.Clear();
			Days.AddRange(days);
		}

		public override bool IsSatisfying (DateTime date)
		{
			return Days.Any(d => date.DayOfWeek == d);
		}
	}

	/// <summary>
	/// Stores a <see cref="Vacation"/> to check whether a given <see cref="DateTime"/> 
	/// lies in between its begin and end dates.
	/// </summary>
	public class VacationDateConstraint : DateConstraint
	{
		/// <summary>
		/// The vacation on which this constraint is based
		/// </summary>
		public Vacation Vacation { get; private set; }

		public VacationDateConstraint (Vacation vacation)
		{
			Vacation = vacation;
		}

		public override bool IsSatisfying (DateTime date)
		{
			//whether given date lies in between vacation's begin and end dates
			return date.Date >= Vacation.BeginDate && date.Date <= Vacation.EndDate;
		}
	}
}