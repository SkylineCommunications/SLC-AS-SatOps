namespace Skyline.DataMiner.Utils.SatOps.Common.Extensions
{
	using System;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public static class FilterExtensions
	{
		public static bool IsMatch<T>(this FilterElement<T> filter, T instance)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			var lambda = filter.getLambda();
			return lambda(instance);
		}

		public static bool IsMatch<T>(this IQuery<T> query, T instance)
		{
			if (query == null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			var filter = query.Filter;
			return filter.IsMatch(instance);
		}
	}
}