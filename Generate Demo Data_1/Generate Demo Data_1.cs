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

12/03/2024	1.0.0.1		JVT, Skyline	Initial version
****************************************************************************
*/

namespace Generate_Demo_Data_1
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Skyline.DataMiner.Automation;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public static void Run(IEngine engine)
		{
			engine.Timeout = TimeSpan.FromHours(1);
			if (!IsDemoSystem())
			{
				// safety to not uninstall things, when not a demo system!
				throw new InvalidOperationException("This is not a demo system. Add a file DemoSystem.txt to the general documents folder to make it a demo system.");
			}

			if (!DetectDemoData())
			{
				// safety to not uninstall things, when no demo data is present!
				throw new InvalidOperationException("No demo data found");
			}

			var exceptions = new List<Exception>();

			if (!TryUninstallDemoData(engine, out var uninstallException))
			{
				exceptions.Add(uninstallException);
			}

			if (!TryInstallDemoData(engine, out var installException))
			{
				exceptions.Add(installException);
			}

			if (exceptions.Any())
			{
				throw new AggregateException(exceptions);
			}
		}

		private static bool DetectDemoData()
		{
			var dir = new DirectoryInfo(@"C:\Skyline DataMiner\Documents\DataMiner Solutions\SatOps\Demo");

			return dir.Exists && dir.EnumerateFiles("*.json", SearchOption.AllDirectories).Any();
		}

		private static bool IsDemoSystem()
		{
			var demoSystemFilePath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\DemoSystem.txt";
			return File.Exists(demoSystemFilePath);
		}

		private static bool TryUninstallDemoData(IEngine engine, out Exception exception)
		{
			var exceptions = new List<Exception>();

			Satellite.Uninstall(engine, exceptions);

			if (exceptions.Any())
			{
				exception = new AggregateException(exceptions);
				return false;
			}

			exception = null;
			return true;
		}

		private static bool TryInstallDemoData(IEngine engine, out Exception exception)
		{
			var exceptions = new List<Exception>();

			Satellite.Import(engine, exceptions);

			if (exceptions.Any())
			{
				exception = new AggregateException(exceptions);
				return false;
			}

			exception = null;
			return true;
		}
	}
}