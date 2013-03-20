using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28498
    /// </summary>
    public class CswUpdateSchema_01V_Case28498 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28498; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            //CswNbtMetaDataObjectClassProp MailReportViewOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp MailReportViewNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
                CswNbtView DefaultView = _CswNbtSchemaModTrnsctn.restoreView( MailReportViewNTP.DefaultValue.AsViewReference.ViewId );
                DefaultView.ViewMode = NbtViewRenderingMode.Grid;
                DefaultView.save();
            }
        } //update()

    }//class CswUpdateSchema_01V_Case28498

}//namespace ChemSW.Nbt.Schema