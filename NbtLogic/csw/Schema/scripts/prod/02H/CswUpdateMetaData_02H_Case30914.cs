using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30914 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30914; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "New fieldtype: Report Link"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswEnumNbtFieldType.ReportLink, CswEnumNbtFieldTypeDataType.INTEGER );
        } // update()

    } // class CswUpdateMetaData_02H_Case30914

}//namespace ChemSW.Nbt.Schema