namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public class Slot : InstanceBase<Slot>
	{
		private static readonly Dictionary<SectionDefinitionID, Action<Slot, Section>> InstanceSectionMapping = new Dictionary<SectionDefinitionID, Action<Slot, Section>>
		{
			[DomIds.SlcSatellite_Management.Sections.Slot.Id] = (obj, section) => obj.SlotSection = new SlotSection(section),
		};

		public Slot(SatelliteManagementHandler satelliteManagementHandler, DomInstance instance) : base(satelliteManagementHandler, instance, DomIds.SlcSatellite_Management.Definitions.Slots)
		{
		}

		public Slot(SatelliteManagementHandler satelliteManagementHandler) : base(satelliteManagementHandler, DomIds.SlcSatellite_Management.Definitions.Slots)
		{
		}

		public SlotSection SlotSection { get; private set; }

		protected override Dictionary<SectionDefinitionID, Action<Slot, Section>> SectionMapping => InstanceSectionMapping;

		public void AddOrReplaceSlotSection(SlotSection slotSection)
		{
			if (slotSection == null)
			{
				throw new ArgumentNullException(nameof(slotSection));
			}

			if (SlotSection != null)
			{
				Instance.Sections.Remove(SlotSection.Section);
			}

			SlotSection = slotSection;
			Instance.Sections.Add(slotSection.Section);
		}

		public string GetStatus()
		{
			var capitalized = char.ToUpper(Instance.StatusId[0]) + Instance.StatusId.Substring(1);

			return capitalized;
		}

		public override void ApplyChanges()
		{
			SlotSection?.ApplyChanges();
		}
	}
}