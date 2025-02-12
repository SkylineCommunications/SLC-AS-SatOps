﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Dom Editor automation script.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Skyline.DataMiner.Utils.SatOps.Common.DOM.Applications.DomIds
{
	using System;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Sections;

	public static class SlcSatellite_Management
	{
		public const string ModuleId = "(slc)satellite_management";
		public static class Enums
		{
			public static class Orbit
			{
				public const string GEO = "GEO";
				public const string MEO = "MEO";
				public const string LEO = "LEO";
				public static string ToValue(OrbitEnum @enum)
				{
					switch (@enum)
					{
						case OrbitEnum.GEO:
							return GEO;
						case OrbitEnum.MEO:
							return MEO;
						case OrbitEnum.LEO:
							return LEO;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static OrbitEnum ToEnum(string s)
				{
					switch (s)
					{
						case GEO:
							return OrbitEnum.GEO;
						case MEO:
							return OrbitEnum.MEO;
						case LEO:
							return OrbitEnum.LEO;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum OrbitEnum
			{
				GEO,
				MEO,
				LEO
			}

			public static class Hemisphere
			{
				public const string Western = "Western";
				public const string Eastern = "Eastern";
				public static string ToValue(HemisphereEnum @enum)
				{
					switch (@enum)
					{
						case HemisphereEnum.Western:
							return Western;
						case HemisphereEnum.Eastern:
							return Eastern;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static HemisphereEnum ToEnum(string s)
				{
					switch (s)
					{
						case Western:
							return HemisphereEnum.Western;
						case Eastern:
							return HemisphereEnum.Eastern;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum HemisphereEnum
			{
				Western,
				Eastern
			}

			public static class Band
			{
				public const string C = "C";
				public const string K = "K";
				public const string Ka = "Ka";
				public const string Ku = "Ku";
				public const string L = "L";
				public const string X = "X";
				public static string ToValue(BandEnum @enum)
				{
					switch (@enum)
					{
						case BandEnum.C:
							return C;
						case BandEnum.K:
							return K;
						case BandEnum.Ka:
							return Ka;
						case BandEnum.Ku:
							return Ku;
						case BandEnum.L:
							return L;
						case BandEnum.X:
							return X;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static BandEnum ToEnum(string s)
				{
					switch (s)
					{
						case C:
							return BandEnum.C;
						case K:
							return BandEnum.K;
						case Ka:
							return BandEnum.Ka;
						case Ku:
							return BandEnum.Ku;
						case L:
							return BandEnum.L;
						case X:
							return BandEnum.X;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum BandEnum
			{
				C,
				K,
				Ka,
				Ku,
				L,
				X
			}

			public static class Polarization
			{
				public const string Circular = "Circular";
				public const string Horizontal = "Horizontal";
				public const string Vertical = "Vertical";
				public static string ToValue(PolarizationEnum @enum)
				{
					switch (@enum)
					{
						case PolarizationEnum.Circular:
							return Circular;
						case PolarizationEnum.Horizontal:
							return Horizontal;
						case PolarizationEnum.Vertical:
							return Vertical;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static PolarizationEnum ToEnum(string s)
				{
					switch (s)
					{
						case Circular:
							return PolarizationEnum.Circular;
						case Horizontal:
							return PolarizationEnum.Horizontal;
						case Vertical:
							return PolarizationEnum.Vertical;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum PolarizationEnum
			{
				Circular,
				Horizontal,
				Vertical
			}

			public static class Linktype
			{
				public const string Feeder = "Feeder";
				public const string User = "User";
				public const string Uplink = "Uplink";
				public const string Downlink = "Downlink";
				public static string ToValue(LinkTypeEnum @enum)
				{
					switch (@enum)
					{
						case LinkTypeEnum.Feeder:
							return Feeder;
						case LinkTypeEnum.User:
							return User;
						case LinkTypeEnum.Uplink:
							return Uplink;
						case LinkTypeEnum.Downlink:
							return Downlink;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static LinkTypeEnum ToEnum(string s)
				{
					switch (s)
					{
						case Feeder:
							return LinkTypeEnum.Feeder;
						case User:
							return LinkTypeEnum.User;
						case Uplink:
							return LinkTypeEnum.Uplink;
						case Downlink:
							return LinkTypeEnum.Downlink;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum LinkTypeEnum
			{
				Feeder,
				User,
				Uplink,
				Downlink
			}

			public static class Transmissiontype
			{
				public const string RX = "RX";
				public const string TX = "TX";
				public const string CarrierInCarrier = "Carrier in Carrier";
				public static string ToValue(TransmissionTypeEnum @enum)
				{
					switch (@enum)
					{
						case TransmissionTypeEnum.RX:
							return RX;
						case TransmissionTypeEnum.TX:
							return TX;
						case TransmissionTypeEnum.CarrierInCarrier:
							return CarrierInCarrier;
						default:
							throw new ArgumentOutOfRangeException(nameof(@enum), @enum, "Invalid value.");
					}
				}

				public static TransmissionTypeEnum ToEnum(string s)
				{
					switch (s)
					{
						case RX:
							return TransmissionTypeEnum.RX;
						case TX:
							return TransmissionTypeEnum.TX;
						case CarrierInCarrier:
							return TransmissionTypeEnum.CarrierInCarrier;
						default:
							throw new ArgumentOutOfRangeException(nameof(s), s, "Invalid value.");
					}
				}
			}

			public enum TransmissionTypeEnum
			{
				RX,
				TX,
				CarrierInCarrier
			}
		}

		public static class Sections
		{
			public static class TransponderPlan
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("8bd0f88c-9989-4172-b678-426164b3f03b"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID PlanName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("cd10689f-2f1c-4f22-95bb-c5c185ce1531"));
				public static FieldDescriptorID AppliedTransponderIds
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e4c64cfb-7d62-4a5c-b9b8-3e2971f6546d"));
			}

			public static class SlotDefinition
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("a881e6a8-676d-4890-8278-4a1ef59030d5"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID DefinitionSlotName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e10f4323-10f5-4eca-bfd0-41302cbe955f"));
				public static FieldDescriptorID DefinitionSlotSize
				{
					get;
				}

				= new FieldDescriptorID(new Guid("f980e442-0cbf-4eb7-b64e-3731c615e0c6"));
				public static FieldDescriptorID RelativeStartFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("f1996c66-6722-4e49-820c-609a1b22e3f3"));
				public static FieldDescriptorID RelativeEndFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("cc063899-9c82-4fb1-b536-b7e5df441edf"));
			}

			public static class LaunchInformation
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("e0d7e42e-79d7-42a0-bc17-2c8882d7ef2c"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID LaunchInfo
				{
					get;
				}

				= new FieldDescriptorID(new Guid("13ee5df8-b947-4d9d-8b17-ea51c0cc4d5c"));
				public static FieldDescriptorID LaunchInServiceDate
				{
					get;
				}

				= new FieldDescriptorID(new Guid("a7bcf0d1-8d54-4564-9e94-539a28a8f058"));
			}

			public static class General
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("a5b9f131-5e22-4d9c-8f4d-6c30746e3831"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID SatelliteName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e1a2592b-189f-4e14-b6fb-d3f12896cab7"));
				public static FieldDescriptorID SatelliteAbbreviation
				{
					get;
				}

				= new FieldDescriptorID(new Guid("2e00f744-1222-4313-adae-856f674c7163"));
				public static FieldDescriptorID Orbit
				{
					get;
				}

				= new FieldDescriptorID(new Guid("c7b90a65-62a5-418f-b14c-68d346a858f0"));
				public static FieldDescriptorID Hemisphere
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e0d7e42e-79d7-42a0-bc17-2c8882d7ef2c"));
				public static FieldDescriptorID LongitudeForGEODegrees
				{
					get;
				}

				= new FieldDescriptorID(new Guid("f8b3eaf6-18b6-439b-8bf7-32a3ef5f8d60"));
				public static FieldDescriptorID InclinationDegrees
				{
					get;
				}

				= new FieldDescriptorID(new Guid("8bd0f88c-9989-4172-b678-426164b3f03b"));
			}

			public static class Satellite
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("7d94a3a2-9e13-4a5d-a8b5-8e8c8b7bf937"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID Operator
				{
					get;
				}

				= new FieldDescriptorID(new Guid("a881e6a8-676d-4890-8278-4a1ef59030d5"));
				public static FieldDescriptorID Coverage
				{
					get;
				}

				= new FieldDescriptorID(new Guid("b9e99e72-3fe2-4eaf-b34d-7ff3f36db30e"));
				public static FieldDescriptorID Applications
				{
					get;
				}

				= new FieldDescriptorID(new Guid("52ea8c65-86aa-4a97-a416-666ba7b7c03d"));
				public static FieldDescriptorID Info
				{
					get;
				}

				= new FieldDescriptorID(new Guid("d71d8fe7-1b6a-4a26-ba2d-ae9a36323272"));
			}

			public static class Transponder
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("f8b3eaf6-18b6-439b-8bf7-32a3ef5f8d60"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID TransponderSatellite
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e6158bd0-c221-4edc-ba93-47ca983e986a"));
				public static FieldDescriptorID Beam
				{
					get;
				}

				= new FieldDescriptorID(new Guid("d485aa89-6704-4eb0-be6f-6e37d2531140"));
				public static FieldDescriptorID TransponderName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("eb69bb13-647b-49e4-b0b9-656a89a48f44"));
				public static FieldDescriptorID Band
				{
					get;
				}

				= new FieldDescriptorID(new Guid("4d97be0e-ec99-4c86-ba9b-c63abfa7c197"));
				public static FieldDescriptorID Bandwidth
				{
					get;
				}

				= new FieldDescriptorID(new Guid("d8b42b9d-646c-4a4d-aa1e-d06d1d2a98d1"));
				public static FieldDescriptorID StartFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("08e2d07d-ee93-4f5b-b3da-88dd9ba4a7fb"));
				public static FieldDescriptorID StopFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("c7a3765d-0501-4ab7-b768-23ec26dd59d2"));
				public static FieldDescriptorID Polarization
				{
					get;
				}

				= new FieldDescriptorID(new Guid("c0dbb8bf-ee2e-4e9a-839a-b5e9af926f2c"));
			}

			public static class Slot
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("aa5f2bd0-62cd-4b1b-8fcd-c60a28ef0c83"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID Transponder
				{
					get;
				}

				= new FieldDescriptorID(new Guid("d4104d16-45e6-4484-a102-f85862d6a52a"));
				public static FieldDescriptorID TransponderPlan
				{
					get;
				}

				= new FieldDescriptorID(new Guid("7182bf5a-6a93-48a6-ba0e-64b83c0e985f"));
				public static FieldDescriptorID SlotName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("3e54fbc3-b659-4d7d-aa57-67b77c9b31c2"));
				public static FieldDescriptorID SlotSize
				{
					get;
				}

				= new FieldDescriptorID(new Guid("832d7d0a-91a3-47a3-84b5-b9673c409b77"));
				public static FieldDescriptorID CenterFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("352f45ec-3183-4f76-a774-7d7d546f38f1"));
				public static FieldDescriptorID SlotStartFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("743e4205-0891-4d56-a704-0ca24220ef86"));
				public static FieldDescriptorID SlotEndFrequency
				{
					get;
				}

				= new FieldDescriptorID(new Guid("c44c1d35-3713-4d8b-8068-d8d85719f93b"));
				public static FieldDescriptorID Resource
				{
					get;
				}

				= new FieldDescriptorID(new Guid("e8a8c5cc-0a24-4d10-8a7d-4392baa32d5c"));
			}

			public static class Origin
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("c7b90a65-62a5-418f-b14c-68d346a858f0"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID Manufacturer
				{
					get;
				}

				= new FieldDescriptorID(new Guid("d8fb3e42-e68c-4297-84a7-3b31c20e4aef"));
				public static FieldDescriptorID Country
				{
					get;
				}

				= new FieldDescriptorID(new Guid("b8b1a9b1-5d1f-4f3f-bd85-543b21e42bf9"));
			}

			public static class Beam
			{
				public static SectionDefinitionID Id
				{
					get;
				}

				= new SectionDefinitionID(new Guid("2fdcd496-83f2-413c-8e66-634769d2d964"))
				{ ModuleId = "(slc)satellite_management" };
				public static FieldDescriptorID BeamName
				{
					get;
				}

				= new FieldDescriptorID(new Guid("3c8f6ec2-6254-48aa-8ec2-d4df46f8cf43"));
				public static FieldDescriptorID BeamSatellite
				{
					get;
				}

				= new FieldDescriptorID(new Guid("a5b9f131-5e22-4d9c-8f4d-6c30746e3831"));
				public static FieldDescriptorID LinkType
				{
					get;
				}

				= new FieldDescriptorID(new Guid("0b1a9b5f-99b0-4212-9334-97dce10e8d94"));
				public static FieldDescriptorID TransmissionType
				{
					get;
				}

				= new FieldDescriptorID(new Guid("3cc7ed32-11c1-41e7-8a1a-32c881312a6d"));
				public static FieldDescriptorID FootprintFile
				{
					get;
				}

				= new FieldDescriptorID(new Guid("22608fe9-fca6-4f98-92b2-9c852dd0d575"));
			}
		}

		public static class Definitions
		{
			public static DomDefinitionId Slots
			{
				get;
			}

			= new DomDefinitionId(new Guid("77d1c352-46c4-4fc0-8f37-7622ff5d3d69"))
			{ ModuleId = "(slc)satellite_management" };
			public static DomDefinitionId TransponderPlans
			{
				get;
			}

			= new DomDefinitionId(new Guid("15c8b52b-5e24-45f2-8a5a-6e04e4095860"))
			{ ModuleId = "(slc)satellite_management" };
			public static DomDefinitionId Beams
			{
				get;
			}

			= new DomDefinitionId(new Guid("5bf282f1-85c7-487d-8bc3-d7ea13939889"))
			{ ModuleId = "(slc)satellite_management" };
			public static DomDefinitionId Transponders
			{
				get;
			}

			= new DomDefinitionId(new Guid("a06d7c77-7d5d-4d02-86e3-94b79a0a1549"))
			{ ModuleId = "(slc)satellite_management" };
			public static DomDefinitionId Satellites
			{
				get;
			}

			= new DomDefinitionId(new Guid("d0964c8f-5d90-4a2a-854e-b69d6f70a97a"))
			{ ModuleId = "(slc)satellite_management" };
		}

		public static class Behaviors
		{
			public static class TranspondersBehavior
			{
				public static DomBehaviorDefinitionId Id
				{
					get;
				}

				= new DomBehaviorDefinitionId(new Guid("ccc94112-1323-456d-bf03-68d572a7e649"))
				{ ModuleId = "(slc)satellite_management" };
				public static class Statuses
				{
					public const string Draft = "draft";
					public const string Active = "active";
					public const string Deprecated = "deprecated";
					public const string Edit = "edit";
				}

				public static class Transitions
				{
					public const string Draft_To_Active = "draft_to_active";
					public const string Active_To_Deprecated = "active_to_deprecated";
					public const string Active_To_Edit = "active_to_edit";
					public const string Edit_To_Active = "edit_to_active";
					public const string Deprecated_To_Active = "deprecated_to_active";
					public const string Draft_To_Edit = "draft_to_edit";
					public const string Edit_To_Deprecated = "edit_to_deprecated";
					public const string Draft_To_Deprecated = "draft_to_deprecated";
				}
			}

			public static class SatellitesBehavior
			{
				public static DomBehaviorDefinitionId Id
				{
					get;
				}

				= new DomBehaviorDefinitionId(new Guid("1343621c-271c-4246-b038-028b539b93d9"))
				{ ModuleId = "(slc)satellite_management" };
				public static class Statuses
				{
					public const string Draft = "draft";
					public const string Active = "active";
					public const string Deprecated = "deprecated";
					public const string Edit = "edit";
				}

				public static class Transitions
				{
					public const string Draft_To_Active = "draft_to_active";
					public const string Active_To_Deprecated = "active_to_deprecated";
					public const string Active_To_Edit = "active_to_edit";
					public const string Edit_To_Active = "edit_to_active";
					public const string Deprecated_To_Active = "deprecated_to_active";
					public const string Draft_To_Deprecated = "draft_to_deprecated";
					public const string Edit_To_Deprecated = "edit_to_deprecated";
					public const string Draft_To_Edit = "draft_to_edit";
				}
			}

			public static class TransponderPlansBehavior
			{
				public static DomBehaviorDefinitionId Id
				{
					get;
				}

				= new DomBehaviorDefinitionId(new Guid("c70bae27-f4e0-41dc-bb02-c8aaf0492f3a"))
				{ ModuleId = "(slc)satellite_management" };
				public static class Statuses
				{
					public const string Draft = "draft";
					public const string Active = "active";
					public const string Deprecated = "deprecated";
					public const string Edit = "edit";
				}

				public static class Transitions
				{
					public const string Draft_To_Active = "draft_to_active";
					public const string Active_To_Deprecated = "active_to_deprecated";
					public const string Active_To_Edit = "active_to_edit";
					public const string Edit_To_Active = "edit_to_active";
					public const string Deprecated_To_Active = "deprecated_to_active";
					public const string Draft_To_Edit = "draft_to_edit";
					public const string Draft_To_Deprecated = "draft_to_deprecated";
					public const string Edit_To_Deprecated = "edit_to_deprecated";
				}
			}

			public static class SlotsBehavior
			{
				public static DomBehaviorDefinitionId Id
				{
					get;
				}

				= new DomBehaviorDefinitionId(new Guid("14d6e47b-ac91-445c-8573-66a06af21622"))
				{ ModuleId = "(slc)satellite_management" };
				public static class Statuses
				{
					public const string Draft = "draft";
					public const string Active = "active";
					public const string Deprecated = "deprecated";
					public const string Edit = "edit";
				}

				public static class Transitions
				{
					public const string Draft_To_Active = "draft_to_active";
					public const string Active_To_Deprecated = "active_to_deprecated";
					public const string Active_To_Edit = "active_to_edit";
					public const string Edit_To_Active = "edit_to_active";
					public const string Deprecated_To_Active = "deprecated_to_active";
					public const string Draft_To_Edit = "draft_to_edit";
					public const string Draft_To_Deprecated = "draft_to_deprecated";
					public const string Edit_To_Deprecated = "edit_to_deprecated";
				}
			}

			public static class BeamsBehavior
			{
				public static DomBehaviorDefinitionId Id
				{
					get;
				}

				= new DomBehaviorDefinitionId(new Guid("d58e07ef-45b4-4c5d-b5cd-b0a16ff41e68"))
				{ ModuleId = "(slc)satellite_management" };
				public static class Statuses
				{
					public const string Draft = "draft";
					public const string Active = "active";
					public const string Deprecated = "deprecated";
					public const string Edit = "edit";
				}

				public static class Transitions
				{
					public const string Draft_To_Active = "draft_to_active";
					public const string Active_To_Deprecated = "active_to_deprecated";
					public const string Edit_To_Active = "edit_to_active";
					public const string Deprecated_To_Active = "deprecated_to_active";
					public const string Draft_To_Edit = "draft_to_edit";
					public const string Draft_To_Deprecated = "draft_to_deprecated";
					public const string Edit_To_Deprecated = "edit_to_deprecated";
					public const string Active_To_Edit = "active_to_edit";
				}
			}
		}
	}
}