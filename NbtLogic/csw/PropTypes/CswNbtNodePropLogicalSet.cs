﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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

        public static implicit operator CswNbtNodePropLogicalSet( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLogicalSet;
        }

        /// <summary>
        /// Represents the data of a set of checkboxes
        /// </summary>
        public CswNbtNodePropLogicalSet( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            if( Empty )
            {
                ResetXml();
            }
            else
            {
                LogicalSetXmlDoc = new XmlDocument();
                LogicalSetXmlDoc.LoadXml( ClobData );
            }

            _ClobDataSubField = ( (CswNbtFieldTypeRuleLogicalSet) _FieldTypeRule ).ClobDataSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ClobDataSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => LogicalSetXmlDoc, x => setXml( CswConvert.ToString( x ) ) ) );
        }//ctor

        private CswNbtSubField _ClobDataSubField;

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
                return ( 0 == ClobData.Length );
            }
        }

        public Int32 Rows
        {
            get
            {
                //if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                //    return 4;
                //else
                //    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
                Int32 ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.Rows] );
                if( Int32.MinValue == ret )
                {
                    ret = 4;
                }
                return ret;
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
            SetPropRowValue( _ClobDataSubField, LogicalSetXmlDoc.InnerXml.ToString() );
            SyncGestalt();
            //PendingUpdate = true;
        }

        public void RefreshStringValue( bool SetPendingUpdate = true )
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
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, CheckedNames );
            PendingUpdate = SetPendingUpdate;
        }

        public DataTable GetDataAsTable( string NameColumn, string KeyColumn )
        {
            DataTable Data = new CswDataTable( "GetDataAsTable_DataTable", "" );
            Data.Columns.Add( NameColumn );
            Data.Columns.Add( KeyColumn );

            foreach( string XValue in XValues )
                Data.Columns.Add( XValue, typeof( bool ) );

            //if( _CswNbtMetaDataNodeTypeProp.IsFK && _CswNbtMetaDataNodeTypeProp.FKType == "fkeydefid" )
            if( CswConvert.ToBoolean( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.IsFK] ) &&
                 _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.FKType] == "fkeydefid" )
            {
                CswTableSelect FkeyDefsSelect = _CswNbtResources.makeCswTableSelect( "YValues_fkeydef_select", "fkey_definitions" );
                //DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", _CswNbtMetaDataNodeTypeProp.FKValue );
                DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.FKValue] ) );
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
                    //_XValues.FromString( _CswNbtMetaDataNodeTypeProp.ValueOptions );
                    _XValues.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.XOptions] );
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
                    //if( _CswNbtMetaDataNodeTypeProp.IsFK && _CswNbtMetaDataNodeTypeProp.FKType == "fkeydefid" )
                    if( CswConvert.ToBoolean( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.IsFK] ) &&
                        _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.FKType] == "fkeydefid" )
                    {
                        CswTableSelect FkeyDefsSelect = _CswNbtResources.makeCswTableSelect( "LogicalSet_fkeydef_select", "fkey_definitions" );
                        //DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", _CswNbtMetaDataNodeTypeProp.FKValue );
                        DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.FKValue] ) );
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
                        //_YValues.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );
                        _YValues.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleLogicalSet.AttributeName.YOptions] );
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
            XmlNode YValueNode = LogicalSetXmlDoc.ChildNodes[0].SelectSingleNode( "YValue[@y='" + CswTools.XPathSafeParameter( YValue ) + "']" );
            if( YValueNode == null )
            {
                YValueNode = LogicalSetXmlDoc.CreateElement( "YValue" );
                XmlAttribute YAttribute = LogicalSetXmlDoc.CreateAttribute( "y" );
                YAttribute.Value = CswTools.XPathSafeParameter( YValue );
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
            XmlNode YValueNode = LogicalSetXmlDoc.ChildNodes[0].SelectSingleNode( "YValue[@y='" + CswTools.XPathSafeParameter( YValue ) + "']" );
            if( YValueNode != null )
            {
                XmlNode XValueNode = YValueNode.SelectSingleNode( XValue );
                if( XValueNode != null )
                    ret = ( XValueNode.InnerText == "1" );
            }
            return ret;
        }

        /*
                /// <summary>
                /// Checks to be sure all values assigned are valid against possible options
                /// </summary>
                public void ValidateValues()
                {
                    // Y values
                    XmlNodeList YValueNodes = LogicalSetXmlDoc.ChildNodes[0].SelectNodes( "YValue" );
                    Collection<XmlNode> DoomedNodes = new Collection<XmlNode>();
                    foreach( XmlNode YValueNode in YValueNodes )
                    {
                        string ThisYValue = YValueNode.Attributes["y"].Value;
                        if( false == YValues.Contains( ThisYValue ) )
                        {
                            DoomedNodes.Add( YValueNode );
                        }
                    }

                    foreach( XmlNode DoomedNode in DoomedNodes )
                    {
                        LogicalSetXmlDoc.ChildNodes[0].RemoveChild( DoomedNode );
                    }

                } // ValidateValues() 
        */

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }



        private string _ElemName_LogicalSetXml = "LogicalSetXml";
        private string _ElemName_LogicalSetJson = "logicalsetjson";

        private string _NameColumn = "name";
        private string _KeyColumn = "key";
        //private string _TableName = "logicalsetitem";

        // ToXml()

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ElemName_LogicalSetJson] = new JObject();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();

            DataTable Data = GetDataAsTable( _NameColumn, _KeyColumn );
            foreach( DataColumn Column in Data.Columns )
            {
                if( Column.ColumnName != _NameColumn &&
                    Column.ColumnName != _KeyColumn &&
                    false == CBAOptions.Columns.Contains( Column.ColumnName ) )
                {
                    CBAOptions.Columns.Add( Column.ColumnName );
                }
            }
            foreach( DataRow Row in Data.Rows )
            {
                CswCheckBoxArrayOptions.Option Option = new CswCheckBoxArrayOptions.Option();
                Option.Key = Row[_KeyColumn].ToString();
                Option.Label = Row[_NameColumn].ToString();
                for( Int32 i = 0; i < CBAOptions.Columns.Count; i++ )
                {
                    Option.Values.Add( CswConvert.ToBoolean( Row[CBAOptions.Columns[i]].ToString() ) );
                }
                CBAOptions.Options.Add( Option );
            }

            CBAOptions.ToJSON( (JObject) ParentObject[_ElemName_LogicalSetJson] );
        } // ToJSON()

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            if( null != JObject[_ElemName_LogicalSetJson] )
            {
                CBAOptions.ReadJson( (JObject) JObject[_ElemName_LogicalSetJson] );
            }
            foreach( CswCheckBoxArrayOptions.Option Option in CBAOptions.Options )
            {
                for( Int32 i = 0; i < Option.Values.Count; i++ )
                {
                    //TODO: this hasn't worked in a LOOOOOOONG Time. Case 29477.
                    SetValue( CBAOptions.Columns[i], Option.Label, Option.Values[i] );
                }
            }
            Save();
        } // ReadJSON()

        /// <summary>
        /// Initialize this object with data from the given DataRow
        /// </summary>
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            LogicalSetXmlDoc = new XmlDocument();

            setXml( PropRow[_ElemName_LogicalSetXml].ToString() );

            Save();
        } // ReadDataRow()

        public override void SyncGestalt()
        {
            RefreshStringValue( false );
        }

    } // class CswNbtNodePropLogicalSet
} // namespace ChemSW.Nbt.PropTypes

