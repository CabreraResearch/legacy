using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26094
    /// </summary>
    public class CswUpdateSchemaCase26094 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass,
                CswNbtObjClassTask.PartsPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.LogicalSet,
                AuditLevel: Audit.AuditLevel.PlainAudit
            );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                TaskOC.getObjectClassProp( CswNbtObjClassTask.PartsPropertyName ),
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.valueoptions,
                CswNbtObjClassTask.PartsXValueName
            );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                TaskOC.getObjectClassProp( CswNbtObjClassTask.PartsPropertyName ),
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd,
                false
            );

        }//Update()
    }//class CswUpdateSchemaCase26094
}//namespace ChemSW.Nbt.Schema