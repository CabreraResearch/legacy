using ChemSW.Core;
using ChemSW.DB;
using System.Data;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27780
    /// </summary>
    public class CswUpdateSchemaCase27780 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            //Delete the SI Configuration category and all the views in it

            //get the doomed view ids
            CswTableSelect node_viewsTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "deleteSIViews_27780", "node_views" );
            CswCommaDelimitedString selectCols = new CswCommaDelimitedString();
            selectCols.FromString( "nodeviewid" );
            DataTable node_views = node_viewsTS.getTable( selectCols, "where category = 'SI Configuration'" );

            //
            foreach( DataRow row in node_views.Rows )
            {
                int viewidInt = CswConvert.ToInt32( row["nodeviewid"] );
                CswNbtViewId viewid = new CswNbtViewId();
                viewid.set( viewidInt );
                if( viewid.isSet() )
                {
                    CswNbtView doomedView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( viewid );
                    doomedView.Delete();
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema