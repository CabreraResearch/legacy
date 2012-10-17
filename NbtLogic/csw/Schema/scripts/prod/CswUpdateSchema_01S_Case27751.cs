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
    public class CswUpdateSchema_01S_Case27751 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswTableUpdate ConfigVarUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "loc_use_images_update", "configuration_variables" );
            DataTable ConfigVarTable = ConfigVarUpdate.getTable( " where variablename = 'loc_use_images'" );
            foreach( DataRow ConfigVarRow in ConfigVarTable.Rows )
            {
                ConfigVarRow["issystem"] = "1";
            }
            ConfigVarUpdate.update( ConfigVarTable );
        }//Update()

    }//class CswUpdateSchema_01S_Case27751 

}//namespace ChemSW.Nbt.Schema