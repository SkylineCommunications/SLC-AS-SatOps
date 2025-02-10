namespace Transponder_Plan_Manager_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class SlotDefinitionPanel : Section
	{
		public SlotDefinitionPanel()
		{
			AddWidget(SlotName, 0, 0);
			AddWidget(SlotSize, 0, 1);
			AddWidget(RelativeStartFrequency, 0, 2);
			AddWidget(RelativeEndFrequency, 0, 3);
			AddWidget(DeleteButton, 0, 4);
		}

		public Button DeleteButton { get; } = new Button("X") { Width = 60 };

		public TextBox SlotName { get; } = new TextBox { Width = 200, PlaceHolder = "Slot Group Name", };

		public TextBox SlotSize { get; } = new TextBox { Width = 200, PlaceHolder = "Slot Size", };

		public TextBox RelativeStartFrequency { get; } = new TextBox { Width = 200, PlaceHolder = "From", };

		public TextBox RelativeEndFrequency { get; } = new TextBox { Width = 200, PlaceHolder = "To", };
	}
}