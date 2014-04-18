using System;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceREAD: CswNbtWebServiceAPI
    {
        public const string VERB = "GET";

        #region Non-Static

        public CswNbtWebServiceREAD( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.View, GenericRequest, Return );
        }

        public void GetResource( CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( GenericRequest.NodeId );
                    if( null != Node && GenericRequest.MetaDataName == Node.getNodeType().NodeTypeName )
                    {
                        Return.NodeId = GenericRequest.NodeId;
                        Return.NodeName = Node.NodeName;
                        Return.NodeType = Node.getNodeType().NodeTypeName;
                        Return.ObjectClass = Node.ObjClass.ObjectClass.ObjectClassName;
                        CswNbtMetaDataPropertySet propSet = Node.getObjectClass().getPropertySet();
                        if( null != propSet )
                        {
                            Return.PropertySet = propSet.Name;
                        }
                        Return.URI = BuildURI( Return.NodeType, Node.NodeId.PrimaryKey );
                        Return.PropertyData = GetPropertyData( Node );
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
            else
            {
                Return.Status = HttpStatusCode.Forbidden;
            }

        }

        public void GetCollection( CswNbtResourceCollection Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( GenericRequest.MetaDataName );
                    if( null != NodeType )
                    {
                        CswNbtView GetNodeTypeView = new CswNbtView( _CswNbtResources );
                        GetNodeTypeView.AddViewRelationship( NodeType, false );
                        ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( GetNodeTypeView, true, false, false );
                        for( int i = 0; i < tree.getChildNodeCount(); i++ )
                        {
                            tree.goToNthChild( i );
                            CswNbtNodeKey nodeKey = tree.getNodeKeyForCurrentPosition();
                            CswPrimaryKey nodeId = tree.getNodeIdForCurrentPosition();
                            CswNbtMetaDataObjectClass objectClass = _CswNbtResources.MetaData.getObjectClass( nodeKey.ObjectClassId );
                            CswNbtMetaDataPropertySet propSet = objectClass.getPropertySet();
                            string propSetStr = string.Empty;
                            if( null != propSet )
                            {
                                propSetStr = propSet.Name;
                            }
                            string nodeName = tree.getNodeNameForCurrentPosition();
                            Return.Add( nodeName, nodeId, NodeType.NodeTypeName, objectClass.ObjectClassName, propSetStr, BuildURI( NodeType.NodeTypeName, nodeKey.NodeId.PrimaryKey ) );
                            tree.goToParentNode();
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

        public static void GetResource( ICswResources CswResources, CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceREAD GET = new CswNbtWebServiceREAD( (CswNbtResources) CswResources );
            GET.GetResource( Return, GenericRequest );
        }

        public static void GetCollection( ICswResources CswResources, CswNbtResourceCollection Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceREAD GET = new CswNbtWebServiceREAD( (CswNbtResources) CswResources );
            GET.GetCollection( Return, GenericRequest );
        }

        #endregion
    }
}