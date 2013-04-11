using ChemSW.Exceptions;

namespace ChemSW.Nbt.LandingPage
{
    /// <summary>
    /// Factory for LandingPageItems
    /// </summary>
    public class CswNbtLandingPageItemFactory
    {
        public static CswNbtLandingPageItem makeLandingPageItem( CswNbtResources CswNbtResources, CswEnumNbtLandingPageItemType LandingPageItemType )
        {
            CswNbtLandingPageItem Item;
            switch(LandingPageItemType)
            {
                case CswEnumNbtLandingPageItemType.Text:
                Item = new CswNbtLandingPageItemText( CswNbtResources );
                    break;
                case CswEnumNbtLandingPageItemType.Add:
                Item = new CswNbtLandingPageItemAdd( CswNbtResources );
                    break;
                case CswEnumNbtLandingPageItemType.Link:
                Item = new CswNbtLandingPageItemLink( CswNbtResources );
                    break;
                case CswEnumNbtLandingPageItemType.Tab:
                Item = new CswNbtLandingPageItemTab( CswNbtResources );
                    break;
                case CswEnumNbtLandingPageItemType.Button:
                Item = new CswNbtLandingPageItemButton( CswNbtResources );
                    break;
                default:
                throw new CswDniException( CswEnumErrorType.Error,
                                           "Unhandled item type: " + LandingPageItemType,
                                           "CswNbtLandingPageItemFactory did not recognize item type: " + LandingPageItemType );
            }
            return Item;
        }

    }
}
