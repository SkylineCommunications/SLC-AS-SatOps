namespace Generate_Flows_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Generate_Flows_1.AMWA_NMOS_IS_05;
	using Generate_Flows_1.NTP_RCP;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.MediaOps.Common.DOM.Applications.DomIds;

	public class FlowGenerator
	{
		private readonly IDms dms;
		private readonly IEngine engine;
		private readonly SelectionView view;
		private readonly ICrudHelperComponent<DomInstance> virtualSignalGroupHelper;
		private Dictionary<string, SupportedElement> elementsByName;
		private Dictionary<string, DomInstance> groupTypeOptions;

		public FlowGenerator(IEngine engine, SelectionView view)
		{
			this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
			dms = engine.GetDms();

			virtualSignalGroupHelper = new DomHelper(engine.SendSLNetMessages, SlcVirtualsignalgroup.ModuleId).DomInstances;

			this.view = view ?? throw new ArgumentNullException(nameof(view));

			view.Elements.Changed += ElementsOnChanged;
			view.GenerateButton.Pressed += GenerateButtonOnPressed;
			view.GenerateVSGroup.Changed += GenerateVSGroupOnChanged;
			view.SelectAllButton.Pressed += (sender, args) => SelectAll();
			view.DeselectAllButton.Pressed += (sender, args) => DeselectAll();
		}

		public void Load()
		{
			elementsByName = GetElements();
			view.Elements.Options = elementsByName.Keys;

			var sourceOptions = GetSourceOptions().ToList();
			view.Sources.SetOptions(sourceOptions);

			var destinationOptions = GetDestinationOptions().ToList();
			view.Destinations.SetOptions(destinationOptions);

			SelectAll();

			groupTypeOptions = GetGroupTypeOptions().ToDictionary(x => x.Key, x => x.Value);
			view.SourceGroupTypes.Options = groupTypeOptions.Keys;
			view.DestinationGroupTypes.Options = groupTypeOptions.Keys;

			view.GenerateButton.IsEnabled = view.Elements.Selected != null;

			UpdateVsGroupGeneration();
		}

		private void DeselectAll()
		{
			view.Sources.UncheckAll();
			view.Destinations.UncheckAll();
		}

		private void ElementsOnChanged(object sender, DropDown.DropDownChangedEventArgs e)
		{
			view.Sources.UncheckAll();
			var sourceOptions = GetSourceOptions().ToList();
			view.Sources.SetOptions(sourceOptions);

			view.Destinations.UncheckAll();
			var destinationOptions = GetDestinationOptions().ToList();
			view.Destinations.SetOptions(destinationOptions);

			UpdateVsGroupGeneration();
		}

		private void GenerateButtonOnPressed(object sender, EventArgs e)
		{
			var element = elementsByName[view.Elements.Selected];

			engine.ShowProgress("Generating sources...");

			DomInstance sourceGroupType = null;
			if (view.SourceGroupTypes.Selected != null)
			{
				groupTypeOptions.TryGetValue(view.SourceGroupTypes.Selected, out sourceGroupType);
			}

			element.GenerateSources(view.Sources.Checked, view.GenerateVSGroup.IsChecked, sourceGroupType);

			engine.ShowProgress("Generating sources...\r\nGenerating destinations...");

			DomInstance destinationGroupType = null;
			if (view.DestinationGroupTypes.Selected != null)
			{
				groupTypeOptions.TryGetValue(view.DestinationGroupTypes.Selected, out destinationGroupType);
			}

			element.GenerateDestinations(view.Destinations.Checked, view.GenerateVSGroup.IsChecked, destinationGroupType);

			engine.ExitSuccess("Finished");
		}

		private void GenerateVSGroupOnChanged(object sender, CheckBox.CheckBoxChangedEventArgs e)
		{
			view.SourceGroupTypes.IsEnabled = e.IsChecked;
			view.DestinationGroupTypes.IsEnabled = e.IsChecked;
		}

		private IEnumerable<string> GetDestinationOptions()
		{
			if (view.Elements.Selected == null)
			{
				return new string[0];
			}

			var element = elementsByName[view.Elements.Selected];
			return element.Destinations;
		}

		private Dictionary<string, SupportedElement> GetElements()
		{
			var elements = new Dictionary<string, SupportedElement>();

			foreach (var element in dms.GetElements())
			{
				switch (element.Protocol.Name)
				{
					case "NTP RCP":
						elements.Add(element.Name, new NtpRcpElement(element, virtualSignalGroupHelper));
						break;

					case "AMWA NMOS IS-05":
						elements.Add(element.Name, new AmwaNmosIs05Element(element, virtualSignalGroupHelper));
						break;

					default: continue;
				}
			}

			return elements;
		}

		private IEnumerable<KeyValuePair<string, DomInstance>> GetGroupTypeOptions()
		{
			return virtualSignalGroupHelper
				.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcVirtualsignalgroup.Definitions.VirtualSignalGroupTypes.Id))
				.Select(instance => new KeyValuePair<string, DomInstance>(instance.Name, instance));
		}

		private IEnumerable<string> GetSourceOptions()
		{
			if (view.Elements.Selected == null)
			{
				return new string[0];
			}

			var element = elementsByName[view.Elements.Selected];
			return element.Sources;
		}

		private void SelectAll()
		{
			view.Sources.CheckAll();
			view.Destinations.CheckAll();
		}

		private void UpdateVsGroupGeneration()
		{
			bool enableSelection = true;
			if (view.Elements.Selected == null)
			{
				enableSelection = false;
			}
			else
			{
				var element = elementsByName[view.Elements.Selected];
				enableSelection = element.SupportsVirtualSignalGroupGeneration;
			}

			if (enableSelection)
			{
				view.GenerateVSGroup.IsVisible = true;
			}
			else
			{
				view.GenerateVSGroup.IsChecked = false;
				view.GenerateVSGroup.IsVisible = false;
			}
		}
	}
}