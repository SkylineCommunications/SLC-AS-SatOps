namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.DismissibleConfirmDialog
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	internal class DismissibleConfirmDialogView : ScriptDialog
	{
		public DismissibleConfirmDialogView(IEngine engine)
			: base(engine)
		{
			InitWidgets();
		}

		#region Properties
		public Label Message { get; set; }

		public Label DontShowLabel { get; set; }

		public CheckBox DontShowCheckBox { get; set; }

		public Button ActionProceedButton { get; set; }

		public Button ActionCancelButton { get; set; }

		#endregion

		#region Methods
		public override void Build()
		{
			Clear();

			Width = 530;

			AddWidget(Message, Layout.RowPosition, 0, 1, 3);

			AddWidget(DontShowLabel, ++Layout.RowPosition, 0);
			AddWidget(DontShowCheckBox, Layout.RowPosition, 1, HorizontalAlignment.Center);

			AddWidget(ActionCancelButton, ++Layout.RowPosition, 1, HorizontalAlignment.Right);
			AddWidget(ActionProceedButton, Layout.RowPosition, 2);

			SetColumnWidth(0, 140);
		}

		private void InitWidgets()
		{
			Message = new Label();

			DontShowLabel = new Label("Don't show this message again?");
			DontShowCheckBox = new CheckBox();

			ActionCancelButton = new Button() { Width = 130, Height = 25};
			ActionProceedButton = new Button() { Width = 130, Height = 25};
		}
		#endregion
	}
}
