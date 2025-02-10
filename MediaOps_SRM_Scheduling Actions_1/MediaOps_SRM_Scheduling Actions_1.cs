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

27/08/2024	1.0.0.1		JVW, Skyline	Initial version
****************************************************************************
*/

namespace MediaOps_SRM_Scheduling_Actions_1
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Utils.MediaOps.Common.SRM;
	using Skyline.DataMiner.Utils.MediaOps.Common.Take.ScriptData;
	using Skyline.DataMiner.Utils.MediaOps.Common.Utils;
	using Skyline.DataMiner.Utils.ScriptPerformanceLogger;

	using DomApplications = Skyline.DataMiner.Utils.MediaOps.Common.DOM.Applications;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "MediaOps_SRM_Scheduling Actions";

		private IEngine engine;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			this.engine = engine;

			using (var logger = new MediaOpsLogger(MediaOpsLogger.Types.Jobs))
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

					engine.ExitFail("Run|Something went wrong: " + e);
				}
			}
		}

		private static void UpdateDom_Start(DomApplications.Workflow.WorkflowHandler workflowHandler, DomApplications.Workflow.Job domJob)
		{
			if (domJob.StatusId != DomApplications.DomIds.SlcWorkflow.Behaviors.Job_Behavior.Statuses.Confirmed)
			{
				return;
			}

			workflowHandler.DomHelper.DomInstances.DoStatusTransition(new DomInstanceId(domJob.InstanceId), DomApplications.DomIds.SlcWorkflow.Behaviors.Job_Behavior.Transitions.Confirmed_To_Running);
		}

		private static void UpdateDom_Stop(DomApplications.Workflow.WorkflowHandler workflowHandler, DomApplications.Workflow.Job domJob)
		{
			if (domJob.StatusId != DomApplications.DomIds.SlcWorkflow.Behaviors.Job_Behavior.Statuses.Running)
			{
				return;
			}

			workflowHandler.DomHelper.DomInstances.DoStatusTransition(new DomInstanceId(domJob.InstanceId), DomApplications.DomIds.SlcWorkflow.Behaviors.Job_Behavior.Transitions.Running_To_Completed);
		}

		private static void ExecuteWorkflow(IEngine engine, WorkflowExecutionAction action, DomApplications.Workflow.Job job, PerformanceLogger performanceLogger)
		{
			if (job.Info.IsSourceControlSurface())
			{
				return;
			}

			using (var meas = performanceLogger.StartMeasurement())
			{
				var jobExecutionScriptName = job.ExecutionScriptName;

				if (String.IsNullOrWhiteSpace(jobExecutionScriptName))
				{
					jobExecutionScriptName = Skyline.DataMiner.Utils.MediaOps.Common.ScriptNames.WorkflowDefault;
				}

				AddMetadataToPerformanceMeasurement(meas, action, jobExecutionScriptName, job);

				var dataProvider = new Skyline.DataMiner.Utils.MediaOps.Common.Take.DataProvider(engine);

				var data = new WorkflowData(dataProvider, performanceLogger)
				{
					Input = new WorkflowInput
					{
						Action = action,
						JobOrWorkflow = job,
					},
				};

				using (data)
				{
					var subScript = engine.PrepareSubScript(jobExecutionScriptName);
					subScript.Synchronous = true;
					subScript.LockElements = true;
					subScript.SelectScriptParam("Input", data.Key);
					subScript.StartScript();

					data.Output.RethrowException();
				}
			}
		}

		private static void AddMetadataToPerformanceMeasurement(Measurement meas, WorkflowExecutionAction action, string jobExecutionScriptName, DomApplications.Workflow.Job job)
		{
			meas.SetMetadata("Workflow_Action", Convert.ToString(action));
			meas.SetMetadata("Workflow_Script", jobExecutionScriptName);
			meas.SetMetadata("Job_ID", Convert.ToString(job.InstanceId));
			meas.SetMetadata("Job_Name", job.Name);
		}

		private void RunSafe(MediaOpsLogger logger)
		{
			var reservationId = Guid.Parse(engine.GetScriptParam("Reservation ID").Value);
			var action = engine.GetScriptParam("Action").Value;

			using (var performanceLogger = new PerformanceLogger($"MediaOps_SRM_Scheduling Actions-{action}"))
			using (performanceLogger.StartMeasurement())
			{
				try
				{
					ExecuteActions(reservationId, action, performanceLogger);
				}
				catch (Exception e)
				{
					// notify failure
					logger.Warning("Exception occurred in MediaOps_SRM_Scheduling Actions: " + e);
					throw;
				}
			}
		}

		private void ExecuteActions(Guid reservationId, string action, PerformanceLogger performanceLogger)
		{
			using (performanceLogger.StartMeasurement())
			{
				var workflowHandler = new DomApplications.Workflow.WorkflowHandler(engine);
				if (!TryGetJobId(reservationId, out var domJobId))
				{
					return;
				}

				using (DomApplications.Workflow.WorkflowHandler.LockJob(domJobId))
				{
					var domJob = workflowHandler.GetJobByDomInstanceId(domJobId);
					if (domJob == null)
					{
						return;
					}

					switch (action)
					{
						case KnownEvents.StartEventName:
							UpdateDom_Start(workflowHandler, domJob);
							ExecuteWorkflow(engine, WorkflowExecutionAction.Connect, domJob, performanceLogger);
							break;

						case KnownEvents.StopEventName:
							UpdateDom_Stop(workflowHandler, domJob);
							ExecuteWorkflow(engine, WorkflowExecutionAction.Disconnect, domJob, performanceLogger);
							break;

						default:
							// do nothing
							break;
					}
				}
			}
		}

		private bool TryGetJobId(Guid reservationId, out Guid domJobId)
		{
			domJobId = Guid.Empty;

			var srmHelpers = new SrmHelpers(engine);
			var coreReservation = srmHelpers.ResourceManagerHelper.GetReservationInstance(reservationId);
			if (coreReservation == null
				|| !coreReservation.Properties.Dictionary.TryGetValue("Job ID", out var jobIdProperty)
				|| !Guid.TryParse(Convert.ToString(jobIdProperty), out domJobId))
			{
				return false;
			}

			return domJobId != Guid.Empty;
		}
	}
}