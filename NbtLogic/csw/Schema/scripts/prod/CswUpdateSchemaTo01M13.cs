using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-13
    /// </summary>
    public class CswUpdateSchemaTo01M13 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 13 ); } }
        // public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region 25290


            //remove nodetypes (should delete nodes automatically)
            Collection<CswNbtMetaDataNodeType> doomed_nts = new Collection<CswNbtMetaDataNodeType>();
            IEnumerable<CswNbtMetaDataNodeType> nts = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes();
            foreach( CswNbtMetaDataNodeType nt in nts )
            {
                if( nt.NodeTypeName == "Inspection Group"
                    || nt.NodeTypeName == "FE Inspection Point"
                    || nt.NodeTypeName == "Physical Inspection Route"
                    || nt.NodeTypeName == "Physical Inspection" ) doomed_nts.Add( nt );
            }
            foreach( CswNbtMetaDataNodeType ntd in doomed_nts )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( ntd );
            }

            // Inspection Group
            // FE Inspection Point
            // Physical Inspection Route
            // Physical Inspection


            //remove remaining views in the inspections category

            //if we have a Department nodetype, then ROOM points to department, and building doesn't
            CswNbtMetaDataNodeType deptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
            if( null != deptNT )
            {
                CswNbtMetaDataNodeType bldg = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
                if( null != bldg )
                {
                    CswNbtMetaDataNodeTypeProp bdept = bldg.getNodeTypeProp( "Department" );
                    if( null != bdept )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( bdept );
                    }
                }
                CswNbtMetaDataNodeType room = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
                if( null != room )
                {
                    CswNbtMetaDataNodeTypeProp rdept = room.getNodeTypeProp( "Department" );
                    if( null == rdept )
                    {
                        CswNbtMetaDataNodeTypeTab tab = room.getFirstNodeTypeTab();
                        CswNbtMetaDataNodeTypeProp deptProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( room, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Department", tab.TabId );
                        deptProp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(),deptNT.NodeTypeId);
                    }
                }
            }


            #endregion 25290


            #region case 24975


            CswNbtMetaDataObjectClass ReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass, CswNbtObjClassReport.SqlPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Memo );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass,
                                                                       CswNbtObjClassReport.btnRunPropertyName,
                                                                       CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                       false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                                                       CswNbtNodePropButton.ButtonMode.button.ToString(),
                                                                       false,
                                                                       AuditLevel.NoAudit,
                                                                       "Run" );

            CswNbtMetaDataNodeType rptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );

            if( null != rptNT )
            {
                CswNbtNode rptNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassReport rptNodeAsReport = CswNbtNodeCaster.AsReport( rptNode );
                rptNodeAsReport.Category.Text = "System";
                rptNodeAsReport.ReportName.Text = "SQL Report View Dictionary";
                rptNodeAsReport.SQL.Text = "select * from vwAutoViewColNames";
                rptNodeAsReport.postChanges( true );


                CswNbtNode rptNode2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassReport rptNodeAsReport2 = CswNbtNodeCaster.AsReport( rptNode2 );
                rptNodeAsReport2.Category.Text = "SI_Example";
                rptNodeAsReport2.ReportName.Text = "OOC Issues last 30 days";
                rptNodeAsReport2.SQL.Text = @"select dept.P17 dept,targ.P04_LOCATION,targ.nodeid ip_nodeid, targ.P01_BARCODE ip_barcode,
                                                 targ.P02_DESCRIPTION ip_desc,
                                                 targ.P07_STATUS ip_status,
                                                 targ.P03_LASTINSPECTIONDATE ip_lastinspected,
                                                 p4391 prot_descr,q.questionno,q.question,q.iscompliant,
                                                 q.answer,q.correctiveaction,q.comments 
                                                from ntsi_protocol pr
                                                 join ocinspectiontargetclass targ on targ.nodeid=pr.P4390INSPECTIONTARGETCL_OCFK
                                                 join vwquestiondetail q on q.nodeid=pr.nodeid
                                                 left outer join ntroom room on room.nodeid=targ.P04_Location_fk
                                                 left outer join ntdepartment dept on dept.nodeid=room.P4486_DEPARTMENT_NTFK
                                                where (q.iscompliant='0' or q.correctiveaction is not null ) and 
                                                 targ.P03_LASTINSPECTIONDATE is null or 
                                                 to_date(targ.P03_LASTINSPECTIONDATE,'mm/dd/yyyy') between SYSDATE-30 and SYSDATE
                                                order by targ.P04_LOCATION,pr.nodeid,q.questionno ";

                rptNodeAsReport2.postChanges( true );


            }
            #endregion case 24975

        }//Update()

    }//class CswUpdateSchemaTo01M13

}//namespace ChemSW.Nbt.Schema