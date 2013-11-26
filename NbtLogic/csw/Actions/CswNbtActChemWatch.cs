
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ChemWatchAuthServices;
using ChemSW.Nbt.ChemWatchCommonServices;
using ChemSW.Nbt.ChemWatchDocumentServices;
using ChemSW.Nbt.ChemWatchMaterialServices;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.Actions.ChemWatch;
using NbtWebApp.WebSvc.Logic.Actions.ChemWatch;
using Country = ChemSW.Nbt.ChemWatchCommonServices.Country;
using Language = ChemSW.Nbt.ChemWatchCommonServices.Language;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActChemWatch
    {
        private const string _chemWatchUserName = "chemswt";
        private const string _chemWatchPassword = "1107ms";

        private static readonly CookieManagerBehavior _cookieBehavior = new CookieManagerBehavior(); //All ChemWatch service clients must share this

        public static CswNbtChemWatchRequest Initialize( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtObjClassChemical ChemicalNode = NbtResources.Nodes[Request.NbtMaterialId]; //TODO: should we verify the Request.NodeId is of a Chemical?

            Return.Supplier = ChemicalNode.Supplier.CachedNodeName;
            Return.PartNo = ChemicalNode.PartNumber.Text;
            Return.MaterialName = ChemicalNode.TradeName.Text;
            Return.NbtMaterialId = ChemicalNode.NodeId;

            string errorMsg;
            if( _authenticate( out errorMsg ) )
            {
                CommonServiceClient cwCommonClient = new CommonServiceClient();
                cwCommonClient.Endpoint.Behaviors.Add( _cookieBehavior );

                // Populate Language list
                Languages cwLanguages = cwCommonClient.GetLanguages();
                foreach( Language cwLanguage in cwLanguages )
                {
                    Return.Languages.Add( new ChemWatchMultiSlctListItem()
                        {
                            Name = cwLanguage.Name,
                            Id = CswConvert.ToString( cwLanguage.Id )
                        } );
                }

                // Populate Country list
                Countries cwCountries = cwCommonClient.GetCountries();
                foreach( Country cwCountry in cwCountries )
                {
                    Return.Countries.Add( new ChemWatchMultiSlctListItem()
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

            return Return;
        }

        public static CswNbtChemWatchRequest MaterialSearch( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
            string errorMsg;

            if( _authenticate( out errorMsg ) )
            {
                MaterialServiceClient cwMaterialClient = new MaterialServiceClient();
                cwMaterialClient.Endpoint.Behaviors.Add( _cookieBehavior );

                ListResultOfMaterial cwMaterials = cwMaterialClient.GetMaterialsByVendorGroupId( Request.Supplier, Request.MaterialName, Request.PartNo, false, 1, 100, "", 0 );
                foreach( Material cwMaterial in cwMaterials.Rows )
                {
                    Return.Materials.Add( new ChemWatchListItem()
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

            return Return;
        }

        public static CswNbtChemWatchRequest SDSDocumentSearch( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
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
                    SDSDoc.ExternalUrl = Doc.ExternalUrl;
                    Return.SDSDocuments.Add( SDSDoc );
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }

            return Return;
        }

        public static CswNbtChemWatchRequest GetSDSDocument( ICswResources CswResources, string filename )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
            string errorMsg;

            if( _authenticate( out errorMsg ) )
            {
                DocumentServiceClient cwDocClient = new DocumentServiceClient();
                cwDocClient.Endpoint.Behaviors.Add( _cookieBehavior );
                try
                {
                    Stream DocStream = cwDocClient.GetDocumentContent( filename );
                    Return.SDSDocument = DocStream;
                }
                catch( Exception ex )
                {
                    Return.Message = ex.Message;
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }

            return Return;
        }

        public static CswNbtChemWatchRequest GetMatchingSuppliers( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
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

            return Return;
        }

        public static CswNbtChemWatchRequest CreateSDSDocuments( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass SDSDocumentOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );

            foreach( ChemWatchSDSDoc SDSDoc in Request.SDSDocuments )
            {
                string Language = _getLanguage( SDSDoc.Language );
                string FileName = SDSDoc.FileName;
                string Country = SDSDoc.Country;
                CswNbtResources.Nodes.makeNodeFromNodeTypeId( SDSDocumentOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassSDSDocument NewSDSDocNode = NewNode;
                        NewSDSDocNode.FileType.Value = CswNbtObjClassSDSDocument.CswEnumDocumentFileTypes.ChemWatch;
                        NewSDSDocNode.ChemWatch.Text = FileName;
                        NewSDSDocNode.Language.Value = Language;
                        NewSDSDocNode.Country.Text = Country;
                        NewSDSDocNode.Owner.RelatedNodeId = Request.NbtMaterialId;
                    } );
            }

            return Return;
        }

        #region Private Helper Methods

        private static void _getMatchingSuppliers( string SearchString, CswNbtChemWatchRequest Return )
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
                ChemWatchListItem cwSupplier = new ChemWatchListItem();
                cwSupplier.Name = cwVendor.Name;
                cwSupplier.Id = cwVendor.VendorGroup.Gid;
                if( false == Return.Suppliers.Any( supplier => supplier.Id == cwVendor.VendorGroup.Gid ) )
                {
                    Return.Suppliers.Add( cwSupplier );
                }
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
