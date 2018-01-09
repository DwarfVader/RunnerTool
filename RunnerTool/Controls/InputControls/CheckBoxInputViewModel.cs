using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class CheckBoxInputViewModel : BaseInputViewModel
	{
		#region public properties

		/// <summary>
		/// The temporary string value that is going to be set
		/// </summary>
		public bool IsChecked { get; set; }

		#endregion

		#region commands

		public ICommand CheckCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CheckBoxInputViewModel (string labelText) : base(labelText)
		{
			Reset();

			//commands
			CheckCommand = new RelayCommand(CheckCommandMethod);
		}

		#endregion

		#region command methods

		void CheckCommandMethod ()
		{
			IsChecked = !IsChecked;
		}

		#endregion

		#region public methods

		public override void Reset ()
		{
			IsChecked = false;
		}
	}

	#endregion
}