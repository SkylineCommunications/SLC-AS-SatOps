namespace Generate_Flows_1
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.ManagerStore;

	public abstract class SupportedElement
	{
		protected readonly IDmsElement element;
		protected readonly ICrudHelperComponent<DomInstance> virtualSignalGroupHelper;

		private IEnumerable<Interface> interfaces;

		protected SupportedElement(
			IDmsElement element,
			ICrudHelperComponent<DomInstance> flowHelper,
			ICrudHelperComponent<DomInstance> vsgroupHelper,
			ICrudHelperComponent<DomInstance> levelsHelper)
		{
			this.element = element ?? throw new ArgumentNullException(nameof(element));
		}

		protected SupportedElement(
			IDmsElement element,
			ICrudHelperComponent<DomInstance> virtualSignalGroupHelper)
		{
			this.element = element ?? throw new ArgumentNullException(nameof(element));
			this.virtualSignalGroupHelper = virtualSignalGroupHelper ?? throw new ArgumentNullException(nameof(virtualSignalGroupHelper));
		}

		public abstract IEnumerable<string> Destinations { get; }

		public abstract IEnumerable<string> Sources { get; }

		public abstract bool SupportsVirtualSignalGroupGeneration { get; }

		protected IEnumerable<Interface> Interfaces
		{
			get
			{
				if (interfaces == null)
				{
					interfaces = element.GetExternalInterfaces();
				}

				return interfaces;
			}
		}

		public abstract void GenerateDestinations(IEnumerable<string> selectedDestinationKeys, bool generateVsGroups, DomInstance groupType);

		public abstract void GenerateSources(IEnumerable<string> selectedSourceKeys, bool generateVsGroups, DomInstance groupType);
	}
}