using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;
using ChemSW;

namespace NbtWebApp.WebSvc.Logic.Menus.LandingPages
{
    /// <summary>
    /// Webservice for interacting with LandingPage data
    /// </summary>
    public class CswNbtWebServiceLandingPageItems
    {
        /// <summary>
        /// Return Object for LandingPageItems
        /// </summary>
        [DataContract]
        public class LandingPageItemsReturn : CswWebSvcReturn
        {
            public LandingPageItemsReturn()
            {
                Data = new LandingPageData();
            }
            [DataMember]
            public LandingPageData Data;
        }

        /// <summary>
        /// Gets all of the LandingPage items associated with a given RoleId or ActionId
        /// </summary>
        public static void getLandingPageItems( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            Return.Data = _CswNbtLandingPageTable.getLandingPageItems( Request );
        }

        /// <summary>
        /// Adds a new LandingPage item to the specified LandingPage
        /// </summary>
        public static void addLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.addLandingPageItem( Request );
        }

        /// <summary>
        /// Moves a LandingPage item to a new cell on the specified LandingPage
        /// </summary>
        public static void moveLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.moveLandingPageItem( Request );
        }

        /// <summary>
        /// Removes a LandingPage item from the specified LandingPage
        /// </summary>
        public static void deleteLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.deleteLandingPageItem( Request );
        }
    } // class CswNbtWebServiceLandingPageItems
} // namespace ChemSW.Nbt.WebServices

