using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemLink : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemLink( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            if( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"] ) != Int32.MinValue )
            {
                CswNbtViewId NodeViewId = new CswNbtViewId( CswConvert.ToInt32( LandingPageRow["to_nodeviewid"].ToString() ) );
                CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( NodeViewId );
                if( null != ThisView && ThisView.IsFullyEnabled() && ThisView.IsVisible() )
                {
                    _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? LandingPageRow["displaytext"].ToString() : ThisView.ViewName;

                    _ItemData.ViewId = NodeViewId.ToString();
                    _ItemData.ViewMode = ThisView.ViewMode.ToString().ToLower();
                    if( ThisView.Root.ChildRelationships[0] != null )
                    {
                        _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + ThisView.Root.ChildRelationships[0].SecondIconFileName;
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
                CswPrimaryKey ReportPk = new CswPrimaryKey();
                ReportPk.FromString( Request.PkValue );
                Int32 PkVal = ReportPk.PrimaryKey;
                _ItemRow["to_reportid"] = CswConvert.ToDbVal( PkVal );
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link LandingPage Item" );
            }
            _setCommonItemDataForDB( Request );
        }
    }
}
