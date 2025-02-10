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
	using Skyline.DataMiner.Utils.SatOps.Common.DOM.Builders;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;
	using Skyline.DataMiner.Net.Sections;

	internal class Transponders
	{
		private const int StartColumnIndex = 0;
		private const int EndColumnIndex = 8;

		private readonly List<Transponders> spreadsheetRows;
		private List<DomInstance> satelliteDomInstancesList;
		private List<DomInstance> beamDomInstancesList;

		public Transponders()
		{
			spreadsheetRows = new List<Transponders>();
			satelliteDomInstancesList = new List<DomInstance>();
			beamDomInstancesList = new List<DomInstance>();
		}

		private enum SpreadsheetColumns
		{
			Id,
			TransponderSatellite,
			Beam,
			TransponderName,
			Band,
			Bandwidth,
			StartFrequency,
			StopFrequency,
			Polarization,
		}

		public string Id { get; set; }

		public string TransponderSatellite { get; set; }

		public string Beam { get; set; }

		public string TransponderName { get; set; }

		public string Band { get; set; }

		public double Bandwidth { get; set; }

		public double StartFrequency { get; set; }

		public double StopFrequency { get; set; }

		public string Polarization { get; set; }

		internal static Transponders GetRowData(List<ICell> cellsWithData)
		{
			var rowData = new Transponders();
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
					case SpreadsheetColumns.TransponderSatellite:
						rowData.TransponderSatellite = sCell;
						break;
					case SpreadsheetColumns.Beam:
						rowData.Beam = sCell;
						break;
					case SpreadsheetColumns.TransponderName:
						rowData.TransponderName = sCell;
						break;
					case SpreadsheetColumns.Band:
						rowData.Band = sCell;
						break;
					case SpreadsheetColumns.Bandwidth:
						rowData.Bandwidth = Convert.ToDouble(sCell);
						break;
					case SpreadsheetColumns.StartFrequency:
						rowData.StartFrequency = Convert.ToDouble(sCell);
						break;
					case SpreadsheetColumns.StopFrequency:
						rowData.StopFrequency = Convert.ToDouble(sCell);
						break;
					case SpreadsheetColumns.Polarization:
						rowData.Polarization = sCell;
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

		internal void GetRows(ISheet sheet)
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

		internal void CreateInstances(IEngine engine, SatOpsLogger logger, DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler)
		{
			satelliteDomInstancesList = Satellites.GetSatelliteDomInstances(satelliteManagementHandler.DomHelper);
			beamDomInstancesList = Beams.GetBeamDomInstances(satelliteManagementHandler.DomHelper);

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
				logger.Information("Transponders imported.");
			}
		}

		internal void CreateInstance(DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler, Transponders row)
		{
			Guid instanceGuid = Guid.Parse(row.Id);
			string statusId = "active";

			var satelliteGuid = GetSatelliteDomInstanceByName(satelliteManagementHandler.DomHelper, row.TransponderSatellite);
			var beamGuid = GetBeamDomInstanceByName(satelliteManagementHandler.DomHelper, row.Beam);
			if (String.IsNullOrEmpty(satelliteGuid) || String.IsNullOrEmpty(beamGuid))
			{
				// empty value. Need a log?
			}

			var instanceBuilder = new DomInstanceBuilder(SlcSatellite_Management.Definitions.Transponders)
					.WithID(instanceGuid)
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.Transponder.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.TransponderSatellite, Guid.Parse(satelliteGuid))
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.Beam, Guid.Parse(beamGuid))
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.TransponderName, row.TransponderName)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.Band, row.Band)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.Bandwidth, row.Bandwidth)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.StartFrequency, row.StartFrequency)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.StopFrequency, row.StopFrequency)
						.WithFieldValue(SlcSatellite_Management.Sections.Transponder.Polarization, row.Polarization)).Build();

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

		internal string GetBeamDomInstanceByName(DomHelper domHelper, string beamName)
		{
			foreach (var beam in beamDomInstancesList)
			{
				var name = beam.GetFieldValue<string>(SlcSatellite_Management.Sections.Beam.Id, SlcSatellite_Management.Sections.Beam.BeamName).GetValue();
				if (beamName.Equals(name))
				{
					return beam.ID.Id.ToString();
				}
			}

			return String.Empty;
		}
	}
}
