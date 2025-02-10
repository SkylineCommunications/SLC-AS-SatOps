namespace Skyline.DataMiner.Utils.SatOps.Common.DOM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.IManager.Objects;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	using SLDataGateway.API.Types.Querying;

	public static class CrudHelperComponentExtensions
	{
		public static IEnumerable<ICollection<T>> ReadPaged<T>(this ICrudHelperComponent<T> helper, FilterElement<T> filter, long pageSize = 500)
			where T : DataType
		{
			if (helper == null)
			{
				throw new ArgumentNullException(nameof(helper));
			}

			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return ReadPagedIterator(helper, filter, pageSize);
		}

		public static IEnumerable<ICollection<T>> ReadPaged<T>(this ICrudHelperComponent<T> helper, IQuery<T> query, long pageSize = 500)
			where T : DataType
		{
			if (helper == null)
			{
				throw new ArgumentNullException(nameof(helper));
			}

			if (query == null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			return ReadPagedIterator(helper, query, pageSize);
		}

		public static IEnumerable<ICollection<T>> ReadAllPaged<T>(this ICrudHelperComponent<T> helper, long pageSize = 500)
			where T : DataType
		{
			if (helper == null)
			{
				throw new ArgumentNullException(nameof(helper));
			}

			return ReadPaged(helper, new TRUEFilterElement<T>(), pageSize);
		}

		public static BulkCreateOrUpdateResult<T, K> CreateOrUpdateInBatches<T, K>(this IBulkCrudHelperComponent<T, K> helper, IEnumerable<T> instances)
			where T : IManagerIdentifiableObject<K>, DataType
			where K : IEquatable<K>
		{
			if (helper == null)
			{
				throw new ArgumentNullException(nameof(helper));
			}

			if (instances == null)
			{
				throw new ArgumentNullException(nameof(instances));
			}

			var successfulItems = new List<T>();
			var unsuccessfulIds = new List<K>();
			var traceDataPerItem = new Dictionary<K, TraceData>();

			foreach (var batch in instances.Batch(100))
			{
				var batchResult = helper.CreateOrUpdate(batch.ToList());

				successfulItems.AddRange(batchResult.SuccessfulItems);
				unsuccessfulIds.AddRange(batchResult.UnsuccessfulIds);

				foreach (var item in batchResult.TraceDataPerItem)
				{
					traceDataPerItem[item.Key] = item.Value;
				}
			}

			return new BulkCreateOrUpdateResult<T, K>(successfulItems, unsuccessfulIds, traceDataPerItem);
		}

		public static BulkDeleteResult<K> DeleteInBatches<T, K>(this IBulkCrudHelperComponent<T, K> helper, IEnumerable<T> instances)
			where T : IManagerIdentifiableObject<K>, DataType
			where K : IEquatable<K>
		{
			if (helper == null)
			{
				throw new ArgumentNullException(nameof(helper));
			}

			if (instances == null)
			{
				throw new ArgumentNullException(nameof(instances));
			}

			var successfulIds = new List<K>();
			var unsuccessfulIds = new List<K>();
			var traceDataPerItem = new Dictionary<K, TraceData>();

			foreach (var batch in instances.Batch(100))
			{
				var batchResult = helper.Delete(batch.ToList());

				successfulIds.AddRange(batchResult.SuccessfulIds);
				unsuccessfulIds.AddRange(batchResult.UnsuccessfulIds);

				foreach (var item in batchResult.TraceDataPerItem)
				{
					traceDataPerItem[item.Key] = item.Value;
				}
			}

			return new BulkDeleteResult<K>(successfulIds, unsuccessfulIds, traceDataPerItem);
		}

		private static IEnumerable<ICollection<T>> ReadPagedIterator<T>(ICrudHelperComponent<T> helper, FilterElement<T> filter, long pageSize) where T : DataType
		{
			var pagingHelper = helper.PreparePaging(filter, pageSize);

			return ReadPagedIterator(pagingHelper);
		}

		private static IEnumerable<ICollection<T>> ReadPagedIterator<T>(ICrudHelperComponent<T> helper, IQuery<T> query, long pageSize) where T : DataType
		{
			var pagingHelper = helper.PreparePaging(query, pageSize);

			return ReadPagedIterator(pagingHelper);
		}

		private static IEnumerable<ICollection<T>> ReadPagedIterator<T>(PagingHelper<T> pagingHelper) where T : DataType
		{
			while (pagingHelper.HasNextPage())
			{
				pagingHelper.MoveToNextPage();

				yield return pagingHelper.GetCurrentPage();
			}
		}
	}
}
