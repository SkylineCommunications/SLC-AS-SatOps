namespace Manual_Slot_Creation_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class SlotPanel : Section
	{
		internal const string Static = "Static";
		internal const string FieldValue = "Field Value";

		public SlotPanel()
		{
			AddWidget(SlotName, 0, 0);
			AddWidget(SlotSize, 0, 1);
			AddWidget(CenterFrequency, 0, 2);
			AddWidget(DeleteButton, 0, 3);
		}

		public Button DeleteButton { get; } = new Button("X") { Width = 60 };

		public TextBox SlotName { get; } = new TextBox { Width = 200, PlaceHolder = "Slot Name", };

		public TextBox SlotSize { get; } = new TextBox { Width = 200, PlaceHolder = "Slot Size", };

		public TextBox CenterFrequency { get; } = new TextBox { Width = 200, PlaceHolder = "Center Frequency", };
	}
}