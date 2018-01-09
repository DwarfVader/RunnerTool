using System;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public static class EnumHelpers
	{
		public static T NextValue<T> (T value) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new Exception("Type is not enum");

			int size = Enum.GetValues(typeof(T)).Length;
			int next = ((int)Convert.ChangeType(value, typeof(int)) + 1) % size;

			return (T)Enum.Parse(typeof(T), next.ToString());
		}

		public static T ValueByIndex<T> (int i) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new Exception("Type is not enum");

			int count = Enum.GetValues(typeof(T)).Length;
			if (i < 0 || i >= count)
				throw new IndexOutOfRangeException("Illegal index");

			return (T)Enum.GetValues(typeof(T)).GetValue(i);
		}

		public static int IndexOf<T> (T element) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new Exception("Type is not enum");

			return Array.IndexOf(Enum.GetValues(typeof(T)), element);
		}
	}
}