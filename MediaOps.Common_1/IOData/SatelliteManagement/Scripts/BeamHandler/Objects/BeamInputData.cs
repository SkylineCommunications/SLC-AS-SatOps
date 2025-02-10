namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler
{
	using System;

	/// <summary>
	/// Represents the input data for a beam.
	/// </summary>
	public class BeamInputData : InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BeamInputData"/> class.
		/// </summary>
		protected BeamInputData()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier for the DOM beam.
		/// </summary>
		public Guid DomBeamId { get; set; }
    }
}
