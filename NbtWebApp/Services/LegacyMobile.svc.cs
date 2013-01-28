using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Mobile.CISProNbt;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Legacy Mobile Data Uploads
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class LegacyMobile
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// This method is called when the user chooses a data file to be uploaded.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns>A string representation of the data file contents.</returns>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "parseDataFile" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLegacyMobile.CswNbtMobileReturn parseDataFile( Stream Stream )
        {

            CswNbtWebServiceLegacyMobile.CswNbtMobileReturn Ret = new CswNbtWebServiceLegacyMobile.CswNbtMobileReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLegacyMobile.CswNbtMobileReturn, Stream>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLegacyMobile.parseDataFile,
                ParamObj: Stream
                );

            SvcDriver.run();
            return ( Ret );
        }


        /// <summary>
        /// This method is called when the "next" button in the wizard is clicked.
        /// </summary>
        /// <param name="LegacyFileData"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "performOperations" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLegacyMobile.CswNbtMobileReturn performOperations( string LegacyFileData )
        {

            CswNbtWebServiceLegacyMobile.CswNbtMobileReturn Ret = new CswNbtWebServiceLegacyMobile.CswNbtMobileReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLegacyMobile.CswNbtMobileReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLegacyMobile.performOperations,
                ParamObj: LegacyFileData
                );

            SvcDriver.run();
            return ( Ret );
        }

    }// class LegacyMobile
}// namespace NbtWebApp
