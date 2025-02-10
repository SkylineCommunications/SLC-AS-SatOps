namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class SlotDefinition : SectionBase<SlotDefinition>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<SlotDefinition, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<SlotDefinition, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotName] = (obj, value) => obj.Name = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotSize] = (obj, value) => obj.Size = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.SlotDefinition.RelativeStartFrequency] = (obj, value) => obj.StartFrequency = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.SlotDefinition.RelativeEndFrequency] = (obj, value) => obj.EndFrequency = Convert.ToDouble(value),
		};

		public SlotDefinition() : base(DomIds.SlcSatellite_Management.Sections.SlotDefinition.Id)
		{
		}

		public SlotDefinition(Section section) : base(section)
		{
		}

		public string Name { get; set; }

		public double Size { get; set; }

		public double StartFrequency { get; set; }

		public double EndFrequency { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<SlotDefinition, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotName, Name);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotSize, Size);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.SlotDefinition.RelativeStartFrequency, StartFrequency);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.SlotDefinition.RelativeEndFrequency, EndFrequency);
		}
	}
}