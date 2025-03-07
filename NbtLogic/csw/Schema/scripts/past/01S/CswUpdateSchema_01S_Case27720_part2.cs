﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720_part2
    /// </summary>
    public class CswUpdateSchema_01S_Case27720_part2 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27720; }
        }

        public override void update()
        {
            // Create a demo mail report (notification)

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );

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
                MailReportNode.Enabled.Checked = Tristate.False;
                MailReportNode.IsDemo = true;

                CswRateInterval HourlyRate = _CswNbtSchemaModTrnsctn.makeRateInterval();
                HourlyRate.setHourly( 1, DateTime.Now );
                MailReportNode.DueDateInterval.RateInterval = HourlyRate;

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

    }//class CswUpdateSchema_01S_Case27720_part2

}//namespace ChemSW.Nbt.Schema