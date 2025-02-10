namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SatOps.Common.Utils;

	using DomApplications = Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications;


	public abstract class SatelliteManagementBase : ISatelliteManagementBase
	{
		protected readonly IEngine engine;
		protected readonly SatOpsLogger logger;
		protected readonly DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler;

		protected SatelliteManagementBase(
			IEngine engine,
			SatOpsLogger logger,
			DomApplications.SatelliteManagement.SatelliteManagementHandler satelliteManagementHandler)
		{
			this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.satelliteManagementHandler = satelliteManagementHandler ??
				throw new ArgumentNullException(nameof(satelliteManagementHandler));
		}

		public abstract void Activate();

		public abstract void Deprecate();

		public abstract void Edit();

		public abstract void Error();
	}
}
