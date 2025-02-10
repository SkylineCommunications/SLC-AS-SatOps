namespace Skyline.DataMiner.Utils.SatOps.Common.Exceptions
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages;

	public class BulkCrudFailedException<T> : CrudFailedException where T : IEquatable<T>
	{
		public BulkCrudFailedException(IBulkOperationResult<T> result) : base(result.GetTraceData())
		{
			Result = result ?? throw new ArgumentNullException(nameof(result));
		}

		public IBulkOperationResult<T> Result { get; }

		public IList<T> SuccessfulIds => Result.SuccessfulIds;

		public IList<T> UnsuccessfulIds => Result.UnsuccessfulIds;

		public IDictionary<T, TraceData> TraceDataPerItem => Result.TraceDataPerItem;
	}
}
