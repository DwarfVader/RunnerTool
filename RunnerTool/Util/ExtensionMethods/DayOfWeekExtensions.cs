using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public static class DayOfWeekExtensions
	{
		public static string GetShortName (this DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
			case DayOfWeek.Sunday:
				return Languages.Get("sundayShort");
			case DayOfWeek.Monday:
				return Languages.Get("mondayShort");
			case DayOfWeek.Tuesday:
				return Languages.Get("tuesdayShort");
			case DayOfWeek.Wednesday:
				return Languages.Get("wednesdayShort");
			case DayOfWeek.Thursday:
				return Languages.Get("thursdayShort");
			case DayOfWeek.Friday:
				return Languages.Get("fridayShort");
			case DayOfWeek.Saturday:
				return Languages.Get("saturdayShort");
			}

			//dummy
			return "";
		}
	}
}