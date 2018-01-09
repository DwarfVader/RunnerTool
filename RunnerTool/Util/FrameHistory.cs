using System.Collections.Generic;

namespace RunnerTool
{
	/// <summary>
	/// Handles call hierarchies on frames/user controls to show the 
	/// previous frame upon closing another. 
	/// </summary>
	public static class FrameHistory
	{
		#region public properties

		/// <summary>
		/// A call stack of view models belonging to opened frames. 
		/// New elements go to the front. 
		/// </summary>
		private static Stack<BaseVisibleViewModel> History { get; set; } = new Stack<BaseVisibleViewModel>();

		#endregion

		#region public methods

		public static void OpenFrame (BaseVisibleViewModel viewModel, bool clearStack = false)
		{
			if (clearStack)
			{
				while (History.Count > 0)
				{
					History.Pop().IsVisible = false;
				}
			}
			else
			{
				if (History.Count > 0)
					History.Peek().IsVisible = false;
			}

			History.Push(viewModel);
			viewModel.IsVisible = true;
		}

		/// <summary>
		/// Get rid of the latest viewmodel
		/// </summary>
		public static void CloseFrame ()
		{
			//hide frame and remove from stack
			History.Pop().IsVisible = false;

			//show previous frame, if existing
			if (History.Count > 0)
				History.Peek().IsVisible = true;
		}

		#endregion
	}
}