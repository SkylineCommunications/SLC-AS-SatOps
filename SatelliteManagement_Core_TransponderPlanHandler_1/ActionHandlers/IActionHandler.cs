namespace SatelliteManagement_Core_TransponderPlanHandler_1.ActionHandlers
{
	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler;

	internal interface IActionHandler
	{
		MediaOpsTraceData TraceData { get; }

		ActionOutput Execute();
	}
}
