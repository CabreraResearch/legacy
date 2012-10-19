using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ChemSW.Nbt.LandingPage
{
    public abstract class CswNbtLandingPageItem
    {
        protected CswNbtResources _CswNbtResources;
        public int LandingPageId { get; set; }
        protected LandingPageData.LandingPageItem _ItemData;
        public LandingPageData.LandingPageItem ItemData
        {
            get { return _ItemData; }
        }

        protected CswNbtLandingPageItem( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ItemData = new LandingPageData.LandingPageItem();
        }        

        protected virtual void _setCommonItemData( DataRow LandingPageRow )
        {
            _ItemData.LandingPageId = LandingPageRow["landingpageid"].ToString();
            if( false == String.IsNullOrEmpty( _ItemData.Text ) )
            {
                _ItemData.LinkType = LandingPageRow["componenttype"].ToString();
                _ItemData.DisplayRow = LandingPageRow["display_row"].ToString();
                _ItemData.DisplayCol = LandingPageRow["display_col"].ToString();
            }
        }

        public abstract void setItemData( DataRow LandingPageRow );
        //public abstract void OnDisable();
    }
}
