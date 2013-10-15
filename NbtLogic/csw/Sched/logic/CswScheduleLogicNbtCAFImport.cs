using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ImportExport;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtCAFImport : ICswScheduleLogic
    {
        public const string CAFDbLink = "CAFLINK";
        public const string DefinitionName = "CAF";

        /// <summary>
        /// I: Insert
        /// U: Update
        /// D: Delete
        /// E: Error
        /// </summary>
        public enum State
        {
            I,
            U,
            D,
            E
        }

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.CAFImport ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        public Int32 getLoadCount( ICswResources CswResources )
        {
            // TODO: Does this SQL need to take State.U into consideration?
            string Sql = "select count(*) cnt from nbtimportqueue@" + CAFDbLink + " where state = '" + State.I + "'";
            CswArbitrarySelect QueueCountSelect = CswResources.makeCswArbitrarySelect( "cafimport_queue_count", Sql );
            DataTable QueueCountTable = QueueCountSelect.getTable();
            _CswScheduleLogicDetail.LoadCount = CswConvert.ToInt32( QueueCountTable.Rows[0]["cnt"] );
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
                try
                {
                    const string QueueTableName = "nbtimportqueue";
                    const string QueuePkName = "nbtimportqueueid";

                    Int32 NumberToProcess = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    string Sql = "select * from "
                        + QueueTableName + "@" + CAFDbLink + " iq"
                        + " join " + CswNbtImportTables.ImportDef.TableName + " id on (id.sheetname = iq.sheetname )"
                        + " where state = '" + State.I + "' or state = '" + State.U
                        + "' order by decode (state, '" + State.I + "', 1, '" + State.U + "', 2) asc, id.sheetorder asc, nbtimportqueueid asc";

                    CswArbitrarySelect QueueSelect = _CswNbtResources.makeCswArbitrarySelect( "cafimport_queue_select", Sql );
                    DataTable QueueTable = QueueSelect.getTable( 0, NumberToProcess, false, true );

                    CswNbtImporter Importer = new CswNbtImporter( _CswNbtResources );
                    foreach( DataRow QueueRow in QueueTable.Rows )
                    {
                        string CurrentTblNamePkCol = CswConvert.ToString( QueueRow["pkcolumnname"] );
                        if( string.IsNullOrEmpty( CurrentTblNamePkCol ) )
                        {
                            throw new Exception( "Could not find pkcolumn in data_dictionary for table " + QueueRow["tablename"] );
                        }

                        string ItemSql = string.Empty;
                        ItemSql = string.IsNullOrEmpty( QueueRow["viewname"].ToString() ) ? "select * from " + QueueRow["tablename"] + "@" + CAFDbLink +
                                                                                          " where " + CurrentTblNamePkCol + " = '" + QueueRow["itempk"] +
                                                                                          "'" : "select * from " + QueueRow["viewname"] + "@" + CAFDbLink +
                                                                                          " where " + CurrentTblNamePkCol + " = '" + QueueRow["itempk"] + "'";

                        CswArbitrarySelect ItemSelect = _CswNbtResources.makeCswArbitrarySelect( "cafimport_queue_select", ItemSql );
                        DataTable ItemTable = ItemSelect.getTable();
                        foreach( DataRow ItemRow in ItemTable.Rows )
                        {
                            string SheetName = QueueRow["sheetname"].ToString();
                            string Error = Importer.ImportRow( ItemRow, DefinitionName, SheetName, true );
                            if( string.IsNullOrEmpty( Error ) )
                            {
                                // record success - delete the record
                                _CswNbtResources.execArbitraryPlatformNeutralSql( "delete from " + QueueTableName + "@" + CAFDbLink +
                                                                                  " where " + QueuePkName + " = " + QueueRow[QueuePkName] );
                            }
                            else
                            {
                                // record failure - record the error on nbtimportqueue
                                _CswNbtResources.execArbitraryPlatformNeutralSql( "update " + QueueTableName + "@" + CAFDbLink +
                                                                                  "   set state = '" + State.E + "', " +
                                                                                  "       errorlog = '" + CswTools.SafeSqlParam( Error ) + "' " +
                                                                                  " where " + QueuePkName + " = " + QueueRow[QueuePkName] );
                            }
                        }
                    }//foreach( DataRow QueueRow in QueueTable.Rows )
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtCAFImport::ImportItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;

                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        public static string generateImportQueueTableSQL( ICswResources CswResources )
        {
            string Ret = string.Empty;

            CswTableSelect ImportDefSelect = CswResources.makeCswTableSelect( "importdef_get_caf_rows", CswNbtImportTables.ImportDef.TableName );
            DataTable ImportDefTable = ImportDefSelect.getTable( "where " + CswNbtImportTables.ImportDef.definitionname + " = '" + DefinitionName + "'" );
            bool FirstRow = true;
            foreach( DataRow DefRow in ImportDefTable.Rows )
            {
                string DataSource = CswConvert.ToString( string.IsNullOrEmpty( CswConvert.ToString( DefRow[CswNbtImportTables.ImportDef.viewname] ) ) ? DefRow[CswNbtImportTables.ImportDef.tablename] : DefRow[CswNbtImportTables.ImportDef.viewname] );
                string CurrentDefRowSql = @"insert into nbtimportqueue@" + CAFDbLink + " (nbtimportqueueid, state, itempk, sheetname, priority, errorlog) "
                                          + " select seq_nbtimportqueueid.nextval@" + CAFDbLink + ", '"
                                          + State.I + "', "
                                          + CswConvert.ToString( DefRow[CswNbtImportTables.ImportDef.pkcolumnname] ) + ", "
                                          + "'" + CswConvert.ToString( DefRow[CswNbtImportTables.ImportDef.sheetname] ) + "', "
                                          + "0, "
                                          + "'' from " + DataSource + "@" + CAFDbLink + " where deleted = '0';";
                CurrentDefRowSql = CurrentDefRowSql + " commit;";

                if( FirstRow )
                {
                    Ret = CurrentDefRowSql + Environment.NewLine;
                    FirstRow = false;
                }
                else
                {
                    Ret = Ret + Environment.NewLine + CurrentDefRowSql + Environment.NewLine;
                }
            }

            return Ret;
        }

        public static string generateTriggerSQL( ICswResources CswResources )
        {
            string Ret = string.Empty;

            // TODO: Finish this after Case 30787 is completed

            return Ret;
        }

    }//CswScheduleLogicNbtCAFImpot

}//namespace ChemSW.Nbt.Sched
