namespace Skyline.DataMiner.Utils.SatOps.Common.Extensions
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;

	public static class StringExtensions
	{
		public static string Wrap(this string text, int width, bool wrapWords = false)
		{
			var result = new StringBuilder();

			using (var sr = new StringReader(text))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (result.Length > 0)
					{
						result.AppendLine();
					}

					result.Append(WrapLine(line, width, wrapWords));
				}
			}

			return result.ToString();
		}

		public static string WrapLine(this string text, int width, bool wrapWords)
		{
			var result = new StringBuilder();
			var line = new StringBuilder();

			var words = text.Split(' ');

			foreach (var word in words)
			{
				if (line.Length + word.Length >= width)
				{
					if (wrapWords && word.Length > width)
					{
						if (line.Length > 0)
							line.Append(" ");

						var remainingLength = width - line.Length - 3;
						var firstPart = word.Substring(0, remainingLength);
						var otherPart = word.Substring(firstPart.Length);

						var trailingWidth = width - 3;
						var parts = Enumerable.Range(0, (int)Math.Ceiling(Convert.ToDouble(otherPart.Length) / trailingWidth)).Select(i => (i * trailingWidth + trailingWidth) <= otherPart.Length ? otherPart.Substring(i * trailingWidth, trailingWidth) : otherPart.Substring(i * trailingWidth)).ToList();

						line.Append($"{firstPart}...");
						result.Append(line.ToString());

						for (int i = 0; i < parts.Count; i++)
						{
							if (i == parts.Count - 1)
							{
								line.Clear();
								line.Append(parts[i]);
							}
							else
							{
								result.AppendLine();
								result.Append($"{parts[i]}...");
							}
						}

						continue;
					}
					else
					{
						if (result.Length > 0)
							result.AppendLine();
						result.Append(line.ToString());
						line.Clear();
					}
				}

				if (line.Length > 0)
					line.Append(" ");

				line.Append(word);
			}

			if (line.Length > 0)
			{
				if (result.Length > 0)
					result.AppendLine();
				result.Append(line.ToString());
			}

			return result.ToString();
		}

		/// <summary>
		/// Truncates a string to first maxLength characthers, and then adds an ellipsis at the end.
		/// If the input string shorter than maxLEngth, returns the same string.
		/// If the input is null, returns null.
		/// </summary>
		/// <param name="text">Input string to be truncated.</param>
		/// <param name="maxLength">Maximum length of the string to be truncated.</param>
		/// <returns>If the string is truncated, returns truncated string with additional ellipsis at the end, with total length of maxLength + 3.</returns>
		public static string TruncateWithEllipsis(this string text, int maxLength)
		{
			if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
			{
				return text;
			}
			else
			{
				return text.Substring(0, maxLength) + "...";
			}
		}
	}
}
