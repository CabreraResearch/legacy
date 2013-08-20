using System;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropData
    {
        private CswPrimaryKey _NodeId;
        private Int32 _NodeTypePropId = Int32.MinValue;
        private Int32 _ObjectClassPropId = Int32.MinValue;
        private DataRow _PropRow = null;

        /// <summary>
        /// Creates a row in the database for this property
        /// </summary>
        public void makePropRow()
        {
            if( _PropRow == null ) //&& dbval != DBNull.Value )  case 22591
            {
                _PropRow = _PropsTable.NewRow();
                if( _NodeId != null )
                {
                    _PropRow["nodeid"] = CswConvert.ToDbVal( _NodeId.PrimaryKey );
                    _PropRow["nodeidtablename"] = _NodeId.TableName;
                }
                _PropRow["nodetypepropid"] = CswConvert.ToDbVal( _NodeTypePropId );
                _PropRow["objectclasspropid"] = CswConvert.ToDbVal( _ObjectClassPropId );
                _PropRow["pendingupdate"] = CswConvert.ToDbVal( false );
                _PropRow["readonly"] = CswConvert.ToDbVal( false );
                _PropsTable.Rows.Add( _PropRow );
            }
        }

        /// <summary>
        /// Sets the value of a column for a property
        /// value should be in native format -- this function will convert to db format
        /// </summary>
        /// <param name="column">Target column</param>
        /// <param name="value">New value</param>
        /// <returns>True if any changes were made</returns>
        public bool SetPropRowValue( CswEnumNbtPropColumn column, object value, bool IsNonModifying = false )
        {
            bool ret = false;
            object dbval;

            if( value is string )
            {
                dbval = CswConvert.ToDbVal( CswConvert.ToString( value ).Trim() );
            }
            else
            {
                dbval = CswConvert.ToDbVal( value );
            }

            if( _PropRow == null ) //&& dbval != DBNull.Value )  case 22591
            {
                makePropRow();
                ret = true;
            }

            if( _PropRow != null )
            {
                if( _NodeId != null )
                {
                    _PropRow["nodeid"] = CswConvert.ToDbVal( _NodeId.PrimaryKey );
                    _PropRow["nodeidtablename"] = _NodeId.TableName;
                }
                _PropRow["nodetypepropid"] = CswConvert.ToDbVal( _NodeTypePropId );
                _PropRow["objectclasspropid"] = CswConvert.ToDbVal( _ObjectClassPropId );

                if( false == ( CswConvert.ToDbVal( _PropRow[column.ToString()] ).Equals( dbval ) ) )
                {
                    _PropRow[column.ToString()] = CswConvert.ToDbVal( value );
                    if( false == IsNonModifying )
                    {
                        WasModified = true;
                        // don't mark modified if all we're doing is changing PendingUpdate
                        // see case 27652
                        WasModifiedForNotification = ( column != CswEnumNbtPropColumn.PendingUpdate );
                    }
                    ret = true;
                }
            }
            // Don't just return WasModified, or else changes to one subfield 
            // will look like changes to another subfield
            return ret;
        }

        private DataTable _PropsTable = null;

        /// <summary>
        /// Constructor for Node Properties
        /// </summary>
        public CswNbtNodePropData( CswNbtResources rsc, DataRow PropRow, DataTable PropsTable, CswPrimaryKey NodeId, Int32 PropId )
        {
            _PropRow = PropRow;
            _NodeId = NodeId;
            _NodeTypePropId = PropId;
            _PropsTable = PropsTable;
            _CswNbtResources = rsc;
        }//ctor()

        /// <summary>
        /// Constructor for Object Class Prop Default Values
        /// </summary>
        public CswNbtNodePropData( CswNbtResources rsc, DataRow PropRow, DataTable PropsTable, Int32 ObjectClassPropId )
        {
            _PropRow = PropRow;
            _ObjectClassPropId = ObjectClassPropId;
            _PropsTable = PropsTable;
            _CswNbtResources = rsc;
        }//ctor()

        //bz # 8287: rearranged a few things
        public void refresh( DataRow NewValueRow )
        {
            _PropRow = NewValueRow;
            _PropsTable = NewValueRow.Table;
        }//refresh

        private bool _WasModified = false;
        private bool _WasModifiedForNotification = false;
        private CswNbtResources _CswNbtResources;

        public bool WasModified
        {
            private set
            {
                if( !SuspendModifyTracking )
                {
                    // We never set it false, so that modifying Field1 but not modifying Field2 will not accidentally clear the modification flag.
                    // Use clearModifiedFlag() to clear it on purpose.
                    if( value )
                    {
                        _WasModified = true;
                    }
                }
            }
            get
            {
                return ( _WasModified );
            }
        }//WasModified

        public bool WasModifiedForNotification
        {
            private set
            {
                if( !SuspendModifyTracking )
                {
                    // We never set it false, so that modifying Field1 but not modifying Field2 will not accidentally clear the modification flag.
                    // Use clearModifiedFlag() to clear it on purpose.
                    if( value )
                    {
                        _WasModifiedForNotification = true;
                    }
                }
            }
            get
            {
                return ( _WasModifiedForNotification );
            }
        }//WasModified

        public void clearModifiedFlag()
        {
            _WasModified = false;
            _WasModifiedForNotification = false;
        }

        public string OtherPropGestalt( Int32 NodeTypePropId )
        {
            foreach( DataRow Row in _PropsTable.Rows )
            {
                if( Row["nodetypepropid"].ToString() == NodeTypePropId.ToString() )
                    return Row["gestalt"].ToString();
            }
            return string.Empty;
        }


        //default val is true so that as props are initially
        //populated they are not triggering the modify [sic.] flag
        private bool _SuspendModifyTracking = true;
        public bool SuspendModifyTracking
        {
            set
            {
                _SuspendModifyTracking = value;
            }
            get
            {
                return ( _SuspendModifyTracking );
            }//
        }//SuspendModifyTracking

        private string _getRowStringVal( CswEnumNbtPropColumn ColumnName )
        {
            string ReturnVal = "";
            if( _PropRow != null )
            {
                if( _PropsTable.Columns.Contains( ColumnName.ToString() ) && !_PropRow.IsNull( ColumnName.ToString() ) )
                {
                    ReturnVal = _PropRow[ColumnName.ToString()].ToString();
                }
            }
            return ( ReturnVal );
        }

        private bool _getRowBoolVal( CswEnumNbtPropColumn ColumnName )
        {
            bool ReturnVal = false;
            if( _PropRow != null )
            {
                if( _PropsTable.Columns.Contains( ColumnName.ToString() ) && !_PropRow.IsNull( ColumnName.ToString() ) )
                {
                    ReturnVal = CswConvert.ToBoolean( _PropRow[ColumnName.ToString()] );
                }
            }
            return ( ReturnVal );
        }

        private Int32 _getRowIntVal( CswEnumNbtPropColumn ColumnName )
        {
            Int32 ReturnVal = Int32.MinValue;
            if( _PropRow != null )
            {
                if( _PropsTable.Columns.Contains( ColumnName.ToString() ) && !_PropRow.IsNull( ColumnName.ToString() ) )
                {
                    ReturnVal = CswConvert.ToInt32( _PropRow[ColumnName.ToString()] );
                }
            }
            return ( ReturnVal );
        }

        private Double _getRowDoubleVal( CswEnumNbtPropColumn ColumnName )
        {
            Double ReturnVal = Double.NaN;
            if( _PropRow != null )
            {
                if( _PropsTable.Columns.Contains( ColumnName.ToString() ) && !_PropRow.IsNull( ColumnName.ToString() ) )
                {
                    ReturnVal = CswConvert.ToDouble( _PropRow[ColumnName.ToString()] );
                }
            }
            return ( ReturnVal );
        }
        private DateTime _getRowDateVal( CswEnumNbtPropColumn ColumnName )
        {
            DateTime ReturnVal = DateTime.MinValue;
            if( _PropRow != null )
            {
                if( _PropsTable.Columns.Contains( ColumnName.ToString() ) && !_PropRow.IsNull( ColumnName.ToString() ) )
                {
                    ReturnVal = CswConvert.ToDateTime( _PropRow[ColumnName.ToString()] );
                }
            }
            return ( ReturnVal );
        }

        public CswPrimaryKey NodeId
        {
            get
            {
                //return _getRowIntVal("nodeid");
                CswPrimaryKey ReturnVal = null;
                if( null != _NodeId )
                {
                    ReturnVal = _NodeId;
                }
                else if( null != _PropRow && DBNull.Value != _PropRow["nodeid"] )
                {
                    ReturnVal = new CswPrimaryKey( _PropRow["nodeidtablename"].ToString(), CswConvert.ToInt32( _PropRow["nodeid"] ) );
                }
                return ( ReturnVal );

            }//get
            set
            {
                //SetPropRowValue("nodeid", value);
                _NodeId = value;

                if( null != _PropRow && _NodeId != null )
                {
                    _PropRow["nodeid"] = CswConvert.ToDbVal( _NodeId.PrimaryKey );
                    _PropRow["nodeidtablename"] = _NodeId.TableName;
                }
            }//set
        } //NodeId

        public Int32 NodeTypePropId
        {
            get
            {
                //return _getRowIntVal("nodetypepropid");
                return _NodeTypePropId;
            }
        } //NodeTypePropId

        public CswNbtMetaDataNodeTypeProp NodeTypeProp
        {
            get
            {
                return _CswNbtResources.MetaData.getNodeTypeProp( _NodeTypePropId );
            }
        } //NodeTypeProp

        private bool _ReadOnlyTemporary = false;

        /// <summary>
        /// True if the property's value cannot be changed by the end user
        /// </summary>
        public bool ReadOnly
        {
            get { return NodeTypeProp.ReadOnly || _ReadOnlyTemporary || _getRowBoolVal( CswEnumNbtPropColumn.ReadOnly ); }
        }
        /// <summary>
        /// Mark a Node's property as ReadOnly. 
        /// </summary>
        /// <param name="value">True to write protect, false to enable write</param>
        /// <param name="SaveToDb">If true and the value is different from the value in the database, write this to jct_nodes_props</param>
        public void setReadOnly( bool value, bool SaveToDb )
        {
            _ReadOnlyTemporary = value;
            if( value != _getRowBoolVal( CswEnumNbtPropColumn.ReadOnly ) && SaveToDb )
            {
                SetPropRowValue( CswEnumNbtPropColumn.ReadOnly, value );
            }
        }

        private bool _HiddenTemporary = false;
        /// <summary>
        /// Determines whether property displays.
        /// </summary>
        public bool Hidden
        {
            get { return NodeTypeProp.Hidden || _HiddenTemporary || _getRowBoolVal( CswEnumNbtPropColumn.Hidden ); }
        }
        /// <summary>
        /// Mark a Node's property as Hidden. 
        /// </summary>
        /// <param name="value">True to hide, false to show</param>
        /// <param name="SaveToDb">If true and the value is different from the value in the database, write this to jct_nodes_props</param>
        public void setHidden( bool value, bool SaveToDb )
        {
            _HiddenTemporary = value;
            if( value != _getRowBoolVal( CswEnumNbtPropColumn.Hidden ) && SaveToDb )
            {
                SetPropRowValue( CswEnumNbtPropColumn.Hidden, value );
            }
        }

        /// <summary>
        /// Determines whether to treat the property as required, temporarily
        /// </summary>
        public bool TemporarilyRequired = false;

        /// <summary>
        /// If the property value comes from an audit record (rather than new)
        /// </summary>
        public bool AuditChanged
        {
            get
            {
                bool ret = false;
                if( _PropRow != null && _PropRow.Table.Columns.Contains( "auditchanged" ) )
                {
                    ret = CswConvert.ToBoolean( _PropRow["auditchanged"] );
                }
                return ret;
            }
        }

        public string GetPropRowValue( CswEnumNbtPropColumn Column )
        {
            string ret = string.Empty;
            if( Column == CswEnumNbtPropColumn.Field1 )
            {
                ret = Field1;
            }
            else if( Column == CswEnumNbtPropColumn.Field1_FK )
            {
                ret = CswConvert.ToString( Field1_Fk );
            }
            else if( Column == CswEnumNbtPropColumn.Field1_Numeric )
            {
                ret = CswConvert.ToString( Field1_Numeric );
            }
            else if( Column == CswEnumNbtPropColumn.Field1_Date )
            {
                ret = CswConvert.ToString( Field1_Date );
            }
            else if( Column == CswEnumNbtPropColumn.Field2 )
            {
                ret = Field2;
            }
            else if( Column == CswEnumNbtPropColumn.Field2_Date )
            {
                ret = CswConvert.ToString( Field2_Date );
            }
            else if( Column == CswEnumNbtPropColumn.Field2_Numeric )
            {
                ret = CswConvert.ToString( Field2_Numeric );
            }
            else if( Column == CswEnumNbtPropColumn.Field3 )
            {
                ret = Field3;
            }
            else if( Column == CswEnumNbtPropColumn.Field4 )
            {
                ret = Field4;
            }
            else if( Column == CswEnumNbtPropColumn.Field5 )
            {
                ret = Field5;
            }
            else if( Column == CswEnumNbtPropColumn.Gestalt )
            {
                ret = Gestalt;
            }
            else if( Column == CswEnumNbtPropColumn.ClobData )
            {
                ret = ClobData;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid PropColumn", "CswNbtNodePropData.GetPropRowValue() found an unhandled PropColumn: " + Column.ToString() );
            }
            return ret;
        } // GetPropRowValue()

        public DateTime GetPropRowValueDate( CswEnumNbtPropColumn Column )
        {
            DateTime ret = DateTime.MinValue;
            if( Column == CswEnumNbtPropColumn.Field1_Date )
            {
                ret = Field1_Date;
            }
            else if( Column == CswEnumNbtPropColumn.Field2_Date )
            {
                ret = Field2_Date;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid PropColumn", "CswNbtNodePropData.GetPropRowValueDate() found an unhandled PropColumn: " + Column.ToString() );
            }
            return ret;
        } // GetPropRowValueDate()

        [DebuggerStepThrough]
        public string GetOriginalPropRowValue( CswEnumNbtPropColumn Column )
        {
            // see case 22613
            string ret = string.Empty;
            if( null != _PropRow )
            {
                try
                {
                    ret = _PropRow[Column.ToString(), DataRowVersion.Original].ToString();
                }
                catch( System.Data.VersionNotFoundException )
                {
                    ret = _PropRow[Column.ToString()].ToString();
                }
            }
            return ret;
        }

        public string Field1
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Field1 ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field1, value ); }
        }

        public Int32 Field1_Fk
        {
            get { return _getRowIntVal( CswEnumNbtPropColumn.Field1_FK ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field1_FK, value ); }
        }

        public Double Field1_Numeric
        {
            get { return _getRowDoubleVal( CswEnumNbtPropColumn.Field1_Numeric ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field1_Numeric, value ); }
        }

        public DateTime Field1_Date
        {
            get { return _getRowDateVal( CswEnumNbtPropColumn.Field1_Date ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field1_Date, value ); }
        }

        public string Field2
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Field2 ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field2, value ); }
        }

        public DateTime Field2_Date
        {
            get { return _getRowDateVal( CswEnumNbtPropColumn.Field2_Date ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field2_Date, value ); }
        }

        public Double Field2_Numeric
        {
            get { return _getRowDoubleVal( CswEnumNbtPropColumn.Field2_Numeric ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field2_Numeric, value ); }
        }

        public string Field3
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Field3 ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field3, value ); }
        }

        public string Field4
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Field4 ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field4, value ); }
        }

        public string Field5
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Field5 ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Field5, value ); }
        }

        public bool PendingUpdate
        {
            get { return _getRowBoolVal( CswEnumNbtPropColumn.PendingUpdate ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.PendingUpdate, value ); }
        }

        public string Gestalt
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.Gestalt ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.Gestalt, value ); }
        }

        public string ClobData
        {
            get { return ( _getRowStringVal( CswEnumNbtPropColumn.ClobData ) ); }
            set { SetPropRowValue( CswEnumNbtPropColumn.ClobData, value ); }
        }

        public Int32 JctNodePropId
        {
            get
            {
                Int32 ReturnVal = Int32.MinValue;
                if( _PropRow != null )
                {
                    if( _PropsTable.Columns.Contains( "jctnodepropid" ) && !_PropRow.IsNull( "jctnodepropid" ) )
                    {
                        ReturnVal = CswConvert.ToInt32( _PropRow["jctnodepropid"].ToString() );
                    }
                }
                return ( ReturnVal );
            }
        }//JctNodePropId prop

        public void copy( CswNbtNodePropData Source )
        {
            //Implementing FieldType specific behavior here. Blame Steve.
            if( null != Source.NodeTypeProp && Source.NodeTypeProp.getFieldTypeValue() == CswEnumNbtFieldType.ViewReference )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( Source.NodeTypeProp.DefaultValue.AsViewReference.ViewId );
                CswNbtView ViewCopy = new CswNbtView( _CswNbtResources );
                ViewCopy.saveNew( View.ViewName, View.Visibility, View.VisibilityRoleId, View.VisibilityUserId, View );
                //ViewCopy.save();
                this.Field1_Fk = ViewCopy.ViewId.get();
            }
            else
            {
                SetPropRowValue( CswEnumNbtPropColumn.Field1_FK, Source.Field1_Fk );
            }

            SetPropRowValue( CswEnumNbtPropColumn.Field1, Source.Field1 );
            SetPropRowValue( CswEnumNbtPropColumn.Field2, Source.Field2 );
            SetPropRowValue( CswEnumNbtPropColumn.Field3, Source.Field3 );
            SetPropRowValue( CswEnumNbtPropColumn.Field4, Source.Field4 );
            SetPropRowValue( CswEnumNbtPropColumn.Field5, Source.Field5 );
            SetPropRowValue( CswEnumNbtPropColumn.Field1_Date, Source.Field1_Date );
            SetPropRowValue( CswEnumNbtPropColumn.Field2_Date, Source.Field2_Date );
            SetPropRowValue( CswEnumNbtPropColumn.Field1_Numeric, Source.Field1_Numeric );
            SetPropRowValue( CswEnumNbtPropColumn.Field2_Numeric, Source.Field2_Numeric );
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Source.Gestalt );
            SetPropRowValue( CswEnumNbtPropColumn.ClobData, Source.ClobData );
        }

        public void ClearValue()
        {
            this.Field1 = string.Empty;
            this.Field2 = string.Empty;
            this.Field3 = string.Empty;
            this.Field4 = string.Empty;
            this.Field5 = string.Empty;
            this.Field1_Fk = Int32.MinValue;
            this.Field1_Date = DateTime.MinValue;
            this.Field2_Date = DateTime.MinValue;
            this.Field1_Numeric = Double.NaN;
            this.Field2_Numeric = Double.NaN;
            this.Gestalt = string.Empty;
            this.ClobData = string.Empty;

            WasModified = true;
            WasModifiedForNotification = true;
        }

        public void ClearBlob()
        {
            CswTableUpdate blobDataTU = _CswNbtResources.makeCswTableUpdate( "clearBlob", "blob_data" );
            DataTable blobDataTbl = blobDataTU.getTable( "where jctnodepropid = " + JctNodePropId );

            foreach( DataRow Row in blobDataTbl.Rows )
            {
                if( false == Row.IsNull( "blobdata" ) )
                {
                    WasModified = true;
                    WasModifiedForNotification = true;
                }
                Row.Delete();
            }
            blobDataTU.update( blobDataTbl );
        }


    }//CswNbtNodePropData

}//namespace ChemSW.Nbt.PropTypes
