using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-60
    /// </summary>
    public class CswUpdateSchemaTo01H60 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 60 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H60( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //Case 22608
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp FinishedOcp = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FinishedOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClassProp CancelledOcp = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CancelledOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClass RolesOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp AdministratorOcp = RolesOC.getObjectClassProp( CswNbtObjClassRole.AdministratorPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AdministratorOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AdministratorOcp, CswNbtSubField.SubFieldName.Checked, Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();
        } // update()


    }//class CswUpdateSchemaTo01H60

}//namespace ChemSW.Nbt.Schema

