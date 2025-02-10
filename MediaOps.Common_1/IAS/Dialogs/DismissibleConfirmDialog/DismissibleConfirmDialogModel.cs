namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.DismissibleConfirmDialog
{
	using System;
	using System.Collections.Concurrent;

	internal class DismissibleConfirmDialogModel
	{
		private static readonly ConcurrentDictionary<string, bool> ShowDialogForUser = new ConcurrentDictionary<string, bool>();

		public DismissibleConfirmDialogModel(string title, string message) : this(title, message, "Proceed", "Cancel")
		{
		}

		public DismissibleConfirmDialogModel(string title, string message, string actionProceedMessage, string actionCancelMessage)
		{
			Title = title;
			Message = message;
			ActionProceedMessage = actionProceedMessage;
			ActionCancelMessage = actionCancelMessage;
		}

		public string Title { get; }

		public string Message { get; }

		public string ActionProceedMessage { get; }

		public string ActionCancelMessage { get; }

		public void SetIsShown(string userName, bool isShown)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}

			ShowDialogForUser[userName] = isShown;
		}

		public bool IsShown(string userName)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}

			if (ShowDialogForUser.TryGetValue(userName, out bool showDialogs))
			{
				return showDialogs;
			}
			else
			{
				return true;
			}
		}
	}
}
