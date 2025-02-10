namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.ResourceStudio;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class SatelliteManagementHelper
	{
		public static List<SlotDefinition> GetSlotDefinitionsFromPlan(DomInstance transponderPlanInstance, DomCache domCache)
		{
			List<SlotDefinition> slotDefs = new List<SlotDefinition>();
			var slotDefinitionSections = transponderPlanInstance.GetSectionsWithDefinition("Slot Definition", domCache);
			foreach (var slotDefinitionSection in slotDefinitionSections)
			{
				var slotName = slotDefinitionSection.GetValue<string>("Definition Slot name", domCache);
				var slotSize = slotDefinitionSection.GetValue<double>("Definition Slot size", domCache);
				var slotStartFreq = slotDefinitionSection.GetValue<double>("Relative start frequency", domCache);
				var slotEndFreq = slotDefinitionSection.GetValue<double>("Relative end frequency", domCache);

				slotDefs.Add(new SlotDefinition
				{
					Name = slotName,
					Size = slotSize,
					StartFrequency = slotStartFreq,
					EndFrequency = slotEndFreq,
				});
			}

			return slotDefs;
		}

		public static void DomStatusTransition(DomHelper domHelper, DomInstance domInstance, string expectedStatus)
		{
			domHelper.DomInstances.DoStatusTransition(domInstance.ID, $"{domInstance.StatusId}_to_{expectedStatus}");
		}

		public static Resource CreateSlotResource(
			SatOpsLogger logger,
			ResourceStudioHelper resourceStudioHelper,
			Transponder transponder,
			string satelliteName,
			double slotSize,
			string slotName)
		{
			try
			{
				var resourceConfig = new ResourceConfiguration
				{
					Name = slotName,
					Concurrency = 1000,
				};
				var objMeta = new ObjectMetadata
				{
					BookingExtensionScriptName = "SAT-AS-Slot Booking Manager",
				};
				var resourceId = resourceStudioHelper.CreateResource(resourceConfig, objMeta);
				var resourceInstance = resourceStudioHelper.GetResource(resourceId);

				SetOrCreateCapabilities(resourceStudioHelper, resourceInstance, satelliteName, transponder.DomTransponder.Instance.Name);
				SetOrCreateCapacities(resourceStudioHelper, resourceInstance, slotSize);

				return resourceInstance;
			}
			catch (Exception ex)
			{
				logger.Error($"Failed to create resource for {slotName}: {ex}");
				return null;
			}
		}

		public static void SetOrCreateCapacities(ResourceStudioHelper resourceStudioHelper, Resource resource, double slotSize)
		{
			var capacitiesDict = resourceStudioHelper.GetCapacities(new List<string> { "Transponder Bandwidth" });

			Capacity transponderBandwith;

			if (!capacitiesDict.ContainsKey("Transponder Bandwidth"))
			{
				var transpBandwidthConfig = new CapacityConfiguration
				{
					Name = "Transponder Bandwidth",
					RangeMin = 0,
					Units = "Mhz",
					Decimals = 2,
				};
				var capacityId = resourceStudioHelper.CreateCapacity(transpBandwidthConfig);
				transponderBandwith = resourceStudioHelper.GetCapacity(capacityId);
			}
			else
			{
				transponderBandwith = capacitiesDict["Transponder Bandwidth"];
			}

			var resourceCapacity = new ResourceCapacity(transponderBandwith)
			{
				CapacityValue = slotSize,
			};
			resource.SetCapacities(new List<ResourceCapacity> { resourceCapacity });
		}

		public static void RemoveCreatedInstances(IEngine engine, DomHelper domHelper, List<DomApplications.SatelliteManagement.Slot> domSlots, List<Resource> resourceInstances)
		{
			// remove created slot instances and resources after exception.
			if (domSlots.Count > 0)
			{
				domHelper.DomInstances.Delete(domSlots.Select(x => x.Instance).ToList()).ThrowOnFailure();
			}

			if (resourceInstances.Count > 0)
			{
				var resourceHelper = new ResourceStudioHelper(engine);
				resourceHelper.DeleteResources(resourceInstances);
			}
		}

		private static void SetOrCreateCapabilities(ResourceStudioHelper resourceStudioHelper, Resource resource, string satelliteName, string transponderName)
		{
			var capabilitiesDict = resourceStudioHelper.GetCapabilities(new List<string> { "Slot Satellite", "Slot Transponder" });
			Capability satelliteCapability;
			Capability transponderCapability;

			if (!capabilitiesDict.ContainsKey("Slot Satellite"))
			{
				var satCapabilityConfig = new CapabilityConfiguration { Name = "Slot Satellite" };
				satCapabilityConfig.Discretes.Add(satelliteName);

				var satelliteCapabilityId = resourceStudioHelper.CreateCapability(satCapabilityConfig);
				satelliteCapability = resourceStudioHelper.GetCapability(satelliteCapabilityId);
			}
			else
			{
				satelliteCapability = capabilitiesDict["Slot Satellite"];
				var discreteExists = satelliteCapability.Discretes.Any(x => x.Equals(satelliteName));
				if (!discreteExists)
				{
					var discretes = satelliteCapability.Discretes.Keys.ToList();
					discretes.Add(satelliteName);

					satelliteCapability = resourceStudioHelper.UpdateCapabilityDiscretes(satelliteCapability, discretes);
				}
			}

			if (!capabilitiesDict.ContainsKey("Slot Transponder"))
			{
				var transpCapabilityConfig = new CapabilityConfiguration { Name = "Slot Transponder" };
				transpCapabilityConfig.Discretes.AddRange(new List<string> { transponderName });

				var capabilityId = resourceStudioHelper.CreateCapability(transpCapabilityConfig);
				transponderCapability = resourceStudioHelper.GetCapability(capabilityId);
			}
			else
			{
				transponderCapability = capabilitiesDict["Slot Transponder"];
				var discreteExists = transponderCapability.Discretes.Any(x => x.Equals(transponderName));

				if (!discreteExists)
				{
					var discretes = transponderCapability.Discretes.Keys.ToList();
					discretes.Add(transponderName);

					transponderCapability = resourceStudioHelper.UpdateCapabilityDiscretes(transponderCapability, discretes);
				}
			}

			var satResourceCapability = new ResourceCapability(satelliteCapability);
			satResourceCapability.Discretes.AddRange(new List<string> { satelliteName });
			var transpResourceCapability = new ResourceCapability(transponderCapability);
			transpResourceCapability.Discretes.AddRange(new List<string> { transponderName });

			var resourceCap = new List<ResourceCapability> { satResourceCapability, transpResourceCapability };

			resource.SetCapabilities(resourceCap);
		}
	}
}
