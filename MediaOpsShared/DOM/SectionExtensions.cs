namespace Skyline.DataMiner.Utils.SatOps.Common.DOM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Sections;

	public static class SectionExtensions
	{
		public static T GetValue<T>(this Section section, string name, DomCache cache)
		{
			if (section == null)
			{
				throw new ArgumentNullException(nameof(section));
			}

			if (cache == null)
			{
				throw new ArgumentNullException(nameof(cache));
			}

			var definition = cache.GetSectionDefinitionById(section.SectionDefinitionID.Id);
			var fieldDescriptor = definition.GetFieldDescriptorByName(name);

			var value = section.GetValue<T>(fieldDescriptor.ID);

			return value.GetValue();
		}

		public static T GetValue<T>(this ValueWrapper<T> valueWrapper)
		{
			// valueWrapper is allowed to be null!
			return valueWrapper != null ? valueWrapper.Value : default;
		}

		public static void SetField<T>(this Section section, string name, T value, DomCache cache)
		{
			if (section == null)
			{
				throw new ArgumentNullException(nameof(section));
			}

			if (cache == null)
			{
				throw new ArgumentNullException(nameof(cache));
			}

			var definition = cache.GetSectionDefinitionById(section.SectionDefinitionID.Id);
			var fieldDescriptor = definition.GetFieldDescriptorByName(name);

			section.SetField(fieldDescriptor.ID, value);
		}

		public static void SetField<T>(this Section section, FieldDescriptorID fieldDescriptorId, T value)
		{
			if (section == null)
			{
				throw new ArgumentNullException(nameof(section));
			}

			if (fieldDescriptorId == null)
			{
				throw new ArgumentNullException(nameof(fieldDescriptorId));
			}

			if (!Equals(value, default))
			{
				var fieldValue = new FieldValue(fieldDescriptorId, ValueWrapperFactory.Create(value));
				section.AddOrReplaceFieldValue(fieldValue);
			}
			else
			{
				section.RemoveFieldValueById(fieldDescriptorId);
			}
		}

		public static bool Remove(this ICollection<Section> sections, SectionID sectionID)
		{
			if (sections == null)
			{
				throw new ArgumentNullException(nameof(sections));
			}

			if (sectionID == null)
			{
				throw new ArgumentNullException(nameof(sectionID));
			}

			var section = sections.FirstOrDefault(x => x.ID.Equals(sectionID));

			if (section == null)
			{
				return false;
			}

			return sections.Remove(section);
		}
	}
}
