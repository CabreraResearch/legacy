using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS51852A : CswUpdateSchemaTo
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
            return "A";
        }
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswEnumNbtFieldType.Permission, CswEnumNbtFieldTypeDataType.CLOB );
        }
    }
}