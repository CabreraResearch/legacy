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
            get { return CswEnumDeveloper.NBT; }
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
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataNodeType CofANT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOC.ObjectClassId, "C of A Document", "MLM" );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswEnumNbtModuleName.CofA, CofANT.NodeTypeId );
            CswNbtMetaDataNodeTypeProp RevisionDateNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( CofANT, CswEnumNbtFieldType.DateTime, "Revision Date", CofANT.getFirstNodeTypeTab().TabId );
            //Default Title
            CswNbtMetaDataNodeTypeProp TitleNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
            TitleNTP.DefaultValue.AsText.Text = "Certificate of Analysis";
            TitleNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            //Set Owner FK to ReceiptLot OC
            CswNbtMetaDataNodeTypeProp OwnerNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            OwnerNTP.PropName = "Receipt Lot";
            OwnerNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), ReceiptLotOC.ObjectClassId );
            //Remove unused Props from Layouts
            CswNbtMetaDataNodeTypeProp DocumentClassNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
            DocumentClassNTP.removeFromAllLayouts();
            CswNbtMetaDataNodeTypeProp LanguageNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
            LanguageNTP.removeFromAllLayouts();
            CswNbtMetaDataNodeTypeProp FormatNTP = CofANT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
            FormatNTP.removeFromAllLayouts();
            //NT Permission
            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != RoleNode )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, CofANT, RoleNode, true );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29563

}//namespace ChemSW.Nbt.Schema