namespace Skyline.DataMiner.Utils.SatOps.Common.IAS
{
	using Skyline.DataMiner.Utils.SatOps.Common.IAS.Components;

	public static class AutomationData
	{
		public static readonly string InitialDropdownValue = "- Select -";

		public static Choice<T> CreateDefaultDropDownOption<T>()
		{
			return Choice.Create<T>(default, InitialDropdownValue);
		}
	}
}
