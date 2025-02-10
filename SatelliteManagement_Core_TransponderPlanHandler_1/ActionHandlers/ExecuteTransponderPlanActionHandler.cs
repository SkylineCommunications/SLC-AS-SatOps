namespace SatelliteManagement_Core_TransponderPlanHandler_1.ActionHandlers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using SatelliteManagementHelpers = Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	internal class ExecuteTransponderPlanActionHandler : IActionHandler
	{
		#region Fields
		private readonly ScriptData scriptData;
		private readonly SatOpsLogger logger;
		private readonly ExecuteTransponderPlanAction inputData;

		private DomApplications.SatelliteManagement.TransponderPlan domTransponderPlan;

		private Lazy<SatelliteManagementHelpers.TransponderPlan> transponderPlanHelperLazy;
		#endregion

		public ExecuteTransponderPlanActionHandler(ScriptData scriptData, SatOpsLogger logger, ExecuteTransponderPlanAction inputData)
		{
			this.scriptData = scriptData ?? throw new ArgumentNullException(nameof(scriptData));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

			TraceData = new MediaOpsTraceData();

			Init();
		}

		#region Properties
		public MediaOpsTraceData TraceData { get; private set; }

		private SatelliteManagementHelpers.TransponderPlan TransponderPlanHelper => transponderPlanHelperLazy.Value;
		#endregion

		#region Methods
		public ActionOutput Execute()
		{
			domTransponderPlan = scriptData.SatelliteManagementHandler.GetTransponderPlanByDomInstanceId(inputData.DomTransponderPlanId) ?? throw new InvalidOperationException($"DOM Transponder Plan with ID '{inputData.DomTransponderPlanId}' does not exist.");

			var actionMethods = new Dictionary<TransponderPlanAction, Action>
			{
				[TransponderPlanAction.Activate] = HandleActivateAction,
				[TransponderPlanAction.Deprecate] = HandleDeprecateAction,
				[TransponderPlanAction.Edit] = HandleEditAction,
			};

			if (!actionMethods.TryGetValue(inputData.TransponderPlanAction, out var action))
			{
				throw new NotSupportedException($"Action '{inputData.TransponderPlanAction}' is not supported.");
			}

			action();

			return null;
		}

		private void HandleActivateAction()
		{
			TransponderPlanHelper.Activate();
		}

		private void HandleDeprecateAction()
		{
			TransponderPlanHelper.Deprecate();
		}

		private void HandleEditAction()
		{
			TransponderPlanHelper.Edit();
		}

		private void Init()
		{
			transponderPlanHelperLazy = new Lazy<SatelliteManagementHelpers.TransponderPlan>(() => GetTransponderPlanHelper());
		}

		private SatelliteManagementHelpers.TransponderPlan GetTransponderPlanHelper()
		{
			return new SatelliteManagementHelpers.TransponderPlan(scriptData.Engine, scriptData.Logger, scriptData.SatelliteManagementHandler, domTransponderPlan);
		}
		#endregion
	}
}
