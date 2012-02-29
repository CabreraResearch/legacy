using System;
using ChemSW.Nbt.MetaData;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M10
    /// </summary>
    public class CswUpdateSchemaTo01M10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24462

            Int32 PackageOcId = _CswNbtSchemaModTrnsctn.getObjectClassId( "Package" );
            if( Int32.MinValue != PackageOcId )
            {
                CswNbtMetaDataObjectClass PackageOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PackageOcId );
                if( null != PackageOc )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( PackageOc );
                }
            }

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            Int32 PackDetailOcId = _CswNbtSchemaModTrnsctn.getObjectClassId( "PackDetail" );
            if( Int32.MinValue != PackDetailOcId )
            {
                CswNbtMetaDataObjectClass PackDetailOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PackDetailOcId );
                if( null != PackDetailOc )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( PackDetailOc );
                }
            }

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            try
            {
                _CswNbtSchemaModTrnsctn.dropTable( "packages" );
            }
            catch( Exception ) { }
            try
            {
                _CswNbtSchemaModTrnsctn.dropTable( "packages_audit" );
            }
            catch( Exception ) { }
            try
            {
                _CswNbtSchemaModTrnsctn.dropTable( "packdetail" );
            }
            catch( Exception ) { }
            try
            {
                _CswNbtSchemaModTrnsctn.dropTable( "packdetail_audit" );
            }
            catch( Exception ) { }

            #endregion case 24462

        }//Update()

    }//class CswUpdateSchemaTo01M09

}//namespace ChemSW.Nbt.Schema