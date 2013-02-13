using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Core;
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

        public abstract void setItemDataForDB( LandingPageData.Request Request );

        protected virtual void _setCommonItemDataForDB( LandingPageData.Request Request )
        {
            Int32 RoleId = Int32.MinValue;
            if( Request.RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                Request.RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = CswConvert.ToPrimaryKey( Request.RoleId );
            if( null != RolePk )
            {
                RoleId = RolePk.PrimaryKey;
            }
            if( Request.ButtonIcon == "blank.gif" )
            {
                Request.ButtonIcon = string.Empty;
            }
            if( Int32.MinValue != RoleId )
            {
                _ItemRow["for_roleid"] = RoleId;
            }
            if( false == String.IsNullOrEmpty( Request.ActionId ) )
            {
                _ItemRow["for_actionid"] = Request.ActionId;
            }
            _ItemRow["componenttype"] = Request.Type;
            if( Request.NewColumn <= 0 )
            {
                _ItemRow["display_col"] = "1";
            }
            else
            {
                _ItemRow["display_col"] = Request.NewColumn;
            }
            if( Request.NewRow <= 0 )
            {
                _ItemRow["display_row"] = _getNextAvailableRowForItem( RoleId, Request.ActionId );
            }
            else
            {
                _ItemRow["display_row"] = Request.NewRow;
            }
            _ItemRow["displaytext"] = Request.Text;
            _ItemRow["buttonicon"] = Request.ButtonIcon;
        }

        private Int32 _getNextAvailableRowForItem( Int32 RoleId, string ActionId )
        {
            String ActionText = String.Empty;
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                ActionText = " and (for_actionid = " + ActionId + ")";
            }
            string SqlText = "select max(display_row) maxrow from landingpage where display_col = 1 and (for_roleid = " + RoleId.ToString() + ")" + ActionText;
            CswArbitrarySelect LandingPageSelect = _CswNbtResources.makeCswArbitrarySelect( "LandingPageForRole", SqlText );
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
