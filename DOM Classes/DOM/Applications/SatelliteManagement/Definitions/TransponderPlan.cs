namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public class TransponderPlan : InstanceBase<TransponderPlan>
	{
		private static readonly Dictionary<SectionDefinitionID, Action<TransponderPlan, Section>> InstanceSectionMapping = new Dictionary<SectionDefinitionID, Action<TransponderPlan, Section>>
		{
			[DomIds.SlcSatellite_Management.Sections.TransponderPlan.Id] = (obj, section) => obj.TransponderPlanSection = new TransponderPlanSection(section),
			[DomIds.SlcSatellite_Management.Sections.SlotDefinition.Id] = (obj, section) => obj.slotDefinitions.Add(new SlotDefinition(section)),
		};

		private readonly List<SlotDefinition> slotDefinitions = new List<SlotDefinition>();

		public TransponderPlan(SatelliteManagementHandler satelliteManagementHandler, DomInstance instance) : base(satelliteManagementHandler, instance, DomIds.SlcSatellite_Management.Definitions.TransponderPlans)
		{
		}

		public TransponderPlan(SatelliteManagementHandler satelliteManagementHandler) : base(satelliteManagementHandler, DomIds.SlcSatellite_Management.Definitions.TransponderPlans)
		{
		}

		public TransponderPlanSection TransponderPlanSection { get; private set; }

		public IReadOnlyCollection<SlotDefinition> SlotDefinitions => slotDefinitions;

		protected override Dictionary<SectionDefinitionID, Action<TransponderPlan, Section>> SectionMapping => InstanceSectionMapping;

		public void AddOrReplaceTransponderPlanSection(TransponderPlanSection transponderPlanSection)
		{
			if (transponderPlanSection == null)
			{
				throw new ArgumentNullException(nameof(transponderPlanSection));
			}

			if (TransponderPlanSection != null)
			{
				Instance.Sections.Remove(TransponderPlanSection.Section);
			}

			TransponderPlanSection = transponderPlanSection;
			Instance.Sections.Add(transponderPlanSection.Section);
		}

		public void AddSlotDefinition(SlotDefinition slotDefinition)
		{
			if (slotDefinition == null)
			{
				throw new ArgumentNullException(nameof(slotDefinition));
			}

			if (slotDefinitions.Exists(x => x.SectionId ==  slotDefinition.SectionId))
			{
				return;
			}

			slotDefinitions.Add(slotDefinition);
			Instance.Sections.Add(slotDefinition.Section);
		}

		public void RemoveSlotDefinition(SlotDefinition slotDefinition)
		{
			if (slotDefinition == null)
			{
				throw new ArgumentNullException(nameof(slotDefinition));
			}

			this.RemoveSlotDefinition(slotDefinition.SectionId);
		}

		public void RemoveSlotDefinition(Guid sectionId)
		{
			if (sectionId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(sectionId));
			}

			if (!slotDefinitions.Exists(x => x.SectionId == sectionId))
			{
				return;
			}

			slotDefinitions.RemoveAll(x => x.SectionId == sectionId);
			Instance.Sections.RemoveAll(x => x.ID.Id == sectionId);
		}

		public override void ApplyChanges()
		{
			TransponderPlanSection?.ApplyChanges();

			foreach (var slotDefinition in slotDefinitions)
			{
				slotDefinition.ApplyChanges();
			}
		}
	}
}