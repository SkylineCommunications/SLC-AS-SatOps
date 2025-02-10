namespace Manual_Slot_Creation_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	public class ManualSlotDialog : Dialog
	{
		public ManualSlotDialog(IEngine engine, Transponder transponder) : base(engine)
		{
			// Set title
			Title = "Create Manual Slots";
			Width = 1000;

			DetailsPanel = new NamePanel();
			BottomPanel = new FinishPanel();
			DetailsPanel.TransponderNameLabel.Text = transponder.DomTransponder.TransponderSection.TransponderName;
			AddButton.Pressed += OnAddPressed;

			InitializeUI();
		}

		public Button AddButton { get; } = new Button("+") { Width = 60 };

		public Button CancelButton { get; } = new Button("Cancel");

		public List<Section> SlotDefinitions { get; } = new List<Section>();

		public NamePanel DetailsPanel { get; set; }

		public FinishPanel BottomPanel { get; set; }

		private void OnAddPressed(object sender, EventArgs e)
		{
			var newPanel = new SlotPanel();
			newPanel.DeleteButton.Pressed += OnDeletePressed;
			SlotDefinitions.Add(newPanel);
			InitializeUI();
		}

		private void OnDeletePressed(object sender, EventArgs e)
		{
			var deletedSection = SlotDefinitions.FirstOrDefault(x => ((SlotPanel)x).DeleteButton.Equals(sender));
			SlotDefinitions.Remove(deletedSection);
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
	}

	public class NamePanel : Section
	{
		public NamePanel()
		{
			AddWidget(TransponderLabel, 0, 0);
			AddWidget(TransponderNameLabel, 1, 0);
			AddWidget(SlotDefinitionsLabel, 2, 0);
			AddWidget(SlotNameLabel, 3, 0);
			AddWidget(SlotSizeLabel, 3, 1);
			AddWidget(CenterFrequencyLabel, 3, 2);
		}

		public Label TransponderLabel { get; private set; } = new Label("Transponder") { Style = TextStyle.Title };

		public Label TransponderNameLabel { get; } = new Label { Style = TextStyle.Bold };

		public Label SlotDefinitionsLabel { get; private set; } = new Label("Slots") { Style = TextStyle.Title };

		public Label SlotNameLabel { get; private set; } = new Label("Slot Name") { Style = TextStyle.None };

		public Label SlotSizeLabel { get; private set; } = new Label("Slot Size") { Style = TextStyle.None };

		public Label CenterFrequencyLabel { get; private set; } = new Label("Center Frequency") { Style = TextStyle.None };
	}

	public class FinishPanel : Section
	{
		public FinishPanel()
		{
			AddWidget(CancelButton, 0, 0);
			AddWidget(CreatePlanButton, 0, 1);
		}

		public Button CancelButton { get; } = new Button("Cancel") { Width = 100 };

		public Button CreatePlanButton { get; } = new Button("Create Slots") { Width = 100 };
	}
}
