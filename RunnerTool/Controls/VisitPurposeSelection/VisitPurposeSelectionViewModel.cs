using RunnerTool.Core;
using System.Collections.Generic;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class VisitPurposeSelectionViewModel : BaseViewModel
	{
		#region public properties

		/// <summary>
		/// The view model of the parenting user control
		/// </summary>
		public VisitPurposeSelectionListViewModel Parent { get; private set; }

		/// <summary>
		/// The chosen selection on a visit purpose for a specific destination for a specific sender. 
		/// NOT referencing to the template selection in the actual model
		/// </summary>
		public VisitPurposeSelection Model { get; private set; }

		/// <summary>
		/// The temporary selection in the <see cref="VisitPurposeSelectionListControl"/>. 
		/// Changes need to applied onto <see cref="Model"/>
		/// </summary>
		public bool IsTempSelected { get; set; }

		#endregion

		#region commands

		public ICommand CheckCommand { get; private set; }

		#endregion

		#region constructor

		public VisitPurposeSelectionViewModel (VisitPurposeSelectionListViewModel parent, VisitPurposeSelection model)
		{
			Parent = parent;
			Model = model;
			IsTempSelected = model.IsSelected;

			//commands
			CheckCommand = new RelayCommand(CheckCommandMethod);
		}

		#endregion

		#region command methods

		void CheckCommandMethod ()
		{
			//toggle selection
			IsTempSelected = !IsTempSelected;

			//check for at least one selected visit purpose
			Parent.CheckDefaultOtherSelection();
		}

		#endregion

		#region public methods

		public void ApplyChanges ()
		{
			Model.IsSelected = IsTempSelected;
		}

		#endregion
	}
}