using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27548
    /// </summary>
    public class CswUpdateSchemaCase27548 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            foreach( CswNbtMetaDataNodeType MailReportNt in MailReportOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp WarningDaysNtp = MailReportNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.WarningDays );
                WarningDaysNtp.removeFromAllLayouts();
                WarningDaysNtp.DefaultValue.AsNumber.Value = 0;
            }
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOc.getNodes( false, false ) )
            {
                
                CswNbtObjClassMailReport NodeAsReport = MailReportNode;
                NodeAsReport.WarningDays.Value = 0;
                NodeAsReport.postChanges( false );

                
            }
        }//Update()

    }//class CswUpdateSchemaCase27548

}//namespace ChemSW.Nbt.Schema