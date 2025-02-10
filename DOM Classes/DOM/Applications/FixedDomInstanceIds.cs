namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomInstanceIds
{
	using System;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;

	public static class SlcWorkflow
	{
		public static class Definitions
		{
			public static class JobNodeRelationshipActions
			{
				public static DomInstanceId TransponderSlot
				{
					get;
				}

				= new DomInstanceId(new Guid("6c825556-7260-4365-b4a8-5fa55bc33919"))
				{ ModuleId = "(slc)workflow" };

				public static DomInstanceId LinkedPool
				{
					get;
				}

				= new DomInstanceId(new Guid("d21969ce-ced8-40c6-bf65-a1452d99c5b4"))
				{ ModuleId = "(slc)workflow" };
			}
		}
	}
}
