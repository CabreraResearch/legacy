using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Welcome;
using ChemSW.NbtWebControls;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the table of components on the Welcome page
    /// </summary>
    public class CswNbtWebServiceWelcomeItems
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtWelcomeTable _CswNbtWelcomeTable;
        public CswNbtWebServiceWelcomeItems( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtWelcomeTable = new CswNbtWelcomeTable( _CswNbtResources );
        }

        private DataTable _getWelcomeTable( Int32 RoleId )
        {
            CswTableSelect WelcomeSelect = _CswNbtResources.makeCswTableSelect( "WelcomeSelect", "welcome" );
            string WhereClause = "where roleid = '" + RoleId.ToString() + "'";
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "welcomeid", OrderByType.Ascending ) );
            return WelcomeSelect.getTable( WhereClause, OrderBy );
        } // _getWelcomeTable()

        public JObject GetWelcomeItems( string strRoleId )
        {
            JObject Ret = new JObject();
            //string ret = string.Empty;
            //var ReturnXML = new XmlDocument();
            //XmlNode WelcomeNode = CswXmlDocument.SetDocumentElement( ReturnXML, "welcome" );

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // Welcome components from database
            DataTable WelcomeTable = _getWelcomeTable( RoleId );

            // see BZ 10234
            if( WelcomeTable.Rows.Count == 0 )
            {
                ResetWelcomeItems( strRoleId );
                WelcomeTable = _getWelcomeTable( RoleId );
            }
            Dictionary<CswNbtViewId, CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, true, false, false, NbtViewRenderingMode.Any );

            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                string WelcomeId = WelcomeRow["welcomeid"].ToString();
                Ret[WelcomeId] = new JObject();

                CswNbtWelcomeTable.WelcomeComponentType ThisType = (CswNbtWelcomeTable.WelcomeComponentType) Enum.Parse( typeof( CswNbtWelcomeTable.WelcomeComponentType ), WelcomeRow["componenttype"].ToString(), true );
                string LinkText = string.Empty;

                switch( ThisType )
                {
                    case CswNbtWelcomeTable.WelcomeComponentType.Add:
                        if( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) != Int32.MinValue )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) );
                            if( NodeType != null )
                            {
                                bool CanAdd = NodeType.getObjectClass().CanAdd && _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, NodeType );
                                if( CanAdd )
                                {
                                    if( WelcomeRow["displaytext"].ToString() != string.Empty )
                                        LinkText = WelcomeRow["displaytext"].ToString();
                                    else
                                        LinkText = "Add New " + NodeType.NodeTypeName;
                                    Ret[WelcomeId]["nodetypeid"] = NodeType.NodeTypeId.ToString();
                                    Ret[WelcomeId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                                    Ret[WelcomeId]["type"] = "add_new_nodetype";
                                }
                            }
                        }
                        break;

                    case CswNbtWelcomeTable.WelcomeComponentType.Link:
                        if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
                            if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.ContainsKey( ThisView.ViewId ) )
                            {
                                LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisView.ViewName;

                                // FogBugz case 9552, Keith Baldwin 7/27/2011
                                // This performs poorly for large views.  
                                // It needs to be cached and refreshed by the user, rather than run every time the welcome page is loaded.
                                //ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( ThisView, false, true, false, false );
                                //if( null != CswNbtTree )
                                //{
                                //    LinkText += " (" + CswNbtTree.getChildNodeCount().ToString() + ")";
                                //}

                                Ret[WelcomeId]["viewid"] = new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString();
                                Ret[WelcomeId]["viewmode"] = ThisView.ViewMode.ToString().ToLower();
                                if( ThisView.Root.ChildRelationships[0] != null )
                                {
                                    if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootNT != null )
                                        {
                                            Ret[WelcomeId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + RootNT.IconFileName;
                                        }
                                    }
                                    else if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.ObjectClassId )
                                    {
                                        CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootOC != null )
                                        {
                                            Ret[WelcomeId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + RootOC.IconFileName;
                                        }
                                    }
                                }
                                Ret[WelcomeId]["type"] = "view";
                            }
                        }
                        if( CswConvert.ToInt32( WelcomeRow["actionid"] ) != Int32.MinValue )
                        {
                            CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( WelcomeRow["actionid"] )];
                            if( null != ThisAction )
                            {
                                if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                                {
                                    LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                                }
                                Ret[WelcomeId]["actionid"] = WelcomeRow["actionid"].ToString();
                                Ret[WelcomeId]["actionname"] = ThisAction.Name.ToString();      // not using CswNbtAction.ActionNameEnumToString here
                                Ret[WelcomeId]["actionurl"] = ThisAction.Url;
                                Ret[WelcomeId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + "wizard.png";
                                Ret[WelcomeId]["type"] = "action";
                            }
                        }
                        if( CswConvert.ToInt32( WelcomeRow["reportid"] ) != Int32.MinValue )
                        {
                            CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( WelcomeRow["reportid"] ) )];
                            if( null != ThisReportNode )
                            {
                                LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisReportNode.NodeName;
                                int idAsInt = CswConvert.ToInt32( WelcomeRow["reportid"] );
                                CswPrimaryKey reportPk = new CswPrimaryKey( "nodes", idAsInt );
                                Ret[WelcomeId]["reportid"] = reportPk.ToString();
                                Ret[WelcomeId]["type"] = "report";
                                Ret[WelcomeId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                            }
                        }
                        break;

                    // case 25734 - no more search links
                    // case CswNbtWelcomeTable.WelcomeComponentType.Search:
                    //if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                    //{
                    //    CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
                    //    if( null != ThisView && ThisView.IsSearchable() )
                    //    {
                    //        LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisView.ViewName;
                    //        Ret[WelcomeId]["viewid"] = new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString();
                    //        Ret[WelcomeId]["viewmode"] = ThisView.ViewMode.ToString().ToLower();
                    //        Ret[WelcomeId]["type"] = "view";
                    //    }
                    //}
                    //break;

                    case CswNbtWelcomeTable.WelcomeComponentType.Text:
                        LinkText = WelcomeRow["displaytext"].ToString();
                        break;

                } // switch( ThisType )

                if( LinkText != string.Empty )
                {
                    Ret[WelcomeId]["linktype"] = WelcomeRow["componenttype"].ToString();
                    //if( WelcomeRow["buttonicon"].ToString() != string.Empty )
                    //{
                    //    Ret[WelcomeId]["buttonicon"] = IconImageRoot + WelcomeRow["buttonicon"].ToString();
                    //}
                    Ret[WelcomeId]["text"] = LinkText;
                    Ret[WelcomeId]["displayrow"] = WelcomeRow["display_row"].ToString();
                    Ret[WelcomeId]["displaycol"] = WelcomeRow["display_col"].ToString();
                }

            } // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

            return Ret;

        } // GetWelcomeItems()


        public void ResetWelcomeItems( string strRoleId )
        {
            _CswNbtWelcomeTable.ResetWelcomeItems( strRoleId );
        } // ResetWelcomeItems()

        /// <summary>
        /// Adds a welcome component to the welcome page
        /// </summary>
        public void AddWelcomeItem( CswNbtWelcomeTable.WelcomeComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            _CswNbtWelcomeTable.AddWelcomeItem( ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, strRoleId );
        } // AddWelcomeItem()


        public bool MoveWelcomeItems( string strRoleId, Int32 WelcomeId, Int32 NewRow, Int32 NewColumn )
        {
            return _CswNbtWelcomeTable.MoveWelcomeItems( strRoleId, WelcomeId, NewRow, NewColumn );
        } // MoveWelcomeItems

        public bool DeleteWelcomeItem( string strRoleId, Int32 WelcomeId )
        {
            return _CswNbtWelcomeTable.DeleteWelcomeItem( strRoleId, WelcomeId );
        } // MoveWelcomeItems

        public JObject getButtonIconList()
        {
            JObject Ret = new JObject();

            //ret.Add( new XElement( "icon", new XAttribute( "filename", "blank.gif" ) ) );

            DirectoryInfo d = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.PhysicalApplicationPath + CswWelcomeTable.IconImageRoot );
            FileInfo[] IconFiles = d.GetFiles();
            foreach( FileInfo IconFile in IconFiles
                .Where( IconFile => ( IconFile.Name.EndsWith( ".gif" ) || IconFile.Name.EndsWith( ".jpg" ) || IconFile.Name.EndsWith( ".png" ) ) ) )
            {
                Ret.Add( new JProperty( IconFile.Name ) );
            }
            return Ret;
        } // _initButtonIconList()


    } // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

