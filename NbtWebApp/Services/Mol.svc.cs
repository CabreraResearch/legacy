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
using System.Text.RegularExpressions;

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
        [WebInvoke( Method = "POST", UriTemplate = "getImgFromFile" )]
        [Description( "Get an img of a mol" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMols.MolDataReturn getMolImgFromFile( Stream dataStream )
        {
            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            MultiPartFormDataFile mpfdf = new MultiPartFormDataFile( dataStream );
            MolData ImgData = new MolData();
            ImgData.molString = mpfdf.FileContents;

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

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Save a mol file" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMols.MolDataReturn saveMolPropFile( Stream dataStream )
        {

            MultiPartFormDataFile mpfdf = new MultiPartFormDataFile( dataStream );
            MolData molImgData = new MolData();
            molImgData.molString = mpfdf.FileContents;
            molImgData.propId = _Context.Request.QueryString["PropId"];

            CswNbtWebServiceMols.MolDataReturn Ret = new CswNbtWebServiceMols.MolDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMols.MolDataReturn, MolData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMols.SaveMolPropFile,
                ParamObj: molImgData
                );

            SvcDriver.run();
            return ( Ret );
        }

        public class MultiPartFormDataFile
        {
            public string ContentDisposition;
            public string ContentType;
            public string Filename;
            public string FileContents;
            public string ParamName;

            public MultiPartFormDataFile( Stream stream )
            {
                using( StreamReader reader = new StreamReader( stream ) )
                {
                    while( reader.Peek() >= 0 )
                    {
                        string line = reader.ReadLine();
                        if( line.Contains( "Content-Disposition" ) )
                        {
                            string[] split = line.Split( ';' );
                            foreach( string s in split )
                            {
                                if( s.Contains( "Content-Disposition" ) )
                                {
                                    ContentDisposition = s.Split( ' ' )[1].Replace( ';', ' ' );
                                }
                                else if( s.Contains( "filename" ) )
                                {
                                    Filename = s.Substring( s.LastIndexOf( ':' ) + 1 ).Trim();
                                }
                            }
                        }
                        else if( line.Contains( "Content-Type" ) )
                        {
                            ContentType = Filename = line.Substring( line.LastIndexOf( ':' ) + 1 ).Trim();
                            string rawFileContent = reader.ReadToEnd();
                            FileContents = Regex.Replace( rawFileContent, @"-*WebKit.*", "" );
                        }
                    }
                }
            }

        }

    }
}