using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    //public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue, LowRes };

    public class CswPropertyTableJQuery : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        private Int32 _selectedTabOrdinal = Int32.MinValue;

        public CswNbtNode SelectedNode;
        public Int32 NodeTypeId;
        public NodeEditMode EditMode;
        public string SelectedTabId;
        public CswNbtView View;

        private CswNbtMetaDataNodeType NodeType
        {
            get
            {
                CswNbtMetaDataNodeType ThisNodeType = null;
                if( SelectedNode != null )
                    ThisNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNode.NodeTypeId );
                else if( NodeTypeId != Int32.MinValue)
                    ThisNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );

                return ThisNodeType;
            }
        }

        public CswPropertyTableJQuery( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            this.DataBinding += new EventHandler( CswPropertyTableJQuery_DataBinding );
        }

        protected override void OnInit( EventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnInit( e );
        }

        private HtmlGenericControl _TabStripDiv;
        protected override void CreateChildControls()
        {
            _TabStripDiv = new HtmlGenericControl( "div" );
            _TabStripDiv.ID = "PropertyTableTabStripDiv";
            this.Controls.Add( _TabStripDiv );

            base.CreateChildControls();
        }

        void CswPropertyTableJQuery_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            _TabStripDiv.Controls.Clear();  // Beware!
            _TabAnchors = new Dictionary<HtmlAnchor, HtmlGenericControl>();

            if( NodeType != null )
            {

                HtmlGenericControl UL = new HtmlGenericControl( "ul" );
                _TabStripDiv.Controls.Add( UL );

                Int32 tabcnt = 0;
                foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.NodeTypeTabs )
                {
                    HtmlGenericControl ThisLI = new HtmlGenericControl( "li" );
                    UL.Controls.Add( ThisLI );

                    HtmlAnchor ThisA = new HtmlAnchor();
                    //ThisA.HRef = "#" + _makeTabId( Tab );
                    ThisLI.Controls.Add( ThisA );

                    HtmlGenericControl ThisSpan = new HtmlGenericControl( "span" );
                    ThisA.Controls.Add( ThisSpan );

                    ThisSpan.Controls.Add( new CswLiteralText( Tab.TabName ) );

                    HtmlGenericControl ThisTabDiv = new HtmlGenericControl( "div" );
                    ThisTabDiv.ID = _makeTabId( Tab );
                    _TabStripDiv.Controls.Add( ThisTabDiv );

                    if( Tab.TabId.ToString() == SelectedTabId )
                        _selectedTabOrdinal = tabcnt;

                    _TabAnchors.Add( ThisA, ThisTabDiv );

                    tabcnt++;
                }
            }
        }

        private string _makeTabId( CswNbtMetaDataNodeTypeTab Tab )
        {
            string ret = string.Empty;
            if( SelectedNode != null )
                ret += SelectedNode.NodeId.ToString();
            ret += "_";
            ret += Tab.TabId.ToString();
            return ret;
        }

        private Dictionary<HtmlAnchor, HtmlGenericControl> _TabAnchors;

        public void initTabStrip()
        {
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                foreach( HtmlAnchor Anchor in _TabAnchors.Keys )
                {
                    Anchor.HRef = "#" + _TabAnchors[Anchor].ClientID;
                }

                String JS = @"  <script language=""Javascript""> 
                                $(document).ready(function() {
                                    var tabs = $('#" + _TabStripDiv.ClientID + @"').tabs({
                                        select: getTabContent
                                    });";
                // set selected tab
                if( _selectedTabOrdinal != Int32.MinValue )
                {
                    JS += "         tabs.tabs({ 'option', 'selected', " + _selectedTabOrdinal.ToString() + " });";
                }
                // trigger the ajax load of the selected tab
                JS += @"            getTabContentAjax($('#" + _TabStripDiv.ClientID + @" a'));
                                });
                            </script> ";

                System.Web.UI.ScriptManager.RegisterStartupScript( this, this.GetType(), this.UniqueID + "_JS", JS, false );

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }


        // Error handling
        //public delegate void ErrorHandler( Exception ex );
        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }


    } // class CswPropertyTableJQuery
} // namespace
