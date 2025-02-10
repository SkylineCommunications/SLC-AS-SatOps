namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler
{
	using System;

	/// <summary>
	/// Represents the input data for a transponder.
	/// </summary>
	public class TransponderInputData : InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransponderInputData"/> class.
		/// </summary>
		protected TransponderInputData()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier for the DOM transponder.
		/// </summary>
		public Guid DomTransponderId { get; set; }
	}
}
