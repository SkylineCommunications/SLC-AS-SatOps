namespace SatelliteManagement_Core_SlotHandler_1.ActionHandlers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.SlotHandler;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using SatelliteManagementHelpers = Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	internal class ExecuteSlotActionHandler : IActionHandler
	{
		#region Fields
		private readonly ScriptData scriptData;

		private readonly ExecuteSlotAction inputData;

		private DomApplications.SatelliteManagement.Slot domSlot;

		private Lazy<SatelliteManagementHelpers.Slot> slotHelperLazy;
		#endregion

		public ExecuteSlotActionHandler(ScriptData scriptData, ExecuteSlotAction inputData)
		{
			this.scriptData = scriptData ?? throw new ArgumentNullException(nameof(scriptData));
			this.inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

			TraceData = new MediaOpsTraceData();

			Init();
		}

		#region Properties
		public MediaOpsTraceData TraceData { get; private set; }

		private SatelliteManagementHelpers.Slot SlotHelper => slotHelperLazy.Value;
		#endregion

		#region Methods
		public ActionOutput Execute()
		{
			domSlot = scriptData.SatelliteManagementHandler.GetSlotByDomInstanceId(inputData.DomSlotId) ?? throw new InvalidOperationException($"DOM Slot with ID '{inputData.DomSlotId}' does not exist.");

			var actionMethods = new Dictionary<SlotAction, Action>
			{
				[SlotAction.Activate] = HandleActivateAction,
				[SlotAction.Deprecate] = HandleDeprecateAction,
			};

			if (!actionMethods.TryGetValue(inputData.SlotAction, out var action))
			{
				throw new NotSupportedException($"Action '{inputData.SlotAction}' is not supported.");
			}

			action();

			return null;
		}

		private void HandleActivateAction()
		{
			SlotHelper.Activate();
		}

		private void HandleDeprecateAction()
		{
			SlotHelper.Deprecate();
		}

		private void Init()
		{
			slotHelperLazy = new Lazy<SatelliteManagementHelpers.Slot>(() => GetSlotHelper());
		}

		private SatelliteManagementHelpers.Slot GetSlotHelper()
		{
			return new SatelliteManagementHelpers.Slot(scriptData.Engine, scriptData.Logger, scriptData.SatelliteManagementHandler, domSlot);
		}
		#endregion
	}
}
