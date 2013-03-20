using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Test.ServiceDrivers
{
    public class CswNbtSdResourcesMgr
    {

        private readonly CswNbtResources _CswNbtResources;
        
        public CswNbtSdResourcesMgr( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public const string DefaultPassword = "Chemsw123!";

        private ICswUser InitSystemUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswSystemUserNames.SysUsr_NbtWebSvcMgr );
        }

        private ICswUser _InitNewUser( ICswResources Resources, CswNbtResources NbtResources, string UserName, CswNbtObjClassRole Role, string Password = DefaultPassword )
        {
            ICswUser Ret = null;
            if( null != NbtResources &&
                false == string.IsNullOrEmpty( UserName ) && 
                null != Role )
            {
                //ICswResources IResources = Resources;
                //CswNbtResources NbtResources = (CswNbtResources) IResources;
                CswNbtMetaDataObjectClass UserOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataNodeType UserNt = UserOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != UserNt )
                {
                    CswNbtObjClassUser NewUser = NbtResources.Nodes.makeNodeFromNodeTypeId( UserNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation : true );
                    NewUser.UsernameProperty.Text = UserName;
                    NewUser.Role.RelatedNodeId = Role.NodeId;
                    NewUser.PasswordProperty.Password = Password;
                    NewUser.postChanges( ForceUpdate: false );
                    
                    _finalize( NbtResources );
                    
                    Ret = NewUser;
                }
            }
            if( null == Ret )
            {
                throw new CswDniException( "Could not intialize new user" );
            }
            return Ret;
        }

        private ICswUser _InitUser( ICswResources Resources, CswNbtResources NbtResources, string UserName )
        {
            ICswUser Ret = null;
            if( null != NbtResources &&
                false == string.IsNullOrEmpty( UserName ) )
            {
                CswNbtObjClassUser NewUser = NbtResources.Nodes.makeUserNodeFromUsername( UserName );
                Ret = NewUser;
                
            }
            if( null == Ret )
            {
                throw new CswDniException( "Could not intialize new user" );
            }
            return Ret;
        }

        private void _ValidateAccessId( string AccessId )
        {
            if( string.IsNullOrEmpty( AccessId ) ||
                false == _CswNbtResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
            {
                throw new CswDniException( ErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        /// <summary>
        /// We're going to be switching Resources like mad men, so better to finalize any time we write a node in this class
        /// </summary>
        private void _finalize( CswNbtResources Resources )
        {
            if( null != Resources )
            {
                Resources.finalize();
            }
        }

        public CswNbtResources makeSystemUserResources( string AccessId )
        {
            _ValidateAccessId( AccessId );
            //CswNbtResources OtherResources = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            CswNbtResources OtherResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.TestProject, true, false );
            OtherResources.AccessId = AccessId;
            OtherResources.InitCurrentUser = InitSystemUser;
            return OtherResources;
        }

        public CswNbtResources makeNewUserResources( string UserName, CswPrimaryKey RoleId )
        {
            //CswNbtResources Ret = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            CswNbtResources Ret = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.TestProject, true, false );
            Ret.AccessId = _CswNbtResources.AccessId;
            CswNbtObjClassRole Role = Ret.Nodes[RoleId];
            Ret.InitCurrentUser = Resources => _InitNewUser( Resources, Ret, UserName, Role );
            return Ret;
        }

        public CswNbtResources makeUserResources( string UserName )
        {
            //CswNbtResources Ret = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            CswNbtResources Ret = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.TestProject, true, false );
            Ret.AccessId = _CswNbtResources.AccessId;
            Ret.InitCurrentUser = Resources => _InitUser( Resources, Ret, UserName );
            return Ret;
        }

        //Make a new role, commit it, and return the NodeId
        public CswPrimaryKey makeNewRole( string RoleName )
        {
            CswPrimaryKey Ret = null;
            if( false == string.IsNullOrEmpty( RoleName ) )
            {
                CswNbtMetaDataObjectClass RoleOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RoleClass );
                CswNbtMetaDataNodeType RoleNt = RoleOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != RoleNt )
                {
                    CswNbtObjClassRole Role = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RoleNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: true );
                    Role.Name.Text = RoleName;
                    Role.postChanges( ForceUpdate: false );
                    
                    _finalize( _CswNbtResources );
                    
                    Ret = Role.NodeId;
                }
                
            }
            return Ret;
        }

    } // class CswNbtSdResourcesMgr

} // namespace ChemSW.Nbt.Test.ServiceDrivers
