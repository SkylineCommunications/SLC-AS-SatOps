namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Sections;

	public class TransponderPlanSection : SectionBase<TransponderPlanSection>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<TransponderPlanSection, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<TransponderPlanSection, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.TransponderPlan.PlanName] = (obj, value) => obj.PlanName = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds] = (obj, value) => obj.AppliedTransponderIds = Convert.ToString(value).Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(x => Guid.Parse(x)).ToList(),
		};

		public TransponderPlanSection() : base(DomIds.SlcSatellite_Management.Sections.TransponderPlan.Id)
		{
		}

		internal TransponderPlanSection(Section section) : base(section)
		{
		}

		public string PlanName { get; set; }

		public List<Guid> AppliedTransponderIds { get; set; } = new List<Guid>();

		protected override Dictionary<FieldDescriptorID, Action<TransponderPlanSection, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.TransponderPlan.PlanName, PlanName);

			if (AppliedTransponderIds.Count > 0)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds, string.Join(",", AppliedTransponderIds.Distinct()));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds);
			}
		}
	}
}