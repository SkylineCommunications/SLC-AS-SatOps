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

namespace SatelliteManagement_GQI_Transponders_In_Satellite_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	[GQIMetaData(Name = "Transponders In Satellite")]
	public class MyDataSource : IGQIDataSource, IGQIInputArguments, IGQIOnInit
	{
		private readonly GQIStringArgument domSatelliteIdArg = new GQIStringArgument("Satellite ID") { IsRequired = true };

		private GQIDMS dms;
		private IGQILogger logger;

		private Guid domSatelliteId;

		private DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;
		private DomApplications.SatelliteManagement.Satellite domSatellite;
		private Dictionary<Guid, DomApplications.SatelliteManagement.Beam> domBeamsById;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			dms = args.DMS;

			logger = args.Logger;
			logger.MinimumLogLevel = GQILogLevel.Debug;

			return default;
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { domSatelliteIdArg };
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			if (!Guid.TryParse(args.GetArgumentValue(domSatelliteIdArg), out domSatelliteId))
			{
				domSatelliteId = Guid.Empty;
			}

			return new OnArgumentsProcessedOutputArgs();
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[]
			{
				new GQIStringColumn("Transponder Name"),
				new GQIStringColumn("Satellite Name"),
				new GQIStringColumn("State"),
				new GQIStringColumn("Beam Name"),
				new GQIStringColumn("Band"),
				new GQIDoubleColumn("Bandwidth"),
				new GQIDoubleColumn("Start Frequency"),
				new GQIDoubleColumn("End Frequency"),
				new GQIStringColumn("Polarization"),
				new GQIStringColumn("ID"),
				new GQIStringColumn("Transponder Plan"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			try
			{
				var rows = BuildAllGqiRows().ToList();

				return new GQIPage(rows.ToArray())
				{
					HasNextPage = false,
				};
			}
			catch (Exception e)
			{
				logger.Error(e.ToString());
				throw new GenIfException(e.Message);
			}
		}

		private static string GetBandAsString(DomApplications.DomIds.SlcSatellite_Management.Enums.BandEnum? enumValue)
		{
			if (!enumValue.HasValue)
			{
				return string.Empty;
			}

			return DomApplications.DomIds.SlcSatellite_Management.Enums.Band.ToValue(enumValue.Value);
		}

		private static string GetPolarizationAsString(DomApplications.DomIds.SlcSatellite_Management.Enums.PolarizationEnum? enumValue)
		{
			if (!enumValue.HasValue)
			{
				return string.Empty;
			}

			return DomApplications.DomIds.SlcSatellite_Management.Enums.Polarization.ToValue(enumValue.Value);
		}

		private IEnumerable<GQIRow> BuildAllGqiRows()
		{
			var rows = new List<GQIRow>();

			if (domSatelliteId == Guid.Empty)
			{
				rows.Add(CreateErrorRow("Error: Satellite Id is not in the right format."));
				return rows;
			}

			var satelliteManagementDomHelper = new DomHelper(dms.SendMessages, DomApplications.DomIds.SlcSatellite_Management.ModuleId);
			satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(satelliteManagementDomHelper);

			domSatellite = satelliteManagementHandler.GetSatelliteByDomInstanceId(domSatelliteId);

			var transponderFilter = DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.Transponder.TransponderSatellite).Equal(domSatellite.InstanceId);
			var domTransponders = satelliteManagementHandler.GetTransponders(transponderFilter);

			var domBeamIds = domTransponders.Where(x => x.TransponderSection.TransponderBeamId != Guid.Empty).Select(x => x.TransponderSection.TransponderBeamId).Distinct().ToList();
			domBeamsById = domBeamIds.Count > 0 ? satelliteManagementHandler.GetBeams(new ORFilterElement<DomInstance>(domBeamIds.Select(x => DomInstanceExposers.Id.Equal(x)).ToArray())).ToDictionary(x => x.InstanceId) : new Dictionary<Guid, DomApplications.SatelliteManagement.Beam>();

			foreach (var domTransponder in domTransponders)
			{
				rows.Add(CreateNewRow(domTransponder));
			}

			return rows;
		}

		private GQIRow CreateNewRow(DomApplications.SatelliteManagement.Transponder domTransponder)
		{
			var beamName = string.Empty;
			if (domBeamsById.TryGetValue(domTransponder.TransponderSection.TransponderBeamId, out var domBeam))
			{
				beamName = domBeam.BeamSection?.BeamName;
			}

			var planName = string.Empty;
			var domTransponderPlan = satelliteManagementHandler.GetTransponderPlans(DomInstanceExposers.FieldValues.DomInstanceField(DomApplications.DomIds.SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds).Contains(Convert.ToString(domTransponder.Instance))).FirstOrDefault();
			if (domTransponderPlan != null)
			{
				planName = domTransponderPlan.TransponderPlanSection?.PlanName;
			}

			return new GQIRow(new[]
			{
				new GQICell { Value = domTransponder.TransponderSection.TransponderName },
				new GQICell { Value = domSatellite.General.SatelliteName },
				new GQICell { Value = domTransponder.GetStatus() },
				new GQICell { Value = beamName },
				new GQICell { Value = GetBandAsString(domTransponder.TransponderSection.Band) },
				new GQICell { Value = domTransponder.TransponderSection.Bandwidth },
				new GQICell { Value = domTransponder.TransponderSection.StartFrequency },
				new GQICell { Value = domTransponder.TransponderSection.StopFrequency },
				new GQICell { Value = GetPolarizationAsString(domTransponder.TransponderSection.Polarization) },
				new GQICell { Value = domTransponder.InstanceId.ToString() },
				new GQICell { Value = planName },
			});
		}

		private GQIRow CreateErrorRow(string message)
		{
			return new GQIRow(new[]
			{
				new GQICell {Value = message},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
				new GQICell {},
			});
		}
	}
}