namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.SatelliteManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public class SatelliteManagementHandler : ModuleHandlerBase
	{
		public SatelliteManagementHandler(IEngine engine) : base(engine, DomIds.SlcSatellite_Management.ModuleId)
		{
		}

		public SatelliteManagementHandler(DomHelper domHelper) : base(domHelper, DomIds.SlcSatellite_Management.ModuleId)
		{
		}

		public Slot GetSlotByDomInstanceId(Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);

			return GetSlotsIterator(filter).SingleOrDefault();
		}

		public IEnumerable<Slot> GetSlots(FilterElement<DomInstance> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return GetSlotsIterator(filter);
		}

		public Slot CreateSlot(Slot slot)
		{
			if (slot == null)
			{
				throw new ArgumentNullException(nameof(slot));
			}

			slot.ApplyChanges();
			var createdInstance = DomHelper.DomInstances.Create(slot.Instance);

			return new Slot(this, createdInstance);
		}

		public Slot UpdateSlot(Slot slot)
		{
			if (slot == null)
			{
				throw new ArgumentNullException(nameof(slot));
			}

			slot.ApplyChanges();
			var updatedInstance = DomHelper.DomInstances.Update(slot.Instance);

			return new Slot(this, updatedInstance);
		}

		public TransponderPlan GetTransponderPlanByDomInstanceId(Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);

			return GetTransponderPlansIterator(filter).SingleOrDefault();
		}

		public IEnumerable<TransponderPlan> GetTransponderPlans(FilterElement<DomInstance> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return GetTransponderPlansIterator(filter);
		}

		public IEnumerable<TransponderPlan> GetAllTransponderPlans()
		{
			var filter = DomInstanceExposers.DomDefinitionId.Equal(DomIds.SlcSatellite_Management.Definitions.TransponderPlans.Id);

			return GetTransponderPlansIterator(filter);
		}

		public TransponderPlan UpdateTransponderPlan(TransponderPlan transponderPlan)
		{
			if (transponderPlan == null)
			{
				throw new ArgumentNullException(nameof(transponderPlan));
			}

			transponderPlan.ApplyChanges();
			var updatedInstance = DomHelper.DomInstances.Update(transponderPlan.Instance);

			return new TransponderPlan(this, updatedInstance);
		}

		public Beam GetBeamByDomInstanceId(Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);

			return GetBeamsIterator(filter).SingleOrDefault();
		}

		public IEnumerable<Beam> GetBeams(FilterElement<DomInstance> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return GetBeamsIterator(filter);
		}

		public Transponder GetTransponderByDomInstanceId(Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);

			return GetTranspondersIterator(filter).SingleOrDefault();
		}

		public IEnumerable<Transponder> GetTransponders(FilterElement<DomInstance> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return GetTranspondersIterator(filter);
		}

		public Satellite GetSatelliteByDomInstanceId(Guid id)
		{
			var filter = DomInstanceExposers.Id.Equal(id);

			return GetSatellitesIterator(filter).SingleOrDefault();
		}

		public IEnumerable<Satellite> GetSatellites(FilterElement<DomInstance> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			return GetSatellitesIterator(filter);
		}

		private IEnumerable<Slot> GetSlotsIterator(FilterElement<DomInstance> filter)
		{
			foreach (var instance in DomHelper.DomInstances.Read(filter))
			{
				if (instance == null || instance.DomDefinitionId.Id != DomIds.SlcSatellite_Management.Definitions.Slots.Id)
				{
					continue;
				}

				yield return new Slot(this, instance);
			}
		}

		private IEnumerable<TransponderPlan> GetTransponderPlansIterator(FilterElement<DomInstance> filter)
		{
			foreach (var instance in DomHelper.DomInstances.Read(filter))
			{
				if (instance == null || instance.DomDefinitionId.Id != DomIds.SlcSatellite_Management.Definitions.TransponderPlans.Id)
				{
					continue;
				}

				yield return new TransponderPlan(this, instance);
			}
		}

		private IEnumerable<Beam> GetBeamsIterator(FilterElement<DomInstance> filter)
		{
			foreach (var instance in DomHelper.DomInstances.Read(filter))
			{
				if (instance == null || instance.DomDefinitionId.Id != DomIds.SlcSatellite_Management.Definitions.Beams.Id)
				{
					continue;
				}

				yield return new Beam(this, instance);
			}
		}

		private IEnumerable<Transponder> GetTranspondersIterator(FilterElement<DomInstance> filter)
		{
			foreach (var instance in DomHelper.DomInstances.Read(filter))
			{
				if (instance == null || instance.DomDefinitionId.Id != DomIds.SlcSatellite_Management.Definitions.Transponders.Id)
				{
					continue;
				}

				yield return new Transponder(this, instance);
			}
		}

		private IEnumerable<Satellite> GetSatellitesIterator(FilterElement<DomInstance> filter)
		{
			foreach (var instance in DomHelper.DomInstances.Read(filter))
			{
				if (instance == null || instance.DomDefinitionId.Id != DomIds.SlcSatellite_Management.Definitions.Satellites.Id)
				{
					continue;
				}

				yield return new Satellite(this, instance);
			}
		}
	}
}
