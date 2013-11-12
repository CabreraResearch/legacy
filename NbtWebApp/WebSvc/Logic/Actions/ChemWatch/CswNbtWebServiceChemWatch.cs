using System;
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.ChemWatchAuthServices;
using NbtWebApp.ChemWatchCommonServices;
using NbtWebApp.ChemWatchMaterialServices;
using NbtWebApp.WebSvc.Logic.Actions.ChemWatch;
using Country = NbtWebApp.ChemWatchCommonServices.Country;
using Language = NbtWebApp.ChemWatchCommonServices.Language;

namespace NbtWebApp.Actions.ChemWatch
{
    public class CswNbtWebServiceChemWatch
    {
        //TODO: make these config vars like ChemCat?
        private const string _chemWatchUserName = "chemswt";
        private const string _chemWatchPassword = "1107ms";

        private static readonly CookieManagerBehavior _cookieBehavior = new CookieManagerBehavior(); //All ChemWatch service clients must share this

        public static void Initialize( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtObjClassChemical ChemicalNode = NbtResources.Nodes[Request.NbtMaterialId]; //TODO: should we verify the Request.NodeId is of a Chemical?

            Return.Data.Supplier = ChemicalNode.Supplier.CachedNodeName;
            Return.Data.PartNo = ChemicalNode.PartNumber.Text;
            Return.Data.MaterialName = ChemicalNode.TradeName.Text;

            string errorMsg;
            if( _authenticate( out errorMsg ) )
            {
                CommonServiceClient cwCommonClient = new CommonServiceClient();
                cwCommonClient.Endpoint.Behaviors.Add( _cookieBehavior );

                // Populate Language list
                Languages cwLanguages = cwCommonClient.GetLanguages();
                foreach( Language cwLanguage in cwLanguages )
                {
                    Return.Data.Languages.Add( new CswNbtChemWatchListItem()
                        {
                            Name = cwLanguage.Name,
                            Id = CswConvert.ToString( cwLanguage.Id )
                        } );
                }

                // Populate Country list
                Countries cwCountries = cwCommonClient.GetCountries();
                foreach( Country cwCountry in cwCountries )
                {
                    Return.Data.Countries.Add( new CswNbtChemWatchListItem()
                    {
                        Name = cwCountry.Name,
                        Id = CswConvert.ToString( cwCountry.Id )
                    } );
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }
        }

        public static void MaterialSearch( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            string errorMsg;
            if( _authenticate( out errorMsg ) )
            {
                MaterialServiceClient cwMaterialClient = new MaterialServiceClient();
                cwMaterialClient.Endpoint.Behaviors.Add( _cookieBehavior );

                Request.Supplier = "SIGMA"; //FOR TESTING

                ListResultOfMaterial cwMaterials = cwMaterialClient.GetMaterialsByVendorGroupId( Request.Supplier, Request.MaterialName, Request.PartNo, false, 1, 100, "", 0 );
                foreach( Material cwMaterial in cwMaterials.Rows )
                {
                    Return.Data.Materials.Add(new CswNbtChemWatchListItem()
                        {
                            Id = cwMaterial.MaterialID.ToString(),
                            Name = cwMaterial.Name
                        });
                }

                // Populate Supplier list
                _getMatchingSuppliers( Return.Data.Supplier, Return );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }
        }

        public static void GetMatchingSuppliers( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            string errorMsg;
            if( _authenticate( out errorMsg ) )
            {
                _getMatchingSuppliers( Request.Supplier, Return );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }
        }

        private static void _getMatchingSuppliers( string SearchString, CswNbtChemWatchReturn Return )
        {
            MaterialServiceClient cwMaterialClient = new MaterialServiceClient();
            cwMaterialClient.Endpoint.Behaviors.Add( _cookieBehavior );

            SearchVendorRequest cwSearchVend = new SearchVendorRequest();
            cwSearchVend.Name = SearchString;
            cwSearchVend.PageNumber = 1;
            cwSearchVend.PageSize = 100;

            ListResultOfVendor cwVendors = cwMaterialClient.SearchVendors( cwSearchVend );
            foreach( Vendor cwVendor in cwVendors.Rows )
            {
                Return.Data.Suppliers.Add( new CswNbtChemWatchListItem()
                    {
                        Name = cwVendor.Name,
                        Id = cwVendor.VendorGroup.Gid
                    } );
            }
        }

        private static bool _authenticate( out string ErrorMsg )
        {
            ErrorMsg = "";
            bool ret = false;
            try
            {
                AuthenticateServiceClient cwAuthClient = new AuthenticateServiceClient();
                cwAuthClient.Endpoint.Behaviors.Add( _cookieBehavior );
                UserCredential cwUserCredential = new UserCredential()
                    {
                        UserName = _chemWatchUserName,
                        Password = _chemWatchPassword
                    };
                GeneralResponseOfAuthenticationResponse cwAuthResponse = cwAuthClient.Authenticate( cwUserCredential ); //providing invalid credentials will throw an exception
                if( cwAuthResponse.ErrorCode == 0 )
                {
                    ret = true;
                }
                else
                {
                    ErrorMsg = cwAuthResponse.ErrorMessage;
                }
            }
            catch( Exception ex )
            {
                ErrorMsg = ex.Message;
            }
            return ret;
        }

    }
}