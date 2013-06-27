using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.Security
{
    /// <summary>
    /// Security class responsible for enforcing all permissions
    /// </summary>
    public class CswNbtPermit
    {

        private Dictionary<CswNbtPermitInfoKey, CswNbtPermitInfo> _PermitInfoItems = new Dictionary<CswNbtPermitInfoKey, CswNbtPermitInfo>();

        public class CswNbtPermitInfoKey: IEquatable<CswNbtPermitInfoKey>, IComparable<CswNbtPermitInfoKey>
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


            public CswNbtPermitInfo( CswNbtResources CswNbtResources, ICswNbtUser CswNbtUser, CswNbtObjClassRole CswNbtObjClassRole, CswNbtMetaDataNodeType NodeTypeIn, CswEnumNbtNodeTypePermission nodeTypePermissionIn, CswPrimaryKey CswPrimaryKey, CswNbtMetaDataNodeTypeProp PropTypeIn )
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



            private CswEnumNbtNodeTypePermission _NodeTypePermission = CswEnumNbtNodeTypePermission.View;
            public CswEnumNbtNodeTypePermission NodeTypePermission
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
            private CswEnumNbtNodeTypeTabPermission _NodeTypeTabPermission = CswEnumNbtNodeTypeTabPermission.View;
            public CswEnumNbtNodeTypeTabPermission NodeTypeTabPermission
            {
                get
                {
                    if( false == _NodeTypeTabPermissionWasSet )
                    {
                        _NodeTypeTabPermissionWasSet = true;
                        switch( NodeTypePermission )
                        {
                            case CswEnumNbtNodeTypePermission.Create:
                                _NodeTypeTabPermission = CswEnumNbtNodeTypeTabPermission.Edit;
                                break;

                            case CswEnumNbtNodeTypePermission.View:
                                _NodeTypeTabPermission = CswEnumNbtNodeTypeTabPermission.View;
                                break;

                            case CswEnumNbtNodeTypePermission.Edit:
                                _NodeTypeTabPermission = CswEnumNbtNodeTypeTabPermission.Edit;
                                break;

                            case CswEnumNbtNodeTypePermission.Delete:
                                _NodeTypeTabPermission = CswEnumNbtNodeTypeTabPermission.Edit;
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

            private CswNbtNode _Node = null;
            public CswNbtNode Node
            {
                get
                {
                    if( null == _Node && CswTools.IsPrimaryKey( NodePrimeKey ) )
                    {
                        _Node = _CswNbtResources.Nodes[NodePrimeKey];
                    }
                    return _Node;
                }
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
                    CswEnumNbtObjectClass ObjectClass = NodeType.getObjectClass().ObjectClass;
                    if( ObjectClass == CswEnumNbtObjectClass.ContainerDispenseTransactionClass )
                    {
                        ReturnVal = NodeTypePermission != CswEnumNbtNodeTypePermission.Delete;
                    }

                    //case 28158: Another way to cope with this would for the CswNbtObjClass abstract 
                    //class to expose an interface along the lines of allowNodeType(). 
                    //If we come across additonal exception cases such as this, we can 
                    //apply such a strategy.
                    if( ( null != PropType ) && ( ObjectClass == CswEnumNbtObjectClass.UserClass ) )
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

            public bool IsUserEditingItsOwnUserNode
            {
                get
                {
                    bool Ret = ( NodePrimeKey == _CswNbtResources.CurrentNbtUser.UserId );
                    return Ret;
                }
            }

        }//CswNbtPermitInfo()


        private CswNbtPermitInfo _CswNbtPermitInfo = null;
        private void _initPermissionInfo( CswNbtObjClassRole CswNbtObjClassRole, ICswNbtUser CswNbtUser, CswNbtMetaDataNodeType NodeType, CswEnumNbtNodeTypePermission Permission, CswPrimaryKey CswPrimaryKey = null, CswNbtMetaDataNodeTypeProp PropType = null )
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

        /// <summary>
        /// Does this User have this NodeTypePermission on this nodetype?
        /// </summary>
        public bool canNodeType( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null )
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
        public bool canNodeType( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role )
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

        /// <summary>
        /// Private logic behind canNodeType
        /// </summary>
        private bool _CanNodeTypeImpl()
        {
            bool ret = _CswNbtPermitInfo.NoExceptionCases;



            // Base case: does the Role have this nodetype permission
            string PermissionValueToCheck =
                CswNbtObjClassRole.MakeNodeTypePermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId,
                                                               _CswNbtPermitInfo.NodeTypePermission );
            ret = ret && _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( PermissionValueToCheck );

            if( ( false == ret ) && ( _CswNbtPermitInfo.NodeTypePermission == CswEnumNbtNodeTypePermission.View ) )
            {
                // Having 'Edit' grants 'View' automaticall y
                ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue(
                          CswNbtObjClassRole.MakeNodeTypePermissionValue(
                              _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CswEnumNbtNodeTypePermission.Edit ) );

            }//if we denied view permission

            return ( ret );

        } // _CanNodeTypeImpl()

        /// <summary>
        /// Determines if the User has permission on this Tab (and only this Tab)
        /// </summary>
        public bool canTab( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab NodeTypeTab, ICswNbtUser User = null, CswPrimaryKey NodeId = null )
        {
            _initPermissionInfo( null, User, NodeType, Permission, NodeId );

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

        /// <summary>
        /// Determines if the Role has permission on this Tab (and only this Tab)
        /// </summary>
        public bool canTab( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role, CswPrimaryKey NodeId = null )
        {
            _initPermissionInfo( Role, null, NodeType, Permission, NodeId );

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

        /// <summary>
        /// Private logic behind canTab
        /// </summary>
        private bool _canTabImpl( CswNbtMetaDataNodeTypeTab NodeTypeTab )
        {

            bool ret = _CswNbtPermitInfo.IsUserEditingItsOwnUserNode || canNodeType( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );

            if( false == ret && null != NodeTypeTab )
            {
                ret = ret || canNodeType( _CswNbtPermitInfo.NodeTypePermission, _CswNbtPermitInfo.NodeType, _CswNbtPermitInfo.User );
                if( _CswNbtPermitInfo.NodeTypePermission == CswEnumNbtNodeTypePermission.View ||
                    _CswNbtPermitInfo.NodeTypePermission == CswEnumNbtNodeTypePermission.Edit )
                {
                    ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, _CswNbtPermitInfo.NodeTypeTabPermission ) );

                    if( false == ret && _CswNbtPermitInfo.NodeTypeTabPermission == CswEnumNbtNodeTypeTabPermission.View )
                    {
                        // Having 'Edit' grants 'View' automatically
                        ret = _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypeTabPermissionValue( _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, NodeTypeTab.FirstTabVersionId, CswEnumNbtNodeTypeTabPermission.Edit ) );
                    }

                }//if permission is view or edit

            }

            return ( ret );

        }//_canTablImpl()

        /// <summary>
        /// Determines if the User has permission on the NodeType or any Tab on the NodeType
        /// </summary>
        public bool canAnyTab( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User = null, CswPrimaryKey NodeId = null )
        {
            _initPermissionInfo( null, User, NodeType, Permission, NodeId );

            return _canAnyTabImpl();

        }//canAnyTab() 

        /// <summary>
        /// Determines if the Role has permission on the NodeType or any Tab on the NodeType
        /// </summary>
        public bool canAnyTab( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, CswPrimaryKey NodeId = null )
        {
            _initPermissionInfo( Role, null, NodeType, Permission, NodeId );

            return _canAnyTabImpl();

        }//canAnyTab() 

        /// <summary>
        /// Private logic behind canAnyTab
        /// </summary>
        private bool _canAnyTabImpl()
        {
            bool ret = _CswNbtPermitInfo.IsUberUser;
            if( false == ret )
            {
                if( _CswNbtPermitInfo.shouldPermissionCheckProceed() )
                {
                    if( true == _CswNbtPermitInfo.NoExceptionCases )
                    {
                        ret = _CanNodeTypeImpl();
                        if( false == ret )
                        {
                            foreach( CswNbtMetaDataNodeTypeTab CurrentTab in _CswNbtPermitInfo.NodeType.getNodeTypeTabs() )
                            {
                                string Permission = CswNbtObjClassRole.MakeNodeTypeTabPermissionValue(
                                    _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId,
                                    _CswNbtPermitInfo.NodeTypeTabPermission );
                                ret = ret ||
                                      _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( Permission );

                                if( _CswNbtPermitInfo.NodeTypeTabPermission == CswEnumNbtNodeTypeTabPermission.View )
                                {
                                    // Having 'Edit' grants 'View' automatically
                                    string EditPermission = CswNbtObjClassRole.MakeNodeTypeTabPermissionValue(
                                        _CswNbtPermitInfo.NodeType.FirstVersionNodeTypeId, CurrentTab.FirstTabVersionId,
                                        CswEnumNbtNodeTypeTabPermission.Edit );
                                    ret = ret ||
                                          _CswNbtPermitInfo.Role.NodeTypePermissions.CheckValue( EditPermission );
                                }
                            } //iterate tabs
                        }
                    }
                    else
                    {
                        ret = true;
                    }

                }//if pre-reqs are in order
            }
            return ( ret );

        }//_canAnyTab()

        /// <summary>
        /// Determines if the Property is editable
        /// </summary>
        public bool isPropWritable( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtNodePropWrapper NodePropWrapper = null, ICswNbtUser User = null )
        {
            bool ret = ( null != MetaDataProp );

            if( ret )
            {
                _initPermissionInfo( null, User, MetaDataProp.getNodeType(), Permission, ( ( null != NodePropWrapper ) ? NodePropWrapper.NodeId : null ), MetaDataProp );

                ret = _CswNbtPermitInfo.IsUberUser;
                if( ( false == ret ) ||
                    ( MetaDataProp.ServerManaged ) ||
                    ( ( MetaDataProp.ReadOnly ) || ( null != NodePropWrapper && NodePropWrapper.ReadOnly ) ) ) // case 29321
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

        /// <summary>
        /// Determines if the Property is editable
        /// </summary>
        public bool isPropWritable( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeTab MetaDataTab, CswNbtNodePropWrapper NodePropWrapper, CswNbtObjClassRole Role )
        {
            bool ret = ( null != MetaDataProp );

            if( ret )
            {
                _initPermissionInfo( Role, null, MetaDataProp.getNodeType(), Permission, ( ( null != NodePropWrapper ) ? NodePropWrapper.NodeId : null ), MetaDataProp );

                ret = _CswNbtPermitInfo.IsUberUser;
                if( ( false == ret ) ||
                    ( MetaDataProp.ServerManaged ) ||
                    ( ( MetaDataProp.ReadOnly ) || ( null != NodePropWrapper && NodePropWrapper.ReadOnly ) ) ) // case 29321
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

            ret = ret &&
                // We've requested an Editable state
                  ( _CswNbtPermitInfo.NodeTypePermission != CswEnumNbtNodeTypePermission.View ) &&
                // No one can write to servermanaged props
                  ( false == MetaDataProp.ServerManaged ) &&
                  (
                // Anyone but an admin cannot write to read-only props  
                // but see case 29095; this is now handled in CswNbtSdTabsAndProps
                //( ( null != _CswNbtPermitInfo.User ) && ( _CswNbtPermitInfo.User.IsAdministrator() ) ) ||
                //Buttons are always "writable"  
                      MetaDataProp.getFieldType().FieldType == CswEnumNbtFieldType.Button ||
                      (
                //This prop is not readonly OR
                          ( ( false == MetaDataProp.ReadOnly ) && ( ( null == NodePropWrapper ) || ( false == NodePropWrapper.ReadOnly ) ) ) ||
                //The prop is required AND readonly AND we're creating a new node
                //This was removed as part of Case 27984, and I think it was a mistake
                          ( MetaDataProp.IsRequired && _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add && null != MetaDataProp.getAddLayout() )
                      )
                  );

            return ( ret );

        }//_isPropWritableImpl()

        public bool isNodeWritable( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswPrimaryKey NodeId, ICswNbtUser User = null )
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


        public bool isNodeWritable( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswPrimaryKey NodeId, CswNbtObjClassRole Role )
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
                    _CswNbtPermitInfo.NodeTypePermission == CswEnumNbtNodeTypePermission.Delete &&
                    ( ( _CswNbtPermitInfo.NodePrimeKey == _CswNbtPermitInfo.User.UserId ||
                        _CswNbtPermitInfo.NodePrimeKey == _CswNbtPermitInfo.User.RoleId ) ) )
                {
                    ret = false;
                }

                // case 24510
                CswNbtNode Node = _CswNbtResources.Nodes[_CswNbtPermitInfo.NodePrimeKey];
                if( null != Node )
                {
                    //TODO - genericize target OC values
                    if( _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ContainerClass ||
                        _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ReportClass ||
                        _CswNbtPermitInfo.NodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.MailReportClass )
                    {
                        ret = ret && CswNbtPropertySetPermission.canNode( _CswNbtResources, _CswNbtPermitInfo.NodeTypePermission, ( (CswNbtObjClassContainer) Node ).getInventoryGroupId(), _CswNbtPermitInfo.User );
                    }
                    if( _CswNbtPermitInfo.NodeTypePermission == CswEnumNbtNodeTypePermission.Edit )
                    {
                        // see case 29095; this is now handled in CswNbtSdTabsAndProps
                        //ret = ret && ( _CswNbtPermitInfo.User.IsAdministrator() || false == Node.ReadOnly );
                        ret = ret && ( false == Node.ReadOnly );
                    }
                }
            }//if NodeId is not null

            return ( ret );
        }//_isNodeWritableImpl()

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( CswEnumNbtNodeTypePermission Permission, Int32 NodeTypeId, bool value )
        {
            set( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ), value );
        }

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, bool value )
        {
            set( Permission, NodeType, _CswNbtResources.CurrentNbtUser, value );
        }

        /// <summary>
        /// Sets a permission for the given nodetype for the user
        /// </summary>
        public void set( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )
        {
            if( User != null )
            {
                set( Permission, NodeType, _getRole( User.RoleId ), value );
            }
        }

        /// <summary>
        /// Set NodeTypeTab permissions on a role
        /// </summary>
        public void set( CswEnumNbtNodeTypeTabPermission Permission, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role, bool value )
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

        public void set( CswEnumNbtNodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, bool value )
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
        public void set( CswEnumNbtNodeTypeTabPermission[] Permissions, CswNbtMetaDataNodeTypeTab NodeTypeTab, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                foreach( CswEnumNbtNodeTypeTabPermission Permission in Permissions )
                {
                    set( Permission, NodeTypeTab, Role, value );
                }
            }
        }

        /// <summary>
        /// Sets a set of permissions for the given nodetype for the user
        /// </summary>
        public void set( CswEnumNbtNodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )
        {
            if( User != null )
            {
                set( Permissions, NodeType, _getRole( User.RoleId ), value );
            }
        }

        /// <summary>
        /// Sets a set of permissions for the given nodetype for the user
        /// </summary>
        public void set( CswEnumNbtNodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, bool value )
        {
            if( Role != null )
            {
                foreach( CswEnumNbtNodeTypePermission Permission in Permissions )
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
        public bool can( CswEnumNbtActionName ActionName )
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
        public bool can( CswEnumNbtActionName ActionName, ICswNbtUser User )
        {
            return can( _CswNbtResources.Actions[ActionName], User );
        }

        /// <summary>
        /// Returns true if the user has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtAction Action, ICswNbtUser User )
        {
            bool ret = false;
            if( null != Action && null != User )
            {
                if( User is CswNbtSystemUser || User.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                {
                    ret = true;
                }
                else
                {
                    ret = can( Action, _getRole( User.RoleId ) );
                }
            }
            return ret;
        } // can( CswNbtAction Action, ICswNbtUser User )

        /// <summary>
        /// Returns true if the role has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswEnumNbtActionName ActionName, CswNbtObjClassRole Role )
        {
            return can( _CswNbtResources.Actions[ActionName], Role );
        }

        /// <summary>
        /// Returns true if the role has the appropriate permissions for the Action
        /// </summary>
        public bool can( CswNbtAction Action, CswNbtObjClassRole Role )
        {
            bool ret = false;
            if( null != Role && null != Action )
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
        public void set( CswEnumNbtActionName ActionName, bool value )
        {
            set( _CswNbtResources.Actions[ActionName], _CswNbtResources.CurrentNbtUser, value );
        }

        /// <summary>
        /// Sets a permission for the given Action for the user
        /// </summary>
        public void set( CswEnumNbtActionName ActionName, ICswNbtUser User, bool value )
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

        public void set( CswEnumNbtActionName ActionName, CswNbtObjClassRole Role, bool value )
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
