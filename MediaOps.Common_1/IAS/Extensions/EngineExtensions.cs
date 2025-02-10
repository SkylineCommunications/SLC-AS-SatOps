namespace Skyline.DataMiner.Utils.SatOps.Common.IAS
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.DismissibleConfirmDialog;

	public static class EngineExtensions
	{
		public static void ShowErrorDialog(this IEngine engine, string message, string title = "Script execution failed")
		{
			var model = new Dialogs.ErrorDialog.ErrorDialogModel(message);
			var view = new Dialogs.ErrorDialog.ErrorDialogView(engine)
			{
				Title = title,
			};
			var presenter = new Dialogs.ErrorDialog.ErrorDialogPresenter(view, model);

			presenter.Close += (sender, arg) =>
			{
				engine.ExitSuccess(string.Empty);
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();
		}

		public static void ShowErrorDialogWithReturn(this IEngine engine, string message, string title = "Script execution failed")
		{
			var model = new Dialogs.ErrorDialog.ErrorDialogModel(message);
			var view = new Dialogs.ErrorDialog.ErrorDialogView(engine)
			{
				Title = title,
			};
			var presenter = new Dialogs.ErrorDialog.ErrorDialogPresenter(view, model);

			presenter.Close += (sender, arg) =>
			{
				// Do nothing
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();
		}

		public static bool ShowConfirmDialog(this IEngine engine, string message, string confirmLabel = "Confirm", string cancelLabel = "Cancel")
		{
			var model = new Dialogs.ConfirmDialog.ConfirmDialogModel(message);
			var view = new Dialogs.ConfirmDialog.ConfirmDialogView(engine);
			var presenter = new Dialogs.ConfirmDialog.ConfirmDialogPresenter(view, model);

			view.ConfirmButton.Text = confirmLabel;
			view.CancelButton.Text = cancelLabel;

			var confirmed = false;
			presenter.Cancel += (sender, arg) =>
			{
				confirmed = false;
			};
			presenter.Confirm += (sender, arg) =>
			{
				confirmed = true;
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();

			return confirmed;
		}

		public static bool ShowDismissibleConfirmDialog(this IEngine engine, string title, string message, string proceedLabel, string cancelLabel, DialogType dialogType)
		{
			var model = new DismissibleConfirmDialogModel(title, message, proceedLabel, cancelLabel);
			var view = new DismissibleConfirmDialogView(engine);
			var presenter = new DismissibleConfirmDialogPresenter(view, model, engine.UserLoginName, dialogType);

			if (!presenter.IsShown())
			{
				return true;
			}

			bool confirmed = false;
			presenter.Cancel += (sender, arg) =>
			{
				confirmed = false;
			};
			presenter.Proceed += (sender, arg) =>
			{
				confirmed = true;
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();

			return confirmed;
		}

		public static void ShowLinkedNodesResultDialog(this IEngine engine, LinkedNodesResult result)
		{
			var model = new Dialogs.LinkedNodesResult.LinkedNodesResultModel(result);
			var view = new Dialogs.LinkedNodesResult.LinkedNodesResultView(engine);
			var presenter = new Dialogs.LinkedNodesResult.LinkedNodesResultPresenter(view, model);

			presenter.Close += (sender, arg) =>
			{
				engine.ExitSuccess(string.Empty);
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();
		}

		public static YesNoCancelOption ShowYesNoCancelDialog(this IEngine engine, string message)
		{
			var model = new Dialogs.YesNoCancelDialog.YesNoCancelDialogModel(message);
			var view = new Dialogs.YesNoCancelDialog.YesNoCancelDialogView(engine);
			var presenter = new Dialogs.YesNoCancelDialog.YesNoCancelDialogPresenter(view, model);

			YesNoCancelOption result = YesNoCancelOption.Cancel;
			presenter.Cancel += (sender, arg) =>
			{
				result = YesNoCancelOption.Cancel;
			};
			presenter.Yes += (sender, arg) =>
			{
				result = YesNoCancelOption.Yes;
			};
			presenter.No += (sender, arg) =>
			{
				result = YesNoCancelOption.No;
			};

			presenter.LoadFromModel();
			presenter.BuildView();

			view.Show();

			return result;
		}
	}
}
