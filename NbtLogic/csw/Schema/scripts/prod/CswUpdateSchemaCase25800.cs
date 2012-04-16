using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25800
    /// </summary>
    public class CswUpdateSchemaCase25800 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass MaterialSynonymOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass );
            CswNbtMetaDataObjectClassProp MaterialSynonymMaterialOcp = MaterialSynonymOc.getObjectClassProp( CswNbtObjClassMaterialSynonym.MaterialPropertyName );
            CswNbtMetaDataObjectClassProp MaterialSynonymNameOcp = MaterialSynonymOc.getObjectClassProp( CswNbtObjClassMaterialSynonym.NamePropertyName );

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SynonymProp = MaterialNt.getNodeTypeProp( "Synonyms" );
                CswNbtMetaDataNodeTypeTab IdentityTab = MaterialNt.getNodeTypeTab( "Identity" );
                if( null == IdentityTab )
                {
                    IdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNt, "Identity", 1 );
                }

                if( null == SynonymProp )
                {
                    SynonymProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MaterialNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Synonyms", "Identity" );
                }
                else
                {
                    CswNbtView OldSynonymsView = _CswNbtSchemaModTrnsctn.restoreView( SynonymProp.ViewId );
                    if( null != OldSynonymsView )
                    {
                        OldSynonymsView.Delete();
                    }
                }

                CswNbtView SynonymsView = _CswNbtSchemaModTrnsctn.makeView();
                SynonymsView.makeNew( "Synonyms_" + MaterialNt.NodeTypeName, NbtViewVisibility.Property );
                SynonymsView.ViewMode = NbtViewRenderingMode.Grid;

                CswNbtViewRelationship MaterialRelationship = SynonymsView.AddViewRelationship( MaterialNt, false );
                CswNbtViewRelationship SynonymRelationship = SynonymsView.AddViewRelationship( MaterialRelationship, NbtViewPropOwnerType.Second, MaterialSynonymMaterialOcp );
                SynonymsView.AddViewProperty( SynonymRelationship, MaterialSynonymNameOcp );
                SynonymsView.save();

                SynonymProp.Extended = CswNbtNodePropGrid.GridPropMode.Small.ToString();
                SynonymProp.MaxValue = 10;
                SynonymProp.ViewId = SynonymsView.ViewId;

            }
        }//Update()

    }//class CswUpdateSchemaCase25800

}//namespace ChemSW.Nbt.Schema