using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25466
    /// </summary>
    public class CswUpdateSchemaCase25466 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Buttons should not be servermanaged

            CswNbtMetaDataObjectClass CustomerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            CswNbtMetaDataObjectClassProp CustomerLoginOCP = CustomerOC.getObjectClassProp( CswNbtObjClassCustomer.LoginPropertyName );
            CswNbtMetaDataObjectClassProp ReportRunOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.btnRunPropertyName );
            CswNbtMetaDataObjectClassProp GeneratorRunNowOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.RunNowPropertyName );
            CswNbtMetaDataObjectClassProp MailReportRunNowOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.RunNowPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerLoginOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportRunOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GeneratorRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );



        }//Update()

    }//class CswUpdateSchemaCase25466

}//namespace ChemSW.Nbt.Schema