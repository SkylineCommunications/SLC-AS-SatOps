namespace SatelliteManagement_Import_Demo_Data_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using NPOI.SS.UserModel;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.DOM.Builders;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	internal class Beams
	{
		private const int StartColumnIndex = 0;
		private const int EndColumnIndex = 5;

		private readonly List<Beams> spreadsheetRows;
		private List<DomInstance> satelliteDomInstancesList;

		public Beams()
		{
			spreadsheetRows = new List<Beams>();
			satelliteDomInstancesList = new List<DomInstance>();
		}

		private enum SpreadsheetColumns
		{
			Id,
			BeamName,
			BeamSatellite,
			LinkType,
			TransmissionType,
			FootprintFile,
		}

		public string Id { get; set; }

		public string BeamName { get; set; }

		public string BeamSatellite { get; set; }

		public string LinkType { get; set; }

		public string TransmissionType { get; set; }

		public string FootprintFile { get; set; }

		public static List<DomInstance> GetBeamDomInstances(DomHelper domHelper)
		{
			return domHelper.DomInstances.ReadAll(SlcSatellite_Management.Definitions.Beams).ToList();
		}

		public void GetRows(ISheet sheet)
		{
			var numRows = sheet.LastRowNum;

			for (int row = 1; row <= numRows; row++)
			{
				IRow currentRow = sheet.GetRow(row);
				if (currentRow == null)
				{
					continue;
				}

				var selectedCells = currentRow.Cells.Where(c => c.ColumnIndex >= StartColumnIndex && c.ColumnIndex <= EndColumnIndex).ToList();
				var tableRow = GetRowData(selectedCells);
				if (tableRow == null)
				{
					continue;
				}

				spreadsheetRows.Add(tableRow);
			}
		}

		public void CreateInstances(IEngine engine, SatOpsLogger logger, DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler)
		{
			satelliteDomInstancesList = Satellites.GetSatelliteDomInstances(satelliteManagementHandler.DomHelper);

			var totalRows = spreadsheetRows.Count;
			var currentCount = 0;
			foreach (var row in spreadsheetRows)
			{
				try
				{
					CreateInstance(satelliteManagementHandler, row);
				}
				catch (Exception ex)
				{
					logger.Warning($"Exception thrown: {Environment.NewLine}{ex}");
				}

				currentCount++;
			}

			if (currentCount == totalRows)
			{
				logger.Information("Beams imported.");
			}
		}

		internal static Beams GetRowData(List<ICell> cellsWithData)
		{
			var rowData = new Beams();
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
					case SpreadsheetColumns.Id:
						rowData.Id = sCell;
						break;
					case SpreadsheetColumns.BeamName:
						rowData.BeamName = sCell;
						break;
					case SpreadsheetColumns.BeamSatellite:
						rowData.BeamSatellite = sCell;
						break;
					case SpreadsheetColumns.LinkType:
						rowData.LinkType = sCell;
						break;
					case SpreadsheetColumns.TransmissionType:
						rowData.TransmissionType = sCell;
						break;
					case SpreadsheetColumns.FootprintFile:
						rowData.FootprintFile = sCell;
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

		internal void CreateInstance(DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler, Beams row)
		{
			var instanceGuid = Guid.Parse(row.Id);
			var statusId = "active";

			var satelliteGuid = GetSatelliteDomInstanceByName(satelliteManagementHandler.DomHelper, row.BeamSatellite);
			if (String.IsNullOrEmpty(satelliteGuid))
			{
				// empty value. Need a log?
			}

			var instanceBuilder = new DomInstanceBuilder(SlcSatellite_Management.Definitions.Beams)
					.WithID(instanceGuid)
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.Beam.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.Beam.BeamName, row.BeamName)
						.WithFieldValue(SlcSatellite_Management.Sections.Beam.BeamSatellite, Guid.Parse(satelliteGuid))
						.WithFieldValue(SlcSatellite_Management.Sections.Beam.LinkType, row.LinkType)
						.WithFieldValue(SlcSatellite_Management.Sections.Beam.TransmissionType, row.TransmissionType)
						.WithFieldValue(SlcSatellite_Management.Sections.Beam.FootprintFile, row.FootprintFile)).Build();

			instanceBuilder.StatusId = statusId;
			satelliteManagementHandler.DomHelper.DomInstances.Create(instanceBuilder);
		}

		internal string GetSatelliteDomInstanceByName(DomHelper domHelper, string satelliteName)
		{
			foreach (var satellite in satelliteDomInstancesList)
			{
				var name = satellite.GetFieldValue<string>(SlcSatellite_Management.Sections.General.Id, SlcSatellite_Management.Sections.General.SatelliteName).GetValue();
				if (satelliteName.Equals(name))
				{
					return satellite.ID.Id.ToString();
				}
			}

			return String.Empty;
		}
	}
}
