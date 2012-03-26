using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25515
    /// </summary>
    public class CswUpdateSchemaCase25515 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Add new action: Multi-edit
            Int32 ActionId = _CswNbtSchemaModTrnsctn.createAction( Actions.CswNbtActionName.Multi_Edit, false, "", "" );

        }//Update()

    }//class CswUpdateSchemaCase25515

}//namespace ChemSW.Nbt.Schema