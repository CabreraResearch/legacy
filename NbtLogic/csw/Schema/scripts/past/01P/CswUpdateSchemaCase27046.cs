using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27046
    /// </summary>
    public class CswUpdateSchemaCase27046 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Subscriptions, false, "", "System" );
        }//Update()

    }//class CswUpdateSchemaCase27046

}//namespace ChemSW.Nbt.Schema