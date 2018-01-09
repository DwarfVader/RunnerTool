using RunnerTool.Core;
using System.Windows.Input;
using System.Windows.Media;

namespace RunnerTool
{
	/// <summary>
	/// The view model for a sender list item
	/// </summary>
	public abstract class BaseListItemViewModel : BaseViewModel
	{
		#region public properties

		public abstract string Name { get; protected set; }

		public abstract string ShortName { get; protected set; }

		public abstract Brush Background { get; protected set; }

		public virtual Brush Foreground { get; protected set; }

		public virtual Brush BorderColor { get; protected set; }

		public virtual float BorderThickness { get; protected set; }

		#endregion

		#region commands

		public virtual ICommand LeftClickCommand { get; protected set; }
		public virtual ICommand RightClickCommand { get; protected set; }

		#endregion

		#region constructor

		public BaseListItemViewModel ()
		{
			//default values, in case they are not getting overridden
			Foreground = Brushes.Black;
			BorderColor = Brushes.Transparent;
			BorderThickness = 0;

			//commands
			LeftClickCommand = new RelayCommand(LeftClickCommandMethod);
			RightClickCommand = new RelayCommand(RightClickCommandMethod);
		}

		#endregion

		#region command methods

		protected virtual void LeftClickCommandMethod () { }

		protected virtual void RightClickCommandMethod () { }

		#endregion
	}
}