using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.MetaData
{
    [DataContract]
    public class CswNbtMetaDataObjectClass : ICswNbtMetaDataObject, ICswNbtMetaDataDefinitionObject, IEquatable<CswNbtMetaDataObjectClass>
    {
        public const string IconPrefix16 = "Images/newicons/16/";
        public const string IconPrefix18 = "Images/newicons/18/";
        public const string IconPrefix100 = "Images/newicons/100/";

        public static CswEnumNbtObjectClass getObjectClassFromString( string ObjectClassName )
        {
            //bz # 7815 -- Should not care if the requested object class doesn't exist anymore
            return ( ObjectClassName );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _ObjectClassRow;
        private CswDateTime _Date;
        public CswNbtMetaDataObjectClass( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row, CswDateTime Date = null)
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _Date = Date;
            Reassign( Row );
        }

        public DataRow _DataRow
        {
            get { return _ObjectClassRow; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
        }

        public const string MetaDataUniqueType = "objectclassid";
        public string UniqueIdFieldName { get { return MetaDataUniqueType; } }

        public void Reassign( DataRow NewRow )
        {
            _ObjectClassRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        [DataMember]
        public Int32 ObjectClassId
        {
            get { return CswConvert.ToInt32( _ObjectClassRow["objectclassid"].ToString() ); }
            private set { var KeepSerializerHappy = value; }
        }

        [DataMember( Name = "ObjectClass" )]
        public string ObjectClassName
        {
            get { return ObjectClass; }
            private set { var KeepSerializerHappy = value; }
        }

        [DataMember( Name = "ViewName" )]
        public string DbViewName
        {
            //get { return "OC" + ObjectClassName.ToUpper(); }
            //private set { var KeepSerializerHappy = value; }

            get
            {
                return CswConvert.ToString( _ObjectClassRow["oraviewname"] );
            }

        }

        public CswEnumNbtObjectClass ObjectClass
        {
            get { return getObjectClassFromString( _ObjectClassRow["objectclass"].ToString() ); }
        }

        public string IconFileName
        {
            get { return _ObjectClassRow["iconfilename"].ToString(); }
        }

        public const Int32 NotSearchableValue = 0;
        public Int32 SearchDeferPropId
        {
            get { return CswConvert.ToInt32( _ObjectClassRow["searchdeferpropid"] ); }
        }

        public Int32 Quota
        {
            get { return CswConvert.ToInt32( _ObjectClassRow["quota"] ); }
        }

        public bool ExcludeInQuotaBar
        {
            get
            {
                return CswConvert.ToBoolean( _ObjectClassRow["excludeinquotabar"] );
            }
            set
            {
                _ObjectClassRow["excludeinquotabar"] = CswConvert.ToBoolean( value );
            }
        }

        public int NodeCount
        {
            get
            {
                return CswConvert.ToInt32( _ObjectClassRow["nodecount"] );
            }
            set
            {
                _ObjectClassRow["nodecount"] = value;
            }
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

        public CswNbtMetaDataPropertySet getPropertySet()
        {
            return _CswNbtMetaDataResources.PropertySetsCollection.getPropertySetForObjectClass( ObjectClassId );
        }

        public CswNbtMetaDataNodeType FirstNodeType
        {
            get
            {
                return getNodeTypes().FirstOrDefault();
            }
        }

        private CswNbtMetaDataObjectClassProp _BarcodeProp = null;
        public ICswNbtMetaDataProp getBarcodeProperty()
        {
            if( null == _BarcodeProp )
            {
                _BarcodeProp = ( from _Prop
                                     in _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByObjectClass( ObjectClassId )
                                 where _Prop.getFieldTypeValue() == CswEnumNbtFieldType.Barcode
                                 select _Prop ).FirstOrDefault();
            }
            return _BarcodeProp;
        }
        public bool HasLabel { get { return false; } }

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
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassPropId );
        }

        public CswNbtView CreateDefaultView( bool IncludeDefaultFilters = true )
        {
            CswNbtView DefaultView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
            DefaultView.ViewName = this.ObjectClass.ToString();
            CswNbtViewRelationship RelationshipToMe = DefaultView.AddViewRelationship( this, IncludeDefaultFilters );
            return DefaultView;
        }

        public Collection<CswNbtNode> getNodes( bool forceReInit, bool includeSystemNodes, bool IncludeDefaultFilters = true, bool IncludeHiddenNodes = false )
        {
            Collection<CswNbtNode> Collection = new Collection<CswNbtNode>();
            CswNbtView View = CreateDefaultView( IncludeDefaultFilters );
            ICswNbtTree Tree = _CswNbtMetaDataResources.CswNbtResources.Trees.getTreeFromView( _CswNbtMetaDataResources.CswNbtResources.CurrentNbtUser, View, false, includeSystemNodes, IncludeHiddenNodes );
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                Collection.Add( Tree.getNodeForCurrentPosition() );
                Tree.goToParentNode();
            }
            return Collection;
        } // getNodes()

        public Dictionary<CswPrimaryKey, string> getNodeIdAndNames( bool forceReInit, bool includeSystemNodes, bool includeDefaultFilters = false, bool IncludeHiddenNodes = false, bool RequireViewPermissions = true )
        {
            Dictionary<CswPrimaryKey, string> Dict = new Dictionary<CswPrimaryKey, string>();
            CswNbtView View = CreateDefaultView( includeDefaultFilters );
            ICswNbtTree Tree = _CswNbtMetaDataResources.CswNbtResources.Trees.getTreeFromView( _CswNbtMetaDataResources.CswNbtResources.CurrentNbtUser, View, RequireViewPermissions, includeSystemNodes, IncludeHiddenNodes );
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                Dict.Add( Tree.getNodeIdForCurrentPosition(), Tree.getNodeNameForCurrentPosition() );
                Tree.goToParentNode();
            }
            return Dict;
        } // getNodeIdAndNames()

        /// <summary>
        /// Returns true if the user is allowed to add a new node of the given ObjectClass through the AddNode dialog.
        /// </summary>
        public bool CanAdd
        {
            get
            {
                return ( ( ( ObjectClass != CswEnumNbtObjectClass.RoleClass &&
                             ObjectClass != CswEnumNbtObjectClass.UserClass ) ||
                           _CswNbtMetaDataResources.CswNbtResources.CurrentNbtUser.IsAdministrator() ) &&
                       ObjectClass != CswEnumNbtObjectClass.RequestClass &&
                       ObjectClass != CswEnumNbtObjectClass.RequestItemClass &&
                       ObjectClass != CswEnumNbtObjectClass.RequestContainerDispenseClass &&
                       ObjectClass != CswEnumNbtObjectClass.RequestContainerUpdateClass &&
                       ObjectClass != CswEnumNbtObjectClass.RequestMaterialCreateClass &&
                       ObjectClass != CswEnumNbtObjectClass.RequestMaterialDispenseClass &&
                       ObjectClass != CswEnumNbtObjectClass.RegulatoryListCasNoClass &&
                       ObjectClass != CswEnumNbtObjectClass.ContainerClass &&
                       ObjectClass != CswEnumNbtObjectClass.ContainerLocationClass &&
                    //ObjectClass != NbtObjectClass.ChemicalClass &&    //Add Chemical now takes user to Create Material
                    //ObjectClass != NbtObjectClass.NonChemicalClass && //Add NonChemical now takes user to Create Material
                       ObjectClass != CswEnumNbtObjectClass.ContainerDispenseTransactionClass &&
                       ObjectClass != CswEnumNbtObjectClass.BatchOpClass &&
                       ObjectClass != CswEnumNbtObjectClass.ReceiptLotClass &&
                       ObjectClass != CswEnumNbtObjectClass.FeedbackClass &&
                       ObjectClass != CswEnumNbtObjectClass.PrintJobClass
                );
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
