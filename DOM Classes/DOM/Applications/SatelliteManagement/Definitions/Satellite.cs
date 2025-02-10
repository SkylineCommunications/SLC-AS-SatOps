namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public class Satellite : InstanceBase<Satellite>
	{
		private static readonly Dictionary<SectionDefinitionID, Action<Satellite, Section>> InstanceSectionMapping = new Dictionary<SectionDefinitionID, Action<Satellite, Section>>
		{
			[DomIds.SlcSatellite_Management.Sections.General.Id] = (obj, section) => obj.General = new General(section),
			[DomIds.SlcSatellite_Management.Sections.Satellite.Id] = (obj, section) => obj.SatelliteSection = new SatelliteSection(section),
			[DomIds.SlcSatellite_Management.Sections.Origin.Id] = (obj, section) => obj.Origin = new Origin(section),
			[DomIds.SlcSatellite_Management.Sections.LaunchInformation.Id] = (obj, section) => obj.LaunchInformation = new LaunchInformation(section),
		};

		public Satellite(SatelliteManagementHandler satelliteManagementHandler, DomInstance instance) : base(satelliteManagementHandler, instance, DomIds.SlcSatellite_Management.Definitions.Satellites)
		{
		}

		public Satellite(SatelliteManagementHandler satelliteManagementHandler) : base(satelliteManagementHandler, DomIds.SlcSatellite_Management.Definitions.Satellites)
		{
		}

		public General General { get; private set; }

		public SatelliteSection SatelliteSection { get; private set; }

		public Origin Origin { get; private set; }

		public LaunchInformation LaunchInformation { get; private set; }

		protected override Dictionary<SectionDefinitionID, Action<Satellite, Section>> SectionMapping => InstanceSectionMapping;

		public void AddOrReplaceGeneral(General general)
		{
			if (general == null)
			{
				throw new ArgumentNullException(nameof(general));
			}

			if (General != null)
			{
				Instance.Sections.Remove(General.Section);
			}

			General = general;
			Instance.Sections.Add(general.Section);
		}

		public void AddOrReplaceSatelliteSection(SatelliteSection satelliteSection)
		{
			if (satelliteSection == null)
			{
				throw new ArgumentNullException(nameof(satelliteSection));
			}

			if (SatelliteSection != null)
			{
				Instance.Sections.Remove(SatelliteSection.Section);
			}

			SatelliteSection = satelliteSection;
			Instance.Sections.Add(satelliteSection.Section);
		}

		public void AddOrReplaceOrigin(Origin origin)
		{
			if (origin == null)
			{
				throw new ArgumentNullException(nameof(origin));
			}

			if (SatelliteSection != null)
			{
				Instance.Sections.Remove(SatelliteSection.Section);
			}

			Origin = origin;
			Instance.Sections.Add(origin.Section);
		}

		public void AddOrReplaceLaunchInformation(LaunchInformation launchInformation)
		{
			if (launchInformation == null)
			{
				throw new ArgumentNullException(nameof(launchInformation));
			}

			if (LaunchInformation != null)
			{
				Instance.Sections.Remove(LaunchInformation.Section);
			}

			LaunchInformation = launchInformation;
			Instance.Sections.Add(launchInformation.Section);
		}

		public override void ApplyChanges()
		{
			General?.ApplyChanges();
			SatelliteSection?.ApplyChanges();
			Origin?.ApplyChanges();
			LaunchInformation?.ApplyChanges();
		}
	}
}