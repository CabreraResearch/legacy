using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53049 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53049; }
        }

        public override string Title
        {
            get { return "Demo Mail Report Group and Permissions"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportGroupClass );
            CswNbtObjClassMailReportGroup DemoMailReports = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportGroupOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassMailReportGroup DemoMailReportNode = NewNode;
                    DemoMailReportNode.IsDemo = true;
                    DemoMailReportNode.Name.Text = "Demo Mail Reports";
                } );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, false, false, true ) )
            {
                if( MailReportNode.IsDemo )
                {
                    MailReportNode.MailReportGroup.RelatedNodeId = DemoMailReports.NodeId;
                    MailReportNode.postChanges( false );
                }
            }
            CswNbtMetaDataObjectClass MailReportGroupPermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportGroupPermissionClass );
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false, false, true ) )
            {
                if( RoleNode.Administrator.Checked == CswEnumTristate.True )
                {
                    CswNbtObjClassMailReportGroupPermission DemoMailReportGroupPerm = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportGroupPermissionOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassMailReportGroupPermission DemoMailReportPermNode = NewNode;
                        DemoMailReportPermNode.Role.RelatedNodeId = RoleNode.NodeId;
                        DemoMailReportPermNode.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                        DemoMailReportPermNode.PermissionGroup.RelatedNodeId = DemoMailReports.NodeId;
                        DemoMailReportPermNode.View.Checked = CswEnumTristate.True;
                        DemoMailReportPermNode.Edit.Checked = CswEnumTristate.True;
                    } );
                }
            }
            CswNbtObjClassMailReportGroup DefaultMailReportGroup = null;
            foreach( CswNbtObjClassMailReportGroup MailReportGroupNode in MailReportGroupOC.getNodes( false, false, false, true ) )
            {
                if( MailReportGroupNode.Name.Text == "Default Mail Report Group" )
                {
                    DefaultMailReportGroup = MailReportGroupNode;
                }
            }
            CswNbtView MyExpiringContainersView = _CswNbtSchemaModTrnsctn.restoreView( "My Expiring Containers" );
            if( null != MyExpiringContainersView && null != DefaultMailReportGroup )
            {
                CswNbtObjClassMailReport MyExpiringContainersReport = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassMailReport MyExpiringContainersReportNode = NewNode;
                    MyExpiringContainersReportNode.Name.Text = "My Expiring Containers";
                    MyExpiringContainersReportNode.Type.Value = CswNbtObjClassMailReport.TypeOptionView;
                    MyExpiringContainersReportNode.Event.Value = "Edit";
                    MyExpiringContainersReportNode.MailReportGroup.RelatedNodeId = DefaultMailReportGroup.NodeId;
                    MyExpiringContainersReportNode.Message.Text = "The following containers will be expiring soon: ";
                    MyExpiringContainersReportNode.Enabled.Checked = CswEnumTristate.False;
                    CswNbtView MyExpContMRView = _CswNbtSchemaModTrnsctn.makeView();
                    MyExpContMRView.saveNew( "My Expiring Containers", CswEnumNbtViewVisibility.Hidden, CopyViewId: MyExpiringContainersView.ViewId.get() );
                    MyExpiringContainersReportNode.ReportView.ViewId = MyExpContMRView.ViewId;
                    CswRateInterval DailyRate = _CswNbtSchemaModTrnsctn.makeRateInterval();
                    DailyRate.setHourly( 24, DateTime.Today );
                    MyExpiringContainersReportNode.DueDateInterval.RateInterval = DailyRate;
                } );
            }
        }
    }
}