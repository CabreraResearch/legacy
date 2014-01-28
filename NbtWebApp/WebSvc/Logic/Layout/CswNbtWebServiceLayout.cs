
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Layout
{
    public class CswNbtWebServiceLayout
    {

        public static void UpdateLayout( ICswResources CswResources, CswNbtLayoutDataReturn Ret, CswNbtNodeTypeLayout Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Permit.can( CswEnumNbtActionName.Design ) )
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Req.NodeTypeId );
                if( null != NodeType )
                {
                    foreach( CswNbtLayoutProp Prop in Req.Props )
                    {
                        CswNbtMetaDataNodeTypeProp ntp = NbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                        if( Prop.RemoveExisting )
                        {
                            ntp.removeFromAllLayouts();
                        }
                        NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Req.Layout, Req.NodeTypeId, ntp, false, Req.TabId, Prop.DisplayRow, Prop.DisplayColumn, Prop.TabGroup );
                    }
                    NbtResources.MetaData.refreshAll();
                    NodeType.DesignNode.RecalculateQuestionNumbers();
                }
            }
        }

        public static void RemovePropsFromLayout( ICswResources CswResources, CswNbtLayoutDataReturn Ret, CswNbtNodeTypeLayout Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            foreach( CswNbtLayoutProp Prop in Req.Props )
            {
                CswNbtMetaDataNodeTypeProp ntp = NbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                if( CswEnumNbtLayoutType.Add == Req.Layout && ntp.IsRequired )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot remove required properties from Add layouts", "" );
                }
                NbtResources.MetaData.NodeTypeLayout.removePropFromLayout( Req.Layout, ntp, Req.TabId );
            }
        }

        public static void GetSearchImageLink( ICswResources CswResources, CswNbtLayoutDataReturn Ret, string Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey NodeId = CswConvert.ToPrimaryKey( Req );
            CswNbtNode Node = NbtResources.Nodes.GetNode( NodeId );
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Node.NodeTypeId );

            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                if( CswEnumNbtFieldType.Image == Prop.getFieldTypeValue() )
                {
                    Ret.Data.ImageLink = CswNbtNodePropImage.getLink( Prop.JctNodePropId, NodeId );
                }
                if( CswEnumNbtFieldType.MOL == Prop.getFieldTypeValue() )
                {
                    Ret.Data.ImageLink = CswNbtNodePropMol.getLink( Prop.JctNodePropId, NodeId );
                }
            }

            if( string.Empty == Ret.Data.ImageLink )
            {
                if( NodeType.IconFileName != string.Empty )
                {
                    Ret.Data.ImageLink = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                }
                else
                {
                    Ret.Data.ImageLink = "Images/icons/300/_placeholder.gif";
                }
            }
        }

        public static void UpdateTabOrder( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtTabMoveRequest Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassDesignNodeTypeTab Tab = NbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetype_tabset", Req.TabId ) );
            Tab.UpdateTabPosition( Req.NewPosition );

            NbtResources.MetaData.refreshAll();
            Tab.RelationalNodeTypeTab.getNodeType().DesignNode.RecalculateQuestionNumbers();
        }

        public static void CreateNewTab( ICswResources CswResources, CswNbtTabAddReturn Ret, CswNbtTabAddRequest Req )
        {

            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataNodeTypeTab Tab =  NbtResources.MetaData.makeNewTabNew( NbtResources.MetaData.getNodeType( Req.NodetypeId ), Req.Name, Req.Order );
            Ret.Data.TabId = Tab.TabId;
        }

        public static void DeleteTab( ICswResources CswResources, CswWebSvcReturn Ret, int TabId )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassDesignNodeTypeTab Tab = NbtResources.MetaData.getNodeTypeTab( TabId ).DesignNode;

            CswNbtView NodetypeTabs = new CswNbtView(NbtResources);
            CswNbtViewRelationship TabRelationship = NodetypeTabs.AddViewRelationship( Tab.NodeType, true );
            //only check tabs for the nodetype that we're working with
            NodetypeTabs.AddViewPropertyAndFilter(
                TabRelationship,
                Tab.NodeType.getNodeTypeProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue ),
                Value: Tab.NodeTypeValue.RelatedNodeId.PrimaryKey.ToString(),
                FilterMode : CswEnumNbtFilterMode.Equals,
                Conjunction : CswEnumNbtFilterConjunction.And,
                SubFieldName: CswEnumNbtSubFieldName.NodeID
                );
            //for now, we don't have a better way to fetch identity tab than by name, but there is another pending case on this
            NodetypeTabs.AddViewPropertyAndFilter(
                TabRelationship,
                Tab.NodeType.getNodeTypeProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName ),
                Value : "Identity",
                Conjunction : CswEnumNbtFilterConjunction.And,
                FilterMode : CswEnumNbtFilterMode.NotEquals
                );
            ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( NodetypeTabs, false, false, true );
            
            //see how many tabs we returned, so we can ensure we're not deleting the last tab in the layout
            if( Tree.getChildNodeCount() > 1 )
            {
                Tab.Node.delete();
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot delete the last tab of a nodetype.", "" );
            }
        }
    }
}