using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30575 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Equipment Type Name is a Required Property"; } }

        public override string ScriptName
        {
            get { return "Name = Required"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30575; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass EquipmentType = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentTypeClass );
            CswNbtMetaDataObjectClassProp NameOCP = EquipmentType.getObjectClassProp( CswNbtObjClassEquipmentType.PropertyName.TypeName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
        }
    }
}


