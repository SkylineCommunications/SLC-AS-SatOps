namespace Generate_Flows_1.NTP_RCP
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

	public class NtpRcpElement : SupportedElement
	{
		private Dictionary<string, DestinationsQActionRow> destinations;
		private Dictionary<string, SourcesQActionRow> sources;

		public NtpRcpElement(
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

		public override bool SupportsVirtualSignalGroupGeneration => true;

		public override void GenerateDestinations(IEnumerable<string> selectedDestinationKeys, bool generateVsGroups, DomInstance groupType)
		{
			ILookup<int, Interface> interfacesByTable = Interfaces
				.ToLookup(@interface => @interface.TableLink);
			Dictionary<string, Interface> destinationInterfacesByKey = interfacesByTable[5100]
				.ToDictionary(@interface => @interface.RowLink);

			foreach (var row in selectedDestinationKeys)
			{
				var rowValue = destinations[row];

				var elementFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Element)
						.Equal(element.DmsElementId.Value);
				var interfaceFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Interface)
						.Equal(Convert.ToString(destinationInterfacesByKey[rowValue.Key].Id));

				if (virtualSignalGroupHelper.Read(elementFilter.AND(interfaceFilter)).Any())
				{
					// already exists, skipping
					continue;
				}

				var flow = new DomInstance
				{
					DomDefinitionId = SlcVirtualsignalgroup.Definitions.Flow,
				};

				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.Name,
					(string)rowValue.Destinationslabel_5102);
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
					(int)SlcVirtualsignalgroup.Enums.TransportType.SDI);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Element,
					element.DmsElementId.Value);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Interface,
					Convert.ToString(destinationInterfacesByKey[rowValue.Key].Id));
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.FlowDirection,
					(int)SlcVirtualsignalgroup.Enums.FlowDirection.Rx);

				virtualSignalGroupHelper.Create(flow);

				if (generateVsGroups)
				{
					var vsgroup = new DomInstance
					{
						DomDefinitionId = SlcVirtualsignalgroup.Definitions.Virtualsignalgroup,
					};

					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Name,
						(string)rowValue.Destinationslabel_5102);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.OperationalState,
						(int)SlcVirtualsignalgroup.Enums.OperationalState.Up);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Role,
						(int)SlcVirtualsignalgroup.Enums.Role.Destination);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.AdministrativeState,
						(int)SlcVirtualsignalgroup.Enums.AdministrativeState.Up);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Type,
						groupType?.ID?.Id);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupSystemlabels.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupSystemlabels.ButtonLabel,
						(string)rowValue.Destinationslabel_5102);

					FilterElement<DomInstance> domFilter = DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.LevelInfo.Name).Equal("SDI");
					var levelInstance = virtualSignalGroupHelper.Read(domFilter).FirstOrDefault();

					VsGroupHelper.AssignFlowToVirtualSignalGroup(vsgroup, flow, levelInstance, VsGroupHelper.FlowColor.Blue);

					virtualSignalGroupHelper.Create(vsgroup);
				}
			}
		}

		public override void GenerateSources(IEnumerable<string> selectedSourceKeys, bool generateVsGroups, DomInstance groupType)
		{
			ILookup<int, Interface> interfacesByTable = Interfaces
				.ToLookup(@interface => @interface.TableLink);
			Dictionary<string, Interface> sourceInterfacesByKey = interfacesByTable[5000]
				.ToDictionary(@interface => @interface.RowLink);

			foreach (var row in selectedSourceKeys)
			{
				var rowValue = sources[row];

				var elementFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Element)
						.Equal(element.DmsElementId.Value);
				var interfaceFilter =
					DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.FlowPath.Interface)
						.Equal(Convert.ToString(sourceInterfacesByKey[rowValue.Key].Id));

				if (virtualSignalGroupHelper.Read(elementFilter.AND(interfaceFilter)).Any())
				{
					// already exists, skipping
					continue;
				}

				var flow = new DomInstance
				{
					DomDefinitionId = SlcVirtualsignalgroup.Definitions.Flow,
				};

				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowInfo.Id,
					SlcVirtualsignalgroup.Sections.FlowInfo.Name,
					(string)rowValue.Sourceslabel_5002);
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
					(int)SlcVirtualsignalgroup.Enums.TransportType.SDI);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Element,
					element.DmsElementId.Value);
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.Interface,
					Convert.ToString(sourceInterfacesByKey[rowValue.Key].Id));
				flow.AddOrUpdateFieldValue(
					SlcVirtualsignalgroup.Sections.FlowPath.Id,
					SlcVirtualsignalgroup.Sections.FlowPath.FlowDirection,
					(int)SlcVirtualsignalgroup.Enums.FlowDirection.Tx);

				virtualSignalGroupHelper.Create(flow);

				if (generateVsGroups)
				{
					var vsgroup = new DomInstance
					{
						DomDefinitionId = SlcVirtualsignalgroup.Definitions.Virtualsignalgroup,
					};

					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Name,
						(string)rowValue.Sourceslabel_5002);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Role,
						(int)SlcVirtualsignalgroup.Enums.Role.Source);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.OperationalState,
						(int)SlcVirtualsignalgroup.Enums.OperationalState.Up);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.AdministrativeState,
						(int)SlcVirtualsignalgroup.Enums.AdministrativeState.Up);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupInfo.Type,
						groupType?.ID?.Id);
					vsgroup.AddOrUpdateFieldValue(
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupSystemlabels.Id,
						SlcVirtualsignalgroup.Sections.VirtualsignalgroupSystemlabels.ButtonLabel,
						(string)rowValue.Sourceslabel_5002);

					FilterElement<DomInstance> domFilter = DomInstanceExposers.FieldValues.DomInstanceField(SlcVirtualsignalgroup.Sections.LevelInfo.Name).Equal("SDI");
					var levelInstance = virtualSignalGroupHelper.Read(domFilter).FirstOrDefault();

					VsGroupHelper.AssignFlowToVirtualSignalGroup(vsgroup, flow, levelInstance, VsGroupHelper.FlowColor.Blue);

					virtualSignalGroupHelper.Create(vsgroup);
				}
			}
		}

		private void GetDestinations()
		{
			IEnumerable<IGrouping<string, DestinationsQActionRow>> sourcesGroupedByLabel =
				element.GetTable(Parameter.Destinations.tablePid)
					.GetRows()
					.Select(row => new DestinationsQActionRow(row))
					.GroupBy(row => (string)row.Destinationslabel_5102);

			destinations = new Dictionary<string, DestinationsQActionRow>();

			foreach (IGrouping<string, DestinationsQActionRow> grouping in sourcesGroupedByLabel)
			{
				if (grouping.Count() == 1)
				{
					destinations.Add(grouping.Key, grouping.Single());
				}
				else
				{
					foreach (DestinationsQActionRow row in grouping)
					{
						destinations.Add($"{grouping.Key} ({Convert.ToInt32(row.Destinationsnumber_5101)})", row);
					}
				}
			}
		}

		private void GetSources()
		{
			IEnumerable<IGrouping<string, SourcesQActionRow>> sourcesGroupedByLabel =
				element.GetTable(Parameter.Sources.tablePid)
					.GetRows()
					.Select(row => new SourcesQActionRow(row))
					.GroupBy(row => (string)row.Sourceslabel_5002);

			sources = new Dictionary<string, SourcesQActionRow>();

			foreach (IGrouping<string, SourcesQActionRow> grouping in sourcesGroupedByLabel)
			{
				if (grouping.Count() == 1)
				{
					sources.Add(grouping.Key, grouping.Single());
				}
				else
				{
					foreach (SourcesQActionRow row in grouping)
					{
						sources.Add($"{grouping.Key} ({Convert.ToInt32(row.Sourcesnumber_5001)})", row);
					}
				}
			}
		}
	}
}