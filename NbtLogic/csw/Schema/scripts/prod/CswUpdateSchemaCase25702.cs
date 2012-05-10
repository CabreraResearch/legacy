using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25702
    /// </summary>
    public class CswUpdateSchemaCase25702 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string OldPropName = "Run Status_OLD";

            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            //rename existing RunStatus to RunStatus_old
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.RunStatusPropertyName ),
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname,
                            OldPropName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.RunStatusPropertyName ),
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname,
                            OldPropName );
        }//Update()

    }//class CswUpdateSchemaCase25702

}//namespace ChemSW.Nbt.Schema