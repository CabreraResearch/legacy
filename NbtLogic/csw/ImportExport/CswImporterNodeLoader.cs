using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public enum AbsentImportNodeHandling { DeduceAndCreate, RejectImport }
    public enum ImportStartPoint { NukeAndStartOver, Resume }


    public class CswImporterNodeLoader : ICswImporter
    {

        private CswNbtResources _CswNbtResources = null;
        private CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;

        private CswNbtImportOptions _CswNbtImportOptions = null;

        private CswNbtImportStatus _CswNbtImportStatus = null;

        private CswImporterDbTables _CswImporterDbTables = null;

        public CswImporterNodeLoader( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter, CswNbtImportStatus CswNbtImportStatus )
        {

            _CswImporterDbTables = new CswImporterDbTables( CswNbtResources );
            _CswNbtImportStatus = CswNbtImportStatus;
            _CswNbtImportOptions = new CswNbtImportOptions(); //This will be passed in as a ctor arg

            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );
        }//ctor


        public void reset()
        {
            _CswImporterDbTables.reset();
            _CswNbtImportStatus.reset();

        }//reset()



        private bool _Stop = false;
        public void stop()
        {

            if(
                ( _CswNbtImportStatus.TargetProcessPhase == ImportProcessPhase.PopulatingImportTableNodes ) ||
                 ( _CswNbtImportStatus.TargetProcessPhase == ImportProcessPhase.PopulatingImportTableNodes )
                )
            {
                //It's actually kind of messy code-wise to stop in the populate-import-table phase, so for now we'll just say "no"
                _CswImportExportStatusReporter.reportProgress( "Stop message received: cannot stop process while in " + _CswNbtImportStatus.TargetProcessPhase.ToString() + " phase" );

            }
            else
            {
                _CswImportExportStatusReporter.reportProgress( "Stop message received, shutting down . . . " );
                _Stop = true;

            }
        }//stop() 

        private string _StatusMessageDivider = "==================================";

        ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;



        private bool _importTablesAreAbsent( ref string ImportTableInconsistencyMessage )
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
        }//_importTablesAreAbsent()



        private void _commitAndRelease()
        {
            string AccessId = _CswNbtResources.AccessId;

            _CswNbtResources.finalize();
            _CswNbtResources.clearUpdates();
            _CswNbtResources.releaseDbResources();

            _CswNbtResources.AccessId = AccessId; //force re-init of resources
            _CswNbtResources.refreshDataDictionary();

            System.GC.Collect();
        }//_commitAndRelease()

        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _CswImportExportStatusReporter.reportProgress( _StatusMessageDivider + "Starting Import: " + ImportAlgorithm.DbTableBased.ToString() );

            ErrorLog = string.Empty;

            _Stop = false;


            //*********************************************************************************************************
            //*********************** Load to dataset
            //            _LastCompletedProcessPhase = _CswNbtImportStatus.CompletedProcessPhase;

            _CswImportExportStatusReporter.reportProgress( "Loading XML document to in memory tables" );



            //*********************************************************************************************************
            //*********************** Local variable definitions

            string ColName_ImportNodesTablePk = "tmpimportnodesid";
            string Colname_NbtNodeId = "nbtnodeid";

            string ColName_ImportPropsTablePk = "tmpimportpropsid";
            string ColName_ImportPropsRealPropId = "nbtnodepropid";


            Collection<string> _AdditonalColumns = new Collection<string>();
            _AdditonalColumns.Add( _ColName_ProcessStatus );
            _AdditonalColumns.Add( _ColName_StatusMessage );
            _AdditonalColumns.Add( _ColName_Source );


            Collection<string> _IndexColumns = new Collection<string>();
            _IndexColumns.Add( _ColName_ProcessStatus );
            _IndexColumns.Add( _ColName_ImportNodeId );


            _CswImportExportStatusReporter.MessageTypesToBeLogged.Remove( ImportExportMessageType.Error );

            _CswImportExportStatusReporter.MessageTypesToBeLogged.Add( ImportExportMessageType.Timing );
            //*********************************************************************************************************
            //*********************** Create Import Tables


            DataSet DataSet = _CswNbtImportExportFrame.AsDataSet();
            DataTable TableOfNodesFromXml = DataSet.Tables["Node"];
            TableOfNodesFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;


            DataTable TableOfPropsFromXml = DataSet.Tables["PropValue"];
            TableOfPropsFromXml.Columns["NodeID"].ColumnName = _ColName_Props_ImportTargetNodeIdUnique; //This is not a joke
            TableOfPropsFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;



            string ImportTableInconsistencyMessage = string.Empty;

            if( _CswImporterDbTables.areImportTablesAbsent( ref ImportTableInconsistencyMessage ) )
            {
                if( string.Empty == ImportTableInconsistencyMessage )
                {
                    _LastCompletedProcessPhase = ImportProcessPhase.LoadingInputFile;

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                    _CswNbtSchemaModTrnsctn.beginTransaction();


                    _CswImporterDbTables.makeImportTable( _TblName_ImportNodes, ColName_ImportNodesTablePk, TableOfNodesFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
                    _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportNodes, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );



                    _CswImporterDbTables.makeImportTable( _TblName_ImportProps, ColName_ImportPropsTablePk, TableOfPropsFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
                    _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportProps, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );
                    _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_ImportProps, ColName_ImportPropsRealPropId, "to be filled in when the node is actually created", false, false );



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

            _createImportTableRecords( TableOfNodesFromXml, _TblName_ImportNodes, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
            _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

            _LastCompletedProcessPhase = ImportProcessPhase.PopulatingImportTableProps;
            _createImportTableRecords( TableOfPropsFromXml, _TblName_ImportProps, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
            _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtImportExportFrame.clear();

            //*********************************************************************************************************
            //*********************** Check integrity of import tables
            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingImportTableProps == _LastCompletedProcessPhase ) )
            {

                string DuplicateCountColumnName = "duplicate";
                string DuplicatesQuery = " SELECT " + _ColName_ImportNodeId + ", COUNT(" + _ColName_ImportNodeId + ") AS " + DuplicateCountColumnName + " FROM " + _TblName_ImportNodes + " GROUP BY " + _ColName_ImportNodeId + " HAVING (COUNT(importnodeid) > 1) ";
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
                        CswCommaDelimitedString.Add( CurrentDataRow[_ColName_ImportNodeId].ToString() );
                    }

                    _CswImportExportStatusReporter.reportError( "Processing cannot proceed because the " + _TblName_ImportNodes + "." + _ColName_ImportNodeId + " column contains the following non-unique values: " + CswCommaDelimitedString );


                }//if-else there are duplicate column values 

            }//import table integrity check



            CswNbtNode GeneralUserRole = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( _CswNbtImportOptions.NameOfDefaultRoleForUserNodes );
            CswTableUpdate CswTableUpdateImportNodesTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatenodesfornodeid", _TblName_ImportNodes );
            if( ( false == _Stop ) && ( ImportProcessPhase.ImportTableIntegrityChecked == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingNbtNodes;
                //_CswImportExportStatusReporter.reportProgress( _StatusMessageDivider + "Creating NBT Nodes" );

                //string WhereClauseForUnprocessedRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.Unprocessed.ToString() + "' and nodetypename <> 'User'";
                //string WhereClauseForUnprocessedRecords = " where " + _ColName_ProcessStatus + "='" + ProcessStati.Unprocessed.ToString() + "'" + " and nodetypename <> 'User'";
                string WhereClauseForUnprocessedRecords = " where " + _ColName_ProcessStatus + "='" + ImportProcessStati.Unprocessed.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_ImportNodes + WhereClauseForUnprocessedRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_Nodes_NodeName );
                SelectColumns.Add( "nodetypename" );
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                string RawNodesQuery = "select " + SelectColumns + " from " + _TblName_ImportNodes + WhereClauseForUnprocessedRecords;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", RawNodesQuery );
                DataTable RawNodesTable = null;
                do
                {
                    if( null != RawNodesTable )
                    {
                        foreach( DataRow CurrentRow in RawNodesTable.Rows ) //will be empty on 1st interation
                        {
                            string CurrentNodeTypeNameInImportTable = CurrentRow["nodetypename"].ToString();
                            string CurrentNodeNameInImportTable = CurrentRow[_ColName_Nodes_NodeName].ToString();
                            string ImportNodeId = CurrentRow[_ColName_ImportNodeId].ToString();

                            DataTable ImprtNodesUpdateTable = CswTableUpdateImportNodesTable.getTable( "where " + _ColName_ImportNodeId + "='" + ImportNodeId + "'" );
                            DataRow ImportNodesUpdateRow = ImprtNodesUpdateTable.Rows[0];

                            try
                            {
                                CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeNameInImportTable );
                                if( null != CurrentNodeType )
                                {


                                    CswNbtNode CswNbtNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                    //CswNbtNode.NodeName = CurrentNodeNameInImportTable; 


                                    if( CurrentNodeType.NodeTypeName.ToLower() == "user" )
                                    {
                                        _rehabilitateUser( CswNbtNode, CurrentNodeNameInImportTable, GeneralUserRole );
                                    }

                                    CswNbtNode.postChanges( false );

                                    Int32 NbtNodeId = CswNbtNode.NodeId.PrimaryKey;


                                    ImportNodesUpdateRow[Colname_NbtNodeId] = NbtNodeId;
                                    ImportNodesUpdateRow[_ColName_Source] = ImportSource.ImportData;
                                    CswTableUpdateImportNodesTable.update( ImprtNodesUpdateTable );

                                    ImportNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Imported.ToString();

                                    // This visibly slows things down and is probably not necessary
                                    //TempPropsUpdateTable = CswTableUpdateTempPropsTable.getTable( " where nodeid = '" + CurrentNodeIdInTempTable + "'" );
                                    //foreach( DataRow CurrentPropsUpdateRow in TempPropsUpdateTable.Rows )
                                    //{
                                    //    CurrentPropsUpdateRow[TempNodesRealNodeIdColName] = RealNbtNodeId;
                                    //}

                                }
                                else
                                {
                                    string Error = "Unable to import node  " + CurrentNodeNameInImportTable + " @ " + _ColName_ImportNodeId + " " + ImportNodeId + ",  because its nodetype (" + CurrentNodeTypeNameInImportTable + ") does not exist in the target schema";
                                    _CswImportExportStatusReporter.reportError( Error );
                                    ImportNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Error.ToString();
                                    ImportNodesUpdateRow[_ColName_StatusMessage] = Error;

                                }//if-else current node's nodetype exists

                            }//try

                            catch( Exception Exception )
                            {
                                ImportNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Error.ToString();
                                ImportNodesUpdateRow[_ColName_StatusMessage] = Exception.Message;

                                _CswImportExportStatusReporter.reportException( Exception );
                            }//catch


                            CswTableUpdateImportNodesTable.update( ImprtNodesUpdateTable );

                            TotalNodesProcesssedSoFar++;

                        }//iterate raw node rows


                        if( RawNodesTable.Rows.Count > 0 )
                        {
                            _commitAndRelease();
                        }

                    }//if we're not on the first iteration

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    RawNodesTable = CswArbitrarySelectUnprocessedNodes.getTable( 0, _CswNbtImportOptions.NodeCreatePageSize, false, false );

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalNodesToProcess, TotalNodesProcesssedSoFar, ProcessStates.InProcess );


                } while( ( false == _Stop ) && ( RawNodesTable.Rows.Count > 0 ) );


                //if by coincidence we hit "stop" just as this phase completed, it's not the end of the world: 
                //when we resume, we'll resume at this phase, but the query won't reutrn results, and we'll just 
                //fall through to the next phase
                if( false == _Stop )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalNodesToProcess, TotalNodesProcesssedSoFar, ProcessStates.Complete );
                }

            }//if temptables have been populated

            //24658: We don't make target nodes that don't exist: they probably don't exist because they were deleted in the target data
            //( we need to remember that IMCS is an applicaiton that probably left around dangling references to records that were deleted)
            /*
            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingNbtNodes == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.VerifyingNbtTargetNodes;

                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                string QueryForMissingTargetNodes = @"select distinct p.importtargetnodeid
                                                      from tmp_import_props p, tmp_import_nodes n
                                                     where p.importtargetnodeid is not null
                                                       and (p.importtargetnodeid not in
                                                           (select distinct importnodeid from tmp_import_nodes n) )
                                                        or n.nbtnodeid is null   ";




                CswArbitrarySelect CswArbitrarySelectMissingTargets = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "findmisingtargetnodes", QueryForMissingTargetNodes );
                DataTable DataTable = CswArbitrarySelectMissingTargets.getTable();
                Int32 AddedNodesCounter = 0;

                if( DataTable.Rows.Count > 0 )
                {
                    if( AbsentImportNodeHandling.DeduceAndCreate == _CswNbtImportOptions.AbsentNodeHandling )
                    {

                        //foreach( DataRow CurrentRow in DataTable.Rows )
                        for( Int32 idx = 0; ( false == _Stop ) && ( idx < DataTable.Rows.Count ); idx++ )
                        {
                            DataRow CurrentRow = DataTable.Rows[idx];


                            string ImportNodeIdOfAbsentNode = CurrentRow[_ColName_Props_ImportTargetNodeIdUnique].ToString();
                            CswNbtImportNodeId CswNbtImportNodeId = new ImportExport.CswNbtImportNodeId( ImportNodeIdOfAbsentNode );


                            if( false == CswNbtImportNodeId.IsNull )
                            {
                                CswNbtMetaDataNodeType NodeTypeOfAbsentNode = _CswNbtResources.MetaData.getNodeType( CswNbtImportNodeId.NodeNodeType );


                                if( null == NodeTypeOfAbsentNode )
                                {
                                    //if the nodetype is trivial -- i.e., the direct derivation (by name) of an object class,
                                    //why not go ahead and create it, and expand the scope of reference target nodes that we 
                                    //can create by inference? 
                                    foreach( CswNbtMetaDataObjectClass CurrentObjectClass in _CswNbtResources.MetaData.ObjectClasses )
                                    {
                                        string FullObjectClassName = CurrentObjectClass.ObjectClass.ToString();
                                        string StrippedObjectClassName = FullObjectClassName.Remove( FullObjectClassName.Length - 5 );
                                        if( StrippedObjectClassName.ToLower() == CswNbtImportNodeId.NodeNodeType.ToLower() )
                                        {
                                            NodeTypeOfAbsentNode = _CswNbtResources.MetaData.makeNewNodeType( FullObjectClassName, CswNbtImportNodeId.NodeNodeType, string.Empty );
                                            break;
                                        }

                                    }//iterate object classes

                                }//if we did not retrieve a related node type



                                if( NodeTypeOfAbsentNode != null )
                                {

                                    //****************************
                                    //Create the node
                                    CswNbtNode TargetNodeThatWasMissing = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeOfAbsentNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                    //TargetNodeThatWasMissing.NodeName = AbsentNodeNodeName; // Kludge to make the node have a value even when no property values are supplied for it

                                    // Kludge specifically for 10340:
                                    if( CswNbtImportNodeId.NodeNodeType.ToLower() == "user" )
                                    {
                                        _rehabilitateUser( TargetNodeThatWasMissing, CswNbtImportNodeId.NodeNodeName, GeneralUserRole );
                                    }

                                    TargetNodeThatWasMissing.postChanges( false, false, true );

                                    //NodeIdMap.Add( CswTools.XmlRealAttributeName( ImportNodeIdOfAbsentNode ).ToLower(), TargetNodeThatWasMissing.NodeId.PrimaryKey );    // for property value references
                                    //NodeRow["destnodeid"] = CswConvert.ToDbVal( TargetNodeThatWasMissing.NodeId.PrimaryKey );       // for posterity


                                    //****************************
                                    //Create the temp node tables so that the references will be there when we create target props
                                    DataTable AddMissingNodeEntryTable = CswTableUpdateImportNodesTable.getEmptyTable();
                                    DataRow NewNodeEntryRow = AddMissingNodeEntryTable.NewRow();
                                    AddMissingNodeEntryTable.Rows.Add( NewNodeEntryRow );
                                    NewNodeEntryRow[_ColName_ImportNodeId] = ImportNodeIdOfAbsentNode;
                                    NewNodeEntryRow[_ColName_Nodes_NodeName] = TargetNodeThatWasMissing.NodeName;
                                    NewNodeEntryRow[_ColName_ProcessStatus] = ImportSource.Deduced.ToString();
                                    NewNodeEntryRow[Colname_NbtNodeId] = TargetNodeThatWasMissing.NodeId.PrimaryKey.ToString();

                                    CswTableUpdateImportNodesTable.update( AddMissingNodeEntryTable );

                                    _CswImportExportStatusReporter.reportProgress( "Added missing node " + TargetNodeThatWasMissing.NodeName );
                                    AddedNodesCounter++;
                                }
                                else
                                {
                                    //if the target node already exists in the destination schema, we'll resolve it when we create the property
                                    if( Int32.MinValue == _getNodeIdForNodeName( ImportNodeIdOfAbsentNode ) )
                                    {
                                        _CswImportExportStatusReporter.reportError( "Unable to auto-create node for ID " + ImportNodeIdOfAbsentNode + ": there is no node type '" + CswNbtImportNodeId.NodeNodeType + "'" );
                                    }
                                }//if there was a nodetype for the missing node


                            }//if the import node ID is not null

                        }//iterate absent target nodes rows


                        //I am not paging this phase on the assumption that there won't be enough 
                        //of these that would require doing so. So it is for now.
                        _CswNbtResources.finalize();
                        _CswNbtResources.clearUpdates();
                        _CswNbtResources.releaseDbResources();

                        _CswImportExportStatusReporter.reportProgress( "Added " + AddedNodesCounter.ToString() + " missing nodes" );


                    }
                    else //( AbsentNodeHandling.RejectImport == _AbsentNodeHandling )
                    {
                        CswDelimitedString CswDelimitedString = new CswDelimitedString( '\n' );
                        foreach( DataRow CurrentRow in DataTable.Rows )
                        {
                            CswDelimitedString.Add( CurrentRow[_ColName_Props_ImportTargetNodeIdUnique].ToString() );

                        }//iterate rows of missing targets

                        _CswImportExportStatusReporter.reportError( "The following target import nodes do not exist in the import data: " + CswDelimitedString );
                    }
                }


                //if by coincidence we hit "stop" just as this phase completed, it's not the end of the world: 
                //when we resume, we'll resume at this phase, but the query won't reutrn results, and we'll just 
                //fall through to the next phase
                if( false == _Stop )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );
                }


            }//if nodes were processed
             * 
             */

            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingNbtNodes == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingNbtProps;



                string WhereClauseForImportedNodeRecords = " where " + _ColName_ProcessStatus + "='" + ImportProcessStati.Imported.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_ImportNodes + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;



                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_Nodes_NodeName );
                SelectColumns.Add( "nodetypename" );
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                string SelectStatementForUnprocessedNodes = "select " + SelectColumns + " from " + _TblName_ImportNodes + WhereClauseForImportedNodeRecords + " order by " + ColName_ImportNodesTablePk;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", SelectStatementForUnprocessedNodes );
                CswTableUpdate CswTableUpdateNodes = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatefornodepropupdatestatus", _TblName_ImportNodes );



                CswTableUpdate CswTableUpdateImportNodeRecords = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update nodes table", _TblName_ImportNodes );


                DataTable NodeRecordsToProcess = null;
                do
                {

                    CswTimer CommitNNodesTimer = new CswTimer();

                    if( ( null != NodeRecordsToProcess ) && ( NodeRecordsToProcess.Rows.Count > 0 ) )
                    {
                        Int32 PropAddCounter = 0;
                        Int32 RelationshipPropAddCounter = 0;
                        CswTimer AddPropsToNodeTimer = new CswTimer();

                        //******************************************************************
                        //Luke, close your eyes and let The Force guide you now . . . .
                        foreach( DataRow CurrentImportNodeRow in NodeRecordsToProcess.Rows )
                        {

                            string CurrentNodeRowError = string.Empty;
                            string CurrentNodeTypeName = CurrentImportNodeRow["nodetypename"].ToString();
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeName );

                            string CurrentImportNodeName = string.Empty;
                            string CurrentImportNodeId = string.Empty;

                            ImportProcessStati CurrentNodeRowErrorStatus = ImportProcessStati.Imported;

                            if( null != NodeType ) //if we can't get the node type, we are hosed
                            {


                                //grab the node we created in the previous process phase
                                Int32 CurrentNbtNodeId = CswConvert.ToInt32( CurrentImportNodeRow[Colname_NbtNodeId] );
                                CswPrimaryKey CurrentNbtPrimeKey = new CswPrimaryKey( "nodes", CurrentNbtNodeId );
                                CswNbtNode CurrentNbtNode = _CswNbtResources.Nodes[CurrentNbtPrimeKey];


                                if( null != CurrentNbtNode )
                                {
                                    try
                                    {


                                        //Select the corresponding property records
                                        CurrentImportNodeId = CurrentImportNodeRow[_ColName_ImportNodeId].ToString();
                                        CurrentImportNodeName = CurrentImportNodeRow[_ColName_Nodes_NodeName].ToString();

                                        //string TheJoinQuery = "select n.importnodeid, n.nodename,p.* from " + _TblName_ImportNodes + " n join " + _TblName_ImportProps + " p on (n.importnodeid=p.importnodeid) where  n.importnodeid='" + CurrentImportNodeId + "'";
                                        //CswArbitrarySelect CswArbitrarySelectUnProcessedProps = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssedprops", TheJoinQuery );
                                        //DataTable CurrentUnprocssedPropsTable = CswArbitrarySelectUnProcessedProps.getTable();

                                        string TheWhereClause = "where  importnodeid='" + CurrentImportNodeId + "'";
                                        CswTableUpdate CswTableUpdateImportProps = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "import_props_update_" + CurrentImportNodeId, _TblName_ImportProps );
                                        DataTable CurrentUnprocssedPropsTable = CswTableUpdateImportProps.getTable( TheWhereClause );




                                        //Apply the property records to the node
                                        RelationshipPropAddCounter = 0;


                                        foreach( DataRow CurrentImportPropRow in CurrentUnprocssedPropsTable.Rows )
                                        {
                                            // previous errors shouldn't prevent us from continuing to import
                                            //                                            CurrentNodeRowErrorStatus = ImportProcessStati.Imported;

                                            string CurrentPropRowErrors = string.Empty;
                                            ImportProcessStati CurrentPropRowErrorStatus = ImportProcessStati.Imported;

                                            try
                                            {



                                                string CurrentNodeTypePropname = CurrentImportPropRow["nodetypepropname"].ToString();

                                                if( ( "user" != NodeType.NodeTypeName.ToLower() ) || ( "role" != CurrentNodeTypePropname.ToLower() ) )
                                                {

                                                    CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp = NodeType.getNodeTypeProp( CurrentNodeTypePropname );

                                                    //if( ( string.Empty == CurrentNbtNode.Properties[CurrentNodeTypeProp].Gestalt.ToString() ) || ( CurrentNbtNode.Properties[CurrentNodeTypeProp].FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ) )
                                                    //{

                                                    if( null != CurrentNodeTypeProp )
                                                    {

                                                        Dictionary<string, Int32> ImportNodeIdToNbtNodeId = new Dictionary<string, int>();
                                                        if( false == CurrentImportPropRow.IsNull( _ColName_Props_ImportTargetNodeIdUnique ) ) //populate ImportNodeIdToNbtNodeId for relationships and locations
                                                        {
                                                            string CurrentImportTargetNodeId = CurrentImportPropRow[_ColName_Props_ImportTargetNodeIdUnique].ToString();

                                                            if( ( false == CurrentImportTargetNodeId.Contains( "--" ) ) ||
                                                                3 == CurrentImportTargetNodeId.Split( new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries ).Length ) //IMCS import references must have all three components in order to be not null
                                                            {

                                                                string Query = "select " + Colname_NbtNodeId + " from " + _TblName_ImportNodes + " where lower(" + _ColName_ImportNodeId + ")=lower('" + CurrentImportTargetNodeId + "')";
                                                                CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "findtargetnodeid", Query );
                                                                DataTable DataTable = CswArbitrarySelect.getTable();
                                                                if( ( DataTable.Rows.Count > 0 ) && ( false == DataTable.Rows[0].IsNull( Colname_NbtNodeId ) ) )
                                                                {

                                                                    Int32 NodeIdOfDestinationNode = CswConvert.ToInt32( DataTable.Rows[0][Colname_NbtNodeId] );
                                                                    if( Int32.MinValue != NodeIdOfDestinationNode )
                                                                    {
                                                                        string CandidateValidationErrorMessage = string.Empty;
                                                                        if( _validateTargetNodeType( CurrentNodeTypeProp, NodeIdOfDestinationNode, ref CandidateValidationErrorMessage ) )
                                                                        {
                                                                            ImportNodeIdToNbtNodeId.Add( CswTools.XmlRealAttributeName( CurrentImportPropRow[_ColName_Props_ImportTargetNodeIdUnique].ToString() ).ToLower(), CswConvert.ToInt32( DataTable.Rows[0][Colname_NbtNodeId] ) );
                                                                            RelationshipPropAddCounter++;
                                                                        }
                                                                        else
                                                                        {
                                                                            CurrentPropRowErrors += "Unable to set the " + CurrentNodeTypePropname + " property of the node with import node id " + CurrentImportNodeId + "): " + CandidateValidationErrorMessage;
                                                                            CurrentPropRowErrorStatus = ImportProcessStati.PropsError;

                                                                        }//if-else
                                                                    }
                                                                    else
                                                                    {
                                                                        CurrentPropRowErrors += "The would-be destination node id (" + DataTable.Rows[0][Colname_NbtNodeId].ToString() + ") for reference from import prop of type " + CurrentNodeTypePropname + " (which is a property of node with import node id " + CurrentImportNodeId + ") cannot be converted to an integer";
                                                                        CurrentPropRowErrorStatus = ImportProcessStati.PropsError;

                                                                    }
                                                                }
                                                                else //_ColName_ImportNodeId did not specify a row in the target schema that provides the nodeid of an imported node; so perhaps it specifies the nbtnodeid of a node that already exists in the schema
                                                                {
                                                                    if( Int32.MinValue != CswConvert.ToInt32( CurrentImportTargetNodeId ) )
                                                                    {
                                                                        CswTableSelect CswTableSelectFromNodes = _CswNbtResources.makeCswTableSelect( "rawselectfromnodes", "nodes" );
                                                                        DataTable ExistingNodeDataTable = CswTableSelectFromNodes.getTable( " where nodeid=" + CurrentImportTargetNodeId );


                                                                        if( ExistingNodeDataTable.Rows.Count > 0 ) //it _does_ exist in the target schema
                                                                        {
                                                                            Int32 ExistingNbtNodeId = CswConvert.ToInt32( CurrentImportTargetNodeId ); //review 24884
                                                                            if( Int32.MinValue != ExistingNbtNodeId )
                                                                            {

                                                                                string CandidateValidationErrorMessage = string.Empty;
                                                                                if( _validateTargetNodeType( CurrentNodeTypeProp, ExistingNbtNodeId, ref CandidateValidationErrorMessage ) )
                                                                                {

                                                                                    ImportNodeIdToNbtNodeId.Add( CswTools.XmlRealAttributeName( CurrentImportPropRow[_ColName_Props_ImportTargetNodeIdUnique].ToString() ).ToLower(), ExistingNbtNodeId );
                                                                                    RelationshipPropAddCounter++;
                                                                                }
                                                                                else
                                                                                {
                                                                                    CurrentPropRowErrors += "Unable to set the " + CurrentNodeTypePropname + " property of the node with import node id " + CurrentImportNodeId + "): " + CandidateValidationErrorMessage;
                                                                                    CurrentPropRowErrorStatus = ImportProcessStati.PropsError;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                CurrentPropRowErrors += "The would-be destination node id (" + DataTable.Rows[0][Colname_NbtNodeId].ToString() + ") for reference from import prop of type " + CurrentNodeTypePropname + " (which is a property of node with import node id " + CurrentImportNodeId + ") cannot be converted to an integer";
                                                                                CurrentPropRowErrorStatus = ImportProcessStati.PropsError;
                                                                            }
                                                                        }
                                                                        else //it is neither in the import data nor in the target schema
                                                                        {
                                                                            //having eliminated null node IDs, this condition would be a true error
                                                                            //(as a opposed to a who-knew-from-null-nodeids error . . . 
                                                                            CurrentPropRowErrors += "Unable to find target node with node id " + CurrentImportTargetNodeId + " for reference from import prop of type " + CurrentNodeTypePropname + " (which is a property of node with import node id " + CurrentImportNodeId + ")";
                                                                            CurrentPropRowErrorStatus = ImportProcessStati.PropsError;
                                                                        }//if-else we were able to find the target node in the destination schema

                                                                    }
                                                                    else
                                                                    {
                                                                        CurrentPropRowErrors += "Unable to find target node with node id " + CurrentImportTargetNodeId + " for reference from import prop of type " + CurrentNodeTypePropname + " (which is a property of node with import node id " + CurrentImportNodeId + ")";
                                                                        CurrentPropRowErrorStatus = ImportProcessStati.PropsError;

                                                                    }//if-else the node id is numeric (and a thus a candidate to be an NBT node ID)

                                                                }//if-else we found the target node in the import data


                                                            }//if the target node id is not null


                                                        }//if our property references a node (i.e., its a relation nodetype) 


                                                        //It appears to me that the third parameter of ReadDataRow() -- a map of source to destination nodetypeids -- 
                                                        //is only necessary when you are importing meta data as well as node data; we're not doing that yet here

                                                        //need to do this so that ReadDataRow will get the columnname he expects :-( 
                                                        //and then we need to change it back; 
                                                        //this is major kludgedelia
                                                        //Need a mechanism for dynamically changing the column names that ReadDataRow expects
                                                        if( ImportProcessStati.PropsError != CurrentNodeRowErrorStatus )
                                                        {
                                                            CurrentImportPropRow.Table.Columns[_ColName_Props_ImportTargetNodeIdUnique].ColumnName = _ColName_Props_ImportTargetNodeIdOriginal;
                                                            try
                                                            {

                                                                CurrentNbtNode.Properties[CurrentNodeTypeProp].ReadDataRow( CurrentImportPropRow, ImportNodeIdToNbtNodeId, null );
                                                                PropAddCounter++;
                                                            }

                                                            finally
                                                            {
                                                                CurrentImportPropRow.Table.Columns[_ColName_Props_ImportTargetNodeIdOriginal].ColumnName = _ColName_Props_ImportTargetNodeIdUnique;
                                                            }
                                                        }//if we have not encountered any errors so far

                                                    }
                                                    else
                                                    {
                                                        CurrentPropRowErrors += "Unable to import the " + CurrentNodeTypePropname + " property for the node named " + CurrentImportNodeName + " (importnodeid " + CurrentImportNodeId + "): it's Node type " + CurrentNodeTypeName + " does not have this property";
                                                        CurrentPropRowErrorStatus = ImportProcessStati.PropsError;

                                                    }//if-else we were able to retrieve the nodetype prop

                                                    //}
                                                    //else
                                                    //{
                                                    //    CurrentRowError += "Did not import property " + CurrentNodeTypePropname + " for node type " + CurrentNodeTypeName + " named " + CurrentImportNodeName + " because a property with that name is already defined for this node";
                                                    //    CurrentErrorStatus = ImportProcessStati.PropsError;


                                                    //}//if the property already exists for the node (the node itself probably already was in the system)



                                                }//if it is not the user node's role property, which we would have already set

                                            }

                                            catch( Exception PropsException )
                                            {
                                                CurrentPropRowErrors += "Exception importing props row: " + PropsException.Message; ;
                                                CurrentPropRowErrorStatus = ImportProcessStati.PropsError;
                                            }

                                            finally
                                            {
                                                CurrentImportPropRow[_ColName_ProcessStatus] = CurrentPropRowErrorStatus.ToString();
                                                if( ImportProcessStati.PropsError == CurrentPropRowErrorStatus )
                                                {
                                                    CurrentImportPropRow[_ColName_StatusMessage] = CurrentPropRowErrors;
                                                }

                                                CswTableUpdateImportProps.update( CurrentUnprocssedPropsTable );

                                            }

                                        }//iterate prop rows


                                        CurrentNbtNode.postChanges( false );//write node when done iterating prop rows
                                        _CswImportExportStatusReporter.reportTiming( AddPropsToNodeTimer, "Add and post " + PropAddCounter.ToString() + " props (of which " + RelationshipPropAddCounter.ToString() + " are relationships) to one node" );
                                        PropAddCounter = 0;
                                        AddPropsToNodeTimer.Start();//reset the timer as soon as we report

                                    }//try 

                                    catch( Exception Exception )
                                    {
                                        string Message = "Error importing properties for import nodeid: " + CurrentImportNodeId + " of node name " + CurrentImportNodeName + " and node type " + CurrentNodeTypeName + ": " + Exception.Message + "; the NBT node has been imported, but it has no properties as a result of this error";

                                        DataTable NodesTable = CswTableUpdateNodes.getTable( " where " + _ColName_ImportNodeId + " = '" + CurrentImportNodeId + "'" );
                                        NodesTable.Rows[0][_ColName_StatusMessage] = Message;
                                        NodesTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.Error.ToString();
                                        CswTableUpdateNodes.update( NodesTable );

                                        _CswImportExportStatusReporter.reportError( Message );

                                    }//catch

                                }
                                else
                                {
                                    CurrentNodeRowError += "Unable to retrieve node with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId;
                                    CurrentNodeRowErrorStatus = ImportProcessStati.Error;

                                }//if-else we were able to build a node from the current node row

                            }
                            else
                            {
                                CurrentNodeRowError += "Unable to import nodetype with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId + ",  its node type  (" + CurrentNodeTypeName + ") could not be retrieved; ";
                                CurrentNodeRowErrorStatus = ImportProcessStati.Error;

                            }//if-else we were able to retrieve the node type

                            DataTable DataTableCurrentNodeTable = CswTableUpdateNodes.getTable( " where " + _ColName_ImportNodeId + "='" + CurrentImportNodeId + "'" );
                            if( DataTableCurrentNodeTable.Rows.Count > 0 )
                            {



                                if( ImportProcessStati.Error != CurrentNodeRowErrorStatus )
                                {
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.PropsAdded.ToString();
                                }
                                else
                                {
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = CurrentNodeRowErrorStatus.ToString();
                                    DataTableCurrentNodeTable.Rows[0][_ColName_StatusMessage] = CurrentNodeRowError;

                                    if( ImportProcessStati.Error == CurrentNodeRowErrorStatus )
                                    {
                                        _CswImportExportStatusReporter.reportError( CurrentNodeRowError );
                                    }
                                }//if else there was an error on the current row




                                TotalNodesProcesssedSoFar++;
                                CswTableUpdateNodes.update( DataTableCurrentNodeTable );
                            }//for some reason, it _can_ happen that this row is empty

                        }//iterate rows of node records to process

                        _commitAndRelease();
                        //                        _CswNbtResources.releaseDbResources();
                        _CswImportExportStatusReporter.reportTiming( CommitNNodesTimer, "Add and commit props for " + _CswNbtImportOptions.NodeAddPropsPageSize.ToString() + " nodes" );



                        _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalNodesToProcess, TotalNodesProcesssedSoFar, ProcessStates.InProcess );


                    }//if we are not on the first iteration

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    NodeRecordsToProcess = CswArbitrarySelectUnprocessedNodes.getTable( 0, _CswNbtImportOptions.NodeAddPropsPageSize, false, false );
                    CommitNNodesTimer.Start();

                } while( ( false == _Stop ) && ( NodeRecordsToProcess.Rows.Count > 0 ) );


                //if by coincidence we hit "stop" just as this phase completed, it's not the end of the world: 
                //when we resume, we'll resume at this phase, but the query won't reutrn results, and we'll just 
                //fall through to the next phase
                if( false == _Stop )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalNodesToProcess, TotalNodesProcesssedSoFar, ProcessStates.Complete );
                }


            }//if nodes have been populated

            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingNbtProps == _LastCompletedProcessPhase ) )
            {
                //Unforunately, we have to wait until _everything_ else is populated in order to see all the nodes and their various relationships 

                _LastCompletedProcessPhase = ImportProcessPhase.PostProcessingNbtNodes;

                string WhereClauseForImportedNodeRecords = " where " + _ColName_ProcessStatus + "='" + ImportProcessStati.PropsAdded.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_ImportNodes + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectpossibleduplicatenodes", "select " + SelectColumns + " from " + _TblName_ImportNodes + WhereClauseForImportedNodeRecords );
                CswTableUpdate CswTableUpdateNodes = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatefornodepropupdatestatus", _TblName_ImportNodes );

                DataTable NodeRecordsToProcess = null;
                do
                {

                    CswTimer PostProcessNodesAndPropsTimer = new CswTimer();

                    if( ( null != NodeRecordsToProcess ) && ( NodeRecordsToProcess.Rows.Count > 0 ) )
                    {

                        string CurrentImportNodeId = string.Empty;
                        string CurrentRowError = string.Empty;

                        List<CswPrimaryKey> DeletedImportNodesThisCycle = new List<CswPrimaryKey>();
                        List<CswPrimaryKey> MarkedPendingNodesThisCycle = new List<CswPrimaryKey>();

                        foreach( DataRow CurrentImportNodeRow in NodeRecordsToProcess.Rows )
                        {
                            CurrentImportNodeId = CurrentImportNodeRow[_ColName_ImportNodeId].ToString();
                            Int32 CurrentNbtNodeId = CswConvert.ToInt32( CurrentImportNodeRow[Colname_NbtNodeId] );
                            CswPrimaryKey CurrentNbtPrimeKey = new CswPrimaryKey( "nodes", CurrentNbtNodeId );
                            CswNbtNode CurrentNbtNode = _CswNbtResources.Nodes[CurrentNbtPrimeKey];

                            if( null != CurrentNbtNode )
                            {
                                //First check for dundancy
                                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in _CswNbtResources.MetaData.getNodeTypeProps( CurrentNbtNode.NodeTypeId ) )
                                {
                                    //Theoretically, we should be chcking uniqueness for props that are required and empty, but this 
                                    //kind of thing is so easy to change in the master, making the risk of deleting nodes wantonly too high
                                    if( MetaDataProp.IsUnique() && ( false == CurrentNbtNode.Properties[MetaDataProp].Empty ) )
                                    {
                                        CswNbtNode OtherNode = _CswNbtResources.Nodes.FindNodeByUniqueProperty( MetaDataProp, CurrentNbtNode.Properties[MetaDataProp] );
                                        if( OtherNode != null && OtherNode.NodeId != CurrentNbtNode.NodeId )
                                        {

                                            string MessageStem = "The " + MetaDataProp.PropName + " property of the node corresponding to this record has the same value -- " + CurrentNbtNode.Properties[MetaDataProp].Gestalt + " --  as a previously existing node; ";

                                            if( IMode == ImportMode.Overwrite )
                                            {
                                                OtherNode.delete();
                                                CurrentRowError += MessageStem + " the node in the target schema is being deleted";
                                            }
                                            else if( IMode == ImportMode.Duplicate )
                                            {
                                                // It would be better not to create the node in the first place
                                                // but we're waiting on BZ 9650 to be fixed.
                                                CurrentNbtNode.delete();
                                                DeletedImportNodesThisCycle.Add( CurrentNbtNode.NodeId );
                                                CurrentRowError += MessageStem + " this record's node is being deleted";
                                                break;
                                            }

                                        } // if( OtherNode != null )

                                    } // if( MetaDataProp.IsUnique )

                                } // foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in Node.NodeType.NodeTypeProps )

                                if( false == DeletedImportNodesThisCycle.Contains( CurrentNbtNode.NodeId ) )
                                {
                                    if( CurrentNbtNode.getObjectClass().ObjectClass == NbtObjectClass.EquipmentClass )
                                    {
                                        if( CurrentNbtNode.Properties[_CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( CurrentNbtNode.NodeTypeId, "Assembly" )].AsRelationship.RelatedNodeId != null )
                                        {
                                            CurrentNbtNode.PendingUpdate = true;
                                            CurrentNbtNode.postChanges( false, false, true );
                                            MarkedPendingNodesThisCycle.Add( CurrentNbtNode.NodeId );
                                        }
                                    }

                                }//if we haven't already deleted this node

                            }
                            else
                            {
                                CurrentRowError += "Unable to retrieve NBT node with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId + " and NodeId of " + CurrentNbtNode.ToString();
                            }//if-else we were able to retrieve the node



                            DataTable DataTableCurrentNodeTable = CswTableUpdateNodes.getTable( " where " + _ColName_ImportNodeId + "='" + CurrentImportNodeId + "'" );
                            if( DataTableCurrentNodeTable.Rows.Count > 0 )
                            {
                                if( string.Empty == CurrentRowError )
                                {
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.RedundancyChecked.ToString();
                                }
                                else
                                {
                                    //we're not putting the errors into the prop records because that would add to the expense of the process. 
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.Error.ToString();
                                    DataTableCurrentNodeTable.Rows[0][_ColName_StatusMessage] = CurrentRowError;
                                    _CswImportExportStatusReporter.reportError( CurrentRowError );
                                }//if else there was an error on the current row

                                CurrentRowError = string.Empty;
                                CswTableUpdateNodes.update( DataTableCurrentNodeTable );

                            }//for some reason, it _can_ happen that this row is empty


                            TotalNodesProcesssedSoFar++;

                        }//iterate node records


                        if( DeletedImportNodesThisCycle.Count > 0 )
                        {
                            string ActionTaken = ( ImportMode.Overwrite == IMode ) ? "nodes in target schema were deleted" : "import nodes were deleted";
                            string NodeList = string.Empty;
                            foreach( CswPrimaryKey CurrentKey in DeletedImportNodesThisCycle )
                            {
                                NodeList += CurrentKey.ToString() + ", ";
                            }
                            _CswImportExportStatusReporter.reportProgress( "The following " + ActionTaken + ": " + NodeList );

                            DeletedImportNodesThisCycle.Clear();
                        }

                        if( MarkedPendingNodesThisCycle.Count > 0 )
                        {
                            string NodeList = string.Empty;
                            foreach( CswPrimaryKey CurrentKey in DeletedImportNodesThisCycle )
                            {
                                NodeList += CurrentKey.ToString() + ", ";
                            }
                            _CswImportExportStatusReporter.reportProgress( "The following nodes were marked pending update: " + NodeList );

                            MarkedPendingNodesThisCycle.Clear();
                        }

                        //CswCommaDelimitedStringOfPendingUpdateNodes

                        _commitAndRelease();
                        _CswImportExportStatusReporter.reportTiming( PostProcessNodesAndPropsTimer, "Post processing of " + _CswNbtImportOptions.NodeAddPropsPageSize.ToString() + " Nodes" );

                    }//if we have node records to process

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    NodeRecordsToProcess = CswArbitrarySelectUnprocessedNodes.getTable( 0, _CswNbtImportOptions.NodeAddPropsPageSize, false, false );
                    PostProcessNodesAndPropsTimer.Start();

                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalNodesToProcess, TotalNodesProcesssedSoFar, ProcessStates.InProcess );


                } while( ( false == _Stop ) && ( NodeRecordsToProcess.Rows.Count > 0 ) );


                if( false == _Stop )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( ImportProcessPhase.Completed, 0, 0, ProcessStates.Complete );
                }


            }

        } // ImportXml()


        private void _rehabilitateUser( CswNbtNode UserNode, string UserName, CswNbtNode RoleNode )
        {
            UserNode.Properties["Username"].AsText.Text = UserName;
            if( RoleNode != null )
            {
                UserNode.Properties["Role"].AsRelationship.RelatedNodeId = RoleNode.NodeId;
            }

            UserNode.Properties["AccountLocked"].AsLogical.Checked = Tristate.True;
        }//rehabilitateUser() 

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


