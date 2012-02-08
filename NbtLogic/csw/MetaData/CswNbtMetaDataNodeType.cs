using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataNodeType : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataNodeType>, IComparable
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _NodeTypeRow;


        public CswNbtMetaDataNodeType( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            //Reassign( Row );
            _NodeTypeRow = Row;
            _UniqueId = CswConvert.ToInt32( Row[UniqueIdFieldName] );
        }

        public DataRow _DataRow
        {
            get { return _NodeTypeRow; }
            //set { _NodeTypeRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "nodetypeid"; } }

        //public void Reassign( DataRow NewRow )
        //{
        //    _NodeTypeRow = NewRow;
        //    _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );

        //    // BZ 8370 - Remove cached data entities
        //    _FirstVersionNodeType = null;
        //    _PriorVersionNodeType = null;
        //    _BarcodeProperty = null;
        //}

        public override string ToString()
        {
            // This is mostly helpful for debugging
            return NodeTypeName + " (" + NodeTypeId.ToString() + ")";
        }

        public Int32 NodeTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypeRow["nodetypeid"].ToString() ); }
        }
        public string NodeTypeName
        {
            get { return _NodeTypeRow["nodetypename"].ToString(); }
            set
            {
                if( _NodeTypeRow["nodetypename"].ToString() != value )
                {
                    // BZ 7648 - Make sure name is unique
                    CswNbtMetaDataNodeType ExistingNodeType = _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( value );
                    if( ExistingNodeType != null && ExistingNodeType.FirstVersionNodeTypeId != this.FirstVersionNodeTypeId )
                        throw new CswDniException( ErrorType.Warning, "Node Type Name must be unique", "Attempted to rename a nodetype to the same name as an existing nodetype" );

                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this );

                    _NodeTypeRow["nodetypename"] = value;
                    _CswNbtMetaDataResources.NodeTypesCollection.clearCache();

                    if( _CswNbtMetaDataResources.CswNbtMetaData.OnEditNodeTypeName != null )
                        _CswNbtMetaDataResources.CswNbtMetaData.OnEditNodeTypeName( this );

                    //refresh view
                    _CswNbtMetaDataResources.CswNbtMetaData._RefreshViewForNodetypeId.Add(this.NodeTypeId);
                }
            }
        }

        public string TableName
        {
            get
            {
                string ret = "nodes";
                if( _NodeTypeRow["tablename"].ToString() != string.Empty )
                    ret = _NodeTypeRow["tablename"].ToString();
                return ret;
            }
            set { _NodeTypeRow["tablename"] = value; }
        }

        public string Category
        {
            get { return _NodeTypeRow["category"].ToString(); }
            set
            {
                if( _NodeTypeRow["category"].ToString() != value )
                {
                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this );
                    _NodeTypeRow["category"] = value;
                }
            }
        }
        public string IconFileName
        {
            get { return _NodeTypeRow["iconfilename"].ToString(); }
            set
            {
                if( _NodeTypeRow["iconfilename"].ToString() != value )
                {
                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this );
                    _NodeTypeRow["iconfilename"] = value;
                }
            }
        }
        public Int32 Quota
        {
            get { return CswConvert.ToInt32( _NodeTypeRow["quota"] ); }
        }
        public string NameTemplateValue
        {
            get { return _NodeTypeRow["nametemplate"].ToString(); }
            set
            {
                if( _NodeTypeRow["nametemplate"].ToString() != value )
                {
                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this );
                    _NodeTypeRow["nametemplate"] = value;
                    // Need to set all node records to pendingupdate if this changes
                    SetNodesToPendingUpdate();
                }
            }
        }
        public string NameTemplateText
        {
            get { return CswNbtMetaData.TemplateValueToTemplateText( getNodeTypeProps(), NameTemplateValue ); }
            set { NameTemplateValue = CswNbtMetaData.TemplateTextToTemplateValue( getNodeTypeProps(), value ); }
        }

        public Int32 PriorVersionNodeTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypeRow["priorversionid"] ); }
            set
            {
                _NodeTypeRow["priorversionid"] = CswConvert.ToDbVal( value );
                _PriorVersionNodeType = null;
            }
        }
        public Int32 FirstVersionNodeTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypeRow["firstversionid"] ); }
            set
            {
                _NodeTypeRow["firstversionid"] = CswConvert.ToDbVal( value );
                _FirstVersionNodeType = null;
            }
        }
        private CswNbtMetaDataNodeType _PriorVersionNodeType = null;
        public CswNbtMetaDataNodeType getPriorVersionNodeType()
        {
            if( _PriorVersionNodeType == null && PriorVersionNodeTypeId > 0 )
                _PriorVersionNodeType = _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( PriorVersionNodeTypeId );
            return _PriorVersionNodeType;
        }
        private CswNbtMetaDataNodeType _FirstVersionNodeType = null;
        public CswNbtMetaDataNodeType getFirstVersionNodeType()
        {
            if( _FirstVersionNodeType == null && FirstVersionNodeTypeId > 0 )
                _FirstVersionNodeType = _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( FirstVersionNodeTypeId );
            return _FirstVersionNodeType;
        }
        public CswNbtMetaDataNodeType getNodeTypeLatestVersion()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeLatestVersion( this );
        }

        public Int32 VersionNo
        {
            get { return CswConvert.ToInt32( _NodeTypeRow["versionno"] ); }
            set
            {
                _NodeTypeRow["versionno"] = CswConvert.ToDbVal( value );
                _CswNbtMetaDataResources.NodeTypesCollection.clearCache();
            }
        }
        public bool IsLocked
        {
            get { return ( _NodeTypeRow["islocked"].ToString() == "1" ); }
            set
            {
                // This is the only change to islocked that's allowed
                if( _NodeTypeRow["islocked"].ToString() == "0" && value )
                {
                    _NodeTypeRow["islocked"] = "1";
                }
                else if( IsLatestVersion() )
                {
                    _NodeTypeRow["islocked"] = CswConvert.ToDbVal( value );
                }
            }
        }

        public bool CanSave()
        {
            return ( ( !IsLocked || IsLatestVersion() ) &&
                     ( _CswNbtMetaDataResources.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, this ) ) );
        }
        public bool CanDelete()
        {
            return ( ( !IsLocked || IsLatestVersion() ) &&
                     ( _CswNbtMetaDataResources.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, this ) ) );
        }


        public bool IsLatestVersion()
        {
            return ( _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeLatestVersion( this ) == this );
        }

        /// <summary>
        /// Returns whether any node exists on this nodetype
        /// </summary>
        public bool InUse
        {
            get
            {
                CswTableSelect NodesTableSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "nodetype_in_use_select", "nodes" );
                return ( NodesTableSelect.getRecordCount( "nodetypeid", this.NodeTypeId ) > 0 );
            }
        }

        /// <summary>
        /// Returns whether the nodetype has a unique and required property, thus
        /// preventing multi-add and copy
        /// </summary>
        /// <param name="ErrorPropName">The name of the unique and required property</param>
        public bool IsUniqueAndRequired( ref string ErrorPropName )
        {
            bool ret = false;
            foreach( CswNbtMetaDataNodeTypeProp Prop in this.getNodeTypeProps() )
            {
                if( Prop.IsRequired && Prop.IsUnique )
                {
                    ErrorPropName = Prop.PropName;
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        //public CswNbtMetaDataObjectClass ObjectClass
        //{
        //    get { return _CswNbtMetaDataResources.CswNbtMetaData.getObjectClass( CswConvert.ToInt32( _NodeTypeRow["objectclassid"].ToString() ) ); }
        //}

        public CswNbtMetaDataObjectClass getObjectClass()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getObjectClass( CswConvert.ToInt32( _NodeTypeRow["objectclassid"].ToString() ) );
        }

        public Int32 ObjectClassId
        {
            get
            {
                return CswConvert.ToInt32( _NodeTypeRow["objectclassid"].ToString() );
            }
        }


        //public Collection<Int32> NodeTypeTabIds { get { return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTabIds( NodeTypeId ); } }
        //public IEnumerable<CswNbtMetaDataNodeTypeTab> NodeTypeTabs { get { return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTabs( NodeTypeId ); } }
        //public Collection<Int32> NodeTypePropIds { get { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropIds( NodeTypeId ); } }
        //public IEnumerable<CswNbtMetaDataNodeTypeProp> NodeTypeProps { get { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProps( NodeTypeId ); } }

        public Collection<Int32> getNodeTypeTabIds()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTabIds( NodeTypeId );
        }
        public IEnumerable<CswNbtMetaDataNodeTypeTab> getNodeTypeTabs()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTabs( NodeTypeId );
        }
        public Collection<Int32> getNodeTypePropIds()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropIds( NodeTypeId );
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProps( NodeTypeId );
        }


        public CswNbtMetaDataNodeTypeTab getFirstNodeTypeTab()
        {
            CswNbtMetaDataNodeTypeTab FirstTab = null;
            foreach( CswNbtMetaDataNodeTypeTab Tab in getNodeTypeTabs() )
            {
                if( FirstTab == null )
                {
                    FirstTab = Tab;
                    break;
                }
            }
            return FirstTab;
        }
        public CswNbtMetaDataNodeTypeTab getSecondNodeTypeTab()
        {
            CswNbtMetaDataNodeTypeTab SecondTab = null;
            bool first = true;
            foreach( CswNbtMetaDataNodeTypeTab Tab in getNodeTypeTabs() )
            {
                if( first )
                    first = false;
                else if( SecondTab == null )
                {
                    SecondTab = Tab;
                    break;
                }
            }
            return SecondTab;
        }

        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( string NodeTypeTabName )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTab( NodeTypeId, NodeTypeTabName );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeTabId )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTab( NodeTypeId, NodeTypeTabId );
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( string NodeTypePropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProp( NodeTypeId, NodeTypePropName );
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypePropId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProp( NodeTypeId, NodeTypePropId );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTabByFirstVersionId( Int32 FirstTabVersionId )
        {
            CswNbtMetaDataNodeTypeTab ret = null;
            foreach( CswNbtMetaDataNodeTypeTab Tab in getNodeTypeTabs() )
            {
                if( Tab.FirstTabVersionId == FirstTabVersionId )
                {
                    ret = Tab;
                    break;
                }
            }
            return ret;
        } // getNodeTypeTabByFirstVersionId()

        public CswNbtMetaDataNodeTypeProp getNodeTypePropByFirstVersionId( Int32 FirstPropVersionId )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            foreach( CswNbtMetaDataNodeTypeProp Prop in getNodeTypeProps() )
            {
                if( Prop.FirstPropVersionId == FirstPropVersionId )
                {
                    ret = Prop;
                    break;
                }
            }
            return ret;
        } // getNodeTypePropByFirstVersionId()

        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassPropName( string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropByObjectClassPropName( NodeTypeId, ObjectClassPropName );
        }

        public Int32 GetMaximumTabOrder()
        {
            Int32 MaximumTabOrder = 0;
            foreach( CswNbtMetaDataNodeTypeTab Tab in this.getNodeTypeTabs() )
            {
                if( Tab.TabOrder > MaximumTabOrder )
                    MaximumTabOrder = Tab.TabOrder;
            }
            return MaximumTabOrder;
        }

        //public Int32 getCurrentMaxDisplayRowAdd()
        //{
        //    Int32 Max = 0;
        //    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeProps )
        //    {
        //        if( Prop.DisplayRowAdd > Max )
        //            Max = Prop.DisplayRowAdd;
        //    }
        //    return Max;
        //}

        public static string _Element_MetaDataNodeType = "MetaDataNodeType";
        public static string _Attribute_NodeTypeId = "nodetypeid";
        public static string _Attribute_ObjectClassId = "objectclassid";
        public static string _Attribute_NodeTypeName = "nodetypename";
        public static string _Attribute_IconFileName = "iconfilename";
        public static string _Attribute_Version = "version";
        public static string _Attribute_Category = "category";
        public static string _Attribute_IsLatestVersion = "islatestversion";
        public static string _Attribute_TableName = "tablename";
        public static string _Attribute_PriorNodeTypeId = "priorversionid";
        public static string _Attribute_FirstNodeTypeId = "firstversionid";
        public static string _Attribute_NameTemplate = "nametemplate";

        public XmlDocument ToXml( CswNbtView View, bool ForMobile, bool PropsInViewOnly )
        {
            CswNbtMetaDataNodeType LatestVersionNT = getNodeTypeLatestVersion();
            XmlDocument XmlDoc = new XmlDocument();

            XmlNode XmlNode = XmlDoc.CreateNode( XmlNodeType.Element, _Element_MetaDataNodeType, "" );
            XmlDoc.AppendChild( XmlNode );

            XmlAttribute NodeTypeIdAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypeId );
            NodeTypeIdAttr.Value = NodeTypeId.ToString();
            XmlNode.Attributes.Append( NodeTypeIdAttr );

            XmlAttribute ObjectClassIdAttr = XmlDoc.CreateAttribute( _Attribute_ObjectClassId );
            ObjectClassIdAttr.Value = ObjectClassId.ToString();
            XmlNode.Attributes.Append( ObjectClassIdAttr );

            XmlAttribute NodeTypeNameAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypeName );
            NodeTypeNameAttr.Value = NodeTypeName;
            XmlNode.Attributes.Append( NodeTypeNameAttr );

            XmlAttribute CategoryAttr = XmlDoc.CreateAttribute( _Attribute_Category );
            CategoryAttr.Value = Category;
            XmlNode.Attributes.Append( CategoryAttr );

            XmlAttribute IconFileNameAttr = XmlDoc.CreateAttribute( _Attribute_IconFileName );
            IconFileNameAttr.Value = IconFileName;
            XmlNode.Attributes.Append( IconFileNameAttr );

            XmlAttribute VersionAttr = XmlDoc.CreateAttribute( _Attribute_Version ); //bz # 8016
            VersionAttr.Value = VersionNo.ToString();
            XmlNode.Attributes.Append( VersionAttr );

            XmlAttribute IsLatestVersionAttr = XmlDoc.CreateAttribute( _Attribute_IsLatestVersion ); //bz # 8016
            IsLatestVersionAttr.Value = LatestVersionNT.NodeTypeId == NodeTypeId ? "1" : "0";
            XmlNode.Attributes.Append( IsLatestVersionAttr );

            XmlAttribute PriorVersionAttr = XmlDoc.CreateAttribute( _Attribute_PriorNodeTypeId );
            PriorVersionAttr.Value = PriorVersionNodeTypeId.ToString();
            XmlNode.Attributes.Append( PriorVersionAttr );

            XmlAttribute FirstVersionAttr = XmlDoc.CreateAttribute( _Attribute_FirstNodeTypeId );
            FirstVersionAttr.Value = FirstVersionNodeTypeId.ToString();
            XmlNode.Attributes.Append( FirstVersionAttr );

            XmlAttribute TableNameAttr = XmlDoc.CreateAttribute( _Attribute_TableName );
            TableNameAttr.Value = LatestVersionNT.TableName;
            XmlNode.Attributes.Append( TableNameAttr );

            XmlAttribute NameTemplateAttr = XmlDoc.CreateAttribute( _Attribute_NameTemplate );
            NameTemplateAttr.Value = NameTemplateText;
            XmlNode.Attributes.Append( NameTemplateAttr );

            foreach( CswNbtMetaDataNodeTypeTab Tab in this.getNodeTypeTabs() )
            {
                XmlNode TabNode = Tab.ToXml( View, XmlDoc, ForMobile, PropsInViewOnly );
                if( TabNode != null )
                    XmlNode.AppendChild( TabNode );
            }
            return XmlDoc;
        }

        private CswNbtMetaDataNodeTypeProp _BarcodeProperty;
        public CswNbtMetaDataNodeTypeProp getBarcodeProperty()
        {
            if( _BarcodeProperty == null )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in this.getNodeTypeProps() )
                {
                    if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode )
                    {
                        if( _BarcodeProperty != null )
                            throw new CswDniException( ErrorType.Warning, "Multiple Barcodes Found", "Nodetype " + NodeTypeName + " has more than one barcode property" );
                        _BarcodeProperty = Prop;
                    }
                }
            }
            return _BarcodeProperty;
        } // getBarcodeProperty()

        public CswNbtView CreateDefaultView()
        {
            CswNbtView DefaultView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
            DefaultView.ViewName = this.NodeTypeName;

            CswNbtViewRelationship RelationshipToMe = DefaultView.AddViewRelationship( this, true );
            //RelationshipToMe.ArbitraryId = RelationshipToMe.SecondId.ToString();

            return DefaultView;
        }

        public Collection<CswNbtNode> getNodes( bool forceReInit, bool includeSystemNodes )
        {
            Collection<CswNbtNode> Collection = new Collection<CswNbtNode>();
            CswNbtView View = CreateDefaultView();
            ICswNbtTree Tree = _CswNbtMetaDataResources.CswNbtResources.Trees.getTreeFromView( View, forceReInit, true, true, includeSystemNodes );
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                Collection.Add( Tree.getNodeForCurrentPosition() );
                Tree.goToParentNode();
            }
            return Collection;
        }



        #region IEquatable

        public static bool operator ==( CswNbtMetaDataNodeType nt1, CswNbtMetaDataNodeType nt2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( nt1, nt2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) nt1 == null ) || ( (object) nt2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( nt1.UniqueId == nt2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataNodeType nt1, CswNbtMetaDataNodeType nt2 )
        {
            return !( nt1 == nt2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtMetaDataNodeType ) )
                return false;
            return this == (CswNbtMetaDataNodeType) obj;
        }

        public bool Equals( CswNbtMetaDataNodeType obj )
        {
            return this == (CswNbtMetaDataNodeType) obj;
        }

        public override int GetHashCode()
        {
            return this.NodeTypeId;
        }

        #endregion IEquatable

        #region IComparable

        public int CompareTo( object obj )
        {
            if( !( obj is CswNbtMetaDataNodeType ) )
                throw new ArgumentException( "object is not a CswNbtMetaDataNodeType" );

            CswNbtMetaDataNodeType OtherNodeType = (CswNbtMetaDataNodeType) obj;
            return CompareTo( OtherNodeType );
        } // CompareTo (object)

        /// <summary>
        /// Comparison function for SortedList
        /// </summary>
        /// <remarks>
        /// See BZ 8725 before changing this function
        /// </remarks>
        public int CompareTo( CswNbtMetaDataNodeType OtherNodeType )
        {
            int ret = 0;
            CswNbtMetaDataNodeType ThisFirstVersionNT = this.getFirstVersionNodeType();
            CswNbtMetaDataNodeType OtherFirstVersionNT =  OtherNodeType.getFirstVersionNodeType();

            if( this.FirstVersionNodeTypeId == OtherNodeType.FirstVersionNodeTypeId )
            {
                // This is inverted on purpose, so new (later) versions are first
                //ret = this.VersionNo.CompareTo( OtherNodeType.VersionNo );
                ret = OtherNodeType.VersionNo.CompareTo( this.VersionNo );
            }
            else if( OtherFirstVersionNT != null )
            {
                // Group things by their first version's name, so that the above clause will apply
                ret = ThisFirstVersionNT.NodeTypeName.CompareTo( OtherFirstVersionNT.NodeTypeName );
            }
            else // (since we build collections by id, the only time this happens is if OtherNodeType is a first version)
            {
                // Group things by their first version's name, so that the above clause will apply
                ret = ThisFirstVersionNT.NodeTypeName.CompareTo( OtherNodeType.NodeTypeName );
            }
            return ret;
        } // CompareTo (CswNbtMetaDataNodeType)

        #endregion IComparable

        public void SetNodesToPendingUpdate()
        {
            CswTableUpdate NodesUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "SetNodesToPendingUpdate", "nodes" );
            DataTable NodesTable = NodesUpdate.getTable( "nodetypeid", NodeTypeId );
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                NodesRow["pendingupdate"] = "1";
            }
            NodesUpdate.update( NodesTable );
        }
    }
}
