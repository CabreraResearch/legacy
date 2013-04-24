using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02B_Case29311_Design : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
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
            CswNbtMetaDataNodeType NodeTypeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypeOC )
                {
                    NodeTypeName = "Design NodeType",
                    Category = "Design"
                } );
            NodeTypeNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName ) );

            CswNbtMetaDataObjectClass NodeTypePropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataNodeType NodeTypePropNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypePropOC )
            {
                NodeTypeName = "Design NodeTypeProp",
                Category = "Design"
            } );
            NodeTypePropNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName ) );

            CswNbtMetaDataObjectClass NodeTypeTabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataNodeType NodeTypeTabNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypeTabOC )
            {
                NodeTypeName = "Design NodeTypeTab",
                Category = "Design"
            } );
            NodeTypeTabNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName ) );



            CswNbtMetaDataNodeTypeProp NTAuditLevelNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.AuditLevel );
            CswNbtMetaDataNodeTypeProp NTCategoryNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Category );
            CswNbtMetaDataNodeTypeProp NTDeferSearchToNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo );
            CswNbtMetaDataNodeTypeProp NTIconFileNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.IconFileName );
            CswNbtMetaDataNodeTypeProp NTLockedNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Locked );
            CswNbtMetaDataNodeTypeProp NTNameTemplateNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplate );
            CswNbtMetaDataNodeTypeProp NTNameTemplateAddNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd );
            CswNbtMetaDataNodeTypeProp NTNodeTypeNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName );
            CswNbtMetaDataNodeTypeProp NTObjectClassNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName );
            CswNbtMetaDataNodeTypeProp NTObjectClassValueNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassValue );

            CswNbtMetaDataNodeTypeProp NTPNodeTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
            CswNbtMetaDataNodeTypeProp NTPFieldTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );
            CswNbtMetaDataNodeTypeProp NTPPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );

            CswNbtMetaDataNodeTypeProp NTTNodeTypeNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue );
            CswNbtMetaDataNodeTypeProp NTTTabNameNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName );




            Dictionary<Int32, CswNbtObjClassDesignNodeType> NTNodes = new Dictionary<Int32, CswNbtObjClassDesignNodeType>();

            DataTable jctTable = jctUpdate.getEmptyTable();

            // NodeTypeNT Props
            {
                Int32 TabId = NodeTypeNT.getFirstNodeTypeTab().TabId;

                // Set view for NameTemplateAddNTP
                CswNbtView NameView = _CswNbtSchemaModTrnsctn.restoreView( NTNameTemplateAddNTP.ViewId );
                NameView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship NameViewRel1 = NameView.AddViewRelationship( NodeTypeNT, false );
                CswNbtViewRelationship NameViewRel2 = NameView.AddViewRelationship( NameViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeNTP, false );
                NameView.save();

                // Set view for DeferSearchToNTP
                CswNbtView DeferView = _CswNbtSchemaModTrnsctn.restoreView( NTDeferSearchToNTP.ViewId );
                DeferView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship DeferViewRel1 = DeferView.AddViewRelationship( NodeTypeNT, false );
                CswNbtViewRelationship DeferViewRel2 = DeferView.AddViewRelationship( DeferViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeNTP, false );
                DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeNTP,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: "Relationship" );
                DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeNTP,
                                                    Conjunction: CswEnumNbtFilterConjunction.Or,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: "Location" );
                DeferView.save();

                // Edit Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                NTNameTemplateNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 2 );
                NTNameTemplateAddNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 2 );
                NTAuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
                NTDeferSearchToNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
                NTLockedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );
                NTObjectClassValueNTP.removeFromLayout( CswEnumNbtLayoutType.Edit );

                // Add Layout
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                NTAuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTDeferSearchToNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTLockedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateAddNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTObjectClassNameNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Table Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

                // Preview Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );


                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetypes table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    CswNbtObjClassDesignNodeType node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                    node.AuditLevel.Value = thisNodeType.AuditLevel;
                    node.Category.Text = thisNodeType.Category;
                    //node.DeferSearchTo.RelatedNodeId = thisNodeType.SearchDeferPropId;
                    node.IconFileName.Value = new CswCommaDelimitedString() { thisNodeType.IconFileName };
                    node.Locked.Checked = CswConvert.ToTristate( thisNodeType.IsLocked );
                    node.NameTemplate.Text = thisNodeType.getNameTemplateText();
                    node.NodeTypeName.Text = thisNodeType.NodeTypeName;
                    node.ObjectClassName.Text = thisNodeType.getObjectClass().ObjectClass.ToString();
                    node.ObjectClassValue.Value = thisNodeType.ObjectClassId.ToString();
                    node.RelationalId = new CswPrimaryKey( "nodetypes", thisNodeType.NodeTypeId );
                    node.postChanges( false );

                    NTNodes.Add( thisNodeType.NodeTypeId, node );
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeNT.TableName = "nodetypes";

                _addJctRow( jctTable, NTNodeTypeNameNTP, NodeTypeNT.TableName, "nodetypename" );
                _addJctRow( jctTable, NTObjectClassValueNTP, NodeTypeNT.TableName, "objectclassid" );
                _addJctRow( jctTable, NTCategoryNTP, NodeTypeNT.TableName, "category" );
                _addJctRow( jctTable, NTIconFileNameNTP, NodeTypeNT.TableName, "iconfilename" );
                _addJctRow( jctTable, NTNameTemplateNTP, NodeTypeNT.TableName, "nametemplate" );
                _addJctRow( jctTable, NTAuditLevelNTP, NodeTypeNT.TableName, "auditlevel" );
                _addJctRow( jctTable, NTDeferSearchToNTP, NodeTypeNT.TableName, "searchdeferpropid", CswEnumNbtSubFieldName.NodeID );
                _addJctRow( jctTable, NTLockedNTP, NodeTypeNT.TableName, "islocked" );
            }


            // NodeTypePropNT Props
            {

                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_props table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                    {
                        CswNbtObjClassDesignNodeTypeProp node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypePropNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                        node.PropName.Text = thisProp.PropName;
                        node.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                        node.FieldType.Value = thisProp.getFieldTypeValue();
                        node.RelationalId = new CswPrimaryKey( "nodetype_props", thisProp.PropId );
                        node.postChanges( false );
                    }
                }


                // Here's where the extra special super-secret magic comes in

                NodeTypePropNT.TableName = "nodetype_props";

                _addJctRow( jctTable, NTPNodeTypeNTP, NodeTypePropNT.TableName, "nodetypeid" );
                _addJctRow( jctTable, NTPFieldTypeNTP, NodeTypePropNT.TableName, "fieldtypeid" );
                _addJctRow( jctTable, NTPPropNameNTP, NodeTypePropNT.TableName, "propname" );
            }


            // NodeTypeTabNT Props
            {
                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_tabset table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeTab thisTab in thisNodeType.getNodeTypeTabs() )
                    {
                        CswNbtObjClassDesignNodeTypeTab node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeTabNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                        node.TabName.Text = thisTab.TabName;
                        node.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                        node.RelationalId = new CswPrimaryKey( "nodetype_tabset", thisTab.TabId );
                        node.postChanges( false );
                    }
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeTabNT.TableName = "nodetype_tabset";

                _addJctRow( jctTable, NTTNodeTypeNTP, NodeTypeTabNT.TableName, "nodetypeid" );
                _addJctRow( jctTable, NTTTabNameNTP, NodeTypeTabNT.TableName, "tabname" );
            }

            jctUpdate.update( jctTable );

            // Create a temporary view for debugging (REMOVE ME)
            CswNbtView DesignView = _CswNbtSchemaModTrnsctn.makeView();
            DesignView.saveNew( "Design", CswEnumNbtViewVisibility.Global );
            DesignView.Category = "Design";
            DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ), false );
            //DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass ), false );
            //DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass ), false );
            DesignView.save();

        } // update()


        private void _addJctRow( DataTable JctTable, CswNbtMetaDataNodeTypeProp Prop, string TableName, string ColumnName, CswEnumNbtSubFieldName SubFieldName = null )
        {
            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( TableName, ColumnName );
            DataRow NodeTypeNameRow = JctTable.NewRow();
            NodeTypeNameRow["nodetypepropid"] = Prop.PropId;
            NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
            if( null != SubFieldName )
            {
                NodeTypeNameRow["subfieldname"] = SubFieldName.ToString();
            }
            else
            {
                NodeTypeNameRow["subfieldname"] = Prop.getFieldTypeRule().SubFields.Default.Name;
            }
            JctTable.Rows.Add( NodeTypeNameRow );
        }

    }//class CswUpdateSchema_02B_Case29311_Design

}//namespace ChemSW.Nbt.Schema