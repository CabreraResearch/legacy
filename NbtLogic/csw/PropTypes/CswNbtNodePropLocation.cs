using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropLocation : CswNbtNodeProp
    {
        public CswNbtNodePropLocation( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _NameSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).NameSubField;
            _NodeIdSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).NodeIdSubField;
            _RowSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).RowSubField;
            _ColumnSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ColumnSubField;
            _PathSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).PathSubField;
            _BarcodeSubField = ( (CswNbtFieldTypeRuleLocation) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).BarcodeSubField;
        }

        private CswNbtSubField _NameSubField;
        private CswNbtSubField _NodeIdSubField;
        private CswNbtSubField _RowSubField;
        private CswNbtSubField _ColumnSubField;
        private CswNbtSubField _PathSubField;
        private CswNbtSubField _BarcodeSubField;

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
                    NewValue = value.PrimaryKey;

                if( _CswNbtNodePropData.SetPropRowValue( _NodeIdSubField.Column, NewValue ) )
                    PendingUpdate = true;
            }
        }
        public string CachedNodeName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _NameSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _NameSubField.Column, value );
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
                _CswNbtNodePropData.SetPropRowValue( _PathSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        public string CachedBarcode
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _BarcodeSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _BarcodeSubField.Column, value );
            }
        }

        public Int32 SelectedRow
        {
            get
            {
                Int32 ret = Int32.MinValue;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _RowSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                    ret = CswConvert.ToInt32( StringVal );
                return ret;
            }
            set
            {
                if( _CswNbtNodePropData.SetPropRowValue( _RowSubField.Column, value ) )
                    PendingUpdate = true;
            }
        }
        public Int32 SelectedColumn
        {
            get
            {
                Int32 ret = Int32.MinValue;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _ColumnSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                    ret = CswConvert.ToInt32( StringVal );
                return ret;
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ColumnSubField.Column, value );
                PendingUpdate = true;
            }
        }

        public void RefreshNodeName()
        {
            if( SelectedNodeId != null )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( SelectedNodeId );
                CswNbtObjClassLocation NodeAsLocation = (CswNbtObjClassLocation) CswNbtNodeCaster.AsLocation( Node );
                CachedNodeName = Node.NodeName;
                CachedPath = _generateLocationPath( Node );
                CachedBarcode = NodeAsLocation.Barcode.Barcode;
            }
            else
            {
                CachedNodeName = CswNbtLocationTree.TopLevelName;
                CachedPath = CachedNodeName;
                CachedBarcode = string.Empty;
            }
            this.PendingUpdate = false;
        }

        private string _pathdelimiter = " > ";
        private string _generateLocationPath( CswNbtNode Node )
        {
            string ret;
            ret = Node.NodeName;
            CswNbtObjClassLocation NodeAsLocation = (CswNbtObjClassLocation) CswNbtNodeCaster.AsLocation( Node );
            if( NodeAsLocation.Location.SelectedNodeId != null )
            {
                string prev = _generateLocationPath( _CswNbtResources.Nodes[NodeAsLocation.Location.SelectedNodeId] );
                if( prev != string.Empty )
                    ret = prev + _pathdelimiter + ret;
            }
            else
            {
                // BZ 9133
                //ret = CswNbtLocationTree.TopLevelName + _pathdelimiter + ret;
            }
            return ret;
        }

        private CswNbtView _View = null;
        public CswNbtView View
        {
            get
            {
                if( _View == null )
                {
                    CswNbtMetaDataObjectClass LocationClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                    CswNbtMetaDataObjectClassProp LocationClassProp = LocationClass.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

                    _View = new CswNbtView( _CswNbtResources );
                    _View.ViewName = "Top";

                    CswNbtViewRelationship LocationLevel1 = _View.AddViewRelationship( LocationClass, true );
                    if( NodeId != null )
                        LocationLevel1.NodeIdsToFilterOut.Add( NodeId );

                    // Only Locations with null parent locations at the root
                    CswNbtViewProperty LocationViewProp = _View.AddViewProperty( LocationLevel1, LocationClassProp );
                    CswNbtViewPropertyFilter LocationViewPropNull = _View.AddViewPropertyFilter( LocationViewProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Null, string.Empty, false );

                    Int32 MaxDepth = 5;
                    if( CswTools.IsInteger( _CswNbtResources.getConfigVariableValue( "loc_max_depth" ) ) )
                        MaxDepth = CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "loc_max_depth" ) );

                    CswNbtViewRelationship PriorLocationLevel = LocationLevel1;
                    for( int i = 2; i <= MaxDepth; i++ )
                    {
                        CswNbtViewRelationship LocationLevelX = _View.AddViewRelationship( PriorLocationLevel, CswNbtViewRelationship.PropOwnerType.Second, LocationClassProp, true );
                        if( NodeId != null )
                            LocationLevelX.NodeIdsToFilterOut.Add( NodeId );
                        PriorLocationLevel = LocationLevelX;
                    }
                } // if( _View == null )
                return _View;
            } // get
        } // View

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedNodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _NodeIdSubField.ToXmlNodeName() );
            if( SelectedNodeId != null )
                SelectedNodeNode.InnerText = SelectedNodeId.ToString();

            XmlNode SelectedColumnNode = CswXmlDocument.AppendXmlNode( ParentNode, _ColumnSubField.ToXmlNodeName() );
            if( SelectedColumn != Int32.MinValue )
                SelectedColumnNode.InnerText = SelectedColumn.ToString();

            XmlNode SelectedRowNode = CswXmlDocument.AppendXmlNode( ParentNode, _RowSubField.ToXmlNodeName() );
            if( SelectedRow != Int32.MinValue )
                SelectedRowNode.InnerText = SelectedRow.ToString();

            CswXmlDocument.AppendXmlNode( ParentNode, _NameSubField.ToXmlNodeName(), CachedNodeName );
            CswXmlDocument.AppendXmlNode( ParentNode, _PathSubField.ToXmlNodeName(), CachedPath );
            CswXmlDocument.AppendXmlNode( ParentNode, _BarcodeSubField.ToXmlNodeName(), CachedBarcode );

            View.SaveToCache( false );
            CswXmlDocument.AppendXmlNode( ParentNode, "viewid", View.SessionViewId.ToString() );

            if( NodeId != null && NodeId.PrimaryKey != Int32.MinValue )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                CswNbtNodeKey NodeKey = Tree.getNodeKeyByNodeId( NodeId );
                if( NodeKey != null )
                {
                    CswXmlDocument.AppendXmlNode( ParentNode, "nodekey", NodeKey.ToString() );
                }
            }
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _NodeIdSubField.ToXmlNodeName( true ), ( SelectedNodeId != null ) ? SelectedNodeId.ToString() : string.Empty ),
                            new XElement( _ColumnSubField.ToXmlNodeName( true ), ( SelectedColumn != Int32.MinValue ) ? SelectedColumn.ToString() : string.Empty ),
                            new XElement( _RowSubField.ToXmlNodeName( true ), ( SelectedRow != Int32.MinValue ) ? SelectedRow.ToString() : string.Empty ),
                            new XElement( _NameSubField.ToXmlNodeName( true ), CachedNodeName ),
                            new XElement( _PathSubField.ToXmlNodeName( true ), CachedPath ),
                            new XElement( _BarcodeSubField.ToXmlNodeName( true ), CachedBarcode ) );

            View.SaveToCache( false );
            ParentNode.Add( new XElement( "viewid", View.SessionViewId.ToString() ) );

            if( NodeId != null && NodeId.PrimaryKey != Int32.MinValue )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                CswNbtNodeKey NodeKey = Tree.getNodeKeyByNodeId( NodeId );
                if( NodeKey != null )
                {
                    ParentNode.Add( new XElement( "nodekey", NodeKey.ToString() ) );
                }
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _NodeIdSubField.ToXmlNodeName( true ), ( SelectedNodeId != null ) ? SelectedNodeId.ToString() : string.Empty ) );
            ParentObject.Add( new JProperty( _ColumnSubField.ToXmlNodeName( true ), ( SelectedColumn != Int32.MinValue ) ? SelectedColumn.ToString() : string.Empty ) );
            ParentObject.Add( new JProperty( _RowSubField.ToXmlNodeName( true ), ( SelectedRow != Int32.MinValue ) ? SelectedRow.ToString() : string.Empty ) );
            ParentObject.Add( new JProperty( _NameSubField.ToXmlNodeName( true ), CachedNodeName ) );
            ParentObject.Add( new JProperty( _PathSubField.ToXmlNodeName( true ), CachedPath ) );
            ParentObject.Add( new JProperty( _BarcodeSubField.ToXmlNodeName( true ), CachedBarcode ) );

            View.SaveToCache( false );
            ParentObject.Add( new JProperty( "viewid", View.SessionViewId.ToString() ) );

            if( NodeId != null && NodeId.PrimaryKey != Int32.MinValue )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                CswNbtNodeKey NodeKey = Tree.getNodeKeyByNodeId( NodeId );
                if( NodeKey != null )
                {
                    ParentObject.Add( new JProperty( "nodekey", NodeKey.ToString() ) );
                }
            }
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //SelectedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _NodeIdSubField.ToXmlNodeName() ), CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() ), NodeMap ) );
            string LocationNodeIdStr = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _NodeIdSubField.ToXmlNodeName() );
            string LocationBarcode = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() );
            Int32 Row = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _RowSubField.ToXmlNodeName() );
            Int32 Column = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _ColumnSubField.ToXmlNodeName() );

            string SelectedNodeId = _saveProp( LocationNodeIdStr, LocationBarcode, NodeMap, Row, Column );
            if( !string.IsNullOrEmpty( SelectedNodeId ) )
            {
                CswXmlDocument.AppendXmlAttribute( XmlNode, "destnodeid", SelectedNodeId );
            }
        } // ReadXml()



        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            string LocationNodeIdStr = string.Empty;
            string LocationBarcode = string.Empty;
            Int32 Row = Int32.MinValue;
            Int32 Column = Int32.MinValue;
            if( null != XmlNode.Element( _NodeIdSubField.ToXmlNodeName( true ) ) )
            {
                LocationNodeIdStr = XmlNode.Element( _NodeIdSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _BarcodeSubField.ToXmlNodeName( true ) ) )
            {
                LocationBarcode = XmlNode.Element( _BarcodeSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _RowSubField.ToXmlNodeName( true ) ) )
            {
                Row = CswConvert.ToInt32( XmlNode.Element( _RowSubField.ToXmlNodeName( true ) ).Value );
            }
            if( null != XmlNode.Element( _ColumnSubField.ToXmlNodeName( true ) ) )
            {
                Column = CswConvert.ToInt32( XmlNode.Element( _ColumnSubField.ToXmlNodeName( true ) ).Value );
            }
            string SelectedNodeId = _saveProp( LocationNodeIdStr, LocationBarcode, NodeMap, Row, Column );
            if( !string.IsNullOrEmpty( SelectedNodeId ) )
            {
                XmlNode.Add( new XElement( "destnodeid", SelectedNodeId ) );
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string LocationNodeIdStr = string.Empty;
            string LocationBarcode = string.Empty;
            Int32 Row = Int32.MinValue;
            Int32 Column = Int32.MinValue;
            if( null != JObject.Property( ( _NodeIdSubField.ToXmlNodeName( true ) ) ) )
            {
                LocationNodeIdStr = (string) JObject.Property( _NodeIdSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != JObject.Property( _BarcodeSubField.ToXmlNodeName( true ) ) )
            {
                LocationBarcode = (string) JObject.Property( _BarcodeSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != JObject.Property( _RowSubField.ToXmlNodeName( true ) ) )
            {
                Row = CswConvert.ToInt32( JObject.Property( _RowSubField.ToXmlNodeName( true ) ).Value );
            }
            if( null != JObject.Property( _ColumnSubField.ToXmlNodeName( true ) ) )
            {
                Column = CswConvert.ToInt32( JObject.Property( _ColumnSubField.ToXmlNodeName( true ) ).Value );
            }
            string SelectedNodeId = _saveProp( LocationNodeIdStr, LocationBarcode, NodeMap, Row, Column );
            if( !string.IsNullOrEmpty( SelectedNodeId ) )
            {
                JObject.Add( new JProperty( "destnodeid", SelectedNodeId ) );
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

            if( SelectedNodeId != null )
            {
                PropRow["destnodeid"] = SelectedNodeId.PrimaryKey;
                PendingUpdate = true;
            }

            if( PropRow.Table.Columns.Contains( _RowSubField.ToXmlNodeName() ) )
            {
                string StringVal = PropRow[_RowSubField.ToXmlNodeName()].ToString();
                if( CswTools.IsInteger( StringVal ) )
                    SelectedRow = CswConvert.ToInt32( StringVal );
            }

            if( PropRow.Table.Columns.Contains( _ColumnSubField.ToXmlNodeName() ) )
            {
                string StringVal = PropRow[_ColumnSubField.ToXmlNodeName()].ToString();
                if( CswTools.IsInteger( StringVal ) )
                    SelectedColumn = CswConvert.ToInt32( StringVal );
            }
            PendingUpdate = true;
        }

        private string _saveProp( string LocationNodeIdStr, string LocationBarcode, Dictionary<Int32, Int32> NodeMap, Int32 Row, Int32 Column )
        {
            string RetVal = string.Empty;
            CswPrimaryKey LocationNodeId = _HandleReference( LocationNodeIdStr, LocationBarcode );
            if( NodeMap != null && NodeMap.ContainsKey( LocationNodeId.PrimaryKey ) )
                LocationNodeId = new CswPrimaryKey( "nodes", NodeMap[LocationNodeId.PrimaryKey] );
            SelectedNodeId = LocationNodeId;
            if( SelectedNodeId != null )
            {
                SelectedRow = Row;
                SelectedColumn = Column;
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
                    CswNbtMetaDataObjectClass LocationObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                    CswNbtMetaDataObjectClassProp BarcodeObjectClassProp = LocationObjectClass.getObjectClassProp( CswNbtObjClassLocation.BarcodePropertyName );

                    CswNbtView LocationView = new CswNbtView( _CswNbtResources );
                    // All locations..
                    CswNbtViewRelationship LocationRelationship = LocationView.AddViewRelationship( LocationObjectClass, false );
                    // ..with barcodes
                    CswNbtViewProperty BarcodeViewProperty = LocationView.AddViewProperty( LocationRelationship, BarcodeObjectClassProp );
                    // ..equal to the given barcode
                    CswNbtViewPropertyFilter BarcodeViewPropertyFilter = LocationView.AddViewPropertyFilter( BarcodeViewProperty, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, LocationBarcode, false );

                    ICswNbtTree LocationTree = _CswNbtResources.Trees.getTreeFromView( LocationView, true, true, false, false );
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

    }//CswNbtNodePropLocation
}//namespace ChemSW.Nbt.PropTypes
