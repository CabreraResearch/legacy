using System;
using ChemSW.Security;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public interface ICswNbtWebServiceResponse
    {
        CswNbtSessionAuthenticationStatus AuthenticationStatus { get; set; }
        CswNbtWebServiceStatus Status { get; set; }
        CswNbtWebServicePerformance Performance { get; set; }
        void finalizeResponse( AuthenticationStatus AuthenticationStatusIn, CswNbtSessionResources CswNbtSessionResources, object OptionalData = null );
        void addError( Exception Exception, CswNbtSessionResources CswNbtSessionResources );
    }
}