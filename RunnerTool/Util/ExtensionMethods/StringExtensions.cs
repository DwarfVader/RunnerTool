using RunnerTool.Core;
using System.Collections.Generic;
using System.Linq;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Creates a new string based on input with the first character 
		/// being lower case
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ToLowerFirst (this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			var charArray = input.ToCharArray();
			charArray[0] = char.ToLower(charArray[0]);

			return new string(charArray);
		}
	}
}