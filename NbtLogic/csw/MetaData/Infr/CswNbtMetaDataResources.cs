using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ChemSW.DB;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataResources
    {
        public bool _PreventVersioning = false;

        public CswNbtResources CswNbtResources;
        public CswNbtMetaData CswNbtMetaData;
        public CswNbtFieldResources CswNbtFieldResources;

        public CswTableUpdate ObjectClassTableUpdate;
        public CswTableUpdate ObjectClassPropTableUpdate;
        public CswTableUpdate NodeTypeTableUpdate;
        public CswTableUpdate FieldTypeTableUpdate;
        public CswTableUpdate NodeTypePropTableUpdate;
        public CswTableUpdate NodeTypeTabTableUpdate;
        public CswTableUpdate PropertySetTableUpdate;
        public CswTableUpdate JctNodesPropsTableUpdate;  // for prop default values

        public CswTableSelect ObjectClassTableSelect;
        public CswTableSelect ObjectClassPropTableSelect;
        public CswTableSelect NodeTypeTableSelect;
        public CswTableSelect FieldTypeTableSelect;
        public CswTableSelect NodeTypePropTableSelect;
        public CswTableSelect PropertySetTableSelect;
        public CswTableSelect NodeTypeTabTableSelect;

        public CswNbtMetaDataCollectionObjectClass ObjectClassesCollection;
        public CswNbtMetaDataCollectionObjectClassProp ObjectClassPropsCollection;
        public CswNbtMetaDataCollectionFieldType FieldTypesCollection;
        public CswNbtMetaDataCollectionNodeType NodeTypesCollection;
        public CswNbtMetaDataCollectionNodeTypeProp NodeTypePropsCollection;
        public CswNbtMetaDataCollectionNodeTypeTab NodeTypeTabsCollection;
        public CswNbtMetaDataCollectionPropertySet PropertySetsCollection;

        private bool _ExcludeDisabledModules;
        public bool ExcludeDisabledModules { get { return CswNbtMetaData.ExcludeDisabledModules; } }

        public CswNbtMetaDataResources( CswNbtResources Resources, CswNbtMetaData MetaData )
        {
            CswNbtResources = Resources;
            CswNbtMetaData = MetaData;
            CswNbtFieldResources = new CswNbtFieldResources( Resources );

            _ExcludeDisabledModules = ExcludeDisabledModules;

            ObjectClassTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_ObjectClass_Select", "object_class" );
            ObjectClassPropTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_ObjectClassProp_Select", "object_class_props" );
            NodeTypeTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_NodeType_Select", "nodetypes" );
            FieldTypeTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_FieldType_Select", "field_types" );
            NodeTypePropTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_NodeTypeProp_Select", "nodetype_props" );
            NodeTypeTabTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_NodeTypeTab_Select", "nodetype_tabset" );
            PropertySetTableSelect = CswNbtResources.makeCswTableSelect( "MetaData_PropertySet_Select", "property_set" );

            ObjectClassTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClass_update", "object_class" );
            ObjectClassPropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClassProp_update", "object_class_props" );
            NodeTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeType_update", "nodetypes" );
            FieldTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_FieldType_update", "field_types" );
            NodeTypePropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeProp_update", "nodetype_props" );
            NodeTypeTabTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeTab_update", "nodetype_tabset" );
            PropertySetTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_PropertySet_update", "property_set" );
            JctNodesPropsTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_JctNodesProps_update", "jct_nodes_props" );

            ObjectClassesCollection = new CswNbtMetaDataCollectionObjectClass( this );
            ObjectClassPropsCollection = new CswNbtMetaDataCollectionObjectClassProp( this );
            FieldTypesCollection = new CswNbtMetaDataCollectionFieldType( this );
            NodeTypesCollection = new CswNbtMetaDataCollectionNodeType( this );
            NodeTypePropsCollection = new CswNbtMetaDataCollectionNodeTypeProp( this );
            NodeTypeTabsCollection = new CswNbtMetaDataCollectionNodeTypeTab( this );
            PropertySetsCollection = new CswNbtMetaDataCollectionPropertySet( this );
        }

        public void refreshAll()
        {
            ObjectClassesCollection.clearCache();
            ObjectClassPropsCollection.clearCache();
            FieldTypesCollection.clearCache();
            NodeTypesCollection.clearCache();
            NodeTypePropsCollection.clearCache();
            NodeTypeTabsCollection.clearCache();
            PropertySetsCollection.clearCache();
        }//refreshAll()

        /// <summary>
        /// Regenerate QuestionNo values for all properties
        /// </summary>
        public void RecalculateQuestionNumbers( CswNbtMetaDataNodeType NodeType )
        {
            foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
            {
                Int32 CurrentQuestionNo = 1;
                // Do non-conditional ones first
                Dictionary<Int32, CswNbtMetaDataNodeTypeProp> PropsToDo = new Dictionary<Int32, CswNbtMetaDataNodeTypeProp>();
                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.getNodeTypePropsByDisplayOrder() )
                {
                    if( Prop.UseNumbering )
                        PropsToDo.Add( Prop.FirstPropVersionId, Prop );
                }

                foreach( CswNbtMetaDataNodeTypeProp Prop in PropsToDo.Values )
                {
                    if( !Prop.hasFilter() )
                    {
                        Prop.QuestionNo = CurrentQuestionNo;
                        Prop.SubQuestionNo = Int32.MinValue;
                        CurrentQuestionNo++;
                    }
                }

                // Now do the conditional ones (with numbered parents)
                Int32[] SubQuestionNos = new Int32[CurrentQuestionNo + 1];
                for( Int32 i = 1; i <= CurrentQuestionNo; i++ )
                    SubQuestionNos[i] = 1;

                foreach( CswNbtMetaDataNodeTypeProp Prop in PropsToDo.Values )
                {
                    if( Prop.hasFilter() )
                    {
                        //CswNbtMetaDataNodeTypeProp ParentProp = NodeTypePropsCollection.getNodeTypeProp( Prop.FilterNodeTypePropId ).getNodeTypePropLatestVersion();
                        CswNbtMetaDataNodeTypeProp ParentProp = PropsToDo[Prop.FilterNodeTypePropId];
                        if( ParentProp != null && ParentProp.QuestionNo != Int32.MinValue )
                        {
                            Prop.QuestionNo = ParentProp.QuestionNo;
                            Prop.SubQuestionNo = SubQuestionNos[ParentProp.QuestionNo];
                            SubQuestionNos[ParentProp.QuestionNo] += 1;
                        }
                    }
                }
            }
        }

        public void finalize()
        {
            bool ChangesMade = false;
            if( NodeTypeTableUpdate != null )
                ChangesMade = NodeTypeTableUpdate.updateAll() || ChangesMade;
            if( NodeTypeTabTableUpdate != null )
                ChangesMade = NodeTypeTabTableUpdate.updateAll() || ChangesMade;
            if( NodeTypePropTableUpdate != null )
                ChangesMade = NodeTypePropTableUpdate.updateAll() || ChangesMade;
            if( JctNodesPropsTableUpdate != null )
                ChangesMade = JctNodesPropsTableUpdate.updateAll() || ChangesMade;

            if( ChangesMade )
            {
                CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );
                //_CswNbtMetaDataTableCache.makeCacheStale(); //this will force a reload of tables
            }
        }

        public ICswNbtFieldTypeRule makeFieldTypeRule( CswEnumNbtFieldType FieldType )
        {
            return CswNbtFieldTypeRuleFactory.makeRule( CswNbtFieldResources, FieldType );
        }
    }
}
