namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class Beam : SatelliteManagementBase
	{
		public Beam(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			Guid domBeamId) : base(engine, logger, satelliteManagementHandler)
		{
			DomBeam = GetDomBeam(domBeamId);
		}

		public Beam(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			DomApplications.SatelliteManagement.Beam domBeam) : base(engine, logger, satelliteManagementHandler)
		{
			DomBeam = domBeam ?? throw new ArgumentNullException(nameof(domBeam));
		}

		public DomApplications.SatelliteManagement.Beam DomBeam { get; }

		public override void Activate()
		{
			if (DomBeam.StatusId.Equals("active"))
			{
				return;
			}

			var domSatellite = satelliteManagementHandler.GetSatelliteByDomInstanceId(DomBeam.BeamSection.BeamSatelliteId);
			if (domSatellite.StatusId != "active" && domSatellite.StatusId != "edit")
			{
				engine.ShowErrorDialog("Beam cannot be changed since the Satellite isn't active.");
			}
			else
			{
				SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomBeam.Instance, "active");
			}
		}

		public override void Deprecate()
		{
			if (DomBeam.StatusId.Equals("deprecated"))
			{
				return;
			}

			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomBeam.Instance, "deprecated");
		}

		public override void Edit()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomBeam.Instance, "edit");
		}

		public override void Error()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomBeam.Instance, "error");
		}

		private DomApplications.SatelliteManagement.Beam GetDomBeam(Guid domBeamId)
		{
			var beamDomInstance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domBeamId);

			if (beamDomInstance == null)
			{
				throw new InvalidOperationException($"Beam Instance {domBeamId} does not exist in DOM.");
			}

			if (!beamDomInstance.DomDefinitionId.Equals(DomApplications.DomIds.SlcSatellite_Management.Definitions.Beams))
			{
				throw new InvalidOperationException($"Instance {domBeamId} does not belong to Beams definition.");
			}

			return new DomApplications.SatelliteManagement.Beam(satelliteManagementHandler, beamDomInstance);
		}
	}
}