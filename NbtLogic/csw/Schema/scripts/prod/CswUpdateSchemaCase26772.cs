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

            //Get all views with a category of "Lab Safety"
            //CswTableUpdate nodeViewsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "select_by_category_26772", "node_views" );
            //DataTable nodeViews = nodeViewsTU.getTable( "category" );
            //foreach( DataRow row in nodeViews.Rows )
            //{
            //    if( row["category"].Equals( "Lab Safety" ) )
            //    {
            //        row["viewname"] = row["viewname"] + " (demo)";
            //        row["category"] = "Lab Safety (demo)";
            //    }
            //}
            //nodeViewsTU.update( nodeViews );

            //CswTableUpdate nodesTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "select_demo_nodes_26772", "nodes" );
            //DataTable demoNodes = nodesTU.getTable( "isdemo", 1 );
            //foreach( DataRow row in demoNodes.Rows )
            //{
            //    row["nodename"] += " (demo)";
            //}



        }//Update()

    }//class CswUpdateSchemaCase26772

}//namespace ChemSW.Nbt.Schema