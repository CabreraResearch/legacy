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



        private string _ProcessStatusColumnName = "processstatus";
        private string _StatusMessageColumnName = "statusmessage";


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
            DataTable TableOfPropsFromXml = DataSet.Tables["PropValue"];

            string TempNodesTableName = "tmp_import_nodes";
            string TempNodesTablePkColName = "tmpimportnodesid";
            string TempNodesRealNodeIdColName = "nbtnodeid";

            string TempPropsTableName = "tmp_import_props";
            string TempPropsTablePkColName = "tmpimportpropsid";
            string TempPropsRealPropIdColName = "nodepropid";


            Collection<string> _AdditonalColumns = new Collection<string>();
            _AdditonalColumns.Add( _ProcessStatusColumnName );
            _AdditonalColumns.Add( _StatusMessageColumnName );



            Int32 MaxInsertRecordsPerTransaction = 500;
            Int32 MaxInsertRecordsPerDisplayUpdate = 1000;

            Int32 NodeCreatePageSize = 10; //number of nodes to create per cycle
            Int32 NodeAddPropsPageSize = 10; //Number of nodes to create properties for per cycle

            ProcessPhase CurrentProcessPhase = ProcessPhase.TempTablesPopulated; //setting this manually for now during testing


            //*********************************************************************************************************
            //*********************** Create Temporary Tables
            if( ProcessPhase.NothingDoneYet == CurrentProcessPhase )
            {


                _CswImportExportStatusReporter.reportStatus( "Creating temporary tables in database" );
                _CswNbtSchemaModTrnsctn.beginTransaction();
                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TempNodesTableName ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TempNodesTableName ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( TempNodesTableName );
                }

                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TempPropsTableName ) || _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TempPropsTableName ) ) //belt and suspenders
                {
                    _CswNbtSchemaModTrnsctn.dropTable( TempPropsTableName );
                }

                _CswNbtSchemaModTrnsctn.commitTransaction();


                _CswNbtSchemaModTrnsctn.beginTransaction();

                _makeTempTable( TempNodesTableName, TempNodesTablePkColName, TableOfNodesFromXml.Columns, 512, _AdditonalColumns );
                _CswNbtSchemaModTrnsctn.addLongColumn( TempNodesTableName, TempNodesRealNodeIdColName, "to be filled in when the node is actually created", false, false );



                _makeTempTable( TempPropsTableName, TempPropsTablePkColName, TableOfPropsFromXml.Columns, 512, _AdditonalColumns );
                //            _CswNbtSchemaModTrnsctn.addForeignKeyColumn( TempPropsTableName, TempNodesTablePkColName, "refers to node records in " + TempNodesTableName + ";", false, false, TempNodesTableName, TempNodesTablePkColName );
                _CswNbtSchemaModTrnsctn.addLongColumn( TempPropsTableName, TempPropsRealPropIdColName, "to be filled in when the node is actually created", false, false );



                _CswNbtSchemaModTrnsctn.commitTransaction();



                //*********************************************************************************************************
                //*********************** Fill Temporary tables
                _CswImportExportStatusReporter.reportStatus( "Filling temporary tables (this may take a while)" );
                _createTempRecords( TableOfNodesFromXml, TempNodesTableName, MaxInsertRecordsPerTransaction, MaxInsertRecordsPerDisplayUpdate );
                _createTempRecords( TableOfPropsFromXml, TempPropsTableName, MaxInsertRecordsPerTransaction, MaxInsertRecordsPerDisplayUpdate );

                CurrentProcessPhase = ProcessPhase.TempTablesPopulated;

            }//if we haven't done anything yet


            if( ProcessPhase.TempTablesPopulated == CurrentProcessPhase )
            {
                //string WhereClauseForUnprocessedRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.Unprocessed.ToString() + "' and nodetypename <> 'User'";
                string WhereClauseForUnprocessedRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.Unprocessed.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + TempNodesTableName + WhereClauseForUnprocessedRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select nodename,nodetypename,nodeid from " + TempNodesTableName + WhereClauseForUnprocessedRecords );
                CswTableUpdate CswTableUpdateTempNodesTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatenodesfornodeid", TempNodesTableName );
                CswTableUpdate CswTableUpdateTempPropsTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatepropsfornodeid", TempPropsTableName );
                DataTable RawNodesTable = null;
                do
                {
                    if( null != RawNodesTable )
                    {
                        foreach( DataRow CurrentRow in RawNodesTable.Rows ) //will be empty on 1st interation
                        {
                            string CurrentNodeTypeNameInTempTable = CurrentRow["nodetypename"].ToString();
                            string CurrentNodeNameInTempTable = CurrentRow["nodename"].ToString();
                            string CurrentNodeIdInTempTable = CurrentRow["nodeid"].ToString();

                            DataTable TempNodesUpdateTable = CswTableUpdateTempNodesTable.getTable( "where nodeid='" + CurrentNodeIdInTempTable + "'" );
                            DataRow TempNodesUpdateRow = TempNodesUpdateTable.Rows[0];

                            if( false == String.IsNullOrEmpty( CurrentNodeNameInTempTable ) )
                            {

                                if( Int32.MinValue == _doesNodeNameAlreadyExist( CurrentNodeIdInTempTable ) )
                                {
                                    CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeNameInTempTable );
                                    if( null != CurrentNodeType )
                                    {
                                        try
                                        {
                                            CswNbtNode CswNbtNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
                                            CswNbtNode.NodeName = CurrentNodeNameInTempTable;
                                            CswNbtNode.postChanges( false );

                                            Int32 RealNbtNodeId = CswNbtNode.NodeId.PrimaryKey;


                                            TempNodesUpdateRow[TempNodesRealNodeIdColName] = RealNbtNodeId;
                                            CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );

                                            DataTable TempPropsUpdateTable = CswTableUpdateTempPropsTable.getTable( " where nodeid = '" + CurrentNodeIdInTempTable + "'" );
                                            foreach( DataRow CurrentPropsUpdateRow in TempPropsUpdateTable.Rows )
                                            {
                                                CurrentPropsUpdateRow[TempPropsRealPropIdColName] = RealNbtNodeId;
                                            }

                                            TempNodesUpdateRow[_ProcessStatusColumnName] = ProcessStati.Imported;


                                        }

                                        catch( Exception Exception )
                                        {
                                            TempNodesUpdateRow[_ProcessStatusColumnName] = ProcessStati.Error;
                                            TempNodesUpdateRow[_StatusMessageColumnName] = Exception.Message;

                                            _CswImportExportStatusReporter.reportException( Exception );
                                        }

                                    }
                                    else
                                    {
                                        string Error = "Unable to import node  " + CurrentNodeNameInTempTable + " @ nodeid " + CurrentNodeIdInTempTable + ",  because its nodetype (" + CurrentNodeTypeNameInTempTable + ") does not exist in the target schema";
                                        _CswImportExportStatusReporter.reportError( Error );
                                        TempNodesUpdateRow[_ProcessStatusColumnName] = ProcessStati.Error;
                                        TempNodesUpdateRow[_StatusMessageColumnName] = Error;

                                    }//if-else current node's nodetype exists

                                }
                                else
                                {
                                    string Error = "Unable to import node " + CurrentNodeNameInTempTable + ",  because it already exists in the database";
                                    _CswImportExportStatusReporter.reportError( Error );
                                    TempNodesUpdateRow[_ProcessStatusColumnName] = ProcessStati.Error;
                                    TempNodesUpdateRow[_StatusMessageColumnName] = Error;


                                }//if-else name is not already defined

                            }
                            else
                            {
                                string Error = "Unable to import node  with nodeid " + CurrentNodeIdInTempTable + ",  because its node name is empty";
                                TempNodesUpdateRow[_ProcessStatusColumnName] = ProcessStati.Error;
                                TempNodesUpdateRow[_StatusMessageColumnName] = Error;
                                _CswImportExportStatusReporter.reportError( Error );

                            }//if-else there is a name for the current node

                            CswTableUpdateTempNodesTable.update( TempNodesUpdateTable );
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
                string WhereClauseForImportedNodeRecords = " where " + _ProcessStatusColumnName + "='" + ProcessStati.PropsAdded.ToString() + "'";
                CswArbitrarySelect CswArbitrarySelectCountOfUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select count(*) from " + TempNodesTableName + WhereClauseForImportedNodeRecords );
                Int32 TotalNodesToProcess = Convert.ToInt32( CswArbitrarySelectCountOfUnprocessedNodes.getTable().Rows[0][0] );
                Int32 TotalNodesProcesssedSoFar = 0;
                CswArbitrarySelect CswArbitrarySelectUnprocessedNodes = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "selectunprocssednodes", "select nodename,nodetypename,nodeid from " + TempNodesTableName + WhereClauseForImportedNodeRecords );

                DataTable NodeRecordsToProcess = null;
                do
                {

                    List<CswPrimaryKey> NodeKeysToProcess = new List<CswPrimaryKey>();

                    if( NodeKeysToProcess.Count > 0 )
                    {

                        if( NodeKeysToProcess.Count > 0 )
                        {
                            _CswNbtResources.finalize();
                            _CswNbtResources.clearUpdates();
                        }


                    }//if we are not on the first iteration

                    _CswNbtSchemaModTrnsctn.beginTransaction();

                    NodeRecordsToProcess = CswArbitrarySelectUnprocessedNodes.getTable(); 

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
                string ColumnName = CurrentColumn.ColumnName;
                if( _CswNbtSchemaModTrnsctn.isColumnDefined( TableName, ColumnName ) )
                {
                    ColumnName += "_";
                }

                _CswNbtSchemaModTrnsctn.addStringColumn( TableName, ColumnName, string.Empty, false, false, ArbitraryStringColumnLength );
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


                NewRow[_ProcessStatusColumnName] = ProcessStati.Unprocessed.ToString();

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
