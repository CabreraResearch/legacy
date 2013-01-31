
using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.Schema;

using Microsoft.Office.Interop.Excel;

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

        private ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;


        private Dictionary<Int32,CswNbtMetaDataForSpreadSheetCol> _metaDataForSpreadSheet = new Dictionary<int, CswNbtMetaDataForSpreadSheetCol>();
        public bool loadImportTables( ref string Msg )
        {
            bool ReturnVal = true;

            //********************************************************************************************************************
            //Begin: Set up the excel paraphanelia
            Microsoft.Office.Interop.Excel.Application excell_Application = null;
            Microsoft.Office.Interop.Excel.Workbooks excell_Workbooks = null;
            Microsoft.Office.Interop.Excel._Workbook excell_OneWorkBook = null;
            Microsoft.Office.Interop.Excel.Sheets excell_Worksheets = null;
            Microsoft.Office.Interop.Excel._Worksheet excell_OneWorksheet = null;
            Microsoft.Office.Interop.Excel.Range excell_Range = null;
            object MissingExcellValue = System.Reflection.Missing.Value;


            excell_Application = new Microsoft.Office.Interop.Excel.Application();
            excell_OneWorkBook = excell_Workbooks.Open( _CswNbtImportExportFrame.FilePath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, true, "XLNormalLoad" );
            excell_Worksheets = excell_OneWorkBook.Worksheets;

            excell_OneWorksheet = (Microsoft.Office.Interop.Excel.Worksheet) excell_Worksheets.get_Item( 1 );
            //End: Set up the excel paraphanelia
            //********************************************************************************************************************


            //********************************************************************************************************************
            //Begin: Set up NBT field-types per-prop mapping
            excell_Range = excell_OneWorksheet.UsedRange;

            List<string> PropColumnNames = new List<string>();


            int NodeTypeNameRow = 1;
            int NodeTypePropNameRow = 3;

            for( Int32 ColIdx = 2; ColIdx <= excell_Range.Columns.Count; ColIdx++)
            {
                string CandidateNodeTypeName = excell_Range.Cells[NodeTypeNameRow, ColIdx];
                string CandidateNodeTypePropName = excell_Range.Cells[NodeTypePropNameRow, ColIdx];



            }//iterate meta data rows of spreadsheet



            //End: Set up NBT field-types per-prop mapping
            //********************************************************************************************************************

            string ImportTableInconsistencyMessage = string.Empty;
            if( _CswImporterDbTables.areImportTablesAbsent( ref ImportTableInconsistencyMessage ) )
            {
                if( string.Empty == ImportTableInconsistencyMessage )
                {
                    _LastCompletedProcessPhase = ImportProcessPhase.LoadingInputFile;

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                    _CswNbtSchemaModTrnsctn.beginTransaction();



                    foreach( DataColumn CurrentColumn in TableOfPropsFromXml.Columns )
                    {
                        PropColumnNames.Add( CurrentColumn.ColumnName );
                    }


                    _CswImporterDbTables.makeImportTables( PropColumnNames );

                    _CswNbtSchemaModTrnsctn.commitTransaction();

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

                }
                else
                {
                    _CswImportExportStatusReporter.reportError( "The import tables are in an inconsistent state: " + ImportTableInconsistencyMessage );

                }//if-else import tables are inconsistent

            }//if import tables are absent


            return ( ReturnVal );

        }//loadImportTables


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
