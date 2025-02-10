namespace Skyline.DataMiner.Utils.SatOps.Common.LCA
{
	using System;

	[Serializable]
	public class AppConfig
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string CreatedBy { get; set; }

		public long CreatedAt { get; set; }

		public AppPage[] Pages { get; set; }
	}

	[Serializable]
	public class AppPage
	{
		public string ID { get; set; }

		public string Name { get; set; }
	}
}
