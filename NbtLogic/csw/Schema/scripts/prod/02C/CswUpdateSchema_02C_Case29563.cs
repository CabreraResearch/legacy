using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29563
    /// </summary>
    public class CswUpdateSchema_02C_Case29563 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29563; }
        }

        private CswEnumNbtNodeTypePermission[] NTPermissions = 
        { 
            CswEnumNbtNodeTypePermission.View, 
            CswEnumNbtNodeTypePermission.Create, 
            CswEnumNbtNodeTypePermission.Edit, 
            CswEnumNbtNodeTypePermission.Delete 
        };

        public override void update()
        {
            //New Document NT
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClass CofADocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
            CswNbtMetaDataNodeType CofANT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "C of A Document" ) ??
                 _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CofADocOC.ObjectClassId, "C of A Document", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswEnumNbtModuleName.CofA, CofANT.NodeTypeId );
            //Default Title
            CswNbtMetaDataNodeTypeProp TitleNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
            TitleNTP.DefaultValue.AsText.Text = "Certificate of Analysis";
            TitleNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            //Set Owner FK to ReceiptLot OC (This needs to be done explicitly for the NTP - see Case 26605)
            CswNbtMetaDataNodeTypeProp OwnerNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            OwnerNTP.PropName = "Receipt Lot";
            OwnerNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), ReceiptLotOC.ObjectClassId );
            //NT Permission
            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != RoleNode )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, CofANT, RoleNode, true );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29563

}//namespace ChemSW.Nbt.Schema