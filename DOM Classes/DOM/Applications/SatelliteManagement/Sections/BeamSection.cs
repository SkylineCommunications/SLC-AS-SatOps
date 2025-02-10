namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class BeamSection : SectionBase<BeamSection>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<BeamSection, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<BeamSection, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.Beam.BeamName] = (obj, value) => obj.BeamName = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Beam.BeamSatellite] = (obj, value) => obj.BeamSatelliteId = Guid.Parse(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.Beam.LinkType] = (obj, value) => obj.LinkType = DomIds.SlcSatellite_Management.Enums.Linktype.ToEnum(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.Beam.TransmissionType] = (obj, value) => obj.TransmissionType = DomIds.SlcSatellite_Management.Enums.Transmissiontype.ToEnum(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.Beam.FootprintFile] = (obj, value) => obj.FootprintFile = Convert.ToString(value),
		};

		public BeamSection() : base(DomIds.SlcSatellite_Management.Sections.Beam.Id)
		{
		}

		internal BeamSection(Section section) : base(section)
		{
		}

		public Guid BeamSatelliteId { get; set; }

		public string BeamName { get; set; }

		public DomIds.SlcSatellite_Management.Enums.LinkTypeEnum? LinkType { get; set; }

		public DomIds.SlcSatellite_Management.Enums.TransmissionTypeEnum? TransmissionType { get; set; }

		public string FootprintFile { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<BeamSection, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Beam.BeamName, BeamName);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Beam.BeamSatellite, BeamSatelliteId);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Beam.FootprintFile, FootprintFile);

			if (LinkType.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Beam.LinkType, DomIds.SlcSatellite_Management.Enums.Linktype.ToValue(LinkType.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Beam.LinkType);
			}

			if (TransmissionType.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Beam.TransmissionType, DomIds.SlcSatellite_Management.Enums.Transmissiontype.ToValue(TransmissionType.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Beam.TransmissionType);
			}
		}
	}
}