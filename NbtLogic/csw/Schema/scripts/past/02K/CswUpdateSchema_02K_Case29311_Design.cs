﻿using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02K_Case29311_Design : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override string AppendToScriptName()
        {
            return "Design";
        }

        public override void update()
        {
            // Clear defunct existing rows from jct_dd_ntp
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jctddntp_update", "jct_dd_ntp" );
            DataTable jctClearTable = jctUpdate.getTable();
            foreach( DataRow jctRow in jctClearTable.Rows )
            {
                jctRow.Delete();
            }
            jctUpdate.update( jctClearTable );


            // Create new Super-MetaData Design nodetypes
            CswNbtMetaDataObjectClass NodeTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
            CswNbtMetaDataObjectClass NodeTypeTabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataObjectClass NodeTypePropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );

            CswNbtObjClassRole ChemSWAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            CswEnumNbtNodeTypePermission[] AllPerms = new CswEnumNbtNodeTypePermission[]
                {
                    CswEnumNbtNodeTypePermission.Create, CswEnumNbtNodeTypePermission.Delete, CswEnumNbtNodeTypePermission.Edit, CswEnumNbtNodeTypePermission.View
                };


            CswNbtMetaDataNodeType NodeTypeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( NodeTypeOC )
                {
                    NodeTypeName = "Design NodeType",
                    Category = "Design"
                } );
            //NodeTypeNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName ) );
            NodeTypeNT._DataRow["nametemplate"] = CswNbtMetaData.TemplateTextToTemplateValue( NodeTypeNT.getNodeTypeProps(), CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName ) );

            _CswNbtSchemaModTrnsctn.Permit.set( AllPerms, NodeTypeNT, ChemSWAdminRole, true );

            CswNbtMetaDataNodeType NodeTypeTabNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( NodeTypeTabOC )
            {
                NodeTypeName = "Design NodeTypeTab",
                Category = "Design"
            } );
            //NodeTypeTabNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue ) + ": " +
            //                                   CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName ) + " Tab" );
            NodeTypeTabNT._DataRow["nametemplate"] = CswNbtMetaData.TemplateTextToTemplateValue( NodeTypeTabNT.getNodeTypeProps(), CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue ) + ": " +
                                                                                                                             CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName ) + " Tab" );

            _CswNbtSchemaModTrnsctn.Permit.set( AllPerms, NodeTypeTabNT, ChemSWAdminRole, true );

            foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )
            {
                CswNbtMetaDataNodeType NodeTypePropNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( NodeTypePropOC )
                    {
                        NodeTypeName = CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( FieldType.FieldType ),
                        Category = "Design"
                    } );
                //NodeTypePropNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue ) + ": " +
                //                                    CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName ) + " Prop" );
                NodeTypePropNT._DataRow["nametemplate"] = CswNbtMetaData.TemplateTextToTemplateValue( NodeTypePropNT.getNodeTypeProps(), CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue ) + ": " +
                                                                                                                                 CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName ) + " Prop" );
                _CswNbtSchemaModTrnsctn.Permit.set( AllPerms, NodeTypePropNT, ChemSWAdminRole, true );
            } // foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )


            CswNbtMetaDataNodeTypeProp NTAuditLevelNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.AuditLevel );
            CswNbtMetaDataNodeTypeProp NTCategoryNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Category );
            CswNbtMetaDataNodeTypeProp NTDeferSearchToNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo );
            CswNbtMetaDataNodeTypeProp NTIconFileNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.IconFileName );
            CswNbtMetaDataNodeTypeProp NTLockedNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Locked );
            CswNbtMetaDataNodeTypeProp NTEnabledNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Enabled );
            CswNbtMetaDataNodeTypeProp NTNameTemplateTextNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateText );
            CswNbtMetaDataNodeTypeProp NTNameTemplateValueNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateValue );
            CswNbtMetaDataNodeTypeProp NTNameTemplateAddNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd );
            CswNbtMetaDataNodeTypeProp NTNodeTypeNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName );
            //CswNbtMetaDataNodeTypeProp NTObjectClassNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName );
            CswNbtMetaDataNodeTypeProp NTObjectClassValueNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClass );
            CswNbtMetaDataNodeTypeProp NTSearchableNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Searchable );

            CswNbtMetaDataNodeTypeProp NTTNodeTypeNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue );
            CswNbtMetaDataNodeTypeProp NTTOrderNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.Order );
            CswNbtMetaDataNodeTypeProp NTTIncludeInReportNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.IncludeInReport );
            CswNbtMetaDataNodeTypeProp NTTServerManagedNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.ServerManaged );
            CswNbtMetaDataNodeTypeProp NTTTabNameNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName );

            CswNbtMetaDataObjectClassProp NTTNodeTypeOCP = NodeTypeTabOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue );

            CswNbtMetaDataObjectClassProp NTPNodeTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
            CswNbtMetaDataObjectClassProp NTPPropNameOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
            CswNbtMetaDataObjectClassProp NTPObjectClassPropNameOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName );
            CswNbtMetaDataObjectClassProp NTPFieldTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );




            Dictionary<Int32, CswNbtObjClassDesignNodeType> NTNodes = new Dictionary<Int32, CswNbtObjClassDesignNodeType>();

            DataTable jctTable = jctUpdate.getEmptyTable();

            // NodeTypeNT Props
            {
                Int32 TabId = NodeTypeNT.getFirstNodeTypeTab().TabId;

                // Set view for NameTemplateAddNTP
                CswNbtView NameView = _CswNbtSchemaModTrnsctn.restoreView( NTNameTemplateAddNTP.ViewId );
                NameView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship NameViewRel1 = NameView.AddViewRelationship( NodeTypeNT, false );
                CswNbtViewRelationship NameViewRel2 = NameView.AddViewRelationship( NameViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                NameView.save();

                // DeferSearchTo is conditional on Searchable
                NTDeferSearchToNTP.setFilterDeprecated( NTSearchableNTP, NTSearchableNTP.getFieldTypeRule().SubFields[CswNbtFieldTypeRuleLogical.SubFieldName.Checked], CswEnumNbtFilterMode.Equals, CswEnumTristate.True );

                // Set view for DeferSearchToNTP
                //CswNbtView DeferView = _CswNbtSchemaModTrnsctn.restoreView( NTDeferSearchToNTP.ViewId );
                //DeferView.Root.ChildRelationships.Clear();
                //CswNbtViewRelationship DeferViewRel1 = DeferView.AddViewRelationship( NodeTypeNT, false );
                //CswNbtViewRelationship DeferViewRel2 = DeferView.AddViewRelationship( DeferViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                //DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeOCP,
                //                                    FilterMode: CswEnumNbtFilterMode.Equals,
                //                                    SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                //                                    Value: _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Relationship ).FieldTypeId.ToString() );
                //DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeOCP,
                //                                    Conjunction: CswEnumNbtFilterConjunction.Or,
                //                                    FilterMode: CswEnumNbtFilterMode.Equals,
                //                                    SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                //                                    Value: _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Location ).FieldTypeId.ToString() );
                //DeferView.save();

                // Add Properties and Tabs grids
                CswNbtMetaDataFieldType GridFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Grid );

                CswNbtMetaDataNodeTypeTab TabsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTabDeprecated( NodeTypeNT, "Tabs", 2 );
                CswNbtMetaDataNodeTypeTab PropertiesTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTabDeprecated( NodeTypeNT, "Properties", 3 );

                CswNbtMetaDataNodeTypeProp NTTabsGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropDeprecated( new CswNbtWcfMetaDataModel.NodeTypeProp( NodeTypeNT, GridFT, "Tabs Grid" ) );
                CswNbtMetaDataNodeTypeProp NTPropsGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropDeprecated( new CswNbtWcfMetaDataModel.NodeTypeProp( NodeTypeNT, GridFT, "Properties Grid" ) );
                {
                    CswNbtView TabsView = _CswNbtSchemaModTrnsctn.restoreView( NTTabsGridNTP.ViewId );
                    TabsView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship ntrel = TabsView.AddViewRelationship( NodeTypeNT, false );
                    CswNbtViewRelationship tabrel = TabsView.AddViewRelationship( ntrel, CswEnumNbtViewPropOwnerType.Second, NTTNodeTypeOCP, false );
                    TabsView.AddViewProperty( tabrel, NTTTabNameNTP, 1 );
                    TabsView.AddViewProperty( tabrel, NTTOrderNTP, 2 );
                    TabsView.save();
                }
                {
                    CswNbtView PropsView = _CswNbtSchemaModTrnsctn.restoreView( NTPropsGridNTP.ViewId );
                    PropsView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship ntrel = PropsView.AddViewRelationship( NodeTypeNT, false );
                    CswNbtViewRelationship proprel = PropsView.AddViewRelationship( ntrel, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                    PropsView.AddViewProperty( proprel, NTPPropNameOCP, 1 );
                    PropsView.AddViewProperty( proprel, NTPObjectClassPropNameOCP, 2 );
                    PropsView.AddViewProperty( proprel, NTPFieldTypeOCP, 3 );
                    PropsView.save();
                }

                // Edit Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                NTNameTemplateTextNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 2 );
                NTNameTemplateAddNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 2 );
                NTAuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
                NTSearchableNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
                NTDeferSearchToNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );
                NTLockedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 9, DisplayColumn: 1 );

                NTTabsGridNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabsTab.TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTPropsGridNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, PropertiesTab.TabId, DisplayRow: 1, DisplayColumn: 1 );

                // Add Layout
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                NTAuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTSearchableNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTDeferSearchToNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTLockedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateTextNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateAddNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTTabsGridNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTPropsGridNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Table Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

                // Preview Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );

                NTNameTemplateValueNTP.removeFromAllLayouts();

                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetypes table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    CswNbtObjClassDesignNodeType node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeNT.NodeTypeId, OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassDesignNodeType NewNTNode = NewNode;
                            NewNTNode.AuditLevel.Value = thisNodeType.AuditLevel;
                            NewNTNode.Category.Text = thisNodeType.Category;
                            //NewNTNode.DeferSearchTo.RelatedNodeId = thisNodeType.SearchDeferPropId;
                            NewNTNode.IconFileName.Value = new CswCommaDelimitedString() { thisNodeType.IconFileName };
                            NewNTNode.Locked.Checked = CswConvert.ToTristate( thisNodeType.IsLocked );
                            NewNTNode.Enabled.Checked = CswConvert.ToTristate( thisNodeType.Enabled );
                            NewNTNode.NameTemplateText.Text = thisNodeType.getNameTemplateText();
                            NewNTNode.NodeTypeName.Text = CswFormat.MakeIntoValidName( thisNodeType.NodeTypeName );
                            NewNTNode.NodeTypeName.makeUnique();
                            //NewNTNode.ObjectClassName.Text = thisNodeType.getObjectClass().ObjectClass.ToString();
                            NewNTNode.ObjectClassProperty.Value = thisNodeType.ObjectClassId.ToString();
                            NewNTNode.Searchable.Checked = thisNodeType.Searchable;  //CswNbtMetaDataObjectClass.NotSearchableValue
                        } );
                    node.RelationalId = new CswPrimaryKey( "nodetypes", thisNodeType.NodeTypeId );
                    //node.postChanges( false );
                    ICswNbtNodePersistStrategy NodePersistStrategy = new CswNbtNodePersistStrategyUpdate
                    {
                        OverrideUniqueValidation = true,
                        OverrideMailReportEvents = true,
                        Creating = true
                    };
                    NodePersistStrategy.postChanges( node.Node );

                    NTNodes.Add( thisNodeType.NodeTypeId, node );
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeNT._DataRow["tablename"] = "nodetypes";

                _addJctRow( jctTable, NTNodeTypeNameNTP, NodeTypeNT.TableName, "nodetypename" );
                _addJctRow( jctTable, NTObjectClassValueNTP, NodeTypeNT.TableName, "objectclassid" );
                _addJctRow( jctTable, NTCategoryNTP, NodeTypeNT.TableName, "category" );
                _addJctRow( jctTable, NTIconFileNameNTP, NodeTypeNT.TableName, "iconfilename" );
                _addJctRow( jctTable, NTNameTemplateValueNTP, NodeTypeNT.TableName, "nametemplate" );
                _addJctRow( jctTable, NTAuditLevelNTP, NodeTypeNT.TableName, "auditlevel" );
                _addJctRow( jctTable, NTSearchableNTP, NodeTypeNT.TableName, "searchable" );
                _addJctRow( jctTable, NTDeferSearchToNTP, NodeTypeNT.TableName, "searchdeferpropid", CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
                _addJctRow( jctTable, NTLockedNTP, NodeTypeNT.TableName, "islocked" );
                _addJctRow( jctTable, NTEnabledNTP, NodeTypeNT.TableName, "enabled" );
            }


            // NodeTypeTabNT Props
            {
                Int32 TabId = NodeTypeTabNT.getFirstNodeTypeTab().TabId;

                // Edit Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                NTTOrderNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                NTTIncludeInReportNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                NTTServerManagedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 1 );

                // Add Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                NTTOrderNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                NTTIncludeInReportNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTTServerManagedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Table Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

                // Preview Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );

                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_tabset table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeTab thisTab in thisNodeType.getNodeTypeTabs() )
                    {
                        CswNbtObjClassDesignNodeTypeTab node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeTabNT.NodeTypeId, OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                            {
                                CswNbtObjClassDesignNodeTypeTab NewNTTNode = NewNode;
                                NewNTTNode.IncludeInReport.Checked = CswConvert.ToTristate( thisTab.IncludeInNodeReport );
                                NewNTTNode.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                                NewNTTNode.Order.Value = thisTab.TabOrder;
                                NewNTTNode.TabName.Text = thisTab.TabName;
                                NewNTTNode.ServerManaged.Checked = CswConvert.ToTristate( thisTab.ServerManaged );
                            } );
                        node.RelationalId = new CswPrimaryKey( "nodetype_tabset", thisTab.TabId );
                        //node.postChanges( false );
                        ICswNbtNodePersistStrategy NodePersistStrategy = new CswNbtNodePersistStrategyUpdate
                        {
                            OverrideUniqueValidation = true,
                            OverrideMailReportEvents = true,
                            Creating = true
                        };
                        NodePersistStrategy.postChanges( node.Node );
                    }
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeTabNT._DataRow["tablename"] = "nodetype_tabset";

                _addJctRow( jctTable, NTTIncludeInReportNTP, NodeTypeTabNT.TableName, "includeinnodereport" );
                _addJctRow( jctTable, NTTNodeTypeNTP, NodeTypeTabNT.TableName, "nodetypeid", CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
                _addJctRow( jctTable, NTTOrderNTP, NodeTypeTabNT.TableName, "taborder" );
                _addJctRow( jctTable, NTTServerManagedNTP, NodeTypeTabNT.TableName, "servermanaged" );
                _addJctRow( jctTable, NTTTabNameNTP, NodeTypeTabNT.TableName, "tabname" );
            }



            // NodeTypePropNT Props
            {
                Dictionary<CswEnumNbtFieldType, CswNbtMetaDataNodeType> propNTDict = new Dictionary<CswEnumNbtFieldType, CswNbtMetaDataNodeType>();

                foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )
                {
                    CswNbtMetaDataNodeType NodeTypePropNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( FieldType.FieldType ) );

                    Int32 TabId = NodeTypePropNT.getFirstNodeTypeTab().TabId;

                    CswNbtMetaDataNodeTypeProp NTPAuditLevelNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel );
                    CswNbtMetaDataNodeTypeProp NTPCompoundUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionFilterNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilterMode );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionPropertyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionSubfieldNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue );
                    CswNbtMetaDataNodeTypeProp NTPFieldTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );
                    CswNbtMetaDataNodeTypeProp NTPHelpTextNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText );
                    CswNbtMetaDataNodeTypeProp NTPNodeTypeValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
                    CswNbtMetaDataNodeTypeProp NTPObjectClassPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName );
                    CswNbtMetaDataNodeTypeProp NTPPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
                    CswNbtMetaDataNodeTypeProp NTPReadOnlyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly );
                    CswNbtMetaDataNodeTypeProp NTPRequiredNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Required );
                    CswNbtMetaDataNodeTypeProp NTPServerManagedNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ServerManaged );
                    CswNbtMetaDataNodeTypeProp NTPUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique );
                    CswNbtMetaDataNodeTypeProp NTPUseNumberingNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering );
                    CswNbtMetaDataNodeTypeProp NTPQuestionNoNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.QuestionNo );
                    CswNbtMetaDataNodeTypeProp NTPSubQuestionNoNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.SubQuestionNo );

                    // Edit layout
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                    NTPDisplayConditionPropertyNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 1 );
                    NTPDisplayConditionSubfieldNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
                    NTPDisplayConditionFilterNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
                    NTPDisplayConditionValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );
                    NTPRequiredNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 9, DisplayColumn: 1 );
                    NTPServerManagedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 10, DisplayColumn: 1 );
                    NTPUniqueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 11, DisplayColumn: 1 );
                    NTPCompoundUniqueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 12, DisplayColumn: 1 );
                    NTPReadOnlyNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 13, DisplayColumn: 1 );
                    NTPUseNumberingNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 14, DisplayColumn: 1 );
                    NTPQuestionNoNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 15, DisplayColumn: 1 );
                    NTPSubQuestionNoNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 16, DisplayColumn: 1 );
                    NTPHelpTextNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 17, DisplayColumn: 1 );
                    NTPAuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 18, DisplayColumn: 1 );

                    // Add layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPRequiredNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                    NTPServerManagedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPAuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPCompoundUniqueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionFilterNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionPropertyNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionSubfieldNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionValueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPHelpTextNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPObjectClassPropNameNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPReadOnlyNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPUniqueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPUseNumberingNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPQuestionNoNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPSubQuestionNoNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                    // Table layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );

                    // Preview layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 1 );


                    // Set default value of "Field Type" to this fieldtype
                    NTPFieldTypeNTP.getDefaultValue( true, true ).AsList.Value = FieldType.FieldTypeId.ToString();
                    NTPFieldTypeNTP.getDefaultValue( true, true ).AsList.Text = FieldType.FieldType.ToString();
                    NTPFieldTypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );


                    ICswNbtFieldTypeRule Rule = FieldType.getFieldTypeRule();

                    // Make all the attribute properties
                    foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                    {
                        _makePropNTP( NodeTypePropNT, TabId, Attr );
                    }

                    // Handle special configurations
                    switch( FieldType.FieldType )
                    {
                        case CswEnumNbtFieldType.Barcode:
                            CswNbtMetaDataNodeTypeProp barcodeSequenceNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleBarCode.AttributeName.Sequence.ToString() );
                            barcodeSequenceNTP._DataRow["isrequired"] = CswConvert.ToDbVal( true );
                            break;

                        case CswEnumNbtFieldType.ChildContents:
                            CswNbtMetaDataNodeTypeProp ccisFkNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleChildContents.AttributeName.IsFK.ToString() );
                            ccisFkNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            ccisFkNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp ccfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleChildContents.AttributeName.FKType.ToString() );
                            ccfktypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            ccfktypeNTP.removeFromAllLayouts();
                            break;

                        case CswEnumNbtFieldType.Composite:
                            //CswNbtMetaDataNodeTypeProp addTemplateNTP = _makePropNTP( NodeTypePropNT, TabId, CswEnumNbtFieldType.Relationship, CswNbtFieldTypeRuleComposite.AttributeName.AddToTemplate, "" );
                            CswNbtMetaDataNodeTypeProp addTemplateNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleComposite.AttributeName.AddToTemplate.ToString() );
                            addTemplateNTP.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );
                            break;

                        case CswEnumNbtFieldType.DateTime:
                            CswNbtMetaDataNodeTypeProp defaultToTodayNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DefaultToToday.ToString() );
                            defaultToTodayNTP._DataRow["isrequired"] = CswConvert.ToDbVal( true );

                            CswNbtMetaDataNodeTypeProp dateTypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DateType.ToString() );
                            dateTypeNTP._DataRow["listoptions"] = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtDateDisplayMode.Date.ToString(),
                                    CswEnumNbtDateDisplayMode.Time.ToString(),
                                    CswEnumNbtDateDisplayMode.DateTime.ToString()
                                }.ToString();
                            break;

                        case CswEnumNbtFieldType.Grid:
                            CswNbtMetaDataNodeTypeProp displaymodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DisplayMode.ToString() );
                            displaymodeNTP._DataRow["listoptions"] = new CswCommaDelimitedString()
                                    {
                                        CswEnumNbtGridPropMode.Full.ToString(),
                                        CswEnumNbtGridPropMode.Small.ToString(),
                                        CswEnumNbtGridPropMode.Link.ToString(),
                                    }.ToString();

                            CswNbtMetaDataNodeTypeProp maxrowsNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.MaximumRows.ToString() );
                            maxrowsNTP.setFilterDeprecated( displaymodeNTP, displaymodeNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumNbtGridPropMode.Small.ToString() );

                            CswNbtMetaDataNodeTypeProp showheadersNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ShowHeaders.ToString() );
                            showheadersNTP.setFilterDeprecated( displaymodeNTP, displaymodeNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumNbtGridPropMode.Small.ToString() );
                            break;

                        case CswEnumNbtFieldType.ImageList:
                            CswNbtMetaDataNodeTypeProp imagenamesNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ImageNames.ToString() );
                            imagenamesNTP._DataRow["textarearows"] = 5;
                            imagenamesNTP._DataRow["textareacols"] = 100;

                            CswNbtMetaDataNodeTypeProp imageurlsNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ImageUrls.ToString() );
                            imageurlsNTP._DataRow["textarearows"] = 5;
                            imageurlsNTP._DataRow["textareacols"] = 100;
                            break;

                        case CswEnumNbtFieldType.Location:
                            CswNbtMetaDataNodeTypeProp locfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKType.ToString() );
                            locfktypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            locfktypeNTP.getDefaultValue( true, true ).AsText.Text = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString();
                            locfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp locfkvalueNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKValue.ToString() );
                            locfkvalueNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            locfkvalueNTP.getDefaultValue( true, true ).AsNumber.Value = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ).ObjectClassId;
                            locfkvalueNTP.removeFromAllLayouts();
                            break;

                        case CswEnumNbtFieldType.MetaDataList:
                            CswNbtMetaDataNodeTypeProp mdlfkvalueNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleMetaDataList.AttributeName.ConstrainToObjectClass.ToString() );
                            mdlfkvalueNTP._DataRow["attribute1"] = CswConvert.ToDbVal( true );
                            break;

                        case CswEnumNbtFieldType.NFPA:
                            CswNbtMetaDataNodeTypeProp nfpadisplaymodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DisplayMode.ToString() );
                            nfpadisplaymodeNTP._DataRow["listoptions"] = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtNFPADisplayMode.Diamond.ToString(),
                                    CswEnumNbtNFPADisplayMode.Linear.ToString(),
                                }.ToString();
                            break;

                        case CswEnumNbtFieldType.NodeTypeSelect:
                            CswNbtMetaDataNodeTypeProp selectmodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.SelectMode.ToString() );
                            selectmodeNTP._DataRow["listoptions"] = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtPropertySelectMode.Single.ToString(),
                                    CswEnumNbtPropertySelectMode.Multiple.ToString(),
                                }.ToString();

                            CswNbtMetaDataNodeTypeProp ntsfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleNodeTypeSelect.AttributeName.FKType.ToString() );
                            ntsfktypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            ntsfktypeNTP.getDefaultValue( true, true ).AsText.Text = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString();
                            ntsfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp ntsfkvalueNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleNodeTypeSelect.AttributeName.ConstrainToObjectClass.ToString() );
                            ntsfkvalueNTP._DataRow["attribute1"] = CswConvert.ToDbVal( true );
                            break;

                        case CswEnumNbtFieldType.PropertyReference:
                            CswNbtMetaDataNodeTypeProp prfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKType.ToString() );
                            prfktypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            //prfktypeNTP.DefaultValue.AsList.Value = CswEnumNbtViewPropIdType.NodeTypePropId.ToString();
                            prfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp relNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Relationship.ToString() );
                            //relNTP.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );

                            //CswNbtView relView = _CswNbtSchemaModTrnsctn.restoreView( relNTP.ViewId );
                            //relView.Root.ChildRelationships.Clear();
                            //CswNbtViewRelationship prrel1 = relView.AddViewRelationship( NodeTypePropNT, false );
                            //CswNbtViewRelationship prrel2 = relView.AddViewRelationship( prrel1, CswEnumNbtViewPropOwnerType.First, NTPNodeTypeOCP, false );
                            //CswNbtViewRelationship prrel3 = relView.AddViewRelationship( prrel2, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                            //relView.AddViewPropertyAndFilter( prrel3,
                            //                                  NTPFieldTypeOCP,
                            //                                  Value: _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Relationship ).FieldTypeId.ToString() );
                            //relView.AddViewPropertyAndFilter( prrel3,
                            //                                  NTPFieldTypeOCP,
                            //                                  Conjunction: CswEnumNbtFilterConjunction.Or,
                            //                                  Value: _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Location ).FieldTypeId.ToString() );
                            //relView.save();

                            CswNbtMetaDataNodeTypeProp propNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.RelatedProperty.ToString() );
                            propNTP.setFilterDeprecated( relNTP, relNTP.getFieldTypeRule().SubFields[CswNbtFieldTypeRuleList.SubFieldName.Value], CswEnumNbtFilterMode.NotNull, null );
                            //propNTP.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );

                            CswNbtMetaDataNodeTypeProp propTypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.RelatedPropType.ToString() );
                            propTypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            //prfktypeNTP.DefaultValue.AsList.Value = CswEnumNbtViewPropIdType.NodeTypePropId.ToString();
                            prfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp useseqNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.UseSequence.ToString() );
                            CswNbtMetaDataNodeTypeProp prsequenceNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Sequence.ToString() );
                            prsequenceNTP.setFilterDeprecated( useseqNTP, useseqNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.True );
                            break;


                        case CswEnumNbtFieldType.Relationship:
                            // Relationship view reference conditional on target
                            CswNbtMetaDataNodeTypeProp reltargetNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Target.ToString() );
                            CswNbtMetaDataNodeTypeProp relviewNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.View.ToString() );
                            relviewNTP.setFilterDeprecated( reltargetNTP, reltargetNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.NotNull, string.Empty );
                            break;


                        case CswEnumNbtFieldType.ReportLink:
                            // FKType is nodeid, servermanaged
                            CswNbtMetaDataNodeTypeProp rlFkTypeNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleReportLink.AttributeName.FKType.ToString() );
                            rlFkTypeNTP.getDefaultValue( true, true ).AsText.Text = "nodeid";
                            rlFkTypeNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                            rlFkTypeNTP.removeFromAllLayouts();

                            // FKValue is relationship to Report
                            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
                            CswNbtMetaDataNodeTypeProp rlFkValueNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleReportLink.AttributeName.Target.ToString() );
                            rlFkValueNTP.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), ReportOC.ObjectClassId );
                            break;

                        case CswEnumNbtFieldType.Sequence:
                            CswNbtMetaDataNodeTypeProp seqSequenceNTP = NodeTypePropNT.getNodeTypeProp( CswNbtFieldTypeRuleSequence.AttributeName.Sequence.ToString() );
                            seqSequenceNTP._DataRow["isrequired"] = CswConvert.ToDbVal( true );
                            break;

                        case CswEnumNbtFieldType.ViewPickList:
                            CswNbtMetaDataNodeTypeProp vplselectmodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.SelectMode.ToString() );
                            vplselectmodeNTP._DataRow["listoptions"] = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtPropertySelectMode.Single.ToString(),
                                    CswEnumNbtPropertySelectMode.Multiple.ToString(),
                                }.ToString();
                            break;
                    } // switch( FieldType.FieldType )

                    propNTDict.Add( FieldType.FieldType, NodeTypePropNT );

                } // foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )


                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_props table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp> propsDict = new Dictionary<int, CswNbtObjClassDesignNodeTypeProp>();
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                    {
                        CswNbtMetaDataNodeType NodeTypePropNT = propNTDict[thisProp.getFieldTypeValue()];

                        CswNbtObjClassDesignNodeTypeProp ntpNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypePropNT.NodeTypeId, OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                            {
                                CswNbtObjClassDesignNodeTypeProp node = NewNode;

                                node.AuditLevel.Value = thisProp.AuditLevel;
                                node.CompoundUnique.Checked = CswConvert.ToTristate( thisProp.IsCompoundUnique() );
                                node.FieldType.Value = thisProp.FieldTypeId.ToString();
                                node.HelpText.Text = thisProp.HelpText;
                                node.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                                if( Int32.MinValue != thisProp.ObjectClassPropId )
                                {
                                    node.ObjectClassPropName.Value = thisProp.ObjectClassPropId.ToString();
                                }
                                node.PropName.Text = thisProp.PropName;
                                node.ReadOnly.Checked = CswConvert.ToTristate( thisProp.ReadOnly );
                                node.Required.Checked = CswConvert.ToTristate( thisProp.IsRequired );
                                node.UseNumbering.Checked = CswConvert.ToTristate( thisProp.UseNumbering );
                                node.Unique.Checked = CswConvert.ToTristate( thisProp.IsUnique() );

                                ICswNbtFieldTypeRule Rule = thisProp.getFieldTypeRule();
                                foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                                {
                                    CswNbtMetaDataNodeTypeProp prop = NodeTypePropNT.getNodeTypeProp( Attr.Name.ToString() );
                                    CswNbtNodePropWrapper wrapper = node.Node.Properties[prop];
                                    if( Attr.Name != CswEnumNbtPropertyAttributeName.NodeTypeValue ) // this would set the nodetypeid to be the prop's nodetypeid, rather than the nodetypevalue.
                                    {
                                        if( Attr.Name == CswEnumNbtPropertyAttributeName.DefaultValue )
                                        {
                                            if( thisProp.HasDefaultValue( true ) )
                                            {
                                                wrapper.copy( thisProp.getDefaultValue( false, true ) );
                                            }
                                        }
                                        else
                                        {
                                            wrapper.SetSubFieldValue( Attr.SubFieldName, thisProp[Attr.Column] );
                                        }


                                        switch( Attr.AttributeFieldType )
                                        {
                                            //case CswEnumNbtFieldType.DateTime:
                                            //    wrapper.AsDateTime.DateTimeValue = CswConvert.ToDateTime( prop[Attr.Column].ToString() );
                                            //    break;
                                            //case CswEnumNbtFieldType.Link:
                                            //    wrapper.AsLink.Href = prop[Attr.Column].ToString();
                                            //    break;
                                            //case CswEnumNbtFieldType.List:
                                            //    wrapper.AsList.Value = prop[Attr.Column].ToString();
                                            //    break;
                                            //case CswEnumNbtFieldType.Logical:
                                            //    wrapper.AsLogical.Checked = CswConvert.ToTristate( prop[Attr.Column] );
                                            //    break;
                                            //case CswEnumNbtFieldType.Memo:
                                            //    wrapper.AsMemo.Text = prop[Attr.Column].ToString();
                                            //    break;
                                            //case CswEnumNbtFieldType.MultiList:
                                            //    CswCommaDelimitedString val = new CswCommaDelimitedString();
                                            //    val.FromString( prop[Attr.Column].ToString() );
                                            //    wrapper.AsMultiList.Value = val;
                                            //    break;
                                            //case CswEnumNbtFieldType.NodeTypeSelect:
                                            //    CswCommaDelimitedString ntsval = new CswCommaDelimitedString();
                                            //    ntsval.FromString( prop[Attr.Column].ToString() );
                                            //    wrapper.AsNodeTypeSelect.SelectedNodeTypeIds = ntsval;
                                            //    break;
                                            //case CswEnumNbtFieldType.Number:
                                            //    wrapper.AsNumber.Value = CswConvert.ToDouble( prop[Attr.Column] );
                                            //    break;
                                            case CswEnumNbtFieldType.Relationship:
                                                // Need to decode the relationship value
                                                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetype_props", Attr.Column.ToString() );
                                                if( false == string.IsNullOrEmpty( _CswNbtSchemaModTrnsctn.CswDataDictionary.ForeignKeyTable ) )
                                                {
                                                    CswPrimaryKey Fk = new CswPrimaryKey( _CswNbtSchemaModTrnsctn.CswDataDictionary.ForeignKeyTable, CswConvert.ToInt32( prop[Attr.Column] ) );
                                                    CswNbtNode FkNode = _CswNbtSchemaModTrnsctn.Nodes.getNodeByRelationalId( Fk );
                                                    if( null != FkNode )
                                                    {
                                                        wrapper.AsRelationship.RelatedNodeId = FkNode.NodeId;
                                                    }
                                                }
                                                break;
                                            //case CswEnumNbtFieldType.Static:
                                            //    wrapper.AsStatic.StaticText = prop[Attr.Column].ToString();
                                            //    break;
                                            //case CswEnumNbtFieldType.Text:
                                            //    wrapper.AsText.Text = prop[Attr.Column].ToString();
                                            //    break;
                                            //case CswEnumNbtFieldType.ViewReference:
                                            //    // Can't set because it's private    
                                            //    //wrapper.AsViewReference.ViewId = new CswNbtViewId( CswConvert.ToInt32( prop[Attr.Column].ToString() ) );
                                            //    wrapper.SetSubFieldValue( CswEnumNbtSubFieldName.ViewID, prop[Attr.Column] );
                                            //    break;
                                        }
                                    }
                                }

                                propsDict.Add( thisProp.PropId, node );
                            } );
                        ntpNode.RelationalId = new CswPrimaryKey( "nodetype_props", thisProp.PropId );
                        //ntpNode.postChanges( false );
                        ICswNbtNodePersistStrategy NodePersistStrategy = new CswNbtNodePersistStrategyUpdate
                        {
                            OverrideUniqueValidation = true,
                            OverrideMailReportEvents = true,
                            Creating = true
                        };
                        NodePersistStrategy.postChanges( ntpNode.Node );

                    } // foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )


                    // Conditional Filters
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                    {
                        if( thisProp.hasFilter() )
                        {
                            CswNbtObjClassDesignNodeTypeProp node = propsDict[thisProp.PropId];
                            CswNbtSubField SubField = null;
                            CswEnumNbtFilterMode FilterMode = null;
                            string FilterValue = string.Empty;
                            thisProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );
                            node.DisplayConditionProperty.RelatedNodeId = propsDict[thisProp.FilterNodeTypePropId].NodeId;
                            node.DisplayConditionSubfield.Value = SubField.Name.ToString();
                            node.DisplayConditionFilterMode.Value = FilterMode.ToString();
                            node.DisplayConditionValue.Text = FilterValue;
                        }
                    } // foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                } // foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )


                // Here's where the extra special super-secret magic comes in
                foreach( CswEnumNbtFieldType FieldType in propNTDict.Keys )
                {
                    CswNbtMetaDataNodeType NodeTypePropNT = propNTDict[FieldType];

                    //CswNbtMetaDataNodeTypeProp NTPAuditLevelNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel );
                    //CswNbtMetaDataNodeTypeProp NTPCompoundUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique );
                    //CswNbtMetaDataNodeTypeProp NTPDisplayConditionFilterNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilterMode );
                    //CswNbtMetaDataNodeTypeProp NTPDisplayConditionPropertyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty );
                    //CswNbtMetaDataNodeTypeProp NTPDisplayConditionSubfieldNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield );
                    //CswNbtMetaDataNodeTypeProp NTPDisplayConditionValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue );
                    //CswNbtMetaDataNodeTypeProp NTPFieldTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );
                    //CswNbtMetaDataNodeTypeProp NTPHelpTextNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText );
                    //CswNbtMetaDataNodeTypeProp NTPNodeTypeValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
                    //CswNbtMetaDataNodeTypeProp NTPObjectClassPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName );
                    //CswNbtMetaDataNodeTypeProp NTPPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
                    //CswNbtMetaDataNodeTypeProp NTPReadOnlyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly );
                    //CswNbtMetaDataNodeTypeProp NTPRequiredNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Required );
                    //CswNbtMetaDataNodeTypeProp NTPServerManagedNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ServerManaged );
                    //CswNbtMetaDataNodeTypeProp NTPUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique );
                    //CswNbtMetaDataNodeTypeProp NTPUseNumberingNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering );

                    NodeTypePropNT._DataRow["tablename"] = "nodetype_props";

                    //_addJctRow( jctTable, NTPAuditLevelNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Auditlevel );
                    //_addJctRow( jctTable, NTPCompoundUniqueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Iscompoundunique );
                    ////_addJctRow( jctTable, NTPDisplayConditionFilterNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionfilter );
                    //_addJctRow( jctTable, NTPDisplayConditionPropertyNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Filterpropid, CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
                    ////_addJctRow( jctTable, NTPDisplayConditionSubfieldNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionsubfield );
                    ////_addJctRow( jctTable, NTPDisplayConditionValueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionvalue );
                    //_addJctRow( jctTable, NTPFieldTypeNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Fieldtypeid );
                    //_addJctRow( jctTable, NTPHelpTextNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Helptext );
                    //_addJctRow( jctTable, NTPNodeTypeValueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Nodetypeid, CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
                    //_addJctRow( jctTable, NTPObjectClassPropNameNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Objectclasspropid );
                    //_addJctRow( jctTable, NTPPropNameNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Propname );
                    //_addJctRow( jctTable, NTPReadOnlyNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Readonly );
                    //_addJctRow( jctTable, NTPRequiredNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Isrequired );
                    //_addJctRow( jctTable, NTPServerManagedNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Servermanaged );
                    //_addJctRow( jctTable, NTPUniqueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Isunique );
                    //_addJctRow( jctTable, NTPUseNumberingNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Usenumbering );

                    ICswNbtFieldTypeRule Rule = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( FieldType ).getFieldTypeRule();
                    foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                    {
                        if( string.Empty != Attr.Column &&
                            CswNbtResources.UnknownEnum != Attr.Column // &&
                            //CswEnumNbtPropertyAttributeColumn.Auditlevel != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Iscompoundunique != Attr.Column &&
                            ////CswEnumNbtPropertyAttributeColumn.Displayconditionfilter != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Filterpropid != Attr.Column &&
                            ////CswEnumNbtPropertyAttributeColumn.Displayconditionsubfield != Attr.Column &&
                            ////CswEnumNbtPropertyAttributeColumn.Displayconditionvalue != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Fieldtypeid != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Helptext != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Nodetypeid != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Objectclasspropid != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Propname != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Readonly != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Isrequired != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Servermanaged != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Isunique != Attr.Column &&
                            //CswEnumNbtPropertyAttributeColumn.Usenumbering != Attr.Column 
                            )
                        {
                            CswNbtMetaDataNodeTypeProp thisNTP = NodeTypePropNT.getNodeTypeProp( Attr.Name );
                            _addJctRow( jctTable, thisNTP, NodeTypePropNT.TableName, Attr.Column, Attr.SubFieldName );
                        }
                    }
                } // foreach( CswEnumNbtFieldType FieldType in propNTDict.Keys )
            } // PROPS


            jctUpdate.update( jctTable );


            //// Create a temporary view for debugging (REMOVE ME)
            //CswNbtView DesignView = _CswNbtSchemaModTrnsctn.makeView();
            //DesignView.saveNew( "Design", CswEnumNbtViewVisibility.Role, ChemSWAdminRole.NodeId );
            //DesignView.Category = "Design";
            //CswNbtViewRelationship NtViewRel = DesignView.AddViewRelationship( NodeTypeOC, false );
            //NtViewRel.setGroupByProp( NTCategoryNTP );
            //DesignView.AddViewPropertyAndFilter( NtViewRel, NTEnabledNTP, Value: CswConvert.TristateToDbVal( CswEnumTristate.True ).ToString() );
            //DesignView.save();

        } // update()


        private void _addJctRow( DataTable JctTable, CswNbtMetaDataNodeTypeProp Prop, string TableName, string ColumnName, CswEnumNbtSubFieldName SubFieldName = null )
        {
            if( false == string.IsNullOrEmpty( ColumnName ) && ColumnName != "Unknown" )
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( TableName, ColumnName );
                DataRow NodeTypeNameRow = JctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = Prop.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                if( null != SubFieldName )
                {
                    NodeTypeNameRow["subfieldname"] = SubFieldName.ToString();
                }
                else if( null != Prop.getFieldTypeRule().SubFields.Default )
                {
                    NodeTypeNameRow["subfieldname"] = Prop.getFieldTypeRule().SubFields.Default.Name;
                }
                JctTable.Rows.Add( NodeTypeNameRow );
            }
        }

        private CswNbtMetaDataNodeTypeProp _makePropNTP( CswNbtMetaDataNodeType NodeTypePropNT, Int32 TabId, CswNbtFieldTypeAttribute Attribute )
        {
            return _makePropNTP( NodeTypePropNT, TabId, Attribute.AttributeFieldType, Attribute.Name, Attribute.Column );
        }
        private CswNbtMetaDataNodeTypeProp _makePropNTP( CswNbtMetaDataNodeType NodeTypePropNT, Int32 TabId, CswEnumNbtFieldType FieldType, string PropName, CswEnumNbtPropertyAttributeColumn ColumnName )
        {
            CswNbtMetaDataNodeTypeProp newNTP = null;
            if( null == NodeTypePropNT.getNodeTypeProp( PropName ) )
            {
                newNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropDeprecated( NodeTypePropNT, FieldType, PropName, TabId );
                newNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                if( ColumnName == CswEnumNbtPropertyAttributeColumn.Isfk )
                {
                    newNTP._DataRow["servermanaged"] = CswConvert.ToDbVal( true );
                    newNTP.getDefaultValue( true, true ).AsLogical.Checked = CswEnumTristate.True;
                    newNTP.removeFromAllLayouts();
                }
            }
            return newNTP;
        } // _makePropNTP()

    }//class CswUpdateSchema_02K_Case29311_Design

}//namespace ChemSW.Nbt.Schema