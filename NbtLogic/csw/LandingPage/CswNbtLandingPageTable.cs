using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using System.Collections.ObjectModel;

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

        public DataTable getLandingPageTable( Int32 RoleId, string ActionId )
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
        public void addLandingPageItem( string Type, string ViewType, string PkValue, Int32 NodeTypeId, string DisplayText, string strRoleId, string ActionId )
        {
            if( strRoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                strRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;
            LandingPageItemType itemType;
            Enum.TryParse( Type, true, out itemType );
            CswNbtView.ViewType RealViewType = (CswNbtView.ViewType) ViewType;

            CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getEmptyTable();
            _addLandingPageItem( LandingPageTable, itemType, RealViewType, PkValue, NodeTypeId, DisplayText, RoleId, ActionId );
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

            switch( ItemType )
            {
                case LandingPageItemType.Add:
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewLandingPageRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                        NewLandingPageRow["buttonicon"] = ButtonIcon;
                        NewLandingPageRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add LandingPage Item" );
                    break;
                case LandingPageItemType.Link:
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
                    NewLandingPageRow["buttonicon"] = ButtonIcon;
                    NewLandingPageRow["displaytext"] = DisplayText;
                    break;
                case LandingPageItemType.Text:
                    if( DisplayText != string.Empty )
                    {
                        NewLandingPageRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text LandingPage Item" );
                    break;
            }
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
