
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27759
    /// </summary>
    public class CswUpdateSchema_01S_DevModule_Case27759 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createModule( "ChemSW Development Module", CswNbtModuleName.Dev.ToString(), false );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema