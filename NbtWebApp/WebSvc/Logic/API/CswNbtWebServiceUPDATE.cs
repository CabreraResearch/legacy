using System;
using System.Net;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceUPDATE: CswNbtWebServiceAPI
    {
        public const string VERB = "PUT";

        #region Non Static

        public CswNbtWebServiceUPDATE( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.View, GenericRequest, Return );
        }

        public void Edit( CswNbtAPIReturn Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( GenericRequest.NodeId );
                    if( null != Node )
                    {
                        CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                        SdTabsAndProps.saveNodeProps( Node, GenericRequest.PropData );
                        if( Node.IsTemp )
                        {
                            Node.PromoteTempToReal();
                        }
                        else
                        {
                            Node.postChanges( false );
                        }
                    }
                    else
                    {
                        Return.Status = HttpStatusCode.NotFound;
                    }
                }
                catch( Exception )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void Edit( ICswResources CswResources, CswNbtAPIReturn Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceUPDATE PUT = new CswNbtWebServiceUPDATE( (CswNbtResources) CswResources );
            PUT.Edit( Return, GenericRequest );
        }

        #endregion
    }
}