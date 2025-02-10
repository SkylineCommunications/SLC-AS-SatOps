namespace SatelliteManagement_Import_Demo_Data_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using NPOI.SS.UserModel;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Utils.DOM.Builders;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	internal class Satellites
	{
		private const int StartColumnIndex = 0;
		private const int EndColumnIndex = 13;

		private readonly List<Satellites> spreadsheetRows;

		public Satellites()
		{
			spreadsheetRows = new List<Satellites>();
		}

		private enum SpreadsheetColumns
		{
			Id,
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

		public string Id { get; set; }

		public string SatelliteName { get; set; }

		public string SatelliteAbbreviation { get; set; }

		public string Orbit { get; set; }

		public string Hemisphere { get; set; }

		public string Azimuth { get; set; }

		public string Operator { get; set; }

		public string Coverage { get; set; }

		public string SatelliteApplications { get; set; }

		public string SatelliteInfo { get; set; }

		public string Manufacturer { get; set; }

		public string Country { get; set; }

		public string LaunchInfo { get; set; }

		public DateTime LaunchInServiceDate { get; set; }

		public static List<DomInstance> GetSatelliteDomInstances(DomHelper domHelper)
		{
			return domHelper.DomInstances.ReadAll(SlcSatellite_Management.Definitions.Satellites).ToList();
		}

		public static Satellites GetRowData(List<ICell> cellsWithData)
		{
			var rowData = new Satellites();
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
					case SpreadsheetColumns.Azimuth:
						rowData.Azimuth = sCell;
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
#pragma warning disable S6580 // Use a format provider when parsing date and time
						rowData.LaunchInServiceDate = DateTime.TryParse(sCell, out DateTime parsedDate) ? parsedDate : DateTime.MinValue;
#pragma warning restore S6580 // Use a format provider when parsing date and time

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

		public static void CreateInstance(DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler, Satellites row)
		{
			var instanceGuid = Guid.Parse(row.Id);
			var statusId = "active";

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
			satelliteManagementHandler.DomHelper.DomInstances.Create(instanceBuilder);
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
				logger.Information("Satellites imported.");
			}
		}
	}
}
