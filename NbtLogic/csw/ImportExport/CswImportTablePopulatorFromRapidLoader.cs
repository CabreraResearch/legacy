
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
        private List<string> _ColsWithoutDestinationProp = new List<string>();


        private const string xls_key_isdata = "ISDATA";
        private const string xls_key_vendor = "SUPPLIER";
        private const string xls_key_tradename = "MATERIALNAME";
        private const string xls_key_productno = "PRODUCTNO";
        private const string xls_key_nfpa_nfpacode = "nfpacode";
        private const string xls_key_nfpa_healthcode = "healthcode";
        private const string xls_key_nfpa_firecode = "firecode";
        private const string xls_key_nfpa_reactivecode = "reactivecode";




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




            _ColsWithoutDestinationProp.Add( xls_key_nfpa_healthcode ); //<== these three along with NFPA code column make  up the NFPA property
            _ColsWithoutDestinationProp.Add( xls_key_nfpa_firecode );   //    this will require special treatment :-( 
            _ColsWithoutDestinationProp.Add( xls_key_nfpa_reactivecode );


            //As destination properties for these columsn are found, they will be moved to the mapping dictionary in reader



            _KnownOutageProperties.Add( "target_organs" );
            _KnownOutageProperties.Add( "model" );
            _KnownOutageProperties.Add( "pathname" );


            _KnownOutageProperties.Add( "unitofmeasurename" ); //together with "netquantity" becomes the "quantity" property in NBT



            //WE know the deestination property, but the field type calls for special handling of the columns
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

                        if( false == _KnownOutageProperties.Contains( NodeTypePropNameCandidate ) && false == _ColsWithoutDestinationProp.Contains( NodeTypePropNameCandidate ) )
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
                        VendorNamePropRow["TEXT"] = current_vendor;
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
                            _addRowOfNodeType( CurrentRlXlsRow, ChemicalNodeType, ImportNodesTable, ImportPropsTable, ref CurrentMaterialRow, CurrentVendorRow, "Supplier" );

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

                if( RlXlsIdx > 50 )
                {
                    TestCaseStop = true;
                }
            }//iterate all rapid loader rows


            //End: Load import tables from spreadsheet
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
                            else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "supplier" )
                            {
                                CurrentImportPropsUpdateRow["NAME"] = CurrentRlXlsCellVal;
                            }
                            else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "nfpa" )
                            {
                                CurrentImportPropsUpdateRow["Special"] = RlXlsDataRow[xls_key_nfpa_nfpacode];

                                //
                                CurrentImportPropsUpdateRow["Flammability"] = RlXlsDataRow[xls_key_nfpa_firecode];
                                CurrentImportPropsUpdateRow["Reactivity"] = RlXlsDataRow[xls_key_nfpa_reactivecode];
                                CurrentImportPropsUpdateRow["Health"] = RlXlsDataRow[xls_key_nfpa_healthcode];

                            }
                            else
                            {
                                _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": needs special handling" );
                            }

                            if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == RelationshipPropName )
                            {
                                if( null != RelationshipDestinationRow )
                                {
                                    CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = RelationshipDestinationRow[CswImporterDbTables._ColName_ImportNodeId];
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

                        RelationshipPropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = RelationshipProp.PropName;
                        RelationshipPropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = RelationshipProp.PropId;
                        RelationshipPropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                        RelationshipPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = RelationshipDestinationRow[CswImporterDbTables._ColName_ImportNodeId];
                        RelationshipPropWasCreated = true;

                    }
                    else
                    {
                        _CswImportExportStatusReporter.reportError( NodeTypeToAdd.NodeTypeName + ": The specified relationship property(" + RelationshipProp + ") does not exist on this nodetype" );
                    }

                }//if we haven't yet created the relationship prop

            }//if we are adding props

        }//_addRowOfNodeType()


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
