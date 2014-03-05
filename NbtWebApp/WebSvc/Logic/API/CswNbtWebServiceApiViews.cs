using System;
using System.Net;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceApiViews: CswNbtWebServiceAPI
    {
        public const string VERB = "GET";

        #region Non Static

        public CswNbtWebServiceApiViews( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return true; //TODO: view perms?
        }

        public void RunTree( CswNbtAPITree Return, int ViewIdInt )
        {
            try
            {
                CswNbtViewId ViewId = new CswNbtViewId( ViewIdInt );
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                if( null != View )
                {
                    ICswNbtTree results = _CswNbtResources.Trees.getTreeFromView( View, true, false, false );
                    for( int i = 0; i < results.getChildNodeCount(); i++ )
                    {
                        results.goToNthChild( i );
                        CswNbtNodeKey NodeKey = results.getNodeKeyForCurrentPosition();
                        string Name = results.getNodeNameForCurrentPosition();
                        CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );
                        CswNbtMetaDataObjectClass oc = nt.getObjectClass();
                        CswNbtMetaDataPropertySet propSet = oc.getPropertySet();
                        string PropSetName = string.Empty;
                        if( null != propSet )
                        {
                            PropSetName = propSet.Name;
                        }

                        CswNbtAPITree.CswNbtTreeResource Parent = Return.Add( Name, NodeKey.NodeId, nt.NodeTypeName, oc.ObjectClassName, PropSetName, string.Empty );
                        _recurseTree( results, Parent );
                        results.goToParentNode();
                    }

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

        private void _recurseTree( ICswNbtTree results, CswNbtAPITree.CswNbtTreeResource Parent )
        {
            for( int i = 0; i < results.getChildNodeCount(); i++ )
            {
                results.goToNthChild( i );
                CswNbtNodeKey NodeKey = results.getNodeKeyForCurrentPosition();
                string Name = results.getNodeNameForCurrentPosition();
                CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );
                CswNbtMetaDataObjectClass oc = nt.getObjectClass();
                CswNbtMetaDataPropertySet propSet = oc.getPropertySet();
                string PropSetName = string.Empty;
                if( null != propSet )
                {
                    PropSetName = propSet.Name;
                }

                CswNbtAPITree.CswNbtTreeResource SubParent = Parent.Add( Name, NodeKey.NodeId, nt.NodeTypeName, oc.ObjectClassName, PropSetName, string.Empty );
                _recurseTree( results, SubParent );
                results.goToParentNode();
            }
        }

        public void RunGrid( CswNbtAPIGrid Return, int ViewIdInt )
        {
            try
            {
                CswNbtViewId ViewId = new CswNbtViewId( ViewIdInt );
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                if( null != View )
                {
                    ICswNbtTree results = _CswNbtResources.Trees.getTreeFromView( View, true, false, false );
                    for( int i = 0; i < results.getChildNodeCount(); i++ )
                    {
                        results.goToNthChild( i );
                        CswNbtNodeKey NodeKey = results.getNodeKeyForCurrentPosition();
                        string Name = results.getNodeNameForCurrentPosition();
                        CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );
                        CswNbtMetaDataObjectClass oc = nt.getObjectClass();
                        CswNbtMetaDataPropertySet propSet = oc.getPropertySet();
                        string PropSetName = string.Empty;
                        if( null != propSet )
                        {
                            PropSetName = propSet.Name;
                        }

                        Return.Add( Name, NodeKey.NodeId, nt.NodeTypeName, oc.ObjectClassName, PropSetName );
                        results.goToParentNode();
                    }

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

        #endregion

        #region Static

        public static void RunGrid( ICswResources CswResources, CswNbtAPIGrid Return, int ViewIdInt )
        {
            CswNbtWebServiceApiViews viewApi = new CswNbtWebServiceApiViews( (CswNbtResources) CswResources );
            viewApi.RunGrid( Return, ViewIdInt );
        }

        public static void RunTree( ICswResources CswResources, CswNbtAPITree Return, int ViewIdInt )
        {
            CswNbtWebServiceApiViews viewApi = new CswNbtWebServiceApiViews( (CswNbtResources) CswResources );
            viewApi.RunTree( Return, ViewIdInt );
        }

        #endregion

    }
}