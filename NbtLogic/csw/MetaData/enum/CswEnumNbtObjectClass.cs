using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    public sealed class CswEnumNbtObjectClass : IEquatable<CswEnumNbtObjectClass>, IComparable<CswEnumNbtObjectClass>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { AliquotClass                      , AliquotClass                       },
            { BatchOpClass                      , BatchOpClass                       },
            { BiologicalClass                   , BiologicalClass                    },
            { ChemicalClass                     , ChemicalClass                      },
            { CofADocumentClass                 , CofADocumentClass                  },
            { CofAMethodClass                   , CofAMethodClass                    },
            { CofAMethodTemplateClass           , CofAMethodTemplateClass            },
            { ContainerClass                    , ContainerClass                     },
            { ContainerLocationClass            , ContainerLocationClass             },
            { ContainerDispenseTransactionClass , ContainerDispenseTransactionClass  },
            { ContainerGroupClass               , ContainerGroupClass                },
            { CustomerClass                     , CustomerClass                      },
            { DesignNodeTypeClass               , DesignNodeTypeClass                },
            { DesignNodeTypePropClass           , DesignNodeTypePropClass            },
            { DesignNodeTypeTabClass            , DesignNodeTypeTabClass             },
            { DesignSequenceClass               , DesignSequenceClass                },
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
            { GHSClass                          , GHSClass                           },
            { GHSPhraseClass                    , GHSPhraseClass                     },
            { InspectionDesignClass             , InspectionDesignClass              },
            { InspectionRouteClass              , InspectionRouteClass               },
            { InventoryGroupClass               , InventoryGroupClass                },
            { InventoryGroupPermissionClass     , InventoryGroupPermissionClass      },
            { InventoryLevelClass               , InventoryLevelClass                },
            { JurisdictionClass                 , JurisdictionClass                  },
            { LocationClass                     , LocationClass                      },
            { MailReportClass                   , MailReportClass                    },
            { ManufacturerEquivalentPartClass   , ManufacturerEquivalentPartClass    },
            { MaterialComponentClass            , MaterialComponentClass             },
            { MaterialSynonymClass              , MaterialSynonymClass               },
            { MethodClass                       , MethodClass                        },
            { NonChemicalClass                  , NonChemicalClass                   },
            { InspectionTargetClass             , InspectionTargetClass              },
            { InspectionTargetGroupClass        , InspectionTargetGroupClass         },
            { ParameterClass                    , ParameterClass                     },
            { PrinterClass                      , PrinterClass                       },
            { PrintJobClass                     , PrintJobClass                      },
            { PrintLabelClass                   , PrintLabelClass                    },
            { ProblemClass                      , ProblemClass                       },
            { ReceiptLotClass                   , ReceiptLotClass                    },
            { RegulatoryListClass               , RegulatoryListClass                },
            { RegulatoryListCasNoClass          , RegulatoryListCasNoClass           },
            { RegulatoryListMemberClass         , RegulatoryListMemberClass          },
            { RegulatoryListListCodeClass       , RegulatoryListListCodeClass        },
            { ReportClass                       , ReportClass                        },
            { ResultClass                       , ResultClass                        },
            { RequestClass                      , RequestClass                       },
            { RequestContainerDispenseClass     , RequestContainerDispenseClass      },
            { RequestContainerUpdateClass       , RequestContainerUpdateClass        },
            { RequestMaterialCreateClass        , RequestMaterialCreateClass         },
            { RequestMaterialDispenseClass      , RequestMaterialDispenseClass       },
            { RoleClass                         , RoleClass                          },
            { SampleClass                       , SampleClass                        },
            { SDSDocumentClass                  , SDSDocumentClass                   },
            { SizeClass                         , SizeClass                          },
            { TaskClass                         , TaskClass                          },
            { TestClass                         , TestClass                          },
            { UnitOfMeasureClass                , UnitOfMeasureClass                 },
            { UserClass                         , UserClass                          },
            { VendorClass                       , VendorClass                        },
            { WorkUnitClass                     , WorkUnitClass                      }
        };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        public static IEnumerable<string> All { get { return _Enums.Values; } }

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
        public CswEnumNbtObjectClass( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtObjectClass( string Val )
        {
            return new CswEnumNbtObjectClass( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtObjectClass item )
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
        public const string ChemicalClass = "ChemicalClass";
        public const string CofADocumentClass = "CofADocumentClass";
        public const string CofAMethodClass = "CofAMethodClass";
        public const string CofAMethodTemplateClass = "CofAMethodTemplateClass";
        public const string ContainerClass = "ContainerClass";
        public const string ContainerLocationClass = "ContainerLocationClass";
        public const string ContainerDispenseTransactionClass = "ContainerDispenseTransactionClass";
        public const string ContainerGroupClass = "ContainerGroupClass";
        public const string CustomerClass = "CustomerClass";
        public const string DesignNodeTypeClass = "DesignNodeTypeClass";
        public const string DesignNodeTypePropClass = "DesignNodeTypePropClass";
        public const string DesignNodeTypeTabClass = "DesignNodeTypeTabClass";
        public const string DesignSequenceClass = "DesignSequenceClass";
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
        public const string GHSClass = "GHSClass";
        public const string GHSPhraseClass = "GHSPhraseClass";
        public const string InspectionDesignClass = "InspectionDesignClass";
        public const string InspectionRouteClass = "InspectionRouteClass";
        public const string InventoryGroupClass = "InventoryGroupClass";
        public const string InventoryGroupPermissionClass = "InventoryGroupPermissionClass";
        public const string InventoryLevelClass = "InventoryLevelClass";
        public const string JurisdictionClass = "JurisdictionClass";
        public const string LocationClass = "LocationClass";
        public const string MailReportClass = "MailReportClass";
        public const string ManufacturerEquivalentPartClass = "ManufacturerEquivalentPartClass";
        public const string MaterialComponentClass = "MaterialComponentClass";
        public const string MaterialSynonymClass = "MaterialSynonymClass";
        public const string MethodClass = "MethodClass";
        public const string NonChemicalClass = "NonChemicalClass";
        public const string InspectionTargetClass = "InspectionTargetClass";
        public const string InspectionTargetGroupClass = "InspectionTargetGroupClass";
        public const string ParameterClass = "ParameterClass";
        public const string PrinterClass = "PrinterClass";
        public const string PrintJobClass = "PrintJobClass";
        public const string PrintLabelClass = "PrintLabelClass";
        public const string ProblemClass = "ProblemClass";
        public const string ReceiptLotClass = "ReceiptLotClass";
        public const string RegulatoryListClass = "RegulatoryListClass";
        public const string RegulatoryListCasNoClass = "RegulatoryListCasNoClass";
        public const string RegulatoryListMemberClass = "RegulatoryListMemberClass";
        public const string RegulatoryListListCodeClass = "RegulatoryListListCodeClass";
        public const string ReportClass = "ReportClass";
        public const string ResultClass = "ResultClass";
        public const string RequestClass = "RequestClass";
        public const string RequestContainerDispenseClass = "RequestContainerDispenseClass";
        public const string RequestContainerUpdateClass = "RequestContainerUpdateClass";
        public const string RequestMaterialDispenseClass = "RequestMaterialDispenseClass";
        public const string RequestMaterialCreateClass = "RequestMaterialCreateClass";
        public const string RoleClass = "RoleClass";
        public const string SampleClass = "SampleClass";
        public const string SDSDocumentClass = "SDSDocumentClass";
        public const string SizeClass = "SizeClass";
        public const string TaskClass = "TaskClass";
        public const string TestClass = "TestClass";
        public const string UnitOfMeasureClass = "UnitOfMeasureClass";
        public const string UserClass = "UserClass";
        public const string VendorClass = "VendorClass";
        public const string WorkUnitClass = "WorkUnitClass";

        #endregion Enum members


        #region IComparable

        public int CompareTo( CswEnumNbtObjectClass other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (NbtObjectClass)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtObjectClass ft1, CswEnumNbtObjectClass ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtObjectClass ft1, CswEnumNbtObjectClass ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtObjectClass ) )
            {
                return false;
            }
            return this == (CswEnumNbtObjectClass) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtObjectClass obj )
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
