using PropertyChanged;
using RunnerTool.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for a sender list item
	/// </summary>
	public class DestinationListItemViewModel : BaseListItemViewModel
	{
		#region delegates

		public Action<Destination> LeftClickCallback { get; set; }

		#endregion

		#region public properties

		public Destination Destination { get; set; }

		public override string Name { get => Destination?.Name; protected set { } }

		public override string ShortName { get => Destination?.ShortName; protected set { } }

		public override Brush Background
		{
			get => (Brush)(new DestinationTypeToBackgroundConverter().Convert(Destination, typeof(Brush), null, null));
			protected set { }
		}

		#endregion

		#region constructor

		public DestinationListItemViewModel (Destination destination)
		{
			Destination = destination;
		}

		#endregion

		#region command methods

		protected override void LeftClickCommandMethod ()
		{
			LeftClickCallback?.Invoke(Destination);
			IoC.DestinationSelectionViewModel.IsVisible = false;
		}

		#endregion
	}
}