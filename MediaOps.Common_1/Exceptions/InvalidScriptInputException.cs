namespace Skyline.DataMiner.Utils.SatOps.Common.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class InvalidScriptInputException : Exception
	{
		public InvalidScriptInputException()
		{
		}

		public InvalidScriptInputException(string message) : base(message)
		{
		}

		public InvalidScriptInputException(string message, Exception inner) : base(message, inner)
		{
		}

		protected InvalidScriptInputException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
