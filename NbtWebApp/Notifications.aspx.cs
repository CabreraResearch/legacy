using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Notifications : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        private RadGrid _NotifyGrid;
        private DropDownList _NodeTypeList;
        private DropDownList _EventList;

        private static string _ObjectClassPrefix = "oc_";
        private static string _NodeTypePrefix = "nt_";

        protected override void CreateChildControls()
        {
            try
            {
                _NotifyGrid = new RadGrid();
                CswTableSelect NotifySelect = Master.CswNbtResources.makeCswTableSelect( "Notifications.aspx.cs::NotifySelect", "notify" );
                DataTable NotifyTable = NotifySelect.getTable();
                _NotifyGrid.DataSource = NotifyTable;
                _NotifyGrid.DataBind();
                //foreach( DataRow NotifyRow in NotifyTable.Rows )
                //{
                //    Int32 NodeTypeId = CswConvert.ToInt32( NotifyRow["nodetypeid"] );
                //    Int32 ObjectClassId = CswConvert.ToInt32( NotifyRow["objectclassid"] );
                //    if( NodeTypeId != Int32.MinValue )
                //    {
                //    }
                //    else if( ObjectClassId != Int32.MinValue )
                //    {

                //    }
                //}
                ph.Controls.Add( _NotifyGrid );

                CswAutoTable Table = new CswAutoTable();
                ph.Controls.Add( Table);

                Table.addControl( 0, 0, new CswLiteralText( "Add a new Notification:" ) );

                Table.addControl( 1, 0, new CswLiteralText( "When someone" ) );

                _NodeTypeList = new CswNodeTypeDropDown( Master.CswNbtResources );
                _NodeTypeList.ID = "NodeTypeList";
                _NodeTypeList.DataBind();
                Table.addControl( 1, 1, _NodeTypeList );

                _EventList = new DropDownList();
                _EventList.ID = "EventList";
                ph.Controls.Add( _EventList );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.CreateChildControls();
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnLoad( e );
        }

    } // class Notifications
} // namespace ChemSW.Nbt.WebPages
    
