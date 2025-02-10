namespace Skyline.DataMiner.Utils.SatOps.Common.DOM
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;

	public static class DomDefinitionExtensions
	{
		public static DomDefinition GetByID(this DomDefinitionCrudHelperComponent helper, Guid id)
		{
			var filter = DomDefinitionExposers.Id.Equal(id);
			return helper.Read(filter).SingleOrDefault();
		}

		public static DomDefinition GetByName(this DomDefinitionCrudHelperComponent helper, string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
			}

			var filter = DomDefinitionExposers.Name.Equal(name);
			return helper.Read(filter).SingleOrDefault();
		}
	}
}