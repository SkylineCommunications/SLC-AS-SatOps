namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class SlotSection : SectionBase<SlotSection>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<SlotSection, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<SlotSection, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.Slot.Transponder] = (obj, value) => obj.TransponderId = Guid.TryParse(Convert.ToString(value), out Guid result) ? result : Guid.Empty,
			[DomIds.SlcSatellite_Management.Sections.Slot.TransponderPlan] = (obj, value) => obj.TransponderPlanId = Guid.TryParse(Convert.ToString(value), out Guid result) ? result : Guid.Empty,
			[DomIds.SlcSatellite_Management.Sections.Slot.SlotName] = (obj, value) => obj.SlotName = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Slot.SlotSize] = (obj, value) => obj.SlotSize = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Slot.CenterFrequency] = (obj, value) => obj.CenterFrequency = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Slot.SlotStartFrequency] = (obj, value) => obj.SlotStartFrequency = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Slot.SlotEndFrequency] = (obj, value) => obj.SlotEndFrequency = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Slot.Resource] = (obj, value) => obj.ResourceId = Guid.TryParse(Convert.ToString(value), out Guid result) ? result : Guid.Empty,
		};

		public SlotSection() : base(DomIds.SlcSatellite_Management.Sections.Slot.Id)
		{
		}

		internal SlotSection(Section section) : base(section)
		{
		}

		public Guid ResourceId { get; set; }

		public Guid TransponderId { get; set; }

		public Guid TransponderPlanId { get; set; }

		public string SlotName { get; set; }

		public string SlotSize { get; set; }

		public string CenterFrequency { get; set; }

		public string SlotStartFrequency { get; set; }

		public string SlotEndFrequency { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<SlotSection, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			if (TransponderId != Guid.Empty)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.Transponder, TransponderId);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Slot.Transponder);
			}

			if (TransponderPlanId != Guid.Empty)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.TransponderPlan, TransponderPlanId);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Slot.TransponderPlan);
			}

			if (ResourceId != Guid.Empty)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.Resource, ResourceId);
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.Slot.Resource);
			}

			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.SlotName, SlotName);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.SlotSize, SlotSize);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.CenterFrequency, CenterFrequency);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.SlotStartFrequency, SlotStartFrequency);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Slot.SlotEndFrequency, SlotEndFrequency);
		}
	}
}