using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28750
    /// </summary>
    public class CswUpdateSchema_01W_Case28750 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28750; }
        }

        public override void update()
        {

            //Re-order the material Supplier, Tradename and Part no props to be Tradename, Supplier, Part No on the edit layout
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp tradenameNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
                CswNbtMetaDataNodeTypeProp supplierNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
                CswNbtMetaDataNodeTypeProp partNoNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );
                CswNbtMetaDataNodeTypeTab materialIdentityNTT = materialNT.getIdentityTab();

                int RowIdx = 1;
                _updatePropLayoutOrder( materialNT, tradenameNTP, RowIdx, materialIdentityNTT );
                RowIdx++;
                _updatePropLayoutOrder( materialNT, supplierNTP, RowIdx, materialIdentityNTT );
                RowIdx++;
                _updatePropLayoutOrder( materialNT, partNoNTP, RowIdx, materialIdentityNTT );

                if( materialNT.NodeTypeName.Equals( "Supply" ) )
                {
                    CswNbtMetaDataNodeTypeTab supplyNTT = materialNT.getNodeTypeTab( "Supply" );
                    CswNbtMetaDataNodeTypeTab pictureNTT = materialNT.getNodeTypeTab( "Picture" );

                    if( null != supplyNTT && null != pictureNTT )
                    {
                        supplyNTT.TabOrder = 1;
                        pictureNTT.TabOrder = 2;
                    }
                }
            }



        } //Update()

        private void _updatePropLayoutOrder( CswNbtMetaDataNodeType MaterialNT, CswNbtMetaDataNodeTypeProp NTP, int RowIdx, CswNbtMetaDataNodeTypeTab IdentityTab )
        {
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MaterialNT.NodeTypeId, NTP, true,
                TabId: IdentityTab.TabId,
                DisplayRow: RowIdx,
                DisplayColumn: 1
            );
        }

    }//class CswUpdateSchema_01W_Case28750

}//namespace ChemSW.Nbt.Schema