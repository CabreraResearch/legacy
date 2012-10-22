using ChemSW.Exceptions;

namespace ChemSW.Nbt.LandingPage
{
    /// <summary>
    /// Factory for LandingPageItems
    /// </summary>
    public class CswNbtLandingPageItemFactory
    {
        public static CswNbtLandingPageItem makeLandingPageItem( CswNbtResources CswNbtResources, CswNbtLandingPageItemType LandingPageItemType )
        {
            CswNbtLandingPageItem Item;
            if( CswNbtLandingPageItemType.Text == LandingPageItemType )
            {
                Item = new CswNbtLandingPageItemText( CswNbtResources );
            }
            else if( CswNbtLandingPageItemType.Add == LandingPageItemType )
            {
                Item = new CswNbtLandingPageItemAdd( CswNbtResources );
            }
            else if( CswNbtLandingPageItemType.Link == LandingPageItemType )
            {
                Item = new CswNbtLandingPageItemLink( CswNbtResources );
            }
            else if( CswNbtLandingPageItemType.Tab == LandingPageItemType )
            {
                Item = new CswNbtLandingPageItemTab( CswNbtResources );
            }
            else
            {
                throw new CswDniException( ErrorType.Error,
                                           "Unhandled item type: " + LandingPageItemType,
                                           "CswNbtLandingPageItemFactory did not recognize item type: " + LandingPageItemType );
            }
            return Item;
        }

    }
}
