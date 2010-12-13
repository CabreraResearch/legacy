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
                    _SelectedNodeTypeIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedNodeTypeIdsSubField.Column ) );
                }
                return _SelectedNodeTypeIds;
            }
            set
            {
                _SelectedNodeTypeIds = value;
                if( _CswNbtNodePropData.SetPropRowValue( _SelectedNodeTypeIdsSubField.Column, value ) )
                {
                    // BZ 10094 - this caused Notification names to populate in the background, which was confusing
                    // PendingUpdate = true;
                    RefreshSelectedNodeTypeNames();
                }
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
            //string[] NodeTypeIdArray = SelectedNodeTypeIds.Split( delimiter );
            string[] SelectedNodeTypeNames = new string[SelectedNodeTypeIds.Count];
            Int32 n = 0;
            foreach(string NodeTypeId in SelectedNodeTypeIds)
            {
                if( NodeTypeId.ToString() != string.Empty )
                {
                    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.NodeTypes )
                    {
                        if( NodeType.NodeTypeId.ToString() == NodeTypeId.ToString() )
                        {
                            SelectedNodeTypeNames[n] = NodeType.LatestVersionNodeType.NodeTypeName;
                            n++;
                        }
                    }
                }
            }

            // Sort alphabetically
            Array.Sort( SelectedNodeTypeNames );
            CswCommaDelimitedString NodeTypeNames = new CswCommaDelimitedString();
            for( int i = 0; i < SelectedNodeTypeNames.Length; i++ )
            {
                NodeTypeNames.Add(SelectedNodeTypeNames[i]);
            }
            return NodeTypeNames;
        }

    }//CswNbtNodePropNodeTypeSelect
}//namespace ChemSW.Nbt.PropTypes
