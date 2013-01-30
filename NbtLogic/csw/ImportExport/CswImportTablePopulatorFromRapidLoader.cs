
using ChemSW.Nbt.Schema;
namespace ChemSW.Nbt.ImportExport
{

    public class CswImportTablePopulatorFromRapidLoader : ICswImportTablePopulator
    {


        private CswNbtResources _CswNbtResources = null;
        private CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;

        private CswNbtImportOptions _CswNbtImportOptions = null;

        private CswNbtImportStatus _CswNbtImportStatus = null;
        private CswImporterDbTables _CswImporterDbTables = null;


        public CswImportTablePopulatorFromRapidLoader( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter, CswNbtImportStatus CswNbtImportStatus )
        {
            _CswNbtImportStatus = CswNbtImportStatus;
            _CswNbtImportOptions = new CswNbtImportOptions(); //This will be passed in as a ctor arg

            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );


            _CswImporterDbTables = new CswImporterDbTables( CswNbtResources, _CswNbtImportOptions );


        }

        private bool _Stop = false;
        public bool Stop
        {
            set { _Stop = value; }
            get { return ( _Stop ); }
        }


        public bool loadImportTables( ref string Msg )
        {
            bool ReturnVal = true;

            return ( ReturnVal );

        }//loadImportTables


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
