using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26323
    /// </summary>
    public class CswUpdateSchemaCase26323 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // fix category on Lab Safety (demo) views
            CswNbtView aView = null;
            string[] viewNames = { "Lab Safety By Location", "Due Lab Safety Inspections", "Completed Lab Safety Inspections", "Action Required Lab Safety Inspections" };

            foreach( string astr in viewNames )
            {
                List<CswNbtView> theViewList = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( astr );
                if( theViewList.Count > 0 )
                {
                    aView = theViewList[0];
                    aView.Category = "Lab Safety";
                    aView.save();
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26323

}//namespace ChemSW.Nbt.Schema