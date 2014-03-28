using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.LandingPage
{
    public abstract class CswNbtLandingPageItem
    {
        protected CswNbtResources _CswNbtResources;        

        protected CswTableUpdate _LandingPageUpdate;
        protected DataTable _LandingPageTable;

        protected DataRow _ItemRow;
        public DataRow ItemRow { get { return _ItemRow; } }

        protected LandingPageData.LandingPageItem _ItemData;
        public LandingPageData.LandingPageItem ItemData { get { return _ItemData; } }

        protected CswNbtLandingPageItem( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ItemData = new LandingPageData.LandingPageItem();
            initEmptyLandingPageTable();
        }

        public void initEmptyLandingPageTable()
        {
            _LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
            _LandingPageTable = _LandingPageUpdate.getEmptyTable();
            _ItemRow = _LandingPageTable.NewRow();
        }

        public abstract void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request );        

        protected virtual void _setCommonItemDataForUI( DataRow LandingPageRow )
        {
            _ItemData.LandingPageId = LandingPageRow["landingpageid"].ToString();
            _ItemData.LinkType = LandingPageRow["componenttype"].ToString();
            _ItemData.DisplayRow = LandingPageRow["display_row"].ToString();
            _ItemData.DisplayCol = LandingPageRow["display_col"].ToString();
            
            // this intentionally overrides values set elsewhere
            string buttonIcon = LandingPageRow["buttonicon"].ToString();
            if( false == string.IsNullOrEmpty( buttonIcon ) )
            {
                _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + buttonIcon;
            }
        } // _setCommonItemDataForUI()

        public abstract void setDBValuesFromRequest( LandingPageData.Request Request );
        protected virtual void _setCommonDbValuesFromRequest( LandingPageData.Request Request )
        {
            _setCommonItemDataForDBImpl( Request.RoleId, Request.ActionId, Request.Type, Request.NewColumn, Request.NewRow, Request.Text, Request.ButtonIcon );
        }


        public abstract void setDBValuesFromExistingLandingPageItem( string RoleId, LandingPageData.LandingPageItem Item );
        protected virtual void _setCommonDBValuesFromExistingLandingPageItem( string RoleId, LandingPageData.LandingPageItem Item )
        {
            //when we receive the string, it has the full path already calculated, but we just want the icon name
            string buttonIcon = Item.ButtonIcon;
            if( Item.ButtonIcon.LastIndexOf( '/' ) > -1 )
            {
                buttonIcon = Item.ButtonIcon.Substring( Item.ButtonIcon.LastIndexOf( '/' ) );
            }
            _setCommonItemDataForDBImpl( RoleId, Item.ActionId, Item.LinkType, CswConvert.ToInt32( Item.DisplayCol ), CswConvert.ToInt32( Item.DisplayRow ), Item.Text, buttonIcon );
        }

        private void _setCommonItemDataForDBImpl( string RoleId, string ActionId, string LinkType, Int32 NewColumn, Int32 NewRow, string Text, string ButtonIcon )
        {
            int NumericRoleId = Int32.MinValue;

            if( RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = CswConvert.ToPrimaryKey( RoleId );
            if( null != RolePk )
            {
                NumericRoleId = RolePk.PrimaryKey;
            }
            if( ButtonIcon == "blank.gif" )
            {
                ButtonIcon = string.Empty;
            }


            if( Int32.MinValue != NumericRoleId )
            {
                _ItemRow["for_roleid"] = NumericRoleId;
            }
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                _ItemRow["for_actionid"] = ActionId;
            }
            _ItemRow["componenttype"] = LinkType;
            if( NewColumn <= 0 )
            {
                _ItemRow["display_col"] = "1";
            }
            else
            {
                _ItemRow["display_col"] = NewColumn;
            }
            if( NewRow <= 0 )
            {
                _ItemRow["display_row"] = _getNextAvailableRowForItem( NumericRoleId, ActionId );
            }
            else
            {
                _ItemRow["display_row"] = NewRow;
            }
            _ItemRow["displaytext"] = Text;
            _ItemRow["buttonicon"] = ButtonIcon;
        }


        private Int32 _getNextAvailableRowForItem( Int32 RoleId, string ActionId )
        {
            String SqlText = "select max(display_row) maxrow from landingpage where display_col = 1 ";
            String ActionClause = " and (for_actionid = :actionid )";
            String RoleClause = " and (for_roleid = :roleid )";
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                SqlText += ActionClause;
            }
            else//TODO - if and when Action Landing Pages are Role-specific, remove this else clause
            {
                SqlText += RoleClause;
            }
            
            CswArbitrarySelect LandingPageSelect = _CswNbtResources.makeCswArbitrarySelect( "LandingPageForRole", SqlText );
            LandingPageSelect.addParameter( "actionid", ActionId );
            LandingPageSelect.addParameter( "roleid", RoleId.ToString() );
            
            DataTable LandingPageSelectTable = LandingPageSelect.getTable();
            Int32 MaxRow = 0;
            if( LandingPageSelectTable.Rows.Count > 0 )
            {
                MaxRow = CswConvert.ToInt32( LandingPageSelectTable.Rows[0]["maxrow"] );
                if( MaxRow < 0 ) MaxRow = 0;
            }
            return MaxRow + 1;
        }

        public virtual void saveToDB()
        {
            _LandingPageTable.Rows.Add( _ItemRow );
            _LandingPageUpdate.update( _LandingPageTable );
        }
    }
}
