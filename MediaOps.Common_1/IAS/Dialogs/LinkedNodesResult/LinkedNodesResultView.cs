namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.LinkedNodesResult
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;

	internal class LinkedNodesResultView : ScriptDialog
	{
		public LinkedNodesResultView(IEngine engine) : base(engine)
		{
			InitWidgets();
		}

		#region Properties
		public List<string> ManualAddedChanges { get; private set; } = new List<string>();

		public List<string> AutomaticAddedSucceededChanges { get; private set; } = new List<string>();

		public List<string> AutomaticAddedFailedChanges { get; private set; } = new List<string>();

		public Button ButtonClose { get; private set; }
		#endregion

		#region Methods
		public override void Build()
		{
			Clear();

			Width = 600;

			Title = "Add Linked Resource Pools";

			AddWidget(new Label("Changes Overview") { Style = TextStyle.Title }, Layout.RowPosition, 0);

			if (ManualAddedChanges.Count > 0)
			{
				AddWidget(new Label("Manual Added") { Style = TextStyle.Bold }, ++Layout.RowPosition, 0);

				ManualAddedChanges.ForEach(x =>
				{
					AddWidget(new Label(StringExtensions.Wrap(x, 100)), ++Layout.RowPosition, 0);
				});

				AddWidget(new WhiteSpace { Height = 10 }, ++Layout.RowPosition, 0);
			}

			if (AutomaticAddedSucceededChanges.Count > 0)
			{
				AddWidget(new Label("Automatic Added") { Style = TextStyle.Bold }, ++Layout.RowPosition, 0);

				AutomaticAddedSucceededChanges.ForEach(x =>
				{
					AddWidget(new Label(StringExtensions.Wrap(x, 100)), ++Layout.RowPosition, 0);
				});

				AddWidget(new WhiteSpace { Height = 10 }, ++Layout.RowPosition, 0);
			}

			if (AutomaticAddedFailedChanges.Count > 0)
			{
				AddWidget(new Label("Failed") { Style = TextStyle.Bold }, ++Layout.RowPosition, 0);

				AutomaticAddedFailedChanges.ForEach(x =>
				{
					AddWidget(new Label(StringExtensions.Wrap(x, 100)), ++Layout.RowPosition, 0);
				});

				AddWidget(new WhiteSpace { Height = 10 }, ++Layout.RowPosition, 0);
			}

			AddWidget(new WhiteSpace { Height = 15 }, ++Layout.RowPosition, 0);

			AddWidget(ButtonClose, ++Layout.RowPosition, 0);
		}

		private void InitWidgets()
		{
			ButtonClose = new Button("Close") { Width = 150, Height = 25 };
		}
		#endregion
	}
}
