using System;
using System.Net;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceDELETE: CswNbtWebServiceAPI
    {
        public const string VERB = "DELETE";

        #region Non Static

        public CswNbtWebServiceDELETE( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return base.hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.Delete, GenericRequest, Return );
        }

        public void Delete( CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtNode DoomedNode = _CswNbtResources.Nodes.GetNode( GenericRequest.NodeId );
                    if( null != DoomedNode )
                    {
                        DoomedNode.delete();
                        Return.Status = HttpStatusCode.OK;
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

        public static void Delete( ICswResources CswResources, CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceDELETE POST = new CswNbtWebServiceDELETE( (CswNbtResources) CswResources );
            POST.Delete( Return, GenericRequest );
        }

        #endregion

    }
}