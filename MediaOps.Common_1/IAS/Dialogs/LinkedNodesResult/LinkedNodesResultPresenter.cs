namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.LinkedNodesResult
{
	using System;

	internal class LinkedNodesResultPresenter
	{
		#region Fields
		private readonly LinkedNodesResultView view;

		private readonly LinkedNodesResultModel model;
		#endregion

		public LinkedNodesResultPresenter(LinkedNodesResultView view,  LinkedNodesResultModel model)
		{
			this.view = view ?? throw new ArgumentNullException(nameof(view));
			this.model = model ?? throw new ArgumentNullException(nameof(model));

			Init();
		}

		#region Events
		public event EventHandler<EventArgs> Close;
		#endregion

		#region Methods
		public void LoadFromModel()
		{
			if (model.Result.ManualAdded.Count > 0)
			{
				view.ManualAddedChanges.AddRange(model.Result.ManualAdded);
			}

			if (model.Result.AutomaticAddedSucceeded.Count > 0)
			{
				view.AutomaticAddedSucceededChanges.AddRange(model.Result.AutomaticAddedSucceeded);
			}

			if (model.Result.AutomaticAddedFailed.Count > 0)
			{
				view.AutomaticAddedFailedChanges.AddRange(model.Result.AutomaticAddedFailed);
			}
		}

		public void BuildView()
		{
			view.Build();
		}

		private void Init()
		{
			view.ButtonClose.Pressed += OnCloseButtonPressed;
		}

		private void OnCloseButtonPressed(object sender, EventArgs e)
		{
			Close?.Invoke(this, EventArgs.Empty);
		}
		#endregion
	}
}
