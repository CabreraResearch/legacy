
using ChemSW;
using ChemSW.Exceptions;
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
                NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Req.Layout, Req.NodeTypeId, ntp, true, Req.TabId, Prop.DisplayRow, Prop.DisplayColumn, Prop.TabGroup );
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

    }
}