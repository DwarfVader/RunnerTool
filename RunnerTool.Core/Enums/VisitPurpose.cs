namespace RunnerTool.Core
{
	/// <summary>
	/// The type of a destination on the trip
	/// </summary>
	public enum VisitPurposeType
	{
		/// <summary>
		/// Get a recipe
		/// </summary>
		Recipe,
		/// <summary>
		/// Let something be signed
		/// </summary>
		Sign,
		/// <summary>
		/// Buy stuff
		/// </summary>
		Buy,
		/// <summary>
		/// Something else
		/// </summary>
		Other
	}

	/// <summary>
	/// The state of a visit purpose. Changed via execution of trip
	/// </summary>
	public enum VisitPurposeState
	{
		/// <summary>
		/// The containing trip has not been executed
		/// </summary>
		Unhandled,
		/// <summary>
		/// This visit purpose has been finished by the containing trip
		/// </summary>
		Finished,
		/// <summary>
		/// This visit purpose has been abandoned by the containing trip
		/// </summary>
		Abandoned
	}
}