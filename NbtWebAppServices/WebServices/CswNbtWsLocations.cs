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
        public CswNbtLocationsResponse list()
        {
            CswNbtLocationsResponse Ret = new CswNbtLocationsResponse( _Context );
            try
            {
                _CswNbtSessionResources = Ret.CswNbtSessionResources;
                CswNbtResources NbtResources = _CswNbtSessionResources.CswNbtResources;
                CswNbtActSystemViews SystemViews = new CswNbtActSystemViews( NbtResources );
                CswNbtView LocationsListView = SystemViews.SiLocationsListView();
                ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( LocationsListView, true, false );
                Int32 LocationCount = Tree.getChildNodeCount();
                CswNbtWsLocationsModel.CswNbtWsLocationListModel LocationList = new CswNbtWsLocationsModel.CswNbtWsLocationListModel();
                if( LocationCount > 0 )
                {
                    for( Int32 N = 0; N < LocationCount; N += 1 )
                    {
                        Tree.goToNthChild( 0 );
                        CswNbtWsLocationsModel.CswNbtWsLocationNodeModel LocationNode = new CswNbtWsLocationsModel.CswNbtWsLocationNodeModel();
                        JArray Props = Tree.getChildNodePropsOfNode();

                        LocationNode.Name = Tree.getNodeNameForCurrentPosition();
                        LocationNode.LocationId = Tree.getNodeIdForCurrentPosition().ToString();
                        foreach( JObject Prop in Props )
                        {
                            if( CswConvert.ToString( Prop["fieldtype"] ).ToLower() == CswNbtMetaDataFieldType.NbtFieldType.Location.ToString().ToLower() )
                            {
                                LocationNode.Path = CswConvert.ToString( Prop["gestalt"] );
                            }
                        }
                        LocationList.LocationsList.Add( LocationNode );
                        Tree.goToParentNode();
                    }
                    Ret.Data = LocationList;
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