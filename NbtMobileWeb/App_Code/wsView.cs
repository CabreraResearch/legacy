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
                ret += _runTreeNodesRecursive( Tree );

            }// if( ParentId.StartsWith( ViewIdPrefix ) )
            else
            {
                // All Views
                DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( false );
                foreach( DataRow ViewRow in ViewDT.Rows )
                {
                    ret += _makeItem( ViewIdPrefix + CswConvert.ToInt32( ViewRow["nodeviewid"] ),
                                      ViewRow["viewname"].ToString(),
                                      string.Empty,
                                      true );
                }
            }
        }
        catch( Exception ex )
        {
            throw ex;
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
                ThisSubItems = _runTabs( Tree, ThisNode );

            ret += _makeItem( NodeIdPrefix + ThisNodeId,
                              ThisNodeName,
                              ThisSubItems,
                              true );

            Tree.goToParentNode();
        }
        return ret;
    }


    private string _runTabs( ICswNbtTree Tree, CswNbtNode Node )
    {
        string ret = string.Empty;
        if( Node.NodeType.NodeTypeTabs.Count > 1 )
        {
            foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
            {
                ret += _makeItem( TabIdPrefix + Tab.TabId + "_" + NodeIdPrefix + Node.NodeId.ToString(),
                                  Tab.TabName,
                                  _runProperties( Node, Tab ),
                                  true );
            }
        }
        else
        {
            ret = _runProperties( Node, Node.NodeType.getFirstNodeTypeTab() );
        }

        return ret;
    }

    private string _runProperties( CswNbtNode Node, CswNbtMetaDataNodeTypeTab Tab )
    {
        string ret = string.Empty;

        foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypeProps )
        {
            if( Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password )
            {
                ret += _makeItem( PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString(),
                    //"<div style=\"float: left;\">" + Prop.PropName + ": </div>" +
                    //"<div style=\"float: right;\">" + Node.Properties[Prop].Gestalt + "</div>",
                                  Prop.PropName + "<small class=\"\">" + Node.Properties[Prop].Gestalt + "</small>",
                                  _runPropertyEditors( Node, Prop ),
                                  false );
            }
        }

        return ret;
    }
    private string _runPropertyEditors( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
    {
        string Html = Prop.PropName + ": <hr/><br/>";
        string IdStr = PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString();
        CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
        switch( Prop.FieldType.FieldType )
        {
            case CswNbtMetaDataFieldType.NbtFieldType.Date:
                Html += "<input type=\"date\" name=\"" + IdStr + "\" value=\"" + PropWrapper.Gestalt + "\" />";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Link:
                Html += "<a href=\"" + PropWrapper.AsLink.Href + "\">" + PropWrapper.AsLink.Text + "</a>";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.List:
                Html += "<select name=\"" + IdStr + "\">";
                foreach( CswNbtNodeTypePropListOption Option in PropWrapper.AsList.Options.Options )
                {
                    Html += "<option value=\"" + Option.Value + "\"";
                    if( PropWrapper.AsList.Value == Option.Value )
                        Html += " selected";
                    Html += ">" + Option.Text + "</option>";
                }
                Html += "</select>";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                Html += "<select name=\"" + IdStr + "\">";
                foreach( Tristate state in Enum.GetValues( typeof( Tristate ) ) )
                {
                    Html += "<option value=\"" + state.ToString() + "\"";
                    if( PropWrapper.AsLogical.Checked == state )
                        Html += " selected";
                    Html += ">" + state.ToString() + "</option>";
                }
                Html += "</select>";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                Html += "<textarea name=\"" + IdStr + "\">" + PropWrapper.AsMemo.Text + "</textarea>";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Number:
                Html += "<input type=\"number\" name=\"" + IdStr + "\" value=\"" + PropWrapper.Gestalt + "\"";
                Html += "/>";
                if( Prop.MinValue != Int32.MinValue )
                    Html += "min = \"" + Prop.MinValue + "\"";
                if( Prop.MaxValue != Int32.MinValue )
                    Html += "max = \"" + Prop.MaxValue + "\"";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Password:
                Html += string.Empty;
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                Html += "<input type=\"text\" name=\"" + IdStr + "_qty\" value=\"" + PropWrapper.AsQuantity.Quantity.ToString() + "\" />";
                Html += "<select name=\"" + IdStr + "_units\">";
                string SelectedUnit = PropWrapper.AsQuantity.Units;
                foreach( CswNbtNode UnitNode in PropWrapper.AsQuantity.UnitNodes )
                {
                    string ThisUnitText = UnitNode.Properties[CswNbtObjClassUnitOfMeasure.NamePropertyName].AsText.Text;
                    Html += "<option value=\"" + UnitNode.Properties[CswNbtObjClassUnitOfMeasure.NamePropertyName].AsText.Text + "\"";
                    if( ThisUnitText == SelectedUnit )
                        Html += " selected";
                    Html += ">" + ThisUnitText + "</option>";
                }
                Html += "</select>";

                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Question:
                Html += "Answer: <hr/><br/>";
                Html += "<input type=\"text\" name=\"" + IdStr + "_ans\" value=\"" + PropWrapper.AsQuestion.Answer + "\" />";
                Html += "Comments: <hr/><br/>";
                Html += "<textarea name=\"" + IdStr + "_com\">" + PropWrapper.AsQuestion.Comments + "</textarea>";
                Html += "Corrective Action: <hr/><br/>";
                Html += "<textarea name=\"" + IdStr + "_cor\">" + PropWrapper.AsQuestion.CorrectiveAction + "</textarea>";
                break;

            case CswNbtMetaDataFieldType.NbtFieldType.Static:
                Html += Prop.StaticText;
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Text:
                Html += "<input type=\"text\" name=\"" + IdStr + "\" value=\"" + PropWrapper.Gestalt + "\" />";
                break;
            case CswNbtMetaDataFieldType.NbtFieldType.Time:
                Html += "<input type=\"time\" name=\"" + IdStr + "\" value=\"" + PropWrapper.Gestalt + "\" />";
                break;
            default:
                Html += PropWrapper.Gestalt;
                break;
        }
        return _makeItem( "", Html, string.Empty, false );
    }


    private string _makeItem( string ID, string Text, string SubItems, bool Arrow )
    {
        string ret = @"<item id=""" + ID + @""" arrow=""" + Arrow.ToString().ToLower() + @""">
                           <text>" + Text + @"</text>";
        if( SubItems != null )
        {
            ret += @"          <subitems>
                                   " + SubItems + @"
                               </subitems>";
        }
        ret += @"      </item>";
        return ret;
    }

} // wsViewListTree

