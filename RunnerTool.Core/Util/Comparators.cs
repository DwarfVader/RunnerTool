namespace RunnerTool.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Comparators
	{
		/// <summary>
		/// Compares two <see cref="Destination"/>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int DestinationCompare (Destination a, Destination b)
		{
			//same type: compare by name
			if (a.Type == b.Type)
			{
				return string.Compare(a.ShortName, b.ShortName, true);
			}

			//compare by destination type
			return a.Type - b.Type;
		}

		/// <summary>
		/// Compares two <see cref="Sender"/>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int SenderCompare (Sender a, Sender b)
		{
			//types are equal
			if (a.Type.Order == b.Type.Order)
			{
				//compare by short name
				return a.ShortName.CompareTo(b.ShortName);
			}

			return a.Type.Order - b.Type.Order;
		}

		/// <summary>
		/// Compares two <see cref="SenderType"/>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int SenderTypeCompare (SenderType a, SenderType b)
		{
			return a.Order - b.Order;
		}

		/// <summary>
		/// Compares two <see cref="Trip"/>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int TripCompare (Trip a, Trip b)
		{
			return a.Date.CompareTo(b.Date);
		}

		/// <summary>
		/// Compares two <see cref="VisitPurpose"/> by their type
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int VisitPurposeTypeCompare (VisitPurpose a, VisitPurpose b)
		{
			return a.Type.CompareTo(b.Type);
		}

		/// <summary>
		/// Comapre two <see cref="Vacation"/> by their begin date, end date and destination name
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int VacationCompare (Vacation a, Vacation b)
		{
			//same begin date
			if (a.BeginDate.CompareTo(b.BeginDate) == 0)
			{
				//same end date
				if (a.EndDate.CompareTo(b.EndDate) == 0)
				{
					return a.Destination.Name.CompareTo(b.Destination.Name);
				}

				return a.EndDate.CompareTo(b.EndDate);
			}

			return a.BeginDate.CompareTo(b.BeginDate);
		}
	}
}