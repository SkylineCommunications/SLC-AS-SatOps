namespace Generate_Flows_1
{
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public static class DmsElementExtensions
	{
		public static IEnumerable<Interface> GetExternalInterfaces(this IDmsElement element)
		{
			const int IsInternalPid = 65151;
			const string False = "0";
			var externalInterfacesFilter = new[]
			{
				new ColumnFilter { Pid = IsInternalPid, Value = False, ComparisonOperator = ComparisonOperator.Equal },
			};

			const int DcfInterfacesTablePid = 65049;
			return element.GetTable(DcfInterfacesTablePid)
				.QueryData(externalInterfacesFilter)
				.Select(row => new Interface(row));
		}
	}
}