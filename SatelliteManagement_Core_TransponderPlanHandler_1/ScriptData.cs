namespace SatelliteManagement_Core_TransponderPlanHandler_1
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;

	internal class ScriptData
	{
		#region Fields
		private readonly IEngine engine;
		private readonly SatOpsLogger logger;
		private DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;
		#endregion

		public ScriptData(IEngine engine, SatOpsLogger logger)
		{
			this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

			Init();
		}

		#region Properties
		internal IEngine Engine => engine;

		internal SatOpsLogger Logger => logger;

		internal DomApplications.SatelliteManagement.SatelliteManagementHandler SatelliteManagementHandler => satelliteManagementHandler;
		#endregion

		#region Methods
		private void Init()
		{
			satelliteManagementHandler = new DomApplications.SatelliteManagement.SatelliteManagementHandler(engine);
		}
		#endregion
	}
}
