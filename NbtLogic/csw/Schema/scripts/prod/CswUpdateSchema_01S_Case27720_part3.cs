using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720_part3
    /// </summary>
    public class CswUpdateSchema_01S_Case27720_part3 : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            CswNbtMetaDataNodeType MailReportNT = MailReportOC.FirstNodeType;

            if( null != MailReportNT )
            {
                CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.DueDateInterval );
                CswNbtMetaDataNodeTypeProp EnabledNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Enabled );
                CswNbtMetaDataNodeTypeProp EventNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Event );
                CswNbtMetaDataNodeTypeProp FinalDueDateNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.FinalDueDate );
                CswNbtMetaDataNodeTypeProp LastProcessedNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.LastProcessed );
                CswNbtMetaDataNodeTypeProp MessageNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Message );
                CswNbtMetaDataNodeTypeProp NameNTP = MailReportNT.getNodeTypeProp( "Name" );
                CswNbtMetaDataNodeTypeProp NextDueDateNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.NextDueDate );
                CswNbtMetaDataNodeTypeProp NodesToReportNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.NodesToReport );
                CswNbtMetaDataNodeTypeProp OutputFormatNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.OutputFormat );
                CswNbtMetaDataNodeTypeProp RecipientsNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Recipients );
                CswNbtMetaDataNodeTypeProp ReportNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Report );
                CswNbtMetaDataNodeTypeProp ReportViewNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
                CswNbtMetaDataNodeTypeProp RunNowNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.RunNow );
                CswNbtMetaDataNodeTypeProp RunStatusNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.RunStatus );
                CswNbtMetaDataNodeTypeProp RunTimeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.RunTime );
                CswNbtMetaDataNodeTypeProp TargetTypeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.TargetType );
                CswNbtMetaDataNodeTypeProp TypeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Type );
                CswNbtMetaDataNodeTypeProp NTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.WarningDays );

                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutEdit = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit;

                CswNbtMetaDataNodeTypeTab Tab1 = MailReportNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeTab Tab2 = MailReportNT.getSecondNodeTypeTab();


                // Fix default mail report layout

                Tab2.TabName = "Schedule";  // formerly "Settings"

                // clear the old layout
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.clearLayout( LayoutEdit, MailReportNT.NodeTypeId );

                // setup the new layout
                if( null != NameNTP )
                {
                    NameNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 1, 1 );
                }
                TypeNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 2, 1 );
                ReportNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 3, 1 );
                EventNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 4, 1 );
                ReportViewNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 5, 1 );
                TargetTypeNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 6, 1 );
                OutputFormatNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 7, 1 );
                MessageNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 8, 1 );
                RecipientsNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 9, 1 );
                EnabledNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 10, 1 );
                RunNowNTP.updateLayout( LayoutEdit, true, Tab1.TabId, 11, 1 );

                DueDateIntervalNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 1, 1 );
                NextDueDateNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 2, 1 );
                FinalDueDateNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 3, 1 );
                RunTimeNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 4, 1 );
                LastProcessedNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 5, 1 );
                RunStatusNTP.updateLayout( LayoutEdit, true, Tab2.TabId, 6, 1 );


                // Remove Output Format from add layout
                OutputFormatNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }
        }//Update()

    }//class CswUpdateSchema_01S_Case27720_part3

}//namespace ChemSW.Nbt.Schema