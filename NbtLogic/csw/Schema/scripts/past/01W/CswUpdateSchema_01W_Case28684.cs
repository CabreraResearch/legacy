using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28684
    /// </summary>
    public class CswUpdateSchema_01W_Case28684 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28684; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClass roleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole roleNode in roleOC.getNodes( false, false, false, true ) )
            {
                if( roleNode.Name.Text.ToLower().Contains( "cispro" ) && false == roleNode.Name.Text.ToLower().Equals( "cispro_admin" ) )
                {
                    foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Delete, containerNT, roleNode, false );
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01W_Case28684

}//namespace ChemSW.Nbt.Schema