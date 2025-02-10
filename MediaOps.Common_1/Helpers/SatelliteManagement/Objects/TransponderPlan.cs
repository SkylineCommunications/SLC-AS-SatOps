namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.ResourceStudio;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;
	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	public class TransponderPlan : SatelliteManagementBase
	{
		public TransponderPlan(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			Guid domTransponderPlanId) : base(engine, logger, satelliteManagementHandler)
		{
			DomTransponderPlan = GetDomTransponderPlan(domTransponderPlanId);
		}

		public TransponderPlan(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler,
			DomApplications.SatelliteManagement.TransponderPlan domTransponderPlan) : base(engine, logger, satelliteManagementHandler)
		{
			DomTransponderPlan = domTransponderPlan ?? throw new ArgumentNullException(nameof(domTransponderPlan));
		}

		public enum UpdateType
		{
			Add,
			Remove,
		}

		public DomApplications.SatelliteManagement.TransponderPlan DomTransponderPlan { get; private set; }

		public override void Activate()
		{
			if (DomTransponderPlan.TransponderPlanSection.AppliedTransponderIds.Count == 0)
			{
				SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponderPlan.Instance, "active");
				return;
			}

			ProcessTransponders(DomTransponderPlan.TransponderPlanSection.AppliedTransponderIds);
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponderPlan.Instance, "active");
		}

		public override void Deprecate()
		{
			DeprecateTranspondersSlots();
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponderPlan.Instance, "deprecated");
		}

		public override void Edit()
		{
			SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponderPlan.Instance, "edit");
		}

		public override void Error()
		{
			// No action
		}

		public void ApplyTransponder(Guid domTransponderId)
		{
			var transponder = new Transponder(engine, logger, satelliteManagementHandler, domTransponderId);
			ApplyTransponder(transponder);
		}

		public void ApplyTransponder(Transponder transponder)
		{
			var createdResources = new List<Resource>();
			var createdSlots = new List<DomApplications.SatelliteManagement.Slot>();
			try
			{
				if (transponder.DomTransponder.StatusId.Equals("active"))
				{
					var domSatellite = satelliteManagementHandler.GetSatelliteByDomInstanceId(transponder.DomTransponder.TransponderSection.TransponderSatelliteId);

					var settings = new ClientMetadata
					{
						ModuleId = string.Empty,
						Prefix = string.Empty,
					};
					var resourceStudioHelper = new ResourceStudioHelper(engine, settings);

					var slotResourceDict = new Dictionary<string, List<Resource>>();
					foreach (var slotDef in DomTransponderPlan.SlotDefinitions)
					{
						var nextStartFrequency = transponder.DomTransponder.TransponderSection.StartFrequency + slotDef.StartFrequency;

						double? actualEndFrequency = GetActualEndFrequency(slotDef, transponder);

						var resources = new List<Resource>();
						int characterIncrement = 1;

						while (nextStartFrequency + slotDef.Size <= actualEndFrequency)
						{
							var centerFrequency = (nextStartFrequency + (nextStartFrequency + slotDef.Size)) / 2;
							var slotName = $"{domSatellite.General.SatelliteAbbreviation} - {transponder.DomTransponder.TransponderSection.TransponderName} Slot {slotDef.Size}{GetCharacterId(characterIncrement)}";

							var domSlotSection = new DomApplications.SatelliteManagement.SlotSection
							{
								TransponderId = transponder.DomTransponder.InstanceId,
								TransponderPlanId = DomTransponderPlan.InstanceId,
								SlotName = slotName,
								SlotSize = Convert.ToString(slotDef.Size),
								CenterFrequency = Convert.ToString(centerFrequency),
								SlotStartFrequency = Convert.ToString(nextStartFrequency),
								SlotEndFrequency = Convert.ToString(nextStartFrequency + slotDef.Size),
							};

							var domSlot = new DomApplications.SatelliteManagement.Slot(satelliteManagementHandler);
							domSlot.AddOrReplaceSlotSection(domSlotSection);
							domSlot.SetStatusId("active");

							domSlot = satelliteManagementHandler.CreateSlot(domSlot);
							createdSlots.Add(domSlot);

							var resourceExists = resourceStudioHelper.GetResource(slotName) != null;
							if (resourceExists)
							{
								throw new NotSupportedException($"Resource '{slotName}' already exists.");
							}

							// TODO
							var resourceInstance = SatelliteManagementHelper.CreateSlotResource(logger, resourceStudioHelper, transponder, domSatellite.General.SatelliteName, slotDef.Size, slotName);
							createdResources.Add(resourceInstance);

							domSlot.SlotSection.ResourceId = resourceInstance.Id;
							satelliteManagementHandler.UpdateSlot(domSlot);

							nextStartFrequency += slotDef.Size;
							characterIncrement++;

							resources.Add(resourceInstance);
						}

						var poolName = $"Transponder {slotDef.Name} Slots";
						slotResourceDict[poolName] = resources;
					}

					AddResourcesToPool(resourceStudioHelper, slotResourceDict);
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, $"Exception occurred in 'TransponderPlan'");
				SatelliteManagementHelper.RemoveCreatedInstances(engine, satelliteManagementHandler.DomHelper, createdSlots, createdResources);
			}
		}

		public void DeprecateTranspondersSlots()
		{
			var domSlots = satelliteManagementHandler.GetSlots(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Slot.TransponderPlan).Equal(DomTransponderPlan.InstanceId)).ToList();

			var slotsFailed = false;
			foreach (var domSlot in domSlots)
			{
				try
				{
					var slot = new Slot(engine, logger, satelliteManagementHandler, domSlot);
					var slotTransponderPlanId = slot.DomSlot.SlotSection.TransponderPlanId;
					if ((slotTransponderPlanId == DomTransponderPlan.Instance.ID.Id) && domSlot.StatusId.Equals("active"))
					{
						slot.Deprecate();
					}
				}
				catch (InvalidOperationException ex)
				{
					logger.Error(ex, $"Exception occurred in 'TransponderPlan'");
					slotsFailed = true;
				}
			}

			if (slotsFailed)
			{
				if (DomTransponderPlan.Instance.StatusId != "error")
				{
					SatelliteManagementHelper.DomStatusTransition(satelliteManagementHandler.DomHelper, DomTransponderPlan.Instance, "error");
				}

				engine.ShowErrorDialog($"Unable to deprecate slot(s) within {DomTransponderPlan.TransponderPlanSection.PlanName}. Please check logging for more details.");
			}

			DomTransponderPlan.TransponderPlanSection.AppliedTransponderIds.Clear();
			DomTransponderPlan = satelliteManagementHandler.UpdateTransponderPlan(DomTransponderPlan);
		}

		public void UpdateTransponderList(Transponder transponder, UpdateType type)
		{
			if (type == UpdateType.Remove)
			{
				DomTransponderPlan.TransponderPlanSection.AppliedTransponderIds.Remove(transponder.DomTransponder.InstanceId);
			}
			else if (type == UpdateType.Add)
			{
				DomTransponderPlan.TransponderPlanSection.AppliedTransponderIds.Add(transponder.DomTransponder.InstanceId);
			}
			else
			{
				// No Action
			}

			DomTransponderPlan = satelliteManagementHandler.UpdateTransponderPlan(DomTransponderPlan);
		}

		private static string GetCharacterId(int columnNumber)
		{
			const int baseValue = 'a' - 1;
			string result = string.Empty;

			do
			{
				int remainder = columnNumber % 26;
				char currentChar = (char)((remainder == 0 ? 26 : remainder) + baseValue);
				result = currentChar + result;
				columnNumber = (columnNumber - 1) / 26;
			}
			while (columnNumber > 0);
			return result;
		}

		private static void AddResourcesToPool(ResourceStudioHelper resourceStudioHelper, Dictionary<string, List<Resource>> slotResourceDict)
		{
			foreach (var slotResource in slotResourceDict)
			{
				var resourcePool = resourceStudioHelper.GetResourcePool(slotResource.Key);
				if (resourcePool == null)
				{
					var resourcePoolConfig = new ResourcePoolConfiguration
					{
						Name = slotResource.Key,
					};
					var settings = new ObjectMetadata();
					var resourcePoolId = resourceStudioHelper.CreateResourcePool(resourcePoolConfig, settings);

					resourcePool = resourceStudioHelper.GetResourcePool(resourcePoolId);
				}

				resourcePool.AssignResources(slotResource.Value);
			}
		}

		private static double? GetActualEndFrequency(DomApplications.SatelliteManagement.SlotDefinition slotDef, Transponder transponder)
		{
			var actualEndFrequency = transponder.DomTransponder.TransponderSection.StartFrequency + slotDef.EndFrequency;
			if (actualEndFrequency > transponder.DomTransponder.TransponderSection.StopFrequency)
			{
				actualEndFrequency = transponder.DomTransponder.TransponderSection.StopFrequency;
			}

			return actualEndFrequency;
		}

		private void ProcessTransponders(List<Guid> transpondersToApply)
		{
			if (transpondersToApply == null || transpondersToApply.Count == 0)
			{
				return;
			}

			var transponderFilter = new ORFilterElement<DomInstance>(transpondersToApply.Select(id => DomInstanceExposers.Id.Equal(id)).ToArray());
			var domTransponders = satelliteManagementHandler.GetTransponders(transponderFilter).ToList();

			var domSlotsWithTransponder = satelliteManagementHandler.GetSlots(DomInstanceExposers.FieldValues.KeyExists(DomApplications.DomIds.SlcSatellite_Management.Sections.Slot.Transponder.Id.ToString()).Equal(true)).ToList();
			var transponderIdsWithSlots = domSlotsWithTransponder.Where(x => x.SlotSection.TransponderId != Guid.Empty).Select(x => x.SlotSection.TransponderId).Distinct().ToList();

			foreach (var domTransponder in domTransponders)
			{
				if (domTransponder.StatusId != "active")
				{
					// transponder needs to be active
					continue;
				}

				if (transponderIdsWithSlots.Contains(domTransponder.InstanceId))
				{
					// transponder already has slots
					continue;
				}

				var transponder = new Transponder(engine, logger, satelliteManagementHandler, domTransponder);
				ApplyTransponder(transponder);
			}
		}

		private DomApplications.SatelliteManagement.TransponderPlan GetDomTransponderPlan(Guid domTransponderPlanId)
		{
			var transponderPlanDomInstance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domTransponderPlanId);

			if (transponderPlanDomInstance == null)
			{
				throw new InvalidOperationException($"Transponder Plan Instance {domTransponderPlanId} does not exist in DOM.");
			}

			if (!transponderPlanDomInstance.DomDefinitionId.Equals(SlcSatellite_Management.Definitions.TransponderPlans))
			{
				throw new InvalidOperationException($"Instance {domTransponderPlanId} does not belong to Transponder Plan definition.");
			}

			return new DomApplications.SatelliteManagement.TransponderPlan(satelliteManagementHandler, transponderPlanDomInstance);
		}
	}
}
