using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemLink : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemLink( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            String DisplayText = LandingPageRow["displaytext"].ToString();
            Int32 ViewId = CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] );
            if( ViewId != Int32.MinValue )
            {
                CswNbtViewId NodeViewId = new CswNbtViewId( ViewId );
                CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( NodeViewId );
                if( null != ThisView && ThisView.IsFullyEnabled() && ThisView.IsVisible() )
                {
                    _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                    _ItemData.ViewId = NodeViewId.ToString();
                    _ItemData.ViewMode = ThisView.ViewMode.ToString().ToLower();
                    if( ThisView.Root.ChildRelationships.Count > 0 && ThisView.Root.ChildRelationships[0] != null )
                    {
                        _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisView.Root.ChildRelationships[0].SecondIconFileName;
                    }
                    _ItemData.Type = "view";
                }
            }
            Int32 ActionId = CswConvert.ToInt32( LandingPageRow["to_actionid"] );
            if( ActionId != Int32.MinValue )
            {
                CswNbtAction ThisAction = _CswNbtResources.Actions[ActionId];
                if( null != ThisAction )
                {
                    if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                    {
                        _ItemData.Text = false == String.IsNullOrEmpty( DisplayText ) ? DisplayText : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                    }
                    _ItemData.ActionId = ActionId.ToString();
                    _ItemData.ActionName = ThisAction.Name.ToString();
                    _ItemData.ActionUrl = ThisAction.Url;
                    _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisAction.IconFileName;
                    _ItemData.Type = "action";
                }
            }
            Int32 ReportId = CswConvert.ToInt32( LandingPageRow["to_reportid"] );
            if( ReportId != Int32.MinValue )
            {
                CswPrimaryKey ReportPk = new CswPrimaryKey( "nodes", ReportId );
                CswNbtNode ThisReportNode = _CswNbtResources.Nodes[ReportPk];
                if( null != ThisReportNode )
                {
                    _ItemData.Text = false == String.IsNullOrEmpty( DisplayText ) ? DisplayText : ThisReportNode.NodeName;
                    _ItemData.ReportId = ReportPk.ToString();
                    _ItemData.Type = "report";
                    _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisReportNode.getNodeType().IconFileName;
                }
            } 
            Int32 SearchId = CswConvert.ToInt32( LandingPageRow["to_searchid"] );
            if( SearchId != Int32.MinValue )
            {
                CswPrimaryKey SearchPk = new CswPrimaryKey( "search", SearchId );
                CswNbtSearch Search = _CswNbtResources.SearchManager.restoreSearch( SearchPk );
                if( null != Search )
                {
                    _ItemData.Text = Search.Name;
                    _ItemData.ReportId = SearchPk.ToString();
                    _ItemData.Type = "search";
                    _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + "magglass.png";
                }
            }
            _setCommonItemDataForUI( LandingPageRow );
        }

        public override void setItemDataForDB( LandingPageData.Request Request )
        {
            CswNbtView.ViewType ViewType = (CswNbtView.ViewType) Request.ViewType;
            if( ViewType == CswNbtView.ViewType.View )
            {
                _ItemRow["to_nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( Request.PkValue ).get() );
            }
            else if( ViewType == CswNbtView.ViewType.Action )
            {
                _ItemRow["to_actionid"] = CswConvert.ToDbVal( Request.PkValue );
            }
            else if( ViewType == CswNbtView.ViewType.Report )
            {
                CswPrimaryKey ReportPk = CswConvert.ToPrimaryKey( Request.PkValue );
                _ItemRow["to_reportid"] = CswConvert.ToDbVal( ReportPk.PrimaryKey );
            }
            else if( ViewType == CswNbtView.ViewType.Search )
            {
                CswPrimaryKey SearchPk = CswConvert.ToPrimaryKey( Request.PkValue );
                _ItemRow["to_searchid"] = CswConvert.ToDbVal( SearchPk.PrimaryKey );
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link LandingPage Item" );
            }
            _setCommonItemDataForDB( Request );
        }
    }
}
