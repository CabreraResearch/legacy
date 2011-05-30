using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
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
            OnMakeNewNodeType( CopyNodeType );
        }

        public void OnMakeNewNodeType( CswNbtMetaDataNodeType NewNodeType )
        {
            // Give the current user's role full permissions to the new nodetype
            if( null != _CswNbtResources.CurrentNbtUser.RoleNode )
            {
				//CswNbtNodePropLogicalSet PropPermissions = ( (CswNbtObjClassRole) _CswNbtResources.CurrentNbtUser.RoleNode ).NodeTypePermissions;
				//PropPermissions.SetValue( NodeTypePermission.Delete.ToString(), NewNodeType.NodeTypeId.ToString(), true );
				//PropPermissions.SetValue( NodeTypePermission.Create.ToString(), NewNodeType.NodeTypeId.ToString(), true );
				//PropPermissions.SetValue( NodeTypePermission.Edit.ToString(), NewNodeType.NodeTypeId.ToString(), true );
				//PropPermissions.SetValue( NodeTypePermission.View.ToString(), NewNodeType.NodeTypeId.ToString(), true );
				//PropPermissions.Save();
				//_CswNbtResources.CurrentNbtUser.RoleNode.postChanges( false );

				_CswNbtResources.Permit.set( new CswNbtPermit.NodeTypePermission[] {
												CswNbtPermit.NodeTypePermission.Delete, 
												CswNbtPermit.NodeTypePermission.Create, 
												CswNbtPermit.NodeTypePermission.Edit, 
												CswNbtPermit.NodeTypePermission.View }, 
											NewNodeType, 
											_CswNbtResources.CurrentNbtUser.RoleNode,
											true );

			}//if we have a current user

            if( NewNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                OnMakeNewInspectionDesignNodeType( NewNodeType );
        }


        public enum NbtPropAction { Add, Edit, Delete };

        public void OnDeleteNodeTypeProp( CswNbtMetaDataNodeTypeProp DeletedProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( DeletedProp, NbtPropAction.Delete );
        }
        public void OnEditNodeTypePropName( CswNbtMetaDataNodeTypeProp EditedProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( EditedProp, NbtPropAction.Edit );
        }
        public void OnEditNodeTypeName( CswNbtMetaDataNodeType EditedNodeType )
        {
            if( EditedNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                OnUpdateInspectionDesignNodeType( EditedNodeType );

        }
        public void OnMakeNewNodeTypeProp( CswNbtMetaDataNodeTypeProp NewProp )
        {
            UpdateEquipmentAssemblyMatchingProperties( NewProp, NbtPropAction.Add );
        }


        // Some ObjectClass specific behavior:  
        // Perhaps this should live in the ObjClass...

        // If adding a property to a nodetype of class equipment or equipmentassembly, 
        // or editing a property on a nodetype of class equipment or equipmentassembly,
        // if there is a matching property of the same propname and fieldtype on the related nodetype or objectclass, 
        // set all equipment nodes pendingupdate = 1 (see BZ 5964)

        public void UpdateEquipmentAssemblyMatchingProperties( CswNbtMetaDataNodeTypeProp EditedProp, NbtPropAction Action )
        {
            if( EditedProp.NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass )
            {
                if( Action != NbtPropAction.Delete )
                {
                    CswNbtMetaDataNodeType EquipmentNodeType = EditedProp.NodeType;
                    //CswNbtObjClassRuleEquipment EquipmentRule = new CswNbtObjClassRuleEquipment();
                    CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.AssemblyPropertyName );
                    if( RelationshipProp != null )
                    {
                        //if (RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString())
                        //{
                        //CswNbtMetaDataNodeType AssemblyNodeType = this.getNodeType(RelationshipProp.FKValue);
                        //CswNbtMetaDataNodeTypeProp AssemblyNodeTypeProp = AssemblyNodeType.getNodeTypeProp(EditedProp.PropName);
                        //if (AssemblyNodeTypeProp != null && AssemblyNodeTypeProp.FieldType == EditedProp.FieldType)
                        //{
                        //// There is a matching property on the assembly.  Mark all nodes of this nodetype as pendingupdate
                        //CswNbtView EquipView = _CswNbtResources.Trees.getTreeViewOfNodeType(EditedNodeType.NodeTypeId);
                        //ICswNbtTree EquipNodesTree = _CswNbtResources.Trees.getTreeFromView(EquipView);
                        //EquipTree.goToRoot();
                        //if (EquipTree.getChildNodeCount() > 0)  // should always be the case
                        //{
                        //    EquipTree.goToNthChild(0);
                        //    if (EquipTree.getChildNodeCount() > 0)   // might not always be the case
                        //    {
                        //        for (int i = 0; i < EquipTree.getChildNodeCount(); i++)
                        //        {
                        //            EquipTree.goToNthChild(i);
                        //            CswNbtNode EquipNode = EquipTree.getNodeForCurrentPosition();
                        //            ((CswNbtNodePropWrapper)EquipNode.Properties[EditedProp]).PendingUpdate = true;
                        //        }
                        //    }
                        //}


                        // We have to update all these nodes always, not just when there's a prop name 
                        // that matches, in case we renamed a prop and it no longer matches.

                        // We do this directly, not using a view, for performance
                        CswTableUpdate NodesTableUpdate = _CswNbtResources.makeCswTableUpdate( "nodes_pendingupdate_update", "nodes" );
                        DataTable NodesTable = NodesTableUpdate.getTable("nodetypeid",EquipmentNodeType.NodeTypeId);
                        foreach( DataRow NodesRow in NodesTable.Rows )
                        {
                            NodesRow["pendingupdate"] = "1";
                        }
                        NodesTableUpdate.update( NodesTable );

                        //}
                        //}
                        //else if (RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString())
                        //{
                        //    CswNbtMetaDataObjectClass AssemblyObjectClass = this.getObjectClass(RelationshipProp.FKValue);
                        //    CswNbtMetaDataObjectClassProp AssemblyObjectClassProp = AssemblyObjectClass.getObjectClassProp(EditedProp.PropName);

                        //    // BZ 5528
                        //    // There's a flaw here.  If the relationship is object class based, and there are no matching object class props,
                        //    // but the nodetype of the node that is the actual target of a relationship has matching nodetype props, then the
                        //    // assembly prop updating logic will occur, but the props on the nodetype won't be readonly.
                        //    if (AssemblyObjectClassProp != null && AssemblyObjectClassProp.FieldType == EditedProp.FieldType)
                        //    {
                        //        // There is a matching property on the assembly.  Mark all nodes of this nodetype as pendingupdate
                        //        CswTableCaddy NodesTableCaddy = _CswNbtResources.makeCswTableCaddy("nodes");
                        //        NodesTableCaddy.FilterColumn = "nodetypeid";
                        //        DataTable NodesTable = NodesTableCaddy[EquipmentNodeType.NodeTypeId].Table;
                        //        foreach (DataRow NodesRow in NodesTable.Rows)
                        //        {
                        //            NodesRow["pendingupdate"] = "1";
                        //        }
                        //        NodesTableCaddy.update(NodesTable);

                        //    }
                        //}
                    }
                }
            }
            else if( EditedProp.NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass )
            {
                CswNbtMetaDataNodeType AssemblyNodeType = EditedProp.NodeType;

                foreach( CswNbtMetaDataNodeType EquipmentNodeType in _CswNbtResources.MetaData.NodeTypes )
                {
                    if( EquipmentNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass )
                    {
                        //CswNbtObjClassRuleEquipment EquipmentRule = new CswNbtObjClassRuleEquipment(); 
                        CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.AssemblyPropertyName );
                        if( RelationshipProp != null )
                        {
                            if( ( RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                                  RelationshipProp.FKValue == AssemblyNodeType.NodeTypeId ) ||
                                ( RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                                  RelationshipProp.FKValue == AssemblyNodeType.ObjectClass.ObjectClassId ) )
                            {

                                //CswNbtMetaDataNodeTypeProp EquipmentNodeTypeProp = EquipmentNodeType.getNodeTypeProp(EditedProp.PropName);
                                //if (EquipmentNodeTypeProp != null && EquipmentNodeTypeProp.FieldType == EditedProp.FieldType)
                                //{
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
                                //}
                            }
                            //else if( RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                            //         RelationshipProp.FKValue == AssemblyNodeType.ObjectClass.ObjectClassId )
                                //{
                            //    // BZ 5528
                            //    // A variation of the same flaw is here -- in this case, the problem is that we don't know
                            //    // if a nodetype is related to this Assembly nodetype if the relationship is object class based, and
                            //    // thus we don't know whether the property should be readonly, unless the prop is an object class prop.
                            //    //CswNbtMetaDataObjectClassProp EquipmentObjectClassProp = EquipmentNodeType.ObjectClass.getObjectClassProp(EditedProp.PropName);
                            //    //if (EquipmentObjectClassProp != null && EquipmentObjectClassProp.FieldType == EditedProp.FieldType)
                            //    //{
                            //    // There is a matching property on the assembly.  Mark all nodes of all nodetypes of this class as pendingupdate
                            //    CswTableCaddy NodesTableCaddy = _CswNbtResources.makeCswTableCaddy( "nodes" );
                            //    DataTable NodesTable = NodesTableCaddy.getTable( "nodetypeid", EquipmentNodeType.NodeTypeId );
                            //    foreach( DataRow NodesRow in NodesTable.Rows )
                            //    {
                            //        NodesRow["pendingupdate"] = "1";
                            //    }
                            //    NodesTableCaddy.update( NodesTable );
                            //    //}
                                //}
                            }
                        }
                    }
                }
            }

        public void OnMakeNewInspectionDesignNodeType( CswNbtMetaDataNodeType NewNodeType )
        {
            CswNbtMetaDataNodeTypeProp NameProp = NewNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataNodeTypeProp DateProp = NewNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );

            // Set 'Name' default value = nodetypename
            NameProp.DefaultValue.AsText.Text = NewNodeType.NodeTypeName;
            
            // The following changes for new forms only
            if( NewNodeType.VersionNo == 1 ) 
            {
                // Set nametemplate = Name + Date
                NewNodeType.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( NameProp.PropName.ToString() ) + " " + CswNbtMetaData.MakeTemplateEntry( DateProp.PropName.ToString() );

                // Set first tab to be "Details"
                CswNbtMetaDataNodeTypeTab FirstTab = NewNodeType.getFirstNodeTypeTab();
                FirstTab.TabName = "Details";
                FirstTab.TabOrder = 10;
                FirstTab.IncludeInNodeReport = false;

                // case 20951 - Add an Action tab
                CswNbtMetaDataNodeTypeTab ActionTab = _CswNbtResources.MetaData.makeNewTab( NewNodeType, "Action", 9 );

                CswNbtMetaDataNodeTypeProp FinishedProp = NewNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
                FinishedProp.NodeTypeTab = ActionTab;
                FinishedProp.DisplayRow = 1;
                FinishedProp.DisplayColumn = 1;

                CswNbtMetaDataNodeTypeProp CancelledProp = NewNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelledPropertyName );
                CancelledProp.NodeTypeTab = ActionTab;
                CancelledProp.DisplayRow = 2;
                CancelledProp.DisplayColumn = 1;

                CswNbtMetaDataNodeTypeProp CancelReasonProp = NewNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelReasonPropertyName );
                CancelReasonProp.NodeTypeTab = ActionTab;
                CancelReasonProp.DisplayRow = 3;  // even though webapp interprets this independently, Mobile needs this to be 3
                CancelReasonProp.DisplayColumn = 1;


                // Add a "Section 1" tab
                _CswNbtResources.MetaData.makeNewTab( NewNodeType, "Section 1", 1 );
            }
        }

        public void OnUpdateInspectionDesignNodeType( CswNbtMetaDataNodeType NodeType )
        {
            CswNbtMetaDataNodeTypeProp NameProp = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );

            // Set 'Name' default value = nodetypename
            NameProp.DefaultValue.AsText.Text = NodeType.NodeTypeName;
        }
    }
}
