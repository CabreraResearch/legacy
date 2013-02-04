
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Schema;
using ChemSW.DB;

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


        private List<string> _ExcludedNodeTypes = new List<string>();
        private List<string> _KnownOutageProperties = new List<string>();

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

            _ExcludedNodeTypes.Add( "garbage" );
            _ExcludedNodeTypes.Add( "biological" );
            _ExcludedNodeTypes.Add( "equipment" );

            //As destination properties for these columsn are found, they will be moved to the mapping dictionary in reader
            _KnownOutageProperties.Add( "healthcode" );
            _KnownOutageProperties.Add( "firecode" );
            _KnownOutageProperties.Add( "reactivecode" );
            _KnownOutageProperties.Add( "target_organs" );
            _KnownOutageProperties.Add( "model" );

        }

        private bool _Stop = false;
        public bool Stop
        {
            set { _Stop = value; }
            get { return ( _Stop ); }
        }

        private ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;


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

                if( RapidLoaderDataTable.Columns.IndexOf( CurrentDataColumn ) > 0 )
                {

                    string ErrorMessage = string.Empty;
                    string NodeTypeNameCandidate = CurrentDataColumn.ColumnName.ToLower();

                    if( true == NodeTypeNameCandidate.ToLower().Contains( "material" ) )//for now just need
                    {

                        string NodeTypePropNameCandidate = RapidLoaderDataTable.Rows[NodeTypePropRowIdx][CurrentDataColumn].ToString().ToLower();

                        if( string.Empty != NodeTypePropNameCandidate && false == _KnownOutageProperties.Contains( NodeTypePropNameCandidate ) )
                        {


                            NodeTypeNameCandidate = Regex.Replace( NodeTypeNameCandidate, @"[\d-]", string.Empty );

                            CswNbtMetaDataForSpreadSheetCol CswNbtMetaDataForSpreadSheetCol = null;
                            Int32 ColumnIndex = RapidLoaderDataTable.Columns.IndexOf( CurrentDataColumn );
                            if( null != ( CswNbtMetaDataForSpreadSheetCol = _CswNbtMetaDataForSpreadSheetColReader.read( NodeTypeNameCandidate, NodeTypePropNameCandidate, ref ErrorMessage ) ) )
                            {

                                _MetaDataByColumnIndex.Add( ColumnIndex, CswNbtMetaDataForSpreadSheetCol );
                            }
                            else
                            {
                                _CswImportExportStatusReporter.reportError( "Unable to map nodetype and proptype at column " + ColumnIndex.ToString() + ": " + ErrorMessage );
                            }

                        }//if the prop is not empty

                    }//if the column is meant to be interpreted

                }//if we're not at the first column

            }//iterate meta data columns


            //End: Set up NBT field-types per-prop mapping
            //********************************************************************************************************************


            //********************************************************************************************************************
            //Begin: Build import tables (especially props) based on mapping

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

            }//if import tables have not yet been built

            //End: Build import tables (especially props) based on mapping
            //********************************************************************************************************************


            //********************************************************************************************************************
            //Begin: Load import tables from spreadsheet

            Int32 CurrentArbitraryImportId = 0;
            CswTableUpdate ImportNodesUpdater = _CswNbtResources.makeCswTableUpdate( "importer_update_" + CswImporterDbTables.TblName_ImportNodes, CswImporterDbTables.TblName_ImportNodes );
            CswTableUpdate ImportPropsUpdater = _CswNbtResources.makeCswTableUpdate( "importer_update_" + CswImporterDbTables.TblName_ImportProps, CswImporterDbTables.TblName_ImportProps );
            Dictionary<string, DataRow> CurrentNodeUpdateRowsByNodeTypeName = new Dictionary<string, DataRow>();
            for( Int32 RlXlsRowIdx = 2; RlXlsRowIdx < RapidLoaderDataTable.Rows.Count; RlXlsRowIdx++ )
            {

                DataTable CurrentUpdateImportNodesTable = ImportNodesUpdater.getEmptyTable();
                DataTable CurrentUpdateImportPropsTable = ImportPropsUpdater.getEmptyTable();


                DataRow CurrentRlXlsRow = RapidLoaderDataTable.Rows[RlXlsRowIdx];

                for( Int32 RlXlsColIdx = 2; RlXlsColIdx < RapidLoaderDataTable.Columns.Count; RlXlsColIdx++ )
                {
                    DataRow CurrentImportNodesUpdateRow = null;
                    if( true == _MetaDataByColumnIndex.ContainsKey( RlXlsColIdx ) )
                    {
                        CswNbtMetaDataForSpreadSheetCol CurrentColMetaData = _MetaDataByColumnIndex[RlXlsColIdx];
                        DataColumn CurrentRlxsCol = RapidLoaderDataTable.Columns[RlXlsColIdx];

                        CurrentArbitraryImportId++;

                        if( false == CurrentNodeUpdateRowsByNodeTypeName.ContainsKey( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName ) )
                        {
                            DataRow DataRow = CurrentUpdateImportNodesTable.NewRow();
                            CurrentNodeUpdateRowsByNodeTypeName.Add( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName, DataRow );
                            DataRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentArbitraryImportId;
                            DataRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentArbitraryImportId;
                            DataRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
                            DataRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypeName] = CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName;

                            //CswImporterDbTables.
                        }//if we don't have a CswImporterDbTables.TblName_ImportNodes table insert row for the current column's nodetype

                        CurrentImportNodesUpdateRow = CurrentNodeUpdateRowsByNodeTypeName[CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName];

                        DataRow CurrentImportPropsUpdateRow = CurrentUpdateImportPropsTable.NewRow();

                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentImportNodesUpdateRow[CswImporterDbTables._ColName_ImportNodeId];
                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentImportNodesUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique];

                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName;
                        CurrentImportPropsUpdateRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropId;
                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();




                        string CurrentRlXlsCellVal = CurrentRlXlsRow[CurrentRlxsCol].ToString();
                        if( 1 == CurrentColMetaData.FieldTypeColNames.Count )
                        {
                        }
                        else
                        {
                            _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": needs special handling" );

                        }//if-else fieldtype colnames == 1

                        CurrentUpdateImportPropsTable.Rows.Add( CurrentImportPropsUpdateRow );
                        ImportPropsUpdater.update( CurrentUpdateImportPropsTable );


                    }//if we have meta data for this column



                }//iterate rapid loader spreadsheet cols


                foreach( DataRow CurrentImportNodesDataRow in CurrentNodeUpdateRowsByNodeTypeName.Values )
                {
                    CurrentUpdateImportNodesTable.Rows.Add( CurrentImportNodesDataRow );

                }//iterate node rows

                ImportNodesUpdater.update( CurrentUpdateImportNodesTable );

                //make sure these have a value in the nodes rows 'ere we commit: 
                // _ColName_Nodes_NodeName
                // 

            }//iterate rapid loader spreadsheet rows
            //Begin: Load import tables from spreadsheet
            //********************************************************************************************************************


            return ( ReturnVal );

        }//loadImportTables


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
