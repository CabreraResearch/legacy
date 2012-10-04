using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Security;
using NbtWebAppServices.Response;
using NbtWebAppServices.Session;
using Newtonsoft.Json.Linq;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWcfLocationsUriMethods
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;

        [OperationContract]
        [WebGet]
        [Description( "Generate a list of Locations" )]
        public CswNbtWcfLocationsResponse list( bool IsMobile = true )
        {
            CswNbtWcfLocationsResponse Ret = new CswNbtWcfLocationsResponse( _Context, IsMobile );
            try
            {
                _CswNbtWcfSessionResources = Ret.CswNbtWcfSessionResources;
                if( Ret.SessionAuthenticationStatus.AuthenticationStatus == AuthenticationStatus.Authenticated.ToString() )
                {
                    CswNbtResources NbtResources = _CswNbtWcfSessionResources.CswNbtResources;
                    CswNbtActSystemViews LocationSystemView = new CswNbtActSystemViews( NbtResources, CswNbtActSystemViews.SystemViewName.SILocationsList, null );
                    CswNbtView LocationsListView = LocationSystemView.SystemView;
                    ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( LocationsListView, true, false );
                    Int32 LocationCount = Tree.getChildNodeCount();
                    CswNbtWcfLocationsDataModel WcfLocationModel = new CswNbtWcfLocationsDataModel();

                    if( LocationCount > 0 )
                    {
                        CswNbtMetaDataObjectClass LocationsOc = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                        Collection<CswNbtWcfLocationsDataModel.CswNbtLocationNodeModel> Locations = new Collection<CswNbtWcfLocationsDataModel.CswNbtLocationNodeModel>();
                        for( Int32 N = 0; N < LocationCount; N += 1 )
                        {
                            Tree.goToNthChild( N );
                            CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();

                            if( NodeKey.ObjectClassId == LocationsOc.ObjectClassId )
                            {
                                CswNbtWcfLocationsDataModel.CswNbtLocationNodeModel LocationNode = new CswNbtWcfLocationsDataModel.CswNbtLocationNodeModel();
                                JArray Props = Tree.getChildNodePropsOfNode();

                                // LocationNode.Name = Tree.getNodeNameForCurrentPosition();
                                LocationNode.LocationId = Tree.getNodeIdForCurrentPosition().ToString();
                                foreach( JObject Prop in Props )
                                {
                                    if( CswConvert.ToString( Prop["fieldtype"] ).ToLower() == CswNbtMetaDataFieldType.NbtFieldType.Location.ToString().ToLower() )
                                    {
                                        LocationNode.Name = CswConvert.ToString( Prop["gestalt"] ) + CswNbtNodePropLocation.PathDelimiter + Tree.getNodeNameForCurrentPosition();
                                    }
                                }
                                Locations.Add( LocationNode );
                            }
                            Tree.goToParentNode();
                        }
                        foreach( var Location in from CswNbtWcfLocationsDataModel.CswNbtLocationNodeModel _Location
                                                                                                            in Locations
                                                 orderby _Location.Name
                                                 select _Location )
                        {
                            WcfLocationModel.Locations.Add( Location );
                        }
                        Ret.Data = WcfLocationModel;
                    }
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