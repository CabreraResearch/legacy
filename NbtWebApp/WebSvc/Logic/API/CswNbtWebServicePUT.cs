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
    public class CswNbtWebServicePUT: CswNbtWebServiceAPI
    {
        public const string VERB = "PUT";

        #region Non Static

        public CswNbtWebServicePUT( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIRequest Request, CswNbtAPIReturn Return )
        {
            return hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.View, Request, Return );
        }

        public void Edit( CswNbtAPIReturn Return, CswNbtAPIRequest Request )
        {
            if( hasPermission( Request, Return ) )
            {
                try
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( Request.NodeId );
                    if( null != Node )
                    {
                        CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                        SdTabsAndProps.saveNodeProps( Node, Request.PropData );
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
                catch( Exception ex )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void Edit( ICswResources CswResources, CswNbtAPIReturn Return, CswNbtAPIRequest Request )
        {
            CswNbtWebServicePUT PUT = new CswNbtWebServicePUT( (CswNbtResources) CswResources );
            PUT.Edit( Return, Request );
        }

        #endregion
    }
}