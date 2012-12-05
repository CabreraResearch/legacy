using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.CISPro;
using System.IO;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Structure Searching Mols
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Mol
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "getImg" )]
        [Description( "Get an img of a mol" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMols.MolDataReturn getMolImgFromText( MolData.MolImgData ImgData )
        {
            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.MolDataReturn, MolData.MolImgData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMols.getMolImg,
                ParamObj: ImgData
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "runStructureSearch" )]
        [Description( "Get an img of a mol" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMols.MolDataReturn runStructureSearch( MolData.StructureSearchViewData SSViewData )
        {
            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.MolDataReturn, MolData.StructureSearchViewData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMols.RunStructureSearch,
                ParamObj: SSViewData
                );

            SvcDriver.run();
            return ( Ret );
        }

    }
}