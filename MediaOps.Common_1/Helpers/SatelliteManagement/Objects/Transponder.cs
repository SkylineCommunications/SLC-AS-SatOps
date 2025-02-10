namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class Transponder : SatelliteManagementBase
	{

		public Transponder(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			Guid domTransponderId) : base(engine, logger, satelliteManagementHandler)
		{
			DomTransponder = GetDomTransponder(domTransponderId);
		}

		public Transponder(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			DomApplications.SatelliteManagement.Transponder domTransponder) : base(engine, logger, satelliteManagementHandler)
		{
			DomTransponder = domTransponder ?? throw new ArgumentNullException(nameof(domTransponder));
		}

		public DomApplications.SatelliteManagement.Transponder DomTransponder { get; private set; }

		public void DeprecateSlots()
		{
			var slotsFailed = false;

			var domSlots = satelliteManagementHandler.GetSlots(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Slot.Transponder).Equal(DomTransponder.InstanceId)).ToList();

			foreach (var domSlot in domSlots)
			{
				try
				{
					var slot = new Slot(engine, logger, satelliteManagementHandler, domSlot);
					slot.Deprecate();
				}
				catch (InvalidOperationException)
				{
					slotsFailed = true;
				}
			}

			if (slotsFailed)
			{
				if (DomTransponder.Instance.StatusId != "error")
				{
					SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponder.Instance, "error");
				}

				engine.ShowErrorDialog($"Unable to deprecate slot(s) within {DomTransponder.TransponderSection.TransponderName}. Please check logging for more details.");
			}
		}

		public override void Activate()
		{
			if (DomTransponder.StatusId.Equals("active"))
			{
				engine.ShowErrorDialog("Transponder is already in Active state.");
				return;
			}

			var domSatellite = satelliteManagementHandler.GetSatelliteByDomInstanceId(DomTransponder.TransponderSection.TransponderSatelliteId);
			if (domSatellite.StatusId != "active" && domSatellite.StatusId != "edit")
			{
				engine.ShowErrorDialog("Transponder cannot be activated since the Satellite isn't active.");
			}
			else
			{
				SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponder.Instance, "active");

				// Get instance again to get updated status.
				DomTransponder = satelliteManagementHandler.GetTransponderByDomInstanceId(DomTransponder.InstanceId);

				var domTransponderPlans = satelliteManagementHandler.GetAllTransponderPlans();
				var domTransponderPlan = domTransponderPlans.FirstOrDefault(x => x.TransponderPlanSection.AppliedTransponderIds.Contains(DomTransponder.InstanceId));

				if (domTransponderPlan != null)
				{
					var transponderPlan = new TransponderPlan(engine, logger, satelliteManagementHandler, domTransponderPlan);
					transponderPlan.ApplyTransponder(this);
				}
			}
		}

		public override void Deprecate()
		{
			if (DomTransponder.Instance.StatusId != "active" && DomTransponder.Instance.StatusId != "error" && DomTransponder.Instance.StatusId != "edit")
			{
				return;
			}

			DeprecateSlots();

			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponder.Instance, "deprecated");
		}

		public override void Edit()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponder.Instance, "edit");
		}

		public override void Error()
		{
			// No Action
		}

		private DomApplications.SatelliteManagement.Transponder GetDomTransponder(Guid domTransponderId)
		{
			var transponderDomInstance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domTransponderId);

			if (transponderDomInstance == null)
			{
				throw new InvalidOperationException($"Transponder Instance {domTransponderId} does not exist in DOM.");
			}

			if (!transponderDomInstance.DomDefinitionId.Equals(SlcSatellite_Management.Definitions.Transponders))
			{
				throw new InvalidOperationException($"Instance {domTransponderId} does not belong to Transponders definition.");
			}

			return new DomApplications.SatelliteManagement.Transponder(satelliteManagementHandler, transponderDomInstance);
		}
	}
}