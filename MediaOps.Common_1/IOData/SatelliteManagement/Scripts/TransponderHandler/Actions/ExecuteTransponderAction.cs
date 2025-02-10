namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	/// <summary>
	/// Represents an action to execute a transponder.
	/// </summary>
	public class ExecuteTransponderAction : TransponderInputData
	{
		/// <summary>
		/// Gets or sets the action to be execute on the transponder.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public TransponderAction TransponderAction { get; set; }
	}
}
