using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImporterFactory
    {

        public static ICswImporter make( ImportTablePopulationMode ImportTablePopulationMode, CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, StatusUpdateHandler OnStatusUpdate, ImportPhaseHandler OnImportPhaseChange, CswNbtImportStatus CswNbtImportStatus )
        {
            ICswImporter ReturnVal = null;

            CswImportExportStatusReporter CswImportExportStatusReporter = new CswImportExportStatusReporter( OnStatusUpdate, OnImportPhaseChange, CswNbtResources, CswNbtImportStatus );

            ICswImportTablePopulator CswImportTablePopulator = null;
            switch( ImportTablePopulationMode )
            {

                //case ImportAlgorithm.Legacy:
                //    ReturnVal = new CswImporterHashTables( CswNbtResources, CswNbtImportExportFrame, OnStatusUpdate );
                //    break;


                case ImportTablePopulationMode.FromXml:
                    CswImportTablePopulator = new CswImportTablePopulatorFromXml( CswNbtResources, CswNbtImportExportFrame, CswImportExportStatusReporter, CswNbtImportStatus );
                    break;

                case ImportTablePopulationMode.FromRapidLoaderXls:
                    CswImportTablePopulator = new CswImportTablePopulatorFromRapidLoader( CswNbtResources, CswNbtImportExportFrame, CswImportExportStatusReporter, CswNbtImportStatus );
                    break;

                default:
                    throw ( new CswDniException( "Unknown populator altorithm: " + ImportTablePopulationMode.ToString() ) );

            }//switch

            
            ReturnVal = new CswImporterNodeLoader( CswNbtResources, CswNbtImportExportFrame, CswImportExportStatusReporter, CswNbtImportStatus, CswImportTablePopulator );

            return ( ReturnVal );
        }


    } // ICswImporter
} // namespace ChemSW.Nbt
