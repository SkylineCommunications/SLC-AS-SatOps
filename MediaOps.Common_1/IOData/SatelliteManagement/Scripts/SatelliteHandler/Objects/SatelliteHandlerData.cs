namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;

	/// <summary>
	/// Represents the data required for handling satellite operations.
	/// </summary>
	public class SatelliteHandlerData : ScriptDataBase
	{
		/// <summary>
		/// Gets or sets the input data for the satellite handler.
		/// </summary>
		public InputData Input { get; set; }

		/// <summary>
		/// Gets the output data from the satellite handler.
		/// </summary>
		public OutputData Output { get; } = new OutputData();
	}
}
