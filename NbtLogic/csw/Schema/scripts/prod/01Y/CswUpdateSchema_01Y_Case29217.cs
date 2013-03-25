using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29217
    /// </summary>
    public class CswUpdateSchema_01Y_Case29217 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29217; }
        }

        public override void update()
        {
            // Remove the UN Code property from the add layout for all Material types
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            if( null != MaterialOC )
            {
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp UNCodeNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );
                    if( null != UNCodeNTP )
                    {
                        UNCodeNTP.removeFromLayout( LayoutType: CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }
                }
            }


        } //Update()

    }//class CswUpdateSchema_01Y_Case29217

}//namespace ChemSW.Nbt.Schema