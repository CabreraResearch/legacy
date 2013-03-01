using System;
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

        public delegate void StoreDataFinishEvent( string ImportDataTableName );
        public StoreDataFinishEvent OnStoreDataFinish;

        public WorkerThread()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.SchemInit, SetupMode.NbtExe, false, false );
            _CswNbtResources.InitCurrentUser = InitUser;
            _CswNbtResources.AccessId = "1";

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

        public delegate void storeDataHandler( string DataFilePath );
        public void storeData( string DataFilePath )
        {
            _Importer.storeData( DataFilePath );
            OnStoreDataFinish( _Importer.ImportDataTableName );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }

        public delegate void importRowsHandler( string ImportDataTableName, Int32 rows );
        public void importRows( string ImportDataTableName, Int32 rows )
        {
            _Importer.ImportDataTableName = ImportDataTableName;
            _Importer.ImportRows( rows );
            
            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }

        public delegate void readBindingsHandler( string BindingsFilePath );
        public void readBindings( string BindingsFilePath )
        {
            _Importer.readBindings( BindingsFilePath );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            OnFinish();
        }
    }
}
