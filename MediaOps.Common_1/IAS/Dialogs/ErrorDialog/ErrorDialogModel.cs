﻿namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.ErrorDialog
{
	internal class ErrorDialogModel
	{
		private readonly string errorMessage;

		public ErrorDialogModel(string errorMessage)
		{
			this.errorMessage = errorMessage;
		}

		public string ErrorMessage
		{
			get { return errorMessage; }
		}
	}
}
