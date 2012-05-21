using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchRow
    {
        private CswNbtResources _CswNbtResources;

        private CswTableUpdate _BatchTableUpdate;
        private DataTable _BatchTable;
        private DataRow _BatchRow;

        /// <summary>
        /// Restore an existing batch row from the database
        /// </summary>
        public CswNbtBatchRow( CswNbtResources CswNbtResources, Int32 BatchId )
        {
            _CswNbtResources = CswNbtResources;
            _BatchTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtBatchManager_update", "batch" );
            _BatchTable = _BatchTableUpdate.getTable( "batchid", BatchId );
            if( _BatchTable.Rows.Count > 0 )
            {
                _BatchRow = _BatchTable.Rows[0];
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Invalid Batch Operation", "CswNbtBatchRow got an invalid batchid: " + BatchId.ToString() );
            }
        }

        /// <summary>
        /// Makes a new batch operation instance in the database
        /// </summary>
        public CswNbtBatchRow( CswNbtResources CswNbtResources, 
                               NbtBatchOpName BatchOpName, 
                               string BatchData, 
                               CswPrimaryKey UserId = null, 
                               Int32 Priority = Int32.MinValue )
        {
            _CswNbtResources = CswNbtResources;
            _BatchTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtBatchManager_new", "batch" );
            _BatchTable = _BatchTableUpdate.getEmptyTable();
            _BatchRow = _BatchTable.NewRow();

            BatchData = BatchData;
            //EndDate = DateTime.MinValue;
            //Log = string.Empty;
            OpName = BatchOpName;
            Priority = Priority;
            //StartDate = DateTime.Now;
            Status = NbtBatchOpStatus.Pending;
            UserId = UserId ?? _CswNbtResources.CurrentNbtUser.UserId;
            
            _BatchTable.Rows.Add( _BatchRow );
            save();
        }



        /// <summary>
        /// For use by CswNbtBatchOps to add a log message
        /// </summary>
        public void appendToLog( string Message )
        {
            Log += DateTime.Now.ToString() + ": " + Message + "\n";
        }


        /// <summary>
        /// Mark an operation started
        /// </summary>
        public void start()
        {
            if( NbtBatchOpStatus.Processing != Status )
            {
                appendToLog( "Operation started." );
                StartDate = DateTime.Now;
                Status = NbtBatchOpStatus.Processing;
                save();
            }
        }

        /// <summary>
        /// Mark an operation finished
        /// </summary>
        public void finish()
        {
            appendToLog( "Operation Complete." );
            EndDate = DateTime.Now;
            Status = NbtBatchOpStatus.Completed;
            save();
        }

        /// <summary>
        /// Mark an operation errored
        /// </summary>
        public void error( Exception ex )
        {
            appendToLog( "Error: " + ex.Message + "; " + ex.StackTrace );
            Status = NbtBatchOpStatus.Error;
            save();
        }

        /// <summary>
        /// Save changes
        /// </summary>
        public void save()
        {
            _BatchTableUpdate.update( _BatchTable );
        }


        #region Row data interfaces

        public Int32 BatchId
        {
            get { return CswConvert.ToInt32( _BatchRow["batchid"] ); }
            set { _BatchRow["batchid"] = CswConvert.ToDbVal( value ); }
        }
        public NbtBatchOpName OpName
        {
            get { return (NbtBatchOpName) _BatchRow["opname"].ToString(); }
            set { _BatchRow["opname"] = CswConvert.ToDbVal( value.ToString() ); }
        }
        public string BatchData
        {
            get { return _BatchRow["batchdata"].ToString(); }
            set { _BatchRow["batchdata"] = CswConvert.ToDbVal( value ); }
        }
        public DateTime StartDate
        {
            get { return CswConvert.ToDateTime( _BatchRow["startdate"] ); }
            set { _BatchRow["startdate"] = CswConvert.ToDbVal( value ); }
        }
        public DateTime EndDate
        {
            get { return CswConvert.ToDateTime( _BatchRow["enddate"] ); }
            set { _BatchRow["enddate"] = CswConvert.ToDbVal( value ); }
        }
        public CswPrimaryKey UserId
        {
            get { return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _BatchRow["userid"] ) ); }
            set { _BatchRow["userid"] = CswConvert.ToDbVal( value.PrimaryKey ); }
        }
        public string Log
        {
            get { return _BatchRow["log"].ToString(); }
            set { _BatchRow["log"] = CswConvert.ToDbVal( value ); }
        }
        public Int32 Priority
        {
            get { return CswConvert.ToInt32( _BatchRow["priority"] ); }
            set { _BatchRow["priority"] = CswConvert.ToDbVal( value ); }
        }
        public NbtBatchOpStatus Status
        {
            get { return (NbtBatchOpStatus) _BatchRow["status"].ToString(); }
            set { _BatchRow["status"] = CswConvert.ToDbVal( value.ToString() ); }
        }

        #endregion Row data interfaces

    } // class CswNbtBatchRow
} // namespace ChemSW.Nbt.Batch
