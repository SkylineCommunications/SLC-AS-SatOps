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

namespace Transponders_Applied_in_Plan_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	[GQIMetaData(Name = "TranspondersAppliedInPlan")]
	public class MyDataSource : IGQIDataSource, IGQIInputArguments, IGQIOnInit
	{
		private readonly GQIStringArgument transponderPlanArg = new GQIStringArgument("Plan ID") { IsRequired = false };

		private GQIDMS dms;
		private IGQILogger logger;

		private string sDomTransponderPlanId;

		private DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;
		private Dictionary<Guid, DomApplications.SatelliteManagement.Satellite> domSatellitesById;
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
			return new GQIArgument[] { transponderPlanArg };
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			sDomTransponderPlanId = args.GetArgumentValue(transponderPlanArg);

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

			if (sDomTransponderPlanId.IsNullOrEmpty())
			{
				return rows;
			}

			var satelliteManagementDomHelper = new DomHelper(dms.SendMessages, DomApplications.DomIds.SlcSatellite_Management.ModuleId);
			satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(satelliteManagementDomHelper);

			List<DomApplications.SatelliteManagement.Transponder> transpondersList = GetTransponderList();

			var domSatelliteIds = transpondersList.Where(x => x.TransponderSection != null && x.TransponderSection.TransponderSatelliteId != Guid.Empty).Select(x => x.TransponderSection.TransponderSatelliteId).Distinct().ToList();
			domSatellitesById = domSatelliteIds.Count > 0 ? satelliteManagementHandler.GetSatellites(new ORFilterElement<DomInstance>(domSatelliteIds.Select(id => DomInstanceExposers.Id.Equal(id)).ToArray())).ToDictionary(x => x.InstanceId) : new Dictionary<Guid, DomApplications.SatelliteManagement.Satellite>();

			var domBeamIds = transpondersList.Where(x => x.TransponderSection != null && x.TransponderSection.TransponderBeamId != Guid.Empty).Select(x => x.TransponderSection.TransponderBeamId).Distinct().ToList();
			domBeamsById = domBeamIds.Count > 0 ? satelliteManagementHandler.GetBeams(new ORFilterElement<DomInstance>(domBeamIds.Select(id => DomInstanceExposers.Id.Equal(id)).ToArray())).ToDictionary(x => x.InstanceId) : new Dictionary<Guid, DomApplications.SatelliteManagement.Beam>();

			foreach (var transponder in transpondersList)
			{
				if (transponder == null)
				{
					continue;
				}

				if (transponder.Instance == null)
				{
					continue;
				}

				rows.Add(CreateNewRow(transponder));
			}

			return rows;
		}

		private List<DomApplications.SatelliteManagement.Transponder> GetTransponderList()
		{
			List<DomApplications.SatelliteManagement.Transponder> transponderList = new List<DomApplications.SatelliteManagement.Transponder>();
			if (!Guid.TryParse(sDomTransponderPlanId, out var domTransponderPlanId))
			{
				// Instead gets instances not applied to a plan
				var domTransponders = satelliteManagementHandler.GetTransponders(new TRUEFilterElement<DomInstance>()).ToList();
				var domTransponderPlans = satelliteManagementHandler.GetTransponderPlans(new TRUEFilterElement<DomInstance>()).ToList();

				foreach (var domTransponder in domTransponders)
				{
					if (domTransponderPlans.Exists(x => x.TransponderPlanSection.AppliedTransponderIds.Contains(domTransponder.InstanceId)))
					{
						continue;
					}

					transponderList.Add(domTransponder);
				}
			}
			else
			{
				var domTransponderPlan = satelliteManagementHandler.GetTransponderPlanByDomInstanceId(domTransponderPlanId);
				if (domTransponderPlan == null)
				{
					throw new ArgumentException("No transponders currently applied.");
				}

				var domTransponderIds = domTransponderPlan.TransponderPlanSection.AppliedTransponderIds.Where(x => x != Guid.Empty).Distinct().ToList();
				if (domTransponderIds.Count > 0)
				{
					transponderList = satelliteManagementHandler.GetTransponders(new ORFilterElement<DomInstance>(domTransponderIds.Select(id => DomInstanceExposers.Id.Equal(id)).ToArray())).ToList();
				}
			}

			return transponderList;
		}

		private GQIRow CreateNewRow(DomApplications.SatelliteManagement.Transponder domTransponder)
		{
			var state = domTransponder.StatusId;
			var captialized = char.ToUpper(state[0]) + state.Substring(1);

			var beamName = string.Empty;
			if (domTransponder.TransponderSection != null && domBeamsById.TryGetValue(domTransponder.TransponderSection.TransponderBeamId, out var domBeam))
			{
				beamName = domBeam.BeamSection?.BeamName;
			}

			var satelliteName = string.Empty;
			if (domTransponder.TransponderSection != null && domSatellitesById.TryGetValue(domTransponder.TransponderSection.TransponderSatelliteId, out var domSatellite))
			{
				satelliteName = domSatellite.General?.SatelliteName;
			}

			return new GQIRow(new[]
			{
				new GQICell { Value = domTransponder.TransponderSection.TransponderName },
				new GQICell { Value = satelliteName },
				new GQICell { Value = captialized },
				new GQICell { Value = beamName },
				new GQICell { Value = GetBandAsString(domTransponder.TransponderSection.Band) },
				new GQICell { Value = domTransponder.TransponderSection.Bandwidth },
				new GQICell { Value = domTransponder.TransponderSection.StartFrequency },
				new GQICell { Value = domTransponder.TransponderSection.StopFrequency },
				new GQICell { Value = GetPolarizationAsString(domTransponder.TransponderSection.Polarization) },
				new GQICell { Value = domTransponder.InstanceId.ToString() },
			});
		}
	}
}