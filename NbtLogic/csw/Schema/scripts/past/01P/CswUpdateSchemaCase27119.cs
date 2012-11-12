
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27119
    /// </summary>
    public class CswUpdateSchemaCase27119 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( "custom_barcodes", "If set to 1, users can define their own barcodes on new containers.", "0", false );
        }//Update()

    }//class CswUpdateSchemaCase27119

}//namespace ChemSW.Nbt.Schema