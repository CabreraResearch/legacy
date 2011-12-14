using System;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-03
    /// </summary>
    public class CswUpdateSchemaTo01L03 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 03 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24023

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp GnRunNowOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.RunNowPropertyName );
            CswNbtMetaDataObjectClassProp GnDueDateOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.NextDueDatePropertyName );
            if( null == GnRunNowOcp )
            {
                CswTableUpdate GeneratorUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + " Generator Run Now Update", "object_class_props" );
                DataTable GeneratorPropsTable = GeneratorUpdate.getEmptyTable();
                DataRow RunNowRow = _CswNbtSchemaModTrnsctn.addObjectClassPropRow( GeneratorPropsTable,
                                                                                   GeneratorOc,
                                                                                   CswNbtObjClassGenerator.RunNowPropertyName,
                                                                                   CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                                   false,
                                                                                   true,
                                                                                   false,
                                                                                   string.Empty,
                                                                                   Int32.MinValue,
                                                                                   false,
                                                                                   false,
                                                                                   false,
                                                                                   true,
                                                                                   string.Empty,
                                                                                   GnDueDateOcp.DisplayColAdd,
                                                                                   ( GnDueDateOcp.DisplayRowAdd + 1 ) );
                string Extended = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended );
                RunNowRow[Extended] = CswConvert.ToDbVal( "button" );
                string SetValOnAdd = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd );
                RunNowRow[SetValOnAdd] = CswConvert.ToDbVal( false );
                string AuditLevelString = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.auditlevel );
                RunNowRow[AuditLevelString] = CswConvert.ToDbVal( AuditLevel.NoAudit.ToString() );
                string StaticText = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext );
                RunNowRow[StaticText] = CswConvert.ToDbVal( "Run Now" );

                GeneratorUpdate.update( GeneratorPropsTable );
            }

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp MrRunNowOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassMailReport.RunNowPropertyName );
            CswNbtMetaDataObjectClassProp MrDueDateOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassMailReport.NextDueDatePropertyName );
            if( null == MrRunNowOcp )
            {
                CswTableUpdate MailReportUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + " Mail Report Run Now Update", "object_class_props" );
                DataTable MailReportPropsTable = MailReportUpdate.getEmptyTable();
                DataRow RunNowRow = _CswNbtSchemaModTrnsctn.addObjectClassPropRow( MailReportPropsTable,
                                                                                   MailReportOc,
                                                                                   CswNbtObjClassMailReport.RunNowPropertyName,
                                                                                   CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                                   false,
                                                                                   true,
                                                                                   false,
                                                                                   string.Empty,
                                                                                   Int32.MinValue,
                                                                                   false,
                                                                                   false,
                                                                                   false,
                                                                                   true,
                                                                                   string.Empty,
                                                                                   MrDueDateOcp.DisplayColAdd,
                                                                                   ( MrDueDateOcp.DisplayRowAdd + 1 ) );
                string Extended = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended );
                RunNowRow[Extended] = CswConvert.ToDbVal( "button" );
                string SetValOnAdd = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd );
                RunNowRow[SetValOnAdd] = CswConvert.ToDbVal( false );
                string AuditLevelString = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.auditlevel );
                RunNowRow[AuditLevelString] = CswConvert.ToDbVal( AuditLevel.NoAudit.ToString() );
                string StaticText = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext );
                RunNowRow[StaticText] = CswConvert.ToDbVal( "Run Now" );

                MailReportUpdate.update( MailReportPropsTable );
            }

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
            #endregion Case 24023


        }//Update()

    }//class CswUpdateSchemaTo01L03

}//namespace ChemSW.Nbt.Schema


