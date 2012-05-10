using System;
using ChemSW.Nbt;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public interface ICswNbtWcfResponse
    {
        CswNbtSessionAuthenticationStatus SessionAuthenticationStatus { get; set; }
        CswNbtWebServiceStatus Status { get; set; }
        CswNbtWebServicePerformance Performance { get; set; }
        CswNbtWcfSessionResources CswNbtWcfSessionResources { get; set; }
        void finalizeResponse( CswNbtResources OtherResources = null );
        void addError( Exception Exception );
    }
}