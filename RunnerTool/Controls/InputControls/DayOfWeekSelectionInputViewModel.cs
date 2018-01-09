using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using RunnerTool.Core;

namespace RunnerTool
{
	public class DayOfWeekSelection : BaseObservable
	{
		public DayOfWeek DayOfWeek { get; private set; }
		public bool IsSelected { get; set; }
		public string DayOfWeekShort => DayOfWeek.ToString().ToLower() + "Short";
		public Brush Foreground => (IsSelected ? Brushes.Red : Brushes.White);

		public ICommand SelectCommand { get; private set; }

		public DayOfWeekSelection (DayOfWeek dayOfWeek)
		{
			DayOfWeek = dayOfWeek;

			//commands
			SelectCommand = new RelayCommand(SelectCommandMethod);
		}

		void SelectCommandMethod ()
		{
			IsSelected = !IsSelected;
		}

		public void Reset ()
		{
			IsSelected = false;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class DayOfWeekSelectionInputViewModel : BaseInputViewModel
	{
		#region public properties

		public List<DayOfWeekSelection> DayOfWeekSelections { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public DayOfWeekSelectionInputViewModel (string labelText) : base(labelText)
		{
			DayOfWeekSelections = new List<DayOfWeekSelection>
			{
				new DayOfWeekSelection(DayOfWeek.Monday),
				new DayOfWeekSelection(DayOfWeek.Tuesday),
				new DayOfWeekSelection(DayOfWeek.Wednesday),
				new DayOfWeekSelection(DayOfWeek.Thursday),
				new DayOfWeekSelection(DayOfWeek.Friday),
				new DayOfWeekSelection(DayOfWeek.Saturday),
				new DayOfWeekSelection(DayOfWeek.Sunday),
			};

			Reset();
		}

		#endregion

		#region public methods

		public override void Reset ()
		{
			DayOfWeekSelections.ForEach(d => d.Reset());
		}

		public void Update (DayOfWeekDateConstraint constraint)
		{
			//reset before new values get set
			Reset();

			//select day of week selections based on given constraint
			DayOfWeekSelections.Where(d => constraint.Days.Contains(d.DayOfWeek)).ToList().ForEach(d => d.IsSelected = true);
		}

		#endregion
	}
}