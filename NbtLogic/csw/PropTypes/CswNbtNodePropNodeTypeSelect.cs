using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Prop Class for NodeTypeSelect Properties
    /// </summary>
    public class CswNbtNodePropNodeTypeSelect : CswNbtNodeProp
    {
        public static char delimiter = ',';

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropNodeTypeSelect( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _SelectedNodeTypeIdsSubField = ( (CswNbtFieldTypeRuleNodeTypeSelect) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SelectedNodeTypeIdsSubField;
        }//ctor

        private CswNbtSubField _SelectedNodeTypeIdsSubField;

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        override public bool Empty
        {
            get
            {
                return ( 0 == SelectedNodeTypeIds.Count );
            }
        }

        /// <summary>
        /// Text value of property
        /// </summary>
        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        private CswCommaDelimitedString _SelectedNodeTypeIds = null;
        /// <summary>
        /// Comma-separated list of Selected NodeTypeIds
        /// </summary>
        public CswCommaDelimitedString SelectedNodeTypeIds
        {
            get
            {
                if( _SelectedNodeTypeIds == null )
                {
                    _SelectedNodeTypeIds = new CswCommaDelimitedString();
                    _SelectedNodeTypeIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedNodeTypeIds_OnChange );
                    _SelectedNodeTypeIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedNodeTypeIdsSubField.Column ) );
                }
                return _SelectedNodeTypeIds;
            }
            set
            {
                _SelectedNodeTypeIds = value;
                _SelectedNodeTypeIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        private void _SelectedNodeTypeIds_OnChange()
        {
            if( _CswNbtNodePropData.SetPropRowValue( _SelectedNodeTypeIdsSubField.Column, _SelectedNodeTypeIds.ToString() ) )
            {
                // BZ 10094 - this caused Notification names to populate in the background, which was confusing
                // PendingUpdate = true;
                RefreshSelectedNodeTypeNames();
            }
        }

        /// <summary>
        /// Refresh the names of the selected nodetypes
        /// </summary>
        public void RefreshSelectedNodeTypeNames()
        {
            _CswNbtNodePropData.Gestalt = SelectedNodeTypeNames().ToString();
            PendingUpdate = false;
        }

        /// <summary>
        /// Mode of operation for this NodeTypeSelect property
        /// </summary>
        public PropertySelectMode SelectMode
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Multi;
            }
        }

        public DataTable Options
        {
            get
            {
                DataTable Data = new CswDataTable( TableName, "" );
                Data.Columns.Add( NameColumn, typeof( string ) );
                Data.Columns.Add( KeyColumn, typeof( int ) );
                Data.Columns.Add( ValueColumn, typeof( bool ) );

                if( SelectMode != PropertySelectMode.Multiple && !Required )
                {
                    DataRow NTRow = Data.NewRow();
                    NTRow[NameColumn] = "[none]";
                    NTRow[KeyColumn] = CswConvert.ToDbVal( Int32.MinValue );
                    NTRow[ValueColumn] = ( SelectedNodeTypeIds.Count == 0 );
                    Data.Rows.Add( NTRow );
                }

                bool first = true;
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
                {
                    DataRow NTRow = Data.NewRow();
                    NTRow[NameColumn] = NodeType.NodeTypeName;          // latest name
                    NTRow[KeyColumn] = NodeType.FirstVersionNodeTypeId;   // first nodetypeid
                    NTRow[ValueColumn] = ( SelectedNodeTypeIds.Contains( NodeType.FirstVersionNodeTypeId.ToString() ) ||
                                         ( first && Required && SelectedNodeTypeIds.Count == 0 ) );
                    Data.Rows.Add( NTRow );
                    first = false;
                }
                return Data;
            }
        }

        public const string NameColumn = "NodeTypeName";
        public const string KeyColumn = "nodetypeid";
        public const string ValueColumn = "Include";
        public const string TableName = "nodetypeselectdatatable";

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedNTsNode = CswXmlDocument.AppendXmlNode( ParentNode, _SelectedNodeTypeIdsSubField.ToXmlNodeName(), SelectedNodeTypeIds.ToString() );
            CswXmlDocument.AppendXmlAttribute( SelectedNTsNode, "SelectMode", SelectMode.ToString() );
            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "Options" );

            DataTable Data = Options;
            foreach( DataRow Row in Data.Rows )
            {
                XmlNode ItemNode = CswXmlDocument.AppendXmlNode( OptionsNode, "item" );
                foreach( DataColumn Column in Data.Columns )
                {
                    XmlNode ColumnNode = CswXmlDocument.AppendXmlNode( ItemNode, "column" );
                    CswXmlDocument.AppendXmlAttribute( ColumnNode, "field", Column.ColumnName );
                    CswXmlDocument.AppendXmlAttribute( ColumnNode, "value", Row[Column].ToString() );
                }
            }
        }

        public override void ToXElement( XElement ParentNode )
        {
            //Not yet implemented
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _SelectedNodeTypeIdsSubField.ToXmlNodeName().ToLower(), SelectedNodeTypeIds.ToString() ) );
            ParentObject.Add( new JProperty( "selectmode", SelectMode.ToString() ) );

            JArray OptionsAry = new JArray();
            JProperty OptionsNode = new JProperty( "options", OptionsAry );
            ParentObject.Add( OptionsNode );

            DataTable Data = Options;
            foreach( DataRow Row in Data.Rows )
            {
                JObject OptionObj = new JObject();
                OptionsAry.Add( OptionObj );
                foreach( DataColumn Column in Data.Columns )
                {
                    OptionObj.Add( new JProperty( Column.ColumnName, Row[Column].ToString() ) );
                }
            }
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //SelectedNodeTypeIds.FromString( _HandleReferences( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedNodeTypeIdsSubField.ToXmlNodeName() ), NodeTypeMap ) );
            CswCommaDelimitedString NewSelectedNodeTypeIds = new CswCommaDelimitedString();

            foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "Options" ).ChildNodes )
            {
                string key = string.Empty;
                string name = string.Empty;
                bool value = false;
                foreach( XmlNode ColumnNode in ItemNode.ChildNodes )
                {
                    if( KeyColumn == ColumnNode.Attributes["field"].Value )
                        key = ColumnNode.Attributes["value"].Value;
                    if( NameColumn == ColumnNode.Attributes["field"].Value )
                        name = ColumnNode.Attributes["value"].Value;
                    if( ValueColumn == ColumnNode.Attributes["field"].Value )
                        value = CswConvert.ToBoolean( ColumnNode.Attributes["value"].Value );
                }
                if( value )
                {
                    NewSelectedNodeTypeIds.Add( key );
                }
            } // foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "Options" ).ChildNodes )

            SelectedNodeTypeIds = NewSelectedNodeTypeIds;
        } // ReadXml()

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            //Not yet implemented
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedNodeTypeIds.FromString( _HandleReferences( CswTools.XmlRealAttributeName( PropRow[_SelectedNodeTypeIdsSubField.ToXmlNodeName()].ToString() ), NodeTypeMap ) );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //SelectedNodeTypeIds.FromString( _HandleReferences( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedNodeTypeIdsSubField.ToXmlNodeName() ), NodeTypeMap ) );
            CswCommaDelimitedString NewSelectedNodeTypeIds = new CswCommaDelimitedString();

            if( null != JObject["options"] )
            {
                JArray OptionsAry = (JArray) JObject["options"];

                foreach( JObject ItemObj in OptionsAry )
                {
                    string key = string.Empty;
                    string name = string.Empty;
                    bool value = false;
                    foreach( JProperty ColumnNode in ItemObj.Properties() )
                    {
                        if( KeyColumn == ColumnNode.Name )
                        {
                            key = CswConvert.ToString( ColumnNode.Value );
                        }
                        if( NameColumn == ColumnNode.Name )
                        {
                            name = CswConvert.ToString( ColumnNode.Value );
                        }
                        if( ValueColumn == ColumnNode.Name )
                        {
                            value = CswConvert.ToBoolean( ColumnNode.Value );
                        }
                    }
                    if( value )
                    {
                        NewSelectedNodeTypeIds.Add( key );
                    }
                } // foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "Options" ).ChildNodes )

                SelectedNodeTypeIds = NewSelectedNodeTypeIds;
            }
        }

        private string _HandleReferences( string NodeTypeIds, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ret = NodeTypeIds;
            if( NodeTypeMap != null )
            {
                ret = "";
                string[] NodeTypeIdsArray = NodeTypeIds.Split( ',' );
                foreach( string NodeTypeIdString in NodeTypeIdsArray )
                {
                    Int32 IdToAdd = CswConvert.ToInt32( NodeTypeIdString );
                    if( NodeTypeMap.ContainsKey( IdToAdd ) )
                        IdToAdd = CswConvert.ToInt32( NodeTypeMap[IdToAdd].ToString() );

                    if( ret != "" )
                        ret += ",";
                    ret += IdToAdd.ToString();
                }
            }
            return ret;
        }


        public CswCommaDelimitedString SelectedNodeTypeNames()
        {
            CswCommaDelimitedString NodeTypeNames = new CswCommaDelimitedString();
            foreach( string NodeTypeId in SelectedNodeTypeIds )
            {

                if( string.Empty != NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
                    if( null != NodeType )
                    {
                        NodeTypeNames.Add( NodeType.LatestVersionNodeType.NodeTypeName );
                    }
                }
            } // foreach(string NodeTypeId in SelectedNodeTypeIds)
            if( 0 == NodeTypeNames.Count )
            {
                NodeTypeNames.Add( "Select new NodeType" );
            }

            // Sort alphabetically
            NodeTypeNames.Sort();

            return NodeTypeNames;
        } // SelectedNodeTypeNames()

    }//CswNbtNodePropNodeTypeSelect
}//namespace ChemSW.Nbt.PropTypes
