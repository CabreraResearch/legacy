using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29882
    /// </summary>
    public class CswUpdateSchema_02C_Case29882 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29882; }
        }

        public override void update()
        {
            List<CswNbtView> ComponentsViews = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Components", CswEnumNbtViewVisibility.Global );
            foreach( CswNbtView ComponentsView in ComponentsViews )
            {
                ComponentsView.Delete();
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29882

}//namespace ChemSW.Nbt.Schema