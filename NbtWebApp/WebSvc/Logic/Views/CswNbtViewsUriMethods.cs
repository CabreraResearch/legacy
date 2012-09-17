using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp.WebSvc.Logic.Views
{
    [DataContract]
    public enum FaultCode
    {
        [EnumMember]
        ERROR,
        [EnumMember]
        INCORRECT_PARAMETER
    }

    [DataContract]
    public class SampleFaultException
    {
        [DataMember]
        public FaultCode errorcode;
        [DataMember]
        public string message;
        [DataMember]
        public string details;
    }

    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtViewsUriMethods
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( SampleFaultException ) )]
        [Description( "Generate a View Select" )]
        public CswNbtViewReturn ViewSelect( ViewSelect.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtViewReturn Ret = new CswNbtViewReturn();
            var GetViewDriverType = new CswWebSvcDriver<CswNbtViewReturn, ViewSelect.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceView.getViewSelectWebSvc,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );

        }
    }
}