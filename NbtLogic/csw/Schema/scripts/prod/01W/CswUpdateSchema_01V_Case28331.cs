using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28331
    /// </summary>
    public class CswUpdateSchema_01V_Case28331: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 28331; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab IdentityTab = MaterialNt.getIdentityTab();
                if( null != IdentityTab )
                {
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Receive );
                    ReceiveNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 2, DisplayRow : 1, TabId : IdentityTab.TabId );
                    CswNbtMetaDataNodeTypeProp RequestNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Request );
                    RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 2, DisplayRow : 2, TabId : IdentityTab.TabId );
                    CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );
                    PartNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 1, DisplayRow : 1, TabId : IdentityTab.TabId );
                    CswNbtMetaDataNodeTypeProp TradenameNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
                    TradenameNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 1, DisplayRow : 2, TabId : IdentityTab.TabId );
                    CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
                    SupplierNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 1, DisplayRow : 3, TabId : IdentityTab.TabId );
                }
            }
        } //Update()

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema