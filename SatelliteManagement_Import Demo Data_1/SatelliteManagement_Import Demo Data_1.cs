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

namespace SatelliteManagement_Import_Demo_Data_1
{
	using System;
	using System.IO;
	using NPOI.SS.UserModel;
	using NPOI.XSSF.UserModel;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ScriptName = "SAT-AS-Import Demo Data";

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			using (var logger = new SatOpsLogger(SatOpsLogger.Types.Satellite))
			{
				var filePath = @"C:\Skyline DataMiner\Documents\DataMiner Solutions\SatOps\Demo\Satellite Management_Demo Data.xlsx";
				if (File.Exists(filePath))
				{
					ProcessFileData(engine, logger, filePath);
				}
				else
				{
					logger.Warning($"An error occurred while trying to create DOM Instances from data in file: {Path.GetFileName(filePath)}");
				}
			}
		}

		private static void ProcessFileData(IEngine engine, SatOpsLogger logger, string filePath)
		{
			try
			{
				if (!Path.GetExtension(filePath).Equals(".xls") && !Path.GetExtension(filePath).Equals(".xlsx"))
				{
					logger.Warning($"File extension: '{Path.GetExtension(filePath)}' not supported.");
					return;
				}

				var satellites = new Satellites();
				var beams = new Beams();
				var transponders = new Transponders();
				var transponderPlans = new TransponderPlans();

				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					IWorkbook workbook = new XSSFWorkbook(fileStream);

					satellites.GetRows(workbook.GetSheet("Satellites"));
					beams.GetRows(workbook.GetSheet("Beams"));
					transponders.GetRows(workbook.GetSheet("Transponders"));
					transponderPlans.GetRows(workbook.GetSheet("Transponder Plans"));
				}

				var satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(engine);

				satellites.CreateInstances(engine, logger, satelliteManagementHandler);
				beams.CreateInstances(engine, logger, satelliteManagementHandler);
				transponders.CreateInstances(engine, logger, satelliteManagementHandler);
				transponderPlans.CreateInstances(engine, logger, satelliteManagementHandler);
			}
			catch (Exception ex)
			{
				logger.Error(ex, $"Exception occurred in '{ScriptName}' while creating instances");
			}
		}
	}
}