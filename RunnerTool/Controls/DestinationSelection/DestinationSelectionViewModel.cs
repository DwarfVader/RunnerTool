using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class DestinationSelectionViewModel : BaseVisibleViewModel
	{
		#region private members

		Action<Destination> itemLeftClickCallback;
		bool isVisible;

		#endregion

		#region public properties

		public Action<Destination> ItemLeftClickCallback
		{
			get => itemLeftClickCallback;
			set
			{
				itemLeftClickCallback = value;

				PropagateItemCallbacks();
			}
		}

		//public List<Destination> Destinations { get; private set; }
		public ObservableCollection<DestinationListItemViewModel> DestinationVMs { get; private set; }

		public override bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;

				IoC.TripListViewModel.RecalcOverlayVisibility();
			}
		}

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public DestinationSelectionViewModel ()
		{
			DestinationVMs = new ObservableCollection<DestinationListItemViewModel>();
			ReloadDestinations();

			//register events
			DB.Instance.OnDestinationsChanged += ReloadDestinations;
		}

		~DestinationSelectionViewModel ()
		{
			//unregister events
			DB.Instance.OnDestinationsChanged -= ReloadDestinations;
		}

		#endregion

		#region command methods

		//TODO: falls out of scheme of framehistory handled views... keep it that way?
		protected override void CloseCommandMethod ()
		{
			IsVisible = false;
		}

		#endregion

		#region private methods

		void PropagateItemCallbacks ()
		{
			foreach (var item in DestinationVMs)
			{
				item.LeftClickCallback = ItemLeftClickCallback;
			}
		}

		void ReloadDestinations ()
		{
			//TODO: either reload whole trip list control or update items here without creating new objects each time! solved by fody???

			DestinationVMs.Clear();
			foreach (var destination in DB.Instance.Destinations)
			{
				DestinationVMs.Add(new DestinationListItemViewModel(destination)
				{
					LeftClickCallback = ItemLeftClickCallback
				});
			}
		}

		#endregion
	}
}