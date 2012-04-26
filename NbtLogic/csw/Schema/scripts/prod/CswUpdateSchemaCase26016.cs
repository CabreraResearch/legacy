
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26016
    /// </summary>
    public class CswUpdateSchemaCase26016 : CswUpdateSchemaTo
    {
        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources.ModulesEnabled().Contains( CswNbtResources.CswNbtModule.CISPro ) )
            {
                _CswNbtSchemaModTrnsctn.ViewSelect.deleteViewByName( "MSDS Expiring Next Month" );
            }
        }//Update()

    }//class CswUpdateSchemaCase26016

}//namespace ChemSW.Nbt.Schema