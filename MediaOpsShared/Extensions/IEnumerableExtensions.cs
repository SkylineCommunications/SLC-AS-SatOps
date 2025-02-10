namespace Skyline.DataMiner.Utils.SatOps.Common.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return source.SelectMany(x => x);
		}
	}
}
