using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public enum AbsentImportNodeHandling { DeduceAndCreate, RejectImport }
    public enum ImportStartPoint { NukeAndStartOver, Resume }

    public class CswImporterDbTables : ICswImporter
    {

        private CswNbtResources _CswNbtResources = null;
        private CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;

        private CswNbtImportOptions _CswNbtImportOptions = null;

        private CswNbtImportStatus _CswNbtImportStatus = null;

        public CswImporterDbTables( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter, CswNbtImportStatus CswNbtImportStatus )
        {
            _CswNbtImportStatus = CswNbtImportStatus;
            _CswNbtImportOptions = new CswNbtImportOptions(); //This will be passed in as a ctor arg

            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );
        }//ctor


        public void reset()
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_TempNodes ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_TempNodes ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( _TblName_TempNodes );
            }

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_TempProps ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_TempProps ) ) //belt and suspenders
            {
                _CswNbtSchemaModTrnsctn.dropTable( _TblName_TempProps );
            } 
            
            _CswNbtImportStatus.reset(); 

        }//reset()

        private bool _Stop = false;
        public void stop()
        {

            if(
                ( _CswNbtImportStatus.TargetProcessPhase == ImportProcessPhase.PopulatingTempTableNodes ) ||
                 ( _CswNbtImportStatus.TargetProcessPhase == ImportProcessPhase.PopulatingTempTableNodes )
                )
            {
                //It's actually kind of messy code-wise to stop in the populate-temp-table phase, so for now we'll just say "no"
                _CswImportExportStatusReporter.reportProgress( "Stop message received: cannot stop process while in " + _CswNbtImportStatus.TargetProcessPhase.ToString() + " phase" );

            }
            else
            {
                _CswImportExportStatusReporter.reportProgress( "Stop message received, shutting down . . . " );
                _Stop = true;

            }
        }//stop() 

        private string _ColName_ProcessStatus = "processstatus";
        private string _ColName_StatusMessage = "statusmessage";
        private string _ColName_Source = "source";
        private string _ColName_ImportNodeId = "importnodeid";
        private string _ColName_Props_ImportTargetNodeIdUnique = "importtargetnodeid";
        private string _ColName_Props_ImportTargetNodeIdOriginal = "NodeID";
        private string _ColName_Nodes_NodeName = "nodename";
        private string _TblName_TempNodes = "tmp_import_nodes";
        private string _TblName_TempProps = "tmp_import_props";

        private string _StatusMessageDivider = "==================================";

        ImportProcessPhase _LastCompletedProcessPhase = ImportProcessPhase.NothingDoneYet;

        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="XmlStr">Source XML string</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _CswImportExportStatusReporter.reportProgress( _StatusMessageDivider + "Starting Import -- Experimental" );

            ErrorLog = string.Empty;

            _Stop = false;


            //*********************************************************************************************************
            //*********************** Load to dataset
            _LastCompletedProcessPhase = _CswNbtImportStatus.CompletedProcessPhase;

            _CswImportExportStatusReporter.reportProgress( "Loading XML document to in memory tables" );



            //*********************************************************************************************************
            //*********************** Local variable definitions

            string ColName_TempNodesTablePk = "tmpimportnodesid";
            string Colname_NbtNodeId = "nbtnodeid";

            string ColName_TempPropsTablePk = "tmpimportpropsid";
            string ColName_TempPropsRealPropId = "nbtnodepropid";


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
            //*********************** Create Temporary Tables
            if( ( false == _Stop ) && ( ImportProcessPhase.NothingDoneYet == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.LoadingInputFile;

                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                DataSet DataSet = _CswNbtImportExportFrame.AsDataSet();
                DataTable TableOfNodesFromXml = DataSet.Tables["Node"];
                TableOfNodesFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;


                DataTable TableOfPropsFromXml = DataSet.Tables["PropValue"];
                TableOfPropsFromXml.Columns["NodeID"].ColumnName = _ColName_Props_ImportTargetNodeIdUnique; //This is not a joke
                TableOfPropsFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;



                _CswNbtSchemaModTrnsctn.beginTransaction();

                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_TempNodes ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_TempNodes ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( _TblName_TempNodes );
                }

                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TblName_TempProps ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TblName_TempProps ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( _TblName_TempProps );
                }

                _CswNbtSchemaModTrnsctn.commitTransaction();




                _CswNbtSchemaModTrnsctn.beginTransaction();

                _makeTempTable( _TblName_TempNodes, ColName_TempNodesTablePk, TableOfNodesFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
                _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_TempNodes, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );



                _makeTempTable( _TblName_TempProps, ColName_TempPropsTablePk, TableOfPropsFromXml.Columns, 512, _AdditonalColumns, _IndexColumns );
                //            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( TempPropsTableName, TempNodesTablePkColName, "refers to node records in " + TempNodesTableName + ";", false, false, TempNodesTableName, TempNodesTablePkColName );
                _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_TempProps, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );
                _CswNbtSchemaModTrnsctn.addLongColumn( _TblName_TempProps, ColName_TempPropsRealPropId, "to be filled in when the node is actually created", false, false );



                _CswNbtSchemaModTrnsctn.commitTransaction();

                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );


                //*********************************************************************************************************
                //*********************** Fill Temporary tables
                //_CswImportExportStatusReporter.reportProgress( "Filling temporary tables (this may take a while)" );
                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingTempTableNodes;
                _createTempRecords( TableOfNodesFromXml, _TblName_TempNodes, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );

                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingTempTableProps;
                _createTempRecords( TableOfPropsFromXml, _TblName_TempProps, _CswNbtImportOptions.MaxInsertRecordsPerTransaction, _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate );
                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.Complete );


                _CswNbtImportExportFrame.clear();


            }//if we haven't done anything yet

            CswNbtNode GeneralUserRole = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( _CswNbtImportOptions.NameOfDefaultRoleForUserNodes );
            CswTableUpdate CswTableUpdateTempNodesTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatenodesfornodeid", _TblName_TempNodes );
            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingTempTableProps == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingNbtNodes;
                //_CswImportExportStatusReporter.reportProgress( _StatusMessageDivider + "Creating NBT Nodes" );

                //string WhereClauseForUnprocessedRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.Unprocessed.ToString() + "' and nodetypename <> 'User'";
                //string WhereClauseForUnprocessedRecords = " where " + _ColName_ProcessStatus + "='" + ProcessStati.Unprocessed.ToString() + "'" + " and nodetypename <> 'User'";
                string WhereClauseForUnprocessedRecords = " where " + _ColName_ProcessStatus + "='" + ImportProcessStati.Unprocessed.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_TempNodes + WhereClauseForUnprocessedRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_Nodes_NodeName );
                SelectColumns.Add( "nodetypename" );
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                string RawNodesQuery = "select " + SelectColumns + " from " + _TblName_TempNodes + WhereClauseForUnprocessedRecords;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", RawNodesQuery );
                CswTableUpdate CswTableUpdateTempPropsTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatepropsfornodeid", _TblName_TempProps );
                DataTable RawNodesTable = null;
                do
                {
                    if( null != RawNodesTable )
                    {
                        foreach( DataRow CurrentRow in RawNodesTable.Rows ) //will be empty on 1st interation
                        {
                            string CurrentNodeTypeNameInTempTable = CurrentRow["nodetypename"].ToString();
                            string CurrentNodeNameInTempTable = CurrentRow[_ColName_Nodes_NodeName].ToString();
                            string ImportNodeId = CurrentRow[_ColName_ImportNodeId].ToString();

                            DataTable TempNodesUpdateTable = CswTableUpdateTempNodesTable.getTable( "where " + _ColName_ImportNodeId + "='" + ImportNodeId + "'" );
                            DataRow TempNodesUpdateRow = TempNodesUpdateTable.Rows[0];

                            try
                            {



                                //if( Int32.MinValue == _doesNodeNameAlreadyExist( ImportNodeId ) )
                                //{
                                CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeNameInTempTable );
                                if( null != CurrentNodeType )
                                {


                                    CswNbtNode CswNbtNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                    //CswNbtNode.NodeName = CurrentNodeNameInTempTable; 


                                    if( CurrentNodeType.NodeTypeName.ToLower() == "user" )
                                    {
                                        _rehabilitateUser( CswNbtNode, CurrentNodeNameInTempTable, GeneralUserRole );
                                    }

                                    CswNbtNode.postChanges( false );

                                    Int32 NbtNodeId = CswNbtNode.NodeId.PrimaryKey;


                                    TempNodesUpdateRow[Colname_NbtNodeId] = NbtNodeId;
                                    TempNodesUpdateRow[_ColName_Source] = ImportSource.ImportData;
                                    CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );

                                    TempNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Imported;

                                    // This visibly slows things down and is probably not necessary
                                    //TempPropsUpdateTable = CswTableUpdateTempPropsTable.getTable( " where nodeid = '" + CurrentNodeIdInTempTable + "'" );
                                    //foreach( DataRow CurrentPropsUpdateRow in TempPropsUpdateTable.Rows )
                                    //{
                                    //    CurrentPropsUpdateRow[TempNodesRealNodeIdColName] = RealNbtNodeId;
                                    //}

                                }
                                else
                                {
                                    string Error = "Unable to import node  " + CurrentNodeNameInTempTable + " @ " + _ColName_ImportNodeId + " " + ImportNodeId + ",  because its nodetype (" + CurrentNodeTypeNameInTempTable + ") does not exist in the target schema";
                                    _CswImportExportStatusReporter.reportError( Error );
                                    TempNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Error;
                                    TempNodesUpdateRow[_ColName_StatusMessage] = Error;

                                }//if-else current node's nodetype exists

                            }//try

                            catch( Exception Exception )
                            {
                                TempNodesUpdateRow[_ColName_ProcessStatus] = ImportProcessStati.Error;
                                TempNodesUpdateRow[_ColName_StatusMessage] = Exception.Message;

                                _CswImportExportStatusReporter.reportException( Exception );
                            }//catch

                            //}
                            //else
                            //{
                            //    string Error = "Unable to import node " + CurrentNodeNameInTempTable + ",  because it already exists in the database";
                            //    _CswImportExportStatusReporter.reportError( Error );
                            //    TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Error;
                            //    TempNodesUpdateRow[_ColName_StatusMessage] = Error;


                            //}//if-else name is not already defined



                            CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );

                            //CswTableUpdateTempPropsTable.update( TempPropsUpdateTable ); <== prop updating visibly slows things down and is not necessary

                            TotalNodesProcesssedSoFar++;

                        }//iterate raw node rows


                        if( RawNodesTable.Rows.Count > 0 )
                        {
                            _CswNbtResources.finalize();
                            _CswNbtResources.clearUpdates();
                            _CswNbtResources.releaseDbResources();
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




            if( ( false == _Stop ) && ( ImportProcessPhase.PopulatingNbtNodes == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.VerifyingNbtTargetNodes;

                _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, 0, 0, ProcessStates.InProcess );

                /*
                string QueryForMissingTargetNodes = @"select distinct p.importtargetnodeid
                                                      from tmp_import_props p
                                                      left outer join tmp_import_nodes n on (p.importtargetnodeid =
                                                                                            n.importnodeid)
                                                     where n.importnodeid is null
                                                       and p.importtargetnodeid is not null";
                 */

                //query for nodes that are targets of relationship proeprties ,
                //and whose which additionally do not have nbtnodeid's, presumably because the 
                //first stage could not be created
                string QueryForMissingTargetNodes = @"select distinct p.importtargetnodeid
                                      from tmp_import_props p
                                     where p.importtargetnodeid is not null
                                       and ( p.importtargetnodeid not in
                                           (select distinct importnodeid from tmp_import_nodes) ) or 
                                           nbtnodeid is null";



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
                                    DataTable AddMissingNodeEntryTable = CswTableUpdateTempNodesTable.getEmptyTable();
                                    DataRow NewNodeEntryRow = AddMissingNodeEntryTable.NewRow();
                                    AddMissingNodeEntryTable.Rows.Add( NewNodeEntryRow );
                                    NewNodeEntryRow[_ColName_ImportNodeId] = ImportNodeIdOfAbsentNode;
                                    NewNodeEntryRow[_ColName_Nodes_NodeName] = TargetNodeThatWasMissing.NodeName;
                                    NewNodeEntryRow[_ColName_ProcessStatus] = ImportSource.Deduced.ToString();
                                    NewNodeEntryRow[Colname_NbtNodeId] = TargetNodeThatWasMissing.NodeId.PrimaryKey.ToString();

                                    CswTableUpdateTempNodesTable.update( AddMissingNodeEntryTable );

                                    _CswImportExportStatusReporter.reportProgress( "Added missing node " + TargetNodeThatWasMissing.NodeName );
                                    AddedNodesCounter++;
                                }
                                else
                                {
                                    _CswImportExportStatusReporter.reportError( "Unable to auto-create node for ID " + ImportNodeIdOfAbsentNode + ": there is no node type '" + CswNbtImportNodeId.NodeNodeType + "'" );
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

            if( ( false == _Stop ) && ( ImportProcessPhase.VerifyingNbtTargetNodes == _LastCompletedProcessPhase ) )
            {

                _LastCompletedProcessPhase = ImportProcessPhase.PopulatingNbtProps;



                string WhereClauseForImportedNodeRecords = " where " + _ColName_ProcessStatus + "='" + ImportProcessStati.Imported.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_TempNodes + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;



                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_Nodes_NodeName );
                SelectColumns.Add( "nodetypename" );
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select " + SelectColumns + " from " + _TblName_TempNodes + WhereClauseForImportedNodeRecords );
                CswTableUpdate CswTableUpdateNodes = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatefornodepropupdatestatus", _TblName_TempNodes );



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

                            string CurrentRowError = string.Empty;
                            string CurrentNodeTypeName = CurrentImportNodeRow["nodetypename"].ToString();
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeName );


                            string CurrentImportNodeId = string.Empty;
                            if( null != NodeType ) //if we can't get the node type, we are hosed
                            {


                                //grab the node we created in the previous process phase
                                Int32 CurrentNbtNodeId = CswConvert.ToInt32( CurrentImportNodeRow[Colname_NbtNodeId] );
                                CswPrimaryKey CurrentNbtPrimeKey = new CswPrimaryKey( "nodes", CurrentNbtNodeId );
                                CswNbtNode CurrentNbtNode = _CswNbtResources.Nodes[CurrentNbtPrimeKey];


                                if( null != CurrentNbtNode )
                                {

                                    //Select he corresponding property records
                                    CurrentImportNodeId = CurrentImportNodeRow[_ColName_ImportNodeId].ToString();
                                    string TheQuery = "select n.importnodeid, n.nodename,p.* from tmp_import_nodes n join tmp_import_props p on (n.importnodeid=p.importnodeid) where  n.importnodeid='" + CurrentImportNodeId + "'";
                                    CswArbitrarySelect CswArbitrarySelectUnProcessedProps = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssedprops", TheQuery );
                                    DataTable CurrentUnprocssedPropsTable = CswArbitrarySelectUnProcessedProps.getTable();

                                    //Apply the property records to the node
                                    RelationshipPropAddCounter = 0;

                                    foreach( DataRow CurrentImportProprow in CurrentUnprocssedPropsTable.Rows )
                                    {

                                        string CurrentNodeTypePropname = CurrentImportProprow["nodetypepropname"].ToString();

                                        if( ( "user" != NodeType.NodeTypeName.ToLower() ) && ( "role" != CurrentNodeTypePropname.ToLower() ) )
                                        {

                                            CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp = NodeType.getNodeTypeProp( CurrentNodeTypePropname );

                                            if( null != CurrentNodeTypeProp )
                                            {

                                                Dictionary<string, Int32> ImportNodeIdToNbtNodeId = new Dictionary<string, int>();
                                                if( false == CurrentImportProprow.IsNull( _ColName_Props_ImportTargetNodeIdUnique ) )
                                                {
                                                    string CurrentImportTargetNodeId = CurrentImportProprow[_ColName_Props_ImportTargetNodeIdUnique].ToString();
                                                    CswNbtImportNodeId CswNbtImportTargetNodeId = new ImportExport.CswNbtImportNodeId( CurrentImportTargetNodeId );
                                                    if( false == CswNbtImportTargetNodeId.IsNull )
                                                    {

                                                        string Query = "select " + Colname_NbtNodeId + " from " + _TblName_TempNodes + " where " + _ColName_ImportNodeId + "='" + CurrentImportTargetNodeId + "'";
                                                        CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "findtargetnodeid", Query );
                                                        DataTable DataTable = CswArbitrarySelect.getTable();
                                                        if( ( DataTable.Rows.Count > 0 ) && ( false == DataTable.Rows[0].IsNull( Colname_NbtNodeId ) ) )
                                                        {
                                                            ImportNodeIdToNbtNodeId.Add( CswTools.XmlRealAttributeName( CurrentImportProprow[_ColName_Props_ImportTargetNodeIdUnique].ToString() ).ToLower(), CswConvert.ToInt32( DataTable.Rows[0][Colname_NbtNodeId] ) );
                                                            RelationshipPropAddCounter++;
                                                        }
                                                        else
                                                        {
                                                            //having eliminated null node IDs, this condition would be a true error
                                                            //(as a opposed to a who-knew-from-null-nodeids error . . . 
                                                            CurrentRowError += "Unable to find target node with node id " + CurrentImportTargetNodeId + " for reference from import prop of type " + CurrentNodeTypePropname + " (which is a property of node with import node id " + CurrentImportNodeId + ")";
                                                        }

                                                    }//if the target node id is not null

                                                }//if our property references a node (i.e., its a relation nodetype) 


                                                //It appears to me that the third parameter of ReadDataRow() -- a map of source to destination nodetypeids -- 
                                                //is only necessary when you are importing meta data as well as node data; we're not doing that yet here

                                                //need to do this so that ReadDataRow will get the columnname he expects :-( 
                                                //and then we need to change it back; 
                                                //this is major kludgedelia
                                                //Need a mechanism for dynamically changing the column names that ReadDataRow expects
                                                CurrentImportProprow.Table.Columns[_ColName_Props_ImportTargetNodeIdUnique].ColumnName = _ColName_Props_ImportTargetNodeIdOriginal;
                                                try
                                                {
                                                    CurrentNbtNode.Properties[CurrentNodeTypeProp].ReadDataRow( CurrentImportProprow, ImportNodeIdToNbtNodeId, null );
                                                    PropAddCounter++;
                                                }

                                                finally
                                                {
                                                    CurrentImportProprow.Table.Columns[_ColName_Props_ImportTargetNodeIdOriginal].ColumnName = _ColName_Props_ImportTargetNodeIdUnique;
                                                }




                                            }
                                            else
                                            {
                                                CurrentRowError += "Unable to import nodeprop with nodetypename " + CurrentNodeTypePropname + ": could not find a node type prop of this name in node type " + CurrentNodeTypeName;
                                            }//if-else we were able to retrieve the nodetype prop

                                        }//if it is not the user node's role property, which we would have already set
                                    }//iterate prop rows


                                    CurrentNbtNode.postChanges( false );//write node when done iterating prop rows
                                    _CswImportExportStatusReporter.reportTiming( AddPropsToNodeTimer, "Add and post " + PropAddCounter.ToString() + " props (of which " + RelationshipPropAddCounter.ToString() + " are relationships) to one node" );
                                    PropAddCounter = 0;
                                    AddPropsToNodeTimer.Start();//reset the timer as soon as we report

                                }
                                else
                                {
                                    CurrentRowError += "Unable to retrieve node with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId;

                                }//if-else we were able to build a node from the current node row


                            }
                            else
                            {
                                CurrentRowError += "Unable to import nodetype with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId + ",  its node type  (" + CurrentNodeTypeName + ") could not be retrieved; ";
                            }//if-else we were able to retrieve the node type

                            DataTable DataTableCurrentNodeTable = CswTableUpdateNodes.getTable( " where " + _ColName_ImportNodeId + "='" + CurrentImportNodeId + "'" );
                            if( DataTableCurrentNodeTable.Rows.Count > 0 )
                            {
                                if( string.Empty == CurrentRowError )
                                {
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.PropsAdded.ToString();
                                }
                                else
                                {
                                    //we're not putting the errors into the prop records because that would add to the expense of the process. 
                                    DataTableCurrentNodeTable.Rows[0][_ColName_ProcessStatus] = ImportProcessStati.Error.ToString();
                                    DataTableCurrentNodeTable.Rows[0][_ColName_StatusMessage] = CurrentRowError;
                                    _CswImportExportStatusReporter.reportError( CurrentRowError );
                                }//if else there was an error on the current row
                                TotalNodesProcesssedSoFar++;
                                CswTableUpdateNodes.update( DataTableCurrentNodeTable );
                            }//for some reason, it _can_ happen that this row is empty

                        }//iterate rows of node records to process

                        _CswNbtResources.finalize();
                        _CswNbtResources.clearUpdates();
                        _CswNbtResources.releaseDbResources();
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
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + _TblName_TempNodes + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( _ColName_ImportNodeId );
                SelectColumns.Add( Colname_NbtNodeId );
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectpossibleduplicatenodes", "select " + SelectColumns + " from " + _TblName_TempNodes + WhereClauseForImportedNodeRecords );
                CswTableUpdate CswTableUpdateNodes = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatefornodepropupdatestatus", _TblName_TempNodes );

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
                                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in CurrentNbtNode.NodeType.NodeTypeProps )
                                {
                                    if( MetaDataProp.IsUnique )
                                    {
                                        CswNbtNode OtherNode = _CswNbtResources.Nodes.FindNodeByUniqueProperty( MetaDataProp, CurrentNbtNode.Properties[MetaDataProp] );
                                        if( OtherNode != null && OtherNode.NodeId != CurrentNbtNode.NodeId )
                                        {
                                            if( IMode == ImportMode.Overwrite )
                                            {
                                                OtherNode.delete();
                                            }
                                            else if( IMode == ImportMode.Duplicate )
                                            {
                                                // It would be better not to create the node in the first place
                                                // but we're waiting on BZ 9650 to be fixed.
                                                CurrentNbtNode.delete();
                                                DeletedImportNodesThisCycle.Add( CurrentNbtNode.NodeId );
                                                break;
                                            }

                                        } // if( OtherNode != null )

                                    } // if( MetaDataProp.IsUnique )

                                } // foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in Node.NodeType.NodeTypeProps )

                                if( false == DeletedImportNodesThisCycle.Contains( CurrentNbtNode.NodeId ) )
                                {
                                    if( CurrentNbtNode.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass )
                                    {
                                        if( CurrentNbtNode.Properties[CurrentNbtNode.NodeType.getNodeTypePropByObjectClassPropName( "Assembly" )].AsRelationship.RelatedNodeId != null )
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

                            TotalNodesProcesssedSoFar++;


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
                                TotalNodesProcesssedSoFar++;
                                CswTableUpdateNodes.update( DataTableCurrentNodeTable );

                            }//for some reason, it _can_ happen that this row is empty


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

                        _CswNbtResources.finalize();
                        _CswNbtResources.clearUpdates();
                        _CswNbtResources.releaseDbResources();
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

        private void _makeTempTable( string TableName, string PkColumnName, DataColumnCollection Columns, Int32 ArbitraryStringColumnLength, Collection<string> AdditionalStringColumns, Collection<string> IndexColumns )
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

        }//_makeTempTable() 


        private void _createTempRecords( DataTable SourceTable, string DestinationTableName, Int32 MaxInsertRecordsPerTransaction, Int32 MaxInsertRecordsPerDisplayUpdate )
        {
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "insertimportrecordsfortable_" + SourceTable.TableName, DestinationTableName );
            DataTable DestinationDataTable = CswTableUpdate.getEmptyTable();
            Int32 TotalRecordsToInsert = SourceTable.Rows.Count;
            Int32 TotalRecordsInsertedSoFar = 0;

            //_CswImportExportStatusReporter.reportProgress( "inserting " + TotalRecordsToInsert.ToString() + " into table " + DestinationTableName );
            Int32 TotalInsertsThisTransaction = 0;
            foreach( DataRow CurrentSourceRow in SourceTable.Rows )
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

                if( ( TotalInsertsThisTransaction >= _CswNbtImportOptions.MaxInsertRecordsPerTransaction ) || ( TotalRecordsInsertedSoFar >= TotalRecordsToInsert ) )
                {
                    CswTableUpdate.update( DestinationDataTable );
                    _CswNbtSchemaModTrnsctn.commitTransaction();
                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    TotalInsertsThisTransaction = 0;
                }

                if( 0 == ( TotalInsertsThisTransaction % _CswNbtImportOptions.MaxInsertRecordsPerDisplayUpdate ) )
                {
                    _CswImportExportStatusReporter.updateProcessPhase( _LastCompletedProcessPhase, TotalRecordsToInsert, TotalRecordsInsertedSoFar );
                }

            }//iterate source table rows


        }//_createTempRecords() 


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

    } // class CswImporterExperimental

} // namespace ChemSW.Nbt


//foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.FieldTypeRule.SubFields )
//{
//    CswNbtSubField.PropColumn.Field1

//    CurrentSubField.Column
//    CurrentNbtNode.Properties[CurrentNodeTypeProp].Field1 = "foo"; 
//}

//switch( CurrentNodeTypeProp.FieldType.FieldType )
//{
//    case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
//        string foo = string.Empty;
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Button:
//        foo = string.Empty;
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Link:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.List:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Location:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Memo:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Number:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Password:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.Text:
//        break;

//    case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
//        break;

//    default:
//        CurrentRowError += "Unhandled field type for property " + CurrentNodeTypePropname + ": " + CurrentNodeTypeProp.FieldType.FieldType.ToString();
//        break;

//}//deal with each field type