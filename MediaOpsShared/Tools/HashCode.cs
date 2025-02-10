namespace Skyline.DataMiner.Utils.SatOps.Common.Tools
{
	public static class HashCode
	{
		public static int Combine(params object[] objects)
		{
			unchecked
			{
				var hash = 17;

				foreach (var obj in objects)
				{
					hash *= 23;
					hash += obj?.GetHashCode() ?? 0;
				}

				return hash;
			}
		}
	}
}
