using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30014
    /// </summary>
    public class CswUpdateSchema_02E_Case30014: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30014; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass sampleOC  = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "SampleClass" );
            CswNbtMetaDataObjectClass testOC    = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "TestClass" );
            CswNbtMetaDataObjectClass resultOC  = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "ResultClass" );
            CswNbtMetaDataObjectClass paramOC   = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "ParameterClass" );
            CswNbtMetaDataObjectClass aliquotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "AliquotClass" );
            CswNbtMetaDataObjectClass bioOC     = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "BiologicalClass" );
            
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( sampleOC );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( testOC );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( resultOC );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( paramOC );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( aliquotOC );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( bioOC );

            _CswNbtSchemaModTrnsctn.deleteModule( "CCPro" );
            _CswNbtSchemaModTrnsctn.deleteModule( "stis" );
            _CswNbtSchemaModTrnsctn.deleteModule( "BioSafety" );

        } // update()

    }//class CswUpdateSchema_02C_Case30014

}//namespace ChemSW.Nbt.Schema