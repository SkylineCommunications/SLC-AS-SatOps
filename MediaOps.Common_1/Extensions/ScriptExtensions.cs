namespace Skyline.DataMiner.Utils.SatOps.Common.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SatOps.Common.Exceptions;

	public static class ScriptExtensions
	{
		public static IList<T> ReadScriptParamListFromApp<T>(this IEngine engine, string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			}

			var param = engine.GetScriptParam(name);

			if (param == null)
			{
				throw new ArgumentException($"Couldn't find script parameter with name '{name}'");
			}

			try
			{
				if (String.IsNullOrWhiteSpace(param.Value))
				{
					return Array.Empty<T>();
				}

				try
				{
					return JsonConvert.DeserializeObject<T[]>(param.Value);
				}
				catch (Exception)
				{
					// needed for when the value is not encapsulated with []
					return new[] { TryConvertSingleValue<T>(param.Value) };
				}
			}
			catch
			{
				throw new InvalidOperationException($"Unable to convert script parameter '{name}' to list of {typeof(T).Name} (value: {param.Value}).");
			}
		}

		public static IList<string> ReadScriptParamListFromApp(this IEngine engine, string name)
		{
			return ReadScriptParamListFromApp<string>(engine, name);
		}

		public static T ReadScriptParamSingleFromApp<T>(this IEngine engine, string name)
		{
			var values = ReadScriptParamListFromApp<T>(engine, name);

			if (values.Count == 0)
			{
				throw new InvalidScriptInputException($"No value was provided for parameter '{name}'");
			}

			if (values.Count > 1)
			{
				throw new InvalidScriptInputException($"Multiple values were provided for parameter '{name}'");
			}

			return values.First();
		}

		public static T ReadScriptParamSingleFromApp<T>(this IEngine engine, string name, T defaultValue)
		{
			var values = ReadScriptParamListFromApp<T>(engine, name);

			if (values.Count == 0)
			{
				return defaultValue;
			}

			if (values.Count > 1)
			{
				throw new InvalidScriptInputException($"Multiple values were provided for parameter '{name}'");
			}

			return values.First();
		}

		public static string ReadScriptParamSingleFromApp(this IEngine engine, string name)
		{
			return ReadScriptParamSingleFromApp<string>(engine, name);
		}

		private static T TryConvertSingleValue<T>(string value)
		{
			if (typeof(T) == typeof(Guid) && Guid.TryParse(value, out var guid))
			{
				return (T)(object)guid;
			}

			if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}

			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}