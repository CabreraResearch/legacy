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
            DataTable LandingPageTable = getLandingPageTable( Request.RoleId, Request.ActionId );
            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                CswNbtLandingPageItem Item = CswNbtLandingPageItemFactory.makeLandingPageItem( _CswNbtResources, LandingPageRow["componenttype"].ToString() );
                Item.setItemData( LandingPageRow );
                Items.LandingPageItems.Add( Item.ItemData );
                                }
            return Items;
        }

        private DataTable getLandingPageTable( string RoleId, string ActionId )
        {
            if( RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( RoleId );
            Int32 PkRoleId = RolePk.PrimaryKey;

            CswTableSelect LandingPageSelect = _CswNbtResources.makeCswTableSelect( "LandingPageSelect", "landingpage" );
            string WhereClause;
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                WhereClause = "where for_actionid = '" + ActionId + "'";
            }
            else
            {
                WhereClause = "where for_roleid = '" + PkRoleId.ToString() + "' and for_actionid is null";
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

        public void moveLandingPageItem( LandingPageData.Request Request )
        {
            if( Request.LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "MoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", Request.LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    DataRow LandingPageRow = LandingPageTable.Rows[0];
                    LandingPageRow["display_row"] = CswConvert.ToDbVal( Request.NewRow );
                    LandingPageRow["display_col"] = CswConvert.ToDbVal( Request.NewColumn );
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }

        public void deleteLandingPageItem( LandingPageData.Request Request )
        {
            if( Request.LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "RemoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", Request.LandingPageId );
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
