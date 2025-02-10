namespace Skyline.DataMiner.Utils.SatOps.Common.IOData.SatelliteManagement.Scripts.TransponderHandler
{
	using System;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.MediaOps.Communication.Exceptions;

	/// <summary>
	/// Represents the base class for input data.
	/// </summary>
	public class InputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputData"/> class.
		/// </summary>
		protected InputData()
		{
		}

		/// <summary>
		/// Sends the input data to the transponder handler.
		/// </summary>
		/// <param name="engine">The Engine interface to use for sending the input data.</param>
		/// <param name="useSerialization">Indicates whether to use serialization for the communication.</param>
		/// <returns>The output of the action as an <see cref="ActionOutput"/> object.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="dms"/> is null.</exception>
		public OutputData SendToTransponderHandler(IEngine engine, bool useSerialization = false)
		{
			var data = new TransponderHandlerData
			{
				Input = this,
			};

			if (useSerialization)
			{
				data.PrepareJsonCommunication(new KnownTypesBinder(), "TransponderHandler Result");
			}

			var subScript = engine.PrepareSubScript("SatelliteManagement_Core_TransponderHandler");

			using (data)
			{
				subScript.SelectScriptParam("Input Data", data.Key);
				subScript.Synchronous = true;
				subScript.LockElements = true;
				subScript.StartScript();
			}

			if (data.Communication == DataMiner.MediaOps.Communication.ScriptData.ScriptDataBase.CommunicationType.Json)
			{
				if (!subScript.GetScriptResult().TryGetValue(data.OutputReturnKey, out var jsonData))
				{
					return null;
				}

				data = JsonConvert.DeserializeObject<TransponderHandlerData>(jsonData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = new KnownTypesBinder() });
			}

			data.Output.RethrowException();

			if (data.Output.TraceData != null && !data.Output.TraceData.HasSucceeded())
			{
				throw new MediaOpsException(data.Output.TraceData);
			}

			return data.Output;
		}
	}
}
