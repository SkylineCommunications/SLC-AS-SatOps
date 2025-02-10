namespace Generate_Flows_1
{
	using System;
	using System.Globalization;

	using Skyline.DataMiner.Net.ServiceManager.Objects;

	public class Interface
	{
		public Interface(object[] row)
		{
			if (row == null)
			{
				throw new ArgumentNullException(nameof(row));
			}

			Id = Convert.ToInt32(row[0], CultureInfo.InvariantCulture);
			Name = Convert.ToString(row[1]);
			CustomName = Convert.ToString(row[2]);
			Type = (InterfaceType)Convert.ToUInt32(row[3], CultureInfo.InvariantCulture);
			DynamicLink = Convert.ToString(row[5]);
			IsInternal = Convert.ToBoolean(Convert.ToInt32(row[6]));

			string[] linkParts = DynamicLink.Split(';');
			if (linkParts.Length == 2)
			{
				if (int.TryParse(linkParts[0], out int tableLink))
				{
					TableLink = tableLink;
				}

				RowLink = linkParts[1];
			}
		}

		public int Id { get; }

		public string Name { get; }

		public string CustomName { get; }

		public InterfaceType Type { get; }

		public string DynamicLink { get; }

		public bool IsInternal { get; }

		public int TableLink { get; }

		public string RowLink { get; }
	}
}