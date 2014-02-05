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
            string Sql = "select count(*) cnt from nbtimportqueue@" + CAFDbLink + " where state = '" + State.I + "' or state = '" + State.U + "'";
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
                    string Sql = "select nbtimportqueueid, state, itempk, pkcolumnname, sheetname, priority, importorder, tablename, coalesce(viewname, tablename) as sourcename, nodetypename from "
                        + QueueTableName + "@" + CAFDbLink + " iq"
                        + " join " + CswNbtImportTables.ImportDefOrder.TableName + " io on ( coalesce(viewname, tablename) = iq.sheetname )"
                        + " where state = '" + State.I + "' or state = '" + State.U
                        + "' order by decode (state, '" + State.I + "', 1, '" + State.U + "', 2) asc, priority desc, importorder asc, nbtimportqueueid asc";

                    CswArbitrarySelect QueueSelect = _CswNbtResources.makeCswArbitrarySelect( "cafimport_queue_select", Sql );
                    DataTable QueueTable = QueueSelect.getTable( 0, NumberToProcess, false );

                    CswNbtImporter Importer = new CswNbtImporter( _CswNbtResources.AccessId, CswEnumSetupMode.NbtExe );
                    foreach( DataRow QueueRow in QueueTable.Rows )
                    {
                        string CurrentTblNamePkCol = CswConvert.ToString( QueueRow["pkcolumnname"] );
                        if( string.IsNullOrEmpty( CurrentTblNamePkCol ) )
                        {
                            throw new Exception( "Could not find pkcolumn in data_dictionary for table " + QueueRow["tablename"] );
                        }

                        string ItemSql = string.Empty;
                        ItemSql = "select * from " + QueueRow["sourcename"] + "@" + CAFDbLink +
                                  " where " + CurrentTblNamePkCol + " = '" + QueueRow["itempk"] + "'";

                        CswArbitrarySelect ItemSelect = _CswNbtResources.makeCswArbitrarySelect( "cafimport_queue_select", ItemSql );
                        DataTable ItemTable = ItemSelect.getTable();
                        foreach( DataRow ItemRow in ItemTable.Rows )
                        {
                            string NodetypeName = QueueRow["nodetypename"].ToString();
                            bool Overwrite = QueueRow["state"].ToString().Equals( "U" );

                            string Error = Importer.ImportRow( ItemRow, DefinitionName, NodetypeName, Overwrite );
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

                    Importer.Finish();

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

        public static string generateCAFViewSQL()
        {
            return Nbt.Properties.Resources.caf;
        }



        public static string generateImportQueueTableSQL( ICswResources CswResources )
        {
            string Ret = string.Empty;

            CswArbitrarySelect ImportDefSelect = CswResources.makeCswArbitrarySelect( "importdef_get_caf_rows", "select distinct pkcolumnname, coalesce(viewname, tablename) as sourcename " +
                                                                                                                "from import_def_order io, import_def id " +
                                                                                                                "where id.importdefid = io.importdefid " +
                                                                                                                "and id.definitionname = '" + DefinitionName + "'" );
            DataTable ImportDefTable = ImportDefSelect.getTable();
            bool FirstRow = true;
            foreach( DataRow DefRow in ImportDefTable.Rows )
            {
                string CurrentDefRowSql = @"insert into nbtimportqueue(nbtimportqueueid, state, itempk, sheetname, priority, errorlog) "
                                          + " select seq_nbtimportqueueid.nextval, '"
                                          + State.I + "', "
                                          + CswConvert.ToString( DefRow[CswNbtImportTables.ImportDefOrder.pkcolumnname] ) + ", "
                                          + "'" + CswConvert.ToString( DefRow["sourcename"] ) + "', "
                                          + "0, "
                                          + "'' from " + CswConvert.ToString( DefRow["sourcename"] ) + " where deleted = '0';";
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
                       "       );";
            }

            return Ret;

            #endregion
        }

        public static string generateTriggerSQL( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            string Ret = "";

            CswArbitrarySelect ImportDefinitions = NbtResources.makeCswArbitrarySelect( "getCafImportDefTables",
                "select tablename, pkcolumnname, coalesce(viewname, tablename) as sourcename from import_def id "
                + "join import_def_order io on id.importdefid = io.importdefid "
                + "where definitionname = 'CAF'" );
            ;
            DataTable ImportDefinitionsTable = ImportDefinitions.getTable();

            //handle the special case of inventory levels
            DataRow maxInventoryRow = ImportDefinitionsTable.NewRow();
            maxInventoryRow["tablename"] = "maxinventory_basic";
            maxInventoryRow["sourcename"] = "inventory_view";
            maxInventoryRow["pkcolumnname"] = "inventorybasicid";
            ImportDefinitionsTable.Rows.Add( maxInventoryRow );


            DataRow GHSPictosRow = ImportDefinitionsTable.NewRow();
            GHSPictosRow["tablename"] = "jct_ghspictos_matsite";
            GHSPictosRow["sourcename"] = "ghs_view";
            GHSPictosRow["pkcolumnname"] = "materialid";
            ImportDefinitionsTable.Rows.Add(GHSPictosRow);


            DataRow GHSSignalRow = ImportDefinitionsTable.NewRow();
            GHSSignalRow["tablename"] = "jct_ghssignal_matsite";
            GHSSignalRow["sourcename"] = "ghs_view";
            GHSSignalRow["pkcolumnname"] = "materialid";
            ImportDefinitionsTable.Rows.Add(GHSSignalRow);



            foreach( DataRow Row in ImportDefinitionsTable.Rows )
            {
                //we cannot check a view that references the target table from within the trigger, so we need to set values for multiplexed tables and views
                //all of this information is derived from the create view statements in Nbt/Scripts/cafsql/CAF.sql
                string ItemSubquery = "";
                string TriggerName = Row["sourcename"].ToString();
                string TableName = Row["tablename"].ToString();
                string DeleteSubquery = "";
                switch( TableName )
                {
                    //max inventory and min inventory are in the same view, but actually require two separate triggers on different source tables
                    case "maxinventory_basic":
                        TriggerName = "max_inventory";
                        ItemSubquery = "select :new.maxinventorybasicid as primarykey from dual";
                        break;
                    case "mininventory_basic":
                        TriggerName = "min_inventory";
                        ItemSubquery = "select :new.mininventorybasicid as primarykey from dual";
                        break;

                    //GHS, synonyms, and documents use a manufactured legacy id which is stored in the import queue that we need to account for in the trigger
                    case "documents":
                        ItemSubquery = "select :new.documentid || '_' || :new.packageid as primarykey from dual where :new.packageid is not null UNION select :new.documentid || '_' || p.packageid as primarykey from packages p where :new.packageid is null and :new.materialid = p.materialid ";
                        break;
                    case "jct_ghsphrase_matsite":
                        DeleteSubquery = "select count(*) into deletecount from (select materialid, siteid from jct_ghspictos_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0 UNION select materialid, siteid from jct_ghssignal_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0);";
                        ItemSubquery = "select p.packageid || '_'|| s.region as primarykey from packages p full join sites s on 1=1 where p.materialid = :new.materialid and s.siteid = :new.siteid";
                        TriggerName = "jct_phrase";
                        break;
                    case "jct_ghspictos_matsite":
                        DeleteSubquery = "select count(*) into deletecount from (select materialid, siteid from jct_ghsphrase_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0 UNION select materialid, siteid from jct_ghssignal_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0);";
                        ItemSubquery = "select p.packageid || '_'|| s.region as primarykey from packages p full join sites s on 1=1 where p.materialid = :new.materialid and s.siteid = :new.siteid";
                        TriggerName = "jct_picto";
                        break;
                    case "jct_ghssignal_matsite":
                        DeleteSubquery = "select count(*) into deletecount from (select materialid, siteid from jct_ghspictos_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0 UNION select materialid, siteid from jct_ghsphrase_matsite where materialid=:new.materialid and siteid=:new.siteid and deleted=0);";
                        ItemSubquery = "select p.packageid || '_'|| s.region as primarykey from packages p full join sites s on 1=1 where p.materialid = :new.materialid and s.siteid = :new.siteid";
                        TriggerName = "jct_signal";
                        break;
                    case "materials_synonyms":
                        ItemSubquery = "select :new.materialsynonymid || '_' || packageid as primarykey from packages p where :new.materialid = p.materialid";
                        break;
                    default:
                        ItemSubquery = "select :new." + Row["pkcolumnname"] + " as primarykey from dual";
                        break;
                }

                string WhenClause = "";
                switch( Row["sourcename"].ToString() )
                {
                    case "docs_view":
                        WhenClause = "new.doctype = 'DOC'";
                        break;
                    case "sds_view":
                        WhenClause = "new.doctype = 'MSDS'";
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
                    case "containers_view":
                        WhenClause = "new.containerclass != 'lotholder'";
                        break;
                }



                Ret += "\r\n\r\n" +
                       "create or replace trigger " + TriggerName + "_trigger" + "\r\n" +
                       "  after insert or update on " + Row["tablename"] + "\r\n" +
                       "for each row" + "\r\n";

                if( false == String.IsNullOrEmpty( WhenClause ) ) { Ret += "  when (" + WhenClause + ")\r\n"; }

                // Case 31062
                Ret += @"declare 
    statestr varchar(1);
    deletecount integer := 0;
              begin
                          if inserting then
                            statestr := 'I';
                          elsif updating then
                            " + DeleteSubquery + @"
                            if :new.deleted = 1 and deletecount = 0 then
                              statestr := 'D';
                            else
                              statestr := 'U';
                            end if;
                          end if;

                          for queue_item in (" + ItemSubquery + @" ) loop

                          for x in (select count(*) cnt
                                      from dual
                                     where exists (select null
                                              from nbtimportqueue
                                             where state = statestr
                                               and itempk = queue_item.primarykey 
                                               and sheetname = '" + Row["sourcename"] + @"')) loop
                            if (x.cnt = 0) then
                              insert into nbtimportqueue
                                (nbtimportqueueid, state, itempk, sheetname, priority, errorlog)
                              values
                                (seq_nbtimportqueueid.nextval,
                                 statestr,
                                 queue_item.primarykey,
                                 '" + Row["sourcename"] + @"',
                                 0,
                                 '');
                            end if;
                          end loop;
                         end loop;
                        end;
                        /";
            }


            //what follows are special cases that they don't have the legacyid of their nbt targets (most of these are tables joined in views for a few columns we map to props)
            //to handle these, we trigger an empty update on relevant rows of the table joined to these, cascading the triggers back to a table that actually is in the import queue
            DataTable SupplementaryTriggers = new DataTable();
            SupplementaryTriggers.Columns.Add( new DataColumn( "triggerName" ) );
            SupplementaryTriggers.Columns.Add( new DataColumn( "triggerTable" ) );
            SupplementaryTriggers.Columns.Add( new DataColumn( "triggerColumn" ) );
            SupplementaryTriggers.Columns.Add( new DataColumn( "joinTable" ) );
            SupplementaryTriggers.Columns.Add( new DataColumn( "joinTableColumn" ) );

            SupplementaryTriggers.Rows.Add( new object[] { "material_import_trigger", "materials", "materialid", "packages", "materialid" } );
            SupplementaryTriggers.Rows.Add( new object[] { "mat_subclass_import_trigger", "materials_subclass", "materialsubclassid", "materials", "materialsubclassid"});
            SupplementaryTriggers.Rows.Add( new object[] { "hazdata_import_trigger", "cispro_hazdata", "materialid", "packages", "materialid"}  );
            SupplementaryTriggers.Rows.Add( new object[] { "dsdpictos_import_trigger", "jct_pictograms_materials", "materialid", "packages", "materialid"}  );
            SupplementaryTriggers.Rows.Add( new object[] { "dsdphrases_import_trigger", "jct_rsphrases_materials", "materialid", "packages", "materialid"}  );
            SupplementaryTriggers.Rows.Add( new object[] { "matprops_import_trigger", "properties_values", "materialid", "packages", "materialid"}  );
            SupplementaryTriggers.Rows.Add( new object[] { "storagecompat_import_trigger", "jct_graphics_materials", "materialid", "packages", "materialid"}  );


            foreach( DataRow Trigger in SupplementaryTriggers.Rows )
            {
                Ret += "\r\n\r\ncreate or replace trigger " + Trigger["triggerName"] + @"
                         after insert or update on " + Trigger["triggerTable"] + @" 
                       for each row
                       begin
                         update " + Trigger["joinTable"] + " set " + Trigger["joinTableColumn"] + " = " + Trigger["joinTableColumn"] + 
                                  " where " + Trigger["joinTableColumn"] + " = :new." + Trigger["triggerColumn"] + @";
                       end;
                        /";
            }
            return Ret;
        }

    }//CswScheduleLogicNbtCAFImpot

}//namespace ChemSW.Nbt.Sched
