﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.CISPro;

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
        public CswNbtWebServiceMols.MolDataReturn getMolImgFromText( MolData ImgData )
        {
            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.MolDataReturn, MolData>(
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
        public CswNbtWebServiceMols.StructureSearchDataReturn runStructureSearch( StructureSearchViewData SSViewData )
        {
            CswNbtWebServiceMols.StructureSearchDataReturn Ret = new CswNbtWebServiceMols.StructureSearchDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.StructureSearchDataReturn, StructureSearchViewData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMols.RunStructureSearch,
                ParamObj: SSViewData
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Save mol text" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMols.MolDataReturn saveMolPropText( MolData MolImgData )
        {
            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.MolDataReturn, MolData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMols.SaveMolPropFile,
                ParamObj: MolImgData
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}