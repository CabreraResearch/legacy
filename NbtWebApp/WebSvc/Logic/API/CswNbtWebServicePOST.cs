


using System;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServicePOST
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
                if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType, User : NbtResources.CurrentNbtUser ) )
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

        public static void Create( ICswResources CswResources, CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( hasPermission( NbtResources, Request, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Request.NodeType );
                    CswNbtNode NewNode = NbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, IsTemp: true );

                    Return.NodeId = NewNode.NodeId;
                    Return.NodeName = NewNode.NodeName;
                    Return.NodeType = NewNode.getNodeType().NodeTypeName;
                    Return.ObjectClass = NewNode.ObjClass.ObjectClass.ObjectClassName;
                    Return.URI = "api/v1/" + NodeType.NodeTypeName + "/" + NewNode.NodeId.PrimaryKey;

                    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( NbtResources );
                    //see CswNbtWebServiceGET for the problems with the line below
                    Return.PropertyData = CswConvert.ToJObject( SdTabsAndProps.getProps( NewNode, string.Empty, null, CswEnumNbtLayoutType.Add )["properties"] );

                    Return.Status = HttpStatusCode.Created;
                }
                catch( Exception ex )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}