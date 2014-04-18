using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.csw.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceC3Search
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceC3Search( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region Data Contracts

        [DataContract]
        public class CswNbtC3SearchReturn: CswWebSvcReturn
        {
            public CswNbtC3SearchReturn()
            {
                Data = new C3SearchResponse();
            }

            [DataMember]
            public C3SearchResponse Data;
        }

        [DataContract]
        public class CswNbtC3CreateMaterialReturn: CswWebSvcReturn
        {
            public CswNbtC3CreateMaterialReturn()
            {
                Data = new C3CreateMaterialResponse();
            }

            [DataMember]
            public C3CreateMaterialResponse Data;
        }

        #endregion

        public static JObject getImportBtnItems( CswNbtResources CswNbtResources )
        {
            JObject ImportableNodeTypes = new JObject();

            Collection<CswEnumNbtObjectClass> MaterialPropSetMembers = CswNbtPropertySetMaterial.Members();
            foreach( CswEnumNbtObjectClass ObjectClassName in MaterialPropSetMembers )
            {
                CswNbtMetaDataObjectClass ObjectClass = CswNbtResources.MetaData.getObjectClass( ObjectClassName );
                foreach( CswNbtMetaDataNodeType CurrentNT in ObjectClass.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp IsConstituentNTP = CurrentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent );
                    if( IsConstituentNTP.HasDefaultValue() &&
                        CswEnumTristate.False == IsConstituentNTP.getDefaultValue( false ).AsLogical.Checked )
                    {
                        JObject NodeType = new JObject();
                        ImportableNodeTypes[CurrentNT.NodeTypeName] = NodeType;
                        NodeType["nodetypename"] = CurrentNT.NodeTypeName;
                        NodeType["nodetypeid"] = CurrentNT.NodeTypeId.ToString();
                        NodeType["iconfilename"] = CswNbtMetaDataObjectClass.IconPrefix16 + CurrentNT.IconFileName;
                        NodeType["objclass"] = ObjectClassName.ToString();
                    }
                }
            }

            return ImportableNodeTypes;
        }//getImportBtnItems()

        /// <summary>
        /// Returns either C3 or ACD depending on which module is enabled.
        /// </summary>
        /// <param name="CswResources"></param>
        /// <param name="Return"></param>
        /// <param name="EmptyObject"></param>
        public static void GetC3DataService( ICswResources CswResources, CswNbtC3SearchReturn Return, object EmptyObject )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            Return.Data.DataService = CswNbtC3ClientManager.DataService;
        }//getC3DataService()

        public static void GetVendorOptions( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            if( CswNbtC3ClientManager.DataService == "C3" )
            {
                Return.Data.VendorListOptions = _getAvailableDataSources( CswNbtC3ClientManager, CswC3Params );
            }
            else if( CswNbtC3ClientManager.DataService == "ACD" )
            {
                Return.Data.VendorListOptions = _getPreferredSuppliers( _CswNbtResources );
            }
        }

        public static void GetACDSuppliers( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswRetObjSearchResults Results = C3SearchClient.getACDSuppliers( CswC3Params );

                // Get the already set perferred suppliers so we can select them
                List<string> PreferredSuppliers = new List<string>();
                CswNbtObjClassUser CurrentUser = _CswNbtResources.Nodes.GetNode( _CswNbtResources.CurrentNbtUser.UserId );
                if( null != CurrentUser )
                {
                    PreferredSuppliers = CurrentUser.C3ACDPreferredSuppliers.Text.Split( ',' ).ToList();
                }

                Collection<ACDSupplier> ACDSuppliers = new Collection<ACDSupplier>();
                IEnumerable<ACDSupplier> SortedSuppliers = new Collection<ACDSupplier>();
                if( null != Results )
                {
                    foreach( CswC3ACDResponseACDSupplier ACDSupplier in Results.ACDSuppliers )
                    {
                        ACDSupplier NewSupplier = new ACDSupplier();
                        NewSupplier.Name = ACDSupplier.Name + ": " + ACDSupplier.Country;
                        NewSupplier.Id = CswConvert.ToString( ACDSupplier.Id );
                        NewSupplier.Selected = PreferredSuppliers.Contains( ACDSupplier.Id.ToString() );
                        ACDSuppliers.Add( NewSupplier );
                    }

                    SortedSuppliers = ACDSuppliers.OrderBy( si => si.Name );
                }

                Return.Data.ACDSuppliers = SortedSuppliers;
            }
        }//GetACDSuppliers()

        public static void UpdateACDPrefSuppliers( ICswResources CswResources, CswNbtC3SearchReturn Return, string PrefSupplierIds )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            if( null == PrefSupplierIds )
            {
                PrefSupplierIds = "";
            }

            CswNbtObjClassUser CurrentUser = _CswNbtResources.Nodes.GetNode( _CswNbtResources.CurrentNbtUser.UserId );
            if( null != CurrentUser )
            {
                CurrentUser.C3ACDPreferredSuppliers.Text = PrefSupplierIds;
                CurrentUser.C3ACDPreferredSuppliers.SyncGestalt();
                CurrentUser.postChanges( false );
            }

        }//UpdateACDPrefSuppliers()

        public static void GetSearchProperties( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            List<string> SearchProperties = new List<string>()
                ;
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
            {
                SearchProperties = ( from ACDSearchParams.SearchFieldType SearchType in Enum.GetValues( typeof( ACDSearchParams.SearchFieldType ) ) select SearchType.ToString() ).ToList();
            }

            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3Products ) )
            {
                SearchProperties = ( from C3SearchParams.SearchFieldType SearchType in Enum.GetValues( typeof( C3SearchParams.SearchFieldType ) ) select SearchType.ToString() ).ToList();
            }

            Return.Data.SearchProperties = SearchProperties;
        }//GetSearchProperties()

        public static void RunC3FilteredSearch( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            JObject Ret = new JObject();
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswRetObjSearchResults SearchResults;
                try
                {
                    SearchResults = C3SearchClient.searchFiltered( CswC3SearchParams );
                }
                catch( Exception exception )
                {
                    throw ( new CswDniException( CswEnumErrorType.Error, "There was an error searching ChemCatCentral", exception.Message, exception ) );
                }

                ExternalImageRet ImageRet = new ExternalImageRet();

                /*
                 * We have to create our own Acd Search params and set Product Id to Int32.MinVal because C3 is serializing ProductId to "0" instead of MinVal.
                 */
                ACDSearchParams acdSearchParams = new ACDSearchParams();
                acdSearchParams.ProductId = Int32.MinValue;
                acdSearchParams.Cdbregno = CswC3SearchParams.ACDSearchParams.Cdbregno;
                getExternalImage( CswResources, ImageRet, acdSearchParams );
                string imageStrBase64 = "data:image/png;base64," + Convert.ToBase64String( ImageRet.Data );

                CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null, Int32.MinValue );
                Ret["table"] = wsTable.getTable( SearchResults, CswC3SearchParams.Field, CswNbtC3ClientManager.DataService, true );
                Ret["filters"] = "";
                Ret["searchterm"] = CswC3SearchParams.Query;
                Ret["field"] = CswC3SearchParams.Field;
                Ret["filtersapplied"] = "";
                Ret["sessiondataid"] = "";
                Ret["searchtarget"] = "chemcatcentral";
                Ret["c3dataservice"] = CswNbtC3ClientManager.DataService;
                Ret["filtered"] = true;
                Ret["staticbase64imagestr"] = imageStrBase64;
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
                {
                    Ret["prefsuppliers"] = CswC3SearchParams.ACDSearchParams.CompanyIds;
                }

                Return.Data.SearchResults = Ret.ToString();
            }
        }//RunC3FilteredSearch()

        public static void GetC3ProductDetails( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                // For now, the only option here is C3
                CswC3SearchParams.DataService = CswNbtC3ClientManager.DataService;

                CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
                if( SearchResults.CswC3SearchResults.Length > 0 )
                {
                    CswC3Product C3ProductDetails = SearchResults.CswC3SearchResults[0];
                    Return.Data.ProductDetails = C3ProductDetails;
                }
            }
        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            JObject Ret = new JObject();
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswRetObjSearchResults SearchResults;

                try
                {
                    SearchResults = C3SearchClient.search( CswC3SearchParams );
                }
                catch( TimeoutException TimeoutException )
                {
                    const string WarningMessage = "The search has timed out. Please use more specific search terms.";
                    throw ( new CswDniException( CswEnumErrorType.Warning, WarningMessage, WarningMessage, TimeoutException ) );
                }

                CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null, Int32.MinValue );
                Ret["table"] = wsTable.getTable( SearchResults, CswC3SearchParams.Field, CswNbtC3ClientManager.DataService );
                Ret["filters"] = "";
                Ret["searchterm"] = CswC3SearchParams.Query;
                Ret["field"] = CswC3SearchParams.Field;
                Ret["filtersapplied"] = "";
                Ret["sessiondataid"] = "";
                Ret["searchtarget"] = "chemcatcentral";
                Ret["c3dataservice"] = CswNbtC3ClientManager.DataService;
                Ret["filtered"] = "C3" == CswNbtC3ClientManager.DataService;
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
                {
                    Ret["prefsuppliers"] = CswC3SearchParams.ACDSearchParams.CompanyIds;
                }

                Return.Data.SearchResults = Ret.ToString();
            }
        }

        public static void getExternalImage( ICswResources CswResources, ExternalImageRet Return, ACDSearchParams ACDSearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswC3SearchParams.ACDSearchParams = ACDSearchParams;
            CswC3Product C3ProductDetails = new CswC3Product();

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswRetObjSearchResults SearchResults = C3SearchClient.getACDMolImage( CswC3SearchParams );
                if( null != SearchResults.CswC3SearchResults && SearchResults.CswC3SearchResults.Length > 0 )
                {
                    C3ProductDetails = SearchResults.CswC3SearchResults[0];
                }
            }

            if( String.IsNullOrEmpty( C3ProductDetails.MolImage ) )
            {
                CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                Return.Data = File.ReadAllBytes( AppDomain.CurrentDomain.BaseDirectory + CswNbtMetaDataObjectClass.IconPrefix100 + ChemicalOC.IconFileName );
            }
            else
            {
                Return.Data = Convert.FromBase64String( C3ProductDetails.MolImage );
            }
        }

        public static void importC3Product( ICswResources CswResources, CswNbtC3CreateMaterialReturn Return, CswNbtC3Import.Request Request )
        {
            Return.Data = CswNbtSdC3.importC3Product( CswResources, Request );
        }//importC3Product()

        public static void CreateMaterialRequestFromC3( ICswResources CswResources, CswNbtC3CreateMaterialReturn Return, CswNbtC3Import.Request Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtMetaDataObjectClass RequestItemOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType FirstRequestItemNT = RequestItemOC.getNodeTypes().FirstOrDefault();
            if( null == FirstRequestItemNT )
            {
                throw new CswDniException(CswEnumErrorType.Error, "Cannot create a Create Material Request because there are no Request Item Types", "Cannot create a Create Material Request when there are no Request Item Nodetypes");
            }

            CswNbtNode RequestNode = NbtResources.Nodes.makeNodeFromNodeTypeId( FirstRequestItemNT.NodeTypeId, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassRequestItem AsRequestItem = NewNode;
                    AsRequestItem.Type.Value = CswNbtObjClassRequestItem.Types.MaterialCreate;
                    AsRequestItem.C3CDBRegNo.Value = Request.Cdbregno;
                    AsRequestItem.C3ProductId.Value = Request.C3ProductId;
                } );
        }

        #region Private Helper Methods

        private static Collection<VendorOption> _getPreferredSuppliers( CswNbtResources CswNbtResources )
        {
            Collection<VendorOption> ACDVendorOptions = new Collection<VendorOption>();

            // We want the 'Preferred' option to default to selected _if_ there are preferred suppliers set on the User
            bool PreferredOptionSelected = false;
            string PreferredSuppliers = _getCurrentUserPrefSuppliers( CswNbtResources );
            if( false == string.IsNullOrEmpty( PreferredSuppliers ) )
            {
                PreferredOptionSelected = true;
            }

            ACDVendorOptions.Add( new VendorOption
            {
                value = "",
                display = "Any Suppliers",
                isSelected = !PreferredOptionSelected
            } );

            ACDVendorOptions.Add( new VendorOption
            {
                value = PreferredSuppliers,
                display = "Preferred Suppliers",
                isSelected = PreferredOptionSelected
            } );

            return ACDVendorOptions;
        }//_getPreferredSuppliers()

        private static Collection<VendorOption> _getAvailableDataSources( CswNbtC3ClientManager CswNbtC3ClientManager, CswC3Params CswC3Params )
        {
            Collection<VendorOption> AvailableDataSources = new Collection<VendorOption>();

            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswRetObjSearchResults SourcesList = C3SearchClient.getDataSources( CswC3Params );
                if( null != SourcesList )
                {
                    //Create the "All Sources" option
                    CswCommaDelimitedString AllSources = new CswCommaDelimitedString();
                    AllSources.FromArray( SourcesList.AvailableDataSources );

                    VendorOption allSourcesDs = new VendorOption();
                    allSourcesDs.value = AllSources.ToString();
                    allSourcesDs.display = "All Sources";
                    AvailableDataSources.Add( allSourcesDs );

                    //Add available data source options
                    foreach( string DataSource in SourcesList.AvailableDataSources )
                    {
                        VendorOption dS = new VendorOption();
                        dS.value = DataSource;
                        dS.display = DataSource;
                        AvailableDataSources.Add( dS );
                    }

                }//if( null != SourcesList )

            }//if( null != C3SearchClient )

            return AvailableDataSources;
        }//_getAvailableDataSources()

        private static string _getCurrentUserPrefSuppliers( CswNbtResources CswNbtResources )
        {
            string Ret = string.Empty;

            CswNbtObjClassUser CurrentUser = CswNbtResources.Nodes.GetNode( CswNbtResources.CurrentNbtUser.UserId );
            Ret = CurrentUser.C3ACDPreferredSuppliers.Text;

            return Ret;
        }//_getCurrentUserPrefSuppliers

        #endregion Private Helper Methods

    }

}