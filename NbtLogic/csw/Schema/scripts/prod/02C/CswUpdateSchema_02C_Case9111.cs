using System.Data;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02C_Case9111 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 9111; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.ConfigVbls.addNewConfigurationValue( CswEnumConfigurationVariableNames.loggly_uri, "https://logs.loggly.com/inputs/75c30bba-4b60-496c-a348-7eb413c01037", "URI for posts to loggly log venue", true );
            _CswNbtSchemaModTrnsctn.ConfigVbls.saveConfigVariables();
        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX


}//namespace ChemSW.Nbt.Schema