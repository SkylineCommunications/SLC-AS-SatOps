namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler
{
	/// <summary>
	/// Specifies the various actions that can be performed on a beam.
	/// </summary>
	public enum BeamAction
	{
		/// <summary>
		/// No action specified.
		/// </summary>
		None,

		/// <summary>
		/// Activate the beam.
		/// </summary>
		Activate,

		/// <summary>
		/// Deprecate the beam.
		/// </summary>
		Deprecate,

		/// <summary>
		/// Move the beam to edit state.
		/// </summary>
		Edit,

		/// <summary>
		/// Move the beam to error state
		/// </summary>
		Error,
	}
}
