using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Properties;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27646
    /// </summary>
    public class CswUpdateSchema_01S_Case27646 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType materialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            CswNbtMetaDataNodeTypeProp documentClassNTP = materialDocumentNT.getNodeTypeProp( "Document Class" );
            if( null != materialDocumentNT && null != documentClassNTP )
            {
                CswNbtMetaDataNodeTypeTab materialDocumentNTT = materialDocumentNT.getFirstNodeTypeTab();
                if( null != materialDocumentNTT )
                {
                    CswNbtMetaDataNodeTypeProp issueDateNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( materialDocumentNT, CswNbtMetaDataFieldType.NbtFieldType.DateTime, "Issue Date", materialDocumentNTT.TabId );

                    issueDateNTP.setFilter( documentClassNTP, documentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.MSDS );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                        CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                        materialDocumentNT.NodeTypeId,
                        issueDateNTP.PropId,
                        true,
                        materialDocumentNTT.TabId );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                        CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                        materialDocumentNT.NodeTypeId,
                        issueDateNTP.PropId,
                        true,
                        materialDocumentNTT.TabId );
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema