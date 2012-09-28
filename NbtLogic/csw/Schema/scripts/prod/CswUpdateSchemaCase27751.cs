using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27751
    /// </summary>
    public class CswUpdateSchemaCase27751 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update configuration_variables set issystem = '1' where variablename = 'loc_use_images'" );
        }//Update()

    }//class CswUpdateSchemaCase27751 

}//namespace ChemSW.Nbt.Schema