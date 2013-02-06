
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Schema;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
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
            _KnownOutageProperties.Add( "unitofmeasurename" );
            _KnownOutageProperties.Add( "pathname" );




            //WE know the deestination property, but the field type calls for special handling of the columns
            _KnownOutageProperties.Add( "nfpacode" ); // ==> NFPA
            _KnownOutageProperties.Add( "Supplier" );
            _KnownOutageProperties.Add( "un_no" ); // ==> UN Code
            //            _KnownOutageProperties.Add( "barcodeid" );//==> Barcode <== the Oracle no likey the "Number" field type col 

        }

        private bool _Stop = false;
        public bool Stop
        {
            set { _Stop = value; }
            get { return ( _Stop ); }
        }

        private ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;


        public bool loadImportTables( ref string Message )
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

            string xls_key_isdata = "ISDATA";
            string xls_key_vendor = "SUPPLIER";
            string xls_key_tradename = "MATERIALNAME";
            string xls_key_productno = "PRODUCTNO";

            string FirstSheetName = RapidLoaderMetaDataTable.Rows[0]["TABLE_NAME"].ToString();

            OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
            string select_statement = "SELECT * FROM [" + FirstSheetName + "] ORDER BY " + xls_key_isdata + "," + xls_key_vendor + "," + xls_key_tradename + "," + xls_key_productno + " ASC";
            OleDbCommand SelectCommand = new OleDbCommand( select_statement, ExcelConn );
            //            OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "] ", ExcelConn );
            DataAdapter.SelectCommand = SelectCommand;

            DataTable RapidLoaderDataTable = new DataTable();

            DataAdapter.Fill( RapidLoaderDataTable );


            ////End: Set up datatable of excel sheet 
            ////**************************************************************************


            //********************************************************************************************************************
            //Begin: Set up NBT field-types per-prop mapping


            List<string> PropColumnNames = new List<string>();
            int NodeTypeRowIdx = 1;
            foreach( DataColumn CurrentDataColumn in RapidLoaderDataTable.Columns )
            {

                if( RapidLoaderDataTable.Columns.IndexOf( CurrentDataColumn ) > 0 )
                {

                    string ErrorMessage = string.Empty;
                    string NodeTypeNameCandidate = RapidLoaderDataTable.Rows[NodeTypeRowIdx][CurrentDataColumn].ToString().ToLower();

                    if( true == NodeTypeNameCandidate.ToLower().Contains( "material" ) || true == NodeTypeNameCandidate.ToLower().Contains( "container" ) )//material includes vendor info
                    {

                        string NodeTypePropNameCandidate = CurrentDataColumn.ColumnName.ToLower();

                        if( false == _KnownOutageProperties.Contains( NodeTypePropNameCandidate ) )
                        {


                            NodeTypeNameCandidate = Regex.Replace( NodeTypeNameCandidate, @"[\d-]", string.Empty ); //remove otiose digits from nodetypename

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
                            if( false == PropColumnNames.Contains( CurrentFieldTypeColName ) && ( "Number" != CurrentFieldTypeColName ) )
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

            string current_material_compount_id = string.Empty;
            string current_vendor = string.Empty;
            DataRow CurrentVendorRow = null;
            DataRow CurrentMaterialRow = null;
            DataRow CurrentContainerRow = null;

            bool TestCaseStop = false;
            for( Int32 RlXlsIdx = 2; RlXlsIdx < RapidLoaderDataTable.Rows.Count && false == TestCaseStop; RlXlsIdx++ )
            {
                DataRow CurrentRlXlsRow = RapidLoaderDataTable.Rows[RlXlsIdx];
                DataTable ImportNodesTable = ImportNodesUpdater.getEmptyTable();
                DataTable ImportPropsTable = ImportPropsUpdater.getEmptyTable();
                if( current_vendor != CurrentRlXlsRow[xls_key_vendor].ToString() )
                {
                    current_vendor = CurrentRlXlsRow[xls_key_vendor].ToString();


                    CswNbtMetaDataNodeType VendorNodeType = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                    CswNbtMetaDataNodeTypeProp VendorNameNodeTypeProp = VendorNodeType.getNodeTypeProp( "Vendor Name" );

                    if( null != VendorNodeType && null != VendorNameNodeTypeProp )
                    {

                        _addRowOfNodeType( CurrentRlXlsRow, VendorNodeType, ImportNodesTable, null, ref CurrentVendorRow );

                        DataRow VendorNamePropRow = ImportPropsTable.NewRow();
                        ImportPropsTable.Rows.Add( VendorNamePropRow );

                        VendorNamePropRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentVendorRow[CswImporterDbTables._ColName_ImportNodeId];
                        VendorNamePropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentVendorRow[CswImporterDbTables._ColName_ImportNodeId];

                        VendorNamePropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = VendorNameNodeTypeProp.PropName;
                        VendorNamePropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = VendorNameNodeTypeProp.PropId;
                        VendorNamePropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
                        //VendorNamePropRow[CurrentColMetaData.FieldTypeColNames[0]] = current_vendor; 
                    }
                    else
                    {
                        _CswImportExportStatusReporter.reportError( "Destination schema does not have the vendor node type and/or vendor name prop" );
                    }//if-else we have vendor node type


                }//if we need to create a curent vendor record

                if( null != CurrentVendorRow )
                {

                    CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtResources.MetaData.getNodeType( "Chemical" );

                    if( null != ChemicalNodeType )
                    {

                        ChemSW.Core.CswDelimitedString MaterialCompoundId = new Core.CswDelimitedString( '-' );
                        MaterialCompoundId.Add( current_vendor );
                        MaterialCompoundId.Add( CurrentRlXlsRow[xls_key_productno].ToString() );
                        MaterialCompoundId.Add( CurrentRlXlsRow[xls_key_tradename].ToString() );

                        if( current_material_compount_id != MaterialCompoundId.ToString() )
                        {
                            current_material_compount_id = MaterialCompoundId.ToString();
                            _addRowOfNodeType( CurrentRlXlsRow, ChemicalNodeType, ImportNodesTable, ImportPropsTable, ref CurrentMaterialRow, CurrentVendorRow, "Vendor" );

                        }//if we need to create a new material record

                    }
                    else
                    {
                        _CswImportExportStatusReporter.reportError( "Could not retrieve chemical node type" );
                    }//if-else we found

                }
                else
                {
                    _CswImportExportStatusReporter.reportError( "Destination schema does not have the vendor node type and/or vendor name prop" );
                }

                if( null != CurrentMaterialRow )
                {
                    CswNbtMetaDataNodeType ContainerNodeType = _CswNbtResources.MetaData.getNodeType( "Container" );

                    _addRowOfNodeType( CurrentRlXlsRow, ContainerNodeType, ImportNodesTable, ImportPropsTable, ref CurrentContainerRow, CurrentMaterialRow, "Material" );

                }
                else
                {
                    _CswImportExportStatusReporter.reportError( "Unable to proceed with container creation: there is no material row" );
                }//if-else we have a material row


                ImportNodesUpdater.update( ImportNodesTable );
                ImportPropsUpdater.update( ImportPropsTable );

                _CswNbtResources.commitTransaction();

                if( RlXlsIdx > 500 )
                {
                    TestCaseStop = true;
                }
            }//iterate all rapid loader rows

            //string xls_key_vendor = "SUPPLIER";
            //string xls_key_tradename = "MATERIALNAME";
            //string xls_key_productno = "PRODUCTNO";


            //for( Int32 RlXlsRowIdx = 2; RlXlsRowIdx < RapidLoaderDataTable.Rows.Count; RlXlsRowIdx++ )
            //{
            //    DataRow CurrentRlXlsRow = RapidLoaderDataTable = RapidLoaderDataTable.Rows[RlXlsRowIdx];

            //    CswTableSelect ImportNodesSelect = _CswNbtResources.makeCswTableSelect( "importer_select_" + CswImporterDbTables.TblName_ImportNodes, CswImporterDbTables.TblName_ImportNodes );

            //    string PartNumberColName = string.Empty;
            //    string TradenameColName = string.Empty;
            //    string SupplierColName = string.Empty;

            //    foreach( KeyValuePair<string, string> CurrentKvPair in _CswNbtMetaDataForSpreadSheetColReader._NodeTypePropNameMapper )
            //    {
            //        switch( CurrentKvPair.Value )
            //        {
            //            case "Part Number":
            //                PartNumberColName = CurrentKvPair.Key;
            //                break;

            //            case "Tradename":
            //                TradenameColName = CurrentKvPair.Key;
            //                break;

            //            case "Supplier":
            //                SupplierColName = CurrentKvPair.Key;
            //                break;

            //            default:
            //                break; //do nothing
            //        }//switch on value

            //    }//iterate kv pairs 

            //    string CurrentRlXlsPartNumberVal = CurrentRlXlsRow[PartNumberColName];
            //    string CurrentRlXlsTradeNameVal = CurrentRlXlsRow[TradenameColName];
            //    string CurrentRlXlsSupplierVal = CurrentRlXlsRow[SupplierColName];

            //    string WhereClause = @" where " + PartNumberColName + "='" + CurrentRlXlsRow[PartNumberColName] + "'" +
            //                                     TradenameColName + "='" + CurrentRlXlsRow[TradenameColName];
            //    DataTable ExistingMaterialRecord = ImportNodesSelect.getTable( WhereClause );

            //}//iterate rapid loader table


            /*
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
                            CurrentUpdateImportNodesTable.Rows.Add( DataRow );

                            CurrentNodeUpdateRowsByNodeTypeName.Add( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName, DataRow );
                            DataRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentArbitraryImportId;
                            //DataRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentArbitraryImportId;
                            DataRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
                            DataRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypeName] = CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName;


                            //Not sure where else to put this or if it's worth polymorphising it
                            //(there will be other relationships)
                            if( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName == "Container" )
                            {
                                DataRow RelationshipPropRow = CurrentUpdateImportPropsTable.NewRow();
                                CurrentUpdateImportPropsTable.Rows.Add( RelationshipPropRow );
                                RelationshipPropRow[CswImporterDbTables._ColName_ImportNodeId] = DataRow[CswImporterDbTables._ColName_ImportNodeId];
                                RelationshipPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = DataRow[CswImporterDbTables._ColName_ImportNodeId];
                                RelationshipPropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = CswNbtObjClassContainer.PropertyName.Material;

                                //not orny safe yet:
                                RelationshipPropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = _CswNbtResources.MetaData.getObjectClassProp( _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.ContainerClass ), "material" ).PropId;

                                RelationshipPropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                            }

                            //CswImporterDbTables.
                        }//if we don't have a CswImporterDbTables.TblName_ImportNodes table insert row for the current column's nodetype

                        CurrentImportNodesUpdateRow = CurrentNodeUpdateRowsByNodeTypeName[CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName];

                        DataRow CurrentImportPropsUpdateRow = CurrentUpdateImportPropsTable.NewRow();

                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentImportNodesUpdateRow[CswImporterDbTables._ColName_ImportNodeId];
                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentImportNodesUpdateRow[CswImporterDbTables._ColName_ImportNodeId];

                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName;
                        CurrentImportPropsUpdateRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropId;
                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();




                        string CurrentRlXlsCellVal = CurrentRlXlsRow[CurrentRlxsCol].ToString();
                        if( 1 == CurrentColMetaData.FieldTypeColNames.Count )
                        {
                            string FieldTypeName = CurrentColMetaData.FieldTypeColNames[0];
                            if( true == CurrentUpdateImportPropsTable.Columns.Contains( FieldTypeName ) )
                            {
                                CurrentImportPropsUpdateRow[FieldTypeName] = CurrentRlXlsCellVal;
                            }
                            else
                            {
                                _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": the " + CswImporterDbTables.TblName_ImportProps + " table does not have a column for field type " + FieldTypeName );
                            }//if-else we have a column for our prop's field type
                        }
                        else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "barcode" )
                        {

                            CurrentImportPropsUpdateRow["BARCODE"] = CurrentRlXlsCellVal.Replace( "'", "" );
                        }
                        else
                        {
                            _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": needs special handling" );
                        }


                        CurrentUpdateImportPropsTable.Rows.Add( CurrentImportPropsUpdateRow );
                        ImportPropsUpdater.update( CurrentUpdateImportPropsTable );


                    }//if we have meta data for this column



                }//iterate rapid loader spreadsheet cols


                ImportNodesUpdater.update( CurrentUpdateImportNodesTable );
                _CswNbtResources.commitTransaction();

                CurrentNodeUpdateRowsByNodeTypeName.Clear();
                CurrentUpdateImportNodesTable = null;
                CurrentUpdateImportPropsTable = null;

                //make sure these have a value in the nodes rows 'ere we commit: 
                // _ColName_Nodes_NodeName
                // 


                //For Testing purpsoes only
                if( CurrentArbitraryImportId > 100 )
                {

                    ////Message = "Ended early for test purpsoes";
                    //ReturnVal = false;

                    break;
                }//

            }//iterate rapid loader spreadsheet rows
             */
            //Begin: Load import tables from spreadsheet
            //********************************************************************************************************************





            return ( ReturnVal );

        }//loadImportTables

        private Int32 _ArbitrarySequentialUniqueId = 0;
        private void _addRowOfNodeType( DataRow RlXlsDataRow, CswNbtMetaDataNodeType NodeTypeToAdd, DataTable ImportNodesTable, DataTable ImportPropsTable, ref DataRow ImportNodesRow, DataRow RelationshipDestinationRow = null, string RelationshipPropName = "" )
        {

            ImportNodesRow = ImportNodesTable.NewRow();
            ImportNodesTable.Rows.Add( ImportNodesRow );

            _ArbitrarySequentialUniqueId++;

            ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId] = _ArbitrarySequentialUniqueId;
            //NodeRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = CurrentArbitraryImportId;
            ImportNodesRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
            ImportNodesRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypeName] = NodeTypeToAdd.NodeTypeName;

            if( null != ImportPropsTable )
            {
                bool RelationshipPropWasCreated = false;
                for( Int32 RlXlsColIdx = 2; RlXlsColIdx < RlXlsDataRow.Table.Columns.Count; RlXlsColIdx++ )
                {
                    if( true == _MetaDataByColumnIndex.ContainsKey( RlXlsColIdx ) )
                    {
                        DataColumn CurrentRlxsCol = RlXlsDataRow.Table.Columns[RlXlsColIdx];
                        CswNbtMetaDataForSpreadSheetCol CurrentColMetaData = _MetaDataByColumnIndex[RlXlsColIdx];

                        if( CurrentColMetaData.CswNbtMetaDataNodeType == NodeTypeToAdd )
                        {

                            DataRow CurrentImportPropsUpdateRow = ImportPropsTable.NewRow();
                            ImportPropsTable.Rows.Add( CurrentImportPropsUpdateRow );

                            CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ImportNodeId] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];
                            CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];

                            CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName;
                            CurrentImportPropsUpdateRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropId;
                            CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                            string CurrentRlXlsCellVal = RlXlsDataRow[CurrentRlxsCol].ToString();
                            if( 1 == CurrentColMetaData.FieldTypeColNames.Count )
                            {
                                string FieldTypeName = CurrentColMetaData.FieldTypeColNames[0];
                                if( true == ImportPropsTable.Columns.Contains( FieldTypeName ) )
                                {
                                    CurrentImportPropsUpdateRow[FieldTypeName] = CurrentRlXlsCellVal;
                                }
                                else
                                {
                                    _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": the " + CswImporterDbTables.TblName_ImportProps + " table does not have a column for field type " + FieldTypeName );
                                }//if-else we have a column for our prop's field type
                            }
                            else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "barcode" )
                            {

                                CurrentImportPropsUpdateRow["BARCODE"] = CurrentRlXlsCellVal.Replace( "'", "" );
                            }
                            else
                            {
                                _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": needs special handling" );
                            }

                            if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == RelationshipPropName )
                            {
                                if( null != RelationshipDestinationRow )
                                {
                                    CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];
                                    RelationshipPropWasCreated = true;
                                }
                                //else
                                //{
                                //    _CswImportExportStatusReporter.reportError( "A relationship property was specified (" + RelationshipPropName + "), but no destination relationship row was provided" );
                                //}//if-else we have a desintation row for the relationship

                            }//if the current prop is a relationship prop

                        }//if the current column matches the target node type

                    }//if this is a column we understand

                }//iterate prop columns

                //DataRow RelationshipDestinationRow = null, string RelationshipPropName = string.Empty 

                if( false == RelationshipPropWasCreated && null != RelationshipDestinationRow && "" != RelationshipPropName )
                {
                    CswNbtMetaDataNodeTypeProp RelationshipProp = NodeTypeToAdd.getNodeTypeProp( RelationshipPropName );
                    if( null != RelationshipProp )
                    {
                        DataRow RelationshipPropRow = ImportPropsTable.NewRow();
                        ImportPropsTable.Rows.Add( RelationshipPropRow );

                        RelationshipPropRow[CswImporterDbTables._ColName_ImportNodeId] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];
                        RelationshipPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];

                        RelationshipPropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = RelationshipProp.PropName;
                        RelationshipPropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = RelationshipProp.PropId;
                        RelationshipPropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                        RelationshipPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];
                        RelationshipPropWasCreated = true;

                    }

                }//if we haven't yet created the relationship prop

            }//if we are adding props

        }//_addRowOfNodeType()


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
