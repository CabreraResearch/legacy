using System;
using ChemSW;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.ChemWatchAuthServices;
using NbtWebApp.ChemWatchCommonServices;
using NbtWebApp.WebSvc.Logic.Actions.ChemWatch;

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

            Request.Supplier = ChemicalNode.Supplier.CachedNodeName;
            Request.PartNo = ChemicalNode.PartNumber.Text;
            Request.MaterialName = ChemicalNode.TradeName.Text;

            string errorMsg;
            if( _authenticate(out errorMsg) )
            {
                CommonServiceClient cwCommonClient = new CommonServiceClient();
                cwCommonClient.Endpoint.Behaviors.Add( _cookieBehavior );

                Languages cwLanguages = cwCommonClient.GetLanguages();
                foreach( Language cwLanguage in cwLanguages )
                {
                    Return.Data.Suppliers.Add(new CswNbtChemWatchListItem()
                        {
                            Name = cwLanguage.Name,
                            Id = cwLanguage.Id
                        });
                }

                Countries cwCountries = cwCommonClient.GetCountries();
                foreach( Country cwCountry in cwCountries )
                {
                    Return.Data.Suppliers.Add( new CswNbtChemWatchListItem()
                    {
                        Name = cwCountry.Name,
                        Id = cwCountry.Id
                    } );
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "There was a problem authenticating with ChemWatch", errorMsg );
            }
        }
        
        private static bool _authenticate(out string ErrorMsg)
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