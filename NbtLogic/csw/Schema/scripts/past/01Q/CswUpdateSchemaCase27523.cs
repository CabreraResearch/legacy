
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27523
    /// </summary>
    public class CswUpdateSchemaCase27523 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtView DoomedUoMView = _CswNbtSchemaModTrnsctn.restoreView( "Units of Measure" );
            DoomedUoMView.Delete();
        }//Update()

    }//class CswUpdateSchemaCase27523

}//namespace ChemSW.Nbt.Schema