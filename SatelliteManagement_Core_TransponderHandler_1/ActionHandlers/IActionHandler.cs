namespace SatelliteManagement_Core_TransponderHandler_1.ActionHandlers
{
	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler;

	internal interface IActionHandler
	{
		MediaOpsTraceData TraceData { get; }

		ActionOutput Execute();
	}
}
