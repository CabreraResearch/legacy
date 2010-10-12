using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.Core;

namespace ChemSW.NbtWebControls
{
    public class CswNodesTree : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtView View;

        public CswNodesTree( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        private HtmlGenericControl _Div;
        protected override void CreateChildControls()
        {
            try
            {
                _Div = new HtmlGenericControl( "div" );
                _Div.ID = "NodesTreeDiv";
                this.Controls.Add( _Div );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.CreateChildControls();
        }

        public string ClientSelectNodeFunctionName = string.Empty;
        public CswNbtNodeKey SelectedNodeKey = null;

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                if( View.SessionViewId == Int32.MinValue )
                    View.SaveToCache();

                String JS = @"<script language=""Javascript""> 
                          $(document).ready(function() { 
                            $('#" + CswTools.SafeJavascriptParam( _Div.ClientID ) + @"').jstree({
                                'plugins': ['themes', 'xml_data', 'ui', 'types'],
                                'themes': {
                                    'theme': 'chemsw'
                                },
                                'core': {
                                    'animation': 100
                                },
                                'xml_data': {
                                    'ajax' : {
                                        'type': 'POST',
                                        'url': 'wsNodesTree.asmx/GetTreeXml',
                                        'data': function(parentnode) {
                                                    var ret = 'SessionViewId=" + CswTools.SafeJavascriptParam( View.SessionViewId.ToString() ) + @"';
                                                    if(parentnode != undefined && parentnode != -1)
                                                        ret = ret + '&parentnodeid=' + parentnode.attr(""id"");
                                                    else
                                                        ret = ret + '&parentnodeid=undefined';
                                                    return ret;
                                                }
                                    },
                                    'xsl': 'nest'
                                },
                                'ui': {
                                ";
                if( SelectedNodeKey != null )
                {
                    JS += @"        'initially_select': { '" + SelectedNodeKey.ToJavaScriptParam() + "' }";
                }
                JS += @"            'select_limit': 1,
                                    'selected_parent_close': 'select_parent'
                                },
                                'types': {
                                    'types': {
                                        'max_depth': -2,
                                        'max_children': -2,
                       ";
                Collection<CswNbtMetaDataNodeType> AllNodeTypes = View.getAllNodeTypes();
                foreach( CswNbtMetaDataNodeType NodeType in AllNodeTypes )
                {
                    JS += @"                'nodetypeid_" + CswTools.SafeJavascriptParam( NodeType.NodeTypeId.ToString() ) + @"': {
                                            'icon': { 'image': 'http://localhost/NbtWebApp/Images/icons/" + CswTools.SafeJavascriptParam( NodeType.IconFileName ) + @"' },
                                            'select_node': true
                                        },
                           ";
                }
                // root
                JS += @"                    'nodetypeid_0': {
                                            'icon': { 'image': 'http://localhost/NbtWebApp/Images/view/viewtree.gif' },
                                            'select_node': false
                                        },
                                        'default': {
                                            'icon': { 'image': 'http://localhost/NbtWebApp/Images/view/category.gif' },
                                            'select_node': false
                                        },
                                    }
                                }
                            })";
                if( ClientSelectNodeFunctionName != string.Empty )
                {
                    JS += @".bind('select_node.jstree', " + ClientSelectNodeFunctionName + ")";
                }
                JS += @";
                          });

                            </script> ";

                System.Web.UI.ScriptManager.RegisterStartupScript( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }// OnPreRender()

        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }

    }// CswNodesTree
}// namespace