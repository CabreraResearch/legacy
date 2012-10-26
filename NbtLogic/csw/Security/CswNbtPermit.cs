using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Security
{
    /// <summary>
    /// Security class responsible for enforcing all permissions
    /// </summary>
    public class CswNbtPermit
    {


        private class CswNbtPermitInfo
        {
            private CswNbtResources _CswNbtResources = null;
            private ICswNbtUser _CswNbtUser = null;
            private CswNbtObjClassRole _CswNbtObjClassRole = null;
            public CswNbtMetaDataNodeType NodeType = null;
            public NodeTypePermission Permission = NodeTypePermission.View;


            public CswNbtPermitInfo( CswNbtResources CswNbtResources, ICswNbtUser CswNbtUser, CswNbtMetaDataNodeType NodeTypeIn, NodeTypePermission PermissionIn )
            {
                NodeType = NodeTypeIn;
                _CswNbtResources = CswNbtResources;
                _CswNbtUser = CswNbtUser;
                Permission = PermissionIn;
            }//ctor


            public bool shouldPermissionCheckProceed()
            {
                return ( ( null != NodeType ) && ( null != User ) && ( null != Role ) && ( true == NoExceptionCases ) );
            }

            public ICswNbtUser User
            {
                get
                {
                    if( null == _CswNbtUser )
                    {
                        _CswNbtUser = _CswNbtResources.CurrentNbtUser;
                    }

                    return ( _CswNbtUser );

                }//get

            }//User

            public CswNbtObjClassRole Role
            {
                get
                {
                    if( null == _CswNbtObjClassRole )
                    {
                        if( null != User )
                        {
                            _CswNbtObjClassRole = _CswNbtResources.Nodes[User.RoleId];
                        }

                    }//if role is null

                    return ( _CswNbtObjClassRole );

                }//get

            }//Role


            private bool _UserTypeIsKnown = false;
            private bool _IsUberUser = false;
            public bool IsUberUser
            {
                get
                {
                    if( false == _UserTypeIsKnown )
                    {
                        _UserTypeIsKnown = true;

                        if( null != User )
                        {
                            if( User is CswNbtSystemUser || User.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                            {
                                _IsUberUser = true;
                            }

                        }//if user is not null

                    }//if we haven't set the user level yet

                    return ( _IsUberUser );
                }
            }//IsUberUser


            private bool _ExceptionCasesHaveBeenChecked = false;
            private bool _NoExceptionCases = true;
            public bool NoExceptionCases
            {
                get
                {

                    if( false == _ExceptionCasesHaveBeenChecked )
                    {
                        _ExceptionCasesHaveBeenChecked = true;

                        // Only Administrators can edit Roles

                        NbtObjectClass ObjectClass = NodeType.getObjectClass().ObjectClass;

                        if( ObjectClass == NbtObjectClass.RoleClass &&
                          Permission != NodeTypePermission.View &&
                          false == User.IsAdministrator() )
                        {
                            _NoExceptionCases = false;
                        }

                        // case 24510
                        if( ObjectClass == NbtObjectClass.ContainerDispenseTransactionClass )
                        {
                            _NoExceptionCases = Permission != NodeTypePermission.Delete;
                        }

                    }//if we have not already checked 

                    return ( _NoExceptionCases );

                }//get

            }//NoExceptionCases

        }//CswNbtPermitInfo()


        private CswNbtPermitInfo _CswNbtPermitInfo = null;
        private void _initPermissionInfo( ICswNbtUser CswNbtUser, CswNbtMetaDataNodeType NodeType, NodeTypePermission Permission )
        {

            if( ( null == _CswNbtPermitInfo ) || ( _CswNbtPermitInfo.User != CswNbtUser ) || ( _CswNbtPermitInfo.NodeType != NodeType ) || ( _CswNbtPermitInfo.Permission != Permission ) )
            {
                _CswNbtPermitInfo = new CswNbtPermitInfo( _CswNbtResources, CswNbtUser, NodeType, Permission );
            }

        }//_initPreReqs()


        /// <summary>
        /// Type of Permission on NodeTypes
        /// </summary>
        public enum NodeTypePermission
        {
            /// <summary>
            /// Permission to view nodes of this type
            /// </summary>
            View,
            /// <summary>
            /// Permission to create new nodes of this type
            /// </summary>
            Create,
            /// <summary>
            /// Permission to delete nodes of this type
            /// </summary>
            Delete,
            /// <summary>
            /// Permission to edit property values of nodes of this type
            /// </summary>
            Edit
        }

        /// <summary>
        /// Type of Permission on NodeTypeTabs
        /// </summary>
        public enum NodeTypeTabPermission
        {
            /// <summary>
            /// Permission to view the tab
            /// </summary>
            View,
            /// <summary>
            /// Permission to edit property values on this tab
            /// </summary>
            Edit
        }

        private CswNbtResources _CswNbtResources;
        public CswNbtPermit( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        // This is probably a performance problem!
        private CswNbtObjClassRole _getRole( CswPrimaryKey RoleId )
        {
            CswNbtNode RoleNode = _CswNbtResources.Nodes[RoleId];
            return (CswNbtObjClassRole) RoleNode;
        }

        #region NodeTypes


        //private ICswNbtUser _vetUser( ICswNbtUser User )
        //{
        //    ICswNbtUser ReturnVal = User;

        //    if( null == ReturnVal )
        //    {
        //        ReturnVal = _CswNbtResources.CurrentNbtUser;
        //    }

        //    return ( ReturnVal );

        //}//_vetUser()


        //private bool _isUberUser( ICswNbtUser User )
        //{
        //    bool ReturnVal = false;

        //    if( null != User )
        //    {
        //        ReturnVal = User is CswNbtSystemUser || User.Username == CswNbtObjClassUser.ChemSWAdminUsername;
        //    }

        //    return ( ReturnVal );
        //}

        /// <summary>
        /// Does this User have this Permission on this nodetype?
        /// </summary>
        /// <param name="Permission"></param>
        /// <param name="NodeType"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool canNodeType( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null )
        {
            bool ret = false;


            _initPermissionInfo( User, NodeType, Permission );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {
                    // Base case: does the Role have this nodetype permission
                    ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );

                    if( Permission == NodeTypePermission.View )
                    {
                        // Having 'Edit' grants 'View' automatically
                        ret = ret || _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, NodeTypePermission.Edit ) );
                    }

                }//if pre-reqs are satisifed
            }
            else
            {
                ret = true;
            }

            return ( ret );

        }//canNodeType() 


        public bool canTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab NodeTypeTab, ICswNbtUser User = null )
        {
            bool ret = false;

            _initPermissionInfo( User, NodeType, Permission );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {
                if( null != NodeType )
                {
                    ret = canNodeType( _CswNbtPermitInfo.Permission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );
                }
                if( false == ret &&
                    null != NodeTypeTab )
                {
                    if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                    {
                        if( Permission == NodeTypePermission.View ||
                            Permission == NodeTypePermission.Edit )
                        {
                            NodeTypeTabPermission TabPermission = (NodeTypeTabPermission) Enum.Parse( typeof( NodeTypeTabPermission ), Permission.ToString() );
                            ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, TabPermission ) );
                            if( false == ret &&
                                TabPermission == NodeTypeTabPermission.View )
                            {
                                // Having 'Edit' grants 'View' automatically
                                ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, NodeTypeTabPermission.Edit ) );
                            }

                        }//if permission is view or edit

                    }//if pre-reqs are satisified

                }//if tab is not null

            }
            else
            {
                ret = true;
            }

            return ( ret );

        }//catTab() 


        public bool canAnyTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null )
        {
            bool ret = false;

            _initPermissionInfo( User, NodeType, Permission );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {
                    NodeTypeTabPermission TabPermission = (NodeTypeTabPermission) Enum.Parse( typeof( NodeTypeTabPermission ), Permission.ToString() );
                    foreach( CswNbtMetaDataNodeTypeTab CurrentTab in _CswNbtPermitInfo.NodeType.getNodeTypeTabs() )
                    {
                        ret = ret || _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId, TabPermission ) );
                        if( TabPermission == NodeTypeTabPermission.View )
                        {
                            // Having 'Edit' grants 'View' automatically
                            ret = ret || _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId, NodeTypeTabPermission.Edit ) );
                        }

                    }//iterate tabs

                }//if pre-reqs are in order
            }
            else
            {
                ret = true;
            }

            return ( ret );

        }//canAnyTab() 


        public bool canProp( NodeTypePermission Permission, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtNodePropWrapper NodePropWrapper = null, ICswNbtUser User = null )
        {
            bool ret = ( null != MetaDataProp );

            if( ret )
            {
                _initPermissionInfo( User, MetaDataProp.getNodeType(), Permission );

                if( false == _CswNbtPermitInfo.IsUberUser )
                {

                    if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                    {
                        // You can't edit readonly properties
                        if(
                            ( Permission != NodeTypePermission.View ) &&
                            ( MetaDataProp.ServerManaged || MetaDataProp.ReadOnly || ( ( null != NodePropWrapper ) && NodePropWrapper.ReadOnly ) ) &&
                            ( false == MetaDataProp.AllowReadOnlyAdd )
                            ) /* Case 24514. Conditionally Permit edit on create. */
                        {
                            ret = false;
                        }
                        if( ret )
                        {
                            ret = ( null == MetaDataTab || canTab( Permission, _CswNbtPermitInfo.NodeType, MetaDataTab ) );
                            if( ret )
                            {
                                CswNbtMetaDataObjectClassProp OCP = MetaDataProp.getObjectClassProp();

                                // case 8218 - Certain properties on the user's preferences are not allowed to be edited
                                if( _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == NbtObjectClass.UserClass &&
                                    false == _CswNbtPermitInfo.User.IsAdministrator() &&
                                    OCP != null &&
                                    ( OCP.PropName == CswNbtObjClassUser.PropertyName.Username ||
                                      OCP.PropName == CswNbtObjClassUser.PropertyName.Role ||
                                      OCP.PropName == CswNbtObjClassUser.PropertyName.FailedLoginCount ||
                                      OCP.PropName == CswNbtObjClassUser.PropertyName.AccountLocked ) )
                                {
                                    ret = false;
                                }

                                // Only admins can change other people's passwords
                                if( ret &&
                                    _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == NbtObjectClass.UserClass &&
                                    false == _CswNbtPermitInfo.User.IsAdministrator() &&
                                    ( null != NodePropWrapper && _CswNbtPermitInfo.User.UserId != NodePropWrapper.NodeId ) &&
                                    OCP != null &&
                                    OCP.PropName == CswNbtObjClassUser.PropertyName.Password )
                                {
                                    ret = false;
                                }
                            }
                        } //if pre-reqs are satisfied
                    }
                }
                else
                {
                    ret = true;
                }//f-else is uber user

            }//if-else we have metadata prop

            return ( ret );

        }//canProp



        public bool canNode( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswPrimaryKey NodeId, ICswNbtUser User = null )
        {

            bool ret = true;

            _initPermissionInfo( User, NodeType, Permission );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {
                    //the case in which it is a problem that we check nodetype permissions: 
                    //  node type level permissions are off, but a tab has edit permission. 
                    //  this is called from CswNbtSdTabsAndProps::makePropJson(). So here, canNodeType() 
                    // gives us false, which means that the readOnly() status below does not get checked. 
                    // Here's a case where it's better to balkanize these methods and let the caller 
                    // decide how to piece them together. 
                    //ret = canNodeType( Permission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );

                    if( null != NodeId && Int32.MinValue != NodeId.PrimaryKey )
                    {
                        // case 2209 - Users can edit their own profile without permissions to the User nodetype
                        if( !ret &&
                            NodeId == _CswNbtPermitInfo.User.UserId )
                        {
                            ret = true;
                        }

                        // Prevent users from deleting themselves or their own roles
                        if( ret &&
                            Permission == NodeTypePermission.Delete &&
                            ( ( NodeId == _CswNbtPermitInfo.User.UserId ||
                                NodeId == _CswNbtPermitInfo.User.RoleId ) ) )
                        {
                            ret = false;
                        }


                        // case 24510
                        if( _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == NbtObjectClass.ContainerClass )
                        {
                            ret = ret && canContainer( NodeId, Permission, null, _CswNbtPermitInfo.User );
                        }

                        CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                        if( null != Node &&
                            Permission != NodeTypePermission.View && Permission != NodeTypePermission.Delete )
                        {
                            ret = ret && ( _CswNbtPermitInfo.Permission != NodeTypePermission.Edit ||
                                           _CswNbtPermitInfo.User.IsAdministrator() ||
                                           false == Node.ReadOnly );
                        }

                    }//if NodeId is not null



                }//if pre-reqs are satisifed

            }
            else
            {
                ret = true;
            }

            return ( ret );

        }//canNode() 




        ///// <summary>
        ///// Returns true if the user has the appropriate permissions for the nodetype
        ///// </summary>
        //public bool can( NodeTypePermission Permission,
        //                 CswNbtMetaDataNodeType NodeType,
        //                 bool CheckAllTabPermissions = false,
        //                 CswNbtMetaDataNodeTypeTab NodeTypeTab = null,
        //                 ICswNbtUser User = null,
        //                 CswPrimaryKey NodeId = null,
        //                 CswNbtMetaDataNodeTypeProp MetaDataProp = null )
        //{
        //    bool ret = false;

        //    // Default to logged-in user
        //    if( User == null )
        //    {
        //        User = _CswNbtResources.CurrentNbtUser;
        //    }

        //    if( User != null )
        //    {
        //        if( User is CswNbtSystemUser || User.Username == CswNbtObjClassUser.ChemSWAdminUsername )
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            CswNbtObjClassRole Role = _getRole( User.RoleId );
        //            if( Role != null && NodeType != null ) // if no role, no permissions
        //            {
        //                // Base case: does the Role have this nodetype permission
        //                ret = Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
        //                if( Permission == NodeTypePermission.View )
        //                {
        //                    // Having 'Edit' grants 'View' automatically
        //                    ret = ret || Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, NodeTypePermission.Edit ) );
        //                }

        //                // case 8411 - Tab permissions
        //                if( ( CheckAllTabPermissions || NodeTypeTab != null ) &&
        //                    ( Permission == NodeTypePermission.View || Permission == NodeTypePermission.Edit ) )
        //                {
        //                    NodeTypeTabPermission TabPermission = (NodeTypeTabPermission) Enum.Parse( typeof( NodeTypeTabPermission ), Permission.ToString() );
        //                    Collection<CswNbtMetaDataNodeTypeTab> TabsToCheck = new Collection<CswNbtMetaDataNodeTypeTab>();
        //                    if( NodeTypeTab != null )
        //                    {
        //                        TabsToCheck.Add( NodeTypeTab );
        //                    }
        //                    else if( CheckAllTabPermissions )
        //                    {
        //                        foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
        //                        {
        //                            TabsToCheck.Add( Tab );
        //                        }
        //                    }

        //                    foreach( CswNbtMetaDataNodeTypeTab Tab in TabsToCheck )
        //                    {
        //                        ret = ret || Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, Tab.FirstTabVersionId, TabPermission ) );
        //                        if( TabPermission == NodeTypeTabPermission.View )
        //                        {
        //                            // Having 'Edit' grants 'View' automatically
        //                            ret = ret || Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, Tab.FirstTabVersionId, NodeTypeTabPermission.Edit ) );
        //                        }
        //                    }
        //                } // if checking tab permissions

        //                // Only Administrators can edit Roles
        //                if( ret &&
        //                    NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass &&
        //                    Permission != NodeTypePermission.View &&
        //                    !User.IsAdministrator() )
        //                {
        //                    ret = false;
        //                }

        //                if( null != NodeId && Int32.MinValue != NodeId.PrimaryKey )
        //                {
        //                    // case 2209 - Users can edit their own profile without permissions to the User nodetype
        //                    if( !ret &&
        //                        NodeId == User.UserId )
        //                    {
        //                        ret = true;
        //                    }

        //                    // Prevent users from deleting themselves or their own roles
        //                    if( ret &&
        //                        Permission == NodeTypePermission.Delete &&
        //                        ( ( NodeId == User.UserId ||
        //                            NodeId == User.RoleId ) ) )
        //                    {
        //                        ret = false;
        //                    }

        //                    // case 24510
        //                    if( NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerDispenseTransactionClass )
        //                    {
        //                        ret = Permission != NodeTypePermission.Delete;
        //                    }

        //                    // case 24510
        //                    if( NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerClass )
        //                    {
        //                        ret = ret && canContainer( NodeId, Permission, null, User );
        //                    }

        //                    if( MetaDataProp != null )
        //                    {

        //                        // You can't edit readonly properties
        //                        if( ret &&
        //                            Permission != NodeTypePermission.View &&
        //                            MetaDataProp.ReadOnly && false == MetaDataProp.AllowReadOnlyAdd ) /* Case 24514. Conditionally Permit edit on create. */
        //                        {
        //                            ret = false;
        //                        }

        //                        CswNbtMetaDataObjectClassProp OCP = MetaDataProp.getObjectClassProp();

        //                        // case 8218 - Certain properties on the user's preferences are not allowed to be edited
        //                        if( ret &&
        //                            NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass &&
        //                            !User.IsAdministrator() &&
        //                            OCP != null &&
        //                            ( OCP.PropName == CswNbtObjClassUser.PropertyName.Username ||
        //                              OCP.PropName == CswNbtObjClassUser.PropertyName.Role ||
        //                              OCP.PropName == CswNbtObjClassUser.PropertyName.FailedLoginCount ||
        //                              OCP.PropName == CswNbtObjClassUser.PropertyName.AccountLocked ) )
        //                        {
        //                            ret = false;
        //                        }

        //                        // Only admins can change other people's passwords
        //                        if( ret &&
        //                            NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass &&
        //                            !User.IsAdministrator() &&
        //                            User.UserId != NodeId &&
        //                            OCP != null &&
        //                            OCP.PropName == CswNbtObjClassUser.PropertyName.Password )
        //                        {
        //                            ret = false;
        //                        }

        //                    } // if( MetaDataProp != null )

        //                } // if( null != Node && null != Node.NodeId )

        //            } // if( Role != null )

        //        } // if-else( User is CswNbtSystemUser )

        //    } // if( User != null )

        //    return ret;

        //} // can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, CswPrimaryKey NodeId, CswNbtMetaDataNodeTypeProp Prop )

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( NodeTypePermission Permission, Int32 NodeTypeId, bool value )
        {
            set( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ), value );
        }

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, bool value )
        {
            set( Permission, NodeType, _CswNbtResources.CurrentNbtUser, value );
        }

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )
        {
            if( User != null )
            {
                set( Permission, NodeType, _getRole( User.RoleId ), value );
            }
        }

        /// <summary>
        /// Set NodeTypeTab permissions on a role
        /// </summary>
        public void set( NodeTypeTabPermission Permission, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                //Role.NodeTypePermissions.SetValue( Permission.ToString(), NodeType.FirstVersionNodeTypeId.ToString(), value );
                //Role.NodeTypePermissions.Save();
                CswNbtMetaDataNodeType NodeType = NodeTypeTab.getNodeType();
                if( value )
                {
                    Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, Permission ) );
                }
                else
                {
                    Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, Permission ) );
                }
                Role.postChanges( false );
            }

        }

        public void set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                //Role.NodeTypePermissions.SetValue( Permission.ToString(), NodeType.FirstVersionNodeTypeId.ToString(), value );
                //Role.NodeTypePermissions.Save();
                if( value )
                    Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                else
                    Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                Role.postChanges( false );
            }

        } // set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser Role, bool value )

        /// <summary>
        /// Sets a set of permissions for the given nodetype for the user
        /// </summary>
        public void set( NodeTypeTabPermission[] Permissions, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                foreach( NodeTypeTabPermission Permission in Permissions )
                {
                    set( Permission, NodeTypeTab, Role, value );
                }
            }
        }

        /// <summary>
        /// Sets a set of permissions for the given nodetype for the user
        /// </summary>
        public void set( NodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )
        {
            if( User != null )
            {
                set( Permissions, NodeType, _getRole( User.RoleId ), value );
            }
        }

        /// <summary>
        /// Sets a set of permissions for the given nodetype for the user
        /// </summary>
        public void set( NodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                foreach( NodeTypePermission Permission in Permissions )
                {
                    //Role.NodeTypePermissions.SetValue( Permission.ToString(), NodeType.NodeTypeId.ToString(), value );
                    if( value )
                        Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                    else
                        Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                }
                //Role.NodeTypePermissions.Save();
                Role.postChanges( false );
            }
        } // set( NodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )

        #endregion NodeTypes

        #region Actions

        /// <summary>
        /// Returns true if the current user has the appropriate permissions for the Action
        /// </summary>
        public bool can( Int32 ActionId )
        {
            return can( _CswNbtResources.Actions[ActionId] );
        }

        /// <summary>
        /// Returns true if the current user has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtActionName ActionName )
        {
            return can( _CswNbtResources.Actions[ActionName] );
        }

        /// <summary>
        /// Returns true if the current user has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtAction Action )
        {
            return can( Action, _CswNbtResources.CurrentNbtUser );
        }

        /// <summary>
        /// Returns true if the user has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtActionName ActionName, ICswNbtUser User )
        {
            return can( _CswNbtResources.Actions[ActionName], User );
        }

        /// <summary>
        /// Returns true if the user has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtAction Action, ICswNbtUser User )
        {
            bool ret = false;
            if( User is CswNbtSystemUser || User.Username == CswNbtObjClassUser.ChemSWAdminUsername )
            {
                ret = true;
            }
            else if( User != null )
            {
                ret = can( Action, _getRole( User.RoleId ) );
            }
            return ret;
        } // can( CswNbtAction Action, ICswNbtUser User )

        /// <summary>
        /// Returns true if the role has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtActionName ActionName, CswNbtObjClassRole Role )
        {
            return can( _CswNbtResources.Actions[ActionName], Role );
        }

        /// <summary>
        /// Returns true if the role has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtAction Action, CswNbtObjClassRole Role )
        {
            bool ret = false;
            if( Role != null )
            {
                ret = Role.ActionPermissions.CheckValue( CswNbtObjClassRole.MakeActionPermissionValue( Action ) );
            }
            return ret;
        } // can( CswNbtAction Action, CswNbtObjClassRole Role )

        /// <summary>
        /// Sets access for the given Action for the user
        /// </summary>
        public void set( Int32 ActionId, bool value )
        {
            set( _CswNbtResources.Actions[ActionId], value );
        }

        /// <summary>
        /// Sets a permission for the given Action for the user
        /// </summary>
        public void set( CswNbtAction Action, bool value )
        {
            set( Action, _CswNbtResources.CurrentNbtUser, value );
        }

        /// <summary>
        /// Sets a permission for the given Action for the user
        /// </summary>
        public void set( CswNbtActionName ActionName, bool value )
        {
            set( _CswNbtResources.Actions[ActionName], _CswNbtResources.CurrentNbtUser, value );
        }

        /// <summary>
        /// Sets a permission for the given Action for the user
        /// </summary>
        public void set( CswNbtActionName ActionName, ICswNbtUser User, bool value )
        {
            set( _CswNbtResources.Actions[ActionName], User, value );
        }

        /// <summary>
        /// Sets a permission for the given Action for the user
        /// </summary>
        public void set( CswNbtAction Action, ICswNbtUser User, bool value )
        {
            if( User != null )
            {
                set( Action, _getRole( User.RoleId ), value );
            }
        }

        public void set( CswNbtActionName ActionName, CswNbtObjClassRole Role, bool value )
        {
            set( _CswNbtResources.Actions[ActionName], Role, value );
        }

        public void set( CswNbtAction Action, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                //Role.ActionPermissions.SetValue( CswNbtAction.PermissionXValue.ToString(), CswNbtAction.ActionNameEnumToString( ActionName ), value );
                //Role.ActionPermissions.Save();
                if( value )
                    Role.ActionPermissions.AddValue( CswNbtObjClassRole.MakeActionPermissionValue( Action ) );
                else
                    Role.ActionPermissions.RemoveValue( CswNbtObjClassRole.MakeActionPermissionValue( Action ) );
                Role.postChanges( false );
            }
        } // set( CswNbtActionName ActionName, ICswNbtUser User, bool value )

        #endregion Actions

        #region Specialty



        /// <summary>
        /// Check container permissions.  Provide one of Permission or Action.
        /// </summary>
        public bool canContainer( CswPrimaryKey ContainerNodeId, NodeTypePermission Permission, CswNbtAction Action )
        {
            return canContainer( ContainerNodeId, Permission, Action, _CswNbtResources.CurrentNbtUser );
        }

        /// <summary>
        /// Check container permissions.  Provide one of Permission or Action.
        /// </summary>
        public bool canContainer( CswPrimaryKey ContainerNodeId, NodeTypePermission Permission, CswNbtAction Action, ICswNbtUser User )
        {
            bool ret = true;
            if( false == ( User is CswNbtSystemUser ) &&
                null != ContainerNodeId &&
                Int32.MinValue != ContainerNodeId.PrimaryKey )
            {
                // Special container permissions, based on Inventory Group                

                // We find the matching InventoryGroupPermission based on:
                //   the Container's Location's Inventory Group
                //   the User's WorkUnit
                //   the User's Role
                // We allow or deny permission to perform the action using the appropriate Logical

                ret = false;

                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClass InvGrpOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
                CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InventoryGroupPermissionClass );

                CswNbtMetaDataObjectClassProp ContainerLocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataObjectClassProp LocationInvGrpOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                CswNbtMetaDataObjectClassProp PermInvGrpOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.InventoryGroup );
                CswNbtMetaDataObjectClassProp PermRoleOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Role );
                CswNbtMetaDataObjectClassProp PermWorkUnitOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );

                CswNbtView InvGrpPermView = new CswNbtView( _CswNbtResources );
                InvGrpPermView.ViewName = "CswNbtPermit_InventoryGroupPermCheck";
                CswNbtViewRelationship ContainerVR = InvGrpPermView.AddViewRelationship( ContainerOC, false );
                CswNbtViewRelationship LocationVR = InvGrpPermView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.First, ContainerLocationOCP, false );
                CswNbtViewRelationship InvGrpVR = InvGrpPermView.AddViewRelationship( LocationVR, NbtViewPropOwnerType.First, LocationInvGrpOCP, false );
                CswNbtViewRelationship InvGrpPermVR = InvGrpPermView.AddViewRelationship( InvGrpVR, NbtViewPropOwnerType.Second, PermInvGrpOCP, false );

                // filter to container id
                ContainerVR.NodeIdsToFilterIn.Add( ContainerNodeId );
                // filter to role and workunit
                InvGrpPermView.AddViewPropertyAndFilter( InvGrpPermVR, PermRoleOCP, User.RoleId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );

                if( null != User.WorkUnitId )
                {
                    InvGrpPermView.AddViewPropertyAndFilter( InvGrpPermVR, PermWorkUnitOCP, User.WorkUnitId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
                }
                ICswNbtTree InvGrpPermTree = _CswNbtResources.Trees.getTreeFromView( InvGrpPermView, false, true, false );

                if( InvGrpPermTree.getChildNodeCount() > 0 )
                {
                    InvGrpPermTree.goToNthChild( 0 ); // container
                    if( InvGrpPermTree.getChildNodeCount() > 0 )
                    {
                        InvGrpPermTree.goToNthChild( 0 ); // location
                        if( InvGrpPermTree.getChildNodeCount() > 0 )
                        {
                            InvGrpPermTree.goToNthChild( 0 ); // inventory group
                            if( InvGrpPermTree.getChildNodeCount() > 0 )
                            {
                                if( null != User.WorkUnitId )
                                {
                                    // Location has an inventory group, but user has no workunit
                                    ret = false;
                                }
                                InvGrpPermTree.goToNthChild( 0 ); // inventory group permission
                                CswNbtNode PermNode = InvGrpPermTree.getNodeForCurrentPosition();
                                CswNbtObjClassInventoryGroupPermission PermNodeAsPerm = PermNode;
                                if( Action != null )
                                {
                                    if( ( Action.Name == CswNbtActionName.DispenseContainer && PermNodeAsPerm.Dispense.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.DisposeContainer && PermNodeAsPerm.Dispose.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.UndisposeContainer && PermNodeAsPerm.Undispose.Checked == Tristate.True ) ||
                                        ( Action.Name == CswNbtActionName.Submit_Request && PermNodeAsPerm.Request.Checked == Tristate.True ) )
                                    {
                                        ret = true;
                                    }
                                    else if( Action.Name == CswNbtActionName.Receiving )
                                    {
                                        foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOC.getLatestVersionNodeTypes() )
                                        {
                                            ret = canNodeType( NodeTypePermission.Create, ContainerNt );
                                            if( ret )
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if( ( Permission == NodeTypePermission.View && PermNodeAsPerm.View.Checked == Tristate.True ) ||
                                        ( Permission == NodeTypePermission.Edit && PermNodeAsPerm.Edit.Checked == Tristate.True ) ||
                                        ( Permission == NodeTypePermission.Create && PermNodeAsPerm.Edit.Checked == Tristate.True ) ||
                                        ( Permission == NodeTypePermission.Delete && PermNodeAsPerm.Edit.Checked == Tristate.True ) )
                                    {
                                        ret = true;
                                    }
                                }
                            } // if( InvGrpPermTree.getChildNodeCount() > 0 ) inventory group permission
                        } // if( InvGrpPermTree.getChildNodeCount() > 0 ) inventory group
                        else
                        {
                            // location has no inventory group, no permissions to enforce
                            ret = true;
                        }
                    } // if( InvGrpPermTree.getChildNodeCount() > 0 ) location
                    else
                    {
                        // container has no location, no permissions to enforce
                        ret = true;
                    }
                } // if( InvGrpPermTree.getChildNodeCount() > 0 ) container
            } // if( Node.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerClass )
            return ret;
        } // canContainer

        #endregion Specialty

    } // class CswNbtPermit
} // namespace ChemSW.Nbt.Security
