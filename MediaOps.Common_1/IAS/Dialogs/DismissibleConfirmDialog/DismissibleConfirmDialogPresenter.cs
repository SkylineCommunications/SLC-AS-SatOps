namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.DismissibleConfirmDialog
{
	using System;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;

	internal class DismissibleConfirmDialogPresenter
	{
		#region Fields
		private readonly DismissibleConfirmDialogView view;

		private readonly DismissibleConfirmDialogModel model;

		private readonly string userName;

		private readonly DialogType dialogType;
		#endregion

		public DismissibleConfirmDialogPresenter(DismissibleConfirmDialogView view, DismissibleConfirmDialogModel model, string userName, DialogType dialogType)
		{
			this.view = view ?? throw new ArgumentNullException(nameof(view));
			this.model = model ?? throw new ArgumentNullException(nameof(model));
			this.userName = userName ?? throw new ArgumentNullException(nameof(userName));
			this.dialogType = dialogType;

			Init();
		}

		#region Events
		public event EventHandler<EventArgs> Proceed;

		public event EventHandler<EventArgs> Cancel;
		#endregion

		#region Methods
		public void LoadFromModel()
		{
			view.Title = model.Title.TruncateWithEllipsis(50);

			view.Message.Text = model.Message.Wrap(90);

			view.ActionProceedButton.Text = model.ActionProceedMessage;
			view.ActionProceedButton.Tooltip = model.ActionProceedMessage;

			view.ActionCancelButton.Text = model.ActionCancelMessage;
			view.ActionCancelButton.Tooltip = model.ActionCancelMessage;
		}

		public void BuildView()
		{
			view.Build();
		}

		public bool IsShown()
		{
			return model.IsShown(GetDialogKeyForUser());
		}

		private void Init()
		{
			view.ActionProceedButton.Pressed += OnProceedButtonPressed;
			view.ActionCancelButton.Pressed += OnCancelButtonPressed;
		}

		private void OnCancelButtonPressed(object sender, EventArgs e)
		{
			Cancel?.Invoke(this, EventArgs.Empty);
		}

		private void OnProceedButtonPressed(object sender, EventArgs e)
		{
			model.SetIsShown(GetDialogKeyForUser(), !view.DontShowCheckBox.IsChecked);

			Proceed?.Invoke(this, EventArgs.Empty);
		}

		private string GetDialogKeyForUser()
		{
			return userName + '-' + dialogType;
		}
		#endregion
	}
}
