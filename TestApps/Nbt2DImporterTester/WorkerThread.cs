using System;
using System.Collections.Specialized;
using ChemSW;
using ChemSW.Config;
using ChemSW.Nbt;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace Nbt2DImporterTester
{
    public class WorkerThread
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbt2DImporter _Importer;

        public delegate void FinishEvent();
        public FinishEvent OnFinish;

        public delegate void StoreDataFinishEvent( StringCollection ImportDataTableNames );
        public StoreDataFinishEvent OnStoreDataFinish;

        public delegate void getCountsFinishEvent( Int32 PendingRows, Int32 ErrorRows );
        public getCountsFinishEvent OnGetCountsFinish;
        
        public delegate void importFinishEvent();
        public importFinishEvent OnImportFinish;

        public WorkerThread()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.SchemInit, SetupMode.NbtExe, false, false );
            _CswNbtResources.InitCurrentUser = InitUser;

            _Importer = new CswNbt2DImporter( _CswNbtResources );
            _Importer.Overwrite = true;
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswSystemUserNames.SysUsr__SchemaImport );
        }

        public CswNbt2DImporter.ErrorHandler OnError
        {
            set { _Importer.OnError = value; }
        }
        public CswNbt2DImporter.MessageHandler OnMessage
        {
            set { _Importer.OnMessage = value; }
        }

        public delegate void storeDataHandler( string AccessId, string DataFilePath );
        public void storeData( string AccessId, string DataFilePath )
        {
            _CswNbtResources.AccessId = AccessId;

            _Importer.storeData( DataFilePath );
            OnStoreDataFinish( _Importer.ImportDataTableNames );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }

        public delegate void importRowsHandler( string AccessId, string ImportDataTableName, Int32 rows );
        public void importRows( string AccessId, string ImportDataTableName, Int32 rows )
        {
            _CswNbtResources.AccessId = AccessId;

            _Importer.ImportRows( rows, ImportDataTableName );
            
            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            
            OnFinish();
            OnImportFinish();
        }

        public delegate void readBindingsHandler( string AccessId, string BindingsFilePath );
        public void readBindings( string AccessId, string BindingsFilePath )
        {
            _CswNbtResources.AccessId = AccessId;

            _Importer.readBindings( BindingsFilePath );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }

        public delegate void getCountsHandler( string AccessId, string ImportDataTableName );
        public void getCounts( string AccessId, string ImportDataTableName )
        {
            _CswNbtResources.AccessId = AccessId;

            Int32 PendingRows;
            Int32 ErrorRows;
            _Importer.getCounts( ImportDataTableName, out PendingRows, out ErrorRows );
            OnGetCountsFinish( PendingRows, ErrorRows );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }
    }
}
