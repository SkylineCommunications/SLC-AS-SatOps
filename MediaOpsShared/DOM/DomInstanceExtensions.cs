﻿namespace Skyline.DataMiner.Utils.SatOps.Common.DOM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Sections.Fields;
	using Skyline.DataMiner.Net.Apps.Sections.Sections;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.SatOps.Common.DOM;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;
	using Skyline.DataMiner.Utils.SatOps.Common.Tools;

	public static class DomInstanceExtensions
	{
		public static DomInstance GetByID(this DomInstanceCrudHelperComponent helper, Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);
			return helper.Read(filter).SingleOrDefault();
		}

		public static IEnumerable<DomInstance> GetByIDs(this DomInstanceCrudHelperComponent helper, IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				throw new ArgumentNullException(nameof(ids));
			}

			return FilterQueryExecutor.RetrieveFilteredItems(ids, DomInstanceExposers.Id.Equal, helper.Read);
		}

		public static IEnumerable<DomInstance> ReadAll(this DomInstanceCrudHelperComponent helper, DomDefinitionId definitionId)
		{
			var filter = DomInstanceExposers.DomDefinitionId.Equal(definitionId.Id);
			return helper.Read(filter);
		}

		public static IEnumerable<DomInstance> ReadAll(this DomInstanceCrudHelperComponent helper, DomDefinition definition)
		{
			return helper.ReadAll(definition.ID);
		}

		public static void BulkDelete(this DomInstanceCrudHelperComponent helper, IEnumerable<DomInstance> instances)
		{
			if (instances == null)
			{
				throw new ArgumentNullException(nameof(instances));
			}

			foreach (var batch in instances.Batch(100))
			{
				helper.Delete(batch.ToList()).ThrowOnFailure();
			}
		}

		public static Section GetSection(this DomInstance instance, SectionDefinitionID definitionId)
		{
			if (definitionId == null)
			{
				throw new ArgumentNullException(nameof(definitionId));
			}

			return instance.Sections.FirstOrDefault(x => x.SectionDefinitionID.Equals(definitionId));
		}

		public static IEnumerable<Section> GetSectionsWithDefinition(this DomInstance instance, SectionDefinitionID definitionId)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			return instance.Sections.Where(x => x.SectionDefinitionID.Equals(definitionId));
		}

		public static IEnumerable<Section> GetSectionsWithDefinition(this DomInstance instance, SectionDefinition definition)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			var definitionId = definition.GetID();
			return instance.GetSectionsWithDefinition(definitionId);
		}

		public static IEnumerable<Section> GetSectionsWithDefinition(this DomInstance instance, string definitionName, DomCache cache)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (String.IsNullOrWhiteSpace(definitionName))
			{
				throw new ArgumentException($"'{nameof(definitionName)}' cannot be null or whitespace.", nameof(definitionName));
			}

			if (cache == null)
			{
				throw new ArgumentNullException(nameof(cache));
			}

			var definition = cache.GetSectionDefinitionByName(definitionName);

			if (definition == null)
			{
				throw new ArgumentException($"Couldn't find section definition with name '{definitionName}'", nameof(definitionName));
			}

			return instance.GetSectionsWithDefinition(definition);
		}

		public static T GetFieldValue<T>(this ISectionContainer container, string sectionName, string fieldName, DomCache cache)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (String.IsNullOrWhiteSpace(sectionName))
			{
				throw new ArgumentException($"'{nameof(sectionName)}' cannot be null or whitespace.", nameof(sectionName));
			}

			if (String.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
			}

			if (cache == null)
			{
				throw new ArgumentNullException(nameof(cache));
			}

			var section = cache.GetSectionDefinitionByName(sectionName);
			var field = section.GetFieldDescriptorByName(fieldName);

			var valueWrapper = container.GetFieldValue<T>(section, field);

			return valueWrapper != null ? valueWrapper.Value : default;
		}

		public static void SetFieldValue<T>(this ISectionContainer container, SectionDefinition sectionDefinition, FieldDescriptor fieldDescriptor, T value)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (sectionDefinition == null)
			{
				throw new ArgumentNullException(nameof(sectionDefinition));
			}

			if (fieldDescriptor == null)
			{
				throw new ArgumentNullException(nameof(fieldDescriptor));
			}

			if (!Equals(value, default))
			{
				container.AddOrUpdateFieldValue(sectionDefinition, fieldDescriptor, value);
			}
			else
			{
				container.RemoveFieldValue(sectionDefinition, fieldDescriptor);
			}
		}

		public static void SetFieldValue<T>(this ISectionContainer container, SectionDefinitionID sectionDefinition, FieldDescriptorID fieldDescriptor, T value)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (sectionDefinition == null)
			{
				throw new ArgumentNullException(nameof(sectionDefinition));
			}

			if (fieldDescriptor == null)
			{
				throw new ArgumentNullException(nameof(fieldDescriptor));
			}

			if (!Equals(value, default))
			{
				container.AddOrUpdateFieldValue(sectionDefinition, fieldDescriptor, value);
			}
			else
			{
				container.RemoveFieldValue(sectionDefinition, fieldDescriptor);
			}
		}

		public static void SetFieldValue<T>(this ISectionContainer container, string sectionName, string fieldName, T value, DomCache cache)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (String.IsNullOrWhiteSpace(sectionName))
			{
				throw new ArgumentException($"'{nameof(sectionName)}' cannot be null or whitespace.", nameof(sectionName));
			}

			if (String.IsNullOrWhiteSpace(fieldName))
			{
				throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
			}

			if (cache == null)
			{
				throw new ArgumentNullException(nameof(cache));
			}

			var section = cache.GetSectionDefinitionByName(sectionName);
			var field = section.GetFieldDescriptorByName(fieldName);

			if (!Equals(value, default))
			{
				container.AddOrUpdateFieldValue(section, field, value);
			}
			else
			{
				container.RemoveFieldValue(section, field);
			}
		}

		public static void RemoveFieldValue(
			this ISectionContainer container,
			SectionDefinition sectionDefinition,
			FieldDescriptor fieldDescriptor)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (sectionDefinition == null)
			{
				throw new ArgumentNullException(nameof(sectionDefinition));
			}

			if (fieldDescriptor == null)
			{
				throw new ArgumentNullException(nameof(fieldDescriptor));
			}

			container.RemoveFieldValue(sectionDefinition.GetID(), fieldDescriptor.ID);
		}

		public static void RemoveFieldValue(
			this ISectionContainer container,
			SectionDefinitionID sectionDefinitionID,
			FieldDescriptorID fieldDescriptorID)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (sectionDefinitionID == null)
			{
				throw new ArgumentNullException(nameof(sectionDefinitionID));
			}

			if (fieldDescriptorID == null)
			{
				throw new ArgumentNullException(nameof(fieldDescriptorID));
			}

			container.GetSections()
				.FirstOrDefault(s => s.SectionDefinitionID.Equals(sectionDefinitionID))
				?.RemoveFieldValueById(fieldDescriptorID);
		}

		public static ListValueWrapper<T> GetOrInitializeListFieldValue<T>(
			this ISectionContainer container,
			SectionDefinition sectionDefinition,
			FieldDescriptor fieldDescriptor)
		{
			return container.GetOrInitializeListFieldValue<T>(sectionDefinition.GetID(), fieldDescriptor.ID);
		}

		public static ListValueWrapper<T> GetOrInitializeListFieldValue<T>(
			this ISectionContainer container,
			SectionDefinitionID sectionDefinitionID,
			FieldDescriptorID fieldDescriptorID)
		{
			ListValueWrapper<T> listValueWrapper =
				container.GetListFieldValue<T>(sectionDefinitionID, fieldDescriptorID);

			if (listValueWrapper == null)
			{
				container.AddOrUpdateListFieldValue(sectionDefinitionID, fieldDescriptorID, new List<T>());
				listValueWrapper = container.GetListFieldValue<T>(sectionDefinitionID, fieldDescriptorID);
			}

			return listValueWrapper;
		}

		public static IEnumerable<DomInstance> WithDefinition(this IEnumerable<DomInstance> instances, DomDefinitionId definition)
		{
			foreach (var instance in instances)
			{
				if (!instance.DomDefinitionId.Equals(definition))
				{
					continue;
				}

				yield return instance;
			}
		}
	}
}