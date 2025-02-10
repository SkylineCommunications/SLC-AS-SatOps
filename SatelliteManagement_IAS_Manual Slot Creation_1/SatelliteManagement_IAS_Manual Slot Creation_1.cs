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
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2024	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace Manual_Slot_Creation_1
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;
	using Skyline.DataMiner.Utils.MediaOps.Helpers.ResourceStudio;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-IAS-Manual Slot Creation";

		private DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			using (var logger = new SatOpsLogger(SatOpsLogger.Types.Satellite))
			{
				// DO NOT REMOVE THIS COMMENTED-OUT CODE OR THE SCRIPT WON'T RUN!
				// DataMiner evaluates if the script needs to launch in interactive mode.
				// This is determined by a simple string search looking for "engine.ShowUI" in the source code.
				// However, because of the toolkit NuGet package, this string cannot be found here.
				// So this comment is here as a workaround.
				//// engine.ShowUI();

				try
				{
					var domTransponderId = engine.ReadScriptParamSingleFromApp<Guid>("Transponder ID");

					satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(engine);
					var transponder = new Transponder(engine, logger, satelliteManagementHandler, domTransponderId);

					//// IAS Toolkit code
					var controller = new InteractiveController(engine);
					var dialog = new ManualSlotDialog(engine, transponder);

					// Button Actions
					dialog.BottomPanel.CancelButton.Pressed += (sender, args) => engine.ExitSuccess("Plan creation Canceled");
					dialog.BottomPanel.CreatePlanButton.Pressed += (sender, args) => CreateManualSlotInstances(engine, logger, dialog, transponder);

					controller.ShowDialog(dialog);
				}
				catch (ScriptAbortException)
				{
					// Safely exit
				}
				catch (Exception e)
				{
					logger.Error(e, $"Exception occurred in '{ScriptName}'");
					engine.ShowErrorDialog($"Error occurred while executing Create Manual Slot: {e.Message}");
				}
			}
		}

		public void CreateManualSlotInstances(IEngine engine, SatOpsLogger logger, ManualSlotDialog dialog, Transponder transponder)
		{
			var slotInstances = new List<DomApplications.SatelliteManagement.Slot>();
			var resourceInstances = new List<Resource>();
			try
			{
				var domSatellite = satelliteManagementHandler.GetSatelliteByDomInstanceId(transponder.DomTransponder.TransponderSection.TransponderSatelliteId);

				string newStatus = "active";

				var settings = new ClientMetadata
				{
					ModuleId = String.Empty,
					Prefix = String.Empty,
				};
				var resourceStudioHelper = new ResourceStudioHelper(engine, settings);

				foreach (var slot in dialog.SlotDefinitions)
				{
					var panel = (SlotPanel)slot;

					var slotName = panel.SlotName.Text;
					var centerFrequency = Convert.ToDouble(panel.CenterFrequency.Text);
					var slotSize = Convert.ToDouble(panel.SlotSize.Text);
					var startFrequency = centerFrequency - (slotSize / 2);
					var endFrequency = centerFrequency + (slotSize / 2);

					if (!IsWithinBounds(transponder, startFrequency, endFrequency))
					{
						engine.ShowErrorDialog($"Slot with center frequency {centerFrequency} MHz and size {slotSize} MHz does not fit within the transponder range{transponder.DomTransponder.TransponderSection.StartFrequency} MHz to {transponder.DomTransponder.TransponderSection.StopFrequency} MHz");
						return;
					}

					var resourceExists = resourceStudioHelper.GetResource(slotName) == null;

					if (resourceExists)
					{
						throw new NotSupportedException($"Resource '{slotName}' already exists.");
					}

					var resourceInstance = SatelliteManagementHelper.CreateSlotResource(logger, resourceStudioHelper, transponder, domSatellite.General.SatelliteName, slotSize, slotName);
					resourceInstances.Add(resourceInstance);

					var domSlotSection = new DomApplications.SatelliteManagement.SlotSection
					{
						TransponderId = transponder.DomTransponder.InstanceId,
						SlotName = slotName,
						SlotSize = Convert.ToString(slotSize),
						CenterFrequency = Convert.ToString(centerFrequency),
						SlotStartFrequency = Convert.ToString(startFrequency),
						SlotEndFrequency = Convert.ToString(endFrequency),
						ResourceId = resourceInstance.Id,
					};

					var domSlot = new DomApplications.SatelliteManagement.Slot(satelliteManagementHandler);
					domSlot.AddOrReplaceSlotSection(domSlotSection);
					domSlot.SetStatusId(newStatus);

					domSlot = satelliteManagementHandler.CreateSlot(domSlot);
					slotInstances.Add(domSlot);
				}

				engine.ExitSuccess("Finished");
			}
			catch (Exception ex)
			{
				logger.Warning(ex, $"Exception occurred in '{ScriptName}'");
				SatelliteManagementHelper.RemoveCreatedInstances(engine, satelliteManagementHandler.DomHelper, slotInstances, resourceInstances);
			}
		}

		private static bool IsWithinBounds(Transponder transponder, double startFreq, double endFreq)
		{
			return transponder.DomTransponder.TransponderSection.StartFrequency <= startFreq && endFreq <= transponder.DomTransponder.TransponderSection.StopFrequency;
		}
	}
}