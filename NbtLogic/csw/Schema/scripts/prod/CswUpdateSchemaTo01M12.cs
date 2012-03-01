using System;
using System.Data;
using System.Collections.Generic;
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
    /// Updates the schema to version 01M-12
    /// </summary>
    public class CswUpdateSchemaTo01M12 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 12 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24975


            CswNbtMetaDataObjectClass ReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass,CswNbtObjClassReport.SqlPropertyName,CswNbtMetaDataFieldType.NbtFieldType.Memo);

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass,
                                                                       CswNbtObjClassReport.btnRunPropertyName,
                                                                       CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                       false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                                                       CswNbtNodePropButton.ButtonMode.button.ToString(),
                                                                       false,
                                                                       AuditLevel.NoAudit,
                                                                       "Run");

            CswNbtMetaDataNodeType rptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Report");

            if( null != rptNT )
            {
                CswNbtNode rptNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassReport rptNodeAsReport = CswNbtNodeCaster.AsReport( rptNode );
                rptNodeAsReport.Category = "System";
                rptNodeAsReport.ReportName.Text = "SQL Report View Dictionary";
                rptNodeAsReport.SQL.Text = "select * from vwAutoViewColNames";                
                rptNodeAsReport.postChanges(true);


                CswNbtNode rptNode2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassReport rptNodeAsReport2 = CswNbtNodeCaster.AsReport( rptNode2 );
                rptNodeAsReport2.Category = "SI_Example";
                rptNodeAsReport2.ReportName.Text = "OOC Issues last 30 days";
                rptNodeAsReport2.SQL.Text = @"select dept.P17 dept,bldg.P24 bldg,targ.P05_LOCATION,targ.nodeid ip_nodeid, targ.P01_BARCODE ip_barcode,
                                                 targ.P02_DESCRIPTION ip_desc,
                                                 targ.P10_STATUS ip_status,
                                                 targ.P04_LASTINSPECTIONDATE ip_lastinspected,
                                                 p4391 prot_descr,q.questionno,q.propname question,q.iscompliant,
                                                 q.answer,q.correctiveaction,q.comments 
                                                from ntsi_protocol pr
                                                 join ocinspectiontargetclass targ on targ.nodeid=pr.P4390INSPECTIONTARGETCL_OCFK
                                                 join vwquestioncompliance q on q.nodeid=pr.nodeid
                                                 join ntroom room on room.nodeid=targ.P05_Location_fk
                                                 join ntbuilding bldg on bldg.nodeid=room.P2339LOCATIONCLASS_OCFK
                                                 left outer join ntdepartment dept on dept.nodeid=bldg.P25_DEPARTMENT_NTFK
                                                where (q.iscompliant='0' or q.correctiveaction is not null )
                                                 and to_date(targ.P04_LASTINSPECTIONDATE,'mm/dd/yyyy') between SYSDATE-30 and SYSDATE
                                                order by targ.P05_LOCATION,pr.nodeid,q.questionno ";

                rptNodeAsReport2.postChanges( true );


            }
            #endregion case 24975

        }//Update()

    }//class CswUpdateSchemaTo01M12

}//namespace ChemSW.Nbt.Schema