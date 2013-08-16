using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDataJob
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDataJob( CswNbtResources CswNbtResources, DataRow DataJobRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = DataJobRow;
        }

        public Int32 ImportDataJobId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.importdatajobid] ); }
        }
        public string FileName
        {
            get { return _row[CswNbtImportTables.ImportDataJob.filename].ToString(); }
        }
        public DateTime DateStarted
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.datestarted] ); }
        }
        public DateTime DateEnded
        {
            get { return CswConvert.ToDateTime( _row[CswNbtImportTables.ImportDataJob.dateended] ); }
        }
        public Int32 UserId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataJob.userid] ); }
        }
    }
}
