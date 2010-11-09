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

/// <summary>
/// Summary description for wsUpdate
/// </summary>
/// 
[ScriptService]
[WebService( Namespace = "http://localhost/NbtWebApp" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class wsUpdate : System.Web.Services.WebService {

    private CswNbtResources _CswNbtResources;
    private CswSessionResourcesNbt _CswInitialization; //will need this for case 20095
    public wsUpdate()
    {
        _CswInitialization = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
        _CswNbtResources = _CswInitialization.CswNbtResources;



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

    [WebMethod( EnableSession = true )]
    public string ConnectTest()
    {
        return ( "Connected" );
    }//

    [WebMethod( EnableSession = true )]
    public string UpdateProperties( string Updates )
    {
        string ReturnVal = string.Empty;

        string UpdatedRowIds = string.Empty;
        string [] UpdateItems = Updates.Split( ';' );
        foreach( string CurrentUpdateItem in UpdateItems )
        {

            try
            {

                string[] ItemBreakdown = CurrentUpdateItem.Split( ',' );
                string ClientRowId = ItemBreakdown[0] ;
                string PropId = ItemBreakdown[1];
                string Value = ItemBreakdown[2];

                string[] ItemId = PropId.Split( '_' );
                Int32 NodeTypePropId = Convert.ToInt32( ItemId[1] );
                Int32 NodeId = Convert.ToInt32( ItemId[4] );

                CswPrimaryKey CswPrimaryKey = new CswPrimaryKey(); 
                CswPrimaryKey.FromString(  "nodes_" + NodeId  );

                CswNbtNode CswNbtNode = _CswNbtResources.Nodes[CswPrimaryKey];
                CswNbtMetaDataNodeTypeProp  CswNbtMetaDataNodeTypeProp  = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
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
                        throw ( new CswDniException( "Unhandled field type " + CswNbtNodePropWrapper.FieldType.FieldType .ToString() ) ); 

                }//switch on field type 

                CswNbtNode.postChanges( false );

                if( UpdatedRowIds.Length > 0 )
                {
                    UpdatedRowIds += ",";
                }

                UpdatedRowIds += ClientRowId; 

            }

            catch( Exception Exception )
            {
                //not sure yet what to do with these exceptions; see bz # 20080
            }

        }//iterate update items

        try
        {

            _CswNbtResources.finalize();

            ReturnVal = UpdatedRowIds;
//            ReturnVal = UpdatedRowIds;

        }

        catch( Exception Exception )
        {
        }//try-catch

        return ( ReturnVal );
    }//

}//wsUpdate
