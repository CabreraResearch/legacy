using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28898
    /// </summary>
    public class CswUpdateSchema_01Y_Case28898 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28898; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            foreach( CswNbtMetaDataNodeType DocumentNT in DocumentOC.getNodeTypes() )
            {
                if( DocumentNT.NodeTypeName == "Material Document" || DocumentNT.NodeTypeName == "SDS Document" )
                {
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.SDS, DocumentNT.NodeTypeId );
                }
            }
        } //Update()
    }//class CswUpdateSchema_01Y_Case28898
}//namespace ChemSW.Nbt.Schema