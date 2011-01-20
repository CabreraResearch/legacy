using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

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
                if( value != null )
                    NewValue = value.PrimaryKey;

                if( _CswNbtNodePropData.SetPropRowValue( _NodeIdSubField.Column, CswConvert.ToDbVal( NewValue ) ) )
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
                if( _CswNbtNodePropData.SetPropRowValue( _RowSubField.Column, CswConvert.ToDbVal( value ) ) )
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
                _CswNbtNodePropData.SetPropRowValue( _ColumnSubField.Column, CswConvert.ToDbVal( value ) );
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

        public CswNbtView View
        {
            get
            {
                CswNbtMetaDataObjectClass LocationClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp LocationClassProp = LocationClass.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

                CswNbtView LocationView = new CswNbtView( _CswNbtResources );
                LocationView.ViewName = "Locations";

                CswNbtViewRelationship LocationLevel1 = LocationView.AddViewRelationship( LocationClass, true );
                if( NodeId != null )
                    LocationLevel1.NodeIdsToFilterOut.Add( NodeId );

                // Only Locations with null parent locations at the root
                CswNbtViewProperty LocationViewProp = LocationView.AddViewProperty( LocationLevel1, LocationClassProp );
                CswNbtViewPropertyFilter LocationViewPropNull = LocationView.AddViewPropertyFilter( LocationViewProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Null, string.Empty, false );

                Int32 MaxDepth = 5;
                if( CswTools.IsInteger( _CswNbtResources.getConfigVariableValue( "loc_max_depth" ) ) )
                    MaxDepth = CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "loc_max_depth" ) );

                CswNbtViewRelationship PriorLocationLevel = LocationLevel1;
                for( int i = 2; i <= MaxDepth; i++ )
                {
                    CswNbtViewRelationship LocationLevelX = LocationView.AddViewRelationship( PriorLocationLevel, CswNbtViewRelationship.PropOwnerType.Second, LocationClassProp, true );
                    if( NodeId != null )
                        LocationLevelX.NodeIdsToFilterOut.Add( NodeId );
                    PriorLocationLevel = LocationLevelX;
                }

                return LocationView;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedNodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _NodeIdSubField.ToXmlNodeName() );
            XmlNode SelectedColumnNode = CswXmlDocument.AppendXmlNode( ParentNode, _ColumnSubField.ToXmlNodeName() );
            XmlNode SelectedRowNode = CswXmlDocument.AppendXmlNode( ParentNode, _RowSubField.ToXmlNodeName() );
            XmlNode CachedNodeNameNode = CswXmlDocument.AppendXmlNode( ParentNode, _NameSubField.ToXmlNodeName(), CachedNodeName );
            XmlNode CachedPathNode = CswXmlDocument.AppendXmlNode( ParentNode, _PathSubField.ToXmlNodeName(), CachedPath );
            XmlNode CachedBarcodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _BarcodeSubField.ToXmlNodeName(), CachedBarcode );

            if( SelectedNodeId != null )
                SelectedNodeNode.InnerText = SelectedNodeId.PrimaryKey.ToString();
            if( SelectedColumn != Int32.MinValue )
                SelectedColumnNode.InnerText = SelectedColumn.ToString();
            if( SelectedRow != Int32.MinValue )
                SelectedRowNode.InnerText = SelectedRow.ToString();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //SelectedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _NodeIdSubField.ToXmlNodeName() ), CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() ), NodeMap ) );
            Int32 LocationNodeId = _HandleReference( CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _NodeIdSubField.ToXmlNodeName() ), CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() ) );
            if( NodeMap != null && NodeMap.ContainsKey( LocationNodeId ) )
                LocationNodeId = NodeMap[LocationNodeId];
            SelectedNodeId = new CswPrimaryKey( "nodes", LocationNodeId );

            CswXmlDocument.AppendXmlAttribute( XmlNode, "destnodeid", SelectedNodeId.PrimaryKey.ToString() );
            SelectedRow = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _RowSubField.ToXmlNodeName() );
            SelectedColumn = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _ColumnSubField.ToXmlNodeName() );
            PendingUpdate = true;
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //SelectedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswConvert.ToInt32(PropRow[_NodeIdSubField.ToXmlNodeName()]), PropRow[_BarcodeSubField.ToXmlNodeName()].ToString(), NodeMap ) );

            string NodeId = CswTools.XmlRealAttributeName( PropRow[_NodeIdSubField.ToXmlNodeName()].ToString() );
            if( NodeMap != null && NodeMap.ContainsKey( NodeId.ToLower() ) )
                SelectedNodeId = new CswPrimaryKey( "nodes", NodeMap[NodeId.ToLower()] );
            else if( CswTools.IsInteger( NodeId ) )
            {
                //RelatedNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeId ) );
                Int32 LocationNodeId = Int32.MinValue;
                if( PropRow.Table.Columns.Contains( _BarcodeSubField.ToXmlNodeName() ) )
                    LocationNodeId = _HandleReference( CswConvert.ToInt32( NodeId ), CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() ) );
                else
                    LocationNodeId = _HandleReference( CswConvert.ToInt32( NodeId ), string.Empty );
                SelectedNodeId = new CswPrimaryKey( "nodes", LocationNodeId );
            }
            else
                SelectedNodeId = null;

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

        private Int32 _HandleReference( Int32 LocationNodeId, string LocationBarcode ) //, Dictionary<Int32, Int32> NodeMap )
        {
            //Int32 ret = Int32.MinValue;
            if( LocationNodeId == Int32.MinValue && LocationBarcode != string.Empty )
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
                    LocationNodeId = LocationNode.NodeId.PrimaryKey;
                }
            }
            return LocationNodeId;
        }

    }//CswNbtNodePropLocation
}//namespace ChemSW.Nbt.PropTypes
