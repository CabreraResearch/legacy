
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChemSW.Config;
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
        //Leaving these here for testing purposes -- this is David's account
        // private const string _chemWatchUserName = "Admin";
        // private const string _chemWatchPassword = "1107ms";
        // private const string _chemWatchDomain = "chemswt";

        private static readonly CookieManagerBehavior _cookieBehavior = new CookieManagerBehavior(); //All ChemWatch service clients must share this

        public static CswNbtChemWatchRequest Initialize( ICswResources CswResources, CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchRequest Return = new CswNbtChemWatchRequest();
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtObjClassChemical ChemicalNode = NbtResources.Nodes[Request.NbtMaterialId];

            Return.Supplier = ChemicalNode.Supplier.CachedNodeName;
            Return.PartNo = ChemicalNode.PartNumber.Text;
            Return.MaterialName = ChemicalNode.TradeName.Text;
            Return.NbtMaterialId = ChemicalNode.NodeId;

            string errorMsg;
            if( _authenticate( NbtResources, out errorMsg ) )
            {
                CommonServiceClient cwCommonClient = new CommonServiceClient();
                cwCommonClient.Endpoint.Behaviors.Add( _cookieBehavior );

                // Populate Language list
                List<ChemWatchMultiSlctListItem> Languages = new List<ChemWatchMultiSlctListItem>();
                Languages cwLanguages = cwCommonClient.GetLanguages();
                foreach( Language cwLanguage in cwLanguages )
                {
                    if( cwLanguage.UILanguage )
                    {
                        Languages.Add( new ChemWatchMultiSlctListItem()
                            {
                                Name = cwLanguage.Name,
                                Id = CswConvert.ToString( cwLanguage.Id )
                            } );
                    }
                }
                IEnumerable<ChemWatchMultiSlctListItem> SortedLanguages = Languages.OrderBy( si => si.Name );
                Return.Languages.options = SortedLanguages.ToList();

                // Populate Country list
                List<ChemWatchMultiSlctListItem> Countries = new List<ChemWatchMultiSlctListItem>();
                Countries cwCountries = cwCommonClient.GetCountries();
                foreach( Country cwCountry in cwCountries )
                {
                    Countries.Add( new ChemWatchMultiSlctListItem()
                    {
                        Name = cwCountry.Name,
                        Id = CswConvert.ToString( cwCountry.Id )
                    } );
                }
                IEnumerable<ChemWatchMultiSlctListItem> SortedCountries = Countries.OrderBy( si => si.Name );
                Return.Countries.options = SortedCountries.ToList();

                // Attempt to populate the Suppliers list
                _getMatchingSuppliers( Return.Supplier, Return );
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
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string errorMsg;

            if( _authenticate( NbtResources, out errorMsg ) )
            {
                MaterialServiceClient cwMaterialClient = new MaterialServiceClient();
                cwMaterialClient.Endpoint.Behaviors.Add( _cookieBehavior );

                List<ChemWatchListItem> Materials = new List<ChemWatchListItem>();
                ListResultOfMaterial cwMaterials = cwMaterialClient.GetMaterialsByVendorGroupId( Request.Supplier, Request.MaterialName, Request.PartNo, false, 1, 100, "", 0 );
                foreach( Material cwMaterial in cwMaterials.Rows )
                {
                    Materials.Add( new ChemWatchListItem()
                        {
                            Id = CswConvert.ToString( cwMaterial.MaterialID ),
                            Name = cwMaterial.Name
                        } );
                }
                IEnumerable<ChemWatchListItem> SortedMaterials = Materials.OrderBy( si => si.Name );
                Return.Materials = SortedMaterials.ToList();
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
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string errorMsg;

            if( _authenticate( NbtResources, out errorMsg ) )
            {
                DocumentServiceClient cwDocClient = new DocumentServiceClient();
                cwDocClient.Endpoint.Behaviors.Add( _cookieBehavior ); //every service client needs to share this

                DocumentRequest DocumentRequest = new DocumentRequest();

                List<int> CountryIdsList = Request.Countries.selected.Select( ListItem => CswConvert.ToInt32( ListItem.Id ) ).ToList();
                int[] CountryIdsArray = CountryIdsList.ToArray();
                DocumentRequest.CountryCode = CountryIdsArray;

                List<int> LanguageIdsList = Request.Languages.selected.Select( ListItem => CswConvert.ToInt32( ListItem.Id ) ).ToList();
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
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string errorMsg;

            if( _authenticate( NbtResources, out errorMsg ) )
            {
                DocumentServiceClient cwDocClient = new DocumentServiceClient();
                cwDocClient.Endpoint.Behaviors.Add( _cookieBehavior );
                Stream DocStream = null;

                try
                {
                    DocStream = cwDocClient.GetDocumentContent( filename );
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

            if( _authenticate( NbtResources, out errorMsg ) )
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

            List<ChemWatchListItem> Suppliers = new List<ChemWatchListItem>();
            ListResultOfVendor cwVendors = cwMaterialClient.SearchVendors( cwSearchVend );
            foreach( Vendor cwVendor in cwVendors.Rows )
            {
                ChemWatchListItem cwSupplier = new ChemWatchListItem();
                cwSupplier.Name = cwVendor.Name;
                cwSupplier.Id = cwVendor.VendorGroup.Gid;
                if( false == Suppliers.Any( supplier => supplier.Id == cwVendor.VendorGroup.Gid && supplier.Name == cwVendor.Name ) )
                {
                    Suppliers.Add( cwSupplier );
                }
            }
            IEnumerable<ChemWatchListItem> SortedSuppliers = Suppliers.OrderBy( si => si.Name );
            Return.Suppliers = SortedSuppliers.ToList();
        }

        private static bool _authenticate( CswNbtResources CswNbtResources, out string ErrorMsg )
        {
            ErrorMsg = "";
            bool ret = false;
            try
            {
                string cwUsername = CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.ChemWatchUsername );
                string cwPassword = CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.ChemWatchPassword );
                string cwDomain = CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.ChemWatchDomain );

                AuthenticateServiceClient cwAuthClient = new AuthenticateServiceClient();
                cwAuthClient.Endpoint.Behaviors.Add( _cookieBehavior );
                UserCredential cwUserCredential = new UserCredential()
                    {
                        UserName = cwUsername,
                        Password = cwPassword,
                        //TODO: Uncomment below when Alexei from ChemWatch provides us with new Authentication service
                        //Domain = cwDomain
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
