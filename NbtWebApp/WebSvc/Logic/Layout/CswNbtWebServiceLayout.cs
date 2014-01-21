
using ChemSW;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Layout
{
    public class CswNbtWebServiceLayout
    {

        public static void UpdateLayout( ICswResources CswResources, CswNbtLayoutDataReturn Ret, CswNbtNodeTypeLayout Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            foreach( CswNbtLayoutProp Prop in Req.Props )
            {
                CswNbtMetaDataNodeTypeProp ntp = NbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Req.Layout, Req.NodeTypeId, ntp, Prop.DoMove, Req.TabId, Prop.DisplayRow, Prop.DisplayColumn, Prop.TabGroup );
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

        public static void UpdateTabOrder( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtTabMoveRequest Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassDesignNodeTypeTab Tab = NbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey("nodetype_tabset", Req.TabId) );
            Tab.UpdateTabPosition( Req.NewPosition );
        }

    }
}