using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01T_CertMethod_Case27868 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass CertMethodTempOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CertMethodTemplateClass );
            CswNbtMetaDataNodeType CertMethodTempNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertMethodTempOc )
            {
                Category = "MLM",
                NodeTypeName = "C of A Method Template"

            } );

            CswNbtMetaDataObjectClass CertMethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CertMethodClass );
            CswNbtMetaDataNodeType CertMethodNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertMethodOc )
            {
                Category = "MLM",
                NodeTypeName = "C of A Method"

            } );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27868; }
        }

        //Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema