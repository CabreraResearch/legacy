using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;

/// <summary>
/// Summary description for wsUpdate
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class wsUpdate : System.Web.Services.WebService {

    private CswNbtResources _CswNbtResources;
    public wsUpdate()
    {
        CswSessionResourcesNbt CswInitialization = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
        _CswNbtResources = CswInitialization.CswNbtResources;

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    private string _FilesPath
    {
        get
        {
            return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
        }
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    
}
