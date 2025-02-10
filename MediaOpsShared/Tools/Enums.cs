namespace Skyline.DataMiner.Utils.SatOps.Common.Tools
{
	using System;
	using System.ComponentModel;

	public static class Enums
	{
		public static string GetDescription<T>(this T value) where T : struct, Enum
		{
			var name = Convert.ToString(value);
			var field = typeof(T).GetField(name);

			if (field != null &&
				Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
			{
				return attr.Description;
			}

			return name;
		}
	}
}
