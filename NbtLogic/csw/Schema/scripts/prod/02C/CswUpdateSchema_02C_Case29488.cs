using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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
            {
                CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                CswNbtMetaDataObjectClassProp RegListMemberRegListOCP = RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList );
                CswNbtMetaDataNodeType RegListMemberNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( RegListMemberOC )
                    {
                        NodeTypeName = "Regulatory List Member",
                        Category = "Materials",
                        SearchDeferObjectClassPropId = RegListMemberRegListOCP.ObjectClassPropId
                    } );
                RegListMemberNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical ) + "-" + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList ) );
            }
            {
                CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
                CswNbtMetaDataObjectClassProp RegListCasNoRegListOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList );
                CswNbtMetaDataNodeType RegListCasNoNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( RegListCasNoOC )
                    {
                        NodeTypeName = "Regulatory List CAS",
                        Category = "Materials",
                        SearchDeferObjectClassPropId = RegListCasNoRegListOCP.ObjectClassPropId
                    } );
                RegListCasNoNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList ) + "-" + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo ) );
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29488

}//namespace ChemSW.Nbt.Schema