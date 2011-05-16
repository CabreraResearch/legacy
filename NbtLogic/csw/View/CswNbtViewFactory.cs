using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Creates Views of any type
    /// </summary>
    public class CswNbtViewFactory
    {
        /// <summary>
        /// Restore a CswNbtViewBase that was saved as a string
        /// </summary>
        /// <param name="CswNbtResources">A CswNbtResources object</param>
        /// <param name="ViewAsString">View saved as a string</param>
        public static CswNbtView restoreView( CswNbtResources CswNbtResources, string ViewAsString )
        {
            CswNbtView Ret = null;

            if( ViewAsString != string.Empty )
            {
                CswNbtView RelationshipView = new CswNbtView( CswNbtResources );
                RelationshipView.LoadXml( ViewAsString );
                Ret = RelationshipView;
            }
            return Ret;
        }


        public static CswNbtView restoreView( CswNbtResources CswNbtResources, Int32 ViewId )
        {
            // try cache first
			CswNbtView ReturnVal = null; // CswNbtResources.ViewCache.getView( ViewId );
			//if( ReturnVal == null )
			//{
                CswTableSelect ViewsTableSelect = CswNbtResources.makeCswTableSelect( "restoreView_select", "node_views" );
                DataTable ViewTable = ViewsTableSelect.getTable( "nodeviewid", ViewId );
                if( ViewTable.Rows.Count > 0 )
                {
                    string ViewAsString = ViewTable.Rows[0]["viewxml"].ToString();
                    ReturnVal = restoreView( CswNbtResources, ViewAsString );
                    ReturnVal.ViewId = ViewId;  // BZ 8068

                    // Override XML values with values from row
                    ReturnVal.Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), ViewTable.Rows[0]["visibility"].ToString() );
                    ReturnVal.VisibilityRoleId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["roleid"] ) );
                    ReturnVal.VisibilityUserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ViewTable.Rows[0]["userid"] ) );
                    ReturnVal.Category = ViewTable.Rows[0]["category"].ToString();
                    ReturnVal.ViewName = ViewTable.Rows[0]["viewname"].ToString();
                }
            //}
            return ( ReturnVal );

        }//restoreView()

    }
}
