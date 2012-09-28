
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27859
    /// </summary>
    public class CswUpdateSchema_MobileModule_Case27859 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.Modules.EnableModule( CswNbtModuleName.Mobile );
        }//Update()

    }//class 

}//namespace ChemSW.Nbt.Schema