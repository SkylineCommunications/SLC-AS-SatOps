namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class SatelliteSection : SectionBase<SatelliteSection>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<SatelliteSection, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<SatelliteSection, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.Satellite.Operator] = (obj, value) => obj.Operator = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Satellite.Coverage] = (obj, value) => obj.Coverage = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Satellite.Applications] = (obj, value) => obj.Applications = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Satellite.Info] = (obj, value) => obj.Info = Convert.ToString(value),
		};

		public SatelliteSection() : base(DomIds.SlcSatellite_Management.Sections.Satellite.Id)
		{
		}

		internal SatelliteSection(Section section) : base(section)
		{
		}

		public string Operator { get; set; }

		public string Coverage { get; set; }

		public string Applications { get; set; }

		public string Info { get; set; }


		protected override Dictionary<FieldDescriptorID, Action<SatelliteSection, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Satellite.Operator, Operator);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Satellite.Coverage, Coverage);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Satellite.Applications, Applications);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Satellite.Info, Info);
		}
	}
}