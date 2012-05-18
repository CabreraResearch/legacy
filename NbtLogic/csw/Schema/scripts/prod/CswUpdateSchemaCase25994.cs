namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25994
    /// </summary>
    public class CswUpdateSchemaCase25994 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswNbtResources.ConfigurationVariables.password_complexity.ToString(), "2" );
        }//Update()

    }//class CswUpdateSchemaCase25994

}//namespace ChemSW.Nbt.Schema