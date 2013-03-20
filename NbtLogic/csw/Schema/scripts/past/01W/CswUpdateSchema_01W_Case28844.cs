using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28844
    /// </summary>
    public class CswUpdateSchema_01W_Case28844 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28844; }
        }

        public override void update()
        {
            // Fix visibility on existing mail report views
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, true ) )
            {
                if( MailReportNode.ReportView.ViewId.isSet() )
                {
                    CswNbtView ReportView = _CswNbtSchemaModTrnsctn.restoreView( MailReportNode.ReportView.ViewId );
                    if( null != ReportView )
                    {
                        ReportView.SetVisibility( NbtViewVisibility.Hidden, null, null );
                        ReportView.save();
                    
                    } // if( null != ReportView )
                } // if( MailReportNode.ReportView.ViewId.isSet() )
            } // foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, true ) )

        } //Update()

    }//class CswUpdateSchema_01V_Case28844

}//namespace ChemSW.Nbt.Schema