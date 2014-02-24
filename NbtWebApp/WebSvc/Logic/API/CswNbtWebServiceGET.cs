using System;
using System.Collections.Generic;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceGET
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
                if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NodeType, User : NbtResources.CurrentNbtUser ) )
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

        public static void GetResource( ICswResources CswResources, CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( hasPermission( NbtResources, Request, Return ) )
            {
                try
                {
                    //CswNbtNode Node = NbtResources.Nodes.GetNode( Request.NodeId );
                    //if( null != Node )
                    //{
                    //    Return.NodeId = Request.NodeId;
                    //    Return.NodeName = Node.NodeName;
                    //    Return.NodeType = Node.getNodeType().NodeTypeName;
                    //    Return.ObjectClass = Node.ObjClass.ObjectClass.ObjectClassName;
                    //    Return.PropertySet = Node.getObjectClass().getPropertySet().Name;
                    //    Return.URI = "api/v1/" + Node.getNodeType().NodeTypeName + "/" + Request.NodeId.PrimaryKey;
                    //
                    //    //    "api/vi/chemicals?casno=123-32-3"
                    //
                    //    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( NbtResources );
                    //    CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                    //    //TODO: better way to get property data - we're forcing it to be by tab...we should get ALL properties, regardless of what tab they're on
                    //    //TODO: getProps() returns an object like "{Node: { ... }, properties: { ... } }" and all we want is properties...getProps() should just return properties
                    //    //TODO: PropertyData is returned as a string and the user is forced to JSON.parse it...this should be an object
                    //    Return.PropertyData = CswConvert.ToJObject( SdTabsAndProps.getProps( Node, NodeType.getFirstNodeTypeTab().TabId.ToString(), null, CswEnumNbtLayoutType.Edit )["properties"] );
                    //}
                    //else
                    //{
                    //    Return.Status = HttpStatusCode.NotFound;
                    //}
                }
                catch( Exception ex )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }

        }

        public static void GetCollection( ICswResources CswResources, CswNbtResourceCollection Return, CswNbtAPIRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( hasPermission( NbtResources, Request, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Request.NodeType );
                    if( null != NodeType )
                    {
                        foreach( KeyValuePair<CswPrimaryKey, string> pair in NodeType.getNodeIdAndNames( false, false ) )
                        {
                            Return.Add( pair.Value, pair.Key, NodeType.NodeTypeName, string.Empty, string.Empty );
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
    }
}