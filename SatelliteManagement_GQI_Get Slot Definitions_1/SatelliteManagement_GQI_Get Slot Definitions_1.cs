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

10/07/2023	1.0.0.1		BSM, Skyline	Initial version
****************************************************************************
*/

namespace Script
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	[GQIMetaData(Name = "Satellite Management - Get Slot Definitions")]
	public class ResourceManagementDataSource : IGQIDataSource, IGQIInputArguments, IGQIOnInit
	{
		private readonly GQIStringArgument transponderPlanArg = new GQIStringArgument("Transponder Plan ID") { IsRequired = false };

		private GQIDMS dms;

		private Guid domTransponderPlanId;

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[]
			{
				new GQIStringColumn("Slot name"),
				new GQIDoubleColumn("Slot size"),
				new GQIDoubleColumn("Relative start frequency"),
				new GQIDoubleColumn("Relative end frequency"),
			};
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { transponderPlanArg };
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return new GQIPage(GetSlotDefinitions())
			{
				HasNextPage = false,
			};
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			if (!Guid.TryParse(args.GetArgumentValue(transponderPlanArg), out domTransponderPlanId))
			{
				domTransponderPlanId = Guid.Empty;
			}

			return new OnArgumentsProcessedOutputArgs();
		}

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			dms = args.DMS;

			return default;
		}

		private GQIRow[] GetSlotDefinitions()
		{
			var rows = new List<GQIRow>();

			if (domTransponderPlanId == Guid.Empty)
			{
				return rows.ToArray();
			}

			var satelliteManagementDomHelper = new DomHelper(dms.SendMessages, DomApplications.DomIds.SlcSatellite_Management.ModuleId);
			var satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(satelliteManagementDomHelper);

			var domTransponderPlan = satelliteManagementHandler.GetTransponderPlanByDomInstanceId(domTransponderPlanId);
			if (domTransponderPlan == null)
			{
				return rows.ToArray();
			}

			foreach (var slotDefinition in domTransponderPlan.SlotDefinitions)
			{
				rows.Add(new GQIRow(new[]
					{
						new GQICell{Value = slotDefinition.Name},
						new GQICell{Value = slotDefinition.Size},
						new GQICell{Value = slotDefinition.StartFrequency},
						new GQICell{Value = slotDefinition.EndFrequency},
					}));
			}

			return rows.ToArray();
		}
	}
}