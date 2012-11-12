using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27055
    /// </summary>
    public class CswUpdateSchemaCase27055 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region PART ONE
            //Remove Part Number (chemical, biological and supply NTPs) from Add layout, so it doesn't show up twice while creating a material
            CswNbtMetaDataNodeType chemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != chemicalNT )
            {
                CswNbtMetaDataNodeTypeProp chemPartNoNTP = chemicalNT.getNodeTypeProp( CswNbtObjClassMaterial.PartNumberPropertyName );
                if( null != chemPartNoNTP )
                {
                    chemPartNoNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }
            }

            CswNbtMetaDataNodeType bioNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            if( null != bioNT )
            {
                CswNbtMetaDataNodeTypeProp bioPartNoNTP = bioNT.getNodeTypeProp( CswNbtObjClassMaterial.PartNumberPropertyName );
                if( null != bioPartNoNTP )
                {
                    bioPartNoNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }
            }

            CswNbtMetaDataNodeType supplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            if( null != supplyNT )
            {
                CswNbtMetaDataNodeTypeProp supplyPartNoNTP = supplyNT.getNodeTypeProp( CswNbtObjClassMaterial.PartNumberPropertyName );
                if( null != supplyPartNoNTP )
                {
                    supplyPartNoNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }
            }
            #endregion

            #region PART FIVE
            //Remove 'Supplier'from add layout so it doesn' show up in Create Materials Wizard
            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeProp supplierNTP = sizeNT.getNodeTypeProp("Supplier");
                if (null != supplierNTP)
                {
                    supplierNTP.removeFromLayout(CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add);
                }
            }

            #endregion

        }//Update()

    }//class CswUpdateSchemaCase27055

}//namespace ChemSW.Nbt.Schema