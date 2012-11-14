using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27876
    /// </summary>
    public class CswUpdateSchema_01U_Case27876 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27876; }
        }

        public override void update()
        {
            CswNbtMetaDataFieldType CASNoFT = _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswNbtMetaDataFieldType.NbtFieldType.CASNo, CswNbtMetaDataFieldType.DataType.TEXT );
        }

        //Update()

    }//class CswUpdateSchemaCase27876

}//namespace ChemSW.Nbt.Schema