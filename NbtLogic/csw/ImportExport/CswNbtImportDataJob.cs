using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.ImportExport
{
    [DataContract]
    public class CswNbtImportDataJob
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDataJob( CswNbtResources CswNbtResources, Int32 ImportJobId )
        {
            CswTableSelect JobSelect = CswNbtResources.makeCswTableSelect( "CswNbtImportDataJob_select", CswNbtImportTables.ImportDataJob.TableName );
            DataTable JobDataTable = JobSelect.getTable( CswNbtImportTables.ImportDataJob.PkColumnName, ImportJobId );
            if( JobDataTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, JobDataTable.Rows[0] );
            }
        }

        public CswNbtImportDataJob( CswNbtResources CswNbtResources, DataRow DataJobRow )
        {
            _finishConstructor( CswNbtResources, DataJobRow );
        }

        private void _finishConstructor( CswNbtResources CswNbtResources, DataRow DataJobRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = DataJobRow;

            // We have to do this up front in order to serialize UserName
            UserName = string.Empty;
            CswNbtObjClassUser UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
            if( null != UserNode )
            {
                UserName = UserNode.Username;
            }
        }


        [DataMember]
        [Description( "Primary Key" )]
        public Int32 ImportDataJobId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.importdatajobid] ); }
            private set { }
        }

        [DataMember]
        [Description( "Uploaded file name" )]
        public string FileName
        {
            get { return _row[CswNbtImportTables.ImportDataJob.filename].ToString(); }
            private set { }
        }

        [DataMember]
        [Description( "Date import was uploaded" )]
        public DateTime DateStarted
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.datestarted] ); }
            private set { }
        }

        [DataMember]
        [Description( "Date the import was finished" )]
        public DateTime DateEnded
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.dateended] ); }
            set 
            {
                CswTableUpdate ImportDataJobUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_DataJob_Update", CswNbtImportTables.ImportDataJob.TableName );
                DataTable ImportDataJobTable = ImportDataJobUpdate.getTable( CswNbtImportTables.ImportDataJob.PkColumnName, this.ImportDataJobId );
                if( ImportDataJobTable.Rows.Count > 0 )
                {
                    ImportDataJobTable.Rows[0][CswNbtImportTables.ImportDataJob.dateended] = CswConvert.ToDbVal( value );
                    ImportDataJobUpdate.update( ImportDataJobTable );
                }
            }
        }

        [DataMember]
        [Description( "Primary key of User responsible for import" )]
        public Int32 UserId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.userid] ); }
            private set { }
        }

        [DataMember]
        [Description( "Username of User responsible for import" )]
        public string UserName;

        private Collection<CswNbtImportDataMap> _Maps = null;
        public Collection<CswNbtImportDataMap> Maps
        {
            get
            {
                if( null == _Maps )
                {
                    _Maps = new Collection<CswNbtImportDataMap>();
                    CswTableSelect MapSelect = _CswNbtResources.makeCswTableSelect( "job_map_select", CswNbtImportTables.ImportDataMap.TableName );
                    DataTable MapTable = MapSelect.getTable( CswNbtImportTables.ImportDataMap.importdatajobid, this.ImportDataJobId );
                    foreach( DataRow MapRow in MapTable.Rows )
                    {
                        _Maps.Add( new CswNbtImportDataMap( _CswNbtResources, MapRow ) );
                    }
                }
                return _Maps;
            }
        }

        public void getStatus( out Int32 RowsDone,
                               out Int32 RowsTotal,
                               out Int32 RowsError,
                               out Int32 ItemsDone,
                               out Int32 ItemsTotal )
        {
            RowsDone = 0;
            RowsTotal = 0;
            RowsError = 0;
            ItemsDone = 0;
            ItemsTotal = 0;

            foreach( CswNbtImportDataMap Map in Maps )
            {
                Int32 thisRowsDone = 0;
                Int32 thisRowsTotal = 0;
                Int32 thisRowsError = 0;
                Int32 thisItemsDone = 0;
                Int32 thisItemsTotal = 0;

                Map.getStatus( out thisRowsDone,
                               out thisRowsTotal,
                               out thisRowsError,
                               out thisItemsDone,
                               out thisItemsTotal );

                RowsDone += thisRowsDone;
                RowsTotal += thisRowsTotal;
                RowsError += thisRowsError;
                ItemsDone += thisItemsDone;
                ItemsTotal += thisItemsTotal;
            }
        } // getStatus()

    } // class
} // namespace
