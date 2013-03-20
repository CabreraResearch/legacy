using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28655
    /// </summary>
    public class CswUpdateSchema_01W_Case28655 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28655; }
        }

        public override void update()
        {

            /* Remove the following NtProps from the add layout of all MaterialNTs */
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach (CswNbtMetaDataNodeType MaterialNTs in MaterialOC.getNodeTypes())
            {
                foreach (CswNbtMetaDataNodeTypeProp NodeTypeProp in MaterialNTs.getNodeTypeProps())
                {
                    // Property: UNCode
                    if (NodeTypeProp == MaterialNTs.getNodeTypePropByObjectClassProp(CswNbtObjClassMaterial.PropertyName.UNCode))
                    {
                        NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }

                    // Property: Approved
                    if( NodeTypeProp == MaterialNTs.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving ) )
                    {
                        NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }

                    // Property: IsTierII
                    if( NodeTypeProp == MaterialNTs.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.IsTierII ) )
                    {
                        NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }

                    // Property: Storage and Handling
                    if( NodeTypeProp.PropName == "Storage and Handling" )
                    {
                        NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }

                    // Property: Isotope
                    if( NodeTypeProp.PropName == "Isotope" )
                    {
                        NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28655

}//namespace ChemSW.Nbt.Schema