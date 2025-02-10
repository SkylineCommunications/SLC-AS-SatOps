namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler
{
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;
	using Skyline.DataMiner.MediaOps.Communication.TraceData;

	/// <summary>
	/// Represents the output data of an action.
	/// </summary>
	public class OutputData : ScriptDataOutputBase
	{
		/// <summary>
		/// Gets or sets the output of the action.
		/// </summary>
		public ActionOutput ActionOutput { get; set; }

		/// <summary>
		/// Gets or sets the trace data for media operations.
		/// </summary>
		public MediaOpsTraceData TraceData { get; set; } = new MediaOpsTraceData();
	}
}
