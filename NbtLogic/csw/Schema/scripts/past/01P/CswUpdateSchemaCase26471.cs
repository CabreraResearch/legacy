using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26471
    /// </summary>
    public class CswUpdateSchemaCase26471 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialSynonymOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass );

            CswNbtMetaDataObjectClassProp NameOcp = MaterialSynonymOc.getObjectClassProp( CswNbtObjClassMaterialSynonym.NamePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_col_add, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_row_add, 1 );

            CswNbtMetaDataObjectClassProp MaterialOcp = MaterialSynonymOc.getObjectClassProp( CswNbtObjClassMaterialSynonym.MaterialPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_col_add, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_row_add, 2 );

        }//Update()

    }//class CswUpdateSchemaCase26471

}//namespace ChemSW.Nbt.Schema