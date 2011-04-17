using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
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
            string TableNameScheduledRules = "scheduledrules";
            CswTableUpdate CswTableUpdateScheduledRules = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schemaupdate_" + SchemaVersion, TableNameScheduledRules );
            DataTable DataTableScheduledRules = CswTableUpdateScheduledRules.getEmptyTable();



            //Update Prop Vals
            DataRow NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtPropVals.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 2000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '0';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            Int32 PropValRulePk = Convert.ToInt32( NewRowScheduledRules[NbtScheduledRuleColumns.ScheduledRuleId.ToString()] );


            //Update MBTF
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtMTBF.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 20000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '0';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Update Inspection
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtInspection.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 20000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '0';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Gen Node
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenNode.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 20000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '0';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            //Gen Email Rpt
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenEmailRpt.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 20000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '0';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            CswTableUpdateScheduledRules.update( DataTableScheduledRules );

            //BaseSleepNSeconds
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.MtSched.Core.BaseScheduleRuleNames.BaseSleepNSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 20000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '1';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );

            Int32 BaseSleepNSeconcsRulePk = Convert.ToInt32( NewRowScheduledRules[NbtScheduledRuleColumns.ScheduledRuleId.ToString()] );


            //CswScheduleLogicThrowRogueException
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.MtSched.Core.BaseScheduleRuleNames.ThrowRogueException.ToString(); 
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 3000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '1';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );


            //CswScheduleLogicSelfFailed
            NewRowScheduledRules = DataTableScheduledRules.NewRow();
            NewRowScheduledRules[NbtScheduledRuleColumns.RuleName.ToString()] = ChemSW.MtSched.Core.BaseScheduleRuleNames.SelfFailed.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.MaxRunTimeMs.ToString()] = 3000;
            NewRowScheduledRules[NbtScheduledRuleColumns.ReprobateThreshold.ToString()] = 3;
            NewRowScheduledRules[NbtScheduledRuleColumns.Disabled.ToString()] = '1';
            NewRowScheduledRules[NbtScheduledRuleColumns.Recurrence.ToString()] = ChemSW.MtSched.Core.Recurrance.NSeconds.ToString();
            NewRowScheduledRules[NbtScheduledRuleColumns.Interval.ToString()] = 5;
            DataTableScheduledRules.Rows.Add( NewRowScheduledRules );


            CswTableUpdateScheduledRules.update( DataTableScheduledRules );

            //Params table
            string TableNameScheduledRuleParams = "ScheduledRuleParams";
            CswTableUpdate CswTableUpdateScheduledRuleParams = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schemaupdate_" + SchemaVersion, TableNameScheduledRuleParams );
            DataTable DataTableScheduledRuleParams = CswTableUpdateScheduledRuleParams.getEmptyTable();

            //Params: PropVals
            DataRow NewParamRow = DataTableScheduledRuleParams.NewRow();
            NewParamRow[NbtScheduledRuleParamsColumns.ScheduledRuleParamId.ToString()] = PropValRulePk;
            NewParamRow[NbtScheduledRuleParamsColumns.ParamName.ToString()] = "ProcessChunkSize";
            NewParamRow[NbtScheduledRuleParamsColumns.ParamVal.ToString()] = "5";
            DataTableScheduledRuleParams.Rows.Add( NewParamRow );

            //Params: Base SleepNSeconds
            NewParamRow = DataTableScheduledRuleParams.NewRow();
            NewParamRow[NbtScheduledRuleColumns.ScheduledRuleId.ToString()] = BaseSleepNSeconcsRulePk;
            NewParamRow[NbtScheduledRuleParamsColumns.ParamName.ToString()] = "sleepsecs_even";
            NewParamRow[NbtScheduledRuleParamsColumns.ParamVal.ToString()] = "5";
            DataTableScheduledRuleParams.Rows.Add( NewParamRow );

            NewParamRow = DataTableScheduledRuleParams.NewRow();
            NewParamRow[NbtScheduledRuleColumns.ScheduledRuleId.ToString()] = BaseSleepNSeconcsRulePk;
            NewParamRow[NbtScheduledRuleParamsColumns.ParamName.ToString()] = "sleepsecs_odd";
            NewParamRow[NbtScheduledRuleParamsColumns.ParamVal.ToString()] = "15";
            DataTableScheduledRuleParams.Rows.Add( NewParamRow );



            CswTableUpdateScheduledRuleParams.update( DataTableScheduledRuleParams );



            ///******************  STATIC SQL SELECT UPDATES
            string TableNameStaticSqlSelects = "static_sql_selects";
            string QueryId = "queryid";
            string QueryText = "querytext";

            CswTableUpdate CswTableUpdateStaticSqlSelects = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "schemaupdate_" + SchemaVersion + ": partitionrules", TableNameStaticSqlSelects );
            DataTable DataTableStaticSqlSelects = CswTableUpdateStaticSqlSelects.getEmptyTable();

            DataRow NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[QueryId.ToString()] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenEmailRpt.ToString();
            NewRowStaticSqlSelects[QueryText] = @"select n.nodeid, t.nodetypename
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

            DataTableStaticSqlSelects.Rows.Add( NewRowStaticSqlSelects );


            NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[QueryId] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.GenNode.ToString();
            NewRowStaticSqlSelects[QueryText] = @"select n.nodeid,n.nodename
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

            DataTableStaticSqlSelects.Rows.Add( NewRowStaticSqlSelects );

            NewRowStaticSqlSelects = DataTableStaticSqlSelects.NewRow();
            NewRowStaticSqlSelects[QueryId] = ChemSW.Nbt.Sched.NbtScheduleRuleNames.UpdtInspection.ToString();
            NewRowStaticSqlSelects[QueryText] = @"select n.nodeid
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
            DataTableStaticSqlSelects.Rows.Add( NewRowStaticSqlSelects );

            CswTableUpdateStaticSqlSelects.update( DataTableStaticSqlSelects );



        } // update()

    }//class CswUpdateSchemaTo01H26

}//namespace ChemSW.Nbt.Schema

