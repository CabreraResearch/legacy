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
            _removeExistingDeficientInspectionsAction();
            _createNewDeficientInspectionsReport();
        }//Update()

        private void _removeExistingDeficientInspectionsAction()
        {
            _CswNbtSchemaModTrnsctn.deleteView( "Deficient Inspections", true );

            string actionId = _getDeficientInspectionsActionIdFromDB();
            _deleteDeficientInspectionActionFromTable( actionId, "jct_modules_actions" );
            _deleteDeficientInspectionActionFromTable( actionId, "actions" );
        }

        private string _getDeficientInspectionsActionIdFromDB()
        {
            string actionId = string.Empty;
            CswTableSelect tableActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "DeleteDefInspAct", "actions" );
            DataTable actTable = tableActionUpdate.getTable( "where actionname = 'Deficient Inspections'" );
            foreach( DataRow row in actTable.Rows )
            {
                actionId = row["actionid"].ToString();
            }
            return actionId;
        }

        private void _deleteDeficientInspectionActionFromTable( string actionId, string tableName )
        {
            CswTableUpdate tableActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DeleteDeficientInspectionsAction", tableName );
            DataTable actTable = tableActionUpdate.getTable( "where actionid = " + actionId );
            foreach( DataRow row in actTable.Rows )
            {
                row.Delete();
            }
            tableActionUpdate.update( actTable );
        }

        private void _createNewDeficientInspectionsReport()
        {
            CswNbtMetaDataNodeType ReportNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
            CswNbtNode ReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassReport NodeAsReport = ReportNode;
            NodeAsReport.ReportName.Text = "Deficient Inspections";
            NodeAsReport.Category.Text = "Lab Safety";
            NodeAsReport.SQL.Text = _getDeficientInspectionsReportSQLText();
            NodeAsReport.postChanges( false );
        }

        private string _getDeficientInspectionsReportSQLText()
        {
            string reportSqlText = string.Empty;

            #region SQL Query Template
            string sqlText = @"select t.nodetypename inspection_type,
                        u.OP_{0} || ' ' || u.OP_{1} inspector,
                        l.op_{2} || ' > ' || l.op_{3} inspection_location,
                        p.op_{4} inspection_point,
                        i.op_{5} inspection_date,
                        i.op_{6} inspection_status,
                        x.propname deficient_question, 
                        x.field1 deficient_answer,
                        x.field1_date date_answered,
                        x.clobdata comments
                    from ocinspectiondesignclass i
                    join nodes n on i.nodeid = n.nodeid
                    join nodetypes t on n.nodetypeid = t.nodetypeid
                    left outer join ocinspectiontargetclass p on (i.OP_{7}_FK = p.nodeid)
                    left outer join ocuserclass u on (i.OP_{8}_FK = u.nodeid)
                    left outer join oclocationclass l on (p.OP_{9}_FK = l.nodeid)
                    left outer join (select t.nodetypename, n.nodeid, p.propname, j.field1, j.field1_date, j.clobdata
                        from nodes n
                        join nodetypes t on t.nodetypeid = n.nodetypeid
                        join object_class oc on t.objectclassid = oc.objectclassid
                        join nodetype_props p on t.nodetypeid = p.nodetypeid
                        join field_types f on p.fieldtypeid = f.fieldtypeid and f.fieldtype = 'Question'
                        join jct_nodes_props j on n.nodeid = j.nodeid and j.nodetypepropid = p.nodetypepropid
                        where j.field3 = '0') x on x.nodeid = i.nodeid
                    where i.op_{6} = 'Action Required'
                    order by x.field1_date";
            #endregion

            #region Object Class Props

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp userFirstName = UserOC.getObjectClassProp( CswNbtObjClassUser.FirstNamePropertyName );
            CswNbtMetaDataObjectClassProp userLastName = UserOC.getObjectClassProp( CswNbtObjClassUser.LastNamePropertyName );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp locationLocation = LocationOC.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );
            CswNbtMetaDataObjectClassProp locationName = LocationOC.getObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp inspectionDesignDueDate = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );
            CswNbtMetaDataObjectClassProp inspectionDesignStatus = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
            CswNbtMetaDataObjectClassProp inspectionDesignTarget = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataObjectClassProp inspectionDesignInspector = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.InspectorPropertyName );
            CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClassProp inspectionTargetDescription = InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            CswNbtMetaDataObjectClassProp inspectionTargetLocation = InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.LocationPropertyName );

            #endregion

            reportSqlText = string.Format( sqlText,
                 userFirstName.ObjectClassPropId.ToString(),
                 userLastName.ObjectClassPropId.ToString(),
                 locationLocation.ObjectClassPropId.ToString(),
                 locationName.ObjectClassPropId.ToString(),
                 inspectionTargetDescription.ObjectClassPropId.ToString(),
                 inspectionDesignDueDate.ObjectClassPropId.ToString(),
                 inspectionDesignStatus.ObjectClassPropId.ToString(),
                 inspectionDesignTarget.ObjectClassPropId.ToString(),
                 inspectionDesignInspector.ObjectClassPropId.ToString(),
                 inspectionTargetLocation.ObjectClassPropId.ToString()
                );

            return reportSqlText;
        }

    }//class CswUpdateSchemaCase26550

}//namespace ChemSW.Nbt.Schema