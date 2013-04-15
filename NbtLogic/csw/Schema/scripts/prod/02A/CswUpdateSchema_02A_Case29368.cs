using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29368
    /// </summary>
    public class CswUpdateSchema_02A_Case29368 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29368; }
        }

        public override void update()
        {
            CswEnumNbtNodeTypePermission[] NTPermissions = 
            { 
                CswEnumNbtNodeTypePermission.View, 
                CswEnumNbtNodeTypePermission.Create, 
                CswEnumNbtNodeTypePermission.Edit, 
                CswEnumNbtNodeTypePermission.Delete 
            };

            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != RoleNode )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.HMIS_Reporting, RoleNode, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Tier_II_Reporting, RoleNode, true );
                CswNbtMetaDataNodeType ControlZoneNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                if( null != ControlZoneNT )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, ControlZoneNT, RoleNode, true );
                }
            }

        } // update()
    }//class CswUpdateSchema_02A_Case29368
}//namespace ChemSW.Nbt.Schema