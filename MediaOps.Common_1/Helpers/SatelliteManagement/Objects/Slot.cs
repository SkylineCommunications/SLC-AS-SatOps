namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.ResourceStudio;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;
	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class Slot : SatelliteManagementBase
	{
		public Slot(
		IEngine engine,
		SatOpsLogger logger,
		DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
		Guid domSlotId) : base(engine, logger, satelliteManagementHandler)
		{
			DomSlot = GetDomSlot(domSlotId);
		}

		public Slot(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			DomApplications.SatelliteManagement.Slot domSlot) : base(engine, logger, satelliteManagementHandler)
		{
			DomSlot = domSlot ?? throw new ArgumentNullException(nameof(domSlot));
		}

		public DomApplications.SatelliteManagement.Slot DomSlot { get; }

		public override void Activate()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSlot.Instance, "active");
		}

		public override void Deprecate()
		{
			try
			{
				var slotStatus = DomSlot.Instance.StatusId;

				if (slotStatus.Equals("deprecated"))
				{
					return;
				}

				var slotResource = DomSlot.SlotSection.ResourceId;

				var settings = new ClientMetadata();
				var resourceStudioHelper = new ResourceStudioHelper(engine, settings);
				var resource = resourceStudioHelper.GetResource(slotResource);

				if (slotResource == Guid.Empty || resource?.Name == null)
				{
					SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSlot.Instance, "deprecated");
					return;
				}

				resource.ExecuteResourceAction(ResourceAction.Deprecate);

				SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSlot.Instance, "deprecated");
			}
			catch (InvalidOperationException e)
			{
				logger.Warning($"Unable to deprecate slot {DomSlot.SlotSection.SlotName} as it is possibly being used in a booking: {e}");
				if (DomSlot.StatusId != "error")
				{
					SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSlot.Instance, "error");
				}

				throw;
			}
		}

		public override void Edit()
		{
			// No Action
		}

		public override void Error()
		{
			// No Action
		}

		private DomApplications.SatelliteManagement.Slot GetDomSlot(Guid domSlotId)
		{
			var slotDomInstance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domSlotId);

			if (slotDomInstance == null)
			{
				throw new InvalidOperationException($"Slot Instance {domSlotId} does not exist in DOM.");
			}

			if (!slotDomInstance.DomDefinitionId.Equals(DomApplications.DomIds.SlcSatellite_Management.Definitions.Slots))
			{
				throw new InvalidOperationException($"Instance {domSlotId} does not belong to Slots definition.");
			}

			return new DomApplications.SatelliteManagement.Slot(satelliteManagementHandler, slotDomInstance);
		}
	}
}