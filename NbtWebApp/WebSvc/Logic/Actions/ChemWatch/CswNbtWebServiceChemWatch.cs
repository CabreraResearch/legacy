using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
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
            Return.Data.NbtMaterialId = ChemicalNode.NodeId;

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
                    ChemWatchSDSDoc SDSDoc = new ChemWatchSDSDoc();
                    SDSDoc.Language = Doc.LanguageCode;
                    SDSDoc.Country = Doc.CountryCode;
                    SDSDoc.FileName = Doc.FileName.Length > 0 ? Doc.FileName : String.Empty;
                    SDSDoc.ExternalUrl = HttpUtility.UrlEncode( Doc.ExternalUrl );
                    //SDSDoc.ExternalUrl = Doc.ExternalUrl;
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

        public static void CreateSDSDocuments( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass SDSDocumentOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );

            foreach( ChemWatchSDSDoc SDSDoc in Request.SDSDocuments )
            {
                string Language = _getLanguage( SDSDoc.Language );
                CswNbtResources.Nodes.makeNodeFromNodeTypeId( SDSDocumentOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassSDSDocument NewSDSDocNode = NewNode;
                        NewSDSDocNode.Language.Value = Language;
                        NewSDSDocNode.Owner.RelatedNodeId = Request.NbtMaterialId;
                    } );
            }
        }

        #region Private Helper Methods

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

        private static string _getLanguage( string ChemWatchLanguage )
        {
            string Ret = string.Empty;

            if( ChemWatchLanguage.ToLower().Contains( "english" ) )
            {
                ChemWatchLanguage = "English";
            }
            else if( ChemWatchLanguage.ToLower().Contains( "french" ) )
            {
                ChemWatchLanguage = "French";
            }

            switch( ChemWatchLanguage )
            {
                case "Bulgarian":
                    Ret = "bg";
                    break;
                case "Czech":
                    Ret = "cs";
                    break;
                case "Danish":
                    Ret = "da";
                    break;
                case "Dutch":
                    Ret = "nl";
                    break;
                case "English":
                    Ret = "en";
                    break;
                case "Estonian":
                    Ret = "et";
                    break;
                case "Finnish":
                    Ret = "fi";
                    break;
                case "French":
                    Ret = "fr";
                    break;
                case "German":
                    Ret = "de";
                    break;
                case "Greek":
                    Ret = "el";
                    break;
                case "Hungarian":
                    Ret = "hu";
                    break;
                case "Irish":
                    Ret = "ga";
                    break;
                case "Italian":
                    Ret = "it";
                    break;
                case "Latvian":
                    Ret = "lv";
                    break;
                case "Lithuanian":
                    Ret = "lt";
                    break;
                case "Maltese":
                    Ret = "mt";
                    break;
                case "Portuguese":
                    Ret = "pt";
                    break;
                case "Romanian":
                    Ret = "ro";
                    break;
                case "Slovakian":
                    Ret = "sk";
                    break;
                case "Slovenian":
                    Ret = "sl";
                    break;
                case "Spanish":
                    Ret = "es";
                    break;
                case "Swedish":
                    Ret = "sv";
                    break;
                default:
                    Ret = string.Empty;
                    break;
            }
            return Ret;
        }

        #endregion

    }
}