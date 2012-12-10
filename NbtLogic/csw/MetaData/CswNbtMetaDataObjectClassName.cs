
using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
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
            { FireClassExemptAmountClass        , FireClassExemptAmountClass         },
            { FireClassExemptAmountSetClass     , FireClassExemptAmountSetClass      },
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
            return ( null != item ) ? item.Value : CswNbtResources.UnknownEnum;
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

        public const string AliquotClass = "AliquotClass";
        public const string BatchOpClass = "BatchOpClass";
        public const string BiologicalClass = "BiologicalClass";
        public const string CofAMethodClass = "CofAMethodClass";
        public const string CofAMethodTemplateClass = "CofAMethodTemplateClass";
        public const string ContainerClass = "ContainerClass";
        public const string ContainerLocationClass = "ContainerLocationClass";
        public const string ContainerDispenseTransactionClass = "ContainerDispenseTransactionClass";
        public const string ContainerGroupClass = "ContainerGroupClass";
        public const string CustomerClass = "CustomerClass";
        public const string DocumentClass = "DocumentClass";
        public const string EnterprisePartClass = "EnterprisePartClass";
        public const string EquipmentAssemblyClass = "EquipmentAssemblyClass";
        public const string EquipmentClass = "EquipmentClass";
        public const string EquipmentTypeClass = "EquipmentTypeClass";
        public const string FeedbackClass = "FeedbackClass";
        public const string FireClassExemptAmountClass = "FireClassExemptAmountClass";
        public const string FireClassExemptAmountSetClass = "FireClassExemptAmountSetClass";
        public const string GeneratorClass = "GeneratorClass";
        public const string GenericClass = "GenericClass";
        public const string InspectionDesignClass = "InspectionDesignClass";
        public const string InspectionRouteClass = "InspectionRouteClass";
        public const string InventoryGroupClass = "InventoryGroupClass";
        public const string InventoryGroupPermissionClass = "InventoryGroupPermissionClass";
        public const string InventoryLevelClass = "InventoryLevelClass";
        public const string JurisdictionClass = "JurisdictionClass";
        public const string LocationClass = "LocationClass";
        public const string MailReportClass = "MailReportClass";
        public const string MaterialClass = "MaterialClass";
        public const string ManufacturerEquivalentPartClass = "ManufacturerEquivalentPartClass";
        public const string MaterialComponentClass = "MaterialComponentClass";
        public const string MaterialSynonymClass = "MaterialSynonymClass";
        public const string MethodClass = "MethodClass";
        public const string InspectionTargetClass = "InspectionTargetClass";
        public const string InspectionTargetGroupClass = "InspectionTargetGroupClass";
        public const string NotificationClass = "NotificationClass";
        public const string ParameterClass = "ParameterClass";
        public const string PrintLabelClass = "PrintLabelClass";
        public const string ProblemClass = "ProblemClass";
        public const string ReceiptLotClass = "ReceiptLotClass";
        public const string RegulatoryListClass = "RegulatoryListClass";
        public const string ReportClass = "ReportClass";
        public const string ResultClass = "ResultClass";
        public const string RequestClass = "RequestClass";
        public const string RequestContainerDispenseClass = "RequestContainerDispenseClass";
        public const string RequestContainerUpdateClass = "RequestContainerUpdateClass";
        public const string RequestMaterialDispenseClass = "RequestMaterialDispenseClass";
        public const string RequestMaterialCreateClass = "RequestMaterialCreateClass";
        public const string RoleClass = "RoleClass";
        public const string SampleClass = "SampleClass";
        public const string SizeClass = "SizeClass";
        public const string TaskClass = "TaskClass";
        public const string TestClass = "TestClass";
        public const string UnitOfMeasureClass = "UnitOfMeasureClass";
        public const string UserClass = "UserClass";
        public const string VendorClass = "VendorClass";
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
