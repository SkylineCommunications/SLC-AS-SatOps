namespace Skyline.DataMiner.Utils.SatOps.GQI.Common.Data_Sources
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Utils.SatOps.Common.LCA;

	[GQIMetaData(Name = "SatOps.GQI.Common.Version Info")]
	public class VersionInfoDataSource : IGQIDataSource
	{
		public GQIColumn[] GetColumns()
		{
			return new[]
			{
				new GQIStringColumn("Component"),
				new GQIStringColumn("Version"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var rows = new List<GQIRow>();
			rows.AddRange(GetPackageVersions());
			rows.AddRange(GetAppsVersions());

			return new GQIPage(rows.ToArray())
			{
				HasNextPage = false,
			};
		}

		private IEnumerable<GQIRow> GetPackageVersions()
		{
			var path = @"C:\Skyline DataMiner\Documents\DataMiner Solutions\SatOps\packageVersions.json";

			if (!File.Exists(path))
			{
				yield break;
			}

			var json = File.ReadAllText(path);

			// {"InstallPackage":"9101.0.1","ScriptsPackage":"9001.0.269"}
			var definition = new { InstallPackage = String.Empty, ScriptsPackage = String.Empty };
			var versionInfo = JsonConvert.DeserializeAnonymousType(json, definition);

			yield return new GQIRow(new[]
			{
				new GQICell{ Value = "Package" },
				new GQICell{ Value = versionInfo.InstallPackage },
			});

			yield return new GQIRow(new[]
			{
				new GQICell{ Value = "Scripts" },
				new GQICell{ Value = versionInfo.ScriptsPackage },
			});
		}

		private IEnumerable<GQIRow> GetAppsVersions()
		{
			var apps = Applications.GetApps()
				.Where(Applications.IsSatOpsApp)
				.OrderBy(x => x.Name, new NaturalSortComparer());

			foreach (var app in apps)
			{
				yield return new GQIRow(new[]
				{
					new GQICell{ Value = "App - " + app.Name },
					new GQICell{ Value = Convert.ToString(app.Info.PublicVersion) },
				});
			}
		}
	}
}
