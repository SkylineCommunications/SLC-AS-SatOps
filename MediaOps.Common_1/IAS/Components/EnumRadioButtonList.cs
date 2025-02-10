namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Components
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Utils.SatOps.Common.Tools;

	public class EnumRadioButtonList<T> : RadioButtonList<T>
		where T : struct, Enum
	{
		private readonly Func<T, string> _convertValueToString;

		public EnumRadioButtonList()
		{
			var type = typeof(T);
			var values = Enum.GetValues(type).Cast<T>();

			SetOptions(values);
		}

		public EnumRadioButtonList(Func<T, string> convertValueToString)
		{
			_convertValueToString = convertValueToString ?? throw new ArgumentNullException(nameof(convertValueToString));

			var type = typeof(T);
			var values = Enum.GetValues(type).Cast<T>();

			SetOptions(values);
		}

		public void SetOptions(IEnumerable<T> values)
		{
			if (values is null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			var options = new List<Choice<T>>();

			foreach (var value in values)
			{
				var displayValue = _convertValueToString?.Invoke(value) ?? value.GetDescription();

				options.Add(Choice.Create(value, displayValue));
			}

			Options = options;
		}
	}
}