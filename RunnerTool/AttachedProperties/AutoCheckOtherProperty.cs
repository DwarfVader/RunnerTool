using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunnerTool
{
	/// <summary>
	/// A property to automatically set the "other" visit purpose if no other purpose was selected
	/// </summary>
	public class AutoCheckOtherProperty : BaseAttachedProperty<AutoCheckOtherProperty, bool>
	{
		public override void OnValueChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			//ensure sender is of correct type
			if (!(sender is VisitPurposeSelectionControl control))
				return;

		}
	}
}