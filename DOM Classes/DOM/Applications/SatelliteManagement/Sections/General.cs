namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Sections;

	public class General : SectionBase<General>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<General, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<General, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.General.SatelliteName] = (obj, value) => obj.SatelliteName = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.General.SatelliteAbbreviation] = (obj, value) => obj.SatelliteAbbreviation = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.General.Orbit] = (obj, value) => obj.Orbit = DomIds.SlcSatellite_Management.Enums.Orbit.ToEnum(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.General.Hemisphere] = (obj, value) => obj.Hemisphere = DomIds.SlcSatellite_Management.Enums.Hemisphere.ToEnum(Convert.ToString(value)),
			[DomIds.SlcSatellite_Management.Sections.General.LongitudeForGEODegrees] = (obj, value) => obj.LongitudeForGEODegrees = Convert.ToDouble(value),
			[DomIds.SlcSatellite_Management.Sections.General.InclinationDegrees] = (obj, value) => obj.InclinationDegrees = Convert.ToDouble(value),
		};

		public General() : base(DomIds.SlcSatellite_Management.Sections.General.Id)
		{
		}

		internal General(Section section) : base(section)
		{
		}

		public string SatelliteName { get; set; }

		public string SatelliteAbbreviation { get; set; }

		public DomIds.SlcSatellite_Management.Enums.OrbitEnum? Orbit { get; set; }

		public DomIds.SlcSatellite_Management.Enums.HemisphereEnum? Hemisphere { get; set; }

		public double LongitudeForGEODegrees { get; set; }

		public double InclinationDegrees { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<General, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.SatelliteName, SatelliteName);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.SatelliteAbbreviation, SatelliteAbbreviation);

			if (Orbit.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.Orbit, DomIds.SlcSatellite_Management.Enums.Orbit.ToValue(Orbit.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.General.Orbit);
			}

			if (Hemisphere.HasValue)
			{
				Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.Hemisphere, DomIds.SlcSatellite_Management.Enums.Hemisphere.ToValue(Hemisphere.Value));
			}
			else
			{
				Section.RemoveFieldValueById(DomIds.SlcSatellite_Management.Sections.General.Hemisphere);
			}

			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.LongitudeForGEODegrees, Convert.ToDouble(LongitudeForGEODegrees));
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.General.InclinationDegrees, Convert.ToDouble(InclinationDegrees));
		}
	}
}