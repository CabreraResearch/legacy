using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29812
    /// </summary>
    public class CswUpdateSchema_02B_Case29812: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29812; }
        }

        public override void update()
        {
            //Give Document permissions to CISPro_Admin
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtObjClassRole CISPro_Admin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );

            CswEnumNbtNodeTypePermission[] permissions = new[]
                {
                    CswEnumNbtNodeTypePermission.Create,
                    CswEnumNbtNodeTypePermission.Delete,
                    CswEnumNbtNodeTypePermission.Edit,
                    CswEnumNbtNodeTypePermission.View
                };

            if( null != CISPro_Admin )
            {
                foreach( CswNbtMetaDataNodeType DocumentNT in DocumentOC.getNodeTypes() )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( permissions, DocumentNT, CISPro_Admin, true );
                }
            }


        } // update()

    }//class CswUpdateSchema_02B_Case29812

}//namespace ChemSW.Nbt.Schema