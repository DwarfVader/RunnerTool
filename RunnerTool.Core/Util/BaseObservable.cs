using System.ComponentModel;

namespace RunnerTool.Core
{
	/// <summary>
	/// A base class for observable classes
	/// </summary>
	public abstract class BaseObservable : INotifyPropertyChanged
	{
		/// <summary>
		/// The event that is fired when any child property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		/// <summary>
		/// Call this to fire a <see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="name"></param>
		public void OnPropertyChanged (string name)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}

	public abstract class BaseObservableContent : INotifyPropertyChangedContent
	{
		/// <summary>
		/// The event that is fired when any child property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		/// <summary>
		/// Call this to fire a <see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="name"></param>
		public void OnPropertyChanged (string name)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}