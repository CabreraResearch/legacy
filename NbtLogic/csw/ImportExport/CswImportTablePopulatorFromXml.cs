
using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
namespace ChemSW.Nbt.ImportExport
{

    public class CswImportTablePopulatorFromXml : ICswImportTablePopulator
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;

        private CswNbtImportOptions _CswNbtImportOptions = null;

        private CswNbtImportStatus _CswNbtImportStatus = null;
        private CswImporterDbTables _CswImporterDbTables = null;

        public CswImportTablePopulatorFromXml( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter, CswNbtImportStatus CswNbtImportStatus )
        {
            _CswNbtImportStatus = CswNbtImportStatus;
            _CswNbtImportOptions = new CswNbtImportOptions(); //This will be passed in as a ctor arg

            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );


            _CswImporterDbTables = new CswImporterDbTables( CswNbtResources, _CswNbtImportOptions );

        }//ctor


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



            _CswImportExportStatusReporter.reportProgress( "Loading XML document to in memory tables" );

            _CswImportExportStatusReporter.MessageTypesToBeLogged.Remove( ImportExportMessageType.Error );

            _CswImportExportStatusReporter.MessageTypesToBeLogged.Add( ImportExportMessageType.Timing );
            //*********************************************************************************************************
            //*********************** Create Import Tables


            DataSet DataSet = _CswNbtImportExportFrame.AsDataSet();
            DataTable TableOfNodesFromXml = DataSet.Tables["Node"];
            TableOfNodesFromXml.Columns["nodeid"].ColumnName = CswImporterDbTables._ColName_ImportNodeId;


            DataTable TableOfPropsFromXml = DataSet.Tables["PropValue"];
            TableOfPropsFromXml.Columns["NodeID"].ColumnName = CswImporterDbTables._ColName_Props_ImportTargetNodeIdUnique; //This is not a joke
            TableOfPropsFromXml.Columns["nodeid"].ColumnName = CswImporterDbTables._ColName_ImportNodeId;



            string ImportTableInconsistencyMessage = string.Empty;

            if( _CswImporterDbTables.areImportTablesAbsent( ref ImportTableInconsistencyMessage ) )
            {
                if( string.Empty == ImportTableInconsistencyMessage )
                {
                    _LastCompletedProcessPhase = ImportProcessPhase.LoadingInputFile;

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                    _CswNbtSchemaModTrnsctn.beginTransaction();



                    List<string> PropColumnNames = new List<string>();
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


            //*********************************************************************************************************
            //*********************** Fill Import tables
            _LastCompletedProcessPhase = ImportProcessPhase.PopulatingImportTableNodes;

            _createImportTableRecords( TableOfNodesFromXml, CswImporterDbTables.TblName_ImportNodes, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
            _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

            _LastCompletedProcessPhase = ImportProcessPhase.PopulatingImportTableProps;
            _createImportTableRecords( TableOfPropsFromXml, CswImporterDbTables.TblName_ImportProps, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
            _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtImportExportFrame.clear();

            //*********************************************************************************************************
            //*********************** Check integrity of import tables
            if( ( false == Stop ) && ( ImportProcessPhase.PopulatingImportTableProps == _LastCompletedProcessPhase ) )
            {

                string DuplicateCountColumnName = "duplicate";
                string DuplicatesQuery = " SELECT " + CswImporterDbTables._ColName_ImportNodeId + ", COUNT(" + CswImporterDbTables._ColName_ImportNodeId + ") AS " + DuplicateCountColumnName + " FROM " + CswImporterDbTables.TblName_ImportNodes + " GROUP BY " + CswImporterDbTables._ColName_ImportNodeId + " HAVING (COUNT(importnodeid) > 1) ";
                CswArbitrarySelect CswArbitrarySelectDuplicateCheck = _CswNbtResources.makeCswArbitrarySelect( "tmp node table duplicate count", DuplicatesQuery );
                DataTable DataTableDuplicatesCheck = CswArbitrarySelectDuplicateCheck.getTable();
                if( DataTableDuplicatesCheck.Rows.Count == 0 )
                {
                    _LastCompletedProcessPhase = ImportProcessPhase.ImportTableIntegrityChecked;
                }
                else
                {
                    CswCommaDelimitedString CswCommaDelimitedString = new CswCommaDelimitedString();
                    foreach( DataRow CurrentDataRow in DataTableDuplicatesCheck.Rows )
                    {

                        CswCommaDelimitedString.Add( CurrentDataRow[CswImporterDbTables._ColName_ImportNodeId].ToString() );
                    }


                    _CswImportExportStatusReporter.reportError( "Processing cannot proceed because the " + CswImporterDbTables.TblName_ImportNodes + "." + CswImporterDbTables._ColName_ImportNodeId + " column contains the following non-unique values: " + CswCommaDelimitedString );


                }//if-else there are duplicate column values 

            }//import table integrity check




            return ( ReturnVal );

        }//loadImportTables


        private void _createImportTableRecords( DataTable SourceTable, string DestinationTableName, Int32 MaxInsertRecordsPerTransaction, Int32 MaxInsertRecordsPerDisplayUpdate )
        {

            CswTableSelect DestinationTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "queryforprexistingimportrecord", DestinationTableName );




            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "insertimportrecordsfortable_" + SourceTable.TableName, DestinationTableName );
            DataTable DestinationDataTable = CswTableUpdate.getEmptyTable();
            Int32 TotalRecordsToInsert = SourceTable.Rows.Count;
            Int32 TotalRecordsInsertedSoFar = 0;
            Int32 TotalInsertsThisTransaction = 0;

            foreach( DataRow CurrentSourceRow in SourceTable.Rows )
            {

                DataTable ExistingImportRecord = DestinationTableSelect.getTable( " where " + CswImporterDbTables._ColName_ImportNodeId + "='" + CurrentSourceRow[CswImporterDbTables._ColName_ImportNodeId].ToString() + "' " );
                if( 0 == ExistingImportRecord.Rows.Count )
                {

                    TotalInsertsThisTransaction++;
                    TotalRecordsInsertedSoFar++;
                    DataRow NewRow = DestinationDataTable.NewRow();
                    DestinationDataTable.Rows.Add( NewRow );


                    NewRow[CswImporterDbTables._ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

                    foreach( DataColumn CurrentColum in SourceTable.Columns )
                    {
                        if( DestinationDataTable.Columns.Contains( CurrentColum.ColumnName ) )
                        {
                            NewRow[CurrentColum.ColumnName] = CurrentSourceRow[CurrentColum.ColumnName].ToString();
                        }

                    }//iterate source table columns
                }
                else
                {
                    TotalRecordsToInsert--;
                }

                bool MoreOfSameRecordToImportIETheseArePropRecords_IAgreeThisIsAKludge_SoSueMe = false;
                Int32 CurrentRowIndex = SourceTable.Rows.IndexOf( CurrentSourceRow );
                if( ( CurrentRowIndex + 1 ) < SourceTable.Rows.Count )
                {
                    if( SourceTable.Rows[CurrentRowIndex + 1][CswImporterDbTables._ColName_ImportNodeId].ToString() == CurrentSourceRow[CswImporterDbTables._ColName_ImportNodeId].ToString() )
                    {
                        MoreOfSameRecordToImportIETheseArePropRecords_IAgreeThisIsAKludge_SoSueMe = true;
                    }
                }

                if( ( ( TotalInsertsThisTransaction >= _CswNbtImportOptions.MaxInsertRecordsPerTransaction ) && ( false == MoreOfSameRecordToImportIETheseArePropRecords_IAgreeThisIsAKludge_SoSueMe ) ) || ( TotalRecordsInsertedSoFar >= TotalRecordsToInsert ) )
                {
                    CswTableUpdate.update( DestinationDataTable );
                    _CswImporterDbTables.commitAndRelease();

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    TotalInsertsThisTransaction = 0;
                }

                if( 0 == ( TotalInsertsThisTransaction % _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate ) )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalRecordsToInsert, TotalRecordsInsertedSoFar );
                }

            }//iterate source table rows


        }//_createImportTableRecords() 

    } // CswImportTablePopulatorFromXml

} // namespace ChemSW.Nbt
