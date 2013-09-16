using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30480 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Update Location Inventory Group Data"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30480; }
        }

        public override string ScriptName
        {
            get { return "IGUpdate"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass InventoryGroupPermissisonOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );

            //1: Get Default Inventory Group
            CswNbtObjClassInventoryGroup DefaultInventoryGroup = null;
            foreach( CswNbtObjClassInventoryGroup InventoryGroup in InventoryGroupOc.getNodes( true, false, false, true ) )
            {
                if( InventoryGroup.Name.Text == "Default Inventory Group" )
                {
                    DefaultInventoryGroup = InventoryGroup;
                }
            }
            if( null == DefaultInventoryGroup )
            {
                DefaultInventoryGroup = InventoryGroupOc.getNodes( true, false, false, true ).FirstOrDefault();
            }

            if( null != DefaultInventoryGroup )
            {
                //2: Set Default Value to Default Inventory Group, if no DefaultValue is present
                CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                foreach( CswNbtMetaDataNodeType LocationNt in LocationOc.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp InventoryGroupNtp = LocationNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                    if( false == InventoryGroupNtp.HasDefaultValue() )
                    {
                        InventoryGroupNtp.DefaultValue.AsRelationship.RelatedNodeId = DefaultInventoryGroup.NodeId;
                    }
                }

                //3: Update any null Inventory Group values with Default
                foreach( CswNbtObjClassLocation Location in LocationOc.getNodes( true, false, false, true ) )
                {
                    if( false == CswTools.IsPrimaryKey( Location.InventoryGroup.RelatedNodeId ) )
                    {
                        Location.InventoryGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                        Location.postChanges( ForceUpdate: false );
                    }
                }

                //4: Ensure at least one Inventory Group Permission exists
                {
                    CswNbtView IgPermitView = _CswNbtSchemaModTrnsctn.makeView();
                    IgPermitView.AddViewRelationship( InventoryGroupPermissisonOc, IncludeDefaultFilters: false );
                    ICswNbtTree Tree = _CswNbtSchemaModTrnsctn.getTreeFromView( IgPermitView, IncludeSystemNodes: false );
                    if( Tree.getChildNodeCount() == 0 )
                    {
                        CswNbtMetaDataNodeType IgPermNt = InventoryGroupPermissisonOc.getLatestVersionNodeTypes().FirstOrDefault();
                        if( null != IgPermNt )
                        {
                            CswNbtObjClassInventoryGroupPermission DefaultPermission = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( IgPermNt.NodeTypeId );
                            DefaultPermission.PermissionGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                            
                            DefaultPermission.ApplyToAllRoles.Checked = CswEnumTristate.True;
                            DefaultPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                            
                            DefaultPermission.Dispense.Checked = CswEnumTristate.True;
                            DefaultPermission.Dispose.Checked = CswEnumTristate.True;
                            DefaultPermission.Edit.Checked = CswEnumTristate.True;
                            DefaultPermission.Request.Checked = CswEnumTristate.True;
                            DefaultPermission.Undispose.Checked = CswEnumTristate.True;

                            DefaultPermission.postChanges( ForceUpdate: false );
                        }
                    }
                }

            }

            //5: Revoke MetaData Permissions
            {
                if( _CswNbtSchemaModTrnsctn.isMaster() )
                {
                    Collection<CswNbtObjClassRole> Roles = new Collection<CswNbtObjClassRole>();
                    {
                        CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                        Collection<CswNbtNode> RoleNodes = RoleOc.getNodes( true, false, false, true );
                        foreach( CswNbtObjClassRole Role in RoleNodes )
                        {
                            if( Role.Name.Text != CswNbtObjClassRole.ChemSWAdminRoleName )
                            {
                                Roles.Add( Role );
                            }
                        }
                    }

                    CswEnumNbtNodeTypePermission[] NtPermissions = { CswEnumNbtNodeTypePermission.Create, CswEnumNbtNodeTypePermission.Edit, CswEnumNbtNodeTypePermission.View, CswEnumNbtNodeTypePermission.Delete };
                    CswEnumNbtNodeTypeTabPermission[] NtTabPermissions = { CswEnumNbtNodeTypeTabPermission.Edit, CswEnumNbtNodeTypeTabPermission.View };
                    CswNbtObjClassRole ChemSwAdmin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                    
                    Func<CswNbtMetaDataNodeType,bool> SetPerms = ( NodeType ) =>
                    {
                        foreach( CswNbtObjClassRole Role in Roles )
                        {
                            _CswNbtSchemaModTrnsctn.Permit.set( NtPermissions, NodeType, Role, false );
                            foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
                            {
                                _CswNbtSchemaModTrnsctn.Permit.set( NtTabPermissions, Tab, Role, false );
                            }
                        }

                        _CswNbtSchemaModTrnsctn.Permit.set( NtPermissions, NodeType, ChemSwAdmin, true );
                        return false;
                    };
                    
                    foreach( CswNbtMetaDataNodeType InventoryGroupNT in InventoryGroupOc.getNodeTypes() )
                    {
                        SetPerms( InventoryGroupNT );
                    }

                    foreach( CswNbtMetaDataNodeType InventoryGroupPermNT in InventoryGroupPermissisonOc.getNodeTypes() )
                    {
                        SetPerms( InventoryGroupPermNT );
                    }
                }
            }
        }
    }
}