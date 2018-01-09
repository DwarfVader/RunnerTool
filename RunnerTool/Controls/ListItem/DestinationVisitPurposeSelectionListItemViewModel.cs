using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for a destination list item
	/// </summary>
	public class DestinationVisitPurposeSelectionListItemViewModel : BaseListItemViewModel
	{
		#region public properties

		public DestinationVisitPurposeSelection Model { get; set; }

		public bool IsInSelectedList { get; set; }

		public override string Name { get => Model?.Destination.Name; protected set { } }

		public override string ShortName { get => Model?.Destination.ShortName; protected set { } }

		public override Brush Background
		{
			get => (Brush)(new DestinationTypeToBackgroundConverter().Convert(Model?.Destination, typeof(Brush), null, null));
			protected set { }
		}

		#endregion

		#region constructor

		public DestinationVisitPurposeSelectionListItemViewModel (DestinationVisitPurposeSelection model, bool isInSelectedList = false)
		{
			Model = model;
			IsInSelectedList = isInSelectedList;
		}

		#endregion

		#region command methods

		/// <summary>
		/// Opens the visit purpose selection list view
		/// </summary>
		protected override void LeftClickCommandMethod ()
		{
			IoC.VisitPurposeSelectionListViewModel.Model = Model;
		}

		/// <summary>
		/// Directly adds/removes the item to/from the selected list
		/// </summary>
		protected override void RightClickCommandMethod ()
		{
			if (IsInSelectedList)
				IoC.CreateTripViewModel.UnselectDestination(Model);
			else
				IoC.CreateTripViewModel.SelectDestination(Model);
		}

		#endregion
	}
}