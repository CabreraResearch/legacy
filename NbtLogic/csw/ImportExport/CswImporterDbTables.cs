using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{

    public class CswImporterDbTables
    {

        private CswNbtResources _CswNbtResources = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        private string _TblName_ImportNodes = "import_nodes";
        private string _TblName_ImportProps = "import_props";
        private string _ColName_ProcessStatus = "processstatus";
        private string _ColName_StatusMessage = "statusmessage";
        private string _ColName_Source = "source";
        private string _ColName_ImportNodeId = "importnodeid";
        private string _ColName_Props_ImportTargetNodeIdUnique = "importtargetnodeid";
        private string _ColName_Props_ImportTargetNodeIdOriginal = "NodeID";
        private string _ColName_Nodes_NodeName = "nodename";

        string ColName_ImportNodesTablePk = "tmpimportnodesid";
        string Colname_NbtNodeId = "nbtnodeid";

        string ColName_ImportPropsTablePk = "tmpimportpropsid";
        string ColName_ImportPropsRealPropId = "nbtnodepropid";

        private Collection<string> _AdditonalColumns = new Collection<string>();
        private Collection<string> _AdditonalColumns = new Collection<string>();
        private Collection<string> _IndexColumns = new Collection<string>();

        private CswNbtImportOptions _CswNbtImportOptions = null;

        public CswImporterDbTables( CswNbtResources CswNbtResources, CswNbtImportOptions CswNbtImportOptions )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );

            _CswNbtImportOptions = CswNbtImportOptions;


            _AdditonalColumns.Add( _ColName_ProcessStatus );


            _AdditonalColumns.Add( _ColName_StatusMessage );
            _AdditonalColumns.Add( _ColName_Source );
            _IndexColumns.Add( _ColName_ProcessStatus );
            _IndexColumns.Add( _ColName_ImportNodeId );


        }//ctor


        public void reset()
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_ImportNodes ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_ImportNodes ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( _TblName_ImportNodes );
            }

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_ImportProps ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_ImportProps ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( _TblName_ImportProps );
            }


        }//reset()





        public bool areImportTablesAbsent( ref string ImportTableInconsistencyMessage )
        {

           bool ReturnVal = true;

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_ImportNodes ) )
            {
                if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_ImportNodes ) )
                {
                    if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_ImportProps ) )
                    {
                        if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_ImportProps ) )
                        {
                            ReturnVal = false;
                            ImportTableInconsistencyMessage = string.Empty;
                        }
                        else
                        {
                            ImportTableInconsistencyMessage = "The import nodes table is defined properly; however, the import props table is defined in the database but not in meta data";
                        }//if-else propstable is defined in meta data
                    }
                    else
                    {
                        ImportTableInconsistencyMessage = "The import nodes nodes table is defined properly; however the import props table is not defined in the database";
                    }//if-else props table is defined in db
                }
                else
                {
                    ImportTableInconsistencyMessage = "The import nodes table is defined in the database, but not in the meta data";
                }//if-else Nodes table is defined in meta data
            }
            else
            {
                ImportTableInconsistencyMessage = string.Empty; //not necessarily an error condition: it just aint there
            }//if-else Nodes table is defined in db

            return ( ReturnVal );
s
        }//_importTablesAreAbsent()



        public void commitAndRelease()
        {
            string AccessId = _CswNbtResources.AccessId;

            _CswNbtResources.finalize();
            _CswNbtResources.clearUpdates();
            _CswNbtResources.releaseDbResources();

            _CswNbtResources.AccessId = AccessId; //force re-init of resources
            _CswNbtResources.refreshDataDictionary();

            System.GC.Collect();

        }//_commitAndRelease()


        public void makeImportTables()
        {


            _CswNbtSchemaModTrnsctn.beginTransaction();


            _makeImportTable( _TblName_ImportNodes, ColName_ImportNodesTablePk, TableOfNodesFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
            _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportNodes, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );



            _makeImportTable( _TblName_ImportProps, ColName_ImportPropsTablePk, TableOfPropsFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
            _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportProps, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportProps, ColName_ImportPropsRealPropId, "to be filled in when the node is actually created", false, false );



            _CswNbtSchemaModTrnsctn.commitTransaction();

        }


        private void _makeImportTable( string TableName, string PkColumnName, DataColumnCollection Columns, Int32 ArbitraryStringColumnLength, Collection<string> AdditionalStringColumns, Collection<string> IndexColumns )
        {

            _CswNbtSchemaModTrnsctn.addTable( TableName, PkColumnName );
            foreach( DataColumn CurrentColumn in Columns )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( TableName, CurrentColumn.ColumnName, string.Empty, false, false, ArbitraryStringColumnLength );
            }

            foreach( string CurrentColumName in AdditionalStringColumns )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( TableName, CurrentColumName, string.Empty, false, false, ArbitraryStringColumnLength );
            }

            foreach( string CurrentColumnName in IndexColumns )
            {
                _CswNbtSchemaModTrnsctn.indexColumn( TableName, CurrentColumnName );
            }

        }//_makeImportTable() 


        public void createImportTableRecords( DataTable SourceTable, string DestinationTableName, Int32 MaxInsertRecordsPerTransaction, Int32 MaxInsertRecordsPerDisplayUpdate )
        {


            CswTableSelect DestinationTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "queryforprexistingimportrecord", DestinationTableName );




            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "insertimportrecordsfortable_" + SourceTable.TableName, DestinationTableName );
            DataTable DestinationDataTable = CswTableUpdate.getEmptyTable();
            Int32 TotalRecordsToInsert = SourceTable.Rows.Count;
            Int32 TotalRecordsInsertedSoFar = 0;
            Int32 TotalInsertsThisTransaction = 0;

            foreach( DataRow CurrentSourceRow in SourceTable.Rows )
            {

                DataTable ExistingImportRecord = DestinationTableSelect.getTable( " where " + _ColName_ImportNodeId + "='" + CurrentSourceRow[_ColName_ImportNodeId].ToString() + "' " );
                if( 0 == ExistingImportRecord.Rows.Count )
                {

                    TotalInsertsThisTransaction++;
                    TotalRecordsInsertedSoFar++;
                    DataRow NewRow = DestinationDataTable.NewRow();
                    DestinationDataTable.Rows.Add( NewRow );


                    NewRow[_ColName_ProcessStatus] = ImportProcessStati.Unprocessed.ToString();

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
                    if( SourceTable.Rows[CurrentRowIndex + 1][_ColName_ImportNodeId].ToString() == CurrentSourceRow[_ColName_ImportNodeId].ToString() )
                    {
                        MoreOfSameRecordToImportIETheseArePropRecords_IAgreeThisIsAKludge_SoSueMe = true;
                    }
                }

                if( ( ( TotalInsertsThisTransaction >= _CswNbtImportOptions.MaxInsertRecordsPerTransaction ) && ( false == MoreOfSameRecordToImportIETheseArePropRecords_IAgreeThisIsAKludge_SoSueMe ) ) || ( TotalRecordsInsertedSoFar >= TotalRecordsToInsert ) )
                {
                    CswTableUpdate.update( DestinationDataTable );
                    _commitAndRelease();

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    TotalInsertsThisTransaction = 0;
                }

                if( 0 == ( TotalInsertsThisTransaction % _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate ) )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalRecordsToInsert, TotalRecordsInsertedSoFar );
                }

            }//iterate source table rows


        }//_createImportTableRecords() 


        private Int32 _doesNodeNameAlreadyExist( string NodeName )
        {
            Int32 ReturnVal = Int32.MinValue;

            CswTableSelect CswTableSelectNodes = _CswNbtResources.makeCswTableSelect( "uniquenodesquery", "nodes" );
            DataTable NodesTable = CswTableSelectNodes.getTable( " where lower(nodename) = '" + NodeName.ToLower() + "'" );

            if( NodesTable.Rows.Count > 0 )
            {
                ReturnVal = CswConvert.ToInt32( NodesTable.Rows[0]["nodeid"] );
            }

            return ( ReturnVal );

        }//_doesNodeNameAlreadyExist() 

        private bool _validateTargetNodeType( CswNbtMetaDataNodeTypeProp SourceNodeTypeProp, Int32 DestinationNodeId, ref string ErrorMessage )
        {
            bool DestinationTypeMatchesSourcesType = true;

            CswNbtNode DestinationNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", DestinationNodeId )];

            if( null != DestinationNode )
            {

                if( SourceNodeTypeProp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                {
                    CswNbtMetaDataNodeType RelatedNodeType = _CswNbtResources.MetaData.getNodeType( SourceNodeTypeProp.FKValue );

                    if( RelatedNodeType.NodeTypeName != DestinationNode.getNodeType().NodeTypeName )
                    {
                        DestinationTypeMatchesSourcesType = false;
                        ErrorMessage = " the node type of the destination node " + DestinationNode.NodeId.ToString() + " named " + DestinationNode.NodeName + " is " + DestinationNode.getNodeType().NodeTypeName + " but the " + SourceNodeTypeProp.PropName + " must reference a node of node type " + RelatedNodeType.NodeTypeName;
                    }

                }
                else if( SourceNodeTypeProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() )
                {
                    CswNbtMetaDataObjectClass RelatedObjectClass = _CswNbtResources.MetaData.getObjectClass( SourceNodeTypeProp.FKValue );
                    if( RelatedObjectClass.ObjectClass != DestinationNode.getObjectClass().ObjectClass )
                    {
                        DestinationTypeMatchesSourcesType = false;
                        ErrorMessage = " object class of the destination node " + DestinationNode.NodeId.ToString() + " named " + DestinationNode.NodeName + " is " + DestinationNode.getObjectClass().ObjectClass.ToString() + " but the " + SourceNodeTypeProp.PropName + " must reference a node of object class " + RelatedObjectClass.ObjectClass.ToString();
                    }
                }
                else
                {
                    DestinationTypeMatchesSourcesType = false;
                    ErrorMessage = " The FK Type of the node type prop" + SourceNodeTypeProp.PropName + " cannot be determined";
                }
            }
            else
            {
                DestinationTypeMatchesSourcesType = false;
                ErrorMessage = "The target node ID " + DestinationNodeId.ToString() + " does not resolve to a known node";
            }//if-else the destnation node id is for real

            return ( DestinationTypeMatchesSourcesType );

        }//_validateTargetNodeType()

    } // class CswImporterExperimental

} // namespace ChemSW.Nbt


