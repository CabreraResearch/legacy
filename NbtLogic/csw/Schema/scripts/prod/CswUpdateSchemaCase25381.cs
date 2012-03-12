using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25381
    /// </summary>
    public class CswUpdateSchemaCase25381 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Buttons should not be readonly unless they are disabled

            CswNbtMetaDataObjectClass CustomerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            CswNbtMetaDataObjectClassProp CustomerLoginOCP = CustomerOC.getObjectClassProp( CswNbtObjClassCustomer.LoginPropertyName );
            CswNbtMetaDataObjectClassProp ReportRunOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.btnRunPropertyName );
            CswNbtMetaDataObjectClassProp GeneratorRunNowOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.RunNowPropertyName );
            CswNbtMetaDataObjectClassProp MailReportRunNowOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.RunNowPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerLoginOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportRunOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GeneratorRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );

        }//Update()

    }//class CswUpdateSchemaCase25381

}//namespace ChemSW.Nbt.Schema