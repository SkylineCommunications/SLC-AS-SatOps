namespace SatelliteManagement_Core_SatelliteHandler_1.ActionHandlers
{
	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler;

	internal interface IActionHandler
	{
		MediaOpsTraceData TraceData { get; }

		ActionOutput Execute();
	}
}
