namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	/// <summary>
	/// Represents an action to execute a satellite.
	/// </summary>
	public class ExecuteSatelliteAction : SatelliteInputData
	{
		/// <summary>
		/// Gets or sets the action to be executed on the satellite.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public SatelliteAction SatelliteAction { get; set; }
	}
}
