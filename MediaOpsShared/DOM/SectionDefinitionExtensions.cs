namespace Skyline.DataMiner.Utils.SatOps.Common.DOM
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;

	public static class SectionDefinitionExtensions
	{
		public static SectionDefinition GetByID(this SectionDefinitionCrudHelperComponent helper, Guid id)
		{
			var filter = SectionDefinitionExposers.ID.Equal(id);
			return helper.Read(filter).SingleOrDefault();
		}

		public static SectionDefinition GetByName(this SectionDefinitionCrudHelperComponent helper, string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
			}

			var filter = SectionDefinitionExposers.Name.Equal(name);
			return helper.Read(filter).SingleOrDefault();
		}

		public static FieldDescriptor GetFieldDescriptorByName(this SectionDefinition definition, string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
			}

			var fieldDescriptor = definition.GetAllFieldDescriptors().SingleOrDefault(x => String.Equals(x.Name, name));
			if (fieldDescriptor == null)
			{
				throw new ArgumentException($"Field descriptor with name '{name}' doesn't exist in definition '{definition.GetName()}'.", nameof(name));
			}

			return fieldDescriptor;
		}
	}
}