namespace SatelliteManagement_Core_SatelliteHandler_1.ActionHandlers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using SatelliteManagementHelpers = Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	internal class ExecuteSatelliteActionHandler : IActionHandler
	{
		#region Fields
		private readonly ScriptData scriptData;

		private readonly ExecuteSatelliteAction inputData;

		private DomApplications.SatelliteManagement.Satellite domSatellite;

		private Lazy<SatelliteManagementHelpers.Satellite> satelliteHelperLazy;
		#endregion

		public ExecuteSatelliteActionHandler(ScriptData scriptData, ExecuteSatelliteAction inputData)
		{
			this.scriptData = scriptData ?? throw new ArgumentNullException(nameof(scriptData));
			this.inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

			TraceData = new MediaOpsTraceData();

			Init();
		}

		#region Properties
		public MediaOpsTraceData TraceData { get; private set; }

		private SatelliteManagementHelpers.Satellite SatelliteHelper => satelliteHelperLazy.Value;
		#endregion

		#region Methods
		public ActionOutput Execute()
		{
			domSatellite = scriptData.SatelliteManagementHandler.GetSatelliteByDomInstanceId(inputData.DomSatelliteId) ?? throw new InvalidOperationException($"DOM Satellite with ID '{inputData.DomSatelliteId}' does not exist.");

			var actionMethods = new Dictionary<SatelliteAction, Action>
			{
				[SatelliteAction.Activate] = HandleActivateAction,
				[SatelliteAction.Deprecate] = HandleDeprecateAction,
				[SatelliteAction.Edit] = HandleEditAction,
			};

			if (!actionMethods.TryGetValue(inputData.SatelliteAction, out var action))
			{
				throw new NotSupportedException($"Action '{inputData.SatelliteAction}' is not supported.");
			}

			action();

			return null;
		}

		private void HandleActivateAction()
		{
			SatelliteHelper.Activate();
		}

		private void HandleDeprecateAction()
		{
			SatelliteHelper.Deprecate();
		}

		private void HandleEditAction()
		{
			SatelliteHelper.Edit();
		}

		private void Init()
		{
			satelliteHelperLazy = new Lazy<SatelliteManagementHelpers.Satellite>(() => GetSatelliteHelper());
		}

		private SatelliteManagementHelpers.Satellite GetSatelliteHelper()
		{
			return new SatelliteManagementHelpers.Satellite(scriptData.Engine, scriptData.Logger, scriptData.SatelliteManagementHandler, domSatellite);
		}
		#endregion
	}
}
