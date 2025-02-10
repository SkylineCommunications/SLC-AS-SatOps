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

01/10/2024	1.0.0.1		JVW, Skyline	Initial version
****************************************************************************
*/

namespace SatelliteManagement_Core_TransponderHandler_1
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	using SatelliteManagement_Core_TransponderHandler_1.ActionHandlers;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-AS-Core_TransponderHandler";

		private IEngine engine;

		private TransponderHandlerData transponderHandlerData;

		private ScriptData scriptData;

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
					RunSafe(logger);
				}
				catch (ScriptAbortException)
				{
					// Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
					throw; // Comment if it should be treated as a normal exit of the script.
				}
				catch (ScriptForceAbortException)
				{
					// Catch forced abort exceptions, caused via external maintenance messages.
					throw;
				}
				catch (ScriptTimeoutException)
				{
					// Catch timeout exceptions for when a script has been running for too long.
					throw;
				}
				catch (InteractiveUserDetachedException)
				{
					// Catch a user detaching from the interactive script by closing the window.
					// Only applicable for interactive scripts, can be removed for non-interactive scripts.
					throw;
				}
				catch (Exception e)
				{
					logger.Error(e, $"Exception occurred in '{ScriptName}'");

					transponderHandlerData.Output.CaptureException(e);
				}
				finally
				{
					if (transponderHandlerData.Communication == ScriptDataBase.CommunicationType.Json)
					{
						engine.AddOrUpdateScriptOutput(transponderHandlerData.OutputReturnKey, JsonConvert.SerializeObject(transponderHandlerData, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = new KnownTypesBinder() }));
					}
				}
			}
		}

		private void RunSafe(SatOpsLogger logger)
		{
			scriptData = new ScriptData(engine, logger);
			transponderHandlerData = ScriptDataManager.GetData<TransponderHandlerData>(engine.GetScriptParam("Input Data").Value, new KnownTypesBinder());

			var handler = InitializeActionHandler();
			transponderHandlerData.Output.ActionOutput = handler.Execute();
			transponderHandlerData.Output.TraceData = handler.TraceData;
		}

		private IActionHandler InitializeActionHandler()
		{
			Dictionary<Type, Func<IActionHandler>> handlerMapping = new Dictionary<Type, Func<IActionHandler>>
			{
				[typeof(ExecuteTransponderAction)] = () => new ExecuteTransponderActionHandler(scriptData, transponderHandlerData.Input as ExecuteTransponderAction),
			};

			if (!handlerMapping.TryGetValue(transponderHandlerData.Input.GetType(), out var initAction))
			{
				throw new NotSupportedException($"Type '{transponderHandlerData.Input.GetType()}' is not supported.");
			}

			return initAction();
		}
	}
}