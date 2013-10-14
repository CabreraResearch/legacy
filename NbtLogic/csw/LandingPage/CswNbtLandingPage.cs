using System;
using ChemSW.Core;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPage
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtLandingPageTable _LandingPageTable;

        public CswNbtLandingPage( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _LandingPageTable = new CswNbtLandingPageTable( _CswNbtResources );
        }

        /// <summary>
        /// Grabs all WelcomePage Items for the given RoleId
        /// </summary>
        /// <param name="RoleId">RoleId to filter by</param>
        /// <returns>Collection of LandingPage Items</returns>
        public LandingPageData getWelcomePageItems( CswPrimaryKey RoleId )
        {
            LandingPageData.Request Request = new LandingPageData.Request
            {
                RoleId = RoleId.ToString()
            };
            return _LandingPageTable.getLandingPageItems( Request );
        }

        /// <summary>
        /// Grabs all LandingPage Items for the given RoleId and/or ActionId
        /// </summary>
        /// <param name="ActionId">Action ID (for Action-specific Landing Pages)</param>
        /// <param name="RoleId">RoleId (for Welcome Page)</param>
        /// <returns></returns>
        public LandingPageData getLandingPageItems( Int32 ActionId, CswPrimaryKey RoleId )
        {
            LandingPageData.Request Request = new LandingPageData.Request
            {
                ActionId = ActionId.ToString(),
                RoleId = RoleId.ToString()
            };
            return _LandingPageTable.getLandingPageItems( Request );
        }

        /// <summary>
        /// Grabs a LandingPage Item with the given Name for the given RoleId and/or Action Id
        /// </summary>
        /// <param name="Name">Label Text of the LandingPage Item to grab</param>
        /// <param name="ActionId">Action ID (for Action-specific Landing Pages)</param>
        /// <param name="RoleId">RoleId (for Welcome Page)</param>
        /// <returns></returns>
        public LandingPageData.LandingPageItem getLandingPageItem( String Name, Int32 ActionId, CswPrimaryKey RoleId )
        {
            LandingPageData.LandingPageItem LandingPageItem = null;
            LandingPageData Items = _LandingPageTable.getLandingPageItems( new LandingPageData.Request
            {
                ActionId = ActionId.ToString(),
                RoleId = RoleId.ToString()
            } );
            foreach( LandingPageData.LandingPageItem Item in Items.LandingPageItems )
            {
                if( Item.Text == Name )
                {
                    LandingPageItem = Item;
                }
            }
            return LandingPageItem;
        }

        /// <summary>
        /// Add a LandingPage Item
        /// </summary>
        /// <param name="Type">The Type of Landing Page Item to add</param>
        /// <param name="ItemToAdd">LandingPage Item data</param>
        public void addLandingPageItem( CswEnumNbtLandingPageItemType Type, LandingPageData.Request ItemToAdd )
        {
            ItemToAdd.Type = Type;
            _LandingPageTable.addLandingPageItem( ItemToAdd );
        }

        /// <summary>
        /// Moves a LandingPage Item to the specified cell (or next available cell)
        /// </summary>
        /// <param name="LandingPageId">Unique identifier of the LandingPage Item to move</param>
        /// <param name="NewRow">New Row Number (subject to change based on availability)</param>
        /// <param name="NewCol">New Column Number</param>
        public void moveLandingPageItem( Int32 LandingPageId, Int32 NewRow, Int32 NewCol )
        {
            LandingPageData.Request Request = new LandingPageData.Request
            {
                LandingPageId = LandingPageId,
                NewRow = NewRow,
                NewColumn = NewCol
            };
            _LandingPageTable.moveLandingPageItem( Request );
        }

        /// <summary>
        /// Updates a LandingPage Item to match the given data
        /// </summary>
        /// <param name="LandingPageId">Unique identifier of the LandingPage Item to update</param>
        /// <param name="ItemToUpdate">New LandingPage Item data (ALL data must be included)</param>
        public void updateLandingPageItem( Int32 LandingPageId, LandingPageData.Request ItemToUpdate )
        {
            ItemToUpdate.LandingPageId = LandingPageId;
            _LandingPageTable.updateLandingPageItem( ItemToUpdate );
        }

        /// <summary>
        /// Deletes a LandingPage Item
        /// </summary>
        /// <param name="LandingPageId">Unique identifier of the LandingPage Item to delete</param>
        public void deleteLandingPageItem( Int32 LandingPageId )
        {
            LandingPageData.Request Request = new LandingPageData.Request
            {
                LandingPageId = LandingPageId
            };
            _LandingPageTable.deleteLandingPageItem( Request );
        }
    }
}
