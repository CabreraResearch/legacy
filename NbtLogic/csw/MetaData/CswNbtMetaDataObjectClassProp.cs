using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{
    [DataContract]
    public class CswNbtMetaDataObjectClassProp : ICswNbtMetaDataObject, ICswNbtMetaDataProp, IEquatable<CswNbtMetaDataObjectClassProp>
    {
        public static CswEnumNbtObjectClassPropAttributes getObjectClassPropAttributesFromString( string AttributeName )
        {
            CswEnumNbtObjectClassPropAttributes ReturnVal = CswResources.UnknownEnum;
            ReturnVal = AttributeName;
            return ( ReturnVal );
        }

        public static String getObjectClassPropAttributesAsString( CswEnumNbtObjectClassPropAttributes Attribute )
        {
            String ReturnVal = String.Empty;
            if( Attribute != CswResources.UnknownEnum )
                ReturnVal = Attribute.ToString();
            return ( ReturnVal );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _ObjectClassPropRow;
        private CswDateTime _Date;

        public CswNbtMetaDataObjectClassProp( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row, CswDateTime Date = null )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _Date = Date;
            Reassign( Row );
        }

        public DataRow _DataRow
        {
            get { return _ObjectClassPropRow; }
            //set { _ObjectClassPropRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "objectclasspropid"; } }

        public void Reassign( DataRow NewRow )
        {
            _ObjectClassPropRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        private ICswNbtFieldTypeRule _FieldTypeRule = null;
        public ICswNbtFieldTypeRule getFieldTypeRule()
        {
            if( _FieldTypeRule == null )
                _FieldTypeRule = _CswNbtMetaDataResources.makeFieldTypeRule( this.getFieldTypeValue() );
            return _FieldTypeRule;
        }
        public Int32 FirstPropVersionId { get { return PropId; } }

        [DataMember]
        public Int32 PropId
        {
            get { return ObjectClassPropId; }
            private set { var KeepSerializerHappy = value; }

        }
        public Int32 ObjectClassPropId
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["objectclasspropid"] ); }
        }

        [DataMember]
        public string PropName
        {
            get { return _ObjectClassPropRow["propname"].ToString(); }
            private set { var KeepSerializerHappy = value; }
        }

        [DataMember( Name = "ColumnName" )]
        public Collection<CswNbtSdDbQueries.Column> DbViewColumns
        {
            get
            {
                Collection<CswNbtSdDbQueries.Column> Ret = new Collection<CswNbtSdDbQueries.Column>();

                string DbName = "OP_" + PropId;
                CswNbtSdDbQueries.Column Gestalt = new CswNbtSdDbQueries.Column();
                Gestalt.Name = PropName;
                Gestalt.Type = "clob";
                Gestalt.DbName = DbName;
                Ret.Add( Gestalt );
                if( this.getFieldType().FieldType == CswEnumNbtFieldType.Relationship || this.getFieldType().FieldType == CswEnumNbtFieldType.Location )
                {
                    CswNbtSdDbQueries.Column Relationship = new CswNbtSdDbQueries.Column();
                    Relationship.Name = PropName + " Fk";
                    Relationship.Type = "number(12,0)";
                    Relationship.DbName = DbName + "_fk";
                    Ret.Add( Relationship );
                }

                return Ret;
            }
            private set
            {
                var KeepSerializerHappy = value;
            }
        }

        [DataMember( Name = "ViewName" )]
        public string DbViewColumnName
        {

            get
            {
                return CswConvert.ToString( _ObjectClassPropRow["oraviewcolname"] );
            }

        }


        public string PropNameWithQuestionNo
        {
            get { return _ObjectClassPropRow["propname"].ToString(); }
        }
        public CswNbtMetaDataObjectClass getObjectClass()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getObjectClass( ObjectClassId );
        }
        public Int32 ObjectClassId
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["objectclassid"] ); }
        }
        public Int32 FieldTypeId
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["fieldtypeid"] ); }
        }
        public CswNbtMetaDataFieldType getFieldType()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getFieldType( FieldTypeId );
        }
        public CswEnumNbtFieldType getFieldTypeValue()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getFieldTypeValue( FieldTypeId );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps()
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropsByObjectClassProp( ObjectClassPropId );
        }

        public bool IsRequired
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["isrequired"] ); }
        }
        public string StaticText
        {
            get { return _ObjectClassPropRow["statictext"].ToString(); }
        }

        public bool IsUnique() //bz # 6686
        {
            return ( CswConvert.ToBoolean( _ObjectClassPropRow["isunique"] ) || IsGlobalUnique() );
        }

        public bool IsCompoundUnique() //bz # 24979
        {
            return ( CswConvert.ToBoolean( _ObjectClassPropRow["iscompoundunique"] ) );
        }

        /// <summary>
        /// Specifies whether all values of the property must be unique for all nodetypes.
        /// </summary>
        /// <returns></returns>
        public bool IsGlobalUnique() // BZ 9754
        {
            return CswConvert.ToBoolean( _ObjectClassPropRow["isglobalunique"] );
        }

        //bz# 5640
        public bool ReadOnly
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["readonly"] ); }
        }


        public Int32 NumberPrecision
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["numberprecision"] ); }
        }
        public double MinValue
        {
            get { return CswConvert.ToDouble( _ObjectClassPropRow["numberminvalue"] ); }
        }
        public double MaxValue
        {
            get { return CswConvert.ToDouble( _ObjectClassPropRow["numbermaxvalue"] ); }
        }


        public Int32 FilterObjectClassPropId
        {
            get
            {
                return CswConvert.ToInt32( _ObjectClassPropRow["filterpropid"] );
            }
            set
            {

                _ObjectClassPropRow["filterpropid"] = CswConvert.ToDbVal( value );
                // This can alter the order
                //if ( OnEditNodeTypePropOrder != null )
                //    OnEditNodeTypePropOrder( this );
            }
        }

        /// <summary>
        /// Default filter delimiter
        /// </summary>
        //public const char FilterDelimiter = '|';
        public void getFilter( ref CswNbtSubField SubField, ref CswEnumNbtFilterMode FilterMode, ref string FilterValue )
        {
            CswNbtMetaDataObjectClassProp FilterOCP = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId );
            if( FilterOCP != null )
            {
                SubField = FilterOCP.getFieldTypeRule().SubFields[(CswEnumNbtSubFieldName) _ObjectClassPropRow["filtersubfield"].ToString()];
                FilterMode = (CswEnumNbtFilterMode) _ObjectClassPropRow["filtermode"].ToString();
                FilterValue = _ObjectClassPropRow["filtervalue"].ToString();
            }
        }

        public bool hasFilter()
        {
            return ( _ObjectClassPropRow["filtersubfield"].ToString() != string.Empty &&
                     _ObjectClassPropRow["filtermode"].ToString() != string.Empty &&
                     _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId ) != null );
        }

        //public void setFilterDeprecated( Int32 FilterObjectClassPropId, CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode, object FilterValue )
        //{
        //    CswNbtMetaDataObjectClassProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId );
        //    setFilterDeprecated( FilterProp, SubField, FilterMode, FilterValue );
        //}

        //public void setFilterDeprecated( CswNbtMetaDataObjectClassProp FilterProp, CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode, object FilterValue )
        //{
        //    if( IsRequired )
        //    {
        //        throw new CswDniException( CswEnumErrorType.Warning, "Required properties cannot be conditional", "User attempted to set a conditional filter on a required property" );
        //    }

        //    bool changed = false;

        //    if( FilterProp != null )
        //    {
        //        Int32 CurrentFilterId = CswConvert.ToInt32( _ObjectClassPropRow["filterpropid"] );
        //        changed = ( CurrentFilterId != FilterProp.FirstPropVersionId ||
        //                    SubField.Name.ToString() != _ObjectClassPropRow["filtersubfield"].ToString() ||
        //                    FilterMode.ToString() != _ObjectClassPropRow["filtermode"].ToString() ||
        //                    FilterValue.ToString() != _ObjectClassPropRow["filtervalue"].ToString() );
        //        if( changed )
        //        {
        //            _ObjectClassPropRow["filterpropid"] = CswConvert.ToDbVal( FilterProp.FirstPropVersionId );
        //            _ObjectClassPropRow["filtersubfield"] = SubField.Name.ToString();
        //            _ObjectClassPropRow["filtermode"] = FilterMode.ToString();
        //            _ObjectClassPropRow["filtervalue"] = FilterValue;
        //        }
        //    }
        //    else
        //    {
        //        _CswNbtMetaDataResources.CswNbtResources.logMessage( "Attempted to create a conditional property filter with based upon a null ObjectClassProperty." );
        //    }
        //    if( changed )
        //    {
        //        _setNodeTypePropFiltersDeprecated( FilterProp, SubField, FilterMode, FilterValue );
        //    }
        //}

        //public void setNodeTypePropFiltersDeprecated()
        //{
        //    CswNbtMetaDataObjectClassProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId );
        //    CswNbtSubField SubField = null;
        //    CswEnumNbtFilterMode FilterMode = CswEnumNbtFilterMode.Unknown;
        //    string FilterValue = string.Empty;
        //    getFilter( ref SubField, ref FilterMode, ref FilterValue );
        //    _setNodeTypePropFiltersDeprecated( FilterProp, SubField, FilterMode, FilterValue );
        //}

        //private void _setNodeTypePropFiltersDeprecated( CswNbtMetaDataObjectClassProp FilterProp, CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode, object FilterValue )
        //{
        //    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in this.getNodeTypeProps() )
        //    {
        //        if( FilterProp != null )
        //        {
        //            foreach( CswNbtMetaDataNodeTypeProp FilterPropNodeTypeProp in FilterProp.getNodeTypeProps() )
        //            {
        //                if( FilterPropNodeTypeProp.NodeTypeId == NodeTypeProp.NodeTypeId )
        //                {
        //                    NodeTypeProp.setFilterDeprecated( FilterPropNodeTypeProp, SubField, FilterMode, FilterValue );
        //                }
        //            }
        //        }
        //    }
        //}

        public CswEnumNbtPropertySelectMode Multi
        {
            get
            {
                if( _ObjectClassPropRow["multi"].ToString() != string.Empty )
                    return CswConvert.ToString( _ObjectClassPropRow["multi"] );
                else
                    return CswEnumNbtPropertySelectMode.Blank;
            }
        }
        public bool IsBatchEntry
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["isbatchentry"] ); }
        }
        public bool ServerManaged
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["servermanaged"] ); }
        }

        public string FKType
        {
            get { return _ObjectClassPropRow["fktype"].ToString(); }
            private set { _ObjectClassPropRow["fktype"] = value; }
        }
        public bool IsFK
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["isfk"] ); }
            private set { _ObjectClassPropRow["isfk"] = CswConvert.ToDbVal( value ); }
        }
        public Int32 FKValue
        {
            get
            {
                if( _ObjectClassPropRow["fkvalue"].ToString() != string.Empty )
                    return CswConvert.ToInt32( _ObjectClassPropRow["fkvalue"].ToString() );
                else
                    return Int32.MinValue;
            }
            private set { _ObjectClassPropRow["fkvalue"] = value; }
        }

        //public void setNodeTypePropFKDeprecated()
        //{
        //    if( IsFK && false == String.IsNullOrEmpty( FKType ) && Int32.MinValue != FKValue )
        //    {
        //        foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in this.getNodeTypeProps() )
        //        {
        //            NodeTypeProp.SetFKDeprecated( FKType, FKValue, ValuePropType, ValuePropId );
        //        }
        //    }
        //}

        public Int32 ValuePropId
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["valuepropid"] ); }
            private set { _ObjectClassPropRow["valuepropid"] = value; }
        }
        public string ValuePropType
        {
            get { return _ObjectClassPropRow["valueproptype"].ToString(); }
            private set { _ObjectClassPropRow["valueproptype"] = value; }
        }

        public Int32 DisplayColAdd
        {
            get
            {
                Int32 ReturnVal = Int32.MinValue;
                if( !_ObjectClassPropRow.IsNull( "display_col_add" ) )
                    ReturnVal = CswConvert.ToInt32( _ObjectClassPropRow["display_col_add"].ToString() );
                return ( ReturnVal );
            }
        }//DisplayColAdd

        public Int32 DisplayRowAdd
        {
            get
            {
                Int32 ReturnVal = Int32.MinValue;
                if( !_ObjectClassPropRow.IsNull( "display_row_add" ) )
                    ReturnVal = CswConvert.ToInt32( _ObjectClassPropRow["display_row_add"].ToString() );
                return ( ReturnVal );
            }
        }//DisplayColAdd


        public bool SetValueOnAdd
        {
            get { return CswConvert.ToBoolean( _ObjectClassPropRow["setvalonadd"] ); }
        }

        public string ViewXml
        {
            get { return _ObjectClassPropRow["viewxml"].ToString(); }
        }
        public string ListOptions
        {
            get { return _ObjectClassPropRow["listoptions"].ToString(); }
        }
        public string ValueOptions
        {
            get { return _ObjectClassPropRow["valueoptions"].ToString(); }
        }
        public string Extended
        {
            get { return _ObjectClassPropRow["extended"].ToString(); }
        }

        //public string DefaultValue
        //{
        //    get 
        //    {
        //        string ReturnVal = "";

        //        if( ! _ObjectClassPropRow.IsNull( "defaultvalue" ) ) 
        //        {
        //            ReturnVal = _ObjectClassPropRow[ "defaultvalue" ].ToString(); 
        //        }

        //        return ( ReturnVal );
        //    }//get

        //}//DefaultValue 



        private CswNbtNodePropData _DefaultValue = null;
        private DataRow _DefaultValueRow = null;

        private void _initDefaultValue( bool CreateMissingRow )
        {
            if( _DefaultValue == null )
            {
                if( _DefaultValueRow == null )
                {
                    if( _ObjectClassPropRow.Table.Columns.Contains( "defaultvalueid" ) )
                    {
                        if( _ObjectClassPropRow["defaultvalueid"] != null && CswTools.IsInteger( _ObjectClassPropRow["defaultvalueid"] ) )
                        {
                            DataTable DefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getTable( "jctnodepropid", CswConvert.ToInt32( _ObjectClassPropRow["defaultvalueid"] ) );
                            //WARNING: there is a possibility that DefaultValueTable will be empty, upon which DefaultValue will not be set.
                            //This can cause _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue() to throw an ORNY exception.
                            //This needs to be fixed at some point - see Reviews K4156 and K4157 for details.
                            if( DefaultValueTable.Rows.Count > 0 )
                                _DefaultValueRow = DefaultValueTable.Rows[0];
                        }
                        else if( CreateMissingRow )
                        {
                            DataTable NewDefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getEmptyTable();
                            _DefaultValueRow = NewDefaultValueTable.NewRow();
                            _DefaultValueRow["objectclasspropid"] = CswConvert.ToDbVal( this.PropId );
                            NewDefaultValueTable.Rows.Add( _DefaultValueRow );

                            _ObjectClassPropRow["defaultvalueid"] = _DefaultValueRow["jctnodepropid"];
                            _CswNbtMetaDataResources.ObjectClassPropTableUpdate.update( _ObjectClassPropRow.Table );
                        }
                    } // if( _NodeTypePropRow.Table.Columns.Contains( "defaultvalueid" ) )
                } // if( _DefaultValueRow == null )

                if( _DefaultValueRow != null )
                {
                    _DefaultValue = new CswNbtNodePropData( _CswNbtMetaDataResources.CswNbtResources, _DefaultValueRow, _DefaultValueRow.Table, ObjectClassPropId, null );
                }

            } // if( _DefaultValue == null )
        }

        public bool HasDefaultValue()
        {
            _initDefaultValue( false );
            return !( _DefaultValue == null );
        }

        /// <summary>
        /// Returns the Object Class Prop's default value (a row in jct_nodes_props)
        /// This does not return a CswNbtNodePropWrapper.  See BZ 5073.
        /// </summary>
        public CswNbtNodePropData DefaultValue
        {
            get
            {
                _initDefaultValue( true );
                return _DefaultValue;
            }
            // NO SET...interact with the CswNbtNodePropData instead
        }



        public void CopyPropToNewPropRow( CswNbtMetaDataNodeTypeProp NewProp ) //DataRow NewPropRow )
        {
            CswTableSelect MappingSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "PropCollDataRelational_mapping", "jct_dd_ntp" );
            DataTable MappingTable = MappingSelect.getTable();

            foreach( DataColumn Column in _ObjectClassPropRow.Table.Columns )
            {
                string ColumnName = Column.ColumnName;
                if( ColumnName != "display_rowadd" &&
                    ColumnName != "display_coladd" &&
                    ColumnName != "setvalonadd" &&
                    ColumnName != "defaultvalueid" )
                {
                    if( false == _ObjectClassPropRow.IsNull( ColumnName ) )
                    {
                        if( _CswNbtMetaDataResources.CswNbtResources.DataDictionary.isColumnDefined( "nodetype_props", Column.ColumnName ) )
                        {
                            _CswNbtMetaDataResources.CswNbtResources.DataDictionary.setCurrentColumn( "nodetype_props", Column.ColumnName );
                            DataRow MappingRow = MappingTable.Rows.Cast<DataRow>()
                                                             .FirstOrDefault( r => NewProp.DesignNode.NodeType.getNodeTypePropIds().Contains( CswConvert.ToInt32( r["nodetypepropid"] ) ) &&
                                                                                   CswConvert.ToInt32( r["datadictionaryid"] ) == _CswNbtMetaDataResources.CswNbtResources.DataDictionary.TableColId );
                            if( null != MappingRow )
                            {

                                NewProp.DesignNode.Node.Properties[CswConvert.ToInt32( MappingRow["nodetypepropid"] )].SetSubFieldValue( (CswEnumNbtSubFieldName) MappingRow["subfieldname"].ToString(), _ObjectClassPropRow[ColumnName] );
                            }
                        } // if( _CswNbtMetaDataResources.CswNbtResources.DataDictionary.isColumnDefined( "nodetype_props", Column.ColumnName ) )
                    } // if( false == _ObjectClassPropRow.IsNull( ColumnName ) )
                } // if( ColumnName != "displayrowadd" && ... )
            } // foreach( DataColumn Column in _ObjectClassPropRow.Table.Columns )
        } // CopyPropToNewPropRow()


        public bool IsNodeReference()
        {
            return ( ( this.getFieldTypeValue() == CswEnumNbtFieldType.Relationship ) ||
                     ( this.getFieldTypeValue() == CswEnumNbtFieldType.Location ) ||
                     ( this.getFieldTypeValue() == CswEnumNbtFieldType.Quantity ) );
        }

        public bool IsUserRelationship()
        {
            bool ret = false;
            if( this.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtMetaDataResources.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                ret = FkMatches( UserOC );
            }
            return ret;
        }


        #region FK Matching

        public bool FkMatches( CswNbtMetaDataNodeType CompareNT )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, CompareNT );
        }

        public bool FkMatches( CswNbtMetaDataObjectClass CompareOC )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, CompareOC );
        }

        public bool FkMatches( CswNbtMetaDataPropertySet ComparePS )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, ComparePS );
        }

        #endregion FK Matching

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataObjectClassProp ocp1, CswNbtMetaDataObjectClassProp ocp2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( ocp1, ocp2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) ocp1 == null ) || ( (object) ocp2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( ocp1.UniqueId == ocp2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataObjectClassProp ocp1, CswNbtMetaDataObjectClassProp ocp2 )
        {
            return !( ocp1 == ocp2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtMetaDataObjectClassProp ) )
                return false;
            return this == (CswNbtMetaDataObjectClassProp) obj;
        }

        public bool Equals( CswNbtMetaDataObjectClassProp obj )
        {
            return this == (CswNbtMetaDataObjectClassProp) obj;
        }

        public override int GetHashCode()
        {
            return this.PropId;
        }

        #endregion IEquatable
    }
}
