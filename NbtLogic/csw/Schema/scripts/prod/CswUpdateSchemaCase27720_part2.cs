using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720_part2
    /// </summary>
    public class CswUpdateSchemaCase27720_part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Create a demo mail report (notification)

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            CswNbtMetaDataNodeType MailReportNT = MailReportOC.FirstNodeType;
            CswNbtMetaDataNodeTypeProp MailReportNameNTP = MailReportNT.getNodeTypeProp( "Name" );

            CswNbtMetaDataNodeType UserNT = UserOC.FirstNodeType;
            CswNbtMetaDataNodeTypeProp UserLockedNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.AccountLocked );


            if( null != MailReportNT && null != UserNT )
            {
                CswNbtObjClassMailReport MailReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                if( null != MailReportNameNTP )
                {
                    MailReportNode.Node.Properties[MailReportNameNTP].AsText.Text = "Locked Users Notification";
                }
                MailReportNode.OutputFormat.Value = "link";

                MailReportNode.Type.Value = CswNbtObjClassMailReport.TypeOptionView;
                MailReportNode.Event.Value = CswNbtObjClassMailReport.EventOption.Edit.ToString();
                MailReportNode.TargetType.SelectedNodeTypeIds.Add( UserNT.NodeTypeId.ToString() );
                MailReportNode.Message.Text = "The following user accounts have been locked:";
                MailReportNode.DueDateInterval.RateInterval.setHourly( 1, DateTime.Now );
                MailReportNode.Enabled.Checked = Tristate.False;

                CswNbtObjClassUser AdminUser = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( "admin" );
                if( null != AdminUser )
                {
                    MailReportNode.Recipients.AddUser( AdminUser.UserId );
                }

                MailReportNode.postChanges( true );

                CswNbtView ReportView = _CswNbtSchemaModTrnsctn.restoreView( MailReportNode.ReportView.ViewId );
                ReportView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship UserRel = ReportView.AddViewRelationship( UserNT, false );
                ReportView.AddViewPropertyAndFilter( UserRel, UserLockedNTP, Tristate.True.ToString() );
                ReportView.save();

            } // if( null != MailReportNT )

        }//Update()

    }//class CswUpdateSchemaCase27720_part2

}//namespace ChemSW.Nbt.Schema