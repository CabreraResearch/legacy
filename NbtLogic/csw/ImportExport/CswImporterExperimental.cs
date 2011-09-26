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

    public class CswImporterExperimental : ICswImporter
    {

        CswNbtResources _CswNbtResources = null;
        CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;



        enum ProcessStati { Unprocessed, Imported, PropsAdded, Error };
        enum ProcessPhase { NothingDoneYet, TempTablesPopulated, NbtNodesPopulated, NbtPropsPouplated, RandomTestComplete };
        public CswImporterExperimental( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter )
        {
            _CswNbtResources = CswNbtResources;


            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
            _CswNbtSchemaModTrnsctn = new Schema.CswNbtSchemaModTrnsctn( _CswNbtResources );


        }



        private string _ColName_ProcessStatus = "processstatus";
        private string _ColName_StatusMessage = "statusmessage";
        private string _ColName_ImportNodeId = "importnodeid";
        private string _ColName_Props_ImportTargetNodeId = "importtargetnodeid";


        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="XmlStr">Source XML string</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, string XmlStr, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _CswImportExportStatusReporter.reportStatus( "Starting Import -- Experimental" );

            ErrorLog = string.Empty;

            //********** THIS TAKES ABOUT 5-10 MINUTES ON THE CABOT DATA :-( 
            //Dictionary<string, string> NodeTypePropNamesByNodeTypeNames = _CswNbtImportExportFrame.NodeTypes;


            //********** Step one: create nodes (not properties) and update XML doc with nodeids
            //foreach( XmlNode CurrentNode in _CswNbtImportExportFrame.Nodes )
            //{

            //    string CurrentNodeTypeNameInXml = CurrentNode.Attributes[CswNbtImportExportFrame._Attribute_NodeTypeName].InnerText;
            //    string CurrentNodeNameInXml = CurrentNode.Attributes[CswNbtImportExportFrame._Attribute_NodeName].InnerText;
            //    string CurrentNodeIdInXml = CurrentNode.Attributes[CswNbtImportExportFrame._Attribute_NodeId].InnerText;


            //}//iterate xml nodes



            //CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeNameInXml );
            //if( null != CurrentNodeType )
            //{
            //    CswNbtNode CswNbtNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
            //    _CswNbtImportExportFrame.replaceNodeIdReferenceValues( CurrentNodeIdInXml, CswNbtNode.NodeId.ToString() );
            //}
            //else
            //{
            //    _CswImportExportStatusReporter.reportError( "Unable to import node  " + CurrentNodeNameInXml + " because its nodetype (" + CurrentNodeTypeNameInXml + ") does not exist in the target schema" );
            //}//if-else current node's nodetype exists

            //*********************************************************************************************************
            //*********************** Load to dataset
            _CswImportExportStatusReporter.reportStatus( "Loading XML document to in memory tables" );
            DataSet DataSet = _CswNbtImportExportFrame.AsDataSet();



            //*********************************************************************************************************
            //*********************** Local variable definitions
            DataTable TableOfNodesFromXml = DataSet.Tables["Node"];
            TableOfNodesFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;


            DataTable TableOfPropsFromXml = DataSet.Tables["PropValue"];
            TableOfPropsFromXml.Columns["NodeID"].ColumnName = _ColName_Props_ImportTargetNodeId; //This is not a joke
            TableOfPropsFromXml.Columns["nodeid"].ColumnName = _ColName_ImportNodeId;

            string TblName_TempNodes = "tmp_import_nodes";
            string ColName_TempNodesTablePk = "tmpimportnodesid";
            string Colname_NbtNodeId = "nbtnodeid";

            string TblName_TempProps = "tmp_import_props";
            string ColName_TempPropsTablePk = "tmpimportpropsid";
            string ColName_TempPropsRealPropId = "nbtnodepropid";


            Collection<string> _AdditonalColumns = new Collection<string>();
            _AdditonalColumns.Add( _ColName_ProcessStatus );
            _AdditonalColumns.Add( _ColName_StatusMessage );



            Int32 MaxInsertRecordsPerTransaction = 500;
            Int32 MaxInsertRecordsPerDisplayUpdate = 1000;

            Int32 NodeCreatePageSize = 10; //number of nodes to create per cycle
            Int32 NodeAddPropsPageSize = 10; //Number of nodes to create properties for per cycle

            ProcessPhase CurrentProcessPhase = ProcessPhase.NothingDoneYet; //setting this manually for now during testing


            //*********************************************************************************************************
            //*********************** Create Temporary Tables
            if( ProcessPhase.NothingDoneYet == CurrentProcessPhase )
            {


                _CswImportExportStatusReporter.reportStatus( "Creating temporary tables in database" );
                _CswNbtSchemaModTrnsctn.beginTransaction();
                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_TempNodes ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_TempNodes ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( TblName_TempNodes );
                }

                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TblName_TempProps ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TblName_TempProps ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( TblName_TempProps );
                }

                _CswNbtSchemaModTrnsctn.commitTransaction();


                _CswNbtSchemaModTrnsctn.beginTransaction();

                _makeTempTable( TblName_TempNodes, ColName_TempNodesTablePk, TableOfNodesFromXml.Columns, 512, _AdditonalColumns );
                _CswNbtSchemaModTrnsctn.addLongColumn( TblName_TempNodes, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );



                _makeTempTable( TblName_TempProps, ColName_TempPropsTablePk, TableOfPropsFromXml.Columns, 512, _AdditonalColumns );
                //            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( TempPropsTableName, TempNodesTablePkColName, "refers to node records in " + TempNodesTableName + ";", false, false, TempNodesTableName, TempNodesTablePkColName );
                _CswNbtSchemaModTrnsctn.addLongColumn( TblName_TempProps, Colname_NbtNodeId, "to be filled in when the node is actually created", false, false );
                _CswNbtSchemaModTrnsctn.addLongColumn( TblName_TempProps, ColName_TempPropsRealPropId, "to be filled in when the node is actually created", false, false );



                _CswNbtSchemaModTrnsctn.commitTransaction();



                //*********************************************************************************************************
                //*********************** Fill Temporary tables
                _CswImportExportStatusReporter.reportStatus( "Filling temporary tables (this may take a while)" );
                _createTempRecords( TableOfNodesFromXml, TblName_TempNodes, MaxInsertRecordsPerTransaction, MaxInsertRecordsPerDisplayUpdate );
                _createTempRecords( TableOfPropsFromXml, TblName_TempProps, MaxInsertRecordsPerTransaction, MaxInsertRecordsPerDisplayUpdate );

                CurrentProcessPhase = ProcessPhase.TempTablesPopulated;

            }//if we haven't done anything yet


            if( ProcessPhase.TempTablesPopulated == CurrentProcessPhase )
            {
                //string WhereClauseForUnprocessedRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.Unprocessed.ToString() + "' and nodetypename <> 'User'";
                string WhereClauseForUnprocessedRecords = " where " + _ColName_ProcessStatus + "='" + ProcessStati.Unprocessed.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + TblName_TempNodes + WhereClauseForUnprocessedRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select nodename,nodetypename,nodeid from " + TblName_TempNodes + WhereClauseForUnprocessedRecords );
                CswTableUpdate CswTableUpdateTempNodesTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatenodesfornodeid", TblName_TempNodes );
                CswTableUpdate CswTableUpdateTempPropsTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatepropsfornodeid", TblName_TempProps );
                DataTable RawNodesTable = null;
                DataTable TempPropsUpdateTable = null;
                do
                {
                    if( null != RawNodesTable )
                    {
                        foreach( DataRow CurrentRow in RawNodesTable.Rows ) //will be empty on 1st interation
                        {
                            string CurrentNodeTypeNameInTempTable = CurrentRow["nodetypename"].ToString();
                            string CurrentNodeNameInTempTable = CurrentRow["nodename"].ToString();
                            string ImportNodeId = CurrentRow[_ColName_ImportNodeId].ToString();

                            DataTable TempNodesUpdateTable = CswTableUpdateTempNodesTable.getTable( "where " + _ColName_ImportNodeId + "='" + ImportNodeId + "'" );
                            DataRow TempNodesUpdateRow = TempNodesUpdateTable.Rows[0];

                            if( false == String.IsNullOrEmpty( CurrentNodeNameInTempTable ) )
                            {

                                if( Int32.MinValue == _doesNodeNameAlreadyExist( ImportNodeId ) )
                                {
                                    CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeNameInTempTable );
                                    if( null != CurrentNodeType )
                                    {
                                        try
                                        {
                                            CswNbtNode CswNbtNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                            CswNbtNode.NodeName = CurrentNodeNameInTempTable;
                                            CswNbtNode.postChanges( false );

                                            Int32 NbtNodeId = CswNbtNode.NodeId.PrimaryKey;


                                            TempNodesUpdateRow[Colname_NbtNodeId] = NbtNodeId;
                                            CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );

                                            // This visibly slows things down and is probably not necessary
                                            //TempPropsUpdateTable = CswTableUpdateTempPropsTable.getTable( " where nodeid = '" + CurrentNodeIdInTempTable + "'" );
                                            //foreach( DataRow CurrentPropsUpdateRow in TempPropsUpdateTable.Rows )
                                            //{
                                            //    CurrentPropsUpdateRow[TempNodesRealNodeIdColName] = RealNbtNodeId;
                                            //}

                                            TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Imported;


                                        }

                                        catch( Exception Exception )
                                        {
                                            TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Error;
                                            TempNodesUpdateRow[_ColName_StatusMessage] = Exception.Message;

                                            _CswImportExportStatusReporter.reportException( Exception );
                                        }

                                    }
                                    else
                                    {
                                        string Error = "Unable to import node  " + CurrentNodeNameInTempTable + " @ " + _ColName_ImportNodeId + " " + ImportNodeId + ",  because its nodetype (" + CurrentNodeTypeNameInTempTable + ") does not exist in the target schema";
                                        _CswImportExportStatusReporter.reportError( Error );
                                        TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Error;
                                        TempNodesUpdateRow[_ColName_StatusMessage] = Error;

                                    }//if-else current node's nodetype exists

                                }
                                else
                                {
                                    string Error = "Unable to import node " + CurrentNodeNameInTempTable + ",  because it already exists in the database";
                                    _CswImportExportStatusReporter.reportError( Error );
                                    TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Error;
                                    TempNodesUpdateRow[_ColName_StatusMessage] = Error;


                                }//if-else name is not already defined

                            }
                            else
                            {
                                string Error = "Unable to import node  with " + _ColName_ImportNodeId + " of " + ImportNodeId + ",  because its node name is empty";
                                TempNodesUpdateRow[_ColName_ProcessStatus] = ProcessStati.Error;
                                TempNodesUpdateRow[_ColName_StatusMessage] = Error;
                                _CswImportExportStatusReporter.reportError( Error );

                            }//if-else there is a name for the current node

                            CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );

                            //CswTableUpdateTempPropsTable.update( TempPropsUpdateTable ); <== prop updating visibly slows things down and is not necessary

                            TotalNodesProcesssedSoFar++;

                        }//iterate raw node rows


                        if( RawNodesTable.Rows.Count > 0 )
                        {
                            _CswNbtResources.finalize();
                            _CswNbtResources.clearUpdates();
                        }

                    }//if we're not on the first iteration

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    RawNodesTable = CswArbitrarySelectUnprocessedNodes.getTable( 0, NodeCreatePageSize, false, false );

                    _CswImportExportStatusReporter.reportStatus( TotalNodesProcesssedSoFar.ToString() + " of " + TotalNodesToProcess.ToString() + " nodes processed so far; processing the next " + RawNodesTable.Rows.Count.ToString() + " nodes." );


                } while( RawNodesTable.Rows.Count > 0 );

                CurrentProcessPhase = ProcessPhase.NbtNodesPopulated;
            }//if temptables have been populated



            if( ProcessPhase.NbtNodesPopulated == CurrentProcessPhase )
            {
                string WhereClauseForImportedNodeRecords = " where " + _ColName_ProcessStatus + "='" + ProcessStati.Imported.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + TblName_TempNodes + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select nodename,nodetypename,nodeid from " + TblName_TempNodes + WhereClauseForImportedNodeRecords );
                CswTableUpdate CswTableUpdateNodes = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatefornodepropupdatestatus", TblName_TempNodes );



                DataTable NodeRecordsToProcess = null;
                do
                {

                    if( NodeRecordsToProcess.Rows.Count > 0 )
                    {


                        foreach( DataRow CurrentImportNodeRow in NodeRecordsToProcess.Rows )
                        {
                            Int32 CurrentNbtNodeId = CswConvert.ToInt32( CurrentImportNodeRow[Colname_NbtNodeId] );
                            CswPrimaryKey CurrentNbtPrimeKey = new CswPrimaryKey( string.Empty, CurrentNbtNodeId );
                            CswNbtNode CurrentNbtNode = _CswNbtResources.Nodes[CurrentNbtPrimeKey];

                            string CurrentImportNodeId = CurrentImportNodeRow[_ColName_ImportNodeId].ToString();
                            CswArbitrarySelect CswArbitrarySelectUnProcessedProps = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssedprops", "select n.importnodeid, n.nodename,p.* from tmp_import_nodes n join tmp_import_props p on (n.importnodeid=p.importnodeid) where  n.importnodeid='" + CurrentImportNodeId + "'" );
                            DataTable CurrentUnprocssedPropsTable = CswArbitrarySelectUnProcessedProps.getTable();
                            string CurrentRowError = string.Empty;

                            //Luke, close your eyes and let The Force guide you now . . . .

                            foreach( DataRow CurrentImportPropRow in CurrentUnprocssedPropsTable.Rows )
                            {
                                string CurrentNodeTypeName = CurrentImportNodeRow["nodetypename"].ToString();
                                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeName );
                                if( null != NodeType )
                                {
                                }
                                else
                                {
                                    CurrentRowError += "Unable to import nodeprop with " + _ColName_ImportNodeId + " of " + CurrentImportNodeId + ",  its node type prop (" + CurrentNodeTypeName + ") could not be retrieved; ";
                                }//if-else we were able to retrieve the node type
                            }


                            DataTable DataTableCurrentNodeTable = CswTableUpdateNodes.getTable( " where " + _ColName_ImportNodeId + "='" + CurrentImportNodeId + "'" );
                            if( string.Empty != CurrentRowError )
                            {
                                DataTableCurrentNodeTable.Rows[1][_ColName_ProcessStatus] = ProcessStati.PropsAdded.ToString();
                            }
                            else
                            {
                                DataTableCurrentNodeTable.Rows[1][_ColName_ProcessStatus] = ProcessStati.Error.ToString();
                                DataTableCurrentNodeTable.Rows[1][_ColName_StatusMessage] = CurrentRowError;
                                _CswImportExportStatusReporter.reportError( CurrentRowError );
                            }//if else there was an error on the current row

                        }//iterate rows of node records to process

                        _CswNbtResources.finalize();
                        _CswNbtResources.clearUpdates();


                    }//if we are not on the first iteration

                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    NodeRecordsToProcess = CswArbitrarySelectUnprocessedNodes.getTable( 0, NodeAddPropsPageSize, false, false );

                } while( NodeRecordsToProcess.Rows.Count > 0 );


            }//if nodes have been populated

            if( ProcessPhase.NbtPropsPouplated == CurrentProcessPhase )
            {
                /* 
                 * This will be the phase in which we do random testing of the data in NBT against the temp tables (and possibly even against the import XML file)
                 */

            }//if props have been populated



        } // ImportXml()

        private void _makeTempTable( string TableName, string PkColumnName, DataColumnCollection Columns, Int32 ArbitraryStringColumnLength, Collection<string> AdditionalStringColumns )
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

        }//_makeTempTable() 


        private void _createTempRecords( DataTable SourceTable, string DestinationTableName, Int32 MaxInsertRecordsPerTransaction, Int32 MaxInsertRecordsPerDisplayUpdate )
        {
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "insertimportrecordsfortable_" + SourceTable.TableName, DestinationTableName );
            DataTable DestinationDataTable = CswTableUpdate.getEmptyTable();
            Int32 TotalRecordsToInsert = SourceTable.Rows.Count;
            Int32 TotalRecordsInsertedSoFar = 0;

            _CswImportExportStatusReporter.reportStatus( "inserting " + TotalRecordsToInsert.ToString() + " into table " + DestinationTableName );
            Int32 TotalInsertsThisTransaction = 0;
            foreach( DataRow CurrentSourceRow in SourceTable.Rows )
            {
                TotalInsertsThisTransaction++;
                TotalRecordsInsertedSoFar++;
                DataRow NewRow = DestinationDataTable.NewRow();
                DestinationDataTable.Rows.Add( NewRow );


                NewRow[_ColName_ProcessStatus] = ProcessStati.Unprocessed.ToString();

                foreach( DataColumn CurrentColum in SourceTable.Columns )
                {
                    if( DestinationDataTable.Columns.Contains( CurrentColum.ColumnName ) )
                    {
                        NewRow[CurrentColum.ColumnName] = CurrentSourceRow[CurrentColum.ColumnName].ToString();
                    }

                }//iterate source table columns

                if( ( TotalInsertsThisTransaction >= MaxInsertRecordsPerTransaction ) || ( TotalRecordsInsertedSoFar >= TotalRecordsToInsert ) )
                {
                    CswTableUpdate.update( DestinationDataTable );
                    _CswNbtSchemaModTrnsctn.commitTransaction();
                    _CswNbtSchemaModTrnsctn.beginTransaction();
                    TotalInsertsThisTransaction = 0;
                }

                if( 0 == ( TotalInsertsThisTransaction % MaxInsertRecordsPerDisplayUpdate ) )
                {
                    _CswImportExportStatusReporter.reportStatus( TotalRecordsInsertedSoFar.ToString() + " of " + TotalRecordsToInsert.ToString() + " temporary records have been inserted into " + DestinationDataTable );
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
