using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29499
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29499A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29499; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CofAOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
            if( false == CofAOC.getNodeTypes().Any() )
            {
                _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( CofAOC.ObjectClassId, "C of A Document", "Materials" );
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29499

}//namespace ChemSW.Nbt.Schema