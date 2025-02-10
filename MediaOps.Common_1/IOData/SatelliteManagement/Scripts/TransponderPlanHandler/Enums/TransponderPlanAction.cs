namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler
{
	/// <summary>
	/// Specifies the various actions that can be performed on a transponder plan.
	/// </summary>
	public enum TransponderPlanAction
	{
		/// <summary>
		/// No action specified.
		/// </summary>
		None,

		/// <summary>
		/// Activate the transponder plan.
		/// </summary>
		Activate,

		/// <summary>
		/// Deprecate the transponder plan.
		/// </summary>
		Deprecate,

		/// <summary>
		/// Move the transponder plan to edit state.
		/// </summary>
		Edit,
	}
}
