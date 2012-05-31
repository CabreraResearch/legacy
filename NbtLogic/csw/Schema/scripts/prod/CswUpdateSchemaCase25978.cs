using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25978
    /// </summary>
    public class CswUpdateSchemaCase25978 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Make the Batch Operation Object Class and default NodeType
            CswNbtMetaDataObjectClass BatchOpOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, "clock.gif", false, false );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            CswCommaDelimitedString StatusOptions = new CswCommaDelimitedString() { 
                NbtBatchOpStatus.Pending.ToString(),  
                NbtBatchOpStatus.Processing.ToString(),  
                NbtBatchOpStatus.Completed.ToString(),  
                NbtBatchOpStatus.Error.ToString()
            };

            CswNbtMetaDataObjectClassProp BatchDataOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.BatchDataPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Memo, ServerManaged: true );
            CswNbtMetaDataObjectClassProp EndDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.EndDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.DateTime, ServerManaged: true );
            CswNbtMetaDataObjectClassProp LogOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.LogPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Comments, ServerManaged: true );
            CswNbtMetaDataObjectClassProp OpNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.OpNamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, ServerManaged: true );
            CswNbtMetaDataObjectClassProp PercentDoneOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.PercentDonePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Number, ServerManaged: true );
            CswNbtMetaDataObjectClassProp PriorityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.PriorityPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Number );
            CswNbtMetaDataObjectClassProp CreatedDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.CreatedDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.DateTime, ServerManaged: true );
            CswNbtMetaDataObjectClassProp StartDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.StartDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.DateTime, ServerManaged: true );
            CswNbtMetaDataObjectClassProp StatusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, 
                                                                                                     ServerManaged: true, 
                                                                                                     ListOptions: StatusOptions.ToString() );
            CswNbtMetaDataObjectClassProp UserOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.UserPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                                                                   IsFk: true,
                                                                                                   FkType: NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                                                                   FkValue: UserOC.ObjectClassId );

            PriorityOCP.DefaultValue.Field1_Numeric = 0;
            PriorityOCP.DefaultValue.Gestalt = "0";
            PercentDoneOCP.DefaultValue.Field1_Numeric = 0;
            PercentDoneOCP.DefaultValue.Gestalt = "0";

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CreatedDateOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StartDateOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EndDateOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );

            CswNbtMetaDataNodeType BatchOpNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( BatchOpOC.ObjectClassId, "Batch Operation", "System" );
            BatchOpNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( OpNameOCP.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( CreatedDateOCP.PropName ) );

            CswNbtMetaDataNodeTypeTab BatchOpTab = BatchOpNT.getFirstNodeTypeTab();

            CswNbtMetaDataNodeTypeProp OpNameNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.OpNamePropertyName );
            CswNbtMetaDataNodeTypeProp UserNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.UserPropertyName );
            CswNbtMetaDataNodeTypeProp PercentDoneNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.PercentDonePropertyName );
            CswNbtMetaDataNodeTypeProp PriorityNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.PriorityPropertyName );
            CswNbtMetaDataNodeTypeProp StatusNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.StatusPropertyName );
            CswNbtMetaDataNodeTypeProp CreatedDateNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.CreatedDatePropertyName );
            CswNbtMetaDataNodeTypeProp StartDateNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.StartDatePropertyName );
            CswNbtMetaDataNodeTypeProp EndDateNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.EndDatePropertyName );
            CswNbtMetaDataNodeTypeProp BatchDataNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.BatchDataPropertyName );
            CswNbtMetaDataNodeTypeProp LogNTP = BatchOpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassBatchOp.LogPropertyName );

            // Set tab layout
            OpNameNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 1, 1 );
            UserNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 2, 1 );
            PriorityNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 3, 1 );
            PercentDoneNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 4, 1 );
            StatusNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 5, 1 );
            CreatedDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 6, 1 );
            StartDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 7, 1 );
            EndDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 8, 1 );
            BatchDataNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 9, 1 );
            LogNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, BatchOpTab.TabId, 10, 1 );

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            // Batch Operations (all) view for chemsw_admin_role
            CswNbtNode ChemSwAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );

            CswNbtView BatchOpView = _CswNbtSchemaModTrnsctn.makeView();
            BatchOpView.makeNew( "Batch Operations (all)", NbtViewVisibility.Role, ChemSwAdminRole.NodeId );
            BatchOpView.Category = "System";
            BatchOpView.SetViewMode( NbtViewRenderingMode.Grid );

            CswNbtViewRelationship BatchOpViewRel = BatchOpView.AddViewRelationship( BatchOpOC, true );
            BatchOpView.AddViewProperty( BatchOpViewRel, OpNameOCP );
            BatchOpView.AddViewProperty( BatchOpViewRel, UserOCP );
            CswNbtViewProperty BatchStatusViewProp = BatchOpView.AddViewProperty( BatchOpViewRel, StatusOCP );
            CswNbtViewProperty BatchPriorityViewProp = BatchOpView.AddViewProperty( BatchOpViewRel, PriorityOCP );
            BatchOpView.AddViewPropertyFilter( BatchStatusViewProp,
                                               Value: NbtBatchOpStatus.Completed.ToString(),
                                               FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                               ShowAtRuntime: true );
            BatchOpView.setSortProperty( BatchPriorityViewProp, NbtViewPropertySortMethod.Descending );
            BatchOpView.save();

            // Batch Operations (all) view for Administrator
            CswNbtNode AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( AdminRole != null )
            {
                CswNbtView BatchOpView2 = _CswNbtSchemaModTrnsctn.makeView();
                BatchOpView2.makeNew( "Batch Operations (all)", NbtViewVisibility.Role, AdminRole.NodeId, null, BatchOpView );
                BatchOpView2.save();
            }


            // My Batch Operations view for everyone else
            CswNbtView BatchOpView3 = _CswNbtSchemaModTrnsctn.makeView();
            BatchOpView3.makeNew( "My Batch Operations", NbtViewVisibility.Global );
            BatchOpView3.Category = "System";
            BatchOpView3.SetViewMode( NbtViewRenderingMode.Tree );

            CswNbtViewRelationship BatchOpViewRel3 = BatchOpView3.AddViewRelationship( BatchOpOC, true );
            BatchOpView3.AddViewPropertyAndFilter( BatchOpViewRel3,
                                                   StatusOCP,
                                                   Value: NbtBatchOpStatus.Completed.ToString(),
                                                   FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                                   ShowAtRuntime: true );
            BatchOpView3.AddViewPropertyAndFilter( BatchOpViewRel3,
                                                   UserOCP,
                                                   Value: "me",
                                                   FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
            BatchOpView3.save();

            // Batch operation scheduled rule
            _CswNbtSchemaModTrnsctn.createScheduledRule( Sched.NbtScheduleRuleNames.BatchOp, MtSched.Core.Recurrence.NSeconds, 5 );


            // Batch threshold config var
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( "batchthreshold", "If an operation affects this number of nodes, run as a batch operation instead", "10", false );

            // case 26446 - All users should have View permissions to batch ops
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false ) )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.View, BatchOpNT, RoleNode, true );
            }

        }//Update()

    }//class CswUpdateSchemaCase25978

}//namespace ChemSW.Nbt.Schema