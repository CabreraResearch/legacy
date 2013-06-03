using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29703
    /// </summary>
    public class CswUpdateSchema_02B_Case29703 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29703; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType RoleNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" );
            if( RoleNodeType != null )
            {
                CswNbtObjClassRole ViewOnlyRole = _createRole( RoleNodeType, "View Only" );

                CswNbtMetaDataNodeType UserNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "User" );
                if( UserNodeType != null )
                {
                    _createUser( UserNodeType, ViewOnlyRole, "view_only" );
                    _createUser( UserNodeType, ViewOnlyRole, "unknown_import_user" );
                }
            }
        }

        private CswNbtObjClassRole _createRole( CswNbtMetaDataNodeType RoleNodeType, string RoleName )
        {
            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
            RoleNode.Name.Text = RoleName;
            RoleNode.Timeout.Value = 30;
            CswEnumNbtNodeTypePermission[] ViewPermissions = { CswEnumNbtNodeTypePermission.View };
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                bool canView = false == ( NodeType.NodeTypeName == "Role" || NodeType.NodeTypeName == "User" );
                _CswNbtSchemaModTrnsctn.Permit.set( ViewPermissions, NodeType, RoleNode, canView );
            }
            RoleNode.postChanges( false );
            return RoleNode;
        }

        private void _createUser( CswNbtMetaDataNodeType UserNodeType, CswNbtObjClassRole RoleNode, string UserName )
        {
            CswNbtObjClassUser UserNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
            UserNode.UsernameProperty.Text = UserName;
            UserNode.PasswordProperty.Password = UserName+"123!";
            UserNode.Role.RelatedNodeId = RoleNode.NodeId;
            UserNode.LanguageProperty.Value = "en";
            UserNode.AccountLocked.Checked = CswEnumTristate.True;
            
            UserNode.postChanges( false );
        }

    }//class CswUpdateSchema_02B_Case29703

}//namespace ChemSW.Nbt.Schema