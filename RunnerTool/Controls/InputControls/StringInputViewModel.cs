namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class StringInputViewModel : BaseInputViewModel
	{
		#region public properties

		/// <summary>
		/// The temporary string value that is going to be set
		/// </summary>
		public string Text { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public StringInputViewModel (string labelText, string text = "") : base(labelText)
		{
			Text = text;
		}

		#endregion

		#region public methods

		public override void Reset ()
		{
			Text = "";
		}
	}

	#endregion
}