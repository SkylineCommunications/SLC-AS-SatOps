namespace SatelliteManagement_Core_SlotHandler_1.ActionHandlers
{
	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.SlotHandler;

	internal interface IActionHandler
	{
		MediaOpsTraceData TraceData { get; }

		ActionOutput Execute();
	}
}
