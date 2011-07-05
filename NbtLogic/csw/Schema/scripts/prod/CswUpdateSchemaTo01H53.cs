using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-53
    /// </summary>
    public class CswUpdateSchemaTo01H53 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 53 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H53( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            foreach( CswNbtMetaDataNodeType InspectNt in InspectionOC.NodeTypes )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in InspectNt.NodeTypeProps )
                {
                    if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                    {
                        CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                        CompliantAnswers.FromString( Prop.ValueOptions );
                        if( CompliantAnswers.Count == 1 )
                        {
                            Prop.DefaultValue.AsQuestion.Answer = CompliantAnswers.ToString();
                        }
                    }
                }
            }


        } // update()


    }//class CswUpdateSchemaTo01H53

}//namespace ChemSW.Nbt.Schema

