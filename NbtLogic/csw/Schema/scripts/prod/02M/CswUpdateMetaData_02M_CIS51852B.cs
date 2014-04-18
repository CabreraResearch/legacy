using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS51852B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 51852; }
        }

        public override string Title
        {
            get { return "Create Permission Fieldtype"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }
        public override void update()
        {
            int PermissionFieldTypeId = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Permission ).FieldTypeId;
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp NTPermissionsOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.NodeTypePermissions );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NTPermissionsOCP, CswEnumNbtObjectClassPropAttributes.fieldtypeid, PermissionFieldTypeId );
        }
    }
}