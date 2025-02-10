namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SlotHandler
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	/// <summary>
	/// Represents an action to execute a slot.
	/// </summary>
	public class ExecuteSlotAction : SlotInputData
	{
		/// <summary>
		/// Gets or sets the action to be executed on the slot.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public SlotAction SlotAction { get; set; }
	}
}
