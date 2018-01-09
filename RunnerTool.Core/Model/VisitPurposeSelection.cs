namespace RunnerTool.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class VisitPurposeSelection : BaseObservableContent
	{
		#region public properties

		/// <summary>
		/// The purpose to visit a destination
		/// </summary>
		public VisitPurposeType Type { get; private set; }

		/// <summary>
		/// The state of visit purpose; needed to store previous value in order to 
		/// restore if trip gets edited
		/// </summary>
		public VisitPurposeState State { get; set; }

		/// <summary>
		/// Indicates, whether the purpose needs to be fulfilled at the visit
		/// </summary>
		public bool IsSelected { get; set; }

		#endregion

		#region constructor

		public VisitPurposeSelection (VisitPurposeSelection other) : this(other.Type, other.State, other.IsSelected) { }

		public VisitPurposeSelection (VisitPurposeType type, bool isSelected) : this(type, VisitPurposeState.Unhandled, isSelected) { }

		public VisitPurposeSelection (VisitPurposeType type, VisitPurposeState state, bool isSelected)
		{
			Type = type;
			State = state;
			IsSelected = isSelected;
		}

		#endregion
	}
}