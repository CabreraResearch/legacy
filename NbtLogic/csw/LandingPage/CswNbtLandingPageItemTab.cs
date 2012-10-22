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
            if( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) != Int32.MinValue && CswConvert.ToInt32( LandingPageRow["to_tabid"] ) != Int32.MinValue )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( LandingPageRow["to_nodetypeid"] ) );
                if( NodeType != null )
                {
                    CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( Request.NodeId ) );
                    if( RequestNode != null && RequestNode.NodeTypeId == NodeType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeTypeTab Tab = NodeType.getNodeTypeTab( CswConvert.ToInt32( LandingPageRow["to_tabid"] ) );
                        if( null != Tab )
                        {
                            CswNbtView TabView = getTabView(Request.NodeViewId, NodeType);
                            if( null != TabView && TabView.IsFullyEnabled() )
                            {
                                _ItemData.Text = LandingPageRow["displaytext"].ToString() != string.Empty ? 
                                    LandingPageRow["displaytext"].ToString() : 
                                    TabView.ViewName + " " + Tab.TabName;
                                _ItemData.ViewId = TabView.ViewId.ToString();
                                _ItemData.ViewMode = TabView.ViewMode.ToString().ToLower();                                
                                _ItemData.Type = "view";

                                _ItemData.TabId = LandingPageRow["to_tabid"].ToString();
                                _ItemData.ButtonIcon = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                                _setCommonItemDataForUI( LandingPageRow );
                            }                            
                        }
                    }                   
                }                
            }            
        }

        private CswNbtView getTabView( string NodeViewId, CswNbtMetaDataNodeType NodeType )
        {
            CswNbtView TabView;
            if( false == String.IsNullOrEmpty( NodeViewId ) )
            {
                CswNbtSessionDataId TabViewId = new CswNbtSessionDataId( NodeViewId );
                TabView = _CswNbtResources.ViewSelect.getSessionView( TabViewId );
            }
            else
            {
                TabView = NodeType.CreateDefaultView();
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
