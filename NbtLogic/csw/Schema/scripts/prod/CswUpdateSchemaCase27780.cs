using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

            #region PART 1 - Hide the settings tab on the Equipment Schedule NT

            List<int> ntpIdsToHide = new List<int>();

            //get the props from the Settings tab and then delete the tab on Equipment Schedule
            CswNbtMetaDataNodeType equipmentScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Schedule" );
            if( null != equipmentScheduleNT )
            {
                foreach( CswNbtMetaDataNodeTypeTab tab in equipmentScheduleNT.getNodeTypeTabs() )
                {
                    if( tab.TabName.Equals( "Settings" ) )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp ntp in tab.getNodeTypeProps() )
                        {
                            ntpIdsToHide.Add( ntp.PropId );
                        }
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( tab );
                    }
                }
            }

            //get the props from the Settings tab and then delete the tab on Assembly Schedule
            CswNbtMetaDataNodeType assemblyScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Schedule" );
            if( null != assemblyScheduleNT )
            {
                foreach( CswNbtMetaDataNodeTypeTab tab in assemblyScheduleNT.getNodeTypeTabs() )
                {
                    if( tab.TabName.Equals( "Settings" ) )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp ntp in tab.getNodeTypeProps() )
                        {
                            ntpIdsToHide.Add( ntp.PropId );
                        }
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( tab );
                    }
                }
            }

            //hide the props
            foreach( int ntpId in ntpIdsToHide )
            {
                CswNbtMetaDataNodeTypeProp ntp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ntpId );
                ntp.removeFromAllLayouts();
            }

            #endregion

            #region PART 2 - Delete the SI Configuration category and all the views in it

            //get the doomed view ids
            CswTableSelect node_viewsTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "deleteSIViews_27780", "node_views" );
            CswCommaDelimitedString selectCols = new CswCommaDelimitedString();
            selectCols.FromString( "nodeviewid" );
            DataTable node_views = node_viewsTS.getTable( selectCols, "where category = 'SI Configuration'" );

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

            #endregion

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema