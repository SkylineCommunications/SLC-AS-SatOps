namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler
{
	using System;

	/// <summary>
	/// Represents the input data for a transponder plan.
	/// </summary>
	public class TransponderPlanInputData : InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransponderPlanInputData"/> class.
		/// </summary>
		protected TransponderPlanInputData()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier for the DOM transponder plan.
		/// </summary>
		public Guid DomTransponderPlanId { get; set; }
	}
}
