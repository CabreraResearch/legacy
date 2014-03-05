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

        protected override bool hasPermission( CswNbtAPIRequest Request, CswNbtAPIReturn Return )
        {
            return base.hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.Delete, Request, Return );
        }

        public void Delete( CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            if( hasPermission( Request, Return ) )
            {
                try
                {
                    CswNbtNode DoomedNode = _CswNbtResources.Nodes.GetNode( Request.NodeId );
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
                catch( Exception ex )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void Delete( ICswResources CswResources, CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            CswNbtWebServiceDELETE POST = new CswNbtWebServiceDELETE( (CswNbtResources) CswResources );
            POST.Delete( Return, Request );
        }

        #endregion

    }
}