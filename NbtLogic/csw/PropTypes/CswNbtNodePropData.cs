using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.DB;

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
        public bool SetPropRowValue( CswNbtSubField.PropColumn column, object value )
        {
            bool ret = false;
            object dbval = CswConvert.ToDbVal( value );

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
					WasModified = true;
					_PropRow[column.ToString()] = CswConvert.ToDbVal( value );
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
        private CswNbtResources _CswNbtResources;

        public bool WasModified
        {
            set
            {
                if( !SuspendModifyTracking )
                {
                    // We never set it false, so that modifying Field1 but not modifying Field2 will not accidentally clear the modification flag.
                    // Use clearModifiedFlag() to clear it on purpose.
                    if( value )
                        _WasModified = true;
                }
            }
            get
            {
                return ( _WasModified );
            }
        }//WasModified

        public void clearModifiedFlag()
        {
            _WasModified = false;
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

        private string _getRowStringVal( CswNbtSubField.PropColumn ColumnName )
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

        private bool _getRowBoolVal( CswNbtSubField.PropColumn ColumnName )
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

        private Int32 _getRowIntVal( CswNbtSubField.PropColumn ColumnName )
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

        private Double _getRowDoubleVal( CswNbtSubField.PropColumn ColumnName )
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
        private DateTime _getRowDateVal( CswNbtSubField.PropColumn ColumnName )
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

        public bool ReadOnly
        {
            get { return _getRowBoolVal( CswNbtSubField.PropColumn.ReadOnly ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.ReadOnly, value ); }
        }//ReadOnly 

        /// <summary>
        /// Determines whether property displays.
        /// </summary>
        public bool Hidden = false;

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

        public string GetPropRowValue( CswNbtSubField.PropColumn Column )
        {
            string ret = string.Empty;
            switch( Column )
            {
                case CswNbtSubField.PropColumn.Field1:
                    ret = Field1;
                    break;
                case CswNbtSubField.PropColumn.Field1_FK:
                    ret = CswConvert.ToString( Field1_Fk );
                    break;
                case CswNbtSubField.PropColumn.Field1_Numeric:
                    ret = CswConvert.ToString( Field1_Numeric );
                    break;
                case CswNbtSubField.PropColumn.Field1_Date:
                    ret = CswConvert.ToString( Field1_Date );
                    break;
                case CswNbtSubField.PropColumn.Field2:
                    ret = Field2;
                    break;
                case CswNbtSubField.PropColumn.Field2_Date:
                    ret = CswConvert.ToString( Field2_Date );
                    break;
                case CswNbtSubField.PropColumn.Field3:
                    ret = Field3;
                    break;
                case CswNbtSubField.PropColumn.Field4:
                    ret = Field4;
                    break;
                case CswNbtSubField.PropColumn.Field5:
                    ret = Field5;
                    break;
                case CswNbtSubField.PropColumn.Gestalt:
                    ret = Gestalt;
                    break;
                case CswNbtSubField.PropColumn.ClobData:
                    ret = ClobData;
                    break;
                default:
					throw new CswDniException( ErrorType.Error, "Invalid PropColumn", "CswNbtNodePropData.GetPropRowValue() found an unhandled PropColumn: " + Column.ToString() );
            }
            return ret;
        }

		public string GetOriginalPropRowValue( CswNbtSubField.PropColumn Column )
		{
			// see case 22613
			string ret = string.Empty;
			try
			{
				ret = _PropRow[Column.ToString(), DataRowVersion.Original].ToString();
			}
			catch( System.Data.VersionNotFoundException ex )
			{
				ret = _PropRow[Column.ToString()].ToString();
			}
			return ret;
		}

        public string Field1
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Field1 ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field1, value ); }
        }

        public Int32 Field1_Fk
        {
            get { return _getRowIntVal( CswNbtSubField.PropColumn.Field1_FK ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field1_FK, value ); }
        }

        public Double Field1_Numeric
        {
            get { return _getRowDoubleVal( CswNbtSubField.PropColumn.Field1_Numeric ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field1_Numeric, value ); }
        }

        public DateTime Field1_Date
        {
            get { return _getRowDateVal( CswNbtSubField.PropColumn.Field1_Date ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field1_Date, value ); }
        }

        public string Field2
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Field2 ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field2, value ); }
        }

        public DateTime Field2_Date
        {
            get { return _getRowDateVal( CswNbtSubField.PropColumn.Field2_Date ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field2_Date, value ); }
        }

        public string Field3
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Field3 ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field3, value ); }
        }

        public string Field4
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Field4 ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field4, value ); }
        }

        public string Field5
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Field5 ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Field5, value ); }
        }

        public bool PendingUpdate
        {
            get { return _getRowBoolVal( CswNbtSubField.PropColumn.PendingUpdate ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.PendingUpdate, value ); }
        }

        public string Gestalt
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.Gestalt ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, value ); }
        }

        public string ClobData
        {
            get { return ( _getRowStringVal( CswNbtSubField.PropColumn.ClobData ) ); }
            set { SetPropRowValue( CswNbtSubField.PropColumn.ClobData, value ); }
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
            if( null != Source.NodeTypeProp && Source.NodeTypeProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.ViewReference )
            {
				CswNbtView View = _CswNbtResources.ViewSelect.restoreView( Source.NodeTypeProp.DefaultValue.AsViewReference.ViewId );
                CswNbtView ViewCopy = new CswNbtView( _CswNbtResources );
                ViewCopy.makeNew( View.ViewName, View.Visibility, View.VisibilityRoleId, View.VisibilityUserId, View );
                ViewCopy.save();
                this.Field1_Fk = ViewCopy.ViewId.get();
            }
            else
            {
				SetPropRowValue( CswNbtSubField.PropColumn.Field1_FK, Source.Field1_Fk );
            }

			SetPropRowValue( CswNbtSubField.PropColumn.Field1, Source.Field1 );
			SetPropRowValue( CswNbtSubField.PropColumn.Field2, Source.Field2 );
			SetPropRowValue( CswNbtSubField.PropColumn.Field3, Source.Field3 );
			SetPropRowValue( CswNbtSubField.PropColumn.Field4, Source.Field4 );
			SetPropRowValue( CswNbtSubField.PropColumn.Field5, Source.Field5 );
			SetPropRowValue( CswNbtSubField.PropColumn.Field1_Date, Source.Field1_Date );
			SetPropRowValue( CswNbtSubField.PropColumn.Field2_Date, Source.Field2_Date );
			SetPropRowValue( CswNbtSubField.PropColumn.Field1_Numeric, Source.Field1_Numeric );
			SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Source.Gestalt );
			SetPropRowValue( CswNbtSubField.PropColumn.ClobData, Source.ClobData );
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
            this.Gestalt = string.Empty;
            this.ClobData = string.Empty;

            WasModified = true;
        }

        public void ClearBlob()
        {

            CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "ClearBlob_update", "jct_nodes_props" );
            JctUpdate.AllowBlobColumns = true;
            DataTable JctTable = JctUpdate.getTable( "jctnodepropid", JctNodePropId );

            if( !JctTable.Rows[0].IsNull( "blobdata" ) )
            {
                WasModified = true;
            }

            JctTable.Rows[0]["blobdata"] = DBNull.Value;
            JctUpdate.update( JctTable );
        }


    }//CswNbtNodePropData

}//namespace ChemSW.Nbt.PropTypes
