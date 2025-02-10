namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler
{
	using System;

	/// <summary>
	/// Represents the input data for a satellite.
	/// </summary>
	public class SatelliteInputData : InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SatelliteInputData"/> class.
		/// </summary>
		protected SatelliteInputData()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier for the DOM satellite.
		/// </summary>
		public Guid DomSatelliteId { get; set; }
	}
}
