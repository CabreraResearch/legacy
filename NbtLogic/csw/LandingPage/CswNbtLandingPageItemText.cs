using System;
using System.Data;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemText : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemText( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            _ItemData.Text = LandingPageRow["displaytext"].ToString();
            _setCommonItemDataForUI( LandingPageRow );
        }

        public override void setItemDataForDB( LandingPageData.Request Request )
        {
            _setCommonItemDataForDB( Request );
            _ItemRow["buttonicon"] = String.Empty;
            if( String.IsNullOrEmpty( Request.Text ) )
            {
                throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text LandingPage Item" );
            }
        }
    }
}
