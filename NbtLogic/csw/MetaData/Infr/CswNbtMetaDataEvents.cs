using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataEvents
    {
        CswNbtResources _CswNbtResources = null;
        public CswNbtMetaDataEvents( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void OnCopyNodeType( CswNbtMetaDataNodeType OriginalNodeType, CswNbtMetaDataNodeType CopyNodeType )
        {
            OnMakeNewNodeType( CopyNodeType, true );
        }

        public void OnMakeNewNodeType( CswNbtMetaDataNodeType NewNodeType, bool IsCopy )
        {
            // Give the current user's role full permissions to the new nodetype
            CswEnumNbtNodeTypePermission[] AllPerms = new CswEnumNbtNodeTypePermission[] {
                                                CswEnumNbtNodeTypePermission.Delete, 
                                                CswEnumNbtNodeTypePermission.Create, 
                                                CswEnumNbtNodeTypePermission.Edit, 
                                                CswEnumNbtNodeTypePermission.View };

            if( null != _CswNbtResources.CurrentNbtUser.RoleId )
            {
                CswNbtNode RoleNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.RoleId];
                CswNbtObjClassRole RoleNodeAsRole = (CswNbtObjClassRole) RoleNode;

                // case 23185 - reset permission options
                RoleNodeAsRole.triggerAfterPopulateProps();

                _CswNbtResources.Permit.set( AllPerms, NewNodeType, RoleNodeAsRole, true );

            }//if we have a current user
            else if( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
            {
                // Grant permission to Administrator
                CswNbtNode RoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( "Administrator" );
                if( RoleNode != null )
                {
                    _CswNbtResources.Permit.set( AllPerms, NewNodeType, (CswNbtObjClassRole) RoleNode, true );
                }
                CswNbtNode RoleNode2 = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                if( RoleNode2 != null )
                {
                    _CswNbtResources.Permit.set( AllPerms, NewNodeType, (CswNbtObjClassRole) RoleNode2, true );
                }
            }

            if( NewNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.InspectionDesignClass )
                OnMakeNewInspectionDesignNodeType( NewNodeType, IsCopy );
        }

        public void OnDeleteNodeTypeProp( CswNbtMetaDataNodeTypeProp DeletedProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( DeletedProp, CswEnumNbtPropAction.Delete );
        }
        public void OnEditNodeTypePropName( CswNbtMetaDataNodeTypeProp EditedProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( EditedProp, CswEnumNbtPropAction.Edit );
        }
        public void OnEditNodeTypeName( CswNbtMetaDataNodeType EditedNodeType )
        {
            if( EditedNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.InspectionDesignClass )
                OnUpdateInspectionDesignNodeType( EditedNodeType );

        }
        public void OnMakeNewNodeTypeProp( CswNbtMetaDataNodeTypeProp NewProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( NewProp, CswEnumNbtPropAction.Add );
        }

        // Some ObjectClass specific behavior:  
        // Perhaps this should live in the ObjClass...

        // If adding a property to a nodetype of class equipment or equipmentassembly, 
        // or editing a property on a nodetype of class equipment or equipmentassembly,
        // if there is a matching property of the same propname and fieldtype on the related nodetype or objectclass, 
        // set all equipment nodes pendingupdate = 1 (see BZ 5964)

        public void UpdateEquipmentAssemblyMatchingProperties( CswNbtMetaDataNodeTypeProp EditedProp, CswEnumNbtPropAction Action )
        {
            CswEnumNbtObjectClass EditedPropObjectClass = _CswNbtResources.MetaData.getObjectClassByNodeTypeId( EditedProp.NodeTypeId ).ObjectClass;
            if( EditedPropObjectClass == CswEnumNbtObjectClass.EquipmentClass )
            {
                if( Action != CswEnumNbtPropAction.Delete )
                {
                    CswNbtMetaDataNodeType EquipmentNodeType = EditedProp.getNodeType();
                    CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly );
                    if( RelationshipProp != null )
                    {
                        // We have to update all these nodes always, not just when there's a prop name 
                        // that matches, in case we renamed a prop and it no longer matches.

                        // We do this directly, not using a view, for performance
                        CswTableUpdate NodesTableUpdate = _CswNbtResources.makeCswTableUpdate( "nodes_pendingupdate_update", "nodes" );
                        DataTable NodesTable = NodesTableUpdate.getTable( "nodetypeid", EquipmentNodeType.NodeTypeId );
                        foreach( DataRow NodesRow in NodesTable.Rows )
                        {
                            NodesRow["pendingupdate"] = "1";
                        }
                        NodesTableUpdate.update( NodesTable );
                    }
                }
            }
            else if( EditedPropObjectClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
            {
                CswNbtMetaDataNodeType AssemblyNodeType = EditedProp.getNodeType();
                CswNbtMetaDataObjectClass EquipmentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
                foreach( CswNbtMetaDataNodeType EquipmentNodeType in EquipmentOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly );
                    if( RelationshipProp != null )
                    {
                        //if( ( RelationshipProp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                        //      RelationshipProp.FKValue == AssemblyNodeType.NodeTypeId ) ||
                        //    ( RelationshipProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                        //      RelationshipProp.FKValue == AssemblyNodeType.ObjectClassId ) ||
                        //    ( RelationshipProp.FKType == NbtViewRelatedIdType.PropertySetId.ToString() &&
                        //      null != AssemblyNodeType.getObjectClass().getPropertySet() &&
                        //      RelationshipProp.FKValue == AssemblyNodeType.getObjectClass().getPropertySet().PropertySetId ) )
                        if( RelationshipProp.FkMatches( AssemblyNodeType ) )
                        {
                            // There is a matching property on the assembly.  Mark all nodes of this nodetype as pendingupdate
                            // We have to update all these nodes always, not just when there's a prop name 
                            // that matches, in case we renamed a prop and it no longer matches.
                            CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "UpdateEquipmentAssemblyMatchingProperties_nodespendingupdate_update", "nodes" );
                            DataTable NodesTable = NodesUpdate.getTable( "nodetypeid", EquipmentNodeType.NodeTypeId );
                            foreach( DataRow NodesRow in NodesTable.Rows )
                            {
                                NodesRow["pendingupdate"] = "1";
                            }
                            NodesUpdate.update( NodesTable );
                        }
                    } // if( RelationshipProp != null )
                } // foreach( CswNbtMetaDataNodeType EquipmentNodeType in EquipmentOC.NodeTypes )
            } // else if( EditedProp.NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentAssemblyClass )
        } // UpdateEquipmentAssemblyMatchingProperties()

        public void OnMakeNewInspectionDesignNodeType( CswNbtMetaDataNodeType NewNodeType, bool IsCopy )
        {
            CswNbtMetaDataNodeTypeProp NameProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );
            Int32 DatePropId = NewNodeType.getNodeTypePropIdByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate );

            // Set 'Name' default value = nodetypename
            NameProp.DefaultValue.AsText.Text = NewNodeType.NodeTypeName;

            // The following changes for new forms only
            if( NewNodeType.VersionNo == 1 && !IsCopy )
            {
                // Set nametemplate = Name + Date
                NewNodeType.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( NameProp.FirstPropVersionId.ToString() ) + " " + CswNbtMetaData.MakeTemplateEntry( DatePropId.ToString() );

                // Set first tab to be "Details"
                CswNbtMetaDataNodeTypeTab FirstTab = NewNodeType.getNodeTypeTab( NewNodeType.NodeTypeName );
                if( null != FirstTab )
                {
                    FirstTab = NewNodeType.getSecondNodeTypeTab();
                    FirstTab.TabName = "Details";
                    FirstTab.TabOrder = 10;
                    FirstTab.IncludeInNodeReport = false;
                }

                // case 20951 - Add an Action tab
                CswNbtMetaDataNodeTypeTab ActionTab = NewNodeType.getNodeTypeTab( "Action" );
                if( ActionTab == null )
                {
                    ActionTab = _CswNbtResources.MetaData.makeNewTab( NewNodeType, "Action", 9 );
                }

                CswNbtMetaDataNodeTypeProp SetPreferredProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.SetPreferred );
                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, SetPreferredProp, true, ActionTab.TabId, 1, 1 );

                CswNbtMetaDataNodeTypeProp FinishedProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Finish );
                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, FinishedProp, true, ActionTab.TabId, 2, 1 );

                CswNbtMetaDataNodeTypeProp CancelledProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Cancel );
                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, CancelledProp, true, ActionTab.TabId, 3, 1 );

                CswNbtMetaDataNodeTypeProp CancelReasonProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.CancelReason );
                //CancelReasonProp.updateLayout( CswEnumNbtLayoutType.Edit, ActionTab.TabId, 3, 1 );
                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, CancelReasonProp, true, ActionTab.TabId, 4, 1 );

            } // if( NewNodeType.VersionNo == 1 && !IsCopy )
        } // OnMakeNewInspectionDesignNodeType()

        public void OnUpdateInspectionDesignNodeType( CswNbtMetaDataNodeType NodeType )
        {
            CswNbtMetaDataNodeTypeProp NameProp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );

            // Set 'Name' default value = nodetypename
            NameProp.DefaultValue.AsText.Text = NodeType.NodeTypeName;
        }
    }
}
