namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler
{
	/// <summary>
	/// Specifies the various actions that can be performed on a transponder.
	/// </summary>
	public enum TransponderAction
	{
		/// <summary>
		/// No action specified.
		/// </summary>
		None,

		/// <summary>
		/// Activate the transponder.
		/// </summary>
		Activate,

		/// <summary>
		/// Deprecate the transponder.
		/// </summary>
		Deprecate,

		/// <summary>
		/// Move the transponder to edit state.
		/// </summary>
		Edit,
	}
}
