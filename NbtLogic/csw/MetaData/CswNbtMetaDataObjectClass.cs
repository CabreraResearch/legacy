using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataObjectClass : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataObjectClass>
    {
        public const string IconPrefix16 = "Images/newicons/16/";
        public const string IconPrefix18 = "Images/newicons/18/";
        public const string IconPrefix100 = "Images/newicons/100/";

        /// <summary>
        /// Template for new NbtObjectClass class
        /// </summary>
        public sealed class NbtObjectClass : IEquatable<NbtObjectClass>
        {
            #region Internals
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { AliquotClass                      , AliquotClass                       },
                                                                       { BatchOpClass                      , BatchOpClass                       },
                                                                       { BiologicalClass                   , BiologicalClass                    },
                                                                       { MaterialComponentClass            , MaterialComponentClass             },
                                                                       { ContainerClass                    , ContainerClass                     },
                                                                       { ContainerDispenseTransactionClass , ContainerDispenseTransactionClass  },
                                                                       { CustomerClass                     , CustomerClass                      },
                                                                       { DocumentClass                     , DocumentClass                      },
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
                                                                       { LocationClass                     , LocationClass                      },
                                                                       { MailReportClass                   , MailReportClass                    },
                                                                       { MaterialClass                     , MaterialClass                      },
                                                                       { MaterialSynonymClass              , MaterialSynonymClass               },
                                                                       { InspectionTargetClass             , InspectionTargetClass              },
                                                                       { InspectionTargetGroupClass        , InspectionTargetGroupClass         },
                                                                       { NotificationClass                 , NotificationClass                  },
                                                                       { ParameterClass                    , ParameterClass                     },
                                                                       { PrintLabelClass                   , PrintLabelClass                    },
                                                                       { ProblemClass                      , ProblemClass                       },
                                                                       { RegulatoryListClass               , RegulatoryListClass                },
                                                                       { ReportClass                       , ReportClass                        },
                                                                       { ResultClass                       , ResultClass                        },
                                                                       { RequestClass                      , RequestClass                       },
                                                                       { RequestItemClass                  , RequestItemClass                   },
                                                                       { RoleClass                         , RoleClass                          },
                                                                       { SampleClass                       , SampleClass                        },
                                                                       { SizeClass                         , SizeClass                          },
                                                                       { TaskClass                         , TaskClass                          },
                                                                       { TestClass                         , TestClass                          },
                                                                       { UnitOfMeasureClass                , UnitOfMeasureClass                 },
                                                                       { UserClass                         , UserClass                          },
                                                                       { VendorClass                       , VendorClass                        },
                                                                       {  WorkUnitClass                    , WorkUnitClass                     }
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

            /// <summary>
            /// Enum member 1
            /// </summary>
            public const string AliquotClass = "AliquotClass";
            public const string BatchOpClass = "BatchOpClass";
            public const string BiologicalClass = "BiologicalClass";
            public const string CertMethodTemplateClass = "CertMethodTemplateClass";
            public const string ContainerClass = "ContainerClass";
            public const string ContainerDispenseTransactionClass = "ContainerDispenseTransactionClass";
            public const string CustomerClass = "CustomerClass";
            public const string DocumentClass = "DocumentClass";
            public const string EquipmentAssemblyClass = "EquipmentAssemblyClass";
            public const string EquipmentClass = "EquipmentClass";
            public const string EquipmentTypeClass = "EquipmentTypeClass";
            public const string FeedbackClass = "FeedbackClass";
            public const string GeneratorClass = "GeneratorClass";
            public const string GenericClass = "GenericClass";
            public const string InspectionDesignClass = "InspectionDesignClass";
            public const string InspectionRouteClass = "InspectionRouteClass";
            public const string InventoryGroupClass = "InventoryGroupClass";
            public const string InventoryGroupPermissionClass = "InventoryGroupPermissionClass";
            public const string InventoryLevelClass = "InventoryLevelClass";
            public const string LocationClass = "LocationClass";
            public const string MailReportClass = "MailReportClass";
            public const string MaterialClass = "MaterialClass";
            public const string MaterialComponentClass = "MaterialComponentClass";
            public const string MaterialSynonymClass = "MaterialSynonymClass";
            public const string InspectionTargetClass = "InspectionTargetClass";
            public const string InspectionTargetGroupClass = "InspectionTargetGroupClass";
            public const string NotificationClass = "NotificationClass";
            public const string ParameterClass = "ParameterClass";
            public const string PrintLabelClass = "PrintLabelClass";
            public const string ProblemClass = "ProblemClass";
            public const string RegulatoryListClass = "RegulatoryListClass";
            public const string ReportClass = "ReportClass";
            public const string ResultClass = "ResultClass";
            public const string RequestClass = "RequestClass";
            public const string RequestItemClass = "RequestItemClass";
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

        public static NbtObjectClass getObjectClassFromString( string ObjectClassName )
        {
            //bz # 7815 -- Should not care if the requested object class doesn't exist anymore
            return ( ObjectClassName );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _ObjectClassRow;

        public CswNbtMetaDataObjectClass( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            Reassign( Row );
        }

        public DataRow _DataRow
        {
            get { return _ObjectClassRow; }
            //set { _ObjectClassRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public const string MetaDataUniqueType = "objectclassid";
        public string UniqueIdFieldName { get { return MetaDataUniqueType; } }

        public void Reassign( DataRow NewRow )
        {
            _ObjectClassRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        public Int32 ObjectClassId
        {
            get { return CswConvert.ToInt32( _ObjectClassRow["objectclassid"].ToString() ); }
        }
        //public string TableName
        //{
        //    get { return _ObjectClassRow["tablename"].ToString(); }
        //}
        public NbtObjectClass ObjectClass
        {
            get { return getObjectClassFromString( _ObjectClassRow["objectclass"].ToString() ); }
        }
        public string IconFileName
        {
            get { return _ObjectClassRow["iconfilename"].ToString(); }
        }

        public Int32 Quota
        {
            get { return CswConvert.ToInt32( _ObjectClassRow["quota"] ); }
        }

        public Collection<Int32> getNodeTypeIds()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeIds( ObjectClassId );
        }

        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypes( ObjectClassId );
        }

        public IEnumerable<CswNbtMetaDataNodeType> getLatestVersionNodeTypes()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypesLatestVersion( ObjectClassId );
        }

        public CswNbtMetaDataNodeType FirstNodeType
        {
            get
            {
                //CswNbtMetaDataNodeType ret = null;
                //foreach( CswNbtMetaDataNodeType NT in NodeTypes )
                //{
                //    ret = NT;
                //    break;
                //}
                //return ret;
                return getNodeTypes().First<CswNbtMetaDataNodeType>();
            }
        } // FirstNodeType

        //private Hashtable _ObjectClassPropsByPropId;
        //private SortedList _ObjectClassPropsByPropName;

        //public delegate void AddPropEventHandler( CswNbtMetaDataObjectClassProp NewProp );
        //public event AddPropEventHandler OnAddProp = null;

        //public void AddProp( CswNbtMetaDataObjectClassProp Prop )
        //{
        //    _ObjectClassPropsByPropId.Add( Prop.PropId, Prop );
        //    _ObjectClassPropsByPropName.Add( Prop.PropName, Prop );
        //    //if ( OnAddProp != null )
        //    //    OnAddProp( Prop );
        //}
        private CswNbtMetaDataObjectClassProp _BarcodeProp = null;
        public CswNbtMetaDataObjectClassProp getBarcodeProp()
        {
            if( null == _BarcodeProp )
            {
                _BarcodeProp = ( from _Prop
                                     in _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByObjectClass( ObjectClassId )
                                 where _Prop.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode
                                 select _Prop ).FirstOrDefault();

            }
            return _BarcodeProp;
        }

        public Collection<Int32> getObjectClassPropIds()
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropIdsByObjectClass( ObjectClassId );
        }
        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassProps()
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByObjectClass( ObjectClassId );
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassId, ObjectClassPropName );
            //if ( _ObjectClassPropsByPropName.Contains( ObjectClassPropName ) )
            //    return _ObjectClassPropsByPropName[ ObjectClassPropName ] as CswNbtMetaDataObjectClassProp;
            //else
            //    return null;
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassId, ObjectClassPropId );
            //if ( _ObjectClassPropsByPropId.Contains( ObjectClassPropId ) )
            //    return _ObjectClassPropsByPropId[ ObjectClassPropId ] as CswNbtMetaDataObjectClassProp;
            //else
            //    return null;
        }

        public CswNbtView CreateDefaultView( bool includeDefaultFilters = true )
        {
            CswNbtView DefaultView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
            DefaultView.ViewName = this.ObjectClass.ToString();

            CswNbtViewRelationship RelationshipToMe = DefaultView.AddViewRelationship( this, includeDefaultFilters );
            //RelationshipToMe.ArbitraryId = RelationshipToMe.SecondId.ToString();
            //DefaultView.Root.addChildRelationship( RelationshipToMe );

            return DefaultView;
        }

        public Collection<CswNbtNode> getNodes( bool forceReInit, bool includeSystemNodes, bool includeDefaultFilters = false )
        {
            Collection<CswNbtNode> Collection = new Collection<CswNbtNode>();

            CswNbtView View = CreateDefaultView( includeDefaultFilters );
            ICswNbtTree Tree = _CswNbtMetaDataResources.CswNbtResources.Trees.getTreeFromView( View, forceReInit, true, true, includeSystemNodes );
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                Collection.Add( Tree.getNodeForCurrentPosition() );
                Tree.goToParentNode();
            }
            return Collection;
        }

        public bool CanAdd
        {
            get
            {
                return ( ( ( ObjectClass != NbtObjectClass.RoleClass &&
                             ObjectClass != NbtObjectClass.UserClass ) ||
                           _CswNbtMetaDataResources.CswNbtResources.CurrentNbtUser.IsAdministrator() ) &&
                       ObjectClass != NbtObjectClass.RequestItemClass &&
                       ObjectClass != NbtObjectClass.RequestClass &&
                       ObjectClass != NbtObjectClass.ContainerClass &&
                       ObjectClass != NbtObjectClass.MaterialClass &&
                       ObjectClass != NbtObjectClass.ContainerDispenseTransactionClass );
            }
        } // CanAdd

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataObjectClass oc1, CswNbtMetaDataObjectClass oc2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( oc1, oc2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) oc1 == null ) || ( (object) oc2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( oc1.UniqueId == oc2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataObjectClass oc1, CswNbtMetaDataObjectClass oc2 )
        {
            return !( oc1 == oc2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtMetaDataObjectClass ) )
                return false;
            return this == (CswNbtMetaDataObjectClass) obj;
        }

        public bool Equals( CswNbtMetaDataObjectClass obj )
        {
            return this == (CswNbtMetaDataObjectClass) obj;
        }

        public override int GetHashCode()
        {
            return this.ObjectClassId;
        }

        #endregion IEquatable
    }
}
