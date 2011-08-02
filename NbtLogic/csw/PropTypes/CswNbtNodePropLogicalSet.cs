﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Represents a set of Logicals together in one property
    /// </summary>
    public class CswNbtNodePropLogicalSet : CswNbtNodeProp
    {
        private XmlDocument LogicalSetXmlDoc;

        /// <summary>
        /// Represents the data of a set of checkboxes
        /// </summary>
        public CswNbtNodePropLogicalSet( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( Empty )
            {
                ResetXml();
            }
            else
            {
                LogicalSetXmlDoc = new XmlDocument();
                LogicalSetXmlDoc.LoadXml( _CswNbtNodePropData.ClobData );
            }

        }//ctor

        private void ResetXml()
        {
            LogicalSetXmlDoc = new XmlDocument();
            XmlElement LogicalSetRootNode = LogicalSetXmlDoc.CreateElement( "LogicalSet" );
            LogicalSetXmlDoc.AppendChild( LogicalSetRootNode );
        }

        /// <summary>
        /// True if the XML is empty
        /// </summary>
        override public bool Empty
        {
            get
            {
                return ( 0 == _CswNbtNodePropData.ClobData.Length );
            }
        }

        /// <summary>
        /// String representation of checked values
        /// </summary>
        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                    return 4;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
        }

        /// <summary>
        /// Overrides the XML with custom XML.  For use by CswSchemaUpdater.
        /// </summary>
        public void setXml( string XmlString )
        {
            if( XmlString != string.Empty )
                LogicalSetXmlDoc.LoadXml( XmlString );
            else
                ResetXml();
            Save();
        }


        /// <summary>
        /// Saves the value of this property
        /// </summary>
        public void Save()
        {
            _CswNbtNodePropData.ClobData = LogicalSetXmlDoc.InnerXml.ToString();
            RefreshStringValue();
            //PendingUpdate = true;
        }

        public void RefreshStringValue()
        {
            string CheckedNames = string.Empty;
            DataTable Data = GetDataAsTable( "Name", "Key" );
            if( XValues.Count == 1 )
            {
                foreach( DataRow Row in Data.Rows )
                {
                    if( CswConvert.ToBoolean( Row[XValues[0]] ) )
                    {
                        if( CheckedNames != string.Empty ) CheckedNames += ",";
                        CheckedNames += Row["Name"].ToString();
                    }
                }
            }
            else
            {
                foreach( string XValue in XValues )
                {
                    if( CheckedNames != string.Empty ) CheckedNames += ";";
                    CheckedNames += XValue + ":";
                    string ThisCheckedNames = string.Empty;
                    foreach( DataRow Row in Data.Rows )
                    {
                        if( CswConvert.ToBoolean( Row[XValues[0]] ) )
                        {
                            if( ThisCheckedNames != string.Empty ) ThisCheckedNames += ",";
                            ThisCheckedNames += Row["Name"].ToString();
                        }
                    }
                    CheckedNames += ThisCheckedNames;
                }
            }
            _CswNbtNodePropData.Gestalt = CheckedNames;
            PendingUpdate = false;
        }

        public DataTable GetDataAsTable( string NameColumn, string KeyColumn )
        {
            DataTable Data = new CswDataTable( "GetDataAsTable_DataTable", "" );
            Data.Columns.Add( NameColumn );
            Data.Columns.Add( KeyColumn );

            foreach( string XValue in XValues )
                Data.Columns.Add( XValue, typeof( bool ) );

            if( _CswNbtMetaDataNodeTypeProp.IsFK && _CswNbtMetaDataNodeTypeProp.FKType == "fkeydefid" )
            {
                CswTableSelect FkeyDefsSelect = _CswNbtResources.makeCswTableSelect( "YValues_fkeydef_select", "fkey_definitions" );
                DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", _CswNbtMetaDataNodeTypeProp.FKValue );
                string Sql = FkeyDefsTable.Rows[0]["sql"].ToString();
                CswArbitrarySelect YValuesSelect = _CswNbtResources.makeCswArbitrarySelect( "YValues_select", Sql );
                DataTable YValuesTable = YValuesSelect.getTable();
                foreach( DataRow CurrentYValueRow in YValuesTable.Rows )
                {
                    DataRow Row = Data.NewRow();
                    Row[NameColumn] = CurrentYValueRow[FkeyDefsTable.Rows[0]["ref_column"].ToString()].ToString();
                    Row[KeyColumn] = CurrentYValueRow[FkeyDefsTable.Rows[0]["pk_column"].ToString()].ToString();
                    foreach( string XValue in XValues )
                    {
                        Row[XValue] = CheckValue( XValue, Row[KeyColumn].ToString() );
                    }
                    Data.Rows.Add( Row );
                }
            }
            else
            {
                foreach( string YValue in YValues )
                {
                    DataRow Row = Data.NewRow();
                    Row[NameColumn] = YValue;
                    Row[KeyColumn] = YValue;
                    foreach( string XValue in XValues )
                    {
                        Row[XValue] = CheckValue( XValue, YValue );
                    }
                    Data.Rows.Add( Row );
                }
            }
            return Data;
        }

        public void ResetCachedXYValues()
        {
            _XValues = null;
            _YValues = null;
        }

        private CswCommaDelimitedString _XValues = null;
        public CswCommaDelimitedString XValues
        {
            get
            {
                if( _XValues == null )
                {
                    _XValues = new CswCommaDelimitedString();
                    _XValues.FromString( _CswNbtMetaDataNodeTypeProp.ValueOptions );
                }
                return _XValues;
            }
            set
            {
                // Allows code to override what's in the database
                _XValues = value;
            }
        }

        // This does not return a CswCommaDelimitedString in order to allow business logic to override YValues 
        // with other kinds of delimited strings
        private CswDelimitedString _YValues = null;
        public CswDelimitedString YValues
        {
            get
            {
                if( _YValues == null )
                {
                    _YValues = new CswCommaDelimitedString();
                    if( _CswNbtMetaDataNodeTypeProp.IsFK && _CswNbtMetaDataNodeTypeProp.FKType == "fkeydefid" )
                    {
                        CswTableSelect FkeyDefsSelect = _CswNbtResources.makeCswTableSelect( "LogicalSet_fkeydef_select", "fkey_definitions" );
                        DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", _CswNbtMetaDataNodeTypeProp.FKValue );
                        string Sql = FkeyDefsTable.Rows[0]["sql"].ToString();

                        CswArbitrarySelect YValuesSelect = _CswNbtResources.makeCswArbitrarySelect( "LogicalSet_YValues_select", Sql );
                        DataTable YValuesTable = YValuesSelect.getTable();
                        foreach( DataRow CurrentRow in YValuesTable.Rows )
                        {
                            _YValues.Add( CurrentRow[FkeyDefsTable.Rows[0]["pk_column"].ToString()].ToString().Trim() );
                        }
                    }
                    else
                    {
                        _YValues.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );
                    }
                }
                return _YValues;
            }
            set
            {
                // Allows code to override what's in the database
                _YValues = value;
            }
        }

        /// <summary>
        /// Changes a checkbox for a particular set of values
        /// </summary>
        public void SetValue( string XValue, string YValue, bool Checked )
        {
            XmlNode YValueNode = LogicalSetXmlDoc.ChildNodes[0].SelectSingleNode( "YValue[@y='" + YValue + "']" );
            if( YValueNode == null )
            {
                YValueNode = LogicalSetXmlDoc.CreateElement( "YValue" );
                XmlAttribute YAttribute = LogicalSetXmlDoc.CreateAttribute( "y" );
                YAttribute.Value = YValue;
                YValueNode.Attributes.Append( YAttribute );
                LogicalSetXmlDoc.ChildNodes[0].AppendChild( YValueNode );
            }

            XmlNode XValueNode = YValueNode.SelectSingleNode( XValue );
            if( XValueNode == null )
            {
                XValueNode = LogicalSetXmlDoc.CreateElement( XValue );
                YValueNode.AppendChild( XValueNode );
            }

            if( Checked )
                XValueNode.InnerText = "1";
            else
                XValueNode.InnerText = "0";
        }

        /// <summary>
        /// Returns whether a checkbox is checked for a particular set of values
        /// </summary>
        public bool CheckValue( string XValue, string YValue )
        {
            bool ret = false;
            XmlNode YValueNode = LogicalSetXmlDoc.ChildNodes[0].SelectSingleNode( "YValue[@y='" + YValue + "']" );
            if( YValueNode != null )
            {
                XmlNode XValueNode = YValueNode.SelectSingleNode( XValue );
                if( XValueNode != null )
                    ret = ( XValueNode.InnerText == "1" );
            }
            return ret;
        }


        private string _ElemName_LogicalSetXml = "LogicalSetXml";

        private string _NameColumn = "name";
        private string _KeyColumn = "key";
        //private string _TableName = "logicalsetitem";

        /// <summary>
        /// Convert this data to XML format, and add beneath the given parent node
        /// </summary>
        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode LSXmlNode = CswXmlDocument.AppendXmlNode( ParentNode, _ElemName_LogicalSetXml );

            DataTable Data = GetDataAsTable( _NameColumn, _KeyColumn );
            foreach( DataRow Row in Data.Rows )
            {
                XmlNode ItemNode = CswXmlDocument.AppendXmlNode( LSXmlNode, "item" );
                foreach( DataColumn Column in Data.Columns )
                {
                    XmlNode ColumnNode = CswXmlDocument.AppendXmlNode( ItemNode, "column" );
                    CswXmlDocument.AppendXmlAttribute( ColumnNode, "field", Column.ColumnName );
                    CswXmlDocument.AppendXmlAttribute( ColumnNode, "value", Row[Column].ToString() );
                }
            }
        } // ToXml()

        public override void ToXElement( XElement ParentNode )
        {
            XElement LSXmlNode = new XElement( _ElemName_LogicalSetXml.ToLower() );
            ParentNode.Add( LSXmlNode );

            DataTable Data = GetDataAsTable( _NameColumn, _KeyColumn );
            foreach( DataRow Row in Data.Rows )
            {
                XElement ItemNode = new XElement( "item" );
                LSXmlNode.Add( ItemNode );
                foreach( DataColumn Column in Data.Columns )
                {
                    ItemNode.Add( new XElement( "column",
                        new XAttribute( "field", Column.ColumnName ),
                        new XAttribute( "value", Row[Column].ToString() ) ) );
                }
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            JProperty LSXmlNode = new JProperty( _ElemName_LogicalSetXml.ToLower() );
            ParentObject.Add( LSXmlNode );

            JObject LSXmlNodeObj = new JObject();
            LSXmlNode.Value = LSXmlNodeObj;

            DataTable Data = GetDataAsTable( _NameColumn, _KeyColumn );
            foreach( DataRow Row in Data.Rows )
            {
                JProperty ItemNode = new JProperty( "item" );
                LSXmlNodeObj.Add( ItemNode );

                JObject ItemNodeObj = new JObject();
                ItemNode.Value = ItemNodeObj;
                foreach( DataColumn Column in Data.Columns )
                {
                    ItemNodeObj.Add( new JProperty( "column",
                        new JProperty( "field", Column.ColumnName ),
                        new JProperty( "value", Row[Column].ToString() ) ) );
                }
            }
        }

        /// <summary>
        /// Initialize this object with data from the given XmlNode
        /// </summary>
        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {

            foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, _ElemName_LogicalSetXml ).ChildNodes )
            {
                // get key and name
                string key = string.Empty;
                string name = string.Empty;
                foreach( XmlNode ColumnNode in ItemNode.ChildNodes )
                {
                    if( _KeyColumn == ColumnNode.Attributes["field"].Value )
                        key = ColumnNode.Attributes["value"].Value;
                    if( _NameColumn == ColumnNode.Attributes["field"].Value )
                        name = ColumnNode.Attributes["value"].Value;
                }
                // save values
                foreach( XmlNode ColumnNode in ItemNode.ChildNodes )
                {
                    string field = ColumnNode.Attributes["field"].Value;
                    string value = ColumnNode.Attributes["value"].Value;
                    if( field != _KeyColumn && field != _NameColumn )
                    {
                        SetValue( field, key, CswConvert.ToBoolean( value ) );
                    }
                }
            }

            Save();
        }


        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            //Not yet implemented
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //Not yet implemented
        }

        /// <summary>
        /// Initialize this object with data from the given DataRow
        /// </summary>
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            LogicalSetXmlDoc = new XmlDocument();

            // Kludge for nodetype permissions...
            if( this.ObjectClassPropName == CswNbtObjClassRole.NodeTypePermissionsPropertyName )
            {
                // Have to map the nodetypes
                ResetXml();
                XmlDocument TempXmlDoc = new XmlDocument();
                TempXmlDoc.LoadXml( PropRow[_ElemName_LogicalSetXml].ToString() );
                XmlNodeList YValueNodes = TempXmlDoc.ChildNodes[0].SelectNodes( "YValue" );
                foreach( XmlNode YValueNode in YValueNodes )
                {
                    Int32 YValue = CswConvert.ToInt32( YValueNode.Attributes["y"].Value );
                    if( NodeTypeMap.ContainsKey( YValue ) && NodeTypeMap[YValue] != Int32.MinValue )
                    {
                        foreach( XmlNode XValueNode in YValueNode.ChildNodes )
                        {
                            SetValue( XValueNode.Name, NodeTypeMap[YValue].ToString(), ( XValueNode.InnerText == "1" ) );
                        }
                    }
                }
            }
            else
            {
                // Load verbatim
                setXml( PropRow[_ElemName_LogicalSetXml].ToString() );
            }
            Save();
        } // ReadDataRow()
    } // class CswNbtNodePropLogicalSet
} // namespace ChemSW.Nbt.PropTypes

