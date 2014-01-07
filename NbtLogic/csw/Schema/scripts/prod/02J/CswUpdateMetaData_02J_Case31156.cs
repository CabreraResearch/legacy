using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case31156 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31156; }
        }

        public override string Title
        {
            get { return "Constituents no longer appear in universal search"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ConstituentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Constituent" );
            if( null != ConstituentNT )
            {
                ConstituentNT._DataRow["searchdeferpropid"] = 0;
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema