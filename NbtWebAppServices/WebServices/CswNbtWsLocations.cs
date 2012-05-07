using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using NbtWebAppServices.Response;
using NbtWebAppServices.Session;
using Newtonsoft.Json.Linq;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWsLocations
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtSessionResources _CswNbtSessionResources = null;

        [OperationContract]
        [WebGet]
        [Description( "Generate a list of Locations" )]
        public CswNbtWebServiceResponseLocations list()
        {
            CswNbtWebServiceResponseLocations Ret = new CswNbtWebServiceResponseLocations( _Context );
            try
            {
                _CswNbtSessionResources = Ret.CswNbtSessionResources;
                CswNbtResources NbtResources = _CswNbtSessionResources.CswNbtResources;
                CswNbtActSystemViews SystemViews = new CswNbtActSystemViews( NbtResources );
                CswNbtView LocationsListView = SystemViews.SiLocationsListView();
                ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( LocationsListView, true, false );
                Int32 LocationCount = Tree.getChildNodeCount();
                CswNbtLocationsResponseModel LocationModel = new CswNbtLocationsResponseModel();

                if( LocationCount > 0 )
                {
                    CswNbtMetaDataObjectClass LocationsOc = _CswNbtSessionResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );

                    for( Int32 N = 0; N < LocationCount; N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();

                        if( NodeKey.ObjectClassId == LocationsOc.ObjectClassId )
                        {
                            CswNbtLocationsResponseModel.CswNbtLocationNodeModel LocationNode = new CswNbtLocationsResponseModel.CswNbtLocationNodeModel();
                            JArray Props = Tree.getChildNodePropsOfNode();

                            LocationNode.Name = Tree.getNodeNameForCurrentPosition();
                            LocationNode.LocationId = Tree.getNodeIdForCurrentPosition().ToString();
                            LocationNode.Path = default( string );
                            foreach( JObject Prop in Props )
                            {
                                if( CswConvert.ToString( Prop["fieldtype"] ).ToLower() == CswNbtMetaDataFieldType.NbtFieldType.Location.ToString().ToLower() )
                                {
                                    LocationNode.Path = CswConvert.ToString( Prop["gestalt"] );
                                }
                            }
                            LocationModel.Add( LocationNode );
                        }
                        Tree.goToParentNode();
                    }
                    Ret.Data = LocationModel;
                }
            }
            catch( Exception Ex )
            {
                Ret.addError( Ex );
            }
            Ret.finalizeResponse();
            return Ret;
        }


    }
}