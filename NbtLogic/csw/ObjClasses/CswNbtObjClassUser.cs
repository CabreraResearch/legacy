using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUser : CswNbtObjClass, ICswNbtUser
    {
        public static string RolePropertyName { get { return "Role"; } }
        public static string AccountLockedPropertyName { get { return "AccountLocked"; } }
        public static string FailedLoginCountPropertyName { get { return "FailedLoginCount"; } }
        public static string PasswordPropertyName { get { return "Password"; } }
        public static string UsernamePropertyName { get { return "Username"; } }
        public static string FirstNamePropertyName { get { return "First Name"; } }
        public static string LastNamePropertyName { get { return "Last Name"; } }
        public static string LastLoginPropertyName { get { return "Last Login"; } }
        public static string QuickLaunchViewsPropertyName { get { return "Quick Launch Views"; } }
        public static string QuickLaunchActionsPropertyName { get { return "Quick Launch Actions"; } }
        public static string EmailPropertyName { get { return "Email"; } }
        public static string PageSizePropertyName { get { return "Page Size"; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtObjClassRole _RoleNodeObjClass = null;
        private CswNbtNode _RoleNode = null;
        private CswNbtNode _UserNode = null;

        public CswNbtObjClassUser( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public CswNbtObjClassUser( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

            if( Node.NodeId != null )
            {
                CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                CswNbtMetaDataObjectClassProp UserName_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.UsernamePropertyName );
                CswNbtMetaDataObjectClassProp Role_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.RolePropertyName );

                // generate the view
                CswNbtView View = new CswNbtView( _CswNbtResources );
                View.ViewName = "CswNbtObjClassUser(" + Node.NodeId.ToString() + ")";
                CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );
                UserRelationship.NodeIdsToFilterIn.Add( Node.NodeId );
                CswNbtViewRelationship RoleRelationship = View.AddViewRelationship( UserRelationship, CswNbtViewRelationship.PropOwnerType.First, Role_ObjectClassProp, false );

                // generate the tree
                ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, true );


                // get user node
                UserTree.goToRoot();
                if( UserTree.getChildNodeCount() > 0 )
                {
                    UserTree.goToNthChild( 0 );

                    //get role node
                    if( UserTree.getChildNodeCount() > 0 )
                    {
                        UserTree.goToNthChild( 0 );
                        _RoleNode = UserTree.getNodeForCurrentPosition();
                        _RoleNodeObjClass = CswNbtNodeCaster.AsRole( _RoleNode );
                    }
                }

            }
        }//ctor()

        public void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
            _RoleNodeObjClass.postChanges( ForceUpdate );
        }


        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();

            // BZ 5906
            UsernameProperty.ReadOnly = true;

            if( ( Role.WasModified && !( _CswNbtResources.CurrentNbtUser.IsAdministrator() ) ) )
            {
                throw new CswDniException( "Only Administrators can change user roles", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to edit a user role." );
            }

        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            //bz # 6555
            if( AccountLocked.Checked != Tristate.True && AccountLocked.WasModified )
            {
                clearFailedLoginCount();
            }

            // BZ 9170
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

            //prevent user from deleting their own user
            if( _CswNbtNode.NodeId == _CswNbtResources.CurrentUser.UserId )
            {
                throw ( new CswDniException( "You can not delete your own user account.", "Current user (" + _CswNbtResources.CurrentUser.Username + ") can not delete own UserClass node." ) );
            }

            ////prevent user from deleting ScheduleRunner
            //if (Username.ToLower() == "schedulerunner")
            //{
            //    throw new CswDniException("You cannot delete the ScheduleRunner user", "Current user (" + _CswNbtResources.CurrentUser.Username + ") attempted to delete the ScheduleRunner user");
            //}


            CswPrimaryKey RoleId = Role.RelatedNodeId;
            if( RoleId != null )
            {
                CswNbtNode RoleNode = _CswNbtResources.Nodes[RoleId];

                //prevent user from deleting admin if they are not an admin
                if( _RoleNodeObjClass.Administrator.Checked == Tristate.True && _CswNbtResources.CurrentNbtUser.IsAdministrator() != true )
                {
                    throw ( new CswDniException( "You can not delete administrator accounts because you are not an administrator.", "Block user account delete because login user (" + _CswNbtResources.CurrentUser.Username + ") is not an administrator." ) );
                }
            }
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            // BZ 6941, 8288
            // Set the Default View to use the selected User, rather than the logged in User
            //DefaultView.User = this;
            QuickLaunchViews.User = this;

            // BZ 8288
            // Quick Launch Actions options should derive from Role's Action Permissions
            if( _RoleNode != null )
            {
                CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();

                foreach( string YValue in _RoleNodeObjClass.ActionPermissions.YValues )
                {
                    if( _RoleNodeObjClass.ActionPermissions.CheckValue( CswNbtObjClassRole.ActionPermissionsXValueName, YValue ) )
                        NewYValues.Add( YValue );
                }
                this.QuickLaunchActions.YValues = NewYValues;
            }

            //BZ 9933
            if( _CswNbtResources.CurrentNbtUser == null || !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                this.FailedLoginCount.Hidden = true;
                this.AccountLocked.Hidden = true;
            }
            _CswNbtObjClassDefault.afterPopulateProps();

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswPrimaryKey UserId { get { return _CswNbtNode.NodeId; } }
        public CswPrimaryKey RoleId { get { return _RoleNode.NodeId; } }

        public CswNbtObjClassUser UserNode { get { return this; } }
        public CswNbtObjClassRole RoleNode { get { return _RoleNodeObjClass; } }
        public string Rolename { get { return _RoleNodeObjClass.Name.Text; } }
        public Int32 RoleTimeout { get { return CswConvert.ToInt32( _RoleNodeObjClass.Timeout.Value ); } }

        public CswNbtNodePropRelationship Role { get { return ( _CswNbtNode.Properties[RolePropertyName].AsRelationship ); } }
        public CswNbtNodePropLogical AccountLocked { get { return ( _CswNbtNode.Properties[AccountLockedPropertyName].AsLogical ); } }
        public CswNbtNodePropNumber FailedLoginCount { get { return ( _CswNbtNode.Properties[FailedLoginCountPropertyName].AsNumber ); } }
        public CswNbtNodePropPassword PasswordProperty { get { return ( _CswNbtNode.Properties[PasswordPropertyName].AsPassword ); } }
        public CswNbtNodePropText UsernameProperty { get { return ( _CswNbtNode.Properties[UsernamePropertyName].AsText ); } }
        public CswNbtNodePropText FirstNameProperty { get { return ( _CswNbtNode.Properties[FirstNamePropertyName].AsText ); } }
        public CswNbtNodePropText LastNameProperty { get { return ( _CswNbtNode.Properties[LastNamePropertyName].AsText ); } }
        public string FirstName { get { return FirstNameProperty.Text; } }
        public string LastName { get { return LastNameProperty.Text; } }
        public string Username { get { return UsernameProperty.Text; } }
        public CswNbtNodePropDate LastLogin { get { return ( _CswNbtNode.Properties[LastLoginPropertyName].AsDate ); } }
        public CswNbtNodePropViewPickList QuickLaunchViews { get { return _CswNbtNode.Properties[QuickLaunchViewsPropertyName].AsViewPickList; } }
        public CswNbtNodePropLogicalSet QuickLaunchActions { get { return _CswNbtNode.Properties[QuickLaunchActionsPropertyName].AsLogicalSet; } }
        public CswNbtNodePropText EmailProperty { get { return _CswNbtNode.Properties[EmailPropertyName].AsText; } }
        public string Email { get { return EmailProperty.Text; } }

        public string EncryptedPassword { get { return PasswordProperty.EncryptedPassword; } }

        public CswNbtNodePropNumber PageSizeProperty { get { return _CswNbtNode.Properties[PageSizePropertyName].AsNumber; } }
        public Int32 PageSize
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( PageSizeProperty.Value );
                if( ret <= 0 ) ret = 10;
                return ret;
            }
        }

        #endregion


        public int getFailedLoginCount()
        {
            double LoginCount = FailedLoginCount.Value;
            if( double.IsNaN( LoginCount ) ) LoginCount = 0;
            if( LoginCount < 0 ) LoginCount = 0;
            return CswConvert.ToInt32( LoginCount );
        }

        public void incFailedLoginCount()
        {
            int failures = getFailedLoginCount();
            failures += 1;

            this.FailedLoginCount.Value = Convert.ToDouble( failures );

            if( failures >= CswConvert.ToInt32( _CswNbtResources.getConfigVariableValue( "failedloginlimit" ) ) )
            {
                this.AccountLocked.Checked = Tristate.True;
            }
        }

        public void clearFailedLoginCount()
        {
            this.FailedLoginCount.Value = 0;
        }

        public bool IsAccountLocked()
        {
            return this.AccountLocked.Checked == Tristate.True;
        }

        public bool IsAdministrator()
        {
            return _RoleNodeObjClass.Administrator.Checked == Tristate.True;
        }

        public bool canEditPassword( CswNbtNode UserNode )
        {
            return ( _CswNbtResources.CurrentNbtUser.UserNode.Node.NodeId == UserNode.NodeId || _CswNbtResources.CurrentNbtUser.IsAdministrator() );
        }

        // see BZ 6971
        //public void GrantPermission(NodeTypePermission Permission, Int32 NodeTypeId)
        //{
        //    if (NodeTypeId > 0 && IsAdministrator())
        //    {
        //        CswNbtNodePropNodeTypePermissions PropPermissions = _RoleNodeObjClass.Permissions;
        //        PropPermissions.SetPermission(NodeTypeId, Permission, true);
        //        PropPermissions.Save();
        //    }
        //}
        //public void RevokePermission(NodeTypePermission Permission, Int32 NodeTypeId)
        //{
        //    if (NodeTypeId > 0 && IsAdministrator())
        //    {
        //        CswNbtNodePropNodeTypePermissions PropPermissions = _RoleNodeObjClass.Permissions;
        //        PropPermissions.SetPermission(NodeTypeId, Permission, false); 
        //        PropPermissions.Save();
        //    }
        //}


        // This procedure should eventually grow to become a fully architected solution for special case permission overrides.
        /// <summary>
        /// Check to see if the user has the permission on a nodetype
        /// </summary>
        /// <param name="Permission">Permission to check</param>
        /// <param name="NodeTypeId">Primary key of NodeType</param>
        /// <param name="NodeKey">Node being edited (optional)</param>
        /// <param name="MetaDataProp">Property being edited (optional)</param>
        /// <returns></returns>
        public bool CheckPermission( NodeTypePermission Permission, Int32 NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
        {
            bool ret = false;
            CswNbtNodePropLogicalSet PropPermissions = _RoleNodeObjClass.NodeTypePermissions;
            if( NodeTypeId > 0 )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( NodeType != null )
                {
                    if( Permission != NodeTypePermission.View && !IsAdministrator() &&
                        NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass )
                    {
                        // Override -- only Administrators have permissions to edit Roles
                        ret = false;
                    }
                    else
                    {
                        if( Node != null )
                        {
                            if( PropPermissions.CheckValue( Permission.ToString(), NodeType.FirstVersionNodeTypeId.ToString() ) )
                            {
                                ret = true;
                            }
                            else
                            {
                                if( null != Node.NodeId )
                                {
                                    // Prevent users from deleting themselves or their own roles
                                    if( ( Node.NodeId == _CswNbtResources.CurrentUser.UserId ||
                                          Node.NodeId == _CswNbtResources.CurrentUser.RoleId ) &&
                                        Permission == NodeTypePermission.Delete )
                                    {
                                        ret = false;
                                    }
                                    else
                                    {
                                        //bz 8218
                                        // Is the user editing his or her own preferences? 
                                        if( Node.NodeId == _CswNbtResources.CurrentUser.UserId )
                                        {
                                            ret = true;
                                            // Certain properties are controlled
                                            if( MetaDataProp != null && MetaDataProp.ObjectClassProp != null )
                                            {
                                                if( MetaDataProp.ObjectClassProp.PropName == UsernamePropertyName ||
                                                    MetaDataProp.ObjectClassProp.PropName == RolePropertyName ||
                                                    MetaDataProp.ObjectClassProp.PropName == FailedLoginCountPropertyName ||
                                                    MetaDataProp.ObjectClassProp.PropName == AccountLockedPropertyName )
                                                {
                                                    ret = false;
                                                }
                                            }

                                        }
                                        //else { ret = false; }
                                    }

                                }//if this node has a node id
                            }
                        }
                        else
                        {
                            ret = PropPermissions.CheckValue( Permission.ToString(), NodeType.FirstVersionNodeTypeId.ToString() );
                        }
                    }
                }
            }
            if( ret && null != Node && null != MetaDataProp )
            {
                ret = _checkNodeTypePropPermission( PropPermissions, Permission, Node, MetaDataProp );
            }

            return ret;

        } // CheckPermission()

        private bool _checkNodeTypePropPermission( CswNbtNodePropLogicalSet PropPermissions, NodeTypePermission Permission, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
        {
            bool ret = ( Permission == NodeTypePermission.View || !Node.Properties[MetaDataProp].ReadOnly );
            switch( MetaDataProp.FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    {
                        if( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass == Node.ObjectClass.ObjectClass &&
                            null != MetaDataProp.ObjectClassProp &&
                            MetaDataProp.ObjectClassProp.ObjectClassPropId == PasswordProperty.ObjectClassPropId )
                        {
                            ret = canEditPassword( Node );
                        }
                        break;
                    }
                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    {
                        // review 21813: Can always view the relationship prop
                        if( Permission != NodeTypePermission.View )
                        {
                            Int32 TargetNodeTypeId = Node.Properties[MetaDataProp].AsRelationship.TargetId;
                            if( Int32.MinValue != TargetNodeTypeId )
                            {
                                var TargetIdType = Node.Properties[MetaDataProp].AsRelationship.TargetType;
                                switch( TargetIdType )
                                {
                                    case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                                        {
                                            CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetNodeTypeId );
                                            if( null != TargetNodeType )
                                            {
                                                if( !IsAdministrator() &&
                                                    null != TargetNodeType.ObjectClass &&
                                                    TargetNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass &&
                                                    null != MetaDataProp.ObjectClassProp &&
                                                    MetaDataProp.ObjectClassProp == _CswNbtResources.MetaData.getObjectClassProp( Role.ObjectClassPropId ) )
                                                {
                                                    ret = false;
                                                }
                                                else
                                                {
                                                    ret = PropPermissions.CheckValue( Permission.ToString(), TargetNodeType.FirstVersionNodeTypeId.ToString() );
                                                }
                                            }
                                            break;
                                        }
                                    case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                                        {
                                            CswNbtMetaDataObjectClass TargetObjectClass = _CswNbtResources.MetaData.getObjectClass( TargetNodeTypeId );
                                            if( null != TargetObjectClass )
                                            {
                                                if( !IsAdministrator() &&
                                                    TargetObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass &&
                                                    MetaDataProp.ObjectClassProp == _CswNbtResources.MetaData.getObjectClassProp( Role.ObjectClassPropId ) )
                                                {
                                                    ret = false;
                                                }
                                                else
                                                {
                                                    // case 21842 - this doesn't work
                                                    // ret = PropPermissions.CheckValue( Permission.ToString(), TargetObjectClass.ObjectClassId.ToString() );
                                                    ret = true;
                                                }
                                            }
                                            break;
                                        }
                                }

                            }
                        }
                        break;
                    }
            }

            return ret;
        }

        /// <summary>
        /// Check create permissions, no existing node required
        /// </summary>
        public bool CheckCreatePermission( Int32 NodeTypeId )
        {
            bool ret = true;
            if( NodeTypeId > 0 )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );

                if( !IsAdministrator() &&
                    NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass )
                {
                    // Override -- only Administrators have permissions to edit Roles
                    ret = false;
                }
                else
                {
                    CswNbtNodePropLogicalSet PropPermissions = _RoleNodeObjClass.NodeTypePermissions;
                    ret = PropPermissions.CheckValue( NodeTypePermission.Create.ToString(), NodeType.FirstVersionNodeTypeId.ToString() );
                }
            }
            return ret;
        } // CheckCreatePermission()

        /// <summary>
        /// Check Action permission
        /// </summary>
        public bool CheckActionPermission( CswNbtActionName ActionName )
        {
            return RoleNode.CheckActionPermission( ActionName );
        }


    }//CswNbtObjClassUser

}//namespace ChemSW.Nbt.ObjClasses
