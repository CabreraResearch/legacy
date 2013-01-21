using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28395
    /// </summary>
    public class CswUpdateSchema_01W_Case28395 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28395; }
        }

        public override void update()
        {
            //Add TierII Reporting Action
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Tier_II_Reporting, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Tier_II_Reporting );
        }//Update()

    }//class CswUpdateSchemaCase_01W_28395

}//namespace ChemSW.Nbt.Schema