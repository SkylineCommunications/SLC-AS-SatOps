namespace Skyline.DataMiner.Utils.SatOps.Common.LCA
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Newtonsoft.Json.Linq;

	public class App
	{
		public App(AppInfo appInfo, AppConfig publishedConfig, AppConfig draftConfig, string path)
		{
			Path = path;

			Info = appInfo;
			Config = publishedConfig;
			DraftConfig = draftConfig;
		}

		public AppInfo Info { get; }

		public AppConfig Config { get; }

		public AppConfig DraftConfig { get; }

		public string Path { get; }

		public string ID => Config.ID;

		public string Name => Config.Name;

		public ICollection<string> GetReferencedScriptNames()
		{
			var scriptNames = new HashSet<string>();

			var version = Info.PublicVersion;
			var dir = new DirectoryInfo(System.IO.Path.Combine(Path, $"version_{version}"));

			foreach (var jsonFile in dir.EnumerateFiles("*.json", SearchOption.AllDirectories))
			{
				var json = File.ReadAllText(jsonFile.FullName);
				var jsonObj = (JContainer)JObject.Parse(json);
				var scriptTokens = GetDescendantPropertyTokens(jsonObj, "Script");

				scriptNames.UnionWith(scriptTokens.Select(x => x.ToString()));
			}

			return scriptNames;
		}

		private IEnumerable<JToken> GetDescendantPropertyTokens(JContainer container, string propertyName)
		{
			foreach (var descendant in container.DescendantsAndSelf())
			{
				if (descendant is JProperty property && property.Name == propertyName)
				{
					yield return property.Value;
				}
			}
		}
	}
}
