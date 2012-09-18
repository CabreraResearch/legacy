
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27759
    /// </summary>
    public class CswUpdateSchema_DevModule_Case27759 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createModule( "ChemSW Development Module", CswNbtModuleName.Dev.ToString(), false );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema