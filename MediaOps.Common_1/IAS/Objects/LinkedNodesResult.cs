namespace Skyline.DataMiner.Utils.SatOps.Common.IAS
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents the result of linked nodes operations.
	/// </summary>
	public class LinkedNodesResult
	{
		/// <summary>
		/// Gets the list of IDs of the nodes that were manually added.
		/// </summary>
		public List<string> ManualAdded { get; set; } = new List<string>();

		/// <summary>
		/// Gets the list of IDs of the nodes that were automatically added successfully.
		/// </summary>
		public List<string> AutomaticAddedSucceeded { get; set; } = new List<string>();

		/// <summary>
		/// Gets the list of IDs of the nodes that failed to be added automatically.
		/// </summary>
		public List<string> AutomaticAddedFailed { get; set; } = new List<string>();
	}
}
