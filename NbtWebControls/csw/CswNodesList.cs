using System;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// Displays a list of selectable nodes
    /// </summary>
    public class CswNodesList : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        private CswAutoTable _Table;
        private Int32 ListLinkCellNumber = 2;

        /// <summary>
        /// View which determines what nodes to display in the List
        /// </summary>
        public CswNbtView View = null;

        /// <summary>
        /// Tree of nodes to display in the List
        /// </summary>
        //public ICswNbtTree Tree = null;
        /// <summary>
        /// Which node should be selected
        /// </summary>
        public CswNbtNodeKey SelectedNodeKey = null;

        /// <summary>
        /// If true, each node entry in the list will be a link
        /// </summary>
        public bool EnableLinks = true;

        /// <summary>
        /// Name of client-side function for clicking a node (node key will be passed in as parameter).  EnableLinks must be true.
        /// </summary>
        public string ClientClickFunctionName = string.Empty;

        /// <summary>
        /// Force a reload of the View's tree
        /// </summary>
        public bool ForceReload = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNodesList( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            this.DataBinding += new EventHandler( CswNodesList_DataBinding );
        }

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                _Table = new CswAutoTable();
                _Table.ID = "NodesListTable";
                _Table.CssClass = "ListTable";
                _Table.EvenRowCssClass = "ListRowEven";
                _Table.OddRowCssClass = "ListRowOdd";
                _Table.SelectedRowCssClass = "ListRowSelected";
                this.Controls.Add( _Table );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.CreateChildControls();
        }

        /// <summary>
        /// DataBind to Tree
        /// </summary>
        protected void CswNodesList_DataBinding( object sender, EventArgs e )
        {
            try
            {
                EnsureChildControls();
                _Table.clear();

                bool NoResults = false;
                if( View == null )
                    NoResults = true;
                else
                {
                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, false );
                    if( Tree == null )
                        NoResults = true;
                    else
                    {
                        if( Tree.getChildNodeCount() == 0 )
                            NoResults = true;
                        else
                        {
                            if( SelectedNodeKey != null )
                            {
                                Tree.makeNodeCurrent( SelectedNodeKey );
                                if( !Tree.isCurrentNodeDefined() || SelectedNodeKey.NodeSpecies == NodeSpecies.Root )
                                    SelectedNodeKey = null;
                                Tree.goToRoot();
                            }

                            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                            {
                                Tree.goToNthChild( c );

                                CswNbtNodeKey CurrentNodeKey = Tree.getNodeKeyForCurrentPosition();
                                Int32 ThisRow = _Table.Rows.Count;

                                Image IconImage = new Image();
                                string IconSuffix = Tree.getNodeForCurrentPosition().IconFileName;
                                string IconName = default( string );
                                if( false == string.IsNullOrEmpty( IconSuffix ) )
                                {
                                    IconName = "Images/icons/" + IconSuffix;
                                }

                                IconImage.ImageUrl = IconName;
                                _Table.addControl( ThisRow, 0, IconImage );

                                _Table.addControl( ThisRow, 1, new CswLiteralNbsp() );

                                if( EnableLinks )
                                {
                                    LinkButton ListLink = new LinkButton();
                                    ListLink.ID = Tree.getNodeKeyForCurrentPosition().ToString();
                                    ListLink.Text = Tree.getNodeNameForCurrentPosition();
                                    ListLink.OnClientClick = "return " + ClientClickFunctionName + "('" + Tree.getNodeKeyForCurrentPosition().ToString() + "');";
                                    _Table.addControl( ThisRow, ListLinkCellNumber, ListLink );
                                    _Table.getCell( ThisRow, ListLinkCellNumber ).Width = Unit.Parse( "100%" );
                                }
                                else
                                {
                                    Label ListLabel = new Label();
                                    ListLabel.ID = Tree.getNodeKeyForCurrentPosition().ToString();
                                    ListLabel.Text = Tree.getNodeNameForCurrentPosition();
                                    _Table.addControl( ThisRow, ListLinkCellNumber, ListLabel );
                                    _Table.getCell( ThisRow, ListLinkCellNumber ).Width = Unit.Parse( "100%" );
                                }

                                if( ( SelectedNodeKey == null && c == 0 ) || // first row
                                    CurrentNodeKey == SelectedNodeKey )      // or selected row
                                {
                                    _Table.SelectedRow = ThisRow;
                                }

                                Tree.goToParentNode();

                            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                        } // if-else( Tree.getChildNodeCount() == 0 )
                    } // if-else( Tree == null )
                } // if-else( View == null )


                if( NoResults )
                {
                    NoResults = true;
                    Label NoResultsLabel = new Label();
                    NoResultsLabel.Text = "No results";
                    _Table.addControl( 0, 0, NoResultsLabel );
                    _Table.SelectedRow = 0;
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        //private Int32 _findListRowNumberByNodeId( CswAutoTable _MainList, CswNbtNodeKey NodeKey )
        //{
        //    Int32 ret = Int32.MinValue;
        //    for( Int32 r = 0; r < _MainList.Rows.Count; r++ )
        //    {
        //        if( ( (WebControl) _MainList.Rows[r].Cells[ListLinkCellNumber].Controls[0] ).ID == NodeKey.NodeId.ToString() )
        //            ret = r;
        //    }
        //    return ret;
        //}

        /// <summary>
        /// Error handler
        /// </summary>
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
