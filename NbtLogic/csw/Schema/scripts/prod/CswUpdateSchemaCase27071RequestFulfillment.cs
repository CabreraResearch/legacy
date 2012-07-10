using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestFulfillment
    /// </summary>
    public class CswUpdateSchemaCase27071RequestFulfillment : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Fulfill_Request, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction(CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.Fulfill_Request);

        }//Update()

    }//class CswUpdateSchemaCase27071RequestFulfillment

}//namespace ChemSW.Nbt.Schema