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
using ChemSW.Nbt.LandingPage;
using ChemSW.NbtWebControls;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the table of components on the LandingPage page
    /// </summary>
    public class CswNbtWebServiceLandingPageItems
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtLandingPageTable _CswNbtLandingPageTable;
        public CswNbtWebServiceLandingPageItems( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtLandingPageTable = new CswNbtLandingPageTable( _CswNbtResources );
        }

        private DataTable _getLandingPageTable( Int32 RoleId )
        {
            CswTableSelect LandingPageSelect = _CswNbtResources.makeCswTableSelect( "LandingPageSelect", "landingpage" );
            string WhereClause = "where for_roleid = '" + RoleId.ToString() + "'";
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "landingpageid", OrderByType.Ascending ) );
            DataTable LandingPageTable = LandingPageSelect.getTable( WhereClause, OrderBy );
            return LandingPageTable;
        }

        public JObject GetLandingPageItems( string strRoleId )
        {
            JObject Ret = new JObject();

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // LandingPage components from database
            DataTable LandingPageTable = _getLandingPageTable( RoleId );

            if( LandingPageTable.Rows.Count == 0 )
            {
                ResetLandingPageItems( strRoleId );
                LandingPageTable = _getLandingPageTable( RoleId );
            }
            Dictionary<CswNbtViewId, CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, true, false, false, NbtViewRenderingMode.Any );

            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                string LandingPageId = LandingPageRow["landingpageid"].ToString();
                Ret[LandingPageId] = new JObject();

                CswNbtLandingPageTable.LandingPageComponentType ThisType = (CswNbtLandingPageTable.LandingPageComponentType) Enum.Parse( typeof( CswNbtLandingPageTable.LandingPageComponentType ), LandingPageRow["componenttype"].ToString(), true );
                string LinkText = string.Empty;

                switch( ThisType )
                {
                    case CswNbtLandingPageTable.LandingPageComponentType.Add:
                        if( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) != Int32.MinValue )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) );
                            if( NodeType != null )
                            {
                                bool CanAdd = NodeType.getObjectClass().CanAdd && _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType );
                                if( CanAdd )
                                {
                                    if( LandingPageRow["displaytext"].ToString() != string.Empty )
                                        LinkText = LandingPageRow["displaytext"].ToString();
                                    else
                                        LinkText = "Add New " + NodeType.NodeTypeName;
                                    Ret[LandingPageId]["nodetypeid"] = NodeType.NodeTypeId.ToString();
                                    Ret[LandingPageId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                                    Ret[LandingPageId]["type"] = "add_new_nodetype";
                                }
                            }
                        }
                        break;

                    case CswNbtLandingPageTable.LandingPageComponentType.Link:
                        if( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) ) );
                            if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.ContainsKey( ThisView.ViewId ) )
                            {
                                LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                                Ret[LandingPageId]["viewid"] = new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) ).ToString();
                                Ret[LandingPageId]["viewmode"] = ThisView.ViewMode.ToString().ToLower();
                                if( ThisView.Root.ChildRelationships[0] != null )
                                {
                                    if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootNT != null )
                                        {
                                            Ret[LandingPageId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + RootNT.IconFileName;
                                        }
                                    }
                                    else if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.ObjectClassId )
                                    {
                                        CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootOC != null )
                                        {
                                            Ret[LandingPageId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + RootOC.IconFileName;
                                        }
                                    }
                                }
                                Ret[LandingPageId]["type"] = "view";
                            }
                        }
                        if( CswConvert.ToInt32( LandingPageRow["to_actionid"] ) != Int32.MinValue )
                        {
                            CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( LandingPageRow["to_actionid"] )];
                            if( null != ThisAction )
                            {
                                if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                                {
                                    LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                                }
                                Ret[LandingPageId]["actionid"] = LandingPageRow["to_actionid"].ToString();
                                Ret[LandingPageId]["actionname"] = ThisAction.Name.ToString();      // not using CswNbtAction.ActionNameEnumToString here
                                Ret[LandingPageId]["actionurl"] = ThisAction.Url;
                                Ret[LandingPageId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + "wizard.png";
                                Ret[LandingPageId]["type"] = "action";
                            }
                        }
                        if( CswConvert.ToInt32( LandingPageRow["to_reportid"] ) != Int32.MinValue )
                        {
                            CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( LandingPageRow["to_reportid"] ) )];
                            if( null != ThisReportNode )
                            {
                                LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisReportNode.NodeName;
                                int idAsInt = CswConvert.ToInt32( LandingPageRow["to_reportid"] );
                                CswPrimaryKey reportPk = new CswPrimaryKey( "nodes", idAsInt );
                                Ret[LandingPageId]["reportid"] = reportPk.ToString();
                                Ret[LandingPageId]["type"] = "report";
                                Ret[LandingPageId]["buttonicon"] = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                            }
                        }
                        break;

                    case CswNbtLandingPageTable.LandingPageComponentType.Text:
                        LinkText = LandingPageRow["displaytext"].ToString();
                        break;

                } // switch( ThisType )

                if( LinkText != string.Empty )
                {
                    Ret[LandingPageId]["linktype"] = LandingPageRow["componenttype"].ToString();
                    Ret[LandingPageId]["text"] = LinkText;
                    Ret[LandingPageId]["displayrow"] = LandingPageRow["display_row"].ToString();
                    Ret[LandingPageId]["displaycol"] = LandingPageRow["display_col"].ToString();
                }

            } // foreach( DataRow LandingPageRow in LandingPageTable.Rows )

            return Ret;

        } // GetLandingPageItems()


        public void ResetLandingPageItems( string strRoleId )
        {
            _CswNbtLandingPageTable.ResetLandingPageItems( strRoleId );
        }

        /// <summary>
        /// Adds a LandingPage component to the LandingPage page
        /// </summary>
        public void AddLandingPageItem( CswNbtLandingPageTable.LandingPageComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            _CswNbtLandingPageTable.AddLandingPageItem( ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, strRoleId );
        }


        public bool MoveLandingPageItems( string strRoleId, Int32 LandingPageId, Int32 NewRow, Int32 NewColumn )
        {
            return _CswNbtLandingPageTable.MoveLandingPageItems( strRoleId, LandingPageId, NewRow, NewColumn );
        }

        public bool DeleteLandingPageItem( string strRoleId, Int32 LandingPageId )
        {
            return _CswNbtLandingPageTable.DeleteLandingPageItem( strRoleId, LandingPageId );
        }

        public JObject getButtonIconList()
        {
            JObject Ret = new JObject();

            DirectoryInfo d = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.PhysicalApplicationPath + CswNbtLandingPageTable.IconImageRoot );
            FileInfo[] IconFiles = d.GetFiles();
            foreach( FileInfo IconFile in IconFiles
                .Where( IconFile => ( IconFile.Name.EndsWith( ".gif" ) || IconFile.Name.EndsWith( ".jpg" ) || IconFile.Name.EndsWith( ".png" ) ) ) )
            {
                Ret.Add( new JProperty( IconFile.Name ) );
            }
            return Ret;
        }
    } // class CswNbtWebServiceLandingPageItems
} // namespace ChemSW.Nbt.WebServices

