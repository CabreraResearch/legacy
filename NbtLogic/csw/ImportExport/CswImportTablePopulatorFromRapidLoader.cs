
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Schema;

//using Microsoft.Office.Interop.Excel;

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


        private CswNbtMetaDataForSpreadSheetColReader _CswNbtMetaDataForSpreadSheetColReader = null;

        private Dictionary<Int32, CswNbtMetaDataForSpreadSheetCol> _MetaDataByColumnIndex = new Dictionary<int, CswNbtMetaDataForSpreadSheetCol>();

        public CswImportTablePopulatorFromRapidLoader( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter, CswNbtImportStatus CswNbtImportStatus )
        {
            _CswNbtImportStatus = CswNbtImportStatus;
            _CswNbtImportOptions = new CswNbtImportOptions(); //This will be passed in as a ctor arg

            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );


            _CswImporterDbTables = new CswImporterDbTables( CswNbtResources, _CswNbtImportOptions );

            _CswNbtMetaDataForSpreadSheetColReader = new CswNbtMetaDataForSpreadSheetColReader( _CswNbtResources );
        }

        private bool _Stop = false;
        public bool Stop
        {
            set { _Stop = value; }
            get { return ( _Stop ); }
        }

        private ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;


        private Dictionary<Int32, CswNbtMetaDataForSpreadSheetCol> _metaDataForSpreadSheet = new Dictionary<int, CswNbtMetaDataForSpreadSheetCol>();
        public bool loadImportTables( ref string Msg )
        {
            bool ReturnVal = true;

            ////**************************************************************************
            ////Begin: Set up datatable of excel sheet 
            /// 

            string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _CswNbtImportExportFrame.FilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
            ExcelConn.Open();

            DataTable RapidLoaderMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
            if( null == RapidLoaderMetaDataTable )
            {
                throw new CswDniException( ErrorType.Error, "Could not process the uploaded file: " + _CswNbtImportExportFrame.FilePath, "GetOleDbSchemaTable failed to parse a valid XLS file." );
            }

            string FirstSheetName = RapidLoaderMetaDataTable.Rows[0]["TABLE_NAME"].ToString();

            OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
            OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
            DataAdapter.SelectCommand = SelectCommand;

            DataTable RapidLoaderDataTable = new DataTable();

            DataAdapter.Fill( RapidLoaderDataTable );


            ////End: Set up datatable of excel sheet 
            ////**************************************************************************


            //********************************************************************************************************************
            //Begin: Set up NBT field-types per-prop mapping


            List<string> PropColumnNames = new List<string>();
            int NodeTypePropRowIdx = 1;
            foreach( DataColumn CurrentDataColumn in RapidLoaderDataTable.Columns )
            {

                if(RapidLoaderDataTable.Columns.IndexOf( CurrentDataColumn ) > 0 )
                {
                        
                    string ErrorMessage = string.Empty;
                    string NodeTypeNameCandidate = CurrentDataColumn.ColumnName.ToLower();

                    if( false == NodeTypeNameCandidate.ToLower().Contains( "garbage" ) )
                    {

                        string NodeTypePropNameCandidate = RapidLoaderDataTable.Rows[NodeTypePropRowIdx][CurrentDataColumn].ToString().ToLower();


                        NodeTypeNameCandidate = Regex.Replace( NodeTypeNameCandidate, @"[\d-]", string.Empty );

                        CswNbtMetaDataForSpreadSheetCol CswNbtMetaDataForSpreadSheetCol = null;
                        Int32 ColumnIndex = RapidLoaderDataTable.Columns.IndexOf(CurrentDataColumn);
                        if (null != (CswNbtMetaDataForSpreadSheetCol = _CswNbtMetaDataForSpreadSheetColReader.read(NodeTypeNameCandidate, NodeTypePropNameCandidate, ref ErrorMessage)))
                        {

                            _metaDataForSpreadSheet.Add(ColumnIndex, CswNbtMetaDataForSpreadSheetCol);
                        }
                        else
                        {
                            _CswImportExportStatusReporter.reportError("Unable to map nodetype and proptype at column " + ColumnIndex.ToString() + ": " + ErrorMessage);
                        }

                    }//if the column is meant to be interpreted

                }//if we're not at the first column
            }//iterate meta data columns


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


                    foreach( CswNbtMetaDataForSpreadSheetCol CurrentColMetaData in _MetaDataByColumnIndex.Values )
                    {
                        List<string> FieldTypeColNames = new List<string>( CurrentColMetaData.FieldTypeColNames );
                        foreach( string CurrentFieldTypeColName in FieldTypeColNames )
                        {
                            if( false == PropColumnNames.Contains( CurrentFieldTypeColName ) )
                            {
                                PropColumnNames.Add( CurrentFieldTypeColName );
                            }
                        }
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
