
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{

    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    [DataContract]
    public sealed class NbtObjectClass : IEquatable<NbtObjectClass>, IComparable<NbtObjectClass>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { AliquotClass                      , AliquotClass                       },
            { BatchOpClass                      , BatchOpClass                       },
            { BiologicalClass                   , BiologicalClass                    },
            { MaterialComponentClass            , MaterialComponentClass             },
            { CofAMethodClass                   , CofAMethodClass                    },
            { CofAMethodTemplateClass           , CofAMethodTemplateClass            },
            { ContainerClass                    , ContainerClass                     },
            { ContainerLocationClass            , ContainerLocationClass             },
            { ContainerDispenseTransactionClass , ContainerDispenseTransactionClass  },
            { ContainerGroupClass               , ContainerGroupClass                },
            { CustomerClass                     , CustomerClass                      },
            { DocumentClass                     , DocumentClass                      },
            { EnterprisePartClass               , EnterprisePartClass                },
            { EquipmentAssemblyClass            , EquipmentAssemblyClass             },
            { EquipmentClass                    , EquipmentClass                     },
            { EquipmentTypeClass                , EquipmentTypeClass                 },
            { FeedbackClass                     , FeedbackClass                      },
            { GeneratorClass                    , GeneratorClass                     },
            { GenericClass                      , GenericClass                       },
            { InspectionDesignClass             , InspectionDesignClass              },
            { InspectionRouteClass              , InspectionRouteClass               },
            { InventoryGroupClass               , InventoryGroupClass                },
            { InventoryGroupPermissionClass     , InventoryGroupPermissionClass      },
            { InventoryLevelClass               , InventoryLevelClass                },
            { JurisdictionClass                 , JurisdictionClass                  },
            { LocationClass                     , LocationClass                      },
            { MailReportClass                   , MailReportClass                    },
            { ManufacturerEquivalentPartClass   , ManufacturerEquivalentPartClass    },
            { MaterialClass                     , MaterialClass                      },
            { MaterialSynonymClass              , MaterialSynonymClass               },
            { MethodClass                       , MethodClass                        },
            { InspectionTargetClass             , InspectionTargetClass              },
            { InspectionTargetGroupClass        , InspectionTargetGroupClass         },
            { NotificationClass                 , NotificationClass                  },
            { ParameterClass                    , ParameterClass                     },
            { PrintLabelClass                   , PrintLabelClass                    },
            { ProblemClass                      , ProblemClass                       },
            { ReceiptLotClass                   , ReceiptLotClass                    },
            { RegulatoryListClass               , RegulatoryListClass                },
            { ReportClass                       , ReportClass                        },
            { ResultClass                       , ResultClass                        },
            { RequestClass                      , RequestClass                       },
            { RequestContainerDispenseClass     , RequestContainerDispenseClass      },
            { RequestContainerUpdateClass       , RequestContainerUpdateClass        },
            { RequestMaterialCreateClass        , RequestMaterialCreateClass         },
            { RequestMaterialDispenseClass      , RequestMaterialDispenseClass       },
            { RoleClass                         , RoleClass                          },
            { SampleClass                       , SampleClass                        },
            { SizeClass                         , SizeClass                          },
            { TaskClass                         , TaskClass                          },
            { TestClass                         , TestClass                          },
            { UnitOfMeasureClass                , UnitOfMeasureClass                 },
            { UserClass                         , UserClass                          },
            { VendorClass                       , VendorClass                        },
            {  WorkUnitClass                    , WorkUnitClass                      }
        };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public NbtObjectClass( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator NbtObjectClass( string Val )
        {
            return new NbtObjectClass( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( NbtObjectClass item )
        {
            return item.Value;
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion Internals

        #region Enum members

        [DataMember]
        public const string AliquotClass = "AliquotClass";
        [DataMember]
        public const string BatchOpClass = "BatchOpClass";
        [DataMember]
        public const string BiologicalClass = "BiologicalClass";
        [DataMember]
        public const string CofAMethodClass = "CofAMethodClass";
        [DataMember]
        public const string CofAMethodTemplateClass = "CofAMethodTemplateClass";
        [DataMember]
        public const string ContainerClass = "ContainerClass";
        [DataMember]
        public const string ContainerLocationClass = "ContainerLocationClass";
        [DataMember]
        public const string ContainerDispenseTransactionClass = "ContainerDispenseTransactionClass";
        [DataMember]
        public const string ContainerGroupClass = "ContainerGroupClass";
        [DataMember]
        public const string CustomerClass = "CustomerClass";
        [DataMember]
        public const string DocumentClass = "DocumentClass";
        [DataMember]
        public const string EnterprisePartClass = "EnterprisePartClass";
        [DataMember]
        public const string EquipmentAssemblyClass = "EquipmentAssemblyClass";
        [DataMember]
        public const string EquipmentClass = "EquipmentClass";
        [DataMember]
        public const string EquipmentTypeClass = "EquipmentTypeClass";
        [DataMember]
        public const string FeedbackClass = "FeedbackClass";
        [DataMember]
        public const string GeneratorClass = "GeneratorClass";
        [DataMember]
        public const string GenericClass = "GenericClass";
        [DataMember]
        public const string InspectionDesignClass = "InspectionDesignClass";
        [DataMember]
        public const string InspectionRouteClass = "InspectionRouteClass";
        [DataMember]
        public const string InventoryGroupClass = "InventoryGroupClass";
        [DataMember]
        public const string InventoryGroupPermissionClass = "InventoryGroupPermissionClass";
        [DataMember]
        public const string InventoryLevelClass = "InventoryLevelClass";
        [DataMember]
        public const string JurisdictionClass = "JurisdictionClass";
        [DataMember]
        public const string LocationClass = "LocationClass";
        [DataMember]
        public const string MailReportClass = "MailReportClass";
        [DataMember]
        public const string MaterialClass = "MaterialClass";
        [DataMember]
        public const string ManufacturerEquivalentPartClass = "ManufacturerEquivalentPartClass";
        [DataMember]
        public const string MaterialComponentClass = "MaterialComponentClass";
        [DataMember]
        public const string MaterialSynonymClass = "MaterialSynonymClass";
        [DataMember]
        public const string MethodClass = "MethodClass";
        [DataMember]
        public const string InspectionTargetClass = "InspectionTargetClass";
        [DataMember]
        public const string InspectionTargetGroupClass = "InspectionTargetGroupClass";
        [DataMember]
        public const string NotificationClass = "NotificationClass";
        [DataMember]
        public const string ParameterClass = "ParameterClass";
        [DataMember]
        public const string PrintLabelClass = "PrintLabelClass";
        [DataMember]
        public const string ProblemClass = "ProblemClass";
        [DataMember]
        public const string ReceiptLotClass = "ReceiptLotClass";
        [DataMember]
        public const string RegulatoryListClass = "RegulatoryListClass";
        [DataMember]
        public const string ReportClass = "ReportClass";
        [DataMember]
        public const string ResultClass = "ResultClass";
        [DataMember]
        public const string RequestClass = "RequestClass";
        [DataMember]
        public const string RequestContainerDispenseClass = "RequestContainerDispenseClass";
        [DataMember]
        public const string RequestContainerUpdateClass = "RequestContainerUpdateClass";
        [DataMember]
        public const string RequestMaterialDispenseClass = "RequestMaterialDispenseClass";
        [DataMember]
        public const string RequestMaterialCreateClass = "RequestMaterialCreateClass";
        [DataMember]
        public const string RoleClass = "RoleClass";
        [DataMember]
        public const string SampleClass = "SampleClass";
        [DataMember]
        public const string SizeClass = "SizeClass";
        [DataMember]
        public const string TaskClass = "TaskClass";
        [DataMember]
        public const string TestClass = "TestClass";
        [DataMember]
        public const string UnitOfMeasureClass = "UnitOfMeasureClass";
        [DataMember]
        public const string UserClass = "UserClass";
        [DataMember]
        public const string VendorClass = "VendorClass";
        [DataMember]
        public const string WorkUnitClass = "WorkUnitClass";

        #endregion Enum members


        #region IComparable

        public int CompareTo( NbtObjectClass other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (NbtObjectClass)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( NbtObjectClass ft1, NbtObjectClass ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( NbtObjectClass ft1, NbtObjectClass ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is NbtObjectClass ) )
            {
                return false;
            }
            return this == (NbtObjectClass) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( NbtObjectClass obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (NbtObjectClass)

    };
}
