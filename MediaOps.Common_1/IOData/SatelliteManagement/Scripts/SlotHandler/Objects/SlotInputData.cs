namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SlotHandler
{
	using System;

	/// <summary>
	/// Represents the input data for a slot.
	/// </summary>
	public class SlotInputData : InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SlotInputData"/> class.
		/// </summary>
		protected SlotInputData()
		{
		}

		/// <summary>
		/// Gets or sets the unique identifier for the DOM slot.
		/// </summary>
		public Guid DomSlotId { get; set; }
	}
}
