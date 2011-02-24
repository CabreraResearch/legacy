using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using ChemSW.CswWebControls;
using ChemSW.DB;

namespace ChemSW.Nbt.WebPages
{
    public partial class Welcome : System.Web.UI.Page
    {
        #region Page Lifecycle

        private Button HiddenRefreshButton;
        private HtmlGenericControl HiddenButtonDiv;

        protected override void OnInit( EventArgs e )
        { 
            try
            {
                // Clear main page context, so add links work
                SelectedNodeKey = null;
                Master.clearView();
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnInit( e );
        } // OnInit()


        protected override void CreateChildControls()
        {
            CswWelcomeTable WelcomeTable = new CswWelcomeTable( Master.CswNbtResources, Master.AjaxManager );
            WelcomeTable.ID = "WelcomeTable";
            WelcomeTable.OnError += new CswErrorHandler( Master.HandleError );
            WelcomeTable.OnViewLinkClick += new CswWelcomeTable.ViewLinkClickHandler( WelcomeTable_OnViewLinkClick );
            WelcomeTable.OnSearchClick += new CswWelcomeTable.SearchClickHandler( WelcomeTable_OnSearchClick );
            WelcomeTable.OnAddedClick += new CswWelcomeTable.AddedClickHandler( WelcomeTable_OnAddedClick );
            centerph.Controls.Add( WelcomeTable );

            HiddenButtonDiv = new HtmlGenericControl( "div" );
            HiddenButtonDiv.ID = "HiddenDeleteButtonDiv";
            HiddenButtonDiv.Style.Add( "display", "none" );
            centerph.Controls.Add( HiddenButtonDiv );

            HiddenRefreshButton = new Button();
            HiddenRefreshButton.ID = "HiddenRefreshButton";
            HiddenRefreshButton.CssClass = "Button";
            HiddenRefreshButton.Text = "Refresh";
            HiddenRefreshButton.Click += new EventHandler( HiddenRefreshButton_Click );
            HiddenButtonDiv.Controls.Add( HiddenRefreshButton ); 
            
            base.CreateChildControls();
        } // CreateChildControls()

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
        }
        #endregion Page Lifecycle


        #region Events
        
        protected void HiddenRefreshButton_Click( object sender, EventArgs e )
        {
        }

        void WelcomeTable_OnViewLinkClick( CswViewListTree.ViewType ViewType, Int32 Pk )
        {
            Master.ChangeMainView( ViewType, Pk );
        }

        void WelcomeTable_OnSearchClick( string ViewString )
        {
            Master.setViewXml( ViewString );
            Master.Redirect( "Search.aspx" );
        }
        void WelcomeTable_OnAddedClick()
        {
            CswNbtNode NewNode = Master.CswNbtResources.Nodes[NewNodeId];
            
            CswNbtView NewNodeView = new CswNbtView( Master.CswNbtResources );
            NewNodeView.ViewName = "New " + NewNode.NodeType.NodeTypeName.ToString();
            CswNbtViewRelationship RootRel = NewNodeView.AddViewRelationship( NewNode.NodeType, false );
            RootRel.NodeIdsToFilterIn.Add( NewNode.NodeId );

            ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( NewNodeView, true, true, false, false );
            Tree.goToNthChild( 0 );
            SelectedNodeKey = Tree.getNodeKeyForCurrentPosition();

            Master.setViewXml( NewNodeView.ToString() );
            Master.GoMain();
        }

        #endregion Events


        private CswNbtNodeKey SelectedNodeKey
        {
            get
            {
                CswNbtNodeKey ret = null;
                if( Session["Main_SelectedNodeKey"] != null )
                {
                    ret = new CswNbtNodeKey( Master.CswNbtResources, Session["Main_SelectedNodeKey"].ToString() );
                }
                return ret;
            }
            set
            {
                if( value != null )
                    Session["Main_SelectedNodeKey"] = value.ToString();
                else
                    Session.Remove( "Main_SelectedNodeKey" );
            }
        }

        private CswPrimaryKey NewNodeId
        {
            get { return (CswPrimaryKey) Session["NewNodeId"]; }
            set { Session["NewNodeId"] = value; }
        }

    } // class Welcome
} // namespace ChemSW.Nbt.WebPages
