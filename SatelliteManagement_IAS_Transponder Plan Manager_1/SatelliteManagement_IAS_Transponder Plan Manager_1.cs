/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
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

19/12/2023	1.0.0.1		BSM, Skyline	Initial version
****************************************************************************
*/

namespace Transponder_Plan_Manager_1
{
	using System;
	using System.Globalization;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Utils.DOM.Builders;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using TransponderPlanManager;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-IAS-Transponder Plan Manager";

		public enum Action
		{
			Create = 1,
			Update = 2,
		}

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
					var planId = engine.ReadScriptParamSingleFromApp<Guid>("Plan ID", Guid.Empty);
					var action = Action.Update;

					if (planId == Guid.Empty)
					{
						action = Action.Create;
					}

					//// IAS Toolkit code
					var controller = new InteractiveController(engine);
					var dialog = new PlanDialog(engine, action, planId);

					// Button Actions
					dialog.BottomPanel.CancelButton.Pressed += (sender, args) => engine.ExitSuccess("Plan creation Canceled");
					dialog.BottomPanel.CreatePlanButton.Pressed += (sender, args) => CreatePlanDOM(engine, dialog);

					controller.Run(dialog);
				}
				catch (ScriptAbortException)
				{
					// ignore abort
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Exception occurred in '{ScriptName}'");
					engine.ShowErrorDialog($"Error occurred during Transponder Plan Manager: {ex.Message}");
				}
			}
		}

		public void CreatePlanDOM(IEngine engine, PlanDialog dialog)
		{
			if (dialog.DetailsPanel.PlanNameTextBox.Text.IsNullOrEmpty())
			{
				dialog.DetailsPanel.PlanNameTextBox.ValidationState = UIValidationState.Invalid;
				dialog.DetailsPanel.PlanNameTextBox.PlaceHolder = "Plan Name cannot be empty.";
				return;
			}

			var domHelper = new DomHelper(engine.SendSLNetMessages, "(slc)satellite_management");

			var domGuid = Guid.NewGuid();
			string newStatus = "draft";
			if (dialog.Action == Action.Update)
			{
				domHelper.DomInstances.Delete(dialog.DomInstanceToUpdate);
				domGuid = dialog.DomInstanceToUpdate.ID.Id;
				newStatus = dialog.DomInstanceToUpdate.StatusId;
			}

			var instanceBuilder = new DomInstanceBuilder(SlcSatellite_Management.Definitions.TransponderPlans)
				.WithID(domGuid)
				.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.TransponderPlan.Id)
					.WithFieldValue(SlcSatellite_Management.Sections.TransponderPlan.PlanName, dialog.DetailsPanel.PlanNameTextBox.Text)
					.WithFieldValue(SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds, String.Empty));

			foreach (var slot in dialog.SlotDefinitions)
			{
				var panel = (SlotDefinitionPanel)slot;

				instanceBuilder.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.SlotDefinition.Id)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotName, panel.SlotName.Text)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotSize, Convert.ToDouble(panel.SlotSize.Text, CultureInfo.InvariantCulture))
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.RelativeStartFrequency, Convert.ToDouble(panel.RelativeStartFrequency.Text, CultureInfo.InvariantCulture))
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.RelativeEndFrequency, Convert.ToDouble(panel.RelativeEndFrequency.Text, CultureInfo.InvariantCulture)));
			}

			var instance = instanceBuilder.Build();
			instance.StatusId = newStatus;
			domHelper.DomInstances.Create(instance);

			engine.ExitSuccess("Finished");
		}
	}
}