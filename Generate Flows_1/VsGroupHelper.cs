namespace Generate_Flows_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Sections.Sections;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Common.DOM.Applications.DomIds;

	public static class VsGroupHelper
	{
		public enum FlowColor
		{
			Blue,
			Red,
		}

		public static void AssignFlowToVirtualSignalGroup(DomInstance virtualSignalGroup, DomInstance flow, DomInstance level, FlowColor color)
		{
			var section = virtualSignalGroup.Sections
				.Find(s => s.FieldValues
					.Any(f => f.FieldDescriptorID.Equals(SlcVirtualsignalgroup.Sections.VirtualsignalgroupLinkedflows.FlowLevel) &&
						f.Value.Equals(ValueWrapperFactory.Create(level.ID.Id))));
			if (section == null)
			{
				section = new Section(SlcVirtualsignalgroup.Sections.VirtualsignalgroupLinkedflows.Id);

				var levelFieldValue = new FieldValue(SlcVirtualsignalgroup.Sections.VirtualsignalgroupLinkedflows.FlowLevel, ValueWrapperFactory.Create(level.ID.Id));
				section.AddOrReplaceFieldValue(levelFieldValue);

				virtualSignalGroup.Sections.Add(section);
			}

			if (color == FlowColor.Blue)
			{
				var fieldValue = new FieldValue(SlcVirtualsignalgroup.Sections.VirtualsignalgroupLinkedflows.BlueFlowID, ValueWrapperFactory.Create(flow.ID.Id));
				section.AddOrReplaceFieldValue(fieldValue);
			}

			if (color == FlowColor.Red)
			{
				var fieldValue = new FieldValue(SlcVirtualsignalgroup.Sections.VirtualsignalgroupLinkedflows.RedFlowID, ValueWrapperFactory.Create(flow.ID.Id));
				section.AddOrReplaceFieldValue(fieldValue);
			}
		}
	}
}