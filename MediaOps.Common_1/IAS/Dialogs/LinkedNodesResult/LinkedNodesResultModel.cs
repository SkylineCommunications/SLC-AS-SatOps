namespace Skyline.DataMiner.Utils.SatOps.Common.IAS.Dialogs.LinkedNodesResult
{
	using Skyline.DataMiner.Utils.SatOps.Common.IAS;

	internal class LinkedNodesResultModel
	{
		public LinkedNodesResultModel(LinkedNodesResult result)
		{
			Result = result;
		}

		public LinkedNodesResult Result { get; private set; }
	}
}
