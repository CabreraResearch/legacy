using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Prop Class for NodeTypeSelect Properties
    /// </summary>
    public class CswNbtNodePropNodeTypeSelect : CswNbtNodeProp
    {
        public static char delimiter = ',';

        public static implicit operator CswNbtNodePropNodeTypeSelect( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNodeTypeSelect;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropNodeTypeSelect( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _SelectedNodeTypeIdsSubField = ( (CswNbtFieldTypeRuleNodeTypeSelect) _FieldTypeRule ).SelectedNodeTypeIdsSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _SelectedNodeTypeIdsSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => SelectedNodeTypeIds, x => SelectedNodeTypeIds = new CswCommaDelimitedString( CswConvert.ToString( x ) ) ) );
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
                    _SelectedNodeTypeIds.FromString( GetPropRowValue( _SelectedNodeTypeIdsSubField ) );
                    _SelectedNodeTypeIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedNodeTypeIds_OnChange );
                }
                return _SelectedNodeTypeIds;
            }
            set
            {
                _SelectedNodeTypeIds = value;
                _SelectedNodeTypeIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedNodeTypeIds_OnChange );
                _SelectedNodeTypeIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        private void _SelectedNodeTypeIds_OnChange()
        {
            if( SetPropRowValue( _SelectedNodeTypeIdsSubField, _SelectedNodeTypeIds.ToString() ) )
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
            SyncGestalt();
            PendingUpdate = false;
        }

        /// <summary>
        /// Mode of operation for this NodeTypeSelect property
        /// </summary>
        public CswEnumNbtPropertySelectMode SelectMode
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.Multi;
                return _CswNbtNodePropData[CswNbtFieldTypeRuleNodeTypeSelect.AttributeName.SelectMode];
            }
        }

        public string FKType
        {
            //get { return _CswNbtMetaDataNodeTypeProp.FKType; }
            get { return _CswNbtNodePropData[CswNbtFieldTypeRuleNodeTypeSelect.AttributeName.FKType]; }
        }

        public Int32 ConstrainToObjectClassId
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.FKValue;
                return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleNodeTypeSelect.AttributeName.ConstrainToObjectClass] );
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

                // client handles this now
                //if( SelectMode != PropertySelectMode.Multiple && !Required )
                //{
                //    DataRow NTRow = Data.NewRow();
                //    NTRow[NameColumn] = "[none]";
                //    NTRow[KeyColumn] = CswConvert.ToDbVal( Int32.MinValue );
                //    NTRow[ValueColumn] = ( SelectedNodeTypeIds.Count == 0 );
                //    Data.Rows.Add( NTRow );
                //}

                bool first = true;
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
                {
                    if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NodeType ) &&          // has view permissions (CIS-52993) and
                        ( Int32.MinValue == ConstrainToObjectClassId ||                                                //  no constraint, or
                          ( FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&                          //  constraint is object-class-based and
                            NodeType.ObjectClassId == ConstrainToObjectClassId ) ||                                    //   constraint matches object class, or 
                          ( FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&                          //  constraint is property-set-based and
                            null != NodeType.getObjectClass() &&                                                       //   nodetype has an object class and 
                            null != NodeType.getObjectClass().getPropertySet() &&                                      //   object class has a property set and 
                            NodeType.getObjectClass().getPropertySet().PropertySetId == ConstrainToObjectClassId ) ) ) //   constraint matches property set
                    {
                        DataRow NTRow = Data.NewRow();
                        NTRow[NameColumn] = NodeType.NodeTypeName;            // latest name
                        NTRow[KeyColumn] = NodeType.FirstVersionNodeTypeId;   // first nodetypeid
                        NTRow[ValueColumn] = ( SelectedNodeTypeIds.Contains( NodeType.FirstVersionNodeTypeId.ToString() ) ||
                                             ( first && Required && SelectedNodeTypeIds.Count == 0 ) );
                        Data.Rows.Add( NTRow );
                        first = false;
                    }
                }
                return Data;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }



        public const string NameColumn = "label";
        public const string KeyColumn = "key";
        public const string ValueColumn = "value";
        public const string TableName = "nodetypeselectdatatable";

        private const string _ElemName_Options = "options";

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_SelectedNodeTypeIdsSubField.ToXmlNodeName().ToLower()] = SelectedNodeTypeIds.ToString();
            ParentObject["selectmode"] = SelectMode.ToString();
            ParentObject[_ElemName_Options] = new JObject();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            CBAOptions.Columns.Add( "Include" );

            DataTable Data = Options;
            foreach( DataRow Row in Data.Rows )
            {
                bool isSelected = SelectedNodeTypeIds.Contains( Row[KeyColumn] );
                if( IsEditModeEditable || isSelected )
                {
                    CswCheckBoxArrayOptions.Option Option = new CswCheckBoxArrayOptions.Option();
                    Option.Key = CswConvert.ToString( Row[KeyColumn] );
                    Option.Label = CswConvert.ToString( Row[NameColumn] );
                    Option.Values.Add( CswConvert.ToBoolean( Row[ValueColumn] ) );
                    CBAOptions.Options.Add( Option );
                }
            }

            CBAOptions.ToJSON( (JObject) ParentObject[_ElemName_Options] );

        } // ToJSON()


        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedNodeTypeIds.FromString( _HandleReferences( CswTools.XmlRealAttributeName( PropRow[_SelectedNodeTypeIdsSubField.ToXmlNodeName()].ToString() ), NodeTypeMap ) );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewSelectedNodeTypeIds = SelectedNodeTypeIds;

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            if( null != JObject[_ElemName_Options] )
            {
                CBAOptions.ReadJson( (JObject) JObject[_ElemName_Options] );
            }
            foreach( CswCheckBoxArrayOptions.Option Option in CBAOptions.Options )
            {
                if( Option.Values.Count > 0 )
                {
                    if( Option.Values.Count > 0 && true == Option.Values[0] )
                    {
                        NewSelectedNodeTypeIds.Add( Option.Key );
                    }
                    else if( false == Option.Values[0] )
                    {
                        NewSelectedNodeTypeIds.Remove( Option.Key );
                    }
                }
            }
            SelectedNodeTypeIds = NewSelectedNodeTypeIds;
        } // ReadJSON()

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
                        NodeTypeNames.Add( NodeType.getNodeTypeLatestVersion().NodeTypeName );
                    }
                }
            } // foreach(string NodeTypeId in SelectedNodeTypeIds)
            if( 0 == NodeTypeNames.Count )
            {
                NodeTypeNames.Add( "[none]" );
            }

            // Sort alphabetically
            NodeTypeNames.Sort();

            return NodeTypeNames;
        } // SelectedNodeTypeNames()

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, SelectedNodeTypeNames().ToString() );
        }

    }//CswNbtNodePropNodeTypeSelect
}//namespace ChemSW.Nbt.PropTypes
