namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.BeamHandler
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	/// <summary>
	/// Represents an action to execute a beam.
	/// </summary>
	public class ExecuteBeamAction : BeamInputData
	{
		/// <summary>
		/// Gets or sets the action to be executed on the beam.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public BeamAction BeamAction { get; set; }
	}
}
