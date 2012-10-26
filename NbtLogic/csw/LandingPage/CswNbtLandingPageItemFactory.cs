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
            switch(LandingPageItemType)
            {
                case CswNbtLandingPageItemType.Text:
                Item = new CswNbtLandingPageItemText( CswNbtResources );
                    break;
                case CswNbtLandingPageItemType.Add:
                Item = new CswNbtLandingPageItemAdd( CswNbtResources );
                    break;
                case CswNbtLandingPageItemType.Link:
                Item = new CswNbtLandingPageItemLink( CswNbtResources );
                    break;
                case CswNbtLandingPageItemType.Tab:
                Item = new CswNbtLandingPageItemTab( CswNbtResources );
                    break;
                case CswNbtLandingPageItemType.Button:
                Item = new CswNbtLandingPageItemButton( CswNbtResources );
                    break;
                default:
                throw new CswDniException( ErrorType.Error,
                                           "Unhandled item type: " + LandingPageItemType,
                                           "CswNbtLandingPageItemFactory did not recognize item type: " + LandingPageItemType );
            }
            return Item;
        }

    }
}
