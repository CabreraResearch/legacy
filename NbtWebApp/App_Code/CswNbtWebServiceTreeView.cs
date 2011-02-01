using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;
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
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTreeView
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceTreeView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private class DashIcon
        {
            public string Text;
            public string Id;
            public string Href;
            public CswNbtResources.CswNbtModule Module;

            public DashIcon( string inText, string inId, string inHref, CswNbtResources.CswNbtModule inModule )
            {
                Text = inText;
                Id = inId;
                Href = inHref;
                Module = inModule;
            }
        }

        public string getDashboard()
        {
            string ret = string.Empty;
            Collection<DashIcon> DashIcons = new Collection<DashIcon>();
            DashIcons.Add( new DashIcon( "IMCS - Instrument Maintenance and Calibration",
                                         "dash_imcs",
                                         "http://www.chemswlive.com/19013.htm",
                                         CswNbtResources.CswNbtModule.IMCS ) );
            DashIcons.Add( new DashIcon( "FE - Fire Extinguisher Inspection",
                                         "dash_fe",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.FE ) );
            DashIcons.Add( new DashIcon( "SI - Site Inspection",
                                         "dash_si",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.SI ) );
            DashIcons.Add( new DashIcon( "STIS - Sample Tracking and Inventory System",
                                         "dash_stis",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.STIS ) );
            DashIcons.Add( new DashIcon( "CISPro - Chemical Inventory System",
                                         "dash_cispro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CISPro ) );
            DashIcons.Add( new DashIcon( "CCPro - Control Charts",
                                         "dash_ccpro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CCPro ) );
            DashIcons.Add( new DashIcon( "BioSafety",
                                         "dash_biosafety",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.BioSafety ) );
            DashIcons.Add( new DashIcon( "Mobile",
                                         "dash_hh",
                                         "http://www.chemswlive.com/cis-pro-mobile.htm",
                                         CswNbtResources.CswNbtModule.Mobile ) );
            DashIcons.Add( new DashIcon( "NBTManager",
                                         "dash_nbtmgr",
                                         "",
                                         CswNbtResources.CswNbtModule.NBTManager ) );

            foreach( DashIcon DashIcon in DashIcons )
            {
                if( _CswNbtResources.IsModuleEnabled( DashIcon.Module ) )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
                else if( DashIcon.Module != CswNbtResources.CswNbtModule.NBTManager )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "_off\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
            }

            return "<dashboard>" + ret + "</dashboard>";
        } // getDashboard()


        public string getHeaderMenu()
        {
            string ret = string.Empty;

            ret += "<item text=\"Home\" href=\"NewMain.shtm\" />";
            ret += "<item text=\"Admin\">";
            ret += "  <item text=\"Current User List\" href=\"\" />";
            ret += "  <item text=\"View Log\" href=\"\" />";
            ret += "  <item text=\"Edit Config Vars\" href=\"\" />";
            ret += "  <item text=\"Statistics\" href=\"\" />";
            ret += "</item>";
            ret += "<item text=\"Preferences\">";
            ret += "  <item text=\"Profile\" href=\"\" />";
            ret += "  <item text=\"Subscriptions\" href=\"\" />";
            ret += "</item>";
            ret += "<item text=\"Help\">";
            ret += "  <item text=\"Help\" popup=\"help/index.htm\" />";
            ret += "  <item text=\"About\" popup=\"About.html\" />";
            ret += "</item>";
            ret += "<item text=\"Logout\" />";

            return "<menu>" + ret + "</menu>";
        }


        public string getViews()
        {
            string ret = string.Empty;
            DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false );
            foreach( DataRow ViewRow in ViewDT.Rows )
            {
                ret += "<view id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
                ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
                ret += "/>";
            }
            return "<views>" + ret + "</views>";
        }

        public string getTree( Int32 ViewId )
        {
            string ret = string.Empty;
            ret += @"<item id=""-1""><content><name>No results</name></content></item>";

            CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );
            
            string TreeXml = "<root>" +
                             "  <item id=\"root\" rel=\"root\">" +
                             "    <content>" +
                             "      <name>" + View.ViewName + "</name>" +
                             "    </content>" +
                                  _runTreeNodesRecursive( Tree ) +
                             "  </item>" +
                             "</root>";

            if( Tree.getChildNodeCount() > 0 )
                ret = "<result>" +
                      "  <tree>" + TreeXml + "</tree>" +
                      "  <types>{ " + _getTypes( View ) + " }</types>" +
                      "</result>";

            return ret;
        } // getTree()

        public string _getTypes( CswNbtView View )
        {
            string ret = string.Empty;
            Collection<CswNbtMetaDataNodeType> NodeTypes = new Collection<CswNbtMetaDataNodeType>();
            ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
            foreach( CswNbtViewRelationship Rel in Relationships )
            {
                if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
                    NodeTypes.Add( NodeType );
                }
                else
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
                    foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
                    {
                        NodeTypes.Add( NodeType );
                    }
                }
            }

            ret += "\"root\": { " +
                    "    \"icon\": { " +
                    "      \"image\": \"Images/view/viewtree.gif\" " +
                    "    } " +
                    "  }";
            ret += ",\"default\": \"\" ";

            foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
            {
                ret += ",\"nt_" + NodeType.FirstVersionNodeTypeId.ToString() + "\": { " +
                        "    \"icon\": { " +
                        "      \"image\": \"Images/icons/" + NodeType.IconFileName + "\" " +
                        "    } " +
                        "  }";
            }
            return ret;
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

                ret += "<item id=\"" + ThisNodeId + "\" rel=\"nt_" + ThisNode.NodeType.FirstVersionNodeTypeId.ToString() + "\">";
                ret += "  <content>";
                ret += "    <name>" + ThisNodeName + "</name>";
                ret += "  </content>";
                ret += _runTreeNodesRecursive( Tree );
                ret += "</item>";

                Tree.goToParentNode();
            }
            return ret;
        }



    } // class CswNbtWebServiceTreeView

} // namespace ChemSW.Nbt.WebServices
