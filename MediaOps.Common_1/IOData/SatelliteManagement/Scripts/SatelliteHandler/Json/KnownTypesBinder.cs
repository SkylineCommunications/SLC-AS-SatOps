namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.SatelliteHandler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json.Serialization;

	using Skyline.DataMiner.MediaOps.Communication.Exceptions;
	using Skyline.DataMiner.MediaOps.Communication.ScriptData;
	using Skyline.DataMiner.MediaOps.Communication.TraceData;

	/// <summary>
	/// A binder that manages known types for JSON serialization and deserialization.
	/// </summary>
	public sealed class KnownTypesBinder : ISerializationBinder
	{
		private readonly IList<Type> knownTypes = new List<Type>
		{
			typeof(MediaOpsException),
			typeof(MediaOpsTraceData),
			typeof(MediaOpsInfoData),
			typeof(MediaOpsWarningData),
			typeof(MediaOpsErrorData),
			typeof(ScriptDataBase),
			typeof(ScriptDataOutputBase),

			typeof(ExecuteSatelliteAction),

			typeof(SatelliteAction),

			typeof(InputData),
			typeof(OutputData),
			typeof(SatelliteHandlerData),
			typeof(SatelliteInputData),

			typeof(ActionOutput),
		};

		/// <summary>
		/// When overridden in a derived class, controls the binding of a serialized object to a type.
		/// </summary>
		/// <param name="serializedType">The type of the object being serialized.</param>
		/// <param name="assemblyName">The name of the assembly where the type is located.</param>
		/// <param name="typeName">The name of the serialized type.</param>
		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = serializedType.Name;
		}

		/// <summary>
		/// When overridden in a derived class, controls the binding of a serialized type to a name.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly where the type is located.</param>
		/// <param name="typeName">The name of the serialized type.</param>
		/// <returns>The type to bind to.</returns>
		public Type BindToType(string assemblyName, string typeName)
		{
			return knownTypes.SingleOrDefault(t => t.Name == typeName);
		}
	}
}
