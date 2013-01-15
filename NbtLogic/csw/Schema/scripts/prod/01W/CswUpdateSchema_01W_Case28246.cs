using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28246
    /// </summary>
    public class CswUpdateSchema_01W_Case28246 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28246; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass MaterialSynonymsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialSynonymClass );
            foreach( CswNbtMetaDataNodeType MaterialSynonymOCNT in MaterialSynonymsOC.getNodeTypes() )
            {
                /* Remove the Language and Type props from the add layout */
                CswNbtMetaDataNodeTypeProp TypeNTP = MaterialSynonymOCNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Type );
                if( null != TypeNTP )
                {
                    TypeNTP.removeFromLayout( LayoutType: CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }

                CswNbtMetaDataNodeTypeProp LanguageNTP = MaterialSynonymOCNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Language );
                if( null != LanguageNTP )
                {
                    LanguageNTP.removeFromLayout( LayoutType: CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }

                /* For pre-existing nodes, set the new properties to the default value */
                foreach( CswNbtObjClassMaterialSynonym MaterialSynonymNode in MaterialSynonymOCNT.getNodes( false, true ) )
                {
                    if( MaterialSynonymNode.Type.Empty )
                    {
                        MaterialSynonymNode.Type.Value = CswNbtObjClassMaterialSynonym.Types.Synonym;
                        MaterialSynonymNode.postChanges( false );
                    }

                    if( MaterialSynonymNode.Language.Empty )
                    {
                        MaterialSynonymNode.Language.Value = CswNbtObjClassMaterialSynonym.Languages.English;
                        MaterialSynonymNode.postChanges( false );
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28246

}//namespace ChemSW.Nbt.Schema