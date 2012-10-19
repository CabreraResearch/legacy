using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;
using ChemSW;

namespace NbtWebApp.WebSvc.Logic.Menus.LandingPages
{
    /// <summary>
    /// Webservice for the table of items on LandingPages
    /// </summary>
    public class CswNbtWebServiceLandingPageItems
    {

        /// <summary>
        /// Return Object for LandingPageItems
        /// </summary>
        [DataContract]
        public class LandingPageItemsReturn : CswWebSvcReturn
        {
            public LandingPageItemsReturn()
            {
                Data = new LandingPageData();
            }
            [DataMember]
            public LandingPageData Data;
        }

        public static void getLandingPageItems( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( _CswNbtResources );
            LandingPageData Items = new LandingPageData();

            if( Request.RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                Request.RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( Request.RoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            DataTable LandingPageTable = _CswNbtLandingPageTable.getLandingPageTable( RoleId, Request.ActionId );
            Dictionary<CswNbtViewId, CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, true, false, false, NbtViewRenderingMode.Any );

            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                string LandingPageId = LandingPageRow["landingpageid"].ToString();
                LandingPageData.LandingPageItem Item = new LandingPageData.LandingPageItem();
                Item.LandingPageId = LandingPageId;

                CswNbtLandingPageTable.LandingPageItemType ThisType = (CswNbtLandingPageTable.LandingPageItemType) Enum.Parse( typeof( CswNbtLandingPageTable.LandingPageItemType ), LandingPageRow["componenttype"].ToString(), true );
                string LinkText = string.Empty;

                switch( ThisType )
                {
                    case CswNbtLandingPageTable.LandingPageItemType.Add:
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
                                    Item.NodeTypeId = NodeType.NodeTypeId.ToString();
                                    Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                                    Item.Type = "add_new_nodetype";
                                }
                            }
                        }
                        break;

                    case CswNbtLandingPageTable.LandingPageItemType.Link:
                        if( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtViewId NodeViewId = new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"].ToString() ) );
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( NodeViewId );
                            if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.ContainsKey( ThisView.ViewId ) )
                            {
                                LinkText = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                                Item.ViewId = NodeViewId.ToString();
                                Item.ViewMode = ThisView.ViewMode.ToString().ToLower();
                                if( ThisView.Root.ChildRelationships[0] != null )
                                {
                                    if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootNT != null )
                                        {
                                            Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootNT.IconFileName;
                                        }
                                    }
                                    else if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.ObjectClassId )
                                    {
                                        CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( ThisView.Root.ChildRelationships[0].SecondId );
                                        if( RootOC != null )
                                        {
                                            Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootOC.IconFileName;
                                        }
                                    }
                                }
                                Item.Type = "view";
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
                                Item.ActionId = LandingPageRow["to_actionid"].ToString();
                                Item.ActionName = ThisAction.Name.ToString();
                                Item.ActionUrl = ThisAction.Url;
                                Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + "wizard.png";
                                Item.Type = "action";
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
                                Item.ReportId = reportPk.ToString();
                                Item.Type = "report";
                                Item.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                            }
                        }
                        break;

                    case CswNbtLandingPageTable.LandingPageItemType.Text:
                        LinkText = LandingPageRow["displaytext"].ToString();
                        break;

                } // switch( ThisType )

                if( LinkText != string.Empty )
                {
                    Item.LinkType = LandingPageRow["componenttype"].ToString();
                    Item.Text = LinkText;
                    Item.DisplayRow = LandingPageRow["display_row"].ToString();
                    Item.DisplayCol = LandingPageRow["display_col"].ToString();
                }
                Items.LandingPageItems.Add( Item );

            } // foreach( DataRow LandingPageRow in LandingPageTable.Rows )

            Return.Data = Items;
        }

        /// <summary>
        /// Adds a new LandingPage item to the specified LandingPage
        /// </summary>
        public static void addLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.addLandingPageItem(
                Request.Type,
                Request.ViewType,
                Request.ViewValue,
                CswConvert.ToInt32( Request.NodeTypeId ),
                Request.Text,
                Request.RoleId,
                Request.ActionId
                );
        }

        /// <summary>
        /// Moves a LandingPage item to a new cell on the specified LandingPage
        /// </summary>
        public static void moveLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.moveLandingPageItem( Request.LandingPageId, Request.NewRow, Request.NewColumn );
        }

        /// <summary>
        /// Removes a LandingPage item from the specified LandingPage
        /// </summary>
        public static void deleteLandingPageItem( ICswResources CswResources, LandingPageItemsReturn Return, LandingPageData.Request Request )
        {
            CswNbtLandingPageTable _CswNbtLandingPageTable = new CswNbtLandingPageTable( (CswNbtResources) CswResources );
            _CswNbtLandingPageTable.deleteLandingPageItem( Request.LandingPageId );
        }
    } // class CswNbtWebServiceLandingPageItems
} // namespace ChemSW.Nbt.WebServices

