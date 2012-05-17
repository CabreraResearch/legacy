using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25704
    /// </summary>
    public class CswUpdateSchemaCase25704 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //nuke old SI report
            CswPrimaryKey oldNodeid = null;
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            foreach( CswNbtNode ReportNode in ReportOC.getNodes( false, false ) )
            {
                CswNbtObjClassReport nodeAsReport = CswNbtNodeCaster.AsReport( ReportNode );
                if( nodeAsReport.ReportName.Text == "Lab 1 Deficiencies" )
                {
                    oldNodeid = ReportNode.NodeId;
                    ReportNode.delete();
                }
            }


            // new nodetypes
            // IMCS_Report category=Equipment
            CswNbtMetaDataNodeType imcsRptNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "ReportClass", "IMCS_Report", "Equipment" );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.IMCS, imcsRptNT.NodeTypeId );
            imcsRptNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassReport.ReportNamePropertyName ) );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.IMCS, imcsRptNT.NodeTypeId );
            // SI_Report category=Inspections
            CswNbtMetaDataNodeType siRptNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "ReportClass", "SI_Report", "Inspections" );
            siRptNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassReport.ReportNamePropertyName ) );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.SI, siRptNT.NodeTypeId );

            //new SI report
            //report and mailer
            CswNbtMetaDataNodeType rptNT = siRptNT;
            CswNbtNode rptNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassReport rptNodeAsReport = CswNbtNodeCaster.AsReport( rptNode );

            CswNbtMetaDataNodeType DesignNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Checklist" );
            Int32 locPropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( DesignNt.NodeTypeId, "Location" );
            Int32 inspnamePropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( DesignNt.NodeTypeId, "Name" );
            Int32 inspdatePropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( DesignNt.NodeTypeId, "Inspection Date" );
            Int32 targPropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( DesignNt.NodeTypeId, "Target" );



            rptNodeAsReport.Category.Text = "Lab Safety";
            rptNodeAsReport.ReportName.Text = "Lab 1 Deficiencies";
            rptNodeAsReport.SQL.Text = @"select des.P" + inspdatePropId.ToString() + @" as InspectionDate, 
                                      des.P" + inspnamePropId.ToString() + @" as InspectionName,
                                      des.P" + locPropId.ToString() + @" as Location,
                                      CASE nvl(q.correctiveaction,'NULL')
                                       WHEN 'NULL' then 'NO'
                                       ELSE 'yes'
                                      END as Resolved,
                                     q.questionno,
                                      q.question,
                                      q.answer,
                                      q.correctiveaction,
                                      q.comments
                                      from ntlabsafetychecklist des
                                      join ntlabsafety targ on targ.nodeid=des.P" + targPropId.ToString() + @"_labsafety_ntfk
                                    join vwquestiondetail q on q.nodeid = des.nodeid
                                          where (q.iscompliant = '0' or q.correctiveaction is not null)
                                           and des.P" + locPropId.ToString() + @" like '%> Lab 1'
                                      order by des.P" + locPropId.ToString() + @", des.P" + inspdatePropId.ToString() + @", q.questionno";
            rptNode.IsDemo = true;
            rptNodeAsReport.postChanges( true );


            //fix the mailer node to the new reportnode
            if( oldNodeid != null )
            {
                CswNbtMetaDataObjectClass mailOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
                foreach( CswNbtNode mailNode in mailOC.getNodes( false, false ) )
                {
                    CswNbtObjClassMailReport nodeAsMail = CswNbtNodeCaster.AsMailReport( mailNode );
                    if( nodeAsMail.Report.RelatedNodeId == oldNodeid )
                    {
                        nodeAsMail.Report.RelatedNodeId = rptNode.NodeId;
                        mailNode.IsDemo = true;
                        nodeAsMail.postChanges( true );
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25704

}//namespace ChemSW.Nbt.Schema