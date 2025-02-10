namespace SatelliteManagement_Core_BeamHandler_1.ActionHandlers
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.MediaOps.Communication.TraceData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using SatelliteManagementHelpers = Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	internal class ExecuteBeamActionHandler : IActionHandler
	{
		#region Fields
		private readonly ScriptData scriptData;

		private readonly ExecuteBeamAction inputData;

		private DomApplications.SatelliteManagement.Beam domBeam;

		private Lazy<SatelliteManagementHelpers.Beam> beamHelperLazy;
		#endregion

		public ExecuteBeamActionHandler(ScriptData scriptData, ExecuteBeamAction inputData)
		{
			this.scriptData = scriptData ?? throw new ArgumentNullException(nameof(scriptData));
			this.inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

			TraceData = new MediaOpsTraceData();

			Init();
		}

		#region Properties
		public MediaOpsTraceData TraceData { get; private set; }

		private SatelliteManagementHelpers.Beam BeamHelper => beamHelperLazy.Value;
		#endregion

		#region Methods
		public ActionOutput Execute()
		{
			domBeam = scriptData.SatelliteManagementHandler.GetBeamByDomInstanceId(inputData.DomBeamId) ?? throw new InvalidOperationException($"DOM Beam with ID '{inputData.DomBeamId}' does not exist.");

			var actionMethods = new Dictionary<BeamAction, Action>
			{
				[BeamAction.Activate] = HandleActivateAction,
				[BeamAction.Deprecate] = HandleDeprecateAction,
				[BeamAction.Edit] = HandleEditAction,
				[BeamAction.Error] = HandleErrorAction,
			};

			if (!actionMethods.TryGetValue(inputData.BeamAction, out var action))
			{
				throw new NotSupportedException($"Action '{inputData.BeamAction}' is not supported.");
			}

			action();

			return null;
		}

		private void HandleActivateAction()
		{
			BeamHelper.Activate();
		}

		private void HandleDeprecateAction()
		{
			BeamHelper.Deprecate();
		}

		private void HandleEditAction()
		{
			BeamHelper.Edit();
		}

		private void HandleErrorAction()
		{
			BeamHelper.Error();
		}

		private void Init()
		{
			beamHelperLazy = new Lazy<SatelliteManagementHelpers.Beam>(() => GetBeamHelper());
		}

		private SatelliteManagementHelpers.Beam GetBeamHelper()
		{
			return new SatelliteManagementHelpers.Beam(scriptData.Engine, scriptData.Logger, scriptData.SatelliteManagementHandler, domBeam);
		}
		#endregion
	}
}
