using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30480 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Take 3 - Update Location Inventory Group Data Some More"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30480; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30480"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass InventoryGroupPermissionOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );
            CswNbtMetaDataNodeType InvGrpNT = InventoryGroupOc.FirstNodeType;
            CswNbtMetaDataNodeType InvGrpPermNT = InventoryGroupPermissionOc.FirstNodeType;
            
            //1: Get Default Inventory Group
            CswNbtObjClassInventoryGroup DefaultInventoryGroup = null;
            CswNbtObjClassInventoryGroup CISProInventoryGroup = null;
            foreach( CswNbtObjClassInventoryGroup InventoryGroup in InventoryGroupOc.getNodes( true, false, false, true ) )
            {
                if( InventoryGroup.Name.Text == "Default Inventory Group" )
                {
                    DefaultInventoryGroup = InventoryGroup;
                }
                else if( InventoryGroup.Name.Text == "CISPro" )
                {
                    CISProInventoryGroup = InventoryGroup;
                }
            }
            if( null == DefaultInventoryGroup )
            {
                DefaultInventoryGroup = InventoryGroupOc.getNodes( true, false, false, true ).FirstOrDefault();
            }
            if( null == DefaultInventoryGroup )
            {
                if( null != InvGrpNT )
                {
                    DefaultInventoryGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InvGrpNT.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassInventoryGroup DefInvGrp = NewNode;
                        DefInvGrp.Name.Text = "Default Inventory Group";
                    } );
                }
            }

            if( null != DefaultInventoryGroup )//At this point DefaultInventoryGroup should never be null
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
                CswNbtMetaDataNodeType IgPermNt = InventoryGroupPermissionOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != IgPermNt )
                {
                    try
                    {
                        CswNbtObjClassInventoryGroupPermission DefaultPermission = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( IgPermNt.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassInventoryGroupPermission WildCardPerm = NewNode;
                            WildCardPerm.PermissionGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                            WildCardPerm.ApplyToAllRoles.Checked = CswEnumTristate.True;
                            WildCardPerm.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                            WildCardPerm.Dispense.Checked = CswEnumTristate.True;
                            WildCardPerm.Dispose.Checked = CswEnumTristate.True;
                            WildCardPerm.Edit.Checked = CswEnumTristate.True;
                            WildCardPerm.Request.Checked = CswEnumTristate.True;
                            WildCardPerm.Undispose.Checked = CswEnumTristate.True;
                        } );
                    }
                    catch( CswDniException ex )//If we're here, it's because this wildcard already exists
                    {
                        if( ex.ErrorType != CswEnumErrorType.Warning )
                        {
                            throw;
                        }
                    }
                }

                //5: Get rid of deprecated CISPro Inventory Group and move their permissions over to Default Inventory Group
                if( null != CISProInventoryGroup )
                {
                    CswNbtMetaDataObjectClass WorkUnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
                    CswNbtObjClassWorkUnit DefaultWorkUnit = null;
                    foreach( CswNbtObjClassWorkUnit WorkUnit in WorkUnitOc.getNodes( true, false, false, true ) )
                    {
                        if( WorkUnit.Name.Text == "Default Work Unit" )
                        {
                            DefaultWorkUnit = WorkUnit;
                        }
                    }
                    if( null != DefaultWorkUnit )
                    {
                        foreach( CswNbtObjClassInventoryGroupPermission InventoryGroupPerm in InventoryGroupPermissionOc.getNodes( true, false, false, true ) )
                        {
                            if( InventoryGroupPerm.PermissionGroup.RelatedNodeId == CISProInventoryGroup.NodeId )
                            {
                                try
                                {
                                    InventoryGroupPerm.PermissionGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                                    InventoryGroupPerm.PermissionGroup.SyncGestalt();
                                    InventoryGroupPerm.WorkUnit.RelatedNodeId = DefaultWorkUnit.NodeId;
                                    InventoryGroupPerm.WorkUnit.SyncGestalt();
                                    InventoryGroupPerm.postChanges( false );
                                }
                                catch( CswDniException )//If we're here, it's because the Permission already exists on Default Inventory Group
                                {
                                    InventoryGroupPerm.Node.delete( true, true );
                                }
                            }
                        }
                    }
                    CISProInventoryGroup.Node.delete( true, true );
                }
            }
            //6: Fix Inventory Group Permissions grid prop
            if( null != InvGrpNT && null != InvGrpPermNT )
            {
                CswNbtMetaDataNodeTypeProp PermissionsNTP = InvGrpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.Permissions );

                CswNbtView InvGrpPermView = _CswNbtSchemaModTrnsctn.restoreView( PermissionsNTP.ViewId );
                if( null == InvGrpPermView )
                {
                    InvGrpPermView = _CswNbtSchemaModTrnsctn.makeSafeView( "Permissions", CswEnumNbtViewVisibility.Property );
                    InvGrpPermView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                }
                InvGrpPermView.Root.ChildRelationships.Clear();

                CswNbtMetaDataNodeTypeProp PermissionGroupNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.PermissionGroup );
                CswNbtMetaDataNodeTypeProp RoleNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Role );
                CswNbtMetaDataNodeTypeProp WorkUnitNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );
                CswNbtMetaDataNodeTypeProp ViewNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.View );
                CswNbtMetaDataNodeTypeProp EditNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Edit );
                CswNbtMetaDataNodeTypeProp DispenseNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Dispense );
                CswNbtMetaDataNodeTypeProp RequestNTP = InvGrpPermNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Request );

                CswNbtViewRelationship RootRel = InvGrpPermView.AddViewRelationship( InvGrpNT, false );
                CswNbtViewRelationship PermRel = InvGrpPermView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, PermissionGroupNTP, true );
                InvGrpPermView.AddViewProperty( PermRel, PermissionGroupNTP, 1 );
                InvGrpPermView.AddViewProperty( PermRel, RoleNTP, 2 );
                InvGrpPermView.AddViewProperty( PermRel, WorkUnitNTP, 3 );
                InvGrpPermView.AddViewProperty( PermRel, ViewNTP, 4 );
                InvGrpPermView.AddViewProperty( PermRel, EditNTP, 5 );
                InvGrpPermView.AddViewProperty( PermRel, DispenseNTP, 6 );
                InvGrpPermView.AddViewProperty( PermRel, RequestNTP, 7 );
                InvGrpPermView.save();

                PermissionsNTP.ViewId = InvGrpPermView.ViewId;
            }
        }
    }
}