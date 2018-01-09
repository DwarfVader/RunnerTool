using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Sender : BaseObservableContent
	{
		#region static methods

		/*public static Sender GetSender (string id)
		{
			return DB.Instance.Senders.First(s => s.ID.Equals(id));
		}*/

		#endregion

		#region public properties

		/// <summary>
		/// The type of this sender
		/// </summary>
		public SenderType Type { get; set; }

		/// <summary>
		/// Name of this sender
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Short name of this sender
		/// </summary>
		public string ShortName { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Sender (SenderType type, string name, string shortName)
		{
			Type = type;
			Name = name;
			ShortName = shortName;
		}

		#endregion
	}
}