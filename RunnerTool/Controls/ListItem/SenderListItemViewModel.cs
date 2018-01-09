using RunnerTool.Core;
using System;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for a sender list item
	/// </summary>
	public class SenderListItemViewModel : BaseListItemViewModel
	{
		#region delegates

		public Action<Sender> LeftClickCallback { get; set; }

		#endregion

		#region public properties

		public Sender Sender { get; set; }

		public override string Name { get => Sender.Name; protected set { } }

		public override string ShortName { get => Sender.ShortName; protected set { } }

		public override Brush Background
		{
			get => (Brush)(new SenderToBackgroundConverter().Convert(Sender, typeof(Brush), null, null));
			protected set { }
		}

		#endregion

		#region constructor

		public SenderListItemViewModel (Sender sender)
		{
			Sender = sender;
		}

		#endregion

		#region command methods

		protected override void LeftClickCommandMethod ()
		{
			LeftClickCallback?.Invoke(Sender);
			IoC.SenderSelectionViewModel.IsVisible = false;
		}

		#endregion
	}
}