using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28236
    /// </summary>
    public class CswUpdateSchema_01U_Case28236 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28236; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType locationNT in locationOC.getNodeTypes() )
            {
                //also put "Inventory Levels" 2nd and the "<location NT name>" tab 1st
                CswNbtMetaDataNodeTypeTab locationTab = locationNT.getNodeTypeTab( locationNT.NodeTypeName );
                CswNbtMetaDataNodeTypeTab invLevelsTab = locationNT.getNodeTypeTab( "Inventory Levels" );

                if( null != locationTab && null != invLevelsTab )
                {
                    locationTab.TabOrder = 1;
                    invLevelsTab.TabOrder = 2;
                }

                CswNbtMetaDataNodeTypeTab containersNTT = locationNT.getNodeTypeTab( "Containers" );
                if( null == containersNTT )
                {
                    containersNTT = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( locationNT, "Containers", locationNT.getMaximumTabOrder() + 1 );
                }
                CswNbtMetaDataNodeTypeProp containersNTP = locationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Containers );
                containersNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, TabId: containersNTT.TabId );
            }
        }

        //Update()

    }//class CswUpdateSchemaCase28236

}//namespace ChemSW.Nbt.Schema