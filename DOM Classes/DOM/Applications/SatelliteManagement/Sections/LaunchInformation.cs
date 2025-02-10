namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public class LaunchInformation : SectionBase<LaunchInformation>
	{
		private static readonly Dictionary<FieldDescriptorID, Action<LaunchInformation, object>> SectionFieldMapping = new Dictionary<FieldDescriptorID, Action<LaunchInformation, object>>
		{
			[DomIds.SlcSatellite_Management.Sections.LaunchInformation.LaunchInfo] = (obj, value) => obj.LaunchInfo = Convert.ToString(value),
			[DomIds.SlcSatellite_Management.Sections.LaunchInformation.LaunchInServiceDate] = (obj, value) => obj.LaunchInServiceDate = Convert.ToDateTime(value),
		};

		public LaunchInformation() : base(DomIds.SlcSatellite_Management.Sections.LaunchInformation.Id)
		{
		}

		internal LaunchInformation(Section section) : base(section)
		{
		}

		public string LaunchInfo { get; set; }

		public DateTimeOffset LaunchInServiceDate { get; set; }

		protected override Dictionary<FieldDescriptorID, Action<LaunchInformation, object>> FieldMapping => SectionFieldMapping;

		internal override void ApplyChanges()
		{
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.LaunchInformation.LaunchInfo, LaunchInfo);
			Section.AddOrUpdateValue(DomIds.SlcSatellite_Management.Sections.LaunchInformation.LaunchInServiceDate, LaunchInServiceDate.UtcDateTime);
		}
	}
}