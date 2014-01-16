
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

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
                NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Req.Layout, Req.NodeTypeId, ntp, true, Req.TabId, Prop.DisplayRow, Prop.DisplayColumn );
            }
        }

        public static void RemovePropsFromLayout( ICswResources CswResources, CswNbtLayoutDataReturn Ret, CswNbtNodeTypeLayout Req )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            foreach( CswNbtLayoutProp Prop in Req.Props )
            {
                CswNbtMetaDataNodeTypeProp ntp = NbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                NbtResources.MetaData.NodeTypeLayout.removePropFromLayout( Req.Layout, ntp, Req.TabId );
            }
        }

    }
}