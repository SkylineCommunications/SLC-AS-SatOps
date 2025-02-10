namespace Import_Satellite_Data_1
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript.Components;
	using HorizontalAlignment = Skyline.DataMiner.Utils.InteractiveAutomationScript.HorizontalAlignment;

	public class ImportDialog : Dialog
	{
		public ImportDialog(IEngine engine) : base(engine)
		{
			Title = "Satellite Import";
			FileSelector = new FileSelector();
			FileLabel = new Label("Select a file to import:");
			Status = new Label(String.Empty);
			ImportButton = new Button("Import");

			DownloadButton = new DownloadButton("Download Template");
			DownloadButton.RemoteFilePath = "/Documents/Satellite Management/Import/ImportSatelliteTemplate.xlsx";

			CloseButton = new Button("Close");
			FileSelector.AllowedFileNameExtensions = new List<string> { ".xls", ".xlsx" };

			AddWidget(FileLabel, 1, 0);
			AddWidget(FileSelector, 2, 0);
			AddWidget(Status, 4, 0);
			AddWidget(ImportButton, 4, 1, HorizontalAlignment.Right);
			AddWidget(DownloadButton, 5, 0, HorizontalAlignment.Left);
			AddWidget(CloseButton, 5, 1, HorizontalAlignment.Right);

			FileSelector.Width = 400;
			FileLabel.Width = 400;
			Status.Width = 300;

			DownloadButton.Width = 150;
			ImportButton.Width = 100;
			CloseButton.Width = 100;
			FileLabel.Style = TextStyle.Heading;
		}

		public Label FileLabel { get; private set; }

		public Label Status { get; private set; }

		public FileSelector FileSelector { get; }

		public Button ImportButton { get; private set; }

		public DownloadButton DownloadButton { get; private set; }

		public Button CloseButton { get; private set; }
	}
}
