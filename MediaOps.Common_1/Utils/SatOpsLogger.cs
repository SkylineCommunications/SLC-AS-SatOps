namespace Skyline.DataMiner.Utils.SatOps.Common.Utils
{
	using System;

	using Serilog;
	using Serilog.Core;

	public sealed class SatOpsLogger : IDisposable
	{
		private readonly Logger _logger;

		public SatOpsLogger(Types logType)
		{
			_logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.File(
					$"C:\\Skyline DataMiner\\Logging\\SatOps\\{logType.ToString()}.txt",
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();
		}

		public enum Types
		{
			Satellite,
		}

		public void Debug(string message)
		{
			_logger.Debug(message);
		}

		public void Error(string message)
		{
			_logger.Error(message);
		}

		public void Error(Exception ex, string message)
		{
			_logger.Error(ex, message);
		}

		public void Information(string message)
		{
			_logger.Information(message);
		}

		public void Warning(string message)
		{
			_logger.Warning(message);
		}

		public void Warning(Exception ex, string message)
		{
			_logger.Warning(ex, message);
		}

		public void Dispose()
		{
			_logger.Dispose();
		}
	}
}