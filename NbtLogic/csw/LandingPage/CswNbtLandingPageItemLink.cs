using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemLink : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemLink( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemData( DataRow LandingPageRow )
        {
            if( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) != Int32.MinValue )
            {
                CswNbtViewId NodeViewId = new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"].ToString() ) );
                CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( NodeViewId );
                Dictionary<CswNbtViewId, CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, true, false, false, NbtViewRenderingMode.Any );
                if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.ContainsKey( ThisView.ViewId ) )
                {
                    _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                    _ItemData.ViewId = NodeViewId.ToString();
                    _ItemData.ViewMode = ThisView.ViewMode.ToString().ToLower();
                    if( ThisView.Root.ChildRelationships[0] != null )
                    {
                        if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                        {
                            CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( ThisView.Root.ChildRelationships[0].SecondId );
                            if( RootNT != null )
                            {
                                _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootNT.IconFileName;
                            }
                        }
                        else if( ThisView.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.ObjectClassId )
                        {
                            CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( ThisView.Root.ChildRelationships[0].SecondId );
                            if( RootOC != null )
                            {
                                _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + RootOC.IconFileName;
                            }
                        }
                    }
                    _ItemData.Type = "view";
                }
            }
            if( CswConvert.ToInt32( LandingPageRow["to_actionid"] ) != Int32.MinValue )
            {
                CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( LandingPageRow["to_actionid"] )];
                if( null != ThisAction )
                {
                    if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                    {
                        _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                    }
                    _ItemData.ActionId = LandingPageRow["to_actionid"].ToString();
                    _ItemData.ActionName = ThisAction.Name.ToString();
                    _ItemData.ActionUrl = ThisAction.Url;
                    _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + "wizard.png";
                    _ItemData.Type = "action";
                }
            }
            if( CswConvert.ToInt32( LandingPageRow["to_reportid"] ) != Int32.MinValue )
            {
                CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( LandingPageRow["to_reportid"] ) )];
                if( null != ThisReportNode )
                {
                    _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisReportNode.NodeName;
                    int idAsInt = CswConvert.ToInt32( LandingPageRow["to_reportid"] );
                    CswPrimaryKey reportPk = new CswPrimaryKey( "nodes", idAsInt );
                    _ItemData.ReportId = reportPk.ToString();
                    _ItemData.Type = "report";
                    _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                }
            }
            _setCommonItemData( LandingPageRow );
        }
    }
}
