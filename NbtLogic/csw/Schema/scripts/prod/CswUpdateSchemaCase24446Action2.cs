using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24446Action2
    /// </summary>
    public class CswUpdateSchemaCase24446Action2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string SQL = "update actions set showinlist=" + CswConvert.ToDbVal( false ) + " where actionname='" + CswNbtActionName.Receiving + "'";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SQL );


        }//Update()

    }//class CswUpdateSchemaCase24446Action2

}//namespace ChemSW.Nbt.Schema