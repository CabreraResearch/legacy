using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25471
    /// </summary>
    public class CswUpdateSchemaCase25471 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Duplicate with CswUpdateSchemaCase25466 and CswUpdateSchemaCase25381
            // Some schemata on Madeye did not get the change.  So we'll make sure they do.

            CswNbtMetaDataObjectClass CustomerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            CswNbtMetaDataObjectClassProp CustomerLoginOCP = CustomerOC.getObjectClassProp( CswNbtObjClassCustomer.LoginPropertyName );
            CswNbtMetaDataObjectClassProp ReportRunOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.btnRunPropertyName );
            CswNbtMetaDataObjectClassProp GeneratorRunNowOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.RunNowPropertyName );
            CswNbtMetaDataObjectClassProp MailReportRunNowOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.RunNowPropertyName );

            // Buttons should not be readonly
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerLoginOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportRunOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GeneratorRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );

            // Buttons should not be servermanaged
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerLoginOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportRunOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GeneratorRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportRunNowOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );

        }//Update()

    }//class CswUpdateSchemaCase25471

}//namespace ChemSW.Nbt.Schema