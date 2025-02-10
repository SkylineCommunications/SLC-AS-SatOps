namespace TransponderPlanManager
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Transponder_Plan_Manager_1;

	public class PlanDialog : Dialog
	{
		public PlanDialog(IEngine engine, Script.Action action, Guid planInstanceId) : base(engine)
		{
			Action = action;

			// Set title
			Title = Action + " Transponder Plan";
			Width = 1000;

			DetailsPanel = new NamePanel();
			BottomPanel = new FinishPanel();
			AddButton.Pressed += OnAddPressed;

			if (action == Script.Action.Update)
			{
				var domHelper = new DomHelper(engine.SendSLNetMessages, "(slc)satellite_management");
				var instances = domHelper.DomInstances.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcSatellite_Management.Definitions.TransponderPlans.Id));
				DomInstanceToUpdate = instances.Count == 0 ? new DomInstance() : instances.Find(x => x.ID.Id == planInstanceId);
				BottomPanel.CreatePlanButton.Text = "Update Plan";
				foreach (var section in DomInstanceToUpdate.Sections)
				{
					GetTransponderPlanInfo(section);
				}

				if (DomInstanceToUpdate.StatusId.Equals("active"))
				{
					AddButton.IsVisible = false;
					DetailsPanel.PlanNameTextBox.IsEnabled = false;
					BottomPanel.CreatePlanButton.IsEnabled = false;
					foreach (var slotDefintion in SlotDefinitions)
					{
						slotDefintion.IsEnabled = false;
					}
				}
			}

			InitializeUI();
		}

		private void InitializeUI()
		{
			Clear();
			AddSection(DetailsPanel, new SectionLayout(0, 0));
			int position = DetailsPanel.RowCount;
			foreach (var slotDefintion in SlotDefinitions)
			{
				AddSection(slotDefintion, new SectionLayout(position, 0));
				position++;
			}

			AddWidget(AddButton, position++, 0);
			AddWidget(new WhiteSpace(), position++, 0);
			AddSection(BottomPanel, position, 0);
		}

		private void GetTransponderPlanInfo(Skyline.DataMiner.Net.Sections.Section section)
		{
			if (section.SectionDefinitionID.Id == SlcSatellite_Management.Sections.TransponderPlan.Id.Id)
			{
				foreach (var fieldValue in section.FieldValues)
				{
					if (fieldValue.FieldDescriptorID.Id == SlcSatellite_Management.Sections.TransponderPlan.PlanName.Id)
					{
						DetailsPanel.PlanNameTextBox.Text = Convert.ToString(fieldValue.Value.Value);
					}
				}
			}

			if (section.SectionDefinitionID.Id == SlcSatellite_Management.Sections.SlotDefinition.Id.Id)
			{
				var slotDefinitionPanel = new SlotDefinitionPanel();
				foreach (var fieldValue in section.FieldValues)
				{
					if (!SlotPanelMapper.TryGetValue(fieldValue.FieldDescriptorID.Id, out var fieldAssignment))
					{
						continue;
					}

					fieldAssignment.Invoke(slotDefinitionPanel, fieldValue.Value.Value);
				}

				slotDefinitionPanel.DeleteButton.Pressed += OnDeletePressed;
				SlotDefinitions.Add(slotDefinitionPanel);
			}
		}

		private void OnAddPressed(object sender, EventArgs e)
		{
			var newPanel = new SlotDefinitionPanel();
			newPanel.DeleteButton.Pressed += OnDeletePressed;
			SlotDefinitions.Add(newPanel);
			InitializeUI();
		}

		private void OnDeletePressed(object sender, EventArgs e)
		{
			var deletedSection = SlotDefinitions.FirstOrDefault(x => ((SlotDefinitionPanel)x).DeleteButton.Equals(sender));
			SlotDefinitions.Remove(deletedSection);
			InitializeUI();
		}

		public Button AddButton { get; } = new Button("+") { Width = 60 };

		public Button CancelButton { get; } = new Button("Cancel");

		public List<Section> SlotDefinitions { get; } = new List<Section>();

		public NamePanel DetailsPanel { get; set; }

		public FinishPanel BottomPanel { get; set; }

		public Script.Action Action { get; set; }

		public DomInstance DomInstanceToUpdate { get; set; }

		private static readonly Dictionary<Guid, Action<SlotDefinitionPanel, object>> SlotPanelMapper = new Dictionary<Guid, Action<SlotDefinitionPanel, object>>
		{
			[SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotName.Id] = (data, value) => data.SlotName.Text = Convert.ToString(value),
			[SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotSize.Id] = (data, value) => data.SlotSize.Text = Convert.ToString(value),
			[SlcSatellite_Management.Sections.SlotDefinition.RelativeStartFrequency.Id] = (data, value) => data.RelativeStartFrequency.Text = Convert.ToString(value),
			[SlcSatellite_Management.Sections.SlotDefinition.RelativeEndFrequency.Id] = (data, value) => data.RelativeEndFrequency.Text = Convert.ToString(value),
		};
	}

	public class NamePanel : Section
	{
		public NamePanel()
		{
			AddWidget(PlanNameLabel, 0, 0);
			AddWidget(PlanNameTextBox, 0, 1, 1, 4);
			AddWidget(SlotDefinitionsLabel, 1, 0);
			AddWidget(SlotNameLabel, 2, 0);
			AddWidget(SlotSizeLabel, 2, 1);
			AddWidget(StartFrequencyLabel, 2, 2);
			AddWidget(EndFrequencyLabel, 2, 3);
		}

		public Label PlanNameLabel { get; } = new Label("Plan Name") { Style = TextStyle.Bold };

		public TextBox PlanNameTextBox { get; } = new TextBox { Width = 200 };

		public Label SlotDefinitionsLabel { get; private set; } = new Label("Slot Definitions") { Style = TextStyle.Title };

		public Label SlotNameLabel { get; private set; } = new Label("Slot Group Name") { Style = TextStyle.None };

		public Label SlotSizeLabel { get; private set; } = new Label("Slot Size") { Style = TextStyle.None };

		public Label StartFrequencyLabel { get; private set; } = new Label("From*") { Style = TextStyle.None, Tooltip = "* MHz offset from Transponder Start" };

		public Label EndFrequencyLabel { get; private set; } = new Label("To*") { Style = TextStyle.None, Tooltip = "* MHz offset from Transponder Start" };
	}

	public class FinishPanel : Section
	{
		public FinishPanel()
		{
			AddWidget(CancelButton, 0, 0);
			AddWidget(CreatePlanButton, 0, 1);
		}

		public Button CancelButton { get; } = new Button("Cancel") { Width = 100 };

		public Button CreatePlanButton { get; } = new Button("Create Plan") { Width = 100 };
	}
}
