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

04/01/2024	1.0.0.1		CMT, Skyline	Initial version
****************************************************************************
*/

namespace Import_Satellite_Data_1
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-IAS-Import Satellite Data";
		private ImportDialog import;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(Engine engine)
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
					var controller = new InteractiveController(engine);
					var importController = new ImportController(engine);
					import = new ImportDialog(engine);
					import.ImportButton.Pressed += (sender, args) => importController.ProcessImportFile(import);
					import.CloseButton.Pressed += (sender, args) => engine.ExitSuccess("IAS Closed");

					controller.ShowDialog(import);
				}
				catch (ScriptAbortException)
				{
					// ignore abort
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Exception occurred in '{ScriptName}'");
					engine.ShowErrorDialog($"Error occurred while importing Satellite Data: {ex.Message}");
				}
			}
		}
	}
}