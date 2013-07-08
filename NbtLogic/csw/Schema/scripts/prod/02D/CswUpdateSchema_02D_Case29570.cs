using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29570
    /// </summary>
    public class CswUpdateSchema_02D_Case29570 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29570; }
        }

        public override void update()
        {
            #region NodeTypes

            CswNbtMetaDataNodeType ReportGroupPermNT = _createPermissionNT( CswEnumNbtObjectClass.ReportGroupPermissionClass, "Report Group Permission" );
            CswNbtMetaDataNodeType MailReportGroupPermNT = _createPermissionNT( CswEnumNbtObjectClass.MailReportGroupPermissionClass, "Mail Report Group Permission" );
            CswNbtMetaDataNodeType ReportGroupNT = _createGroupNT( CswEnumNbtObjectClass.ReportGroupClass, "Report Group", CswNbtObjClassReportGroup.PropertyName.Reports );
            CswNbtMetaDataNodeType MailReportGroupNT = _createGroupNT( CswEnumNbtObjectClass.MailReportGroupClass, "Mail Report Group", CswNbtObjClassMailReportGroup.PropertyName.MailReports );
            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();//For some reason this is needed here or else the following code will throw an ORNI on a full master reset
            foreach( CswNbtMetaDataNodeType InvGrpPermNT in InvGrpPermOC.getNodeTypes() )
            {
                _setPermissionPropFilters( InvGrpPermNT );
            }

            #endregion NodeTypes

            #region Permission Group Nodes

            CswNbtObjClassReportGroup DefaultReportGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportGroupNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    ( (CswNbtObjClassReportGroup) NewNode ).Name.Text = "Default Report Group";
                } );

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            foreach( CswNbtObjClassReport ReportNode in ReportOC.getNodes( false, false ) )
            {
                ReportNode.ReportGroup.RelatedNodeId = DefaultReportGroup.NodeId;
                ReportNode.postChanges( false );
            }

            CswNbtObjClassMailReportGroup DefaultMailReportGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportGroupNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    ( (CswNbtObjClassMailReportGroup) NewNode ).Name.Text = "Default Mail Report Group";
                } );

            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, false ) )
            {
                MailReportNode.MailReportGroup.RelatedNodeId = DefaultMailReportGroup.NodeId;
                MailReportNode.postChanges( false );
            }

            #endregion Permission Group Nodes

            #region Inventory Group Permission Nodes

            CswNbtMetaDataNodeType InventoryGroupNT = InvGrpPermOC.FirstNodeType;
            if( null != InventoryGroupNT )
            {
                CswNbtMetaDataNodeTypeProp PermissionGroupNTP = InventoryGroupNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetPermission.PropertyName.PermissionGroup );
                if( null != PermissionGroupNTP )
                {
                    PermissionGroupNTP.PropName = CswNbtPropertySetPermission.PropertyName.PermissionGroup;
                }
            }

            foreach( CswNbtObjClassInventoryGroupPermission InvGrpPermNode in InvGrpPermOC.getNodes( false, false ) )
            {
                InvGrpPermNode.ApplyToAllWorkUnits.Checked = CswEnumTristate.False;
                InvGrpPermNode.ApplyToAllRoles.Checked = CswEnumTristate.False;
                InvGrpPermNode.PermissionGroup.RefreshNodeName();
                InvGrpPermNode.postChanges( false );
            }

            CswNbtMetaDataObjectClass InvGrpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            foreach( CswNbtObjClassInventoryGroup InvGrpNode in InvGrpOC.getNodes( false, false ) )
            {
                if( InvGrpNode.Name.Text == "Default Inventory Group" && null != InventoryGroupNT )
                {
                    _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InventoryGroupNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassInventoryGroupPermission WildCardInventoryGroupPermission = NewNode;
                            WildCardInventoryGroupPermission.ApplyToAllRoles.Checked = CswEnumTristate.True;
                            WildCardInventoryGroupPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                            WildCardInventoryGroupPermission.PermissionGroup.RelatedNodeId = InvGrpNode.NodeId;
                            WildCardInventoryGroupPermission.View.Checked = CswEnumTristate.True;
                            WildCardInventoryGroupPermission.Edit.Checked = CswEnumTristate.True;
                            WildCardInventoryGroupPermission.Dispense.Checked = CswEnumTristate.False;
                            WildCardInventoryGroupPermission.Dispose.Checked = CswEnumTristate.False;
                            WildCardInventoryGroupPermission.Request.Checked = CswEnumTristate.False;
                            WildCardInventoryGroupPermission.Undispose.Checked = CswEnumTristate.False;
                            WildCardInventoryGroupPermission.WorkUnit.RelatedNodeId = null;
                            WildCardInventoryGroupPermission.WorkUnit.RefreshNodeName();
                            WildCardInventoryGroupPermission.WorkUnit.SyncGestalt();
                        } );
                }
            }

            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportGroupPermNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassReportGroupPermission WildCardReportGroupPermission = NewNode;
                    WildCardReportGroupPermission.ApplyToAllRoles.Checked = CswEnumTristate.True;
                    WildCardReportGroupPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                    WildCardReportGroupPermission.PermissionGroup.RelatedNodeId = DefaultReportGroup.NodeId;
                    WildCardReportGroupPermission.View.Checked = CswEnumTristate.True;
                    WildCardReportGroupPermission.Edit.Checked = CswEnumTristate.True;
                    WildCardReportGroupPermission.postChanges( false );
                } );
            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportGroupPermNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassMailReportGroupPermission WildCardMailReportGroupPermission = NewNode;
                    WildCardMailReportGroupPermission.ApplyToAllRoles.Checked = CswEnumTristate.True;
                    WildCardMailReportGroupPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                    WildCardMailReportGroupPermission.PermissionGroup.RelatedNodeId = DefaultMailReportGroup.NodeId;
                    WildCardMailReportGroupPermission.View.Checked = CswEnumTristate.True;
                    WildCardMailReportGroupPermission.Edit.Checked = CswEnumTristate.True;
                    WildCardMailReportGroupPermission.postChanges( false );
                } );

            #endregion Inventory Group Permission Nodes
        } // update()

        private CswNbtMetaDataNodeType _createPermissionNT( CswEnumNbtObjectClass PermissionClass, string PermissionNTName )
        {
            CswNbtMetaDataObjectClass PermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermissionClass );
            CswNbtMetaDataNodeType PermissionNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( PermissionOC.ObjectClassId, PermissionNTName, "System" );
            PermissionNT.setNameTemplateText(
                CswNbtMetaData.MakeTemplateEntry( CswNbtPropertySetPermission.PropertyName.PermissionGroup ) + "-" +
                CswNbtMetaData.MakeTemplateEntry( CswNbtPropertySetPermission.PropertyName.Role ) + "-" +
                CswNbtMetaData.MakeTemplateEntry( CswNbtPropertySetPermission.PropertyName.WorkUnit ) );
            _setPermissionPropFilters( PermissionNT );
            return PermissionNT;
        }

        private void _setPermissionPropFilters( CswNbtMetaDataNodeType PermissionNT )
        {
            CswNbtMetaDataNodeTypeProp ApplyToAllWorkUnitsNTP = PermissionNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits );
            CswNbtMetaDataNodeTypeProp WorkUnitNTP = PermissionNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetPermission.PropertyName.WorkUnit );
            WorkUnitNTP.setFilter( ApplyToAllWorkUnitsNTP, ApplyToAllWorkUnitsNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );
            CswNbtMetaDataNodeTypeProp ApplyToAllRolesNTP = PermissionNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles );
            CswNbtMetaDataNodeTypeProp RoleNTP = PermissionNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetPermission.PropertyName.Role );
            RoleNTP.setFilter( ApplyToAllRolesNTP, ApplyToAllRolesNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );
        }

        private CswNbtMetaDataNodeType _createGroupNT( CswEnumNbtObjectClass GroupClass, string GroupNTName, string TargetsGridPropName )
        {
            CswNbtMetaDataObjectClass GroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( GroupClass );
            CswNbtMetaDataNodeType GroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( GroupOC.ObjectClassId, GroupNTName, "System" );
            GroupNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassReportGroup.PropertyName.Name ) );
            CswNbtMetaDataNodeTypeTab TaregetsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTabDeprecated( GroupNT, TargetsGridPropName );
            CswNbtMetaDataNodeTypeProp TargetsNTP = GroupNT.getNodeTypePropByObjectClassProp( TargetsGridPropName );
            TargetsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TaregetsTab.TabId );
            CswNbtMetaDataNodeTypeTab PermissionsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTabDeprecated( GroupNT, CswNbtObjClassReportGroup.PropertyName.Permissions );
            CswNbtMetaDataNodeTypeProp PermissionsNTP = GroupNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReportGroup.PropertyName.Permissions );
            PermissionsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, PermissionsTab.TabId );
            return GroupNT;
        }

    }//class CswUpdateSchema_02C_Case29570

}//namespace ChemSW.Nbt.Schema