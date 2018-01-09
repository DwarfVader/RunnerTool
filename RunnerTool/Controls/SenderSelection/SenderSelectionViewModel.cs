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
	public class SenderSelectionViewModel : BaseVisibleViewModel
	{
		#region private members

		Action<Sender> itemLeftClickCallback;
		bool isVisible;

		#endregion

		#region public properties

		public Action<Sender> ItemLeftClickCallback
		{
			get => itemLeftClickCallback;
			set
			{
				itemLeftClickCallback = value;

				PropagateItemCallbacks();
			}
		}

		//public List<Sender> Senders { get; private set; }
		public ObservableCollection<SenderListItemViewModel> SenderVMs { get; private set; }

		public override bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;

				IoC.CreateTripViewModel.RecalcOverlayVisibility();
				IoC.TripListViewModel.RecalcOverlayVisibility(); //TODO: a better solution?
			}
		}

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SenderSelectionViewModel ()
		{
			SenderVMs = new ObservableCollection<SenderListItemViewModel>();

			//intiallly loads the whole sender list
			UpdateSenderList();

			//register events
			DB.Instance.OnSendersChanged += UpdateSenderList;
		}

		~SenderSelectionViewModel ()
		{
			//unregister events
			DB.Instance.OnSendersChanged -= UpdateSenderList;
		}

		#endregion

		#region command methods

		//TODO: falls out of scheme of frame history handled views
		protected override void CloseCommandMethod ()
		{
			IsVisible = false;
		}

		#endregion

		#region private methods

		void PropagateItemCallbacks ()
		{
			foreach (var item in SenderVMs)
			{
				item.LeftClickCallback = ItemLeftClickCallback;
			}
		}

		void UpdateSenderList ()
		{
			//TODO: currently just reloading; improve to a real update

			SenderVMs.Clear();
			foreach (var sender in DB.Instance.Senders)
			{
				SenderVMs.Add(new SenderListItemViewModel(sender)
				{
					LeftClickCallback = ItemLeftClickCallback
				});
			}
		}

		#endregion
	}
}