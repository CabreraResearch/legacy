using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImporterFactory
    {

        public static ICswImporter make( ImportAlgorithm ImportAlgorithm, CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, StatusUpdateHandler OnStatusUpdate, ImportPhaseHandler OnImportPhaseChange, CswNbtImportStatus CswNbtImportStatus )
        {
            ICswImporter ReturnVal = null;


            switch( ImportAlgorithm )
            {

                case ImportAlgorithm.Legacy:
                    ReturnVal = new CswImporterHashTables( CswNbtResources, CswNbtImportExportFrame, OnStatusUpdate );
                    break;

                case ImportAlgorithm.DbTableBased:
                    //CswNbtImportStatus CswNbtImportStatus = new CswNbtImportStatus( CswNbtResources );
                    ReturnVal = new CswImporterDbTables( CswNbtResources, CswNbtImportExportFrame, new CswImportExportStatusReporter( OnStatusUpdate, OnImportPhaseChange, CswNbtResources, CswNbtImportStatus ), CswNbtImportStatus );
                    break;

                default:
                    throw ( new CswDniException( "Unknown Import altorithm: " + ImportAlgorithm.ToString() ) );
            }//switch

            return ( ReturnVal );
        }


    } // ICswImporter
} // namespace ChemSW.Nbt
