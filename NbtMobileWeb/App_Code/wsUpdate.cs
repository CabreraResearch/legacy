using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.Session;

/// <summary>
/// Summary description for wsUpdate
/// </summary>
/// 
[ScriptService]
[WebService( Namespace = "http://localhost/NbtWebApp" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class wsUpdate : System.Web.Services.WebService
{

    private CswNbtWebServiceResources _CswNbtWebServiceResources;
    public wsUpdate()
    {

        _CswNbtWebServiceResources = new CswNbtWebServiceResources( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );

    }

    private string _FilesPath
    {
        get
        {
            return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
        }
    }

    [WebMethod( EnableSession = true )]
    public string ConnectTest()
    {

        try
        {
            _CswNbtWebServiceResources.startSession();
        }

        finally
        {
            _CswNbtWebServiceResources.endSession();
        }

        return ( "Connected" );
    }//

    [WebMethod( EnableSession = true )]
    public string UpdateProperties( string Updates )
    {


        string ReturnVal = string.Empty;

        try
        {
            string UpdatedRowIds = string.Empty;
            string[] UpdateItems = Updates.Split( ';' );
            foreach( string CurrentUpdateItem in UpdateItems )
            {

                string[] ItemBreakdown = CurrentUpdateItem.Split( ',' );
                string ClientRowId = ItemBreakdown[0];
                string PropId = ItemBreakdown[1];
                string Value = ItemBreakdown[2];

                string[] ItemId = PropId.Split( '_' );
                Int32 NodeTypePropId = Convert.ToInt32( ItemId[1] );
                Int32 NodeId = Convert.ToInt32( ItemId[4] );

                CswPrimaryKey CswPrimaryKey = new CswPrimaryKey();
                CswPrimaryKey.FromString( ItemId[1] + "_"  + NodeId );

                CswNbtNode CswNbtNode = _CswNbtWebServiceResources.CswNbtResources.Nodes[CswPrimaryKey];
                CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtWebServiceResources.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                CswNbtNodePropWrapper CswNbtNodePropWrapper = CswNbtNode.Properties[CswNbtMetaDataNodeTypeProp];



                switch( CswNbtNodePropWrapper.FieldType.FieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Text:
                        CswNbtNodePropWrapper.AsText.Text = Value;
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Question:
                        CswNbtNodePropWrapper.AsQuestion.Answer = Value;
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Date:
                        CswNbtNodePropWrapper.AsDate.DateValue = Convert.ToDateTime( Value );
                        break;

                    case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                        CswNbtNodePropWrapper.AsMemo.Text = Value;
                        break;

                    default:
                        throw ( new CswDniException( "Unhandled field type " + CswNbtNodePropWrapper.FieldType.FieldType.ToString() ) );

                }//switch on field type 

                CswNbtNode.postChanges( false );

                if( UpdatedRowIds.Length > 0 )
                {
                    UpdatedRowIds += ",";
                }

                UpdatedRowIds += ClientRowId;


            }//iterate update items


            ReturnVal = UpdatedRowIds;

            _CswNbtWebServiceResources.endSession(); 


        }//try


        catch( Exception Exception ) 
        {
            _CswNbtWebServiceResources.CswNbtResources.CswLogger.reportError( Exception );
        } //tach



       return ( ReturnVal );
    }//

}//wsUpdate
