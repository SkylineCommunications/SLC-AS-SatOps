namespace Skyline.DataMiner.Utils.SatOps.Common.LCA
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Newtonsoft.Json;

	public static class Applications
	{
		public const string ApplicationsDirectory = @"C:\Skyline DataMiner\applications";

		private static string[] _satopsApplicationNames = new[]
		{
			"Satellite Data & Capacity Planning",
		};

		public static IEnumerable<App> GetApps()
		{
			foreach (string directory in Directory.GetDirectories(ApplicationsDirectory))
			{
				AppInfo appInfo;
				AppConfig publishedConfig;
				AppConfig draftConfig = null;
				try
				{
					appInfo = JsonConvert.DeserializeObject<AppInfo>(File.ReadAllText(Path.Combine(directory, "App.info.json")));
					publishedConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(Path.Combine(directory, $"version_{appInfo.PublicVersion}", "App.config.json")));

					if (appInfo.DraftVersion > 0)
					{
						draftConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(Path.Combine(directory, $"version_{appInfo.DraftVersion}", "App.config.json"))); 
					}
				}
				catch (Exception)
				{
					continue;
				}

				yield return new App(appInfo, publishedConfig, draftConfig, directory);
			}
		}

		public static bool IsSatOpsApp(App app)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			return _satopsApplicationNames.Contains(app.Name);
		}
	}
}
