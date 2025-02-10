namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class Satellite : SatelliteManagementBase
	{
		public Satellite(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			Guid domSatelliteId) : base(engine, logger, satelliteManagementHandler)
		{
			DomSatellite = GetDomSatellite(domSatelliteId);
		}

		public Satellite(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			DomApplications.SatelliteManagement.Satellite domSatellite) : base(engine, logger, satelliteManagementHandler)
		{
			DomSatellite = domSatellite ?? throw new ArgumentNullException(nameof(domSatellite));
		}

		public DomApplications.SatelliteManagement.Satellite DomSatellite { get; }

		public List<ISatelliteManagementBase> FindAllInstancesInSatellite()
		{
			var instancesInSatellite = new List<ISatelliteManagementBase>();

			var domSatelliteBeams = satelliteManagementHandler.GetBeams(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Beam.BeamSatellite).Equal(DomSatellite.InstanceId)).ToList();
			domSatelliteBeams.ForEach(x => instancesInSatellite.Add(new Beam(engine, logger, satelliteManagementHandler, x)));

			var domSatelliteTransponders = satelliteManagementHandler.GetTransponders(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Transponder.TransponderSatellite).Equal(DomSatellite.InstanceId)).ToList();
			domSatelliteTransponders.ForEach(x => instancesInSatellite.Add(new Transponder(engine, logger, satelliteManagementHandler, x)));

			return instancesInSatellite;
		}

		public override void Activate()
		{
			if (DomSatellite.StatusId.Equals("active"))
			{
				return;
			}

			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSatellite.Instance, "active");

			var domInstancesToUpdate = FindAllInstancesInSatellite();
			foreach (var child in domInstancesToUpdate)
			{
				child.Activate();
			}
		}

		public override void Deprecate()
		{
			if (DomSatellite.StatusId.Equals("deprecated"))
			{
				return;
			}

			var domInstancesToUpdate = FindAllInstancesInSatellite();

			foreach (var child in domInstancesToUpdate)
			{
				child.Deprecate();
			}

			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSatellite.Instance, "deprecated");
		}

		public override void Edit()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomSatellite.Instance, "edit");
		}

		public override void Error()
		{
			// No Action
		}

		private DomApplications.SatelliteManagement.Satellite GetDomSatellite(Guid domSatelliteId)
		{
			var satelliteDomInstance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domSatelliteId);

			if (satelliteDomInstance == null)
			{
				throw new InvalidOperationException($"Satellite Instance {domSatelliteId} does not exist in DOM.");
			}

			if (!satelliteDomInstance.DomDefinitionId.Equals(SlcSatellite_Management.Definitions.Satellites))
			{
				throw new InvalidOperationException($"Instance {domSatelliteId} does not belong to Satellites definition.");
			}

			return new DomApplications.SatelliteManagement.Satellite(satelliteManagementHandler, satelliteDomInstance);
		}
	}
}