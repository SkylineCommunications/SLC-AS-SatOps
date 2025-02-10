namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Sections;

	public abstract class SectionBase<T> : IEquatable<SectionBase<T>>
		where T : SectionBase<T>
	{
		private readonly Section section;

		protected SectionBase(Section section)
		{
			this.section = section ?? throw new ArgumentNullException(nameof(section));

			ParseSection();
		}

		protected SectionBase(SectionDefinitionID sectionDefinitionId)
		{
			section = new Section
			{
				SectionDefinitionID = sectionDefinitionId,
			};

			IsNew = true;
		}

		public Guid SectionId => section.ID.Id;

		public bool IsNew { get; private set; }

		internal Section Section => section;

		protected abstract Dictionary<FieldDescriptorID, Action<T, object>> FieldMapping { get; }

		internal abstract void ApplyChanges();

		private void ParseSection()
		{
			foreach (var fieldValue in section.FieldValues)
			{
				if (!FieldMapping.TryGetValue(fieldValue.FieldDescriptorID, out var action))
				{
					continue;
				}

				action(this as T, fieldValue.Value.Value);
			}
		}

		public override string ToString()
		{
			return section.ToString();
		}

		#region IEquatable

		public override bool Equals(object obj)
		{
			return Equals(obj as SectionBase<T>);
		}

		public virtual bool Equals(SectionBase<T> other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return SectionId == other.SectionId;
		}

		public override int GetHashCode()
		{
			return SectionId.GetHashCode();
		}

		public static bool operator ==(SectionBase<T> left, SectionBase<T> right)
		{
			return EqualityComparer<SectionBase<T>>.Default.Equals(left, right);
		}

		public static bool operator !=(SectionBase<T> left, SectionBase<T> right)
		{
			return !(left == right);
		}

		#endregion
	}
}