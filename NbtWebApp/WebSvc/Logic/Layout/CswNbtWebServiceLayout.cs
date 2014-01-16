
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace NbtWebApp.WebSvc.Logic.Layout
{
    public class CswNbtWebServiceLayout
    {

        public static void UpdateLayout( ICswResources CswResources, CswNbtLayoutDataReturn Ret, CswNbtLayoutDataCollection Req )
        {
            foreach( CswNbtLayoutData Layout in Req.Props )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtMetaDataNodeTypeProp ntp = NbtResources.MetaData.getNodeTypeProp( Layout.NodeTypePropId );
                NbtResources.MetaData.NodeTypeLayout.updatePropLayout( Layout.Layout, Layout.NodeTypeId, ntp, true, Layout.TabId, Layout.DisplayRow, Layout.DisplayColumn );
            }
        }

    }
}