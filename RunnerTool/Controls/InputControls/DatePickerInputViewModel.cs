using System;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class DatePickerInputViewModel : BaseInputViewModel
	{
		#region public properties

		/// <summary>
		/// The date of the date picker
		/// </summary>
		public DateTime Date { get; set; }

		#endregion

		#region commands

		public ICommand SelectCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public DatePickerInputViewModel (string labelText) : base(labelText)
		{
			Reset();

			//commands
			SelectCommand = new RelayCommand(SelectCommandMethod);
		}

		#endregion

		#region command methods

		void SelectCommandMethod ()
		{

		}

		#endregion

		#region public methods

		public override void Reset ()
		{
			Date = DateTime.UtcNow.Date;
		}
	}

	#endregion
}