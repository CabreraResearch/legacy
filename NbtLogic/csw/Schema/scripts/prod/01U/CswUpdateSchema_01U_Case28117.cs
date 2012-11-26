using ChemSW.Nbt.csw.Dev;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using System.Collections.Generic;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28117
    /// </summary>
    public class CswUpdateSchema_01U_Case28117 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28117; }
        }

        public override void update()
        {
            #region part 1 - move the global Locations view to the System category
            List<CswNbtView> views = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Locations" );
            foreach( CswNbtView view in views )
            {
                if( string.IsNullOrEmpty( view.Category ) && view.Visibility.Equals( NbtViewVisibility.Global ) && view.ViewMode.Equals( NbtViewRenderingMode.Tree ) )
                {
                    view.Category = "System";
                    view.save();
                }
            }
            #endregion

            #region part 2 - Move Deficient Inspections and Lab 1 Deficiencies Report nodes into Lab Safety (demo) category

            CswTableSelect nodesTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "fixReportNodes_28117", "nodes" );
            DataTable nodesDT = nodesTS.getTable( "where nodename = 'Deficient Inspections' or nodename = 'Lab 1 Deficiencies'" );

            foreach( DataRow row in nodesDT.Rows )
            {
                int pkAsInt = CswConvert.ToInt32(row["nodeid"]);
                CswPrimaryKey pk = new CswPrimaryKey( "nodes", pkAsInt );
                CswNbtObjClassReport reportNode = _CswNbtSchemaModTrnsctn.Nodes.GetNode( pk );
                reportNode.Category.Text = "Lab Safety (demo)";
                reportNode.postChanges( false );
            }

            #endregion

            #region part 3 - moving more views around

            //views that belong in the Containers category (Expiring Containers, Missing Containers, Above Maximum Inventory, Below Minimum Inventory)
            CswTableSelect containersTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "conainerViews_28117", "node_views" );
            DataTable containersDT = containersTS.getTable( "where viewname = 'Expiring Containers' or viewname = 'Missing Containers' or viewname = 'Above Maximum Inventory' or viewname = 'Below Minimum Inventory' or viewname = 'Containers'" );

            foreach( DataRow row in containersDT.Rows )
            {
                CswNbtViewId viewId = new CswNbtViewId( CswConvert.ToInt32( row["nodeviewid"] ) );
                CswNbtView containerView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( viewId );
                containerView.Category = "Containers";
                containerView.save();
            }

            #endregion
        }

        //Update()

    }//class CswUpdateSchemaCase28117

}//namespace ChemSW.Nbt.Schema