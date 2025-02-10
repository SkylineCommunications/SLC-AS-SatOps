namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public class Transponder : InstanceBase<Transponder>
	{
		private static readonly Dictionary<SectionDefinitionID, Action<Transponder, Section>> InstanceSectionMapping = new Dictionary<SectionDefinitionID, Action<Transponder, Section>>
		{
			[DomIds.SlcSatellite_Management.Sections.Transponder.Id] = (obj, section) => obj.TransponderSection = new TransponderSection(section),
		};

		public Transponder(SatelliteManagementHandler satelliteManagementHandler, DomInstance instance) : base(satelliteManagementHandler, instance, DomIds.SlcSatellite_Management.Definitions.Transponders)
		{
		}

		public Transponder(SatelliteManagementHandler satelliteManagementHandler) : base(satelliteManagementHandler, DomIds.SlcSatellite_Management.Definitions.Transponders)
		{
		}

		public TransponderSection TransponderSection { get; private set; }

		protected override Dictionary<SectionDefinitionID, Action<Transponder, Section>> SectionMapping => InstanceSectionMapping;

		public void AddOrReplaceTransponderSection(TransponderSection transponderSection)
		{
			if (transponderSection == null)
			{
				throw new ArgumentNullException(nameof(transponderSection));
			}

			if (TransponderSection != null)
			{
				Instance.Sections.Remove(TransponderSection.Section);
			}

			TransponderSection = transponderSection;
			Instance.Sections.Add(transponderSection.Section);
		}

		public string GetStatus()
		{
			var capitalized = char.ToUpper(Instance.StatusId[0]) + Instance.StatusId.Substring(1);

			return capitalized;
		}

		public override void ApplyChanges()
		{
			TransponderSection?.ApplyChanges();
		}
	}
}