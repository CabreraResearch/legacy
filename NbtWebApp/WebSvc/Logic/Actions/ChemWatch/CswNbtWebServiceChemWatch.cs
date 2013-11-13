using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.ChemWatchAuthServices;
using NbtWebApp.ChemWatchCommonServices;
using NbtWebApp.ChemWatchDocumentServices;
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
                    Return.Data.Languages.Add( new ChemWatchMultiSlctListItem()
                        {
                            Name = cwLanguage.Name,
                            Id = CswConvert.ToString( cwLanguage.Id )
                        } );
                }

                // Populate Country list
                Countries cwCountries = cwCommonClient.GetCountries();
                foreach( Country cwCountry in cwCountries )
                {
                    Return.Data.Countries.Add( new ChemWatchMultiSlctListItem()
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
                    Return.Data.Materials.Add( new ChemWatchListItem()
                        {
                            Id = CswConvert.ToString( cwMaterial.MaterialID ),
                            Name = cwMaterial.Name
                        } );
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }
        }

        public static void SDSDocumentSearch( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            string errorMsg;
            if( _authenticate( out errorMsg ) )
            {
                DocumentServiceClient cwDocClient = new DocumentServiceClient();
                cwDocClient.Endpoint.Behaviors.Add( _cookieBehavior ); //every service client needs to share this

                DocumentRequest DocumentRequest = new DocumentRequest();

                List<int> CountryIdsList = Request.Countries.Select( ListItem => CswConvert.ToInt32( ListItem.Id ) ).ToList();
                int[] CountryIdsArray = CountryIdsList.ToArray();
                DocumentRequest.CountryCode = CountryIdsArray;

                List<int> LanguageIdsList = Request.Languages.Select( ListItem => CswConvert.ToInt32( ListItem.Id ) ).ToList();
                int[] LanguageIdsArray = LanguageIdsList.ToArray();
                DocumentRequest.LanguageCode = LanguageIdsArray;

                DocumentRequest.MaterialId = CswConvert.ToString( Request.ChemWatchMaterialId );
                DocumentRequest.IsShowOwn = false;
                DocumentRequest.IsLatest = true;
                DocumentRequest.ShowOnlyGold = false;
                DocumentRequest.PageNumber = 1;
                DocumentRequest.PageSize = 50;
                DocumentRequest.HideGold = true;
                DocumentRequest.Gid = Request.Supplier;

                ListResultOfDocument DocumentList = cwDocClient.GetDocumentsByMaterialId( DocumentRequest );
                foreach( Document Doc in DocumentList.Rows )
                {
                    // todo: we need to display language and country on client and pass filename/externalurl
                    ChemWatchSDSDoc SDSDoc = new ChemWatchSDSDoc();
                    SDSDoc.Language = Doc.LanguageCode;
                    SDSDoc.Country = Doc.CountryCode;
                    SDSDoc.File = Doc.FileName.Length > 0 ? Doc.FileName : Doc.ExternalUrl;
                    Return.Data.SDSDocuments.Add( SDSDoc );
                }
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
                Return.Data.Suppliers.Add( new ChemWatchListItem()
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