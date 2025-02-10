namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class TransponderSection : SectionBase<TransponderSection>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<TransponderSection, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<TransponderSection, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.Transponder.TransponderName] = (obj, value) => obj.TransponderName = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Transponder.TransponderSatellite] = (obj, value) => obj.TransponderSatelliteId = Guid.TryParse(Convert.ToString(value), out Guid result) ? result : Guid.Empty,
			[DomIds.SlcSatellite_Management.Sections.Transponder.Beam] = (obj, value) => obj.TransponderBeamId = Guid.TryParse(Convert.ToString(value), out Guid result) ? result : Guid.Empty,
			[DomIds.SlcSatellite_Management.Sections.Transponder.Band] = (obj, value) => obj.Band = DomIds.SlcSatellite_Management.Enums.Band.ToEnum(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.Transponder.Bandwidth] = (obj, value) => obj.Bandwidth = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.Transponder.StartFrequency] = (obj, value) => obj.StartFrequency = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.Transponder.StopFrequency] = (obj, value) => obj.StopFrequency = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.Transponder.Polarization] = (obj, value) => obj.Polarization = DomIds.SlcSatellite_Management.Enums.Polarization.ToEnum(Convert.ToString(value)),
		};

		public TransponderSection() : base(DomIds.SlcSatellite_Management.Sections.Transponder.Id)
		{
		}

		internal TransponderSection(Section section) : base(section)
		{
		}

		public Guid TransponderSatelliteId { get; set; }

		public Guid TransponderBeamId { get; set; }

		public string TransponderName { get; set; }

		public DomIds.SlcSatellite_Management.Enums.BandEnum? Band { get; set; }

		public double? Bandwidth { get; set; }

		public double? StartFrequency { get; set; }

		public double? StopFrequency { get; set; }

		public DomIds.SlcSatellite_Management.Enums.PolarizationEnum? Polarization { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<TransponderSection, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.TransponderName, TransponderName);

			if (TransponderSatelliteId != Guid.Empty)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.TransponderSatellite, TransponderSatelliteId);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.TransponderSatellite);
			}

			if (TransponderBeamId != Guid.Empty)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.Beam, TransponderBeamId);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.Beam);
			}

			if (Band.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.Band, DomIds.SlcSatellite_Management.Enums.Band.ToValue(Band.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.Band);
			}

			if (Polarization.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.Polarization, DomIds.SlcSatellite_Management.Enums.Polarization.ToValue(Polarization.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.Polarization);
			}

			if (Bandwidth.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.Bandwidth, Bandwidth.Value);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.Bandwidth);
			}

			if (StartFrequency.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.StartFrequency, StartFrequency);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.StartFrequency);
			}

			if (StopFrequency.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Transponder.StopFrequency, StopFrequency);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Transponder.StopFrequency);
			}
		}
	}
}