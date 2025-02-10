﻿namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.YesNoCancelDialog
{
	internal class YesNoCancelDialogModel
	{
		private readonly string message;

		public YesNoCancelDialogModel(string message)
		{
			this.message =	message;
		}

		public string Message => message;
	}
}
