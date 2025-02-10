namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Status;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.SatOps.Common.Extensions;

	public class ModuleHandlerBase
	{
		private readonly Dictionary<Guid, DomDefinition> domDefinitionsById = new Dictionary<Guid, DomDefinition>();

		private readonly Dictionary<Guid, DomBehaviorDefinition> domBehaviorDefinitionsById = new Dictionary<Guid, DomBehaviorDefinition>();

		protected ModuleHandlerBase(IEngine engine, string moduleId)
		{
			if (engine == null)
			{
				throw new ArgumentNullException(nameof(engine));
			}

			DomHelper = new DomHelper(engine.SendSLNetMessages, moduleId);
		}

		protected ModuleHandlerBase(IConnection connection, string moduleId)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			DomHelper = new DomHelper(connection.HandleMessages, moduleId);
		}

		protected ModuleHandlerBase(DomHelper domHelper, string moduleId)
		{
			if (domHelper == null)
			{
				throw new ArgumentNullException(nameof(domHelper));
			}

			if (domHelper.ModuleId != moduleId)
			{
				throw new ArgumentException($"DomHelper with module ID '{domHelper.ModuleId}' is provided while module ID '{moduleId}' is expected");
			}

			DomHelper = domHelper;
		}

		public DomHelper DomHelper { get; }

		public void BulkCreateOrUpdateInstances<T>(ICollection<T> instances) where T : InstanceBase<T>
		{
			if (instances == null)
			{
				throw new ArgumentNullException(nameof(instances));
			}

			foreach (var instance in instances)
			{
				instance.ApplyChanges();
			}

			foreach (var x in instances.Batch(100))
			{
				DomHelper.DomInstances.CreateOrUpdate(x.Select(y => y.Instance).ToList()).ThrowOnFailure();
			}
		}

		public void BulkDeleteInstances<T>(ICollection<T> instances) where T : InstanceBase<T>
		{
			if (instances == null)
			{
				throw new ArgumentNullException(nameof(instances));
			}

			foreach (var x in instances.Batch(100))
			{
				DomHelper.DomInstances.Delete(x.Select(y => y.Instance).ToList()).ThrowOnFailure();
			}
		}

		internal IReadOnlyCollection<DomStatus> GetStatusesForDomDefinition(DomDefinitionId domDefinitionId)
		{
			if (domDefinitionId == null)
			{
				throw new ArgumentNullException(nameof(domDefinitionId));
			}

			var domDefinition = GetDomDefinition(domDefinitionId.Id);
			var domBehaviorDefinition = GetDomBehaviorDefinition(domDefinition.DomBehaviorDefinitionId.Id);

			return domBehaviorDefinition.Statuses;
		}

		private DomDefinition GetDomDefinition(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}

			if (domDefinitionsById.TryGetValue(id, out var domDefinition))
			{
				return domDefinition;
			}

			domDefinition = DomHelper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(id)).SingleOrDefault();
			if (domDefinition == null)
			{
				throw new InvalidOperationException($"Module {DomHelper.ModuleId} does not contain a DOM Definition with ID {id}.");
			}

			domDefinitionsById.Add(id, domDefinition);

			return domDefinition;
		}

		private DomBehaviorDefinition GetDomBehaviorDefinition(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}

			if (domBehaviorDefinitionsById.TryGetValue(id, out var domBehaviorDefinition))
			{
				return domBehaviorDefinition;
			}

			domBehaviorDefinition = DomHelper.DomBehaviorDefinitions.Read(DomBehaviorDefinitionExposers.Id.Equal(id)).SingleOrDefault();
			if (domBehaviorDefinition == null)
			{
				throw new InvalidOperationException($"Module {DomHelper.ModuleId} does not contain a DOM Behavior Definition with ID {id}.");
			}

			domBehaviorDefinitionsById.Add(id, domBehaviorDefinition);

			return domBehaviorDefinition;
		}
	}
}
