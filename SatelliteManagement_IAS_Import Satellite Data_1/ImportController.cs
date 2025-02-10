namespace Import_Satellite_Data_1
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using NPOI.SS.UserModel;
	using NPOI.XSSF.UserModel;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Utils.DOM.Builders;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;

	public class ImportController
	{
		private readonly IEngine engine;

		public ImportController(IEngine engine)
		{
			this.engine = engine;
		}

		public enum SpreadsheetColumns
		{
			SatelliteName,
			SatelliteAbbreviation,
			Orbit,
			Hemisphere,
			Azimuth,
			Operator,
			SatelliteCoverage,
			SatelliteApplications,
			SatelliteInfo,
			Manufacturer,
			Country,
			LaunchInfo,
			LaunchServiceDate,
		}

		public void ProcessImportFile(ImportDialog import)
		{
			var directoryPath = "C:\\Skyline DataMiner\\Documents\\Satellite Management\\Import";
			var uploadedFile = import.FileSelector.UploadedFilePaths[0];
			var fileName = Path.GetFileName(uploadedFile);
			var destinationFilePath = Path.Combine(directoryPath, fileName);

			if (!File.Exists(destinationFilePath))
			{
				import.FileSelector.CopyUploadedFiles(directoryPath);
			}
			else
			{
				File.Copy(uploadedFile, destinationFilePath, true);
			}

			var folder = Directory.GetFiles(directoryPath).Select(file => fileName).First();

			if (folder.Contains(Path.GetFileName(fileName)))
			{
				import.Status.Text = "File successfully imported.";
				ProcessFileData(destinationFilePath, import);
			}
			else
			{
				import.Status.Text = $"An error occurred while importing the following file: {fileName}";
			}
		}

		private void ProcessFileData(string destinationFilePath, ImportDialog import)
		{
			int startColumnIndex = 0;
			int endColumnIndex = 12;

			var numRows = 0;
			var numIncorrectRows = 0;
			var emptyRows = 0;

			var errorRowPosition = new List<int>();
			var spreadsheetRows = new List<SpreadsheetData>();

			if (!import.FileSelector.AllowedFileNameExtensions.Contains(Path.GetExtension(destinationFilePath)))
			{
				import.Status.Text = $"File extension: '{Path.GetExtension(destinationFilePath)}' not supported.";
				return;
			}

			using (FileStream fileStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook workbook = new XSSFWorkbook(fileStream);
				ISheet sheet = workbook.GetSheetAt(0);
				numRows = sheet.LastRowNum;

				for (int row = 1; row <= numRows; row++)
				{
					IRow currentRow = sheet.GetRow(row);
					if (currentRow == null)
					{
						numIncorrectRows++;
						errorRowPosition.Add(row);
						continue;
					}

					var selectedCells = currentRow.Cells.Where(c => c.ColumnIndex >= startColumnIndex && c.ColumnIndex <= endColumnIndex).ToList();
					var tableRow = GetRowData(selectedCells);
					if (tableRow == null)
					{
						emptyRows++;
						continue;
					}

					spreadsheetRows.Add(tableRow);
				}
			}

			CheckSatelliteInstances(spreadsheetRows, import);
		}

		private void CheckSatelliteInstances(List<SpreadsheetData> spreadsheetRows, ImportDialog importDialog)
		{
			var domHelper = new DomHelper(engine.SendSLNetMessages, SlcSatellite_Management.ModuleId);
			var domCache = new DomCache(domHelper);
			var satelliteDomDict = CreateSatelliteInstanceDictionary(domCache);

			var totalRows = spreadsheetRows.Count;
			var currentCount = 0;
			foreach (var row in spreadsheetRows)
			{
				if (satelliteDomDict.TryGetValue(row.SatelliteName, out DomInstance satelliteInstance))
				{
					UpdateOrCreateInstance(domHelper, satelliteInstance, row);
				}
				else
				{
					UpdateOrCreateInstance(domHelper, null, row);
				}

				currentCount++;
				var progress = ((double)currentCount / totalRows) * 100;
				importDialog.Status.Text = $"Importing Satellites {Convert.ToInt32(progress)}%";
				importDialog.Show(false);
			}

			importDialog.Status.Text = "Satellites imported.";
		}

		private Dictionary<string, DomInstance> CreateSatelliteInstanceDictionary(DomCache domCache)
		{
			var satelliteDomInstances = domCache.GetInstancesByDefinition(SlcSatellite_Management.Definitions.Satellites);
			var satelliteInstanceDict = new Dictionary<string, DomInstance>();

			foreach (var satellite in satelliteDomInstances)
			{
				var satelliteName = satellite.GetFieldValue<string>("General", "Satellite name", domCache);
				satelliteInstanceDict[satelliteName] = satellite;
			}

			return satelliteInstanceDict;
		}

		private void UpdateOrCreateInstance(DomHelper domHelper, DomInstance satelliteInstance, SpreadsheetData row)
		{
			Guid instanceGuid;
			string statusId;
			bool isCreate = false;
			if (satelliteInstance == null)
			{
				instanceGuid = Guid.NewGuid();
				statusId = "draft";
				isCreate = true;
			}
			else
			{
				instanceGuid = satelliteInstance.ID.Id;
				statusId = satelliteInstance.StatusId;
			}

			var instanceBuilder = new DomInstanceBuilder(SlcSatellite_Management.Definitions.Satellites)
					.WithID(instanceGuid)
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.General.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.General.SatelliteName, row.SatelliteName)
						.WithFieldValue(SlcSatellite_Management.Sections.General.SatelliteAbbreviation, row.SatelliteAbbreviation)
						.WithFieldValue(SlcSatellite_Management.Sections.General.Orbit, row.Orbit)
						.WithFieldValue(SlcSatellite_Management.Sections.General.Hemisphere, row.Hemisphere))
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.Satellite.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.Satellite.Operator, row.Operator)
						.WithFieldValue(SlcSatellite_Management.Sections.Satellite.Coverage, row.Coverage)
						.WithFieldValue(SlcSatellite_Management.Sections.Satellite.Applications, row.SatelliteApplications)
						.WithFieldValue(SlcSatellite_Management.Sections.Satellite.Info, row.SatelliteInfo))
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.Origin.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.Origin.Manufacturer, row.Manufacturer)
						.WithFieldValue(SlcSatellite_Management.Sections.Origin.Country, row.Country))
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.LaunchInformation.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.LaunchInformation.LaunchInfo, row.LaunchInfo)
						.WithFieldValue(SlcSatellite_Management.Sections.LaunchInformation.LaunchInServiceDate, row.LaunchInServiceDate)).Build();

			instanceBuilder.StatusId = statusId;

			if (isCreate)
			{
				domHelper.DomInstances.Create(instanceBuilder);
			}
			else
			{
				domHelper.DomInstances.Update(instanceBuilder);
			}
		}

		private SpreadsheetData GetRowData(List<ICell> cellsWithData)
		{
			var rowData = new SpreadsheetData();
			var totalCells = cellsWithData.Count;
			var emptyCells = 0;

			foreach (var cell in cellsWithData)
			{
				if (String.IsNullOrWhiteSpace(cell.ToString()))
				{
					emptyCells++;
					continue;
				}

				var sCell = cell.ToString();

				switch ((SpreadsheetColumns)cell.ColumnIndex)
				{
					case SpreadsheetColumns.SatelliteName:
						rowData.SatelliteName = sCell;
						break;
					case SpreadsheetColumns.SatelliteAbbreviation:
						rowData.SatelliteAbbreviation = sCell;
						break;
					case SpreadsheetColumns.Orbit:
						rowData.Orbit = sCell;
						break;
					case SpreadsheetColumns.Hemisphere:
						rowData.Hemisphere = sCell;
						break;
					case SpreadsheetColumns.Operator:
						rowData.Operator = sCell;
						break;
					case SpreadsheetColumns.SatelliteCoverage:
						rowData.Coverage = sCell;
						break;
					case SpreadsheetColumns.SatelliteApplications:
						rowData.SatelliteApplications = sCell;
						break;
					case SpreadsheetColumns.SatelliteInfo:
						rowData.SatelliteInfo = sCell;
						break;
					case SpreadsheetColumns.Manufacturer:
						rowData.Manufacturer = sCell;
						break;
					case SpreadsheetColumns.Country:
						rowData.Country = sCell;
						break;
					case SpreadsheetColumns.LaunchInfo:
						rowData.LaunchInfo = sCell;
						break;
					case SpreadsheetColumns.LaunchServiceDate:
						rowData.LaunchInServiceDate = DateTime.TryParse(sCell, out DateTime parsedDate) ? parsedDate : DateTime.MinValue;

						break;
					default:
						// no action
						break;
				}
			}

			if (emptyCells == totalCells)
			{
				return null;
			}

			return rowData;
		}

		public class SpreadsheetData
		{
			public string SatelliteName { get; set; }

			public string SatelliteAbbreviation { get; set; }

			public string Orbit { get; set; }

			public string Hemisphere { get; set; }

			public string Operator { get; set; }

			public string Coverage { get; set; }

			public string SatelliteApplications { get; set; }

			public string SatelliteInfo { get; set; }

			public string Manufacturer { get; set; }

			public string Country { get; set; }

			public string LaunchInfo { get; set; }

			public DateTime LaunchInServiceDate { get; set; }
		}
	}
}
