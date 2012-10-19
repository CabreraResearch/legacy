using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageTable
    {

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Types of LandingPage Page Item
        /// </summary>
        public enum LandingPageItemType
        {
            /// <summary>
            /// Link to a View, Report, or Action
            /// </summary>
            Link,
            /// <summary>
            /// Static text
            /// </summary>
            Text,
            /// <summary>
            /// Link for Add New Node dialog
            /// </summary>
            Add
        }

        public CswNbtLandingPageTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public LandingPageData getLandingPageItems( LandingPageData.Request Request )
        {
            LandingPageData Items = new LandingPageData();

            if( Request.RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                Request.RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( Request.RoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            DataTable LandingPageTable = getLandingPageTable( RoleId, Request.ActionId );
            Dictionary<CswNbtViewId, CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, true, false, false, NbtViewRenderingMode.Any );

            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                string LandingPageId = LandingPageRow["landingpageid"].ToString();
                LandingPageData.LandingPageItem Item = new LandingPageData.LandingPageItem();
                Item.LandingPageId = LandingPageId;

                LandingPageItemType ThisType = (LandingPageItemType) Enum.Parse( typeof( LandingPageItemType ), LandingPageRow["componenttype"].ToString(), true );
                string LinkText = string.Empty;

                switch( ThisType )
                {
                    case LandingPageItemType.Add:
                        if( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) != Int32.MinValue )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) );
                            if( NodeType != null )
                            {
                                bool CanAdd = NodeType.getObjectClass().CanAdd && _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType );
                                if( CanAdd )
                                {
                                    if( LandingPageRow["displaytext"].ToString() != string.Empty )
                                        LinkText = LandingPageRow["displaytext"].ToString();
                                    else
                                        LinkText = "Add New " + NodeType.NodeTypeName;
                                    Item.NodeTypeId = NodeType.NodeTypeId.ToString();
                                    Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                                    Item.Type = "add_new_nodetype";
                                }
                            }
                        }
                        break;

                    case LandingPageItemType.Link:
                        if( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtViewId NodeViewId = new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"].ToString() ) );
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( NodeViewId );
                            if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.ContainsKey( ThisView.ViewId ) )
                            {
                                LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                                Item.ViewId = NodeViewId.ToString();
                                Item.ViewMode = ThisView.ViewMode.ToString().ToLower();
                                if( ThisView.Root.ChildRelationships[0] != null )
                                {
                                    if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootNT != null )
                                        {
                                            Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootNT.IconFileName;
                                        }
                                    }
                                    else if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.ObjectClassId )
                                    {
                                        CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootOC != null )
                                        {
                                            Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootOC.IconFileName;
                                        }
                                    }
                                }
                                Item.Type = "view";
                            }
                        }
                        if( CswConvert.ToInt32( LandingPageRow["to_actionid"] ) != Int32.MinValue )
                        {
                            CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( LandingPageRow["to_actionid"] )];
                            if( null != ThisAction )
                            {
                                if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                                {
                                    LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                                }
                                Item.ActionId = LandingPageRow["to_actionid"].ToString();
                                Item.ActionName = ThisAction.Name.ToString();
                                Item.ActionUrl = ThisAction.Url;
                                Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + "wizard.png";
                                Item.Type = "action";
                            }
                        }
                        if( CswConvert.ToInt32( LandingPageRow["to_reportid"] ) != Int32.MinValue )
                        {
                            CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( LandingPageRow["to_reportid"] ) )];
                            if( null != ThisReportNode )
                            {
                                LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisReportNode.NodeName;
                                int idAsInt = CswConvert.ToInt32( LandingPageRow["to_reportid"] );
                                CswPrimaryKey reportPk = new CswPrimaryKey( "nodes", idAsInt );
                                Item.ReportId = reportPk.ToString();
                                Item.Type = "report";
                                Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                            }
                        }
                        break;

                    case LandingPageItemType.Text:
                        LinkText = LandingPageRow["displaytext"].ToString();
                        break;

                } // switch( ThisType )

                if( LinkText != string.Empty )
                {
                    Item.LinkType = LandingPageRow["componenttype"].ToString();
                    Item.Text = LinkText;
                    Item.DisplayRow = LandingPageRow["display_row"].ToString();
                    Item.DisplayCol = LandingPageRow["display_col"].ToString();
                }
                Items.LandingPageItems.Add( Item );

            } // foreach( DataRow LandingPageRow in LandingPageTable.Rows )

            return Items;
        }

        private DataTable getLandingPageTable( Int32 RoleId, string ActionId )
        {
            CswTableSelect LandingPageSelect = _CswNbtResources.makeCswTableSelect( "LandingPageSelect", "landingpage" );
            string WhereClause;
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                WhereClause = "where for_actionid = '" + ActionId + "'";
            }
            else
            {
                WhereClause = "where for_roleid = '" + RoleId.ToString() + "' and for_actionid is null";
            }
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "landingpageid", OrderByType.Ascending ) );
            DataTable LandingPageTable = LandingPageSelect.getTable( WhereClause, OrderBy );
            return LandingPageTable;
        }

        /// <summary>
        /// Adds a new item to the LandingPage
        /// </summary>
        public void addLandingPageItem( LandingPageData.Request Request )
        {
            if( Request.RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                Request.RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( Request.RoleId );
            Int32 RoleId = RolePk.PrimaryKey;
            LandingPageItemType itemType;
            Enum.TryParse( Request.Type, true, out itemType );
            CswNbtView.ViewType RealViewType = (CswNbtView.ViewType) Request.ViewType;

            CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getEmptyTable();
            _addLandingPageItem( LandingPageTable, itemType, RealViewType, Request.PkValue, CswConvert.ToInt32( Request.NodeTypeId ), Request.Text, RoleId, Request.ActionId );
            LandingPageUpdate.update( LandingPageTable );
        }

        private void _addLandingPageItem( DataTable LandingPageTable, LandingPageItemType ItemType, CswNbtView.ViewType ViewType, string PkValue,
                                          Int32 NodeTypeId, string DisplayText, Int32 RoleId, string ActionId, string ButtonIcon = "" )
        {
            Int32 Row = _getNextAvailableRowForItem( RoleId, ActionId );
            Int32 Column = 1;

            if( ButtonIcon == "blank.gif" )
                ButtonIcon = string.Empty;

            DataRow NewLandingPageRow = LandingPageTable.NewRow();
            if( Int32.MinValue != RoleId )
            {
                NewLandingPageRow["for_roleid"] = RoleId;
            }
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                NewLandingPageRow["for_actionid"] = ActionId;
            }
            NewLandingPageRow["componenttype"] = ItemType.ToString();
            NewLandingPageRow["display_col"] = Column;
            NewLandingPageRow["display_row"] = Row;
            NewLandingPageRow["displaytext"] = DisplayText;
            NewLandingPageRow["buttonicon"] = ButtonIcon;

            switch( ItemType )
            {
                case LandingPageItemType.Add:
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewLandingPageRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add LandingPage Item" );
                    break;
                case LandingPageItemType.Link:
                    _setLinkValue( NewLandingPageRow, ViewType, PkValue );
                    break;
                case LandingPageItemType.Text:
                    NewLandingPageRow["buttonicon"] = String.Empty;
                    if( String.IsNullOrEmpty( DisplayText ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text LandingPage Item" );
                    }
                    break;
            }//switch
            LandingPageTable.Rows.Add( NewLandingPageRow );

        } // _AddLandingPageItem()

        private Int32 _getNextAvailableRowForItem( Int32 RoleId, string ActionId )
        {
            String ActionText = String.Empty;
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                ActionText = " and (for_actionid = " + ActionId + ")";
            }
            string SqlText = "select max(display_row) maxcol from landingpage where display_col = 1 and (for_roleid = " + RoleId.ToString() + ")" + ActionText;
            CswArbitrarySelect LandingPageSelect = _CswNbtResources.makeCswArbitrarySelect( "LandingPageForRole", SqlText );
            DataTable LandingPageSelectTable = LandingPageSelect.getTable();
            Int32 MaxRow = 0;
            if( LandingPageSelectTable.Rows.Count > 0 )
            {
                MaxRow = CswConvert.ToInt32( LandingPageSelectTable.Rows[0]["maxcol"] );
                if( MaxRow < 0 ) MaxRow = 0;
            }
            return MaxRow + 1;
        }

        private void _setLinkValue( DataRow NewLandingPageRow, CswNbtView.ViewType ViewType, string PkValue )
        {
            if( ViewType == CswNbtView.ViewType.View )
            {
                NewLandingPageRow["to_nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( PkValue ).get() );
            }
            else if( ViewType == CswNbtView.ViewType.Action )
            {
                NewLandingPageRow["to_actionid"] = CswConvert.ToDbVal( PkValue );
            }
            else if( ViewType == CswNbtView.ViewType.Report )
            {
                CswPrimaryKey ReportPk = new CswPrimaryKey();
                ReportPk.FromString( PkValue );
                Int32 PkVal = ReportPk.PrimaryKey;
                NewLandingPageRow["to_reportid"] = CswConvert.ToDbVal( PkVal );
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link LandingPage Item" );
            }
        }

        public void moveLandingPageItem( Int32 LandingPageId, Int32 NewRow, Int32 NewColumn )
        {
            if( LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "MoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    DataRow LandingPageRow = LandingPageTable.Rows[0];
                    LandingPageRow["display_row"] = CswConvert.ToDbVal( NewRow );
                    LandingPageRow["display_col"] = CswConvert.ToDbVal( NewColumn );
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }

        public void deleteLandingPageItem( Int32 LandingPageId )
        {
            if( LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "RemoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    foreach( DataRow LandingPageRow in LandingPageTable.Rows )
                    {
                        LandingPageRow.Delete();
                    }
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }
    }
}
