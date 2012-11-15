using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataObjectClassProp : ICswNbtMetaDataObject, ICswNbtMetaDataProp, IEquatable<CswNbtMetaDataObjectClassProp>
    {
        public enum ObjectClassPropAttributes
        {
            auditlevel,
            fieldtypeid,
            isbatchentry,
            isrequired,
            isunique,
            iscompoundunique,
            servermanaged,
            valuefieldid,
            valuepropid,
            valueproptype,
            numberprecision,
            listoptions,
            viewxml,
            isfk,
            fktype,
            fkvalue,
            multi,
            readOnly,
            display_col_add,
            display_row_add,
            setvalonadd,
            numberminvalue,
            numbermaxvalue,
            statictext,
            filter,
            filterpropid,
            usenumbering,
            valueoptions,
            propname,
            isglobalunique,
            extended,
            textarearows,
            textareacols,
            Unknown
        }

        public static ObjectClassPropAttributes getObjectClassPropAttributesFromString( string AttributeName )
        {
            ObjectClassPropAttributes ReturnVal = ObjectClassPropAttributes.Unknown;
            if( Enum.IsDefined( typeof( ObjectClassPropAttributes ), AttributeName ) )
            {
                ReturnVal = (ObjectClassPropAttributes) Enum.Parse( typeof( ObjectClassPropAttributes ), AttributeName, true );
            }
            return ( ReturnVal );
        }

        public static String getObjectClassPropAttributesAsString( ObjectClassPropAttributes Attribute )
        {
            String ReturnVal = String.Empty;
            if( Attribute != ObjectClassPropAttributes.Unknown )
                ReturnVal = Attribute.ToString();
            return ( ReturnVal );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _ObjectClassPropRow;

        public CswNbtMetaDataObjectClassProp( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
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
                _FieldTypeRule = _CswNbtMetaDataResources.makeFieldTypeRule( this.getFieldType().FieldType );
            return _FieldTypeRule;
        }
        public Int32 FirstPropVersionId { get { return PropId; } }
        public Int32 PropId
        {
            get { return ObjectClassPropId; }
        }
        public Int32 ObjectClassPropId
        {
            get { return CswConvert.ToInt32( _ObjectClassPropRow["objectclasspropid"] ); }
        }
        public string PropName
        {
            get { return _ObjectClassPropRow["propname"].ToString(); }
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


        //public void setFilter( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        //{
        //    _ObjectClassPropRow[ "filter" ] = SubField.Column.ToString() + FilterDelimiter + FilterMode.ToString() + FilterDelimiter + FilterValue.ToString();
        //    // This can alter the order
        //    //if ( OnEditNodeTypePropOrder != null )
        //    //    OnEditNodeTypePropOrder( this );
        //}
        //public void clearFilter()
        //{
        //    _ObjectClassPropRow[ "filter" ] = string.Empty;
        //    // This can alter the order
        //    //if ( OnEditNodeTypePropOrder != null )
        //    //    OnEditNodeTypePropOrder( this );
        //}

        /// <summary>
        /// Default filter delimiter
        /// </summary>
        public const char FilterDelimiter = '|';
        public void getFilter( ref CswNbtSubField SubField, ref CswNbtPropFilterSql.PropertyFilterMode FilterMode, ref string FilterValue )
        {
            if( _ObjectClassPropRow["filter"].ToString() != string.Empty )
            {
                string[] filter = _ObjectClassPropRow["filter"].ToString().Split( FilterDelimiter );
                //CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) Enum.Parse( typeof( CswNbtSubField.PropColumn ), filter[0] );
                CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) filter[0];
                SubField = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId ).getFieldTypeRule().SubFields[Column];
                //FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), filter[1] );
                FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) filter[1];
                if( filter.GetUpperBound( 0 ) > 1 )
                    FilterValue = filter[2];
            }
        }
        public string getFilterString()
        {
            return _ObjectClassPropRow["filter"].ToString();
        }
        public bool hasFilter()
        {
            return ( _ObjectClassPropRow["filter"].ToString() != string.Empty );
        }

        public static string makeFilter( CswNbtMetaDataObjectClassProp Prop, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            return makeFilter( Prop.getFieldTypeRule().SubFields.Default, FilterMode, FilterValue );
        }

        public static string makeFilter( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            return SubField.Column.ToString() + FilterDelimiter + FilterMode + FilterDelimiter + FilterValue;
        }

        public void setFilter( Int32 FilterObjectClassPropId, CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            string FilterString = makeFilter( SubField, FilterMode, FilterValue );
            CswNbtMetaDataObjectClassProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId );
            _setFilter( FilterProp, FilterString );
        }

        public void setFilter( CswNbtMetaDataObjectClassProp FilterProp, CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            string FilterString = makeFilter( SubField, FilterMode, FilterValue );
            _setFilter( FilterProp, FilterString );
        }

        public void setFilter( Int32 FilterObjectClassPropId, string FilterString )
        {
            CswNbtMetaDataObjectClassProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( FilterObjectClassPropId );
            _setFilter( FilterProp, FilterString );
        }

        private void _setFilter( CswNbtMetaDataObjectClassProp FilterProp, string FilterString )
        {
            if( IsRequired )
            {
                throw new CswDniException( ErrorType.Warning, "Required properties cannot be conditional", "User attempted to set a conditional filter on a required property" );
            }

            bool changed = false;

            if( FilterProp != null )
            {
                Int32 CurrentFilterId = CswConvert.ToInt32( _ObjectClassPropRow["filterpropid"] );
                changed = CurrentFilterId != FilterProp.FirstPropVersionId;
                if( changed )
                {
                    _ObjectClassPropRow["filterpropid"] = CswConvert.ToDbVal( FilterProp.FirstPropVersionId );
                }
            }
            else
            {
                FilterString = "";
                _CswNbtMetaDataResources.CswNbtResources.logMessage( "Attempted to create a conditional property filter with based upon a null ObjectClassProperty." );
            }
            string CurrentFilter = CswConvert.ToString( _ObjectClassPropRow["filter"] );
            if( CurrentFilter != FilterString )
            {
                changed = true;
                _ObjectClassPropRow["filter"] = CswConvert.ToDbVal( FilterString );
            }

            //if( changed )
            //{
            //    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in this.getNodeTypeProps() )
            //    {

            //    }

            //}
        }

        public PropertySelectMode Multi
        {
            get
            {
                if( _ObjectClassPropRow["multi"].ToString() != string.Empty )
                    return (PropertySelectMode) Enum.Parse( typeof( PropertySelectMode ), _ObjectClassPropRow["multi"].ToString() );
                else
                    return PropertySelectMode.Blank;
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
                    _DefaultValue = new CswNbtNodePropData( _CswNbtMetaDataResources.CswNbtResources, _DefaultValueRow, _DefaultValueRow.Table, ObjectClassPropId );
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



        public void CopyPropToNewPropRow( DataRow NewPropRow )
        {
            foreach( DataColumn Column in _ObjectClassPropRow.Table.Columns )
            {
                string ColumnName = Column.ColumnName;
                if( ColumnName != "display_rowadd" &&
                    ColumnName != "display_coladd" &&
                    ColumnName != "setvalonadd" &&
                    ColumnName != "defaultvalueid" )
                {
                    if( NewPropRow.Table.Columns.Contains( ColumnName ) )
                    {
                        if( !_ObjectClassPropRow.IsNull( ColumnName ) )
                        {
                            NewPropRow[ColumnName] = _ObjectClassPropRow[ColumnName];
                        }
                    }
                }
            }
        } // CopyPropToNewPropRow()

        public bool IsUserRelationship()
        {
            bool ret = false;
            if( this.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
            {
                if( FKType != string.Empty )
                {
                    //NbtViewRelatedIdType TargetType = (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), FKType, true );
                    NbtViewRelatedIdType TargetType = (NbtViewRelatedIdType) FKType;

                    if( TargetType == NbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType TargetNodeType = _CswNbtMetaDataResources.CswNbtResources.MetaData.getNodeType( FKValue );
                        ret = ( TargetNodeType.getObjectClass().ObjectClass == NbtObjectClass.UserClass );
                    }
                    else if( TargetType == NbtViewRelatedIdType.ObjectClassId )
                    {
                        CswNbtMetaDataObjectClass TargetObjectClass = _CswNbtMetaDataResources.CswNbtResources.MetaData.getObjectClass( FKValue );
                        ret = ( TargetObjectClass.ObjectClass == NbtObjectClass.UserClass );

                    }
                }
            }
            return ret;
        }

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
