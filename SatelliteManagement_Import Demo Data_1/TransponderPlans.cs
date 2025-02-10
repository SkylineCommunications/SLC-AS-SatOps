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
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	internal class TransponderPlans
	{
		private const int StartColumnIndex = 0;
		private const int EndColumnIndex = 6;

		private readonly Dictionary<string, List<TransponderPlans>> slotsPerPlanDic;

		public TransponderPlans()
		{
			slotsPerPlanDic = new Dictionary<string, List<TransponderPlans>>();
		}

		private enum SpreadsheetColumns
		{
			Id,
			PlanName,
			AppliedTransponderIds,
			DefinitionSlotName,
			DefinitionSlotSize,
			RelativeStartFrequency,
			RelativeEndFrequency,
		}

		public string Id { get; set; }

		public string PlanName { get; set; }

		public string AppliedTransponderIds { get; set; }

		public string DefinitionSlotName { get; set; }

		public double DefinitionSlotSize { get; set; }

		public double RelativeStartFrequency { get; set; }

		public double RelativeEndFrequency { get; set; }

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

				if (slotsPerPlanDic.ContainsKey(tableRow.Id))
				{
					slotsPerPlanDic[tableRow.Id].Add(tableRow);
				}
				else
				{
					slotsPerPlanDic.Add(tableRow.Id, new List<TransponderPlans> { tableRow });
				}
			}
		}

		internal static TransponderPlans GetRowData(List<ICell> cellsWithData)
		{
			var rowData = new TransponderPlans();
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
					case SpreadsheetColumns.PlanName:
						rowData.PlanName = sCell;
						break;
					case SpreadsheetColumns.AppliedTransponderIds:
						rowData.AppliedTransponderIds = sCell;
						break;
					case SpreadsheetColumns.DefinitionSlotName:
						rowData.DefinitionSlotName = sCell;
						break;
					case SpreadsheetColumns.DefinitionSlotSize:
						rowData.DefinitionSlotSize = Convert.ToDouble(sCell);
						break;
					case SpreadsheetColumns.RelativeStartFrequency:
						rowData.RelativeStartFrequency = Convert.ToDouble(sCell);
						break;
					case SpreadsheetColumns.RelativeEndFrequency:
						rowData.RelativeEndFrequency = Convert.ToDouble(sCell);
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

		internal static void ApplyTransponderPlans(IEngine engine, SatOpsLogger logger, DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler, string appliedTransponderIDs, DomInstance transponderPlanInstance)
		{
			var transponders = appliedTransponderIDs.Split(',');
			foreach (var transponder in transponders)
			{
				if (String.IsNullOrWhiteSpace(transponder))
				{
					continue;
				}

				TransponderPlan transponderPlan = new TransponderPlan(engine, logger, satelliteManagementHandler, new DomApplications.SatelliteManagement.TransponderPlan(satelliteManagementHandler, transponderPlanInstance));
				transponderPlan.ApplyTransponder(Guid.Parse(transponder));
			}
		}

		internal static DomInstance CreateInstance(DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler, KeyValuePair<string, List<TransponderPlans>> row)
		{
			var instanceGuid = Guid.Parse(row.Key);
			var statusId = "active";

			var firstRow = row.Value[0];
			var instanceBuilder = new DomInstanceBuilder(SlcSatellite_Management.Definitions.TransponderPlans)
					.WithID(instanceGuid)
					.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.TransponderPlan.Id)
						.WithFieldValue(SlcSatellite_Management.Sections.TransponderPlan.PlanName, firstRow.PlanName)
						.WithFieldValue(SlcSatellite_Management.Sections.TransponderPlan.AppliedTransponderIds, firstRow.AppliedTransponderIds));

			foreach (var plan in row.Value)
			{
				instanceBuilder.AddSection(new DomSectionBuilder(SlcSatellite_Management.Sections.SlotDefinition.Id)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotName, plan.DefinitionSlotName)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.DefinitionSlotSize, plan.DefinitionSlotSize)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.RelativeStartFrequency, plan.RelativeStartFrequency)
					.WithFieldValue(SlcSatellite_Management.Sections.SlotDefinition.RelativeEndFrequency, plan.RelativeEndFrequency));
			}

			var newInstance = instanceBuilder.Build();
			newInstance.StatusId = statusId;

			return satelliteManagementHandler.DomHelper.DomInstances.Create(newInstance);
		}

		internal void CreateInstances(IEngine engine, SatOpsLogger logger, DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler)
		{
			var totalRows = slotsPerPlanDic.Count;
			var currentCount = 0;
			foreach (var row in slotsPerPlanDic)
			{
				try
				{
					var firstRow = row.Value[0];
					var instance = CreateInstance(satelliteManagementHandler, row);
					ApplyTransponderPlans(engine, logger, satelliteManagementHandler, firstRow.AppliedTransponderIds, instance);
				}
				catch (Exception ex)
				{
					logger.Warning($"Exception thrown: {Environment.NewLine}{ex}");
				}

				currentCount++;
			}

			if (currentCount == totalRows)
			{
				logger.Information("Transponder Plans imported.");
			}
		}
	}
}
