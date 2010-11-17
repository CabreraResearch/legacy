using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;   // supports ScriptService attribute
using System.Xml;
using System.Web.Services;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;


/// <summary>
/// ViewListTree Webservice
/// </summary>
[ScriptService]
[WebService( Namespace = "http://localhost/NbtWebApp" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class wsView : System.Web.Services.WebService
{
    private CswNbtResources _CswNbtResources;

    private string _FilesPath
    {
        get
        {
            return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
        }
    }

    public wsView()
    {
        CswSessionResourcesNbt CswInitialization = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
        _CswNbtResources = CswInitialization.CswNbtResources;

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    private static string ViewIdPrefix = "viewid_";
    private static string PropIdPrefix = "prop_";
    private static string TabIdPrefix = "tab_";
    private static string NodeIdPrefix = "nodeid_";
    private static string NodeKeyPrefix = "nodekey_";


    [WebMethod( EnableSession = true )]
    public string Run( string ParentId )
    {
        string ret = string.Empty;
        try
        {
            if( ParentId.StartsWith( ViewIdPrefix ) )
            {
                // Get the full XML for the entire view
                Int32 ViewId = CswConvert.ToInt32( ParentId.Substring( ViewIdPrefix.Length ) );
                CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                //View.SaveToCache();
                //Session["SessionViewId"] = View.SessionViewId;

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );
                if( Tree.getChildNodeCount() > 0 )
                    ret = _runTreeNodesRecursive( Tree );
                else
                {
                    ret = @"<node id="""" name=""No results""></node>";
                }
            }// if( ParentId.StartsWith( ViewIdPrefix ) )
            else
            {
                // All Views
                DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( false );
                foreach( DataRow ViewRow in ViewDT.Rows )
                {
                    ret += "<view id=\"" + ViewIdPrefix + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
                    ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
                    ret += "/>";
                }
            }
        }
        catch( Exception ex )
        {
            ret = "<error>" + ex.Message + "</error>";
        }
        return "<root>" + ret + "</root>";
    }

    private string _runTreeNodesRecursive( ICswNbtTree Tree )
    {
        string ret = string.Empty;
        for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
        {
            Tree.goToNthChild( c );

            CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeId = ThisNode.NodeId.ToString();

            string ThisSubItems = _runTreeNodesRecursive( Tree );
            if( ThisSubItems == string.Empty )
            {
                ThisSubItems = _runProperties( ThisNode );
            }

            ret += "<node id=\"" + NodeIdPrefix + ThisNodeId + "\"";
            ret += " name=\"" + ThisNodeName + "\"";
            ret += " nodetype=\"" + ThisNode.NodeType.NodeTypeName + "\"";
            ret += " objectclass=\"" + ThisNode.ObjectClass.ObjectClass.ToString() + "\"";
            ret += " iconfilename=\"" + ThisNode.NodeType.IconFileName + "\"";
            ret += "><subitems>" + ThisSubItems + "</subitems>";
            ret += "</node>";

            Tree.goToParentNode();
        }
        return ret;
    }


    private string _runProperties( CswNbtNode Node )
    {
        string ret = string.Empty;
        foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
        {
            foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypeProps )
            {
                if( !Prop.HideInMobile &&
                    Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password )
                {
                    CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                    ret += "<prop id=\"" + PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() + "\"";
                    ret += " name=\"" + Prop.PropName + "\"";
                    ret += " tab=\"" + Tab.TabName + "\"";
                    ret += " fieldtype=\"" + Prop.FieldType.FieldType.ToString() + "\"";
                    ret += " gestalt=\"" + PropWrapper.Gestalt.Replace( "\"", "&quot;" ) + "\"";
                    ret += " ocpname=\"" + PropWrapper.ObjectClassPropName + "\"";
                    ret += ">";
                    XmlDocument XmlDoc = new XmlDocument();
                    CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
                    PropWrapper.ToXml( XmlDoc.DocumentElement );
                    ret += XmlDoc.DocumentElement.InnerXml;
                    ret += "<subitems></subitems>";
                    ret += "</prop>";
                }
            }
        }

        return ret;
    }


} // wsView

