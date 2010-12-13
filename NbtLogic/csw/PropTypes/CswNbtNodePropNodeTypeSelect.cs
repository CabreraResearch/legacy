using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;

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

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedNTsNode = CswXmlDocument.AppendXmlNode( ParentNode, _SelectedNodeTypeIdsSubField.ToXmlNodeName(), SelectedNodeTypeIds.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedNodeTypeIds.FromString( _HandleReferences( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedNodeTypeIdsSubField.ToXmlNodeName() ), NodeTypeMap ) );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedNodeTypeIds.FromString( _HandleReferences( CswTools.XmlRealAttributeName( PropRow[_SelectedNodeTypeIdsSubField.ToXmlNodeName()].ToString() ), NodeTypeMap ) );
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
                    Int32 IdToAdd = Convert.ToInt32( NodeTypeIdString );
                    if( NodeTypeMap.ContainsKey( IdToAdd ) )
                        IdToAdd = Convert.ToInt32( NodeTypeMap[IdToAdd].ToString() );

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
            foreach(string NodeTypeId in SelectedNodeTypeIds)
            {
                if( NodeTypeId.ToString() != string.Empty )
                {
                    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )
                    {
                        if( NodeType.NodeTypeId.ToString() == NodeTypeId.ToString() )
                        {
                            NodeTypeNames.Add( NodeType.LatestVersionNodeType.NodeTypeName );
                        }
                    }
                }
            } // foreach(string NodeTypeId in SelectedNodeTypeIds)

            // Sort alphabetically
            NodeTypeNames.Sort();

            return NodeTypeNames;
        } // SelectedNodeTypeNames()

    }//CswNbtNodePropNodeTypeSelect
}//namespace ChemSW.Nbt.PropTypes
