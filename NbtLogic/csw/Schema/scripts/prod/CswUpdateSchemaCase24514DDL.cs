

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514DDL : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "jct_nodes_props", "hidden", "Determines whether property displays.", true, false );

        }//Update()

    }//class CswUpdateSchemaCase24514DDL

}//namespace ChemSW.Nbt.Schema