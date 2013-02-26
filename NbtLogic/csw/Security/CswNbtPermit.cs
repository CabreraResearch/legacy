using System;
using System.Collections.Generic;
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

        private Dictionary<CswNbtPermitInfoKey, CswNbtPermitInfo> _PermitInfoItems = new Dictionary<CswNbtPermitInfoKey, CswNbtPermitInfo>();

        public class CswNbtPermitInfoKey : IEquatable<CswNbtPermitInfoKey>, IComparable<CswNbtPermitInfoKey>
        {
            private readonly Int32 HashMultiplier = 1;
            public CswNbtPermitInfoKey( CswNbtObjClassRole CswNbtObjClassRole, CswNbtMetaDataNodeType NodeTypeIn )
            {
                Role = CswNbtObjClassRole;
                NodeType = NodeTypeIn;
                if( null != Role )
                {
                    HashMultiplier += Role.GetHashCode();
                }
                if( null != NodeType )
                {
                    HashMultiplier += NodeType.GetHashCode();
                }
            }

            private char _Delimiter = '_';


            public CswNbtObjClassRole Role = null;
            public CswNbtMetaDataNodeType NodeType = null;

            public bool Equals( CswNbtPermitInfoKey other )
            {

                bool ReturnVal = false;

                if( ( null != Role ) && ( null != NodeType ) && ( null != other.Role ) && ( null != other.NodeType ) )
                {
                    if( ( other.Role.NodeId == Role.NodeId ) && ( other.NodeType.FirstVersionNodeTypeId == NodeType.FirstVersionNodeTypeId ) )
                    {
                        ReturnVal = true;
                    }
                }
                else
                {
                    ReturnVal = true;
                }//if-else all values are null

                return ( ReturnVal );

            }//equals

            public int CompareTo( CswNbtPermitInfoKey other )
            {
                string ThisSortKey = this.ToString();
                string OtherSortKey = other.ToString();

                return ( String.Compare( ThisSortKey, OtherSortKey ) );
            }

            public override string ToString()
            {
                CswDelimitedString DelimStr = new CswDelimitedString( _Delimiter );
                DelimStr.Add( Role.Name.Text );
                DelimStr.Add( NodeType.NodeTypeName );
                return DelimStr.ToString();
            }

            public override int GetHashCode()
            {
                return 17 * HashMultiplier;
            }
        }//CswNbtPermitInfoKey

        private class CswNbtPermitInfo
        {
            private CswNbtResources _CswNbtResources = null;
            private ICswNbtUser _CswNbtUser = null;
            private CswNbtObjClassRole _CswNbtObjClassRole = null;

            public CswNbtMetaDataNodeType NodeType = null;
            public CswNbtMetaDataNodeTypeProp PropType = null;


            public CswNbtPermitInfo( CswNbtResources CswNbtResources, ICswNbtUser CswNbtUser, CswNbtObjClassRole CswNbtObjClassRole, CswNbtMetaDataNodeType NodeTypeIn, NodeTypePermission nodeTypePermissionIn, CswPrimaryKey CswPrimaryKey, CswNbtMetaDataNodeTypeProp PropTypeIn )
            {
                PropType = PropTypeIn;
                _NodePrimeKey = CswPrimaryKey;
                NodeType = NodeTypeIn;
                _CswNbtResources = CswNbtResources;
                _CswNbtUser = CswNbtUser;
                _CswNbtObjClassRole = CswNbtObjClassRole;
                NodeTypePermission = nodeTypePermissionIn;
            }//ctor


            public bool shouldPermissionCheckProceed()
            {
                return ( ( null != NodeType ) && ( null != Role ) );
            }



            private NodeTypePermission _NodeTypePermission = NodeTypePermission.View;
            public NodeTypePermission NodeTypePermission
            {
                get
                {
                    return ( _NodeTypePermission );
                }

                set
                {
                    _NodeTypePermission = value;
                    _NodeTypeTabPermissionWasSet = false;
                }

            }

            private bool _NodeTypeTabPermissionWasSet = false;
            private NodeTypeTabPermission _NodeTypeTabPermission = NodeTypeTabPermission.View;
            public NodeTypeTabPermission NodeTypeTabPermission
            {
                get
                {
                    if( false == _NodeTypeTabPermissionWasSet )
                    {
                        _NodeTypeTabPermissionWasSet = true;
                        switch( NodeTypePermission )
                        {
                            case NodeTypePermission.Create:
                                _NodeTypeTabPermission = CswNbtPermit.NodeTypeTabPermission.Edit;
                                break;

                            case NodeTypePermission.View:
                                _NodeTypeTabPermission = CswNbtPermit.NodeTypeTabPermission.View;
                                break;

                            case NodeTypePermission.Edit:
                                _NodeTypeTabPermission = CswNbtPermit.NodeTypeTabPermission.Edit;
                                break;

                            case NodeTypePermission.Delete:
                                _NodeTypeTabPermission = CswNbtPermit.NodeTypeTabPermission.Edit;
                                break;

                        }//switch()

                    }//if we haven't set the tab permish yet

                    return ( _NodeTypeTabPermission );

                }//get

            }//NodeTypeTabPermission


            public ICswNbtUser User
            {
                get
                {
                    return ( _CswNbtUser );

                }//get

            }//User


            private CswPrimaryKey _NodePrimeKey = null;
            public CswPrimaryKey NodePrimeKey
            {
                set { _NodePrimeKey = value; }
                get { return ( _NodePrimeKey ); }
            }

            public CswNbtObjClassRole Role
            {
                get
                {
                    //                    _CswNbtObjClassRole = _CswNbtResources.Nodes[User.RoleId];


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


            public bool NoExceptionCases
            {
                get
                {

                    bool ReturnVal = true;
                    // case 24510
                    NbtObjectClass ObjectClass = NodeType.getObjectClass().ObjectClass;
                    if( ObjectClass == NbtObjectClass.ContainerDispenseTransactionClass )
                    {
                        ReturnVal = NodeTypePermission != NodeTypePermission.Delete;
                    }

                    //case 28158: Another way to cope with this would for the CswNbtObjClass abstract 
                    //class to expose an interface along the lines of allowNodeType(). 
                    //If we come across additonal exception cases such as this, we can 
                    //apply such a strategy.
                    if( ( null != PropType ) && ( ObjectClass == NbtObjectClass.UserClass ) )
                    {

                        CswNbtObjClassUser CswNbtObjClassUser = _CswNbtResources.Nodes[_CswNbtUser.UserId];
                        if( null != CswNbtObjClassUser )
                        {
                            if( PropType.getObjectClassPropName() == CswNbtObjClassUser.PropertyName.Password &&
                                false == CswNbtObjClassUser.IsPasswordReadyOnly )
                            {
                                ReturnVal = false;
                            }
                        }

                    }//get() 

                    return ( ReturnVal );

                }//get

            }//NoExceptionCases

        }//CswNbtPermitInfo()


        private CswNbtPermitInfo _CswNbtPermitInfo = null;
        private void _initPermissionInfo( CswNbtObjClassRole CswNbtObjClassRole, ICswNbtUser CswNbtUser, CswNbtMetaDataNodeType NodeType, NodeTypePermission Permission, CswPrimaryKey CswPrimaryKey = null, CswNbtMetaDataNodeTypeProp PropType = null )
        {

            if( null == CswNbtObjClassRole )
            {
                CswPrimaryKey RoleId = null;
                if( null != CswNbtUser )
                {
                    RoleId = CswNbtUser.RoleId;
                }
                else
                {

                    if( null != _CswNbtResources.CurrentNbtUser )
                    {
                        CswNbtUser = _CswNbtResources.CurrentNbtUser;
                        RoleId = CswNbtUser.RoleId;
                    }

                }//if the user we got is null

                if( null != RoleId )
                {
                    CswNbtObjClassRole = _getRole( RoleId );
                }//if we were able to get a roleid

            }//if the role we got is null

            if( null != CswNbtObjClassRole )
            {

                CswNbtPermitInfoKey CswNbtPermitInfoKey = new CswNbtPermitInfoKey( CswNbtObjClassRole, NodeType );
                if( _PermitInfoItems.ContainsKey( CswNbtPermitInfoKey ) )
                {
                    _CswNbtPermitInfo = _PermitInfoItems[CswNbtPermitInfoKey];

                    //must reset these per-request because they change per request for the same role and nodetype (e.g., see allowAny() )
                    if( null != CswPrimaryKey )
                    {
                        _CswNbtPermitInfo.NodePrimeKey = CswPrimaryKey;
                    }

                    if( null != PropType )
                    {
                        _CswNbtPermitInfo.PropType = PropType;
                    }

                    _CswNbtPermitInfo.NodeTypePermission = Permission;
                }
                else
                {
                    _CswNbtPermitInfo = new CswNbtPermitInfo( _CswNbtResources, CswNbtUser, CswNbtObjClassRole, NodeType, Permission, CswPrimaryKey, PropType );
                    _PermitInfoItems.Add( CswNbtPermitInfoKey, _CswNbtPermitInfo );

                }
            }
            else //the permit info in this case is not catalogued, and permit info won't allow any ops to proceed
            {
                _CswNbtPermitInfo = new CswNbtPermitInfo( _CswNbtResources, CswNbtUser, CswNbtObjClassRole, NodeType, Permission, CswPrimaryKey, PropType );
            }//if we were able to retrieve a role

        }//_initPermissionInfo()


        /// <summary>
        /// Type of NodeTypePermission on NodeTypes
        /// </summary>
        public enum NodeTypePermission
        {
            /// <summary>
            /// NodeTypePermission to view nodes of this type
            /// </summary>
            View,
            /// <summary>
            /// NodeTypePermission to create new nodes of this type
            /// </summary>
            Create,
            /// <summary>
            /// NodeTypePermission to delete nodes of this type
            /// </summary>
            Delete,
            /// <summary>
            /// NodeTypePermission to edit property values of nodes of this type
            /// </summary>
            Edit
        }

        /// <summary>
        /// Type of NodeTypePermission on NodeTypeTabs
        /// </summary>
        public enum NodeTypeTabPermission
        {
            /// <summary>
            /// NodeTypePermission to view the tab
            /// </summary>
            View,
            /// <summary>
            /// NodeTypePermission to edit property values on this tab
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
            return _CswNbtResources.Nodes[RoleId];
        }

        #region NodeTypes

        public bool canNodeType( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null )
        {
            _initPermissionInfo( null, User, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _CanNodeTypeImpl();
                    }
                    else
                    {
                        ret = true;
                    }

                }//if pre-reqs are satisifed
            }

            return ( ret );
        }

        /// <summary>
        /// Does this User have this NodeTypePermission on this nodetype?
        /// </summary>
        /// <param name="Permission"></param>
        /// <param name="NodeType"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public bool canNodeType( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role )
        {
            _initPermissionInfo( Role, null, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _CanNodeTypeImpl();
                    }
                    else
                    {
                        ret = true;

                    }

                }//if pre-reqs are satisifed
            }

            return ( ret );

        }//canNodeType() 



        private bool _CanNodeTypeImpl()
        {
            bool ret = _CswNbtPermitInfo.NoExceptionCases;



            // Base case: does the Role have this nodetype permission
            string PermissionValueToCheck =
                CswNbtObjClassRole.MakeNodeTypePermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId,
                                                               _CswNbtPermitInfo.NodeTypePermission );
            ret = ret && _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( PermissionValueToCheck );

            if( ( false == ret ) && ( _CswNbtPermitInfo.NodeTypePermission == NodeTypePermission.View ) )
            {
                // Having 'Edit' grants 'View' automaticall y
                ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue(
                          CswNbtObjClassRole.MakeNodeTypePermissionValue(
                              _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypePermission.Edit ) );

            }//if we denied view permission

            return ( ret );

        } // _CanNodeTypeImpl()


        public bool canTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab NodeTypeTab, ICswNbtUser User = null )
        {
            _initPermissionInfo( null, User, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _canTabImpl( NodeTypeTab );
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }

            return ( ret );

        }//catTab() 


        public bool canTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role )
        {
            _initPermissionInfo( Role, null, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _canTabImpl( NodeTypeTab );
                    }
                    else
                    {
                        ret = true;
                    }

                }//if can proceed

            }//if ubser user

            return ( ret );

        }//catTab() 



        private bool _canTabImpl( CswNbtMetaDataNodeTypeTab NodeTypeTab )
        {

            bool ret = canNodeType( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );

            if( false == ret && null != NodeTypeTab )
            {
                ret = ret || canNodeType( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );
                if( _CswNbtPermitInfo.NodeTypePermission == NodeTypePermission.View ||
                    _CswNbtPermitInfo.NodeTypePermission == NodeTypePermission.Edit )
                {
                    //NodeTypeTabPermission TabPermission = (NodeTypeTabPermission) Enum.Parse( typeof( NodeTypeTabPermission ), _CswNbtPermitInfo.NodeTypePermission.ToString() );
                    ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, _CswNbtPermitInfo.NodeTypeTabPermission ) );

                    if( false == ret && _CswNbtPermitInfo.NodeTypeTabPermission == NodeTypeTabPermission.View )
                    {
                        // Having 'Edit' grants 'View' automatically
                        ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, NodeTypeTabPermission.Edit ) );
                    }

                }//if permission is view or edit

            }

            return ( ret );

        }//_canTablImpl()


        public bool canAnyTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null )
        {
            _initPermissionInfo( null, User, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {
                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _canAnyTabImpl();
                    }
                    else
                    {
                        ret = true;
                    }
                }//if pre-reqs are in order
            }

            return ( ret );

        }//canAnyTab() 


        public bool canAnyTab( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role )
        {
            _initPermissionInfo( Role, null, NodeType, Permission );

            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {

                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {

                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {

                        ret = _canAnyTabImpl();
                    }
                    else
                    {
                        ret = true;
                    }

                }//if pre-reqs are in order
            }

            return ( ret );

        }//canAnyTab() 


        private bool _canAnyTabImpl()
        {
            bool ret = false;


            //NodeTypeTabPermission TabPermission = (NodeTypeTabPermission) Enum.Parse( typeof( NodeTypeTabPermission ), _CswNbtPermitInfo.NodeTypePermission.ToString() );
            foreach( CswNbtMetaDataNodeTypeTab CurrentTab in _CswNbtPermitInfo.NodeType.getNodeTypeTabs() )
            {
                ret = ret ||
                      _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue(
                          CswNbtObjClassRole.MakeNodeTypeTabPermissionValue(
                              _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId,
                              _CswNbtPermitInfo.NodeTypeTabPermission ) );

                if( _CswNbtPermitInfo.NodeTypeTabPermission == NodeTypeTabPermission.View )
                {
                    // Having 'Edit' grants 'View' automatically
                    ret = ret ||
                          _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue(
                              CswNbtObjClassRole.MakeNodeTypeTabPermissionValue(
                                  _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId,
                                  NodeTypeTabPermission.Edit ) );
                }

            } //iterate tabs


            return ( ret );

        }//_canAnyTab()


        public bool isPropWritable( NodeTypePermission Permission, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtNodePropWrapper NodePropWrapper = null, ICswNbtUser User = null )
        {
            bool ret = ( null != MetaDataProp );

            if( ret )
            {
                _initPermissionInfo( null, User, MetaDataProp.getNodeType(), Permission, ( ( null != NodePropWrapper ) ? NodePropWrapper.NodeId : null ), MetaDataProp );

                ret = _CswNbtPermitInfo.IsUberUser;
                if( ( false == ret ) || ( true == MetaDataProp.ServerManaged ) ) //we already known that ( null != MetaDataProp ) 
                {
                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {

                        if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                        {
                            ret = _isPropWritableImpl( MetaDataTab, MetaDataProp, NodePropWrapper );
                        }
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }//if-else we have metadata prop

            return ( ret );

        }//isPropWritable

        public bool isPropWritable( NodeTypePermission Permission, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtNodePropWrapper NodePropWrapper, CswNbtObjClassRole Role )
        {
            bool ret = ( null != MetaDataProp );

            if( ret )
            {
                _initPermissionInfo( Role, null, MetaDataProp.getNodeType(), Permission, ( ( null != NodePropWrapper ) ? NodePropWrapper.NodeId : null ), MetaDataProp );

                ret = _CswNbtPermitInfo.IsUberUser;
                if( false == ret )
                {
                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {
                        if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                        {
                            ret = _isPropWritableImpl( MetaDataTab, MetaDataProp, NodePropWrapper );
                        }
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }//if-else we have metadata prop

            return ( ret );

        }//isPropWritable

        private bool _isPropWritableImpl( CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtNodePropWrapper NodePropWrapper )
        {
            bool ret = _CswNbtPermitInfo.NoExceptionCases;

            ret = ret || ( null == MetaDataTab || canTab( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.NodeType, MetaDataTab ) );

            // Anyone but an admin cannot write to read-only props
            // Even admins cannot write to servermanaged props
            ret = ret &&
                  ( _CswNbtPermitInfo.NodeTypePermission != NodeTypePermission.View ) &&
                  ( false == MetaDataProp.ServerManaged ) &&
                  (
                      ( ( null != _CswNbtPermitInfo.User ) && ( _CswNbtPermitInfo.User.IsAdministrator() ) ) ||
                      (
                          ( false == MetaDataProp.ReadOnly ) && ( ( null == NodePropWrapper ) || ( false == NodePropWrapper.ReadOnly ) )
                      )
                  );

            //if( ret &&
            //        (
            //            ( _CswNbtPermitInfo.NodeTypePermission != NodeTypePermission.View ) &&
            //            ( false == MetaDataProp.ServerManaged ) &&
            //            (  MetaDataProp.ReadOnly || ( ( null != NodePropWrapper ) && NodePropWrapper.ReadOnly ) ) &&
            //            ( false == MetaDataProp.AllowReadOnlyAdd )
            //        )
            //    )/* Case 24514. Conditionally Permit edit on create. */
            //{
            //    ret = false;
            //}

            return ( ret );

        }//_isPropWritableImpl()

        public bool isNodeWritable( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswPrimaryKey NodeId, ICswNbtUser User = null )
        {

            bool ret = true;

            _initPermissionInfo( null, User, NodeType, Permission, NodeId );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {
                if( true == _CswNbtPermitInfo.NoExceptionCases )
                {

                    if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                    {
                        ret = _isNodeWritableImpl();

                    }//if pre-reqs are satisifed

                }
            }

            return ( ret );

        }//isNodeWritable() 


        public bool isNodeWritable( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswPrimaryKey NodeId, CswNbtObjClassRole Role )
        {
            bool ret = true;

            _initPermissionInfo( Role, null, NodeType, Permission, NodeId );

            if( false == _CswNbtPermitInfo.IsUberUser )
            {
                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {
                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {
                        ret = _isNodeWritableImpl();
                    }
                }//if pre-reqs are satisifed
            }

            return ( ret );
        }//isNodeWritable() 


        private bool _isNodeWritableImpl()
        {
            bool ret = true;

            //the case in which it is a problem that we check nodetype permissions: 
            //  node type level permissions are off, but a tab has edit permission. 
            //  this is called from CswNbtSdTabsAndProps::makePropJson(). So here, canNodeType() 
            // gives us false, which means that the readOnly() status below does not get checked. 
            // Here's a case where it's better to balkanize these methods and let the caller 
            // decide how to piece them together. 
            //ret = canNodeType( NodeTypePermission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );

            if( ret && ( null != _CswNbtPermitInfo.NodePrimeKey && Int32.MinValue != _CswNbtPermitInfo.NodePrimeKey.PrimaryKey ) )
            {
                // Prevent users from deleting themselves or their own roles
                if( ret &&
                    _CswNbtPermitInfo.NodeTypePermission == NodeTypePermission.Delete &&
                    ( ( _CswNbtPermitInfo.NodePrimeKey == _CswNbtPermitInfo.User.UserId ||
                        _CswNbtPermitInfo.NodePrimeKey == _CswNbtPermitInfo.User.RoleId ) ) )
                {
                    ret = false;
                }

                // case 24510
                CswNbtNode Node = _CswNbtResources.Nodes[_CswNbtPermitInfo.NodePrimeKey];
                if( null != Node )
                {
                    if( _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == NbtObjectClass.ContainerClass )
                    {

                        CswNbtObjClassContainer CswNbtObjClassContainer = Node;

                        ret = ret && CswNbtObjClassContainer.canContainer( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.User );
                    }
                    if( _CswNbtPermitInfo.NodeTypePermission == NodeTypePermission.Edit )
                    {
                        ret = ret && ( _CswNbtPermitInfo.User.IsAdministrator() || false == Node.ReadOnly );
                    }
                }
            }//if NodeId is not null

            return ( ret );
        }//_isNodeWritableImpl()

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
                //Role.NodeTypePermissions.SetValue( NodeTypePermission.ToString(), NodeType.FirstVersionNodeTypeId.ToString(), value );
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
                //Role.NodeTypePermissions.SetValue( NodeTypePermission.ToString(), NodeType.FirstVersionNodeTypeId.ToString(), value );
                //Role.NodeTypePermissions.Save();
                if( value )
                    Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                else
                    Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType.FirstVersionNodeTypeId, Permission ) );
                Role.postChanges( false );
            }

        } // set( NodeTypePermission NodeTypePermission, CswNbtMetaDataNodeType NodeType, ICswNbtUser Role, bool value )

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
                    //Role.NodeTypePermissions.SetValue( NodeTypePermission.ToString(), NodeType.NodeTypeId.ToString(), value );
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





        #endregion Specialty

    } // class CswNbtPermit
} // namespace ChemSW.Nbt.Security
