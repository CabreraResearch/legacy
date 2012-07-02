using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26772
    /// </summary>
    public class CswUpdateSchemaCase26772 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //Get all views with a category of "Lab Safety" and add '(demo)' to 'viewname' and 'category'
            CswTableSelect nodeViewsTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "select_by_category_26772", "node_views" );
            DataTable nodeViews = nodeViewsTS.getTable( "where category = 'Lab Safety'" );
            foreach( DataRow row in nodeViews.Rows )
            {
                CswNbtView curView = _CswNbtSchemaModTrnsctn.restoreView( row["viewname"].ToString() );
                if( null != curView ) //paranoid
                {
                    curView.ViewName += " (demo)";
                    curView.Category += " (demo)";
                    curView.save();
                }
            }


        }//Update()

    }//class CswUpdateSchemaCase26772

}//namespace ChemSW.Nbt.Schema