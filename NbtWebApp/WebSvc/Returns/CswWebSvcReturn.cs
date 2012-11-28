using System;
using System.Runtime.Serialization;
using ChemSW;
//using ChemSW.Nbt;
using ChemSW.Nbt;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.WebSvc;

namespace NbtWebApp.WebSvc.Returns
{
    /// <summary>
    /// Base class for WCF return objects. All WCF returns must inherit from this class.
    /// </summary>
    [DataContract]
    public class CswWebSvcReturn : ICswWebSvcRetObj
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CswWebSvcReturn()
        {
            Authentication = new CswWebSvcSessionAuthenticateData.Authentication.Response();
            Status = new CswWebSvcReturnBase.Status();
            Performance = new CswWebSvcReturnBase.Performance();
            Logging = new CswWebSvcReturnBase.Logging();
        }

        private CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// CswResources to support run() and finalize()
        /// </summary>

        /// <summary>
        /// Authentication status and Timeout for this request's response 
        /// </summary>
        [DataMember]
        public CswWebSvcSessionAuthenticateData.Authentication.Response Authentication { get; set; }

        /// <summary>
        /// Status of this request's response, include error content (if any)
        /// </summary>
        [DataMember]
        public CswWebSvcReturnBase.Status Status { get; set; }

        /// <summary>
        /// Performance data associated with this request
        /// </summary>
        [DataMember]
        public CswWebSvcReturnBase.Performance Performance { get; set; }

        /// <summary>
        /// Logging data associated with this request
        /// </summary>
        [DataMember]
        public CswWebSvcReturnBase.Logging Logging { get; set; }


        #region inherited
        public ICswResources CswResources
        {
            set
            {
                _CswNbtResources = (CswNbtResources) value;
            }

        }



        /// <summary>
        /// Add an exception to this request's Status's Error collection
        /// </summary>
        public void addException( Exception Exception )
        {
            Status.Errors.Add( CswWebSvcCommonMethods.wError( _CswNbtResources, Exception ) );
        }

        /// <summary>
        /// Finalize this request to set Authentication, Logging, Performance and Error content to the response.
        /// </summary>
        public void finalize( AuthenticationStatus AuthenticationStatus )
        {
            try
            {
                CswWebSvcCommonMethods.wAddAuthenticationStatus( _CswNbtResources, null, this, AuthenticationStatus );
            }
            catch( Exception Exception )
            {
                addException( Exception );
            }
            // ******************************************
            // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
            // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
            // (see case 26273)
            // ******************************************
        }//finaize() 
        #endregion




    } //CswWebSvcRetJObj

} // namespace ChemSW.Nbt.WebServices
