
using ChemSW.Log;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26924
    /// </summary>
    public class CswUpdateSchemaCase26924 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.netquantity_enforced, "When set to 1, total quantity to deduct in the Dispense Container wizard cannot exceed the parent container's netquantity..", LogLevels.None, IsSystem: false );
        }//Update()

    }//class CswUpdateSchemaCase26924

}//namespace ChemSW.Nbt.Schema