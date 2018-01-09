using System.Collections.Generic;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class ComboBoxInputViewModel : BaseInputViewModel
	{
		#region public properties

		/// <summary>
		/// The list of strings to be items of a combo box
		/// </summary>
		public string[] Items { get; set; }

		/// <summary>
		/// The currently selected item
		/// </summary>
		public int SelectedIndex { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ComboBoxInputViewModel (string labelText, string[] itemNames) : base(labelText)
		{
			Items = itemNames;
			SelectedIndex = 0;
		}

		#endregion

		#region public methods

		public override void Reset ()
		{
			SelectedIndex = 0;
		}

		#endregion
	}
}