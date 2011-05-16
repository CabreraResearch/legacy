using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Actions;

namespace ChemSW.NbtWebControls
{
    public class CswQuickLaunch : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        private CswAutoTable _QuickLaunchTable;

        private Hashtable ViewsInQuickLaunch = null;
        private Hashtable ActionIdsInQuickLaunch = null;

        public object PreviousView1;
        public object PreviousView2;
        public object PreviousView3;
        public object PreviousView4;
        public object PreviousView5;

        public CswQuickLaunch( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();
            base.OnInit( e );
        }

        protected override void CreateChildControls()
        {
            _QuickLaunchTable = new CswAutoTable();
            _QuickLaunchTable.ID = "QuickLaunchTable";
            _QuickLaunchTable.CssClass = "QuickLaunchTable";
            this.Controls.Add( _QuickLaunchTable );

            base.CreateChildControls();
        }

        public void Clear()
        {
            _QuickLaunchTable.Controls.Clear();
            ViewsInQuickLaunch = new Hashtable();
            ActionIdsInQuickLaunch = new Hashtable();
        }

        public override void DataBind()
        {
            if( _CswNbtResources.CurrentUser != null && _CswNbtResources.CurrentNbtUser.UserNode != null )
            {
                Clear();

                CswNbtNodePropViewPickList UsersViews = ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchViews;
                CswNbtNodePropLogicalSet UsersActions = ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchActions;

                // First user specified ones
                Collection<Int32> SelectedViewIds = UsersViews.SelectedViewIds.ToIntCollection();
                foreach( Int32 ViewId in SelectedViewIds )
                {
                    if( ViewId > 0 )
                    {
                        CswNbtView ThisView = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                        _AddQuickLaunchLinkView( ThisView, true );
                    }
                }
                DataTable ActionTable = UsersActions.GetDataAsTable( "actionname", "actionid" );
                foreach( int ThisActionId in from DataRow ActionRow in ActionTable.Rows where CswConvert.ToBoolean( ActionRow["Include"] ) select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow["actionname"].ToString() )].ActionId into ThisActionId where ThisActionId > 0 select ThisActionId )
                {
                    _AddQuickLaunchLinkAction( ThisActionId, true );
                }

                // Now recent ones
                if( PreviousView1 != null )
                {
                    if( PreviousView1 is CswNbtView )
                        _AddQuickLaunchLinkView( (CswNbtView) PreviousView1, false );
                    else
                        _AddQuickLaunchLinkAction( (Int32) PreviousView1, false );
                }
                if( PreviousView2 != null )
                {
                    if( PreviousView2 is CswNbtView )
                        _AddQuickLaunchLinkView( (CswNbtView) PreviousView2, false );
                    else
                        _AddQuickLaunchLinkAction( (Int32) PreviousView2, false );
                }
                if( PreviousView3 != null )
                {
                    if( PreviousView3 is CswNbtView )
                        _AddQuickLaunchLinkView( (CswNbtView) PreviousView3, false );
                    else
                        _AddQuickLaunchLinkAction( (Int32) PreviousView3, false );
                }
                if( PreviousView4 != null )
                {
                    if( PreviousView4 is CswNbtView )
                        _AddQuickLaunchLinkView( (CswNbtView) PreviousView4, false );
                    else
                        _AddQuickLaunchLinkAction( (Int32) PreviousView4, false );
                }
                if( PreviousView5 != null )
                {
                    if( PreviousView5 is CswNbtView )
                        _AddQuickLaunchLinkView( (CswNbtView) PreviousView5, false );
                    else
                        _AddQuickLaunchLinkAction( (Int32) PreviousView5, false );
                }

            } // if( _CswNbtResources.CurrentUser != null && _CswNbtResources.CurrentUser.UserNode != null )

            base.DataBind();
        } // DataBind()


        private void _AddQuickLaunchLinkView( CswNbtView View, bool Sticky )
        {
            if( View != null )   // BZ 8551 #11
            {
                if( ( View.ViewId > 0 && !ViewsInQuickLaunch.ContainsKey( View.ViewId ) ) ||
                    ( View.ViewId <= 0 && View.SessionViewId.isSet() && !ViewsInQuickLaunch.ContainsKey( View.SessionViewId ) ) )
                {
                    _AddQuickLaunchHeader();

                    //Alphabetical insertion sort - Find where to insert this row
                    Int32 row = _QuickLaunchTable.Rows.Count;
                    for( int r = 1; r < _QuickLaunchTable.Rows.Count; r++ )
                    {
                        LinkButton OtherViewLink = _QuickLaunchTable.Rows[r].Cells[0].Controls[1] as LinkButton;
                        if( OtherViewLink.Text.CompareTo( View.ViewName ) > 0 )
                        {
                            row = r;
                            break;
                        }
                    }
                    _QuickLaunchTable.insertRow( row );

                    Literal BulletLiteral = new Literal();
                    BulletLiteral.Text = "&bull;&nbsp;";
                    _QuickLaunchTable.addControl( row, 0, BulletLiteral );

                    LinkButton ViewLink = new LinkButton();
                    if( View.ViewId > 0 )
                    {
                        ViewLink.ID = "ViewLink_" + View.ViewId;
                        ViewLink.Click += new EventHandler( ViewLink_Click );
                        ViewsInQuickLaunch.Add( View.ViewId, View );
                    }
                    else
                    {
                        ViewLink.ID = "SessionViewLink_" + View.SessionViewId;
                        ViewLink.Click += new EventHandler( SessionViewLink_Click );
                        ViewsInQuickLaunch.Add( View.SessionViewId, View );
                    }
                    ViewLink.Text = View.ViewName;
                    _QuickLaunchTable.addControl( row, 0, ViewLink );

                    _QuickLaunchTable.addControl( row, 1, new CswLiteralNbsp() );
                }
            } 
        }

        private void _AddQuickLaunchLinkAction( Int32 ActionId, bool Sticky )
        {
            if( !ActionIdsInQuickLaunch.ContainsKey( ActionId ) )
            {
                _AddQuickLaunchHeader();

                //Alphabetical insertion sort - Find where to insert this row
                Int32 row = _QuickLaunchTable.Rows.Count;
                for( int r = 1; r < _QuickLaunchTable.Rows.Count; r++ )
                {
                    LinkButton OtherViewLink = _QuickLaunchTable.Rows[r].Cells[0].Controls[1] as LinkButton;
                    if( OtherViewLink.Text.CompareTo( _CswNbtResources.Actions[ActionId].Name.ToString() ) > 0 )
                    {
                        row = r;
                        break;
                    }
                }
                _QuickLaunchTable.insertRow( row );

                Literal BulletLiteral = new Literal();
                BulletLiteral.Text = "&bull;&nbsp;";
                _QuickLaunchTable.addControl( row, 0, BulletLiteral );

                LinkButton ActionLink = new LinkButton();
                ActionLink.ID = "ActionLink_" + ActionId.ToString();
                ActionLink.Text = _CswNbtResources.Actions[ActionId].Name.ToString();
                ActionLink.Click += new EventHandler( ActionLink_Click );
                _QuickLaunchTable.addControl( row, 0, ActionLink );

                ActionIdsInQuickLaunch.Add( ActionId, ActionId );
            }
        }

        private bool QuickLaunchHeaderAdded = false;
        private void _AddQuickLaunchHeader()
        {
            if( !QuickLaunchHeaderAdded )
            {
                Literal QuickLaunchLiteral = new Literal();
                QuickLaunchLiteral.Text = "Quick Launch:";
                _QuickLaunchTable.addControl( 0, 0, QuickLaunchLiteral );

                HtmlGenericControl ConfigButtonDiv = new HtmlGenericControl( "div" );
                ConfigButtonDiv.Style.Add( HtmlTextWriterStyle.TextAlign, "right" );
                _QuickLaunchTable.addControl( 0, 1, ConfigButtonDiv );

                CswImageButton ConfigButton = new CswImageButton( CswImageButton.ButtonType.Configure );
                ConfigButton.OnClientClick = "CswQuickLaunch_openConfigPopup(); return false;";
                ConfigButtonDiv.Controls.Add( ConfigButton );

                QuickLaunchHeaderAdded = true;
            }
        }

        public delegate void ViewLinkClickEvent( Int32 ViewId );
		public delegate void SessionViewLinkClickEvent( CswNbtSessionViewId SessionViewId );
        public delegate void ActionLinkClickEvent( Int32 ActionId );

        public event SessionViewLinkClickEvent OnSessionViewLinkClick = null;
        public event ViewLinkClickEvent OnViewLinkClick = null;
        public event ActionLinkClickEvent OnActionLinkClick = null;

        protected void SessionViewLink_Click( object sender, EventArgs e )
        {
			if( OnSessionViewLinkClick != null )
				OnSessionViewLinkClick( new CswNbtSessionViewId( CswConvert.ToInt32( ( (LinkButton) sender ).ID.Substring( "SessionViewLink_".Length ) ) ) );
        }
        protected void ViewLink_Click( object sender, EventArgs e )
        {
            if( OnViewLinkClick != null )
                OnViewLinkClick( CswConvert.ToInt32( ( (LinkButton) sender ).ID.Substring( "ViewLink_".Length ) ) );
        }
        protected void ActionLink_Click( object sender, EventArgs e )
        {
            if( OnActionLinkClick != null )
                OnActionLinkClick( CswConvert.ToInt32( ( (LinkButton) sender ).ID.Substring( "ActionLink_".Length ) ) );
        }

        public event CswErrorHandler OnError;

        void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }
    }
}
