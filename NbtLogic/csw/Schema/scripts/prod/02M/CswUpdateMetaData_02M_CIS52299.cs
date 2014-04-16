using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52299 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52299; }
        }

        public override string Title
        {
            get { return "NumericRange Fieldtype"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswEnumNbtFieldType.NumericRange, CswEnumNbtFieldTypeDataType.DOUBLE );
        }
    }
}