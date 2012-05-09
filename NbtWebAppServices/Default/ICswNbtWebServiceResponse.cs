using System;
using ChemSW.Nbt;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public interface ICswNbtWebServiceResponse
    {
        CswNbtSessionAuthenticationStatus SessionAuthenticationStatus { get; set; }
        CswNbtWebServiceStatus Status { get; set; }
        CswNbtWebServicePerformance Performance { get; set; }
        CswNbtSessionResources CswNbtSessionResources { get; set; }
        void finalizeResponse( CswNbtResources OtherResources = null );
        void addError( Exception Exception );
    }
}