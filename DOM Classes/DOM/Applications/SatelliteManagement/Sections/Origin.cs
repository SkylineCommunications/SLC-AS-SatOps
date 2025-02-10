namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class Origin : SectionBase<Origin>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<Origin, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<Origin, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.Origin.Manufacturer] = (obj, value) => obj.Manufacturer = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.Origin.Country] = (obj, value) => obj.Country = Convert.ToString(value),
		};

		public Origin() : base(DomIds.SlcSatellite_Management.Sections.Origin.Id)
		{
		}

		internal Origin(Section section) : base(section)
		{
		}

		public string Manufacturer { get; set; }

		public string Country { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<Origin, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Origin.Manufacturer, Manufacturer);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.Origin.Country, Country);
		}
	}
}