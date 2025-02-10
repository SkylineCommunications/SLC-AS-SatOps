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

namespace SatelliteManagement_DOM_Button_Actions_1
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-AS-DOM Button Actions";

		private readonly Dictionary<DomDefinitionId, Func<IEngine, SatOpsLogger, DomApplications.SatelliteManagement.SatelliteManagementHandler, Guid, ISatelliteManagementBase>> actionMapping = new Dictionary<DomDefinitionId, Func<IEngine, SatOpsLogger, DomApplications.SatelliteManagement.SatelliteManagementHandler, Guid, ISatelliteManagementBase>>
		{
			{ SlcSatellite_Management.Definitions.Satellites, (engine, logger, satelliteManagementHandler, domInstanceId) => new Satellite(engine, logger, satelliteManagementHandler, domInstanceId) },
			{ SlcSatellite_Management.Definitions.Beams, (engine, logger, satelliteManagementHandler, domInstanceId) => new Beam(engine, logger,satelliteManagementHandler, domInstanceId) },
			{ SlcSatellite_Management.Definitions.Transponders, (engine,logger, satelliteManagementHandler, domInstanceId) => new Transponder(engine, logger, satelliteManagementHandler, domInstanceId) },
			{ SlcSatellite_Management.Definitions.TransponderPlans, (engine,logger, satelliteManagementHandler, domInstanceId) => new TransponderPlan(engine,logger, satelliteManagementHandler, domInstanceId) },
			{ SlcSatellite_Management.Definitions.Slots, (engine,logger, satelliteManagementHandler, domInstanceId) => new Slot(engine,logger, satelliteManagementHandler, domInstanceId) },
		};

		private IEngine engine;
		private ISatelliteManagementBase satelliteManagementObj;

		private static void HandleAction(ISatelliteManagementBase satelliteManagementObj, string actionId)
		{
			switch (actionId)
			{
				case "activate":
					satelliteManagementObj.Activate();
					break;
				case "deprecate":
					satelliteManagementObj.Deprecate();
					break;
				case "edit":
					satelliteManagementObj.Edit();
					break;
				case "error":
					satelliteManagementObj.Error();
					break;
				default:
					throw new ArgumentException("Invalid action ID");
			}
		}

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
					// DO NOT REMOVE THIS COMMENTED-OUT CODE OR THE SCRIPT WON'T RUN!
					// DataMiner evaluates if the script needs to launch in interactive mode.
					// This is determined by a simple string search looking for "engine.ShowUI" in the source code.
					// However, because of the toolkit NuGet package, this string cannot be found here.
					// So this comment is here as a workaround.
					//// engine.ShowUI();

					RunSafe(logger);
				}
				catch (ScriptAbortException)
				{
					// do nothing
				}
				catch (InteractiveUserDetachedException)
				{
					// do nothing
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Exception occurred in '{ScriptName}'");

					if (satelliteManagementObj != null)
					{
						satelliteManagementObj.Error();
					}

					engine.ShowErrorDialog($"Error while trying to transition instance: {ex.Message}");
				}
			}
		}

		private void RunSafe(SatOpsLogger logger)
		{
			var domInstanceId = engine.ReadScriptParamSingleFromApp<Guid>("DOM ID");
			var buttonAction = engine.ReadScriptParamSingleFromApp<string>("Button Action");
			var satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(engine);
			var instance = satelliteManagementHandler.DomHelper.DomInstances.GetByID(domInstanceId);
			var actionId = buttonAction.ToLowerInvariant();

			if (!actionMapping.TryGetValue(instance.DomDefinitionId, out var mappingAction))
			{
				throw new NotSupportedException($"Dom Definition {instance.GetDomDefinition().Name} not supported.");
			}

			satelliteManagementObj = mappingAction(engine, logger, satelliteManagementHandler, domInstanceId);
			HandleAction(satelliteManagementObj, actionId);
		}
	}
}