using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26550
    /// </summary>
    public class CswUpdateSchemaCase26550 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region Remove Existing Deficient_Inspections Action

            _CswNbtSchemaModTrnsctn.deleteView( "Deficient Inspections", true );

            string actionWhereClause = "where actionid = 100";
            CswTableUpdate tableJctModActUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DeleteDefInspActJctMod", "jct_modules_actions" );
            DataTable jctModActTable = tableJctModActUpdate.getTable( actionWhereClause );
            foreach( DataRow row in jctModActTable.Rows )
            {
                row.Delete();
            }
            tableJctModActUpdate.update( jctModActTable );

            CswTableUpdate tableActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DeleteDefInspAct", "actions" );
            DataTable actTable = tableActionUpdate.getTable( actionWhereClause );
            foreach( DataRow row in actTable.Rows )
            {
                row.Delete();
            }
            tableActionUpdate.update( actTable );

            #endregion

            #region Create new Deficient Inspections Report

            CswNbtMetaDataNodeType ReportNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
            CswNbtNode Report = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassReport NodeAsReport = Report;
            NodeAsReport.ReportName.Text = "Deficient Inspections";
            NodeAsReport.Category.Text = "Lab Safety";
            NodeAsReport.SQL.Text =
                @"select t.nodetypename inspection_type,
                        u.OP_77 || ' ' || u.OP_78 inspector,
                        l.op_108 || ' > ' || l.op_1249 inspection_location,
                        p.op_1244 inspection_point,
                        i.op_295 inspection_date,
                        i.op_1233 inspection_status,
                        x.propname deficient_question, 
                        x.field1 deficient_answer,
                        x.field1_date date_answered,
                        x.clobdata comments
                    from ocinspectiondesignclass i
                    join nodes n on i.nodeid = n.nodeid
                    join nodetypes t on n.nodetypeid = t.nodetypeid
                    left outer join ocinspectiontargetclass p on (i.OP_292_FK = p.nodeid)
                    left outer join ocuserclass u on (i.OP_1372_FK = u.nodeid)
                    left outer join oclocationclass l on (p.OP_1239_FK = l.nodeid)
                    left outer join (select t.nodetypename, n.nodeid, p.propname, j.field1, j.field1_date, j.clobdata
                        from nodes n
                        join nodetypes t on t.nodetypeid = n.nodetypeid
                        join object_class oc on t.objectclassid = oc.objectclassid
                        join nodetype_props p on t.nodetypeid = p.nodetypeid
                        join field_types f on p.fieldtypeid = f.fieldtypeid and f.fieldtype = 'Question'
                        join jct_nodes_props j on n.nodeid = j.nodeid and j.nodetypepropid = p.nodetypepropid
                        where j.field3 = '0') x on x.nodeid = i.nodeid
                    where i.op_1233 = 'Action Required'
                    order by x.field1_date";

            NodeAsReport.postChanges( false );

            #endregion

        }//Update()

    }//class CswUpdateSchemaCase26550

}//namespace ChemSW.Nbt.Schema