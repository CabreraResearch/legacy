using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28254
    /// </summary>
    public class CswUpdateSchema_01T_Case28254 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28254; }
        }

        public override void update()
        {
            // Fix servermanaged on Mail Report Last Processed
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp LastProcessedOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.LastProcessed );
            
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( 
                LastProcessedOCP, 
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, 
                Value: true );

        } // Update()

    }//class CswUpdateSchema_01T_Case28254

}//namespace ChemSW.Nbt.Schema