namespace SatelliteManagement_Core_TransponderHandler_1.ActionHandlers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using SatelliteManagementHelpers = Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	internal class ExecuteTransponderActionHandler : IActionHandler
	{
		#region Fields
		private readonly ScriptData scriptData;

		private readonly ExecuteTransponderAction inputData;

		private DomApplications.SatelliteManagement.Transponder domTransponder;

		private Lazy<SatelliteManagementHelpers.Transponder> transponderHelperLazy;
		#endregion

		public ExecuteTransponderActionHandler(ScriptData scriptData, ExecuteTransponderAction inputData)
		{
			this.scriptData = scriptData ?? throw new ArgumentNullException(nameof(scriptData));
			this.inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

			TraceData = new MediaOpsTraceData();

			Init();
		}

		#region Properties
		public MediaOpsTraceData TraceData { get; private set; }

		private SatelliteManagementHelpers.Transponder TransponderHelper => transponderHelperLazy.Value;
		#endregion

		#region Methods
		public ActionOutput Execute()
		{
			domTransponder = scriptData.SatelliteManagementHandler.GetTransponderByDomInstanceId(inputData.DomTransponderId) ?? throw new InvalidOperationException($"DOM Transponder with ID '{inputData.DomTransponderId}' does not exist.");

			var actionMethods = new Dictionary<TransponderAction, Action>
			{
				[TransponderAction.Activate] = HandleActivateAction,
				[TransponderAction.Deprecate] = HandleDeprecateAction,
				[TransponderAction.Edit] = HandleEditAction,
			};

			if (!actionMethods.TryGetValue(inputData.TransponderAction, out var action))
			{
				throw new NotSupportedException($"Action '{inputData.TransponderAction}' is not supported.");
			}

			action();

			return null;
		}

		private void HandleActivateAction()
		{
			TransponderHelper.Activate();
		}

		private void HandleDeprecateAction()
		{
			TransponderHelper.Deprecate();
		}

		private void HandleEditAction()
		{
			TransponderHelper.Edit();
		}

		private void Init()
		{
			transponderHelperLazy = new Lazy<SatelliteManagementHelpers.Transponder>(() => GetTransponderHelper());
		}

		private SatelliteManagementHelpers.Transponder GetTransponderHelper()
		{
			return new SatelliteManagementHelpers.Transponder(scriptData.Engine, scriptData.Logger, scriptData.SatelliteManagementHandler, domTransponder);
		}
		#endregion
	}
}
