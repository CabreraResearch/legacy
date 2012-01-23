using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-15
    /// </summary>
    public class CswUpdateSchemaTo01L15 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 15 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24656

            CswNbtMetaDataNodeType InspectionSchedNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            if( null != InspectionSchedNt )
            {
                CswNbtMetaDataNodeTypeProp ParentTypeNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
                ParentTypeNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }
            #endregion Case 24656

            #region Case 24394

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            CswNbtMetaDataObjectClassProp EnabledOcp = MailReportOc.getObjectClassProp( CswNbtObjClassMailReport.EnabledPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( EnabledOcp, EnabledOcp.FieldTypeRule.SubFields.Default.Name, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EnabledOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EnabledOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            #endregion Case 24394


        }//Update()

    }//class CswUpdateSchemaTo01L15

}//namespace ChemSW.Nbt.Schema


