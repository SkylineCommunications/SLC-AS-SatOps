namespace Generate_Flows_1
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class SelectionView : Dialog
	{
		public SelectionView(IEngine engine) : base(engine)
		{
			Title = "Generate Flows";
			var row = 0;
			AddWidget(new Label("Element:"), row, 0);
			AddWidget(Elements, row, 1);

			row++;
			AddWidget(GenerateVSGroup, row, 0);

			row++;
			AddWidget(new Label("Sources:") { Style = TextStyle.Bold }, row, 0);
			AddWidget(new Label("Destinations:") { Style = TextStyle.Bold }, row, 1);

			row++;
			AddWidget(Sources, row, 0);
			AddWidget(Destinations, row, 1);

			row++;
			AddWidget(SelectAllButton, row, 0);
			AddWidget(DeselectAllButton, row, 1);

			row++;
			AddWidget(new Label("Source Group Type:") { Style = TextStyle.Bold }, row, 0);
			AddWidget(new Label("Destination Group Type:") { Style = TextStyle.Bold }, row, 1);

			row++;
			AddWidget(SourceGroupTypes, row, 0);
			AddWidget(DestinationGroupTypes, row, 1);

			row++;
			AddWidget(GenerateButton, row, 0);
		}

		public DropDown Elements { get; } = new DropDown
		{
			IsSorted = true,
			IsDisplayFilterShown = true,
		};

		public CheckBox GenerateVSGroup { get; } = new CheckBox("Create virtual signal group for each flow");

		public CheckBoxList Sources { get; } = new CheckBoxList
		{
			MaxHeight = 350,
			MaxWidth = 200,
		};

		public CheckBoxList Destinations { get; } = new CheckBoxList
		{
			MaxHeight = 350,
			MaxWidth = 200,
		};

		public Button SelectAllButton { get; } = new Button("Select All");

		public Button DeselectAllButton { get; } = new Button("Deselect All");

		public DropDown SourceGroupTypes { get; } = new DropDown { IsEnabled = false };

		public DropDown DestinationGroupTypes { get; } = new DropDown { IsEnabled = false };

		public Button GenerateButton { get; } = new Button("Generate");
	}
}