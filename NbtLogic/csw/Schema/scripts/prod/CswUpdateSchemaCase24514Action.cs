using System;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514Action : CswUpdateSchemaTo
    {
        public override void update()
        {
            Int32 SubmitRequestActionId = _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Submit_Request, true, "", "Requests" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.Submit_Request );

        }//Update()

    }//class CswUpdateSchemaCase24514Action

}//namespace ChemSW.Nbt.Schema