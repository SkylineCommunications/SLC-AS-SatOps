namespace Skyline.DataMiner.Utils.SatOps.Common.Tools
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public static class FilterQueryExecutor
	{
		public static IEnumerable<T> RetrieveFilteredItems<T, TId>(IEnumerable<TId> ids, Func<TId, FilterElement<T>> filterProvider, Func<FilterElement<T>, IEnumerable<T>> filterResolver)
		{
			if (ids == null)
			{
				throw new ArgumentNullException(nameof(ids));
			}

			if (filterProvider == null)
			{
				throw new ArgumentNullException(nameof(filterProvider));
			}

			if (filterResolver == null)
			{
				throw new ArgumentNullException(nameof(filterResolver));
			}

			return RetrieveFilteredItemsIterator(ids, filterProvider, filterResolver);
		}

		private static IEnumerable<T> RetrieveFilteredItemsIterator<T, TId>(IEnumerable<TId> ids, Func<TId, FilterElement<T>> filterProvider, Func<FilterElement<T>, IEnumerable<T>> filterResolver)
		{
			var batch = new List<FilterElement<T>>();
			var count = 0;

			// max_clause_count is by default set to minimum 1024
			// https://www.elastic.co/guide/en/elasticsearch/reference/7.17/search-settings.html
			const int limit = 1000;

			int CountSubFilters(FilterElement<T> filter)
			{
				switch (filter)
				{
					case ANDFilterElement<T> and: return and.subFilters.Sum(CountSubFilters);
					case ORFilterElement<T> or: return or.subFilters.Sum(CountSubFilters);
					default: return 1;
				}
			}

			foreach (var id in ids.Distinct())
			{
				var filter = filterProvider(id);
				var subfilterCount = filter.flatten().Sum(CountSubFilters);

				if (count + subfilterCount > limit && batch.Any())
				{
					foreach (var item in filterResolver(new ORFilterElement<T>(batch.ToArray())))
					{
						yield return item;
					}

					batch.Clear();
					count = 0;
				}

				batch.Add(filter);
				count += subfilterCount;
			}

			// don't forget the last items
			if (batch.Any())
			{
				foreach (var item in filterResolver(new ORFilterElement<T>(batch.ToArray())))
				{
					yield return item;
				}
			}
		}
	}
}
