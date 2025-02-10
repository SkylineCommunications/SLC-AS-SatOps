namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public class Beam : InstanceBase<Beam>
	{
		private static readonly Dictionary<SectionDefinitionID, Action<Beam, Section>> InstanceSectionMapping = new Dictionary<SectionDefinitionID, Action<Beam, Section>>
		{
			[DomIds.SlcSatellite_Management.Sections.Beam.Id] = (obj, section) => obj.BeamSection = new BeamSection(section),
		};

		public Beam(SatelliteManagementHandler satelliteManagementHandler, DomInstance instance) : base(satelliteManagementHandler, instance, DomIds.SlcSatellite_Management.Definitions.Beams)
		{
		}

		public Beam(SatelliteManagementHandler satelliteManagementHandler) : base(satelliteManagementHandler, DomIds.SlcSatellite_Management.Definitions.Beams)
		{
		}

		public BeamSection BeamSection { get; private set; }

		protected override Dictionary<SectionDefinitionID, Action<Beam, Section>> SectionMapping => InstanceSectionMapping;

		public void AddOrReplaceBeamSection(BeamSection beamSection)
		{
			if (beamSection == null)
			{
				throw new ArgumentNullException(nameof(beamSection));
			}

			if (BeamSection != null)
			{
				Instance.Sections.Remove(BeamSection.Section);
			}

			BeamSection = beamSection;
			Instance.Sections.Add(beamSection.Section);
		}

		public override void ApplyChanges()
		{
			BeamSection?.ApplyChanges();
		}
	}
}