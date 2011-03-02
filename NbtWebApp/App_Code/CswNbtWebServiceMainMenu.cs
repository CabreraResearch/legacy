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
    public class CswNbtWebServiceMainMenu
    {

        /// <summary>
        /// Supported Export formats
        /// </summary>
        public enum ExportOutputFormat
        {
            CSV,
            Excel,
            PDF,
            Word,
            MobileXML,
            ReportXML
        }

        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceMainMenu( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public string getMenu( Int32 ViewId, string NodePkString )
        {
            string ret = string.Empty;
            CswNbtView View = null;
            if( ViewId != Int32.MinValue )
            {
                View = _CswNbtResources.ViewCache.getView( ViewId );
            }
            CswPrimaryKey NodePk = null;
            CswNbtNode Node = null;
            if( NodePkString != string.Empty )
            {
                NodePk = new CswPrimaryKey();
                NodePk.FromString( NodePkString );
                Node = _CswNbtResources.Nodes.GetNode( NodePk );				
            }

            // SEARCH
            string SearchHref = "Search.aspx?viewid=" + ViewId.ToString();
            if( NodePk != null )
                SearchHref += "&nodeid=" + NodePk.PrimaryKey.ToString();
            ret += "<item text=\"Search\" href=\"" + SearchHref + "\" />";

            // ADD
            ret += "<item text=\"Add\">";
            foreach( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry in View.Root.AllowedChildNodeTypes() )
            {
                //ret += "  <item text=\"" + Entry.NodeType.NodeTypeName + "\" popup=\"Popup_EditNode.aspx?dcv=0&dcsn=0&nodetypeid=" + Entry.NodeType.NodeTypeId + "&parentnodekey=" + NodePk.ToString() + "&svid=" + View.SessionViewId.ToString() + "&checkednodeids=&sourceviewid=\" />";
                ret += "  <item text=\"" + Entry.NodeType.NodeTypeName + "\" nodetypeid=\"" + Entry.NodeType.NodeTypeId.ToString() + "\" action=\"AddNode\" />";
            }
            ret += "</item>";

            // COPY
            ret += "<item text=\"Copy\" href=\"Popup_CopyNode.aspx?nodekey=" + NodePk.ToString() + "\"/>";

            // DELETE
            if( NodePk != null && Node != null && Node.NodeSpecies == NodeSpecies.Plain )
            {
                //if( SelectedNodeKeyViewNode is CswNbtViewRelationship &&
                //    ( (CswNbtViewRelationship) SelectedNodeKeyViewNode ).AllowDelete &&
                if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Delete, Node.NodeTypeId, Node, null ) )
                {
                    ret += "<item text=\"Delete\" action=\"DeleteNode\" nodeid=\"" + Node.NodeId.ToString() + "\" nodename=\"" + Node.NodeName + "\" />";
                }
            }

            // SAVE VIEW AS
            if( View != null && View.ViewId <= 0 )
            {
                ret += "<item text=\"SaveViewAs\" popup=\"Popup_NewView.aspx?sessionviewid=" + View.SessionViewId.ToString() + "\"/>";
            }

            // PRINT LABEL
            if( Node != null && Node.NodeType != null )
            {
                CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.NodeType.BarcodeProperty;
                if( BarcodeProperty != null )
                {
                    ret += "<item text=\"Print Label\" popup=\"Popup_PrintLabel.aspx?nodeid=" + NodePk.ToString() + "&propid=" + BarcodeProperty.PropId.ToString() + "&checkednodeids=\" />";
                }
            }

            // PRINT
            if( View != null && View.ViewMode == NbtViewRenderingMode.Grid )
                ret += "<item text=\"Print\" popup=\"PrintGrid.aspx?sessionviewid=" + View.SessionViewId.ToString() + "\" />";

            // EXPORT
            ret += "<item text=\"Export\">";
            if( NbtViewRenderingMode.Grid == View.ViewMode )
            {
                foreach( ExportOutputFormat FormatType in Enum.GetValues( typeof( ExportOutputFormat ) ) )
                {
                    if( ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    {
                        ret += "  <item text=\"" + FormatType.ToString() + "\" popup=\"Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode.ToString() + "\" />";
                    }
                }
            }
            else  // tree or list
            {
                ret += "  <item text=\"Report XML\" />";
                if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    ret += "  <item text=\"Mobile XML\" popup=\"Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode.ToString() + "\" />";
            }
            ret += "</item>";

            // MOBILE
            if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
            {
                ret += "<item text=\"Mobile\">";
                ret += "  <item text=\"Export Mobile XML\" popup=\"Popup_Export.aspx?sessionviewid=" + View.SessionViewId.ToString() + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode.ToString() + "\" />";
                ret += "  <item text=\"Import Mobile XML\" href=\"" + _CswNbtResources.Actions[CswNbtActionName.Load_Mobile_Data].Url + "\"/>";
                ret += "</item>";
            }

            //// SWITCH VIEW
            //ret += "<item text=\"Switch View\" popup=\"Popup_ChangeView.aspx\"/>";

            // EDIT VIEW
            if( ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Edit_View ) )
            {
                string EditViewHref = "EditView.aspx?viewid=" + ViewId.ToString();
                if( View.Visibility == NbtViewVisibility.Property )
                    EditViewHref += "&step=2";
                ret += "<item text=\"Edit View\" href=\"" + EditViewHref + "\" />";
            }

            // MULTI-EDIT
            ret += "<item text=\"Multi-Edit\" action=\"multiedit\" />";

            return "<menu>" + ret.Replace( "&", "&amp;" ) + "</menu>";
        }


    } // class CswNbtWebServiceMainMenu

} // namespace ChemSW.Nbt.WebServices
