using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-04
    /// </summary>
    public class CswUpdateSchemaTo01L04 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 04 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24023

            //Generators
            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeProp DueDateNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.NextDueDatePropertyName );
                CswNbtMetaDataNodeTypeProp RunNowNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.RunNowPropertyName );

                RunNowNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DueDateNtp );
            }

            //Mail Reports
            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            foreach( CswNbtMetaDataNodeType MailReportNt in MailReportOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeProp DueDateNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.NextDueDatePropertyName );
                CswNbtMetaDataNodeTypeProp RunNowNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.RunNowPropertyName );

                RunNowNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DueDateNtp );
            }

            #endregion Case 24023

            #region Case 24431

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            CswTableUpdate ScheduledRulesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_scheduledrules_update", "scheduledrules" );
            DataTable ScheduledRulesTable = ScheduledRulesUpdate.getTable();
            foreach( DataRow Row in ScheduledRulesTable.Rows )
            {
                Row["totalroguecount"] = CswConvert.ToDbVal( 0 );
                Row["reprobate"] = CswConvert.ToDbVal( 0 );
                Row["failedcount"] = CswConvert.ToDbVal( 0 );
            }
            ScheduledRulesUpdate.update( ScheduledRulesTable );

            #endregion Case 24431

            #region Case 24241

            Int32 ViewScheduledRulesId = _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.View_Scheduled_Rules, true, string.Empty, "System" );
            Int32 NbtManagerModuleId = _CswNbtSchemaModTrnsctn.getModuleId( CswNbtResources.CswNbtModule.NBTManager );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( NbtManagerModuleId, ViewScheduledRulesId );

            #endregion Case 24241

        }//Update()

    }//class CswUpdateSchemaTo01L04

}//namespace ChemSW.Nbt.Schema


