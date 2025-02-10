namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;

	/// <summary>
	/// Represents the data required for handling transponder operations.
	/// </summary>
	public class TransponderHandlerData : ScriptDataBase
	{
		/// <summary>
		/// Gets or sets the input data for the transponder handler.
		/// </summary>
		public InputData Input { get; set; }

		/// <summary>
		/// Gets the output data from the transponder handler.
		/// </summary>
		public OutputData Output { get; } = new OutputData();
	}
}
