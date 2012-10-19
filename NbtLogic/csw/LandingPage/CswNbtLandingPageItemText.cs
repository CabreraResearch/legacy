using System.Data;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemText : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemText( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemData( DataRow LandingPageRow )
        {
            _ItemData.Text = LandingPageRow["displaytext"].ToString();
            _setCommonItemData( LandingPageRow );
        }
    }
}
