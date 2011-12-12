using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-04
    /// </summary>
    public class CswUpdateSchemaTo01L04 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 04 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24023

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp RunNowOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.RunNowPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RunNowOcp, CswNbtSubField.SubFieldName.Text, "Run Now" );

            foreach( CswNbtNode GeneratorNode in GeneratorOc.getNodes( true, false ) )
            {
                CswNbtObjClassGenerator Generator = CswNbtNodeCaster.AsGenerator( GeneratorNode );
                Generator.RunNow.Text = "Run Now";
            }

            #endregion Case 24023


        }//Update()

    }//class CswUpdateSchemaTo01L04

}//namespace ChemSW.Nbt.Schema


