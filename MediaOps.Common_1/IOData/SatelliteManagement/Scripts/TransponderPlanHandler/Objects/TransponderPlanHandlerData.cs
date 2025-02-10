namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;

	/// <summary>
	/// Represents the data required for handling transponder plan operations.
	/// </summary>
	public class TransponderPlanHandlerData : ScriptDataBase
	{
		/// <summary>
		/// Gets or sets the input data for the transponder plan handler.
		/// </summary>
		public InputData Input { get; set; }

		/// <summary>
		/// Gets the output data from the transponder plan handler.
		/// </summary>
		public OutputData Output { get; } = new OutputData();
	}
}
