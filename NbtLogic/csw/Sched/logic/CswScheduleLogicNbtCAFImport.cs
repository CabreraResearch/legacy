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
            return CswConvert.ToInt32( QueueCountTable.Rows[0]["cnt"] ); ;
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
                    DataTable QueueTable = QueueSelect.getTable( 0, NumberToProcess, false );

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
                                // truncate error to 2000 chars
                                string SafeError = CswTools.SafeSqlParam( Error );
                                if( SafeError.Length > 2000 )
                                {
                                    SafeError = SafeError.Substring( 0, 2000 );
                                }
                                // record failure - record the error on nbtimportqueue
                                _CswNbtResources.execArbitraryPlatformNeutralSql( "update " + QueueTableName + "@" + CAFDbLink +
                                                                                  "   set state = '" + State.E + "', " +
                                                                                  "       errorlog = '" + SafeError + "' " +
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
                string CurrentDefRowSql = @"insert into nbtimportqueue(nbtimportqueueid, state, itempk, sheetname, priority, errorlog) "
                                          + " select seq_nbtimportqueueid.nextval, '"
                                          + State.I + "', "
                                          + CswConvert.ToString( DefRow[CswNbtImportTables.ImportDef.pkcolumnname] ) + ", "
                                          + "'" + CswConvert.ToString( DefRow[CswNbtImportTables.ImportDef.sheetname] ) + "', "
                                          + "0, "
                                          + "'' from " + DataSource + " where deleted = '0';";
                CurrentDefRowSql = CurrentDefRowSql + "\ncommit;";

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

        public static string generateCAFCleanupSQL( ICswResources CswResources )
        {
            string Ret = "";

            #region Locations

            for( int i = 1; i <= 5; i++ )
            {

                Ret += "\r\n\r\n" +
                       "update locations_level" + i + " set locationlevel" + i + "name = locationlevel" + i + "name || '_' || locationlevel" + i + "id " + "\r\n" +
                       "  where locationlevel" + i + "id in (" + "\r\n" +
                       "      select locationlevel" + i + "id from locations_level" + i + " where " + "\r\n" +
                       "          locationlevel" + i + "name in (" + "\r\n" +
                       "             select locationlevel" + i + "name from locations_level" + i + "\r\n" +
                       "                group by locationlevel" + i + "name " + "\r\n" +
                       "                   having count(*) > 1 " + "\r\n" +
                       "              )" + "\r\n" +
                       "       )";
            }

            return Ret;

            #endregion
        }

        public static string generateTriggerSQL( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            string Ret = "";

            CswTableSelect ImportDefinitions = NbtResources.makeCswTableSelect( "FetchImportDefinitions", "import_def" );
            CswCommaDelimitedString ProjectedColumns = new CswCommaDelimitedString { new[] { "tablename", "viewname", "pkcolumnname", "sheetname" } };
            DataTable ImportDefinitionsTable = ImportDefinitions.getTable( ProjectedColumns, "where definitionname = 'CAF'" );

            //handle the special case of inventory levels
            DataRow maxInventoryRow = ImportDefinitionsTable.NewRow();
            maxInventoryRow["tablename"] = "maxinventory_basic";
            maxInventoryRow["viewname"] = "inventory_view";
            maxInventoryRow["sheetname"] = "inventory_view";
            maxInventoryRow["pkcolumnname"] = "inventorybasicid";
            ImportDefinitionsTable.Rows.Add( maxInventoryRow );


            foreach( DataRow Row in ImportDefinitionsTable.Rows )
            {
                string TableOrView = string.IsNullOrEmpty( Row["viewname"].ToString() ) ? Row["tablename"].ToString() : Row["viewname"].ToString();

                //we cannot check a view that references the target table from within the trigger, so we need to set values for multiplexed tables and views
                //all of this information is derived from the create view statements in Nbt/Scripts/cafsql/CAF.sql
                string SourceColumn;
                string TriggerName;
                switch( Row["tablename"].ToString() )
                {
                    case "maxinventory_basic":
                        SourceColumn = "maxinventorybasicid";
                        TriggerName = "max_inventory";
                        break;
                    case "mininventory_basic":
                        SourceColumn = "mininventorybasicid";
                        TriggerName = "min_inventory";
                        break;
                    case "documents":
                        SourceColumn = "documentid";
                        TriggerName = Row["sheetname"].ToString();
                        break;
                    default:
                        SourceColumn = Row["pkcolumnname"].ToString();
                        TriggerName = Row["sheetname"].ToString();
                        break;
                }

                string WhenClause = "";
                switch( Row["sheetname"].ToString() )
                {
                    case "docs_view":
                        WhenClause = "new.packageid is not null and new.doctype = 'DOC'";
                        break;
                    case "sds_view":
                        WhenClause = "new.packageid is null and new.doctype = 'MSDS'";
                        break;
                    case "cofa_docs_view":
                        WhenClause = "new.CA_FileName is not null";
                        break;
                    case "each_view":
                        WhenClause = "lower(new.unittype)='each'";
                        break;
                    case "volume_view":
                        WhenClause = "lower(new.unittype)='volume'";
                        break;
                    case "weight_view":
                        WhenClause = "lower(new.unittype)='weight'";
                        break;
                }



                Ret += "\r\n\r\n" +
                       "create or replace trigger " + TriggerName + "_trigger" + "\r\n" +
                       "  after insert or update on " + Row["tablename"] + "\r\n" +
                       "for each row" + "\r\n";

                if( false == String.IsNullOrEmpty( WhenClause ) ) { Ret += "  when (" + WhenClause + ")\r\n"; }

                Ret += "begin" + "\r\n" +
                       "  if inserting then" + "\r\n" +
                       "    insert into nbtimportqueue(nbtimportqueueid, state, itempk, sheetname, priority, errorlog)" + "\r\n" +
                       "       values (seq_nbtimportqueueid.nextval, 'I', :new." + SourceColumn + ", '" + Row["sheetname"] + "', 0, '');" + "\r\n" +
                       "  elsif updating then" + "\r\n" +
                       "    if :new.deleted = 1 then" + "\r\n" +
                       "      insert into nbtimportqueue(nbtimportqueueid, state, itempk, sheetname, priority, errorlog)" + "\r\n" +
                       "         values (seq_nbtimportqueueid.nextval, 'D', :new." + SourceColumn + ", '" + Row["sheetname"] + "', 0, '');" + "\r\n" +
                       "    else" + "\r\n" +
                       "      insert into nbtimportqueue(nbtimportqueueid, state, itempk, sheetname, priority, errorlog)" + "\r\n" +
                       "         values (seq_nbtimportqueueid.nextval, 'U', :new." + SourceColumn + ", '" + Row["sheetname"] + "', 0, '');" + "\r\n" +
                       "    end if;" + "\r\n" +
                       "  end if;" + "\r\n" +
                       "end;\r\n/";


            }



            return Ret;
        }

    }//CswScheduleLogicNbtCAFImpot

}//namespace ChemSW.Nbt.Sched
