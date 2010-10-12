using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;          // supports WebMethod attribute
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Web.Services.Protocols;
using System.IO;
using System.Data;
using System.Xml;
using System.Collections.Generic;   // supports IDictionary
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.Config;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.Security;
using Telerik.Web.UI;
using ChemSW.Config;
using ChemSW.Exceptions;
using ChemSW.Session;
using ChemSW.CswWebControls;
using System.Xml;


/// <summary>
/// NodeTypeTree WebService
/// </summary>
[ScriptService]
[WebService( Namespace = "http://localhost/NbtWebApp" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class MobileService : System.Web.Services.WebService
{
    public MobileService()
    {
    }//ctor

    CswNbtResources _CswNbtResources = null;
    CswSessionResourcesNbt _CswSessionResourcesNbt = null;
    CswSessionManager _CswSessionManager = null;


    //Dimitri: we need to have a check credientials method 
    //so that the HH can check once and then use those 
    //credentials throughout the session.
    private void _init( string AccessId, string UserName, string Password )
    {
        _CswSessionResourcesNbt = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, AccessId, CswNbtWebTools.getFilePath(), SetupMode.Web );

        _CswSessionManager = _CswSessionResourcesNbt.CswSessionManager;

        _CswNbtResources = _CswSessionResourcesNbt.CswNbtResources;


        ChemSW.Security.AuthenticationStatus AuthenticationStatus;
        if ( ChemSW.Security.AuthenticationStatus.Authenticated != ( AuthenticationStatus = _CswSessionManager.Authenticate( UserName, Password, CswNbtWebTools.getIpAddress() ) ) )
        {
            throw ( new CswDniException( "Authentication Error: " + AuthenticationStatus.ToString() ) );
        }
    }//_init() 

    private void _deInit()
    {
        _CswSessionManager.DeAuthenticate();
        _CswNbtResources.finalize();
        _CswNbtResources.release();
    }//_deinit()

    [WebMethod( EnableSession = true )]
    public DataSet GetTestDs( string AccessId, string UserName, string Password )
    {
        DataSet ReturnVal = null;
        try
        {
            _init( AccessId, UserName, Password );

            CswNbtView FeView = new CswNbtView( _CswNbtResources );

            if ( FeView.LoadView( "Equipment" ) )
            {
                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( FeView, false, true, false, true );
                CswNbtTreeDs CswNbtTreeDs = new CswNbtTreeDs( _CswNbtResources );
                ReturnVal = CswNbtTreeDs.TreeToDataSet(  CswNbtTree ); //Dimitri: This should take a tree param rather than in the ctor
            }

        }//try 

        catch ( Exception Exception )
        {
            _CswNbtResources.logError( Exception );
            throw ( Exception );
        }


        finally
        {
            _deInit();

        }
        return ( ReturnVal );
    }

    [WebMethod( EnableSession = true )]
    public void PutTestDs( string AccessId, string UserName, string Password, DataSet DataSet )
    {
        try
        {
            _init( AccessId, UserName, Password );
            CswNbtTreeDs CswNbtTreeDs = new CswNbtTreeDs( _CswNbtResources  );
            CswNbtTreeDs.writeTreeDs( DataSet ); 

        }

        catch ( Exception Exception )
        {
            _CswNbtResources.logError( Exception );
            throw ( Exception );
        }

        finally
        {
            _deInit();

        }

    }//PutTestDs() 

    [WebMethod( EnableSession = true )]
    public DataSet GetFeDs( string AccessId, string UserName, string Password )
    {

        DataSet ReturnVal = null;
        try
        {
            //Vlad: Since the code is based on the structure of the DS, we need to be building 
            //up these views in code
            _init( AccessId, UserName, Password );
            CswNbtView FeView = new CswNbtView( _CswNbtResources );


            if ( FeView.LoadView( "Roles and Users" ) )//Vlad: This needs to be whatever the real FE view is
            {
                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( FeView, false, true, false, true );
                CswNbtTreeDs CswNbtTreeDs = new CswNbtTreeDs( _CswNbtResources );
                ReturnVal = CswNbtTreeDs.TreeToDataSet( CswNbtTree );
            }

        }//try 

        catch ( Exception Exception )
        {
            _CswNbtResources.logError( Exception );
            throw ( Exception );
        }

        finally
        {
            _deInit();

        }

        return ( ReturnVal );

    } // GetNodeChildren()

} // MobileService
