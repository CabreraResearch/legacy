using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29570
    /// </summary>
    public class CswUpdateSchema_02D_Case29570B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29570; }
        }

        public override void update()
        {
            List<CswNbtView> ReportViews = _CswNbtSchemaModTrnsctn.restoreViews( "Reports" );
            foreach( CswNbtView ReportView in ReportViews )
            {
                if( ReportView.Visibility != CswEnumNbtViewVisibility.Property )
                {
                    ReportView.Root.ChildRelationships.Clear();
                    CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
                    CswNbtViewRelationship ReportGroupVR = ReportView.AddViewRelationship( ReportGroupOC, IncludeDefaultFilters: false );
                    CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
                    CswNbtMetaDataObjectClassProp ReportGroupOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportGroup );
                    ReportView.AddViewRelationship( ReportGroupVR, CswEnumNbtViewPropOwnerType.Second, ReportGroupOCP, false );
                    ReportView.save();
                }
            }

            List<CswNbtView> MailReportViews = _CswNbtSchemaModTrnsctn.restoreViews( "Mail Reports" );
            foreach( CswNbtView MailReportView in MailReportViews )
            {
                _updateMailReportView( MailReportView );
            }

            List<CswNbtView> MailReportFEViews = _CswNbtSchemaModTrnsctn.restoreViews( "Mail Reports (FE)" );
            foreach( CswNbtView MailReportView in MailReportFEViews )
            {
                _updateMailReportView( MailReportView );
            }

            List<CswNbtView> MailReportIMCSViews = _CswNbtSchemaModTrnsctn.restoreViews( "Mail Reports (IMCS)" );
            foreach( CswNbtView MailReportView in MailReportIMCSViews )
            {
                _updateMailReportView( MailReportView );
            }
        } // update()

        private void _updateMailReportView( CswNbtView MailReportView )
        {
            if( MailReportView.Visibility != CswEnumNbtViewVisibility.Property )
            {
                MailReportView.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass MailReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportGroupClass );
                CswNbtViewRelationship MailReportGroupVR = MailReportView.AddViewRelationship( MailReportGroupOC, IncludeDefaultFilters: false );
                CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
                CswNbtMetaDataObjectClassProp MailReportGroupOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.MailReportGroup );
                MailReportView.AddViewRelationship( MailReportGroupVR, CswEnumNbtViewPropOwnerType.Second, MailReportGroupOCP, false );
                MailReportView.save();
            }
        }

    }//class CswUpdateSchema_02C_Case29570B

}//namespace ChemSW.Nbt.Schema