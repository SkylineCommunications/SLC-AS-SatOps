namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler
{
	/// <summary>
	/// Specifies the various actions that can be performed on a satellite.
	/// </summary>
	public enum SatelliteAction
	{
		/// <summary>
		/// No action specified.
		/// </summary>
		None,

		/// <summary>
		/// Activate the satellite.
		/// </summary>
		Activate,

		/// <summary>
		/// Deprecate the satellite.
		/// </summary>
		Deprecate,

		/// <summary>
		/// Move the satellite to edit state.
		/// </summary>
		Edit,
	}
}
