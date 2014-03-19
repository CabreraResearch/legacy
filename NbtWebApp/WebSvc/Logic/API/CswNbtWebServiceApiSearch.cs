using System;
using System.Net;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.WebServices;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceApiSearch: CswNbtWebServiceAPI
    {
        public const string VERB = "GET";

        #region Non Static

        public CswNbtWebServiceApiSearch( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return true; //TODO: search permissions?
        }

        public void Search( CswNbtResourceCollection Return, CswNbtApiSearchRequest SearchRequest )
        {
            try
            {
                int filter_nt_id = Int32.MinValue;
                if( false == String.IsNullOrEmpty( SearchRequest.NodeType ) )
                {
                    filter_nt_id = _CswNbtResources.MetaData.getNodeType( SearchRequest.NodeType ).NodeTypeId;
                }

                CswNbtWebServiceSearch searchService = new CswNbtWebServiceSearch( _CswNbtResources, null );
                CswNbtSearch search = searchService.getSearch( SearchRequest.Query, SearchRequest.SearchType, filter_nt_id, Int32.MinValue, Int32.MinValue );
                ICswNbtTree results = search.Results();
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
            catch( Exception )
            {
                Return.Status = HttpStatusCode.InternalServerError;
            }
        }

        #endregion

        #region Static

        public static void Search( ICswResources CswResources, CswNbtResourceCollection Return, CswNbtApiSearchRequest SearchRequest )
        {
            CswNbtWebServiceApiSearch SEARCH = new CswNbtWebServiceApiSearch( (CswNbtResources) CswResources );
            SEARCH.Search( Return, SearchRequest );
        }

        #endregion

    }
}