using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageItemTab : CswNbtLandingPageItem
    {
        public CswNbtLandingPageItemTab( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }

        public override void setItemDataForUI( DataRow LandingPageRow, LandingPageData.Request Request )
        {
            Int32 NodeTypeId = CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] );
            Int32 TabId = CswConvert.ToInt32( LandingPageRow["to_tabid"] );
            if( NodeTypeId != Int32.MinValue && TabId != Int32.MinValue )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( NodeType != null )
                {
                    CswNbtMetaDataNodeTypeTab Tab = NodeType.getNodeTypeTab( TabId );
                    if( null != Tab )
                    {
                        CswNbtView TabView = getTabView( Request.NodeId, Request.NodeViewId, NodeType );
                        if( null != TabView && TabView.IsFullyEnabled() )
                        {
                            String DisplayText = LandingPageRow["displaytext"].ToString();
                            _ItemData.Text = false == String.IsNullOrEmpty( DisplayText ) ? DisplayText : TabView.ViewName + " " + Tab.TabName;
                            _ItemData.ViewId = TabView.SessionViewId.ToString();
                            _ItemData.ViewMode = TabView.ViewMode.ToString().ToLower();                                
                            _ItemData.Type = "view";
                            _ItemData.TabId = TabId.ToString();
                            _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                            _setCommonItemDataForUI( LandingPageRow );
                        }                            
                    }
                }                
            }            
        }

        private CswNbtView getTabView( string NodeId, string NodeViewId, CswNbtMetaDataNodeType NodeType )
        {
            CswNbtView TabView = null;
            if( false == String.IsNullOrEmpty( NodeViewId ) )
            {
                CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( NodeId ) );
                if( RequestNode != null && RequestNode.NodeTypeId == NodeType.NodeTypeId )
                {
                    CswNbtSessionDataId TabViewId = new CswNbtSessionDataId(NodeViewId);
                    TabView = _CswNbtResources.ViewSelect.getSessionView(TabViewId);
                }
            }
            return TabView;
        }

        public override void setItemDataForDB( LandingPageData.Request Request )
        {
            Int32 TabId = CswConvert.ToInt32( Request.PkValue );
            Int32 NodeTypeId = CswConvert.ToInt32( Request.NodeTypeId );
            if( TabId != Int32.MinValue && NodeTypeId != Int32.MinValue )
            {
                _ItemRow["to_tabid"] = CswConvert.ToDbVal( TabId );
                _ItemRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
            }
            else
            {
                throw new CswDniException(ErrorType.Warning, "You must select a tab", "No tab selected for new Tab LandingPage Item");
            }
            _setCommonItemDataForDB( Request );
        }
    }
}
