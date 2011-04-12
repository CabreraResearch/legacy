using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-24
    /// </summary>
    public class CswUpdateSchemaTo01H26 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 26 ); } }
        public CswUpdateSchemaTo01H26( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {

            ///*******************  Schedule rule table entries             
            string TableName = "scheduledrules";
            string ColName_RuleName = "rulename";
            string ColName_MaxRunTimeMs = "maxruntimems";
            string ColName_ThreadId = "threadid";
            string ColName_ReprobateThreshold = "reprobatethreshold";
            string ColName_TotalRogueCount = "totalroguecount";
            string ColName_FailedCount = "failedcount";
            string ColName_Reprobate = "reprobate";
            string ColName_Disabled = "disabled";
            string ColName_StatusMessage = "statusmessage";
            string ColName_Recurrence = "recurrence";
            string ColName_Interval = "interval";
            string ColName_RunStartTime = "runstarttime";
            string ColName_RunEndTime = "runendtime";
            string ColName_LastRun = "lastrun";




            CswTableUpdate CswTableUpdateScheduledRules = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schemaupdate_" + SchemaVersion, TableName );
            DataTable DataTableScheduledRules = CswTableUpdateScheduledRules.getEmptyTable();


            //Update Prop Vals
            DataRow NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[ColName_RuleName] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtPropVals.ToString();
            NewRowScheduledRules[ColName_MaxRunTimeMs] = 2000;
            NewRowScheduledRules[ColName_ReprobateThreshold] = 3;
            NewRowScheduledRules[ColName_Disabled] = 0;
            NewRowScheduledRules[ColName_Recurrence] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[ColName_Interval] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Update MBTF
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[ColName_RuleName] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtMTBF.ToString();
            NewRowScheduledRules[ColName_MaxRunTimeMs] = 5000;
            NewRowScheduledRules[ColName_ReprobateThreshold] = 3;
            NewRowScheduledRules[ColName_Disabled] = 0;
            NewRowScheduledRules[ColName_Recurrence] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[ColName_Interval] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Update Inspection
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[ColName_RuleName] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtInspection.ToString();
            NewRowScheduledRules[ColName_MaxRunTimeMs] = 5000;
            NewRowScheduledRules[ColName_ReprobateThreshold] = 3;
            NewRowScheduledRules[ColName_Disabled] = 0;
            NewRowScheduledRules[ColName_Recurrence] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[ColName_Interval] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Gen Node
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[ColName_RuleName] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenNode.ToString();
            NewRowScheduledRules[ColName_MaxRunTimeMs] = 5000;
            NewRowScheduledRules[ColName_ReprobateThreshold] = 3;
            NewRowScheduledRules[ColName_Disabled] = 0;
            NewRowScheduledRules[ColName_Recurrence] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[ColName_Interval] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Gen Email Rpt
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[ColName_RuleName] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenEmailRpt.ToString();
            NewRowScheduledRules[ColName_MaxRunTimeMs] = 5000;
            NewRowScheduledRules[ColName_ReprobateThreshold] = 3;
            NewRowScheduledRules[ColName_Disabled] = 0;
            NewRowScheduledRules[ColName_Recurrence] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[ColName_Interval] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            CswTableUpdateScheduledRules.update( DataTableScheduledRules );




            ///******************  STATIC SQL SELECT UPDATES
            string TableNameStaticSqlSelects = "static_sql_selects";
            string ColName_QueryId = "queryid";
            string ColName_QueryText = "querytext";


            CswTableUpdate CswTableUpdateStaticSqlSelects = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schemaupdate_" + SchemaVersion + ": partitionrules", TableNameStaticSqlSelects );
            DataTable DataTableStaticSqlSelects = CswTableUpdateStaticSqlSelects.getEmptyTable();




            DataRow NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[ColName_QueryId] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenEmailRpt.ToString();
            NewRowStaticSqlSelects[ColName_QueryText] = @"select n.nodeid, t.nodetypename
                                                          from nodes n
                                                          join nodetypes t on n.nodetypeid = t.nodetypeid
                                                          join object_class o on t.objectclassid = o.objectclassid
                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1 enabled
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Enabled') e on (e.objectclassid =
                                                                                                     o.objectclassid and
                                                                                                     e.nodeid = n.nodeid and
                                                                                                     e.nodetypeid = t.nodetypeid)

                                                          left outer join (select op.objectclassid,
                                                                                  p.nodetypeid,
                                                                                  j.nodeid,
                                                                                  p.propname,
                                                                                  j.field1_date finalduedate
                                                                             from object_class_props op
                                                                             join nodetype_props p on op.objectclasspropid =
                                                                                                      p.objectclasspropid
                                                                             join jct_nodes_props j on j.nodetypepropid =
                                                                                                       p.nodetypepropid
                                                                            where op.propname = 'Final Due Date') fdd on (fdd.objectclassid =
                                                                                                                         o.objectclassid and
                                                                                                                         fdd.nodeid =
                                                                                                                         n.nodeid and
                                                                                                                         fdd.nodetypeid =
                                                                                                                         t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date nextduedate
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Next Due Date') ndd on (ndd.objectclassid =
                                                                                                             o.objectclassid and
                                                                                                             ndd.nodeid = n.nodeid and
                                                                                                             ndd.nodetypeid =
                                                                                                             t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_numeric warningdays
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Warning Days') wd on (wd.objectclassid =
                                                                                                           o.objectclassid and
                                                                                                           wd.nodeid = n.nodeid and
                                                                                                           wd.nodetypeid =
                                                                                                           t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date initialduedate,
                                                                       j.field1,
                                                                       j.field2,
                                                                       j.field3,
                                                                       j.field4,
                                                                       j.field5,
                                                                       j.field1_date
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Due Date Interval') ddi on (ddi.objectclassid =
                                                                                                                 o.objectclassid and
                                                                                                                 ddi.nodeid =
                                                                                                                 n.nodeid and
                                                                                                                 ddi.nodetypeid =
                                                                                                                 t.nodetypeid)

                                                         where ((o.objectclass = 'MailReportClass' and e.enabled = '1' and
                                                               (sysdate >= (ddi.initialduedate - wd.warningdays) and
                                                               sysdate >= (ndd.nextduedate - wd.warningdays)) and
                                                               (fdd.finalduedate is null or sysdate <= fdd.finalduedate)))";

            DataTableStaticSqlSelects.Rows.Add( DataTableStaticSqlSelects );


            NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[ColName_QueryId] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenNode.ToString();
            NewRowStaticSqlSelects[ColName_QueryText] = @"select n.nodeid,n.nodename
                                                          from nodes n
                                                          join nodetypes t on n.nodetypeid = t.nodetypeid
                                                          join object_class o on t.objectclassid = o.objectclassid

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1 enabled
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Enabled') e on (e.objectclassid =
                                                                                                     o.objectclassid and
                                                                                                     e.nodeid = n.nodeid and
                                                                                                     e.nodetypeid = t.nodetypeid)

                                                          left outer join (select op.objectclassid,
                                                                                  p.nodetypeid,
                                                                                  j.nodeid,
                                                                                  p.propname,
                                                                                  j.field1_date finalduedate
                                                                             from object_class_props op
                                                                             join nodetype_props p on op.objectclasspropid =
                                                                                                      p.objectclasspropid
                                                                             join jct_nodes_props j on j.nodetypepropid =
                                                                                                       p.nodetypepropid
                                                                            where op.propname = 'Final Due Date') fdd on (fdd.objectclassid =
                                                                                                                         o.objectclassid and
                                                                                                                         fdd.nodeid =
                                                                                                                         n.nodeid and
                                                                                                                         fdd.nodetypeid =
                                                                                                                         t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date nextduedate
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Next Due Date') ndd on (ndd.objectclassid =
                                                                                                             o.objectclassid and
                                                                                                             ndd.nodeid = n.nodeid and
                                                                                                             ndd.nodetypeid =
                                                                                                             t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_numeric warningdays
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Warning Days') wd on (wd.objectclassid =
                                                                                                           o.objectclassid and
                                                                                                           wd.nodeid = n.nodeid and
                                                                                                           wd.nodetypeid =
                                                                                                           t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date initialduedate,
                                                                       j.field1,
                                                                       j.field2,
                                                                       j.field3,
                                                                       j.field4,
                                                                       j.field5,
                                                                       j.field1_date
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname = 'Due Date Interval') ddi on (ddi.objectclassid =
                                                                                                                 o.objectclassid and
                                                                                                                 ddi.nodeid =
                                                                                                                 n.nodeid and
                                                                                                                 ddi.nodetypeid =
                                                                                                                 t.nodetypeid)

                                                         where ((o.objectclass = 'GeneratorClass' and e.enabled = '1' and
                                                               (sysdate >= (ddi.initialduedate - wd.warningdays) and
                                                               sysdate >= (ndd.nextduedate - wd.warningdays)) and
                                                               (fdd.finalduedate is null or sysdate <= fdd.finalduedate)))";

            DataTableStaticSqlSelects.Rows.Add( DataTableStaticSqlSelects );

            NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[ColName_QueryId] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtInspection.ToString();
            NewRowStaticSqlSelects[ColName_QueryText] = @"select n.nodeid
                                                          from nodes n
                                                          join nodetypes t on n.nodetypeid = t.nodetypeid
                                                          join object_class o on t.objectclassid = o.objectclassid

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1_date duedate,
                                                                       j.field1_numeric warningdays
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname like 'Due Date') dd on (dd.objectclassid =
                                                                                                          o.objectclassid and
                                                                                                          dd.nodeid = n.nodeid and
                                                                                                          dd.nodetypeid =
                                                                                                          t.nodetypeid)

                                                          join (select op.objectclassid,
                                                                       p.nodetypeid,
                                                                       j.nodeid,
                                                                       p.propname,
                                                                       j.field1 status
                                                                  from object_class_props op
                                                                  join nodetype_props p on op.objectclasspropid =
                                                                                           p.objectclasspropid
                                                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                                                 where op.propname like 'Status') s on (s.objectclassid =
                                                                                                       o.objectclassid and
                                                                                                       s.nodeid = n.nodeid and
                                                                                                       s.nodetypeid = t.nodetypeid)

                                                         where o.objectclass = 'InspectionDesignClass'
                                                           and sysdate >= (dd.duedate)
                                                           and s.status = 'Pending'";
            DataTableStaticSqlSelects.Rows.Add( DataTableStaticSqlSelects );

            CswTableUpdateStaticSqlSelects.update( DataTableStaticSqlSelects );



        } // update()

    }//class CswUpdateSchemaTo01H26

}//namespace ChemSW.Nbt.Schema

