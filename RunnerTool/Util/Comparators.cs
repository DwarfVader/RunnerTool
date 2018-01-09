namespace RunnerTool
{
	/// <summary>
	/// A collection of some comparator methods
	/// </summary>
	public static class Comparators
	{
		/// <summary>
		/// Compares two <see cref="DestinationVisitPurposeSelection"/>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int DestinationVisitSelectionCompare (DestinationVisitPurposeSelection a, DestinationVisitPurposeSelection b)
		{
			//same type: compare by name
			if (a.Destination.Type == b.Destination.Type)
			{
				return string.Compare(a.Destination.ShortName, b.Destination.ShortName, true);
			}

			//compare by destination type
			return a.Destination.Type - b.Destination.Type;
		}

		public static int DestinationListItemViewModelCompare (DestinationListItemViewModel a, DestinationListItemViewModel b)
		{
			//same type: compare by name
			if (a.Destination.Type == b.Destination.Type)
			{
				return string.Compare(a.Destination.ShortName, b.Destination.ShortName, true);
			}

			//compare by destination type
			return a.Destination.Type - b.Destination.Type;
		}

		public static int SenderListItemViewModelCompare (SenderListItemViewModel a, SenderListItemViewModel b)
		{
			//same type
			if (a.Sender.Type.Order == b.Sender.Type.Order)
			{
				//compare by short name
				return a.Sender.ShortName.CompareTo(b.Sender.ShortName);
			}

			return a.Sender.Type.Order - b.Sender.Type.Order;
		}

		public static int VacationListItemViewModelCompare (VacationListItemViewModel a, VacationListItemViewModel b)
		{
			//same begin date
			if (a.Vacation.BeginDate.CompareTo(b.Vacation.BeginDate) == 0)
			{
				//same end date
				if (a.Vacation.EndDate.CompareTo(b.Vacation.EndDate) == 0)
				{
					//compare names of destinations
					return a.Vacation.Destination.Name.CompareTo(b.Vacation.Destination.Name);
				}

				//compare end dates
				return a.Vacation.EndDate.CompareTo(b.Vacation.EndDate);
			}

			//compare begin dates
			return a.Vacation.BeginDate.CompareTo(b.Vacation.BeginDate);
		}
	}
}