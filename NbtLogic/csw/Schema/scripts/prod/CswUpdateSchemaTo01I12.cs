using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-12
    /// </summary>
    public class CswUpdateSchemaTo01I12 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 12 ); } }
        public CswUpdateSchemaTo01I12( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserDtp = UserOC.getObjectClassProp( CswNbtObjClassUser.DateFormatPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserDtp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, "M/d/yyyy,d-M-yyyy,yyyy/M/d,dd MMM yyyy" );
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();
        }//Update()

    }//class CswUpdateSchemaTo01I12

}//namespace ChemSW.Nbt.Schema


