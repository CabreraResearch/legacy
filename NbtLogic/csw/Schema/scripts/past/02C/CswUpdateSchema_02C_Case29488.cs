using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29488
    /// </summary>
    public class CswUpdateSchema_02C_Case29488 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29488; }
        }

        public override void update()
        {
            // Add nodetypes for new object classes
            CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
            CswNbtMetaDataObjectClassProp RegListMemberChemicalOCP = RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical );
            CswNbtMetaDataNodeType RegListMemberNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( RegListMemberOC )
                {
                    NodeTypeName = "Regulatory List Member",
                    Category = "Materials",
                    SearchDeferObjectClassPropId = RegListMemberChemicalOCP.ObjectClassPropId
                } );
            RegListMemberNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical ) + "-" + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList ) );
            
            RegListMemberNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.ByUser ).removeFromLayout( CswEnumNbtLayoutType.Add );

            CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
            CswNbtMetaDataObjectClassProp RegListCasNoRegListOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList );
            CswNbtMetaDataNodeType RegListCasNoNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( RegListCasNoOC )
                {
                    NodeTypeName = "Regulatory List CAS",
                    Category = "Materials",
                    SearchDeferObjectClassPropId = RegListCasNoRegListOCP.ObjectClassPropId
                } );
            RegListCasNoNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList ) + "-" + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo ) );

            // Grant permissions to cispro_admin
            CswNbtObjClassRole CISProAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            CswEnumNbtNodeTypePermission[] AllPerms = new CswEnumNbtNodeTypePermission[]
                {
                    CswEnumNbtNodeTypePermission.Create, CswEnumNbtNodeTypePermission.Delete, CswEnumNbtNodeTypePermission.Edit, CswEnumNbtNodeTypePermission.View
                };
            _CswNbtSchemaModTrnsctn.Permit.set( AllPerms, RegListMemberNT, CISProAdminRole, true );
            _CswNbtSchemaModTrnsctn.Permit.set( AllPerms, RegListCasNoNT, CISProAdminRole, true );

        } // update()

    }//class CswUpdateSchema_02C_Case29488

}//namespace ChemSW.Nbt.Schema