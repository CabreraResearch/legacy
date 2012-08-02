using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27462
    /// </summary>
    public class CswUpdateSchema_MaterialComponents_Case27462 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialComponentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp PercentageOcp = MaterialComponentOc.getObjectClassProp( CswNbtObjClassMaterialComponent.PercentagePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PercentageOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PercentageOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClassProp MixtureOcp = MaterialComponentOc.getObjectClassProp( CswNbtObjClassMaterialComponent.MixturePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            // This is a placeholder script that does nothing.
        }//Update()

    }//class CswUpdateSchema_MaterialComponents_Case27462

}//namespace ChemSW.Nbt.Schema