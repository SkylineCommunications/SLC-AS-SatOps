namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Status;
	using Skyline.DataMiner.Net.Sections;

	public abstract class InstanceBase<T> : IEquatable<InstanceBase<T>>
		where T : InstanceBase<T>
	{
		private readonly ModuleHandlerBase moduleHandler;

		private readonly DomInstance instance;

		private readonly DomDefinitionId domDefinitionId;

		protected InstanceBase(ModuleHandlerBase moduleHandler, DomInstance instance, DomDefinitionId domDefinitionId)
		{
			this.moduleHandler = moduleHandler ?? throw new ArgumentNullException(nameof(moduleHandler));
			this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
			this.domDefinitionId = domDefinitionId ?? throw new ArgumentNullException(nameof(domDefinitionId));

			ParseInstance();
		}

		protected InstanceBase(ModuleHandlerBase moduleHandler, DomDefinitionId domDefinitionId)
		{
			this.moduleHandler = moduleHandler ?? throw new ArgumentNullException(nameof(moduleHandler));
			this.domDefinitionId = domDefinitionId ?? throw new ArgumentNullException(nameof(domDefinitionId));

			instance = new DomInstance
			{
				DomDefinitionId = domDefinitionId,
			};

			IsNew = true;
		}

		protected InstanceBase(DomInstance instance, DomDefinitionId domDefinitionId, bool isNew = false)
		{
			this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
			this.domDefinitionId = domDefinitionId ?? throw new ArgumentNullException(nameof(domDefinitionId));

			IsNew = isNew;

			ParseInstance();
		}

		public Guid InstanceId => instance.ID.Id;

		public DomInstanceId DomInstanceId => instance.ID;

		public string InstanceName => instance.Name;

		public string StatusId => instance.StatusId;

		public bool IsNew { get; protected set; }

		public ModuleHandlerBase ModuleHandler => moduleHandler;

		internal DomInstance Instance => instance;

		protected abstract Dictionary<SectionDefinitionID, Action<T, Section>> SectionMapping { get; }

		public void SetStatusId(string statusId)
		{
			if (string.IsNullOrEmpty(statusId))
			{
				throw new ArgumentNullException(nameof(statusId));
			}

			if (!IsNew)
			{
				throw new InvalidOperationException($"Setting the status is only possible for new instances. Use method DoStatusTransition instead.");
			}

			var statusExists = GetDomStatuses().SingleOrDefault(x => x.Id == statusId) != null;
			if (!statusExists)
			{
				throw new InvalidOperationException($"DOM status with ID {StatusId} does not exist.");
			}

			Instance.StatusId = statusId;
		}

		public void SetInstanceId(Guid instanceId)
		{
			if (instanceId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(instanceId));
			}

			if (!IsNew)
			{
				throw new InvalidOperationException($"Setting the instance ID is only possible for new instances.");
			}

			instance.ID = new DomInstanceId(instanceId);
		}

		public abstract void ApplyChanges();

		public bool HasChanges()
		{
			var clonedInstance = Instance.Clone() as DomInstance;

			ApplyChanges();

			return !Instance.Equals(clonedInstance);
		}

		protected DomInstance CloneInstance()
		{
			var duplicatedInstance = Instance.Clone() as DomInstance;
			duplicatedInstance.ID = new DomInstanceId(Guid.NewGuid());

			duplicatedInstance.Sections.ForEach(x =>
			{
				x.ID = new SectionID(Guid.NewGuid());
			});

			return duplicatedInstance;
		}

		protected IReadOnlyCollection<DomStatus> GetDomStatuses()
		{
			if (moduleHandler == null)
			{
				throw new InvalidOperationException($"Operation is not possible since the class was initialized without the ModuleHandler.");
			}

			return moduleHandler.GetStatusesForDomDefinition(instance.DomDefinitionId);
		}

		private void ParseInstance()
		{
			if (!string.IsNullOrEmpty(instance.ID.ModuleId) && instance.ID.ModuleId != domDefinitionId.ModuleId)
			{
				throw new InvalidOperationException($"Invalid Module ID for instance with ID '{instance.ID.Id}'. Current: {instance.ID.ModuleId} | Expected: {domDefinitionId.ModuleId}");
			}

			if (instance.DomDefinitionId.Id != domDefinitionId.Id)
			{
				throw new InvalidOperationException($"Invalid DOM Definition ID for instance with ID '{instance.ID.Id}'.");
			}

			foreach (var section in instance.Sections)
			{
				if (!SectionMapping.TryGetValue(section.SectionDefinitionID, out var action))
				{
					continue;
				}

				action(this as T, section);
			}
		}

		public override string ToString()
		{
			return $"{InstanceName} [{InstanceId}]";
		}

		#region IEquatable
		public override bool Equals(object obj)
		{
			return Equals(obj as InstanceBase<T>);
		}

		public virtual bool Equals(InstanceBase<T> other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return InstanceId == other.InstanceId;
		}

		public override int GetHashCode()
		{
			return InstanceId.GetHashCode();
		}

		public static bool operator ==(InstanceBase<T> left, InstanceBase<T> right)
		{
			return EqualityComparer<InstanceBase<T>>.Default.Equals(left, right);
		}

		public static bool operator !=(InstanceBase<T> left, InstanceBase<T> right)
		{
			return !(left == right);
		}
		#endregion
	}
}