using System;
using System.Collections.ObjectModel;
using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31110 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31110; }
        }

        public override string Title
        {
            get { return "Set permission on CISPro Report Group"; }
        }

        public override void update()
        {
            // CISPro Report Group (created in CswUpdateSchema_02H_28562C) should only be accessible to CISPro roles

            CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
            CswNbtMetaDataObjectClass PermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupPermissionClass );
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );

            CswNbtMetaDataObjectClassProp GroupNameOCP = ReportGroupOC.getObjectClassProp( CswNbtObjClassReportGroup.PropertyName.Name );
            CswNbtMetaDataObjectClassProp PermissionGroupOCP = PermissionOC.getObjectClassProp( CswNbtObjClassReportGroupPermission.PropertyName.PermissionGroup );
            CswNbtMetaDataObjectClassProp RoleNameOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );

            // Delete existing (default) permissions
            CswNbtView View = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship Rel1 = View.AddViewRelationship( ReportGroupOC, false );
            View.AddViewPropertyAndFilter( Rel1, GroupNameOCP, Value: "CISPro Report Group", FilterMode: CswEnumNbtFilterMode.Equals );
            CswNbtViewRelationship Rel2 = View.AddViewRelationship( Rel1, CswEnumNbtViewPropOwnerType.Second, PermissionGroupOCP, false );

            ICswNbtTree results = _CswNbtSchemaModTrnsctn.getTreeFromView( View, true );
            CswNbtObjClassReportGroup CISProGroup = null;
            for( Int32 g = 0; g < results.getChildNodeCount(); g++ )
            {
                results.goToNthChild( g );

                CISProGroup = results.getNodeForCurrentPosition();
                for( Int32 p = 0; p < results.getChildNodeCount(); p++ )
                {
                    results.goToNthChild( p );

                    CswNbtNode PermNode = results.getNodeForCurrentPosition();
                    PermNode.delete( false, true );

                    results.goToParentNode();
                }

                results.goToParentNode();
            } // for( Int32 g = 0; g < results.getChildNodeCount(); g++ )

            if( null != CISProGroup )
            {
                // Get all cispro roles
                CswNbtView rolesView = _CswNbtSchemaModTrnsctn.makeView();
                CswNbtViewRelationship parent = rolesView.AddViewRelationship( RoleOC, false );
                rolesView.AddViewPropertyAndFilter( parent,
                                                    MetaDataProp: RoleNameOCP,
                                                    SubFieldName: CswEnumNbtSubFieldName.Text,
                                                    FilterMode: CswEnumNbtFilterMode.Contains,
                                                    Value: "cispro" );
                rolesView.AddViewPropertyAndFilter( parent,
                                                    Conjunction: CswEnumNbtFilterConjunction.Or,
                                                    MetaDataProp: RoleNameOCP,
                                                    SubFieldName: CswEnumNbtSubFieldName.Text,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: "Administrator" );
                rolesView.AddViewPropertyAndFilter( parent,
                                                    Conjunction: CswEnumNbtFilterConjunction.Or,
                                                    MetaDataProp: RoleNameOCP,
                                                    SubFieldName: CswEnumNbtSubFieldName.Text,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: CswNbtObjClassRole.ChemSWAdminRoleName );

                ICswNbtTree rolesTree = _CswNbtSchemaModTrnsctn.getTreeFromView( rolesView, true );
                Collection<CswPrimaryKey> CISProRoleIds = new Collection<CswPrimaryKey>();
                for( int i = 0; i < rolesTree.getChildNodeCount(); i++ )
                {
                    rolesTree.goToNthChild( i );
                    CISProRoleIds.Add( rolesTree.getNodeIdForCurrentPosition() );
                    rolesTree.goToParentNode();
                }

                // Grant permission to all cispro roles
                CswNbtMetaDataNodeType PermissionNT = PermissionOC.FirstNodeType;
                if( null != PermissionNT )
                {
                    foreach( CswPrimaryKey RoleId in CISProRoleIds )
                    {
                        _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PermissionNT.NodeTypeId, delegate( CswNbtNode NewNode )
                            {
                                CswNbtPropertySetPermission NewPermission = NewNode;
                                NewPermission.ApplyToAllRoles.Checked = CswEnumTristate.False;
                                NewPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                                NewPermission.PermissionGroup.RelatedNodeId = CISProGroup.NodeId;
                                NewPermission.View.Checked = CswEnumTristate.True;
                                NewPermission.Edit.Checked = CswEnumTristate.True;
                                NewPermission.Role.RelatedNodeId = RoleId;
                            } );
                    }
                }
            } // if( null != CISProGroup )

        } // update()

    } // class CswUpdateSchema_02H_Case31110 

}//namespace ChemSW.Nbt.Schema