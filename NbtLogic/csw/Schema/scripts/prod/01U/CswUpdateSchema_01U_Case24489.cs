using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24489
    /// </summary>
    public class CswUpdateSchema_01U_Case24489 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            if( null != ContainerLocationOc )
            {
                CswNbtMetaDataNodeType ContainerLocationNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Location" );
                if( null == ContainerLocationNt )
                {
                    //Create new NodeType
                    ContainerLocationNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( ContainerLocationOc.ObjectClassId, "Container Location", "Materials" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, ContainerLocationNt.NodeTypeId );
                }
            }
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 24489; }
        }

    }//class CswUpdateSchema_01U_Case24489

}//namespace ChemSW.Nbt.Schema