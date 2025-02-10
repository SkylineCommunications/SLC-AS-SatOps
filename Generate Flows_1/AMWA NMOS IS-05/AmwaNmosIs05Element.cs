namespace Generate_Flows_1.AMWA_NMOS_IS_05
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Common.DOM.Applications.DomIds;

	public class AmwaNmosIs05Element : SupportedElement
	{
		private Dictionary<string, ReceiversQActionRow> destinations;
		private Dictionary<string, SendersQActionRow> sources;

		public AmwaNmosIs05Element(
			IDmsElement element,
			ICrudHelperComponent<DomInstance> virtualSignalGroupHelper) : base(element, virtualSignalGroupHelper)
		{
		}

		public override IEnumerable<string> Destinations
		{
			get
			{
				if (destinations == null)
				{
					GetDestinations();
				}

				return destinations.Keys;
			}
		}

		public override IEnumerable<string> Sources
		{
			get
			{
				if (sources == null)
				{
					GetSources();
				}

				return sources.Keys;
			}
		}

		public override bool SupportsVirtualSignalGroupGeneration => false;

		public override void GenerateDestinations(IEnumerable<string> selectedDestinationKeys, bool generateVsGroups, DomInstance groupType)
		{
			foreach (var row in selectedDestinationKeys)
			{
				var rowValue = destinations[row];

				GenerateDestinationFlow(rowValue, true);
				GenerateDestinationFlow(rowValue, false);
			}
		}

		public override void GenerateSources(IEnumerable<string> selectedSourceKeys, bool generateVsGroups, DomInstance groupType)
		{
			foreach (var row in selectedSourceKeys)
			{
				var rowValue = sources[row];

				GenerateSourceFlow(rowValue, true);
				GenerateSourceFlow(rowValue, false);
			}
		}

		private void GenerateDestinationFlow(ReceiversQActionRow row, bool primary)
		{
			var interfaceBinding = primary ?
				(string)row.Receiversprimaryinterfacebinding_119 :
				(string)row.Receiverssecondaryinterfacebinding_120;

			var interfaceId = Interfaces.FirstOrDefault(i => i.DynamicLink == $"1;{interfaceBinding}")?
				.Id.ToString();

			if (String.IsNullOrEmpty(interfaceId))
			{
				// if interface is not found then don't generate this flow
				return;
			}

			FilterElement<DomInstance> elementFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Element)
						.Equal(element.DmsElementId.Value);
			FilterElement<DomInstance> subInterfaceFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.SubInterface)
						.Equal(row.Key);
			FilterElement<DomInstance> interfaceFilter =
				DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Interface)
					.Equal(interfaceId);

			var flow = virtualSignalGroupHelper.Read(elementFilter.AND(subInterfaceFilter).AND(interfaceFilter)).FirstOrDefault();

			var flowExists = flow != null;
			if (flowExists)
			{
				// nothing to update
				return;
			}

			flow = new DomInstance
			{
				DomDefinitionId = SlcVirtualsignalgroup.Definitions.Flow,
			};

			var name = (string)row.Receiversdescription_114 ?? row.Key;
			name = $"{element.Name} {name} {(primary ? " Primary" : " Secondary")}";

			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowInfo.Id,
				SlcVirtualsignalgroup.Sections.FlowInfo.Name,
				name);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowInfo.Id,
				SlcVirtualsignalgroup.Sections.FlowInfo.OperationalState,
				(int)SlcVirtualsignalgroup.Enums.OperationalState.Up);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowInfo.Id,
				SlcVirtualsignalgroup.Sections.FlowInfo.AdministrativeState,
				(int)SlcVirtualsignalgroup.Enums.AdministrativeState.Up);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowInfo.Id,
				SlcVirtualsignalgroup.Sections.FlowInfo.TransportType,
				(int)SlcVirtualsignalgroup.Enums.TransportType.ST211020);

			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowPath.Id,
				SlcVirtualsignalgroup.Sections.FlowPath.Element,
				element.DmsElementId.Value);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowPath.Id,
				SlcVirtualsignalgroup.Sections.FlowPath.Interface,
				interfaceId);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowPath.Id,
				SlcVirtualsignalgroup.Sections.FlowPath.SubInterface,
				row.Key);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowPath.Id,
				SlcVirtualsignalgroup.Sections.FlowPath.FlowDirection,
				(int)SlcVirtualsignalgroup.Enums.FlowDirection.Rx);

			virtualSignalGroupHelper.Create(flow);
		}

		private void GenerateSourceFlow(SendersQActionRow row, bool primary)
		{
			var interfaceBinding = primary ?
				(string)row.Sendersprimaryinterfacebinding_318 :
				(string)row.Senderssecondaryinterfacebinding_319;

			var interfaceId = Interfaces.FirstOrDefault(i => i.DynamicLink == $"1;{interfaceBinding}")?
				.Id.ToString();

			if (String.IsNullOrEmpty(interfaceId))
			{
				// if interface is not found then don't generate this flow
				return;
			}

			FilterElement<DomInstance> elementFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Element)
						.Equal(element.DmsElementId.Value);
			FilterElement<DomInstance> subInterfaceFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.SubInterface)
						.Equal(row.Key);
			FilterElement<DomInstance> interfaceFilter =
				DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Interface)
					.Equal(interfaceId);

			var flow = virtualSignalGroupHelper.Read(elementFilter.AND(subInterfaceFilter).AND(interfaceFilter)).FirstOrDefault();

			var flowExists = flow != null;
			if (!flowExists)
			{
				flow = new DomInstance
				{
					DomDefinitionId = SlcVirtualsignalgroup.Definitions.Flow,
				};

				var name = (string)row.Sendersdescription_314 ?? row.Key;
				name = $"{element.Name} {name} {(primary ? " Primary" : " Secondary")}";

				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.Name,
					name);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.OperationalState,
					(int)SlcVirtualsignalgroup.Enums.OperationalState.Up);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.AdministrativeState,
					(int)SlcVirtualsignalgroup.Enums.AdministrativeState.Up);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.TransportType,
					(int)SlcVirtualsignalgroup.Enums.TransportType.ST211020);

				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Element,
					element.DmsElementId.Value);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Interface,
					interfaceId);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.SubInterface,
					row.Key);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.FlowDirection,
					(int)SlcVirtualsignalgroup.Enums.FlowDirection.Tx);
			}

			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowTransport.Id,
				SlcVirtualsignalgroup.Sections.FlowTransport.SourceIP,
				primary ? (string)row.Senderstransportparamsprimarysourceip_304 : (string)row.Senderstransportparamssecondarysourceip_309);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowTransport.Id,
				SlcVirtualsignalgroup.Sections.FlowTransport.DestinationIP,
				primary ? (string)row.Senderstransportparamsprimarydestinationip_307 : (string)row.Senderstransportparamssecondarydestinationip_312);
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowTransport.Id,
				SlcVirtualsignalgroup.Sections.FlowTransport.DestinationPort,
				primary ? Convert.ToInt64(row.Senderstransportparamsprimarydestinationport_306) : Convert.ToInt64(row.Senderstransportparamssecondarydestinationport_311));
			flow.AddOrUpdateFieldValue(
				SlcVirtualsignalgroup.Sections.FlowTransport.Id,
				SlcVirtualsignalgroup.Sections.FlowTransport.BitrateMbps,
				1500.0);

			if (flowExists)
			{
				virtualSignalGroupHelper.Update(flow);
			}
			else
			{
				virtualSignalGroupHelper.Create(flow);
			}
		}

		private void GetDestinations()
		{
			IEnumerable<IGrouping<string, ReceiversQActionRow>> destinationsGroupedByLabel =
				element.GetTable(Parameter.Receivers.tablePid)
					.GetRows()
					.Select(row => new ReceiversQActionRow(row))
					.GroupBy(row => Convert.ToString(row.Receiversdescription_114).Replace(';', ','));

			destinations = new Dictionary<string, ReceiversQActionRow>();

			foreach (IGrouping<string, ReceiversQActionRow> grouping in destinationsGroupedByLabel)
			{
				if (grouping.Count() == 1)
				{
					destinations.Add(grouping.Key, grouping.Single());
				}
				else
				{
					foreach (ReceiversQActionRow row in grouping)
					{
						destinations.Add($"{grouping.Key} ({row.Receiversid_101})", row);
					}
				}
			}
		}

		private void GetSources()
		{
			IEnumerable<IGrouping<string, SendersQActionRow>> sourcesGroupedByLabel =
				element.GetTable(Parameter.Senders.tablePid)
					.GetRows()
					.Select(row => new SendersQActionRow(row))
					.GroupBy(row => Convert.ToString(row.Sendersdescription_314).Replace(';', ','));

			sources = new Dictionary<string, SendersQActionRow>();

			foreach (IGrouping<string, SendersQActionRow> grouping in sourcesGroupedByLabel)
			{
				if (grouping.Count() == 1)
				{
					sources.Add(grouping.Key, grouping.Single());
				}
				else
				{
					foreach (SendersQActionRow row in grouping)
					{
						sources.Add($"{grouping.Key} ({row.Sendersid_301})", row);
					}
				}
			}
		}
	}
}