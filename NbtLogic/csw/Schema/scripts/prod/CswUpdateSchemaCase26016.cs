
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26016
    /// </summary>
    public class CswUpdateSchemaCase26016 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.ViewSelect.deleteViewByName( "MSDS Expiring Next Month" );
            CswNbtView MSDSView = _CswNbtSchemaModTrnsctn.makeView();
            MSDSView.makeNew( "MSDS Expiring Next Month", NbtViewVisibility.Global );
            MSDSView.ViewMode = NbtViewRenderingMode.Tree;
            MSDSView.Category = "Materials";

            CswNbtMetaDataNodeType MaterialDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != MaterialDocumentNt )
            {
                CswNbtViewRelationship DocumentVr = MSDSView.AddViewRelationship( MaterialDocumentNt, true );
                CswNbtMetaDataNodeTypeProp ExpirationDateNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.ExpirationDatePropertyName );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, ExpirationDateNtp, "today+30", FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );
                CswNbtMetaDataNodeTypeProp DocumentClassNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.DocumentClassPropertyName );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, DocumentClassNtp, "MSDS" );
                MSDSView.save();
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.CISPro, MaterialDocumentNt.NodeTypeId );
            }

            CswNbtMetaDataNodeType ContainerDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
            if( null != ContainerDocumentNt )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.CISPro, ContainerDocumentNt.NodeTypeId );
            }

        }//Update()

    }//class CswUpdateSchemaCase26016

}//namespace ChemSW.Nbt.Schema