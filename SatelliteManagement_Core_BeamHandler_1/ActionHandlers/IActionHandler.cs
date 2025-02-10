namespace SatelliteManagement_Core_BeamHandler_1.ActionHandlers
{
	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler;

	internal interface IActionHandler
	{
		MediaOpsTraceData TraceData { get; }

		ActionOutput Execute();
	}
}
