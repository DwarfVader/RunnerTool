namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseInputViewModel : BaseViewModel
	{
		#region public properties

		/// <summary>
		/// Descriptive label text
		/// </summary>
		public string LabelText { get; protected set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public BaseInputViewModel (string labelText)
		{
			LabelText = labelText;
		}

		#endregion

		#region public methods

		public abstract void Reset ();

		#endregion
	}
}