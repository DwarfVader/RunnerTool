using System.Linq;

namespace RunnerTool.Core
{
	/// <summary>
	/// The type of a sender
	/// </summary>
	public class SenderType
	{
		#region static methods

		public static SenderType GetByOrder (int order)
		{
			return DB.Instance.SenderTypes.First(s => s.Order == order);
		}

		#endregion

		public int Order { get; set; }

		public string Name { get; set; }

		public SenderType (int order, string name)
		{
			Order = order;
			Name = name;
		}
	}
}