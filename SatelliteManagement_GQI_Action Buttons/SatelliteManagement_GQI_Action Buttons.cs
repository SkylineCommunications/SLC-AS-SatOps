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

dd/mm/2024	1.0.0.1		BSM, Skyline	Initial version
****************************************************************************
*/

namespace SatelliteManagement.GQI.DOM.Get_DOM_Buttons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;

	[GQIMetaData(Name = "SAT-Get DOM Buttons")]
	public class SatOpsDataSource : IGQIDataSource, IGQIInputArguments, IGQIOnInit
	{
		private readonly GQIStringArgument domIdArg = new GQIStringArgument("DOM ID") { IsRequired = false };

		private GQIDMS dms;

		private Guid domId;

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[]
			{
				new GQIStringColumn("Button Name"),
			};
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { domIdArg };
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return new GQIPage(GetActionButtons())
			{
				HasNextPage = false,
			};
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			if (!Guid.TryParse(args.GetArgumentValue(domIdArg), out domId))
			{
				domId = Guid.Empty;
			}

			return new OnArgumentsProcessedOutputArgs();
		}

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			dms = args.DMS;

			return default;
		}

		private GQIRow[] GetActionButtons()
		{
			var rows = new List<GQIRow>();

			if (domId == Guid.Empty)
			{
				return rows.ToArray();
			}

			var domHelper = new DomHelper(dms.SendMessages, "(slc)satellite_management");
			var domInstance = domHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(new DomInstanceId(domId))).Single();
			var statusId = domInstance.StatusId;
			switch (statusId)
			{
				case "draft":
					AddButton("Activate", rows);
					return rows.ToArray();

				case "active":
				case "error":
					AddButton("Deprecate", rows);
					AddButton("Edit", rows);
					return rows.ToArray();

				case "edit":
					AddButton("Activate", rows);
					return rows.ToArray();

				case "deprecated":
					AddButton("Activate", rows);
					return rows.ToArray();

				default:
					return rows.ToArray();
			}
		}

		private static void AddButton(string buttonName, List<GQIRow> rows)
		{
			rows.Add(new GQIRow(new[]
			{
				new GQICell{Value = buttonName},
			}));
		}
	}
}