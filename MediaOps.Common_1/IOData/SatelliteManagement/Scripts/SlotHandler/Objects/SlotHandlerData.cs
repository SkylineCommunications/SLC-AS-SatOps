namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SlotHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;

	/// <summary>
	/// Represents the data required for handling slot operations.
	/// </summary>
	public class SlotHandlerData : ScriptDataBase
	{
		/// <summary>
		/// Gets or sets the input data for the slot handler.
		/// </summary>
		public InputData Input { get; set; }

		/// <summary>
		/// Gets the output data from the slot handler.
		/// </summary>
		public OutputData Output { get; } = new OutputData();
	}
}
