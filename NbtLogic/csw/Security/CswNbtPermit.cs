using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Security
{
	/// <summary>
	/// Security class responsible for enforcing all permissions
	/// </summary>
	public class CswNbtPermit
	{
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
			/// Permission to edit property values on this tab
			/// </summary>
			Edit
		}

		private CswNbtResources _CswNbtResources;
		public CswNbtPermit( CswNbtResources Resources )
		{
			_CswNbtResources = Resources;
		}

		#region NodeTypes

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, Int32 NodeTypeId )
		{
			return can( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ) );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, Int32 NodeTypeId, ICswNbtUser User )
		{
			return can( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ), User );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, Int32 NodeTypeId, ICswNbtUser User, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			return can( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ), User, Node, Prop );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, Int32 NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			return can( Permission, _CswNbtResources.MetaData.getNodeType( NodeTypeId ), null, Node, Prop );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType )
		{
			return can( Permission, NodeType, null );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User )
		{
			return can( Permission, NodeType, User, null, null );
		}

		public bool can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			return can( Permission, NodeType, null, Node, Prop );
		}

		/// <summary>
		/// Returns true if the user has the appropriate permissions for the nodetype
		/// </summary>
		public bool can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
		{
			bool ret = false;

			// Default to logged-in user
			if( User == null )
			{
				User = _CswNbtResources.CurrentNbtUser;
			}

			if( User != null )
			{
				if( User is CswNbtSystemUser )
				{
					ret = true;
				}
				else
				{
					CswNbtObjClassRole Role = User.RoleNode;
					if( Role != null && NodeType != null )
					{
						// Base case
						ret = Role.NodeTypePermissions.CheckValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType, Permission ) );

						// Only Administrators can edit Roles
						if( ret &&
							NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass &&
							Permission != NodeTypePermission.View &&
							!User.IsAdministrator() )
						{
							ret = false;
						}

						if( null != Node && null != Node.NodeId )
						{
							// case 2209 - Users can edit their own profile without permissions to the User nodetype
							if( !ret &&
								Node.NodeId == User.UserId )
							{
								ret = true;
							}

							// Prevent users from deleting themselves or their own roles
							if( ret &&
								Permission == NodeTypePermission.Delete &&
								( ( Node.NodeId == User.UserId ||
									Node.NodeId == User.RoleId ) ) )
							{
								ret = false;
							}

							if( MetaDataProp != null )
							{
								// You can't edit readonly properties
								if( ret &&
									Permission != NodeTypePermission.View &&
									Node.Properties[MetaDataProp].ReadOnly )
								{
									ret = false;
								}

								// case 8218 - Certain properties on the user's preferences are not allowed to be edited
								if( ret &&
									Node.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.UserClass &&
									!User.IsAdministrator() &&
									MetaDataProp.ObjectClassProp != null &&
									( MetaDataProp.ObjectClassProp.PropName == CswNbtObjClassUser.UsernamePropertyName ||
									  MetaDataProp.ObjectClassProp.PropName == CswNbtObjClassUser.RolePropertyName ||
									  MetaDataProp.ObjectClassProp.PropName == CswNbtObjClassUser.FailedLoginCountPropertyName ||
									  MetaDataProp.ObjectClassProp.PropName == CswNbtObjClassUser.AccountLockedPropertyName ) )
								{
									ret = false;
								}

								// Only admins can change other people's passwords
								if( ret &&
									Node.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.UserClass &&
									!User.IsAdministrator() &&
									User.UserNode.NodeId != Node.NodeId &&
									MetaDataProp.ObjectClassProp != null &&
									MetaDataProp.ObjectClassProp.PropName == CswNbtObjClassUser.PasswordPropertyName )
								{
									ret = false;
								}

							} // if( MetaDataProp != null )
						} // if( null != Node && null != Node.NodeId )
					} // if( Role != null )
				} // if-else( User is CswNbtSystemUser )
			} // if( User != null )

			return ret;

		} // can( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )

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
				set( Permission, NodeType, User.RoleNode, value );
			}
		}

		public void set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, CswNbtObjClassRole Role, bool value )
		{
			if( Role != null )
			{
				//Role.NodeTypePermissions.SetValue( Permission.ToString(), NodeType.FirstVersionNodeTypeId.ToString(), value );
				//Role.NodeTypePermissions.Save();
				if( value )
					Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType, Permission ) );
				else
					Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType, Permission ) );
				Role.postChanges( false );
			}

		} // set( NodeTypePermission Permission, CswNbtMetaDataNodeType NodeType, ICswNbtUser Role, bool value )

		/// <summary>
		/// Sets a set of permissions for the given nodetype for the user
		/// </summary>
		public void set( NodeTypePermission[] Permissions, CswNbtMetaDataNodeType NodeType, ICswNbtUser User, bool value )
		{
			if( User != null )
			{
				set( Permissions, NodeType, User.RoleNode, value );
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
						Role.NodeTypePermissions.AddValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType, Permission ) );
					else
						Role.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( NodeType, Permission ) );
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
			if( User is CswNbtSystemUser )
			{
				ret = true;
			}
			else if( User != null )
			{
				ret = can( Action, User.RoleNode );
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
				set( Action, User.RoleNode, value );
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


	} // class CswNbtPermit
} // namespace ChemSW.Nbt.Security
