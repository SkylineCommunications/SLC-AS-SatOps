namespace Generate_Demo_Data_1
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;

	internal class Satellite
	{
		internal static void Import(IEngine engine, List<Exception> exceptions)
		{
			if (!TryExecuteSatelliteManagementDemoDataScript(engine, InputData.ScriptAction.Install, out var exception))
				exceptions.Add(exception);
		}

		internal static void Uninstall(IEngine engine, List<Exception> exceptions)
		{
			if (!TryExecuteSatelliteManagementDemoDataScript(engine, InputData.ScriptAction.Uninstall, out var exception))
				exceptions.Add(exception);
		}

		private static bool TryExecuteSatelliteManagementDemoDataScript(IEngine engine, InputData.ScriptAction action, out Exception exception)
		{
			exception = null;

			try
			{
				return ExecuteSatelliteManagementDemoDataScript(engine, action);
			}
			catch (Exception e)
			{
				exception = e;
			}

			return false;
		}

		private static bool ExecuteSatelliteManagementDemoDataScript(IEngine engine, InputData.ScriptAction action)
		{
			if (action == InputData.ScriptAction.Install)
			{
				var subScript = engine.PrepareSubScript("SAT-AS-Import Demo Data");
				subScript.Synchronous = true;
				subScript.StartScript();
			}
			else
			{
				var subScript = engine.PrepareSubScript("SAT-AS-Delete Satellite Module Instances");
				subScript.Synchronous = true;
				subScript.StartScript();
			}

			return true;
		}
	}
}
