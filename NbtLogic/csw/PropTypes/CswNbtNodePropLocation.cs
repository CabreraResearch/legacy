using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropLocation: CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLocation( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLocation;
        }

        public CswNbtNodePropLocation( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _NameSubField = _FieldTypeRule.NameSubField;
            _NodeIdSubField = _FieldTypeRule.NodeIdSubField;
            _RowSubField = _FieldTypeRule.RowSubField;
            _ColumnSubField = _FieldTypeRule.ColumnSubField;
            _PathSubField = _FieldTypeRule.PathSubField;
            _BarcodeSubField = _FieldTypeRule.BarcodeSubField;
        }

        private CswNbtFieldTypeRuleLocation _FieldTypeRule;
        private CswNbtSubField _NameSubField;
        private CswNbtSubField _NodeIdSubField;
        private CswNbtSubField _RowSubField;
        private CswNbtSubField _ColumnSubField;
        private CswNbtSubField _PathSubField;
        private CswNbtSubField _BarcodeSubField;

        public static string GetTopLevelName( CswNbtResources NbtResources )
        {
            return NbtResources.ConfigVbls.getConfigVariableValue( "LocationViewRootName" );
        }

        public bool CreateContainerLocation = true;

        override public bool Empty
        {
            get
            {
                return ( _CswNbtNodePropData.GetPropRowValue( _NodeIdSubField.Column ) == Int32.MinValue.ToString() );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public CswPrimaryKey SelectedNodeId
        {
            get
            {
                CswPrimaryKey ret = null;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _NodeIdSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                {
                    Int32 Val = CswConvert.ToInt32( StringVal );
                    if( Val != Int32.MinValue )
                        ret = new CswPrimaryKey( "nodes", Val );
                }
                return ret;
            }
            set
            {
                Int32 NewValue = Int32.MinValue;
                if( value != null && value.PrimaryKey > 0 )
                {
                    NewValue = value.PrimaryKey;
                }

                if( _CswNbtNodePropData.SetPropRowValue( _NodeIdSubField.Column, NewValue ) )
                {
                    RefreshNodeName();
                    PendingUpdate = true;
                }
            }
        }
        public string CachedNodeName
        {
            get
            {
                string NodeName = _CswNbtNodePropData.GetPropRowValue( _NameSubField.Column );
                if( string.IsNullOrEmpty( NodeName ) )
                {
                    NodeName = RefreshNodeName();
                }
                return NodeName;
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _NameSubField.Column, value, IsNonModifying : true );
            }
        }

        public string CachedPath
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _PathSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _PathSubField.Column, value, IsNonModifying : true );
                SyncGestalt();
            }
        }

        public string CachedFullPath
        {
            get { return Gestalt; }
        }

        public string CachedBarcode
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _BarcodeSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _BarcodeSubField.Column, value, IsNonModifying : true );
            }
        }

        //TODO: Deprecated; remove
        public Int32 SelectedRow
        {
            get
            {
                Int32 ret = Int32.MinValue;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _RowSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                {
                    ret = CswConvert.ToInt32( StringVal );
                }
                return ret;
            }
            set
            {
                if( _CswNbtNodePropData.SetPropRowValue( _RowSubField.Column, value, IsNonModifying : true ) )
                {
                    PendingUpdate = true;
                }
            }
        }

        //TODO: Deprecated; remove
        public Int32 SelectedColumn
        {
            get
            {
                Int32 ret = Int32.MinValue;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _ColumnSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                {
                    ret = CswConvert.ToInt32( StringVal );
                }
                return ret;
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ColumnSubField.Column, value, IsNonModifying : true );
                {
                    PendingUpdate = true;
                }
            }
        }

        public string RefreshNodeName()
        {
            CachedNodeName = CswNbtNodePropLocation.GetTopLevelName( _CswNbtResources );
            CachedPath = CachedNodeName;
            CachedBarcode = string.Empty;

            if( SelectedNodeId != null )
            {
                CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes.GetNode( SelectedNodeId );
                if( null != NodeAsLocation )
                {
                    CachedNodeName = NodeAsLocation.NodeName;
                    CachedPath = _generateLocationPath( NodeAsLocation );
                    CachedBarcode = NodeAsLocation.Barcode.Barcode;
                    SyncGestalt();
                }
            }

            this.PendingUpdate = false;
            return CachedNodeName;
        }

        public static readonly string PathDelimiter = " > ";
        private string _generateLocationPath( CswNbtObjClassLocation NodeAsLocation )
        {
            string ret;
            ret = NodeAsLocation.NodeName;
            if( NodeAsLocation.Location.SelectedNodeId != null )
            {
                string prev = _generateLocationPath( _CswNbtResources.Nodes[NodeAsLocation.Location.SelectedNodeId] );
                if( prev != string.Empty )
                    ret = prev + PathDelimiter + ret;
            }
            return ret;
        }

        public Int32 getLocationLevel()
        {
            return _getLocationLevel( _CswNbtResources.Nodes[SelectedNodeId] );
        }

        private Int32 _getLocationLevel( CswNbtObjClassLocation LocationNode )
        {
            Int32 Level = 1;
            if( LocationNode.Location.SelectedNodeId != null )
            {
                Level += _getLocationLevel( _CswNbtResources.Nodes[LocationNode.Location.SelectedNodeId] );
            }
            return Level;
        }

        public static CswNbtView LocationPropertyView( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp Prop, CswPrimaryKey NodeId = null, Collection<CswPrimaryKey> InventoryGroupIds = null )
        {
            CswNbtMetaDataObjectClass ContainerOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass UserOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            bool IsLocationNode = ( null != Prop && Prop.getNodeType().getObjectClass().ObjectClass == CswEnumNbtObjectClass.LocationClass );
            bool IsContainerNode = ( null != Prop && null != ContainerOC && Prop.getNodeType().ObjectClassId == ContainerOC.ObjectClassId );
            bool IsUserNode = ( null != Prop && null != ContainerOC && Prop.getNodeType().ObjectClassId == UserOC.ObjectClassId );

            CswNbtView Ret = new CswNbtView( CswNbtResources );
            Ret.ViewName = GetTopLevelName( CswNbtResources );
            Ret.Root.Included = IsLocationNode;
            CswNbtObjClassLocation.makeLocationsTreeView( ref Ret, CswNbtResources,
                                                          NodeIdToFilterOut : NodeId,
                                                          RequireAllowInventory : ( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) && ( IsContainerNode || IsUserNode ) ),
                                                          InventoryGroupIds : InventoryGroupIds );
            return Ret;
        }

        private CswNbtView _View = null;
        public CswNbtView View
        {
            get { return _View ?? ( _View = LocationPropertyView( _CswNbtResources, NodeTypeProp, NodeId, null ) ); } // get
            set { _View = value; }
        } // View

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_NodeIdSubField.ToXmlNodeName( true )] = string.Empty;
            ParentObject[_NameSubField.ToXmlNodeName( true )] = string.Empty;
            ParentObject[_PathSubField.ToXmlNodeName( true )] = string.Empty;
            ParentObject[_BarcodeSubField.ToXmlNodeName( true )] = string.Empty;
            //ParentObject[_ColumnSubField.ToXmlNodeName( true )] = ( SelectedColumn != Int32.MinValue ) ? SelectedColumn.ToString() : string.Empty;
            //ParentObject[_RowSubField.ToXmlNodeName( true )] = ( SelectedRow != Int32.MinValue ) ? SelectedRow.ToString() : string.Empty;

            CswNbtNode SelectedNode = _CswNbtResources.Nodes[SelectedNodeId];
            if( null != SelectedNode )
            {
                ParentObject[_NodeIdSubField.ToXmlNodeName( true )] = SelectedNode.NodeId.ToString();
                ParentObject[_NameSubField.ToXmlNodeName( true )] = CachedNodeName;
                ParentObject[_PathSubField.ToXmlNodeName( true )] = CachedPath;
                ParentObject[_BarcodeSubField.ToXmlNodeName( true )] = CachedBarcode;

                ParentObject["selectednodelink"] = SelectedNode.NodeLink;
            }

            View.SaveToCache( false );
            ParentObject["viewid"] = View.SessionViewId.ToString();

            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            ParentObject["locationobjectclassid"] = LocationOC.ObjectClassId.ToString();
            JArray LocationNTArray = new JArray();
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                LocationNTArray.Add( LocationNT.NodeTypeId );
            }
            ParentObject["locationnodetypeids"] = LocationNTArray;
        }

        // ReadXml()

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string LocationNodeIdStr = string.Empty;
            string LocationBarcode = string.Empty;
            //Int32 Row = Int32.MinValue;
            //Int32 Column = Int32.MinValue;
            if( null != JObject[_NodeIdSubField.ToXmlNodeName( true )] )
            {
                LocationNodeIdStr = (string) JObject[_NodeIdSubField.ToXmlNodeName( true )];
            }
            if( null != JObject[_BarcodeSubField.ToXmlNodeName( true )] )
            {
                LocationBarcode = (string) JObject[_BarcodeSubField.ToXmlNodeName( true )];
            }
            //if( null != JObject[_RowSubField.ToXmlNodeName( true )] )
            //{
            //    Row = CswConvert.ToInt32( JObject[_RowSubField.ToXmlNodeName( true )] );
            //}
            //if( null != JObject[_ColumnSubField.ToXmlNodeName( true )] )
            //{
            //    Column = CswConvert.ToInt32( JObject[_ColumnSubField.ToXmlNodeName( true )] );
            //}
            string SelectedNodeId = _saveProp( LocationNodeIdStr, LocationBarcode, NodeMap ); //, Row, Column );
            if( !string.IsNullOrEmpty( SelectedNodeId ) )
            {
                JObject["destnodeid"] = SelectedNodeId;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //SelectedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswConvert.ToInt32(PropRow[_NodeIdSubField.ToXmlNodeName()]), PropRow[_BarcodeSubField.ToXmlNodeName()].ToString(), NodeMap ) );

            string NodeId = CswTools.XmlRealAttributeName( PropRow[_NodeIdSubField.ToXmlNodeName()].ToString() );
            if( NodeMap != null && NodeMap.ContainsKey( NodeId.ToLower() ) )
                SelectedNodeId = new CswPrimaryKey( "nodes", NodeMap[NodeId.ToLower()] );
            else
            {
                //RelatedNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeId ) );
                CswPrimaryKey LocationNodeId = null;
                if( PropRow.Table.Columns.Contains( _BarcodeSubField.ToXmlNodeName() ) )
                    LocationNodeId = _HandleReference( NodeId, CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() ) );
                else
                    LocationNodeId = _HandleReference( NodeId, string.Empty );
                SelectedNodeId = LocationNodeId;
            }

            if( null != SelectedNodeId )
            {
                PendingUpdate = true;
            }

            //if( PropRow.Table.Columns.Contains( _RowSubField.ToXmlNodeName() ) )
            //{
            //    string StringVal = PropRow[_RowSubField.ToXmlNodeName()].ToString();
            //    if( CswTools.IsInteger( StringVal ) )
            //        SelectedRow = CswConvert.ToInt32( StringVal );
            //}

            //if( PropRow.Table.Columns.Contains( _ColumnSubField.ToXmlNodeName() ) )
            //{
            //    string StringVal = PropRow[_ColumnSubField.ToXmlNodeName()].ToString();
            //    if( CswTools.IsInteger( StringVal ) )
            //        SelectedColumn = CswConvert.ToInt32( StringVal );
            //}
            PendingUpdate = true;
        }

        private string _saveProp( string LocationNodeIdStr, string LocationBarcode, Dictionary<Int32, Int32> NodeMap ) //, Int32 Row, Int32 Column )
        {
            string RetVal = string.Empty;
            CswPrimaryKey LocationNodeId = _HandleReference( LocationNodeIdStr, LocationBarcode );
            if( NodeMap != null && NodeMap.ContainsKey( LocationNodeId.PrimaryKey ) )
            {
                LocationNodeId = new CswPrimaryKey( "nodes", NodeMap[LocationNodeId.PrimaryKey] );
            }
            SelectedNodeId = LocationNodeId;
            if( SelectedNodeId != null )
            {
                //SelectedRow = Row;
                //SelectedColumn = Column;
                RetVal = SelectedNodeId.PrimaryKey.ToString();
            }
            RefreshNodeName();
            return RetVal;
        }

        private CswPrimaryKey _HandleReference( string LocationNodeIdStr, string LocationBarcode ) //, Dictionary<Int32, Int32> NodeMap )
        {
            CswPrimaryKey LocationNodeId = new CswPrimaryKey();
            if( !string.IsNullOrEmpty( LocationNodeIdStr ) )
            {
                LocationNodeId.FromString( LocationNodeIdStr );
                if( LocationNodeId.PrimaryKey == Int32.MinValue && LocationBarcode != string.Empty )
                {
                    // Find the location with this barcode value
                    CswNbtMetaDataObjectClass LocationObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                    CswNbtMetaDataObjectClassProp BarcodeObjectClassProp = LocationObjectClass.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Barcode );

                    CswNbtView LocationView = new CswNbtView( _CswNbtResources );
                    // All locations..
                    CswNbtViewRelationship LocationRelationship = LocationView.AddViewRelationship( LocationObjectClass, false );
                    // ..with barcodes
                    CswNbtViewProperty BarcodeViewProperty = LocationView.AddViewProperty( LocationRelationship, BarcodeObjectClassProp );
                    // ..equal to the given barcode
                    CswNbtViewPropertyFilter BarcodeViewPropertyFilter = LocationView.AddViewPropertyFilter( BarcodeViewProperty, CswEnumNbtSubFieldName.Barcode, CswEnumNbtFilterMode.Equals, LocationBarcode, false );

                    ICswNbtTree LocationTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, LocationView, true, false, false );
                    if( LocationTree.getChildNodeCount() > 0 )
                    {
                        LocationTree.goToNthChild( 0 );
                        CswNbtNode LocationNode = LocationTree.getNodeForCurrentPosition();
                        LocationNodeId = LocationNode.NodeId;
                    }
                }
            } // if(!string.IsNullOrEmpty(LocationNodeIdStr))
            return LocationNodeId;
        } // _HandleReference()

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, CachedPath );
        }

    }//CswNbtNodePropLocation
}//namespace ChemSW.Nbt.PropTypes
