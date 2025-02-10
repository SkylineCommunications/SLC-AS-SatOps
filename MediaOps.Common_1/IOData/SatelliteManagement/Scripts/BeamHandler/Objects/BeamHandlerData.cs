namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;

	/// <summary>
	/// Represents the data required for handling beam operations.
	/// </summary>
	public class BeamHandlerData : ScriptDataBase
	{
		/// <summary>
		/// Gets or sets the input data for the beam handler.
		/// </summary>
		public InputData Input { get; set; }

		/// <summary>
		/// Gets the output data from the beam handler.
		/// </summary>
		public OutputData Output { get; } = new OutputData();
	}
}
