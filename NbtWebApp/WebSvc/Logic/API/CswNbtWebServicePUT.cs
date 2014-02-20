


using System;
using System.Net;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServicePUT
    {
        /// <summary>
        /// Based on the input, verifies if the user has permission to continue
        /// </summary>
        private static bool hasPermission( CswNbtResources NbtResources, CswNbtAPIRequest Request, CswNbtAPIReturn Return )
        {
            bool ret = false;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Request.NodeType );
            if( null != NodeType )
            {
                if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, NodeType, User : NbtResources.CurrentNbtUser ) )
                {
                    ret = true;
                }
                else
                {
                    Return.Status = Return.Status = HttpStatusCode.Forbidden; //Permission denied
                }
            }
            else
            {
                Return.Status = Return.Status = HttpStatusCode.NotFound;
            }
            return ret;
        }

        public static void Edit( ICswResources CswResources, CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( hasPermission( NbtResources, Request, Return ) )
            {
                try
                {
                    CswNbtNode Node = NbtResources.Nodes.GetNode( Request.NodeId );
                    if( null != Node )
                    {
                        CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( NbtResources );
                        SdTabsAndProps.saveNodeProps( Node, Request.PropData );
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
    }
}