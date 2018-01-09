using Microsoft.Win32;
using System;
using System.Timers;

namespace RunnerTool.Core
{
	public static class DayChangedNotifier
	{
		public delegate void DayChangedDelegate ();
		public static event DayChangedDelegate DayChanged;

		private static Timer timer;

		static DayChangedNotifier ()
		{
			timer = new Timer(GetSleepTime());
			timer.Elapsed += (s, e) =>
			{
				OnDayChanged();
				timer.Interval = GetSleepTime();
			};
			timer.Start();

			//system time got changed
			SystemEvents.TimeChanged += (s, e) => { timer.Interval = GetSleepTime(); };
		}

		static double GetSleepTime ()
		{
			var nextMidnight = DateTime.Today.AddDays(1);
			return (nextMidnight - DateTime.Now).TotalMilliseconds;
		}

		static void OnDayChanged ()
		{
			DayChanged?.Invoke();
		}
	}
}