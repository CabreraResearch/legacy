
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;
using ChemSW.Nbt.PropTypes;

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
                        NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Req.Layout, Req.NodeTypeId, ntp, Prop.DoMove, Req.TabId, Prop.DisplayRow, Prop.DisplayColumn, Prop.TabGroup );
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
            CswNbtObjClassDesignNodeTypeTab Tab = NbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey("nodetype_tabset", Req.TabId) );
            Tab.UpdateTabPosition( Req.NewPosition );
        }

    }
}