using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// The view model for a destination item
	/// </summary>
	public class VisitPurposeSelectionListViewModel : BaseVisibleViewModel
	{
		#region private members

		bool isVisible;
		DestinationVisitPurposeSelection model;

		#endregion

		#region public properties

		public DestinationVisitPurposeSelection Model
		{
			get => model;
			set
			{
				model = value;
				ModelChanged();
			}
		}

		public ObservableCollection<VisitPurposeSelectionViewModel> VisitPurposeSelectionVMs { get; private set; }

		public override bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				IoC.CreateTripViewModel.RecalcOverlayVisibility();
			}
		}

		#endregion

		#region commands

		public ICommand CancelCommand { get; private set; }

		public ICommand SelectCommand { get; private set; }

		#endregion

		#region constructor

		public VisitPurposeSelectionListViewModel ()
		{
			VisitPurposeSelectionVMs = new ObservableCollection<VisitPurposeSelectionViewModel>();

			//commands
			CancelCommand = new RelayCommand(CancelCommandMethod);
			SelectCommand = new RelayCommand(SelectCommandMethod);
		}

		#endregion

		#region command methods

		void CancelCommandMethod ()
		{
			Model = null;
		}

		/// <summary>
		/// Add the destination to the trip with selected visit purposes
		/// </summary>
		void SelectCommandMethod ()
		{
			//tell visit purpose selection VM to apply changes to the model
			foreach (var item in VisitPurposeSelectionVMs)
			{
				item.ApplyChanges();
			}

			//tell trip creation VM to select destination
			IoC.CreateTripViewModel.SelectDestination(Model);

			//reset this VM to hide the view
			Model = null;
		}

		#endregion

		#region public methods

		public void ModelChanged ()
		{
			IsVisible = Model != null;

			VisitPurposeSelectionVMs.Clear();
			if (Model != null)
			{
				foreach (var item in Model.VisitPurposes)
				{
					VisitPurposeSelectionVMs.Add(new VisitPurposeSelectionViewModel(this, item));
				}
			}
		}

		/// <summary>
		/// If no visit purpose was selected, automatically check "other"
		/// </summary>
		public void CheckDefaultOtherSelection ()
		{
			VisitPurposeSelectionViewModel otherVM = null;
			int selectedCount = 0;

			foreach (var item in VisitPurposeSelectionVMs)
			{
				//retrieve view model for "other" visit purpose
				if (item.Model.Type == VisitPurposeType.Other)
					otherVM = item;

				//count number of selected purposes
				if (item.IsTempSelected)
					selectedCount++;
			}

			//auto select "other" visit purpose if nothing else was selected
			if (selectedCount == 0 && otherVM != null)
				otherVM.IsTempSelected = true;
		}

		#endregion
	}
}