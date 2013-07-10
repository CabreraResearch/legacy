using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29846
    /// </summary>
    public class CswUpdateSchema_02D_Case29846: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29846; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab firstTab = ContainerNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp ManuLotNoNTP = ContainerNT.getNodeTypeProp( "Manufacturer Lot Number" );
                if( null == ManuLotNoNTP )
                {
                    ManuLotNoNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ContainerNT, CswEnumNbtFieldType.Text, "Manufacturer Lot Number", firstTab.TabId );
                }

                ManuLotNoNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
            }


        } // update()

    }//class CswUpdateSchema_02C_Case29846

}//namespace ChemSW.Nbt.Schema