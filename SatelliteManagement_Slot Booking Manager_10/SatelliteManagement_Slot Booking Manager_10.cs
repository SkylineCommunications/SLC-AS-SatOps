/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

Skyline Communications NV
Ambachtenstraat 33
B-8870 Izegem
Belgium
Tel.  : +32 51 31 35 69
Fax.  : +32 51 31 01 29
E-mail  : info@skyline.be
Web    : www.skyline.be
Contact  : Ben Vandenberghe

****************************************************************************
Revision History:

DATE    VERSION    AUTHOR      COMMENTS

dd/mm/2024  1.0.0.1    XXX, Skyline  Initial version
****************************************************************************
*/

namespace Slot_Bookings_Extension_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.BookingExtensionHandler;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.ResourceStudio;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.Scheduling;
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;
	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-AS-Slot Booking Manager";

		private IEngine engine;

		private DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			this.engine = engine;
			using (var logger = new SatOpsLogger(SatOpsLogger.Types.Satellite))
			{
				try
				{
					var bookingExtensionHandler = SchedulingHelper.LoadBookingExtensionHandler(engine);
					var domResourceId = bookingExtensionHandler.InputData.BookedNode.DomResourceId;

					satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(engine);
					var selectedSlot = satelliteManagementHandler.GetSlots(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Slot.Resource).Equal(domResourceId)).SingleOrDefault();

					var slotToBook = new Slot(engine, logger, satelliteManagementHandler, selectedSlot);

					var slotsByTransponder = GetTransponderSlots(slotToBook.DomSlot.SlotSection.TransponderId);
					var reservedNodes = GetReservedNodes(logger, slotToBook, slotsByTransponder);

					bookingExtensionHandler.ReturnResult(reservedNodes, Skyline.DataMiner.Utils.MediaOps.Helpers.Scheduling.JobNodeRelationshipActions.TransponderSlotId);
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Exception occurred in '{ScriptName}'");
				}
			}
		}

		private List<ExtensionNode> GetReservedNodes(SatOpsLogger logger, Slot slotToBook, List<DomApplications.SatelliteManagement.Slot> slotsByTransponder)
		{
			var reservedNodes = new List<ExtensionNode>();

			var resourceStudioHelper = new ResourceStudioHelper(engine);

			foreach (var domSlot in slotsByTransponder)
			{
				var comparedSlot = new Slot(engine, logger, satelliteManagementHandler, domSlot);
				var resource = resourceStudioHelper.GetResource(comparedSlot.DomSlot.SlotSection.ResourceId);
				var resourcePool = resource.ResourcePools.FirstOrDefault(); // As for now, the resource will be only link to 1 Resource pool

				// resource.ResourcePools
				// resourceStudioHelper.GetResourcePool()
				// var resourcePool = resourceStudioHelper.GetResourcePool($"Transponder 2 Mhz Slots");
				var overlap = GetOverlap(slotToBook, comparedSlot);
				if (overlap > 0 && slotToBook.DomSlot.SlotSection.SlotName != comparedSlot.DomSlot.SlotSection.SlotName)
				{
					var capacitiesDict = resourceStudioHelper.GetCapacities(new List<string> { "Transponder Bandwidth" });
					ResourceCapacity resourceCapacity = GetCapacity(comparedSlot, resourceStudioHelper, capacitiesDict);

					reservedNodes.Add(new ExtensionNode
					{
						DomResourceId = comparedSlot.DomSlot.SlotSection.ResourceId,
						DomResourcePoolId = resourcePool.Id,
						Hidden = true,
						Billable = false,
						Alias = comparedSlot.DomSlot.SlotSection.SlotName,
						Action = ActionType.ReserveNode,
						CapacityUsages = new List<CapacityUsage>
						{
						 	new CapacityUsage
						 	{
						 		DomCapacityId = resourceCapacity?.Id ?? Guid.Empty,
						 		Value = overlap,
						 	},
						},

						// Connections = new Connection
						// {
						//  SourceNodeId = Convert.ToString(slotToBook.ID),
						//  DestinationNodeId = Convert.ToString(comparedSlot.ID),
						//  Type = "reference",
						// },
					});
				}
			}

			return reservedNodes;
		}

		private static ResourceCapacity GetCapacity(Slot slotToBook, ResourceStudioHelper resourceStudioHelper, IDictionary<string, Capacity> capacitiesDict)
		{
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
				var transponderBandwithId = resourceStudioHelper.CreateCapacity(transpBandwidthConfig);
				transponderBandwith = resourceStudioHelper.GetCapacity(transponderBandwithId);
			}
			else
			{
				transponderBandwith = capacitiesDict["Transponder Bandwidth"];
			}

			var resourceCapacity = new ResourceCapacity(transponderBandwith)
			{
				CapacityValue = Convert.ToDouble(slotToBook.DomSlot.SlotSection.SlotSize),
			};
			return resourceCapacity;
		}

		public static double GetOverlap(Slot bookedSlot, Slot comparedSlot)
		{
			var minimumEnd = Math.Min(Convert.ToDouble(bookedSlot.DomSlot.SlotSection.SlotEndFrequency), Convert.ToDouble(comparedSlot.DomSlot.SlotSection.SlotEndFrequency));
			var maximumStart = Math.Max(Convert.ToDouble(bookedSlot.DomSlot.SlotSection.SlotStartFrequency), Convert.ToDouble(comparedSlot.DomSlot.SlotSection.SlotStartFrequency));
			return minimumEnd - maximumStart;
		}

		private List<DomApplications.SatelliteManagement.Slot> GetTransponderSlots(Guid domTransponderId)
		{
			return satelliteManagementHandler.GetSlots(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Slot.Transponder).Equal(domTransponderId)).ToList();
		}
	}
}