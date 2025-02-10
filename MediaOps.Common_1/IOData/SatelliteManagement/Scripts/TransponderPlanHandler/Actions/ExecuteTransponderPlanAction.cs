namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderPlanHandler
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	/// <summary>
	/// Represents an action to execute a transponder plan.
	/// </summary>
	public class ExecuteTransponderPlanAction : TransponderPlanInputData
	{
		/// <summary>
		/// Gets or sets the action to be executed on the transponder plan.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public TransponderPlanAction TransponderPlanAction { get; set; }
	}
}
