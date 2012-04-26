
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
            CswNbtView MSDSView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "MSDS Expiring Next Month" );
            if( null == MSDSView )
            {
                MSDSView = _CswNbtSchemaModTrnsctn.makeView();
                MSDSView.makeNew( "MSDS Expiring Next Month", NbtViewVisibility.Global );
                MSDSView.ViewMode = NbtViewRenderingMode.Tree;
                MSDSView.Category = "Materials";
            }
            MSDSView.Root.ChildRelationships.Clear();

            CswNbtMetaDataNodeType MaterialDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != MaterialDocumentNt )
            {
                CswNbtViewRelationship DocumentVr = MSDSView.AddViewRelationship( MaterialDocumentNt, true );
                CswNbtMetaDataNodeTypeProp ExpirationDateNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.ExpirationDatePropertyName );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, ExpirationDateNtp, "today+30", FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );
                CswNbtMetaDataNodeTypeProp DocumentClassNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.DocumentClassPropertyName );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, DocumentClassNtp, "MSDS" );
                MSDSView.save();
            }
            else
            {
                MSDSView.Delete();
            }

        }//Update()

    }//class CswUpdateSchemaCase26016

}//namespace ChemSW.Nbt.Schema