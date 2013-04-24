using System;
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
            DataTable jctTable = jctUpdate.getTable();
            foreach( DataRow jctRow in jctTable.Rows )
            {
                jctRow.Delete();
            }
            jctUpdate.update( jctTable );

            // Create new Super-MetaData Design nodetypes
            _makeNodeTypeNT();
            _makeNodeTypePropNT();
            _makeNodeTypeTabNT();

            // Create a temporary view for debugging (REMOVE ME)
            CswNbtView DesignView = _CswNbtSchemaModTrnsctn.makeView();
            DesignView.saveNew( "Design", CswEnumNbtViewVisibility.Global );
            DesignView.Category = "Design";
            DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ), false );
            DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass ), false );
            DesignView.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass ), false );
            DesignView.save();

        } // update()


        private void _makeNodeTypeNT()
        {
            CswNbtMetaDataObjectClass NodeTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
            CswNbtMetaDataNodeType NodeTypeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypeOC )
                {
                    NodeTypeName = "Design NodeType",
                    Category = "Design"                    
                } );
            NodeTypeNT.setNameTemplateText( "Design " + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName ) );

            Int32 TabId = NodeTypeNT.getFirstNodeTypeTab().TabId;

            CswNbtMetaDataNodeTypeProp AuditLevelNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.AuditLevel );
            CswNbtMetaDataNodeTypeProp CategoryNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Category );
            CswNbtMetaDataNodeTypeProp DeferSearchToNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo );
            CswNbtMetaDataNodeTypeProp IconFileNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.IconFileName );
            CswNbtMetaDataNodeTypeProp LockedNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Locked );
            CswNbtMetaDataNodeTypeProp NameTemplateNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplate );
            CswNbtMetaDataNodeTypeProp NameTemplateAddNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd );
            CswNbtMetaDataNodeTypeProp NodeTypeNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName );
            CswNbtMetaDataNodeTypeProp ObjectClassNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName );
            CswNbtMetaDataNodeTypeProp ObjectClassValueNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassValue );

            // Edit Layout
            NodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
            ObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
            CategoryNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
            IconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
            NameTemplateNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 1 );
            NameTemplateAddNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 2 );
            AuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
            DeferSearchToNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
            LockedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );
            ObjectClassValueNTP.removeFromLayout( CswEnumNbtLayoutType.Edit );

            // Add Layout
            ObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
            NodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
            CategoryNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
            AuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            DeferSearchToNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            IconFileNameNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            LockedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            NameTemplateNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            NameTemplateAddNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            ObjectClassNameNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

            // Table Layout
            NodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            ObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

            // Preview Layout
            NodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            ObjectClassNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );


            // Here's where the extra special super-secret magic comes in

            NodeTypeNT.TableName = "nodetypes";

            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jctddntp_update_nt", "jct_dd_ntp" );
            DataTable jctTable = jctUpdate.getEmptyTable();
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "nodetypename" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = NodeTypeNameNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = NodeTypeNameNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "objectclassid" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = ObjectClassValueNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = ObjectClassValueNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "category" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = CategoryNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = CategoryNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "iconfilename" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = IconFileNameNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = IconFileNameNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "nametemplate" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = NameTemplateNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = NameTemplateNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "auditlevel" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = AuditLevelNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = AuditLevelNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "searchdeferpropid" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = DeferSearchToNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = DeferSearchToNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetypes", "islocked" );
                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = LockedNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                NodeTypeNameRow["subfieldname"] = LockedNTP.getFieldTypeRule().SubFields.Default.Name;
                jctTable.Rows.Add( NodeTypeNameRow );
            }

            jctUpdate.update( jctTable );

            // Populate nodes
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
                node.postChanges( false );
            }


        } // _makeNodeTypeNT()


        private void _makeNodeTypePropNT()
        {
        } // _makeNodeTypePropNT()

        private void _makeNodeTypeTabNT()
        {
        } // _makeNodeTypeTabNT()

    }//class CswUpdateSchema_02B_Case29311_Design

}//namespace ChemSW.Nbt.Schema