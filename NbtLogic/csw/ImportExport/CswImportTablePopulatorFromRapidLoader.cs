
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using ChemSW.Core;
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
        private Dictionary<string, Int32> _UofMNodeIdsByUofmName = new Dictionary<string, int>();
        private Dictionary<string, string> _PhysicalStateMap = new Dictionary<string, string>();
        private Dictionary<string, string> _CISProUofMNamesToNbtSizeNames = new Dictionary<string, string>();
        private List<string> _ColsWithoutDestinationProp = new List<string>();
        private Int32 _ContainerOwnerUserNodeId = Int32.MinValue;
        private Int32 _SitePk = Int32.MinValue;


        private const string xls_key_isdata = "ISDATA";
        private const string xls_key_vendor = "SUPPLIER";
        private const string xls_key_tradename = "MATERIALNAME";
        private const string xls_key_productno = "PRODUCTNO";
        private const string xls_key_nfpa_nfpacode = "nfpacode";
        private const string xls_key_nfpa_healthcode = "healthcode";
        private const string xls_key_nfpa_firecode = "firecode";
        private const string xls_key_nfpa_reactivecode = "reactivecode";

        private const string xls_key_size_unitofmeasurename = "unitofmeasurename";
        private const string xls_key_size_catalogno = "catalogno";




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

            _ColsWithoutDestinationProp.Add( xls_key_nfpa_reactivecode );


            //As destination properties for these columsn are found, they will be moved to the mapping dictionary in reader



            _KnownOutageProperties.Add( "target_organs" );
            _KnownOutageProperties.Add( "model" );
            _KnownOutageProperties.Add( "pathname" );
            _KnownOutageProperties.Add( "capacity" );
            _KnownOutageProperties.Add( "package" );


            //WE know the deestination property, but the field type calls for special handling of the columns
            _KnownOutageProperties.Add( "Supplier" );
            _KnownOutageProperties.Add( "un_no" ); // ==> UN Code
            //            _KnownOutageProperties.Add( "barcodeid" );//==> Barcode <== the Oracle no likey the "Number" field type col 



            _CISProUofMNamesToNbtSizeNames.Add( "L", "Liters" );
            _CISProUofMNamesToNbtSizeNames.Add( "ML", "mL" );
            _CISProUofMNamesToNbtSizeNames.Add( "PT", "ounces" ); //<==WRONG for now
            _CISProUofMNamesToNbtSizeNames.Add( "oz", "ounces" ); 
            _CISProUofMNamesToNbtSizeNames.Add( "G", "g" );
            _CISProUofMNamesToNbtSizeNames.Add( "GAL", "gal" );
            _CISProUofMNamesToNbtSizeNames.Add( "KG", "kg" );
            _CISProUofMNamesToNbtSizeNames.Add( "KIT", "kg" );
            _CISProUofMNamesToNbtSizeNames.Add( "GM", "g" );
            _CISProUofMNamesToNbtSizeNames.Add( "ML (each)", "mL" );
            _CISProUofMNamesToNbtSizeNames.Add( "vial", "Each" );
            _CISProUofMNamesToNbtSizeNames.Add( "each", "Each" );
            _CISProUofMNamesToNbtSizeNames.Add( "MG", "mg" );
            _CISProUofMNamesToNbtSizeNames.Add( "LB", "lb" );

            _PhysicalStateMap.Add( "S", CswNbtObjClassMaterial.PhysicalStates.Solid );
            _PhysicalStateMap.Add( "G", CswNbtObjClassMaterial.PhysicalStates.Gas );
            _PhysicalStateMap.Add( "L", CswNbtObjClassMaterial.PhysicalStates.Liquid );


            //_CISProSizeNamesToNbtSizeNames.Add(); 

        }//ctor 

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


            //******** Meta data
            CswNbtMetaDataNodeType VendorNodeType = _CswNbtResources.MetaData.getNodeType( "Vendor" );
            CswNbtMetaDataNodeTypeProp VendorNameNodeTypeProp = VendorNodeType.getNodeTypeProp( "Vendor Name" );

            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtResources.MetaData.getNodeType( "Chemical" );
            CswNbtMetaDataNodeType SizeNodeType = _CswNbtResources.MetaData.getNodeType( "Size" );
            CswNbtMetaDataNodeType ContainerNodeType = _CswNbtResources.MetaData.getNodeType( "Container" );

            CswNbtMetaDataNodeType SiteNodeType = _CswNbtResources.MetaData.getNodeType( "Site" );


            ////**************************************************************************
            ////Begin: Set up NBT field-types per-prop mapping
            /// 

            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure CurrentOfM in UnitOfMeasureOC.getNodes( false, true ) )
            {
                _UofMNodeIdsByUofmName.Add( CurrentOfM.Name.Text, CurrentOfM.NodeId.PrimaryKey );
            }


            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtObjClassUser CurrentUserNode in UserOC.getNodes( false, true ) )
            {
                if( "cispro_admin" == CurrentUserNode.Username.ToLower() )
                {
                    _ContainerOwnerUserNodeId = CurrentUserNode.NodeId.PrimaryKey;
                }
            }

            foreach( CswNbtObjClassLocation LocationNode in SiteNodeType.getNodes( false, true ) )
            {
                if( "Site 1" == LocationNode.Name.Text )
                {
                    _SitePk = LocationNode.NodeId.PrimaryKey;
                }
            }



            //***********************************
            //Set up ADO connection to spread sheet -- we'll use this twice
            string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _CswNbtImportExportFrame.FilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
            ExcelConn.Open();

            DataTable RapidLoaderMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
            if( null == RapidLoaderMetaDataTable )
            {
                throw new CswDniException( ErrorType.Error, "Could not process the uploaded file: " + _CswNbtImportExportFrame.FilePath, "GetOleDbSchemaTable failed to parse a valid XLS file." );
            }


            string FirstSheetName = RapidLoaderMetaDataTable.Rows[0]["TABLE_NAME"].ToString();
            //***********************************




            //**********************
            //retrieve spreadsheet and set up metadata for it
            OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
            //            string select_statement = "SELECT * FROM [" + FirstSheetName + "]  where [CATALOGNO] IS NOT NULL AND [UNITOFMEASURE] IS NOT NULL ORDER BY " + xls_key_isdata + "," + xls_key_vendor + "," + xls_key_tradename + "," + xls_key_productno + " ASC";
            string select_statement = "SELECT * FROM [" + FirstSheetName + "]  ORDER BY " + xls_key_isdata + "," + xls_key_vendor + "," + xls_key_tradename + "," + xls_key_productno + " ASC";
            OleDbCommand SelectCommand = new OleDbCommand( select_statement, ExcelConn );
            //            OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "] ", ExcelConn );
            DataAdapter.SelectCommand = SelectCommand;

            DataTable RapidLoaderDataTable = new DataTable();

            DataAdapter.Fill( RapidLoaderDataTable );



            List<string> PropColumnNames = new List<string>();
            int NodeTypeRowIdx = 0;

            foreach( DataColumn CurrentDataColumn in RapidLoaderDataTable.Columns )
            {

                if( RapidLoaderDataTable.Columns.IndexOf( CurrentDataColumn ) > 0 )
                {

                    string ErrorMessage = string.Empty;
                    string NodeTypeNameCandidate = RapidLoaderDataTable.Rows[NodeTypeRowIdx][CurrentDataColumn].ToString().ToLower();


                    //***** BEGIN: STARK RAVING KLUDGE
                    //For some unknown reason, the blessed select from spreadsheet oblivion is nuking the node type data for these props. 
                    //This is probably some frickin' spreadsheet format voo doo because if you try to change the nodetype name in the 
                    //specified column, you get an error about that column being a double. 
                    //This kludgedelia will make the rest of our algorithm work. Jeeze.
                    if( "netquantity" == CurrentDataColumn.ColumnName.ToString().ToLower() ||
                        "expirationdate" == CurrentDataColumn.ColumnName.ToString().ToLower() ||
                        "receiveddate" == CurrentDataColumn.ColumnName.ToString().ToLower() )
                    {
                        NodeTypeNameCandidate = "container";
                    }
                    else if( "specific_gravity" == CurrentDataColumn.ColumnName.ToString().ToLower() ||
                        "molecular_weight" == CurrentDataColumn.ColumnName.ToString().ToLower() )
                    {
                        NodeTypeNameCandidate = "material";
                    }
                    else if( "capacity" == CurrentDataColumn.ColumnName.ToString().ToLower() )
                    {
                        NodeTypeNameCandidate = "size";
                    }
                    else if( "storpress" == CurrentDataColumn.ColumnName.ToString().ToLower() ||
                        "stortemp" == CurrentDataColumn.ColumnName.ToString().ToLower() ||
                        "usetype" == CurrentDataColumn.ColumnName.ToString().ToLower() )
                    {
                        NodeTypeNameCandidate = "container";
                    }

                    //***** END: STARK RAVING KLUDGE


                    if( true == NodeTypeNameCandidate.ToLower().Contains( "material" ) ||
                        true == NodeTypeNameCandidate.ToLower().Contains( "container" ) ||
                        true == NodeTypeNameCandidate.ToLower().Contains( "size" ) )//material includes vendor info
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

            //*************************************
            //*** Manually add location node types


            //CswNbtMetaDataNodeType SiteNodeType = _CswNbtResources.MetaData.getNodeType( "Site" );
            CswNbtMetaDataNodeType BuildingNodeType = _CswNbtResources.MetaData.getNodeType( "Building" );
            CswNbtMetaDataNodeType RoomNodeType = _CswNbtResources.MetaData.getNodeType( "Room" );
            CswNbtMetaDataNodeType CabinetNodeType = _CswNbtResources.MetaData.getNodeType( "Cabinet" );
            CswNbtMetaDataNodeType ShelfNodeType = _CswNbtResources.MetaData.getNodeType( "Shelf" );


            //*** We are somewhat dundantly creating the spreadsheet col thingies for the locations to get the subfield collection
            //*** However, we are not adding them to the dictionary for the spreadsheet because that would mess up our retreival
            //*** of meta data when iterating the spreadsheet . All we really need here are the subfields to add to the import_props table. 
            //*** we only need for two props on one location -- the rest are the same
            string LocationMetaDataErrorMessage = string.Empty;
            List<CswNbtMetaDataForSpreadSheetCol> LocationMetaData = new List<CswNbtMetaDataForSpreadSheetCol>();
            LocationMetaData.Add( _CswNbtMetaDataForSpreadSheetColReader.read( SiteNodeType.NodeTypeName, "Name", ref LocationMetaDataErrorMessage ) );
            LocationMetaData.Add( _CswNbtMetaDataForSpreadSheetColReader.read( SiteNodeType.NodeTypeName, "Allow Inventory", ref LocationMetaDataErrorMessage ) );






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

                    /// This and previous loop should be encapsualted
                    foreach( CswNbtMetaDataForSpreadSheetCol CurrentColMetaData in LocationMetaData )
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


            // Updaters for the import nodes we just created
            CswTableUpdate ImportNodesUpdater = _CswNbtResources.makeCswTableUpdate( "importer_update_" + CswImporterDbTables.TblName_ImportNodes, CswImporterDbTables.TblName_ImportNodes );
            CswTableUpdate ImportPropsUpdater = _CswNbtResources.makeCswTableUpdate( "importer_update_" + CswImporterDbTables.TblName_ImportProps, CswImporterDbTables.TblName_ImportProps );


            ////**************************************************************************
            ////Begin: Build location tree
            /// 
            //CswImporterLocationTree




            OleDbDataAdapter DataAdapterPathnames = new OleDbDataAdapter();
            string select_statement_pathanes = "SELECT DISTINCT [PATHNAME] FROM [" + FirstSheetName + "]  ORDER BY [PATHNAME]";
            OleDbCommand SelectCommandPathnames = new OleDbCommand( select_statement_pathanes, ExcelConn );
            //            OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "] ", ExcelConn );
            DataAdapterPathnames.SelectCommand = SelectCommandPathnames;

            DataTable RapidLoaderDataTablePathNames = new DataTable();

            DataAdapterPathnames.Fill( RapidLoaderDataTablePathNames );


            CswImporterLocationTree CswImporterLocationTree = new ImportExport.CswImporterLocationTree();

            //populate the location tree
            foreach( DataRow CurrentPathnameRow in RapidLoaderDataTablePathNames.Rows )
            {
                string CurrentPathName = CurrentPathnameRow["pathname"].ToString();
                if( string.Empty != CurrentPathName )
                {
                    ChemSW.Core.CswCommaDelimitedString CurrentCommaDelimtedString = new Core.CswCommaDelimitedString();
                    CurrentCommaDelimtedString.FromString( CurrentPathName );
                    CswImporterLocationTree.AddPath( CurrentCommaDelimtedString );
                }//if we nave a string

            }//

            //create site
            DataTable LocationNodesTable = ImportNodesUpdater.getEmptyTable();
            DataTable LocationPropsTable = ImportPropsUpdater.getEmptyTable();
            
            //add all locations in breadth first order
            foreach( CswImporterLocationTree.LocationEntry Entry in CswImporterLocationTree.BreadthFirst() )
            {
                CswNbtMetaDataNodeType LocationNT = null;
                bool AllowInventory = true;
                switch( Entry.Level )
                {
                    case 1:
                        LocationNT = BuildingNodeType;
                        break;
                    case 2:
                        LocationNT = RoomNodeType;
                        break;
                    case 3:
                        LocationNT = CabinetNodeType;
                        break;
                    case 4:
                        LocationNT = ShelfNodeType;
                        break;
                }

                Int32 ParentImportNodeId;
                if( null != Entry.Parent )
                {
                    ParentImportNodeId = Entry.Parent.ImportNodeId;
                }
                else
                {
                    ParentImportNodeId = _SitePk;
                }
                DataRow NewLocationRow = _addLocation( LocationNT, LocationNodesTable, LocationPropsTable, Entry.Name, AllowInventory, ParentImportNodeId );
                Entry.ImportNodeId = CswConvert.ToInt32( NewLocationRow[CswImporterDbTables._ColName_ImportNodeId] );
            }

            ImportNodesUpdater.update( LocationNodesTable );
            ImportPropsUpdater.update( LocationPropsTable );


            /// 
            ////End: Build location tree
            ////**************************************************************************

            //********************************************************************************************************************
            //Begin: Load import tables from spreadsheet


            string current_material_compount_id = string.Empty;
            string current_vendor = string.Empty;
            DataRow CurrentVendorRow = null;
            DataRow CurrentMaterialRow = null;
            DataRow CurrentContainerRow = null;
            DataRow CurrentSizeRow = null;
            Dictionary<string, DataRow> SizesRowsByCompoundId = new Dictionary<string, DataRow>();








            //***********************************************************************************************
            //BEGIN: Create import user

            //CswNbtMetaDataNodeType UserNodeType = _CswNbtResources.MetaData.getNodeType( "User" );
            //CswNbtMetaDataNodeTypeProp UserNameNodeTypeProp = UserNodeType.getNodeTypeProp( "Username" );
            //CswNbtMetaDataNodeTypeProp UserNameNodeTypeProp = UserNodeType.getNodeTypeProp( "Role" );


            //DataTable ImporUsertNodeTable = ImportNodesUpdater.getEmptyTable();
            //DataTable ImportUserPropsTable = ImportPropsUpdater.getEmptyTable();

            //CswNbtObjClassRole UserRole = null;
            //CswNbtMetaDataObjectClass RoleObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            //foreach( CswNbtObjClassRole CurrentRole in RoleObjectClass.getNodes( false, true ) )
            //{
            //    if( "CISPro_General" == CurrentRole.Name )
            //    {
            //        UserRole = CurrentRole;
            //    }
            //}


            //_addRowOfNodeType( null, UserNodeType, ImporUsertNodeTable, null, ref ImportUserRow );

            //DataRow UserNamePropRow = ImporUsertNodeTable.NewRow();
            //ImportUserPropsTable.Rows.Add( UserNamePropRow );

            //UserNamePropRow[CswImporterDbTables._ColName_ImportNodeId] = ImportUserRow[CswImporterDbTables._ColName_ImportNodeId];
            //UserNamePropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ImportUserRow[CswImporterDbTables._ColName_ImportNodeId];
            //UserNamePropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = UserNameNodeTypeProp.PropName;
            //UserNamePropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = UserNameNodeTypeProp.PropId;
            //UserNamePropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
            //UserNamePropRow["TEXT"] = "CisProImportUser";



            //END: Create import user
            //***********************************************************************************************


            //***********************************************************************************************
            //BEGIN: Set Import USer



            //END: 
            //***********************************************************************************************


            bool TestCaseStop = false;
            for( Int32 RlXlsIdx = 2; RlXlsIdx < RapidLoaderDataTable.Rows.Count && false == TestCaseStop; RlXlsIdx++ )
            {

                DataRow CurrentRlXlsRow = RapidLoaderDataTable.Rows[RlXlsIdx];
                DataTable ImportNodesTable = ImportNodesUpdater.getEmptyTable();
                DataTable ImportPropsTable = ImportPropsUpdater.getEmptyTable();


                if( current_vendor != CurrentRlXlsRow[xls_key_vendor].ToString() )
                {
                    current_vendor = CurrentRlXlsRow[xls_key_vendor].ToString();



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

                ChemSW.Core.CswDelimitedString CurrentSizeCompoundId = null;
                ChemSW.Core.CswDelimitedString MaterialCompoundId = null;
                if( null != CurrentVendorRow )
                {


                    if( null != ChemicalNodeType )
                    {

                        MaterialCompoundId = new Core.CswDelimitedString( '-' );
                        MaterialCompoundId.Add( current_vendor );
                        MaterialCompoundId.Add( CurrentRlXlsRow[xls_key_productno].ToString() );
                        MaterialCompoundId.Add( CurrentRlXlsRow[xls_key_tradename].ToString() );

                        if( current_material_compount_id != MaterialCompoundId.ToString() )
                        {
                            current_material_compount_id = MaterialCompoundId.ToString();
                            _addRowOfNodeType( CurrentRlXlsRow, ChemicalNodeType, ImportNodesTable, ImportPropsTable, ref CurrentMaterialRow, CurrentVendorRow, "Supplier" );
                            SizesRowsByCompoundId.Clear();

                        }//if we need to create a new material record

                    }
                    else
                    {
                        _CswImportExportStatusReporter.reportError( "Could not retrieve chemical node type" );
                    }//if-else we found

                    if( CurrentMaterialRow != null )
                    {

                        CurrentSizeCompoundId = new Core.CswDelimitedString( '-' );
                        CurrentSizeCompoundId.Add( CurrentRlXlsRow[xls_key_size_catalogno].ToString() );
                        CurrentSizeCompoundId.Add( CurrentRlXlsRow[xls_key_size_unitofmeasurename].ToString() );

                        if( false == SizesRowsByCompoundId.ContainsKey( CurrentSizeCompoundId.ToString() ) )
                        {
                            _addRowOfNodeType( CurrentRlXlsRow, SizeNodeType, ImportNodesTable, ImportPropsTable, ref CurrentSizeRow, CurrentMaterialRow, "Material" );
                            SizesRowsByCompoundId.Add( CurrentSizeCompoundId.ToString(), CurrentSizeRow );

                        }
                        else
                        {
                            CurrentSizeRow = SizesRowsByCompoundId[CurrentSizeCompoundId.ToString()];

                        }//if-else we already have the size row

                    }//if we have a material


                }
                else
                {
                    _CswImportExportStatusReporter.reportError( "Destination schema does not have the vendor node type and/or vendor name prop" );
                }

                if( null != CurrentMaterialRow )
                {
                    string UomName = "";
                    if( null != CurrentSizeCompoundId )
                    {
                        UomName = CurrentSizeCompoundId[1]; //living dangerously; another kludge
                    }

                    _addRowOfNodeType( CurrentRlXlsRow, ContainerNodeType, ImportNodesTable, ImportPropsTable, ref CurrentContainerRow, CurrentMaterialRow, "Material", UomName );

                    _addRelationshipForRow( CurrentContainerRow, ContainerNodeType, ImportNodesTable, ImportPropsTable, CswConvert.ToInt32(CurrentSizeRow[CswImporterDbTables._ColName_ImportNodeId]), "Size" );

                    CswCommaDelimitedString pathCDS = new CswCommaDelimitedString();
                    pathCDS.FromString(CurrentRlXlsRow["pathname"].ToString());
                    CswImporterLocationTree.LocationEntry pathEntry = CswImporterLocationTree.FindPath( pathCDS );

                    if( null != pathEntry )
                    {
                        _addRelationshipForRow( CurrentContainerRow, ContainerNodeType, ImportNodesTable, ImportPropsTable, pathEntry.ImportNodeId, "Location" );
                    }

                    //manually add user
                    CswNbtMetaDataNodeTypeProp ContainerOwnerNodeTypeProp = ContainerNodeType.getNodeTypeProp( "Owner" );
                    DataRow OwnerPropRow = ImportPropsTable.NewRow();
                    ImportPropsTable.Rows.Add( OwnerPropRow );

                    OwnerPropRow[CswImporterDbTables._ColName_ImportNodeId] = CurrentContainerRow[CswImporterDbTables._ColName_ImportNodeId];
                    OwnerPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = _ContainerOwnerUserNodeId;

                    OwnerPropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = ContainerOwnerNodeTypeProp.PropName;
                    OwnerPropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = ContainerOwnerNodeTypeProp.PropId;
                    OwnerPropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                }
                else
                {
                    _CswImportExportStatusReporter.reportError( "Unable to proceed with container creation: there is no material row" );
                }//if-else we have a material row


                ImportNodesUpdater.update( ImportNodesTable );
                ImportPropsUpdater.update( ImportPropsTable );

                _CswNbtResources.commitTransaction();

                //if( RlXlsIdx > 1000 )
                //{
                //    TestCaseStop = true;
                //    _CswImportExportStatusReporter.reportProgress( "Stopping import record creation afte having imported material: " + MaterialCompoundId.ToString() );
                //}
            }//iterate all rapid loader rows


            //End: Load import tables from spreadsheet
            //********************************************************************************************************************





            return ( ReturnVal );

        }//loadImportTables


        //the two import tables don't need to be parameters here -- they can be on the class
        private Int32 _ArbitrarySequentialUniqueId = 0;
        private void _addRowOfNodeType( DataRow RlXlsDataRow, CswNbtMetaDataNodeType NodeTypeToAdd, DataTable ImportNodesTable, DataTable ImportPropsTable, ref DataRow ImportNodesRow, DataRow RelationshipDestinationRow = null, string RelationshipPropName = "", string ContainerSizeUnit = "" )
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
                                string FieldTypeColumnName = CurrentColMetaData.FieldTypeColNames[0];
                                // Special case for Physical State remapping
                                if( null != CurrentColMetaData.CswNbtMetaDataNodeTypeProp.getObjectClassProp() &&
                                    CurrentColMetaData.CswNbtMetaDataNodeTypeProp.getObjectClassProp().PropName == CswNbtObjClassMaterial.PropertyName.PhysicalState &&
                                    _PhysicalStateMap.ContainsKey( CurrentRlXlsCellVal ) )
                                {
                                    CurrentImportPropsUpdateRow[FieldTypeColumnName] = _PhysicalStateMap[CurrentRlXlsCellVal];
                                }
                                else
                                {
                                    if( true == ImportPropsTable.Columns.Contains( FieldTypeColumnName ) )
                                    {
                                        CurrentImportPropsUpdateRow[FieldTypeColumnName] = CurrentRlXlsCellVal;
                                    }
                                    else
                                    {
                                        _CswImportExportStatusReporter.reportError( CurrentColMetaData.CswNbtMetaDataNodeType.NodeTypeName + ":" + CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName + ": the " + CswImporterDbTables.TblName_ImportProps + " table does not have a column for field type " + FieldTypeColumnName );
                                    } //if-else we have a column for our prop's field type
                                }
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
                                CurrentImportPropsUpdateRow["Special"] = CurrentRlXlsCellVal;

                                //
                                CurrentImportPropsUpdateRow["Flammability"] = RlXlsDataRow[xls_key_nfpa_firecode];
                                CurrentImportPropsUpdateRow["Reactivity"] = RlXlsDataRow[xls_key_nfpa_reactivecode];
                                CurrentImportPropsUpdateRow["Health"] = RlXlsDataRow[xls_key_nfpa_healthcode];

                            }
                            else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "initial quantity" )
                            {
                                ///Should be related node ID of node of type unit of measure
                                ///
                                string CandidateUnitOfMeasureName = string.Empty;
                                if( false == _CISProUofMNamesToNbtSizeNames.ContainsKey( CurrentRlXlsCellVal ) )
                                {
                                    CandidateUnitOfMeasureName = CurrentRlXlsCellVal;
                                }
                                else
                                {
                                    CandidateUnitOfMeasureName = _CISProUofMNamesToNbtSizeNames[CurrentRlXlsCellVal];
                                }

                                if( true == _UofMNodeIdsByUofmName.ContainsKey( CandidateUnitOfMeasureName ) )
                                {
                                    //CurrentImportPropsUpdateRow["NodeID"] = _UofMNodeIdsByUofmName[CandidateUnitOfMeasureName];
                                    CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = _UofMNodeIdsByUofmName[CandidateUnitOfMeasureName];
                                    CurrentImportPropsUpdateRow["Name"] = CandidateUnitOfMeasureName;
                                    //                                    CurrentImportPropsUpdateRow["Value"] = 1;
                                }
                                else
                                {
                                    _CswImportExportStatusReporter.reportError( "Unable to import the unit of measure called " + CandidateUnitOfMeasureName );
                                }

                            }
                            else if( CurrentColMetaData.CswNbtMetaDataNodeTypeProp.PropName.ToLower() == "quantity" )
                            {
                                CurrentImportPropsUpdateRow["Value"] = CurrentRlXlsCellVal;
                                CurrentImportPropsUpdateRow["Name"] = ContainerSizeUnit;

                                if( true == _CISProUofMNamesToNbtSizeNames.ContainsKey( ContainerSizeUnit ) )
                                {
                                    if( _UofMNodeIdsByUofmName.ContainsKey( _CISProUofMNamesToNbtSizeNames[ContainerSizeUnit] ) )
                                    {
                                        CurrentImportPropsUpdateRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = _UofMNodeIdsByUofmName[_CISProUofMNamesToNbtSizeNames[ContainerSizeUnit]];
                                    }
                                    else
                                    {
                                        _CswImportExportStatusReporter.reportError( "Error creating quantity: the CISPRO unit of measure name " + ContainerSizeUnit + " mapped to NBT unit of measur ename " + _UofMNodeIdsByUofmName[ContainerSizeUnit] + ": cannot find a the target unit of measure node" );
                                    }
                                }
                                else
                                {
                                    _CswImportExportStatusReporter.reportError( "Error creating quantity with unit of measure name " + ContainerSizeUnit + ": there is no mapping of for this unit of measure name" );
                                }
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
                                    //else
                                }
                                //{
                                //    _CswImportExportStatusReporter.reportError( "A relationship property was specified (" + RelationshipPropName + "), but no destination relationship row was provided" );
                                //}//if-else we have a desintation row for the relationship

                            }//if the current prop is a relationship prop

                        }//if the current column matches the target node type

                    }//if this is a column we understand

                }//iterate prop columns

                //DataRow RelationshipDestinationRow = null, string RelationshipPropName = string.Empty 

                if( ( false == RelationshipPropWasCreated ) && ( null != RelationshipDestinationRow ) && ( "" != RelationshipPropName ) )
                {
                    _addRelationshipForRow( ImportNodesRow, NodeTypeToAdd, ImportNodesTable, ImportPropsTable, CswConvert.ToInt32(RelationshipDestinationRow[CswImporterDbTables._ColName_ImportNodeId]), RelationshipPropName );
                }

            }//if we are adding props

        }//_addRowOfNodeType()

        private DataRow _addPropertyRow( DataTable PropsTable, DataRow NodeRow, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        {
            DataRow PropertyRow = PropsTable.NewRow();
            PropsTable.Rows.Add( PropertyRow );

            PropertyRow[CswImporterDbTables._ColName_ImportNodeId] = NodeRow[CswImporterDbTables._ColName_ImportNodeId];
            PropertyRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = CswNbtMetaDataNodeTypeProp.PropName;
            PropertyRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = CswNbtMetaDataNodeTypeProp.PropId;
            PropertyRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();
            
            return PropertyRow;
        }

        private DataRow _addLocation( CswNbtMetaDataNodeType LocationType, DataTable LocationNodesTable, DataTable LocationPropsTable, string LocationName, bool AllowInventory, Int32 ParentNodeId = Int32.MinValue )
        {
            DataRow NewLocationRow = null;
            _addRowOfNodeType( null, LocationType, LocationNodesTable, null, ref NewLocationRow );

            DataRow NamePropRow  = _addPropertyRow( LocationPropsTable, NewLocationRow, LocationType.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Name ) );
            NamePropRow["text"] = LocationName;

            DataRow AllowInventoryPropRow = _addPropertyRow( LocationPropsTable, NewLocationRow, LocationType.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.AllowInventory ) );
            AllowInventoryPropRow["checked"] = CswConvert.ToDbVal( AllowInventory );

            if( Int32.MinValue != ParentNodeId )
            {
                CswNbtMetaDataNodeTypeProp LocationProp = LocationType.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );

                DataRow LocationPropRow = _addPropertyRow( LocationPropsTable, NewLocationRow, LocationProp );
                LocationPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = ParentNodeId;
            }
            return NewLocationRow;
        }//_addLocation()


        //These params can also be pared down
        private void _addRelationshipForRow( DataRow SourceImportNodesRow, CswNbtMetaDataNodeType NodeTypeToAdd, DataTable ImportNodesTable, DataTable ImportPropsTable, Int32 TargetImportNodeId, string RelationshipPropName = "" )
        {
            if( ( Int32.MinValue != TargetImportNodeId ) && ( "" != RelationshipPropName ) )
            {
                CswNbtMetaDataNodeTypeProp RelationshipProp = NodeTypeToAdd.getNodeTypeProp( RelationshipPropName );
                if( null != RelationshipProp )
                {
                    DataRow RelationshipPropRow = ImportPropsTable.NewRow();
                    ImportPropsTable.Rows.Add( RelationshipPropRow );

                    RelationshipPropRow[CswImporterDbTables._ColName_ImportNodeId] = SourceImportNodesRow[CswImporterDbTables._ColName_ImportNodeId];

                    RelationshipPropRow[CswImporterDbTables._ColName_Infra_Nodes_NodeTypePropName] = RelationshipProp.PropName;
                    RelationshipPropRow[CswImporterDbTables.ColName_ImportPropsRealPropId] = RelationshipProp.PropId;
                    RelationshipPropRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                    RelationshipPropRow[CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique] = TargetImportNodeId;  //RelationshipDestinationRow[CswImporterDbTables._ColName_ImportNodeId];
                }
                else
                {
                    _CswImportExportStatusReporter.reportError( NodeTypeToAdd.NodeTypeName + ": The specified relationship property(" + RelationshipProp + ") does not exist on this nodetype" );
                }

            }
            else
            {
                _CswImportExportStatusReporter.reportError( "Unable to add relationship row" );
            }//

        }//_addRelationshipForRow()

    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
