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
            CswTableSelect JobSelect = _CswNbtResources.makeCswTableSelect( "CswNbtImportDataJob_select", CswNbtImportTables.ImportDataJob.TableName );
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
        }


        [DataMember]
        [Description( "Primary Key" )]
        public Int32 ImportDataJobId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.importdatajobid] ); }
        }

        [DataMember]
        [Description( "Uploaded file name" )]
        public string FileName
        {
            get { return _row[CswNbtImportTables.ImportDataJob.filename].ToString(); }
        }

        [DataMember]
        [Description( "Date import was uploaded" )]
        public DateTime DateStarted
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.datestarted] ); }
        }

        [DataMember]
        [Description( "Date the import was finished" )]
        public DateTime DateEnded
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.dateended] ); }
        }

        [DataMember]
        [Description( "Primary key of User responsible for import" )]
        public Int32 UserId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.userid] ); }
        }

        [DataMember]
        [Description( "Username of User responsible for import" )]
        public string UserName
        {
            get
            {
                string ret = string.Empty;
                CswNbtObjClassUser UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
                if( null != UserNode )
                {
                    ret = UserNode.Username;
                }
                return ret;
            }
        } // UserName

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

        public void getStatus( out Int32 RowsPending,
                               out Int32 RowsTotal,
                               out Int32 RowsError,
                               out Int32 ItemsPending,
                               out Int32 ItemsTotal )
        {
            RowsPending = 0;
            RowsTotal = 0;
            RowsError = 0;
            ItemsPending = 0;
            ItemsTotal = 0;

            foreach( CswNbtImportDataMap Map in Maps )
            {
                Int32 thisRowsPending = 0;
                Int32 thisRowsTotal = 0;
                Int32 thisRowsError = 0;
                Int32 thisItemsPending = 0;
                Int32 thisItemsTotal = 0;

                Map.getStatus( out thisRowsPending,
                               out thisRowsTotal,
                               out thisRowsError,
                               out thisItemsPending,
                               out thisItemsTotal );

                RowsPending += thisRowsPending;
                RowsTotal += thisRowsTotal;
                RowsError += thisRowsError;
                ItemsPending += thisItemsPending;
                ItemsTotal += thisItemsTotal;
            }
        } // getStatus()

    } // class
} // namespace
