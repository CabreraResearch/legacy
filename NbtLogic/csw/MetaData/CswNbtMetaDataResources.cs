using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataResources
    {
        public CswNbtResources CswNbtResources;
        public CswNbtMetaData CswNbtMetaData;
        public CswNbtFieldResources CswNbtFieldResources;

        public CswTableUpdate ObjectClassTableUpdate;
        public CswTableUpdate ObjectClassPropTableUpdate;
        public CswTableUpdate NodeTypeTableUpdate;
        public CswTableUpdate FieldTypeTableUpdate;
        public CswTableUpdate NodeTypePropTableUpdate;
        public CswTableUpdate NodeTypeTabTableUpdate;
        public CswTableUpdate JctNodesPropsTableUpdate;  // for prop default values

        public CswNbtMetaDataCollectionObjectClass ObjectClassesCollection;
        public CswNbtMetaDataCollectionObjectClassProp ObjectClassPropsCollection;
        public CswNbtMetaDataCollectionFieldType FieldTypesCollection;
        public CswNbtMetaDataCollectionNodeType NodeTypesCollection;
        public CswNbtMetaDataCollectionNodeTypeProp NodeTypePropsCollection;
        public CswNbtMetaDataCollectionNodeTypeTab NodeTypeTabsCollection;
        
        public CswNbtMetaDataResources( CswNbtResources Resources, CswNbtMetaData MetaData )
        {
            CswNbtResources = Resources;
            CswNbtMetaData = MetaData;
            CswNbtFieldResources = new CswNbtFieldResources( Resources );

            ObjectClassesCollection = new CswNbtMetaDataCollectionObjectClass( this );
            ObjectClassPropsCollection = new CswNbtMetaDataCollectionObjectClassProp( this );
            FieldTypesCollection = new CswNbtMetaDataCollectionFieldType( this );
            NodeTypesCollection = new CswNbtMetaDataCollectionNodeType( this );
            NodeTypePropsCollection = new CswNbtMetaDataCollectionNodeTypeProp( this );
            NodeTypeTabsCollection = new CswNbtMetaDataCollectionNodeTypeTab( this );
        }

        public void refreshAll( bool ExcludeDisabledModules )
        {
            CswTimer refreshAllTimer = new CswTimer();
            // Post existing changes first, so we don't lose them
            finalize();

            // So we get the freshest data dictionary around (kein 'ein hara)
            // Nah -- If schema updater needs to do this, it should call it directly
            // CswNbtResources.refreshDataDictionary();

            // We instance table caddies here so that they can be refreshed too

            ObjectClassTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClass_update", "object_class" );
            ObjectClassPropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClassProp_update", "object_class_props" );
            NodeTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeType_update", "nodetypes" );
            FieldTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_FieldType_update", "field_types" );
            NodeTypePropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeProp_update", "nodetype_props" );
            NodeTypeTabTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeTab_update", "nodetype_tabset" );
            JctNodesPropsTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_JctNodesProps_update", "jct_nodes_props" );

            RefreshAllFieldTypes( ExcludeDisabledModules );
            RefreshAllObjectClasses( ExcludeDisabledModules );
            RefreshAllObjectClassProps( ExcludeDisabledModules );
            RefreshAllNodeTypes( ExcludeDisabledModules );
            RefreshAllNodeTypeTabs( ExcludeDisabledModules );
            RefreshAllNodeTypeProps( ExcludeDisabledModules );

            CswNbtResources.logTimerResult( "Did CswNbtMetaDataResources.refreshAll()", refreshAllTimer.ElapsedDurationInSecondsAsString );

        }//refreshAll()


        private void RefreshAllObjectClasses( bool ExcludeDisabledModules )
        {
            string WhereClause = string.Empty;
            if( ExcludeDisabledModules )
            {
                WhereClause = @"where (exists (select j.jctmoduleobjectclassid
                                                 from jct_modules_objectclass j
                                                 join modules m on j.moduleid = m.moduleid
                                                where j.objectclassid = object_class.objectclassid
                                                  and m.enabled = '1')
                                       or not exists (select j.jctmoduleobjectclassid
                                                        from jct_modules_objectclass j
                                                        join modules m on j.moduleid = m.moduleid
                                                       where j.objectclassid = object_class.objectclassid))";
            }

            RefreshMetaDataObject( ObjectClassesCollection, ObjectClassTableUpdate.getTable( WhereClause,
                new Collection<OrderByClause> { new OrderByClause( "objectclass", OrderByType.Ascending ) } ) );
        }
        private void RefreshAllObjectClassProps( bool ExcludeDisabledModules )
        {
            string WhereClause = string.Empty;
            if( ExcludeDisabledModules )
            {
                WhereClause = @"where (exists (select j.jctmoduleobjectclassid
                                                 from jct_modules_objectclass j
                                                 join modules m on j.moduleid = m.moduleid
                                                where j.objectclassid = object_class_props.objectclassid
                                                  and m.enabled = '1')
                                       or not exists (select j.jctmoduleobjectclassid
                                                        from jct_modules_objectclass j
                                                        join modules m on j.moduleid = m.moduleid
                                                       where j.objectclassid = object_class_props.objectclassid))";
            }
            RefreshMetaDataObject( ObjectClassPropsCollection, ObjectClassPropTableUpdate.getTable( WhereClause,
                new Collection<OrderByClause> { new OrderByClause( "propname", OrderByType.Ascending ) } ) );
        }
        private void RefreshAllNodeTypes( bool ExcludeDisabledModules )
        {
            string WhereClause = string.Empty;
            if( ExcludeDisabledModules )
            {
                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
                                                    from jct_modules_objectclass j
                                                    join modules m on j.moduleid = m.moduleid
                                                   where j.objectclassid = nodetypes.objectclassid
                                                     and m.enabled = '1')
                                          or not exists (select j.jctmoduleobjectclassid
                                                           from jct_modules_objectclass j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.objectclassid = nodetypes.objectclassid) )
                                    and ( exists (select j.jctmodulenodetypeid
                                                  from jct_modules_nodetypes j
                                                  join modules m on j.moduleid = m.moduleid
                                                 where j.nodetypeid = nodetypes.nodetypeid
                                                   and m.enabled = '1')
                                          or not exists (select j.jctmodulenodetypeid
                                                           from jct_modules_nodetypes j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.nodetypeid = nodetypes.nodetypeid) ) )";
            }
            RefreshMetaDataObject( NodeTypesCollection, NodeTypeTableUpdate.getTable( WhereClause, 
                new Collection<OrderByClause> { new OrderByClause( "nodetypeid", OrderByType.Ascending ) } ) );
        }
        private void RefreshAllFieldTypes( bool ExcludeDisabledModules )
        {
            RefreshMetaDataObject( FieldTypesCollection, FieldTypeTableUpdate.getTable( string.Empty, new Collection<OrderByClause> { new OrderByClause( "fieldtype", OrderByType.Ascending ) } ) );
        }
        private void RefreshAllNodeTypeProps( bool ExcludeDisabledModules )
        {
            string WhereClause = string.Empty;
            if( ExcludeDisabledModules )
            {
                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
                                                    from jct_modules_objectclass j
                                                    join modules m on j.moduleid = m.moduleid
                                                   where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_props.nodetypeid)
                                                     and m.enabled = '1')
                                          or not exists (select j.jctmoduleobjectclassid
                                                           from jct_modules_objectclass j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_props.nodetypeid)) )
                                    and ( exists (select j.jctmodulenodetypeid
                                                  from jct_modules_nodetypes j
                                                  join modules m on j.moduleid = m.moduleid
                                                 where j.nodetypeid = nodetype_props.nodetypeid
                                                   and m.enabled = '1')
                                          or not exists (select j.jctmodulenodetypeid
                                                           from jct_modules_nodetypes j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.nodetypeid = nodetype_props.nodetypeid) ) )";
            }
            
            RefreshMetaDataObject( NodeTypePropsCollection, NodeTypePropTableUpdate.getTable( WhereClause,
                new Collection<OrderByClause> { new OrderByClause( "propname", OrderByType.Ascending ) } ) );

            // BZ 9139 - Fill in the relational TableName/ColumnName on the subfield here, so that it's cached
            CswTableSelect JctDdNtpSelect = CswNbtResources.makeCswTableSelect( "JctDdNtp_select", "jct_dd_ntp" );
            DataTable JctDdNtpTable = JctDdNtpSelect.getTable();
            foreach( DataRow JctDdNtpRow in JctDdNtpTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp ThisProp = NodeTypePropsCollection.getNodeTypeProp( CswConvert.ToInt32( JctDdNtpRow["nodetypepropid"] ) );
                if( ThisProp != null )
                {
                    CswNbtSubField.SubFieldName ThisSubFieldName = (CswNbtSubField.SubFieldName) Enum.Parse( typeof( CswNbtSubField.SubFieldName ), JctDdNtpRow["subfieldname"].ToString(), true );
                    CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( JctDdNtpRow["datadictionaryid"] ) );
                    ThisProp.FieldTypeRule.SubFields[ThisSubFieldName].RelationalTable = CswNbtResources.DataDictionary.TableName;
                    ThisProp.FieldTypeRule.SubFields[ThisSubFieldName].RelationalColumn = CswNbtResources.DataDictionary.ColumnName;
                }
            }
        } // RefreshAllNodeTypeProps()
        private void RefreshAllNodeTypeTabs( bool ExcludeDisabledModules )
        {
            string WhereClause = string.Empty;
            if( ExcludeDisabledModules )
            {
                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
                                                    from jct_modules_objectclass j
                                                    join modules m on j.moduleid = m.moduleid
                                                   where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)
                                                     and m.enabled = '1')
                                          or not exists (select j.jctmoduleobjectclassid
                                                           from jct_modules_objectclass j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)) )
                                    and ( exists (select j.jctmodulenodetypeid
                                                  from jct_modules_nodetypes j
                                                  join modules m on j.moduleid = m.moduleid
                                                 where j.nodetypeid = nodetype_tabset.nodetypeid
                                                   and m.enabled = '1')
                                          or not exists (select j.jctmodulenodetypeid
                                                           from jct_modules_nodetypes j
                                                           join modules m on j.moduleid = m.moduleid
                                                          where j.nodetypeid = nodetype_tabset.nodetypeid) ) )";
            }
            RefreshMetaDataObject( NodeTypeTabsCollection, NodeTypeTabTableUpdate.getTable( WhereClause,
                new Collection<OrderByClause> { new OrderByClause( "taborder", OrderByType.Ascending ) } ) );
        }



        // BZ 8205 - Refresh the data row assignment but leave the existing object intact
        private void RefreshMetaDataObject( ICswNbtMetaDataObjectCollection ObjectCollection, DataTable UpdatedTable )
        {
            ObjectCollection.ClearKeys();

            Collection<ICswNbtMetaDataObject> Objects = new Collection<ICswNbtMetaDataObject>();
            foreach ( ICswNbtMetaDataObject ThisObject in ObjectCollection.All )
                Objects.Add( ThisObject );

            foreach ( DataRow ThisRow in UpdatedTable.Rows )
            {
                ICswNbtMetaDataObject MatchingObject = null;
                foreach ( ICswNbtMetaDataObject ThisObject in Objects )
                {
                    if ( ThisObject.UniqueId == CswConvert.ToInt32( ThisRow[ ThisObject.UniqueIdFieldName ] ) )
                    {
                        // Reassign the match to the new DataRow
                        ThisObject.Reassign( ThisRow );   // won't change the uniqueid, since they match
                        ObjectCollection.RegisterExisting( ThisObject );
                        MatchingObject = ThisObject;
                        break;
                    }
                }
                if ( MatchingObject == null )
                {
                    // Make New
                    ObjectCollection.RegisterNew( ThisRow );
                }
                else
                {
                    // Take match out of the collection
                    Objects.Remove( MatchingObject );
                }
            }

            // Get rid of the leftovers
            foreach ( ICswNbtMetaDataObject ThisObject in Objects )
            {
                ObjectCollection.Remove( ThisObject );
            }
        } // RefreshMetaDataObject()


        /// <summary>
        /// Regenerate QuestionNo values for all properties
        /// </summary>
        public void RecalculateQuestionNumbers( CswNbtMetaDataNodeType NodeType )
        {
            foreach ( CswNbtMetaDataNodeTypeTab Tab in NodeType.NodeTypeTabs )
            {
                Int32 CurrentQuestionNo = 1;
                // Do non-conditional ones first
                Collection<CswNbtMetaDataNodeTypeProp> PropsToDo = new Collection<CswNbtMetaDataNodeTypeProp>();
                foreach ( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                {
                    if ( Prop.UseNumbering )
                        PropsToDo.Add( Prop );
                }

                foreach ( CswNbtMetaDataNodeTypeProp Prop in PropsToDo )
                {
                    if ( !Prop.hasFilter() )
                    {
                        Prop.QuestionNo = CurrentQuestionNo;
                        Prop.SubQuestionNo = Int32.MinValue;
                        CurrentQuestionNo++;
                    }
                }

                // Now do the conditional ones (with numbered parents)
                Int32[] SubQuestionNos = new Int32[ CurrentQuestionNo + 1 ];
                for ( Int32 i = 1; i <= CurrentQuestionNo; i++ )
                    SubQuestionNos[ i ] = 1;

                foreach ( CswNbtMetaDataNodeTypeProp Prop in PropsToDo )
                {
                    if ( Prop.hasFilter() )
                    {
                        CswNbtMetaDataNodeTypeProp ParentProp = NodeTypePropsCollection.getNodeTypeProp( Prop.FilterNodeTypePropId ).LatestVersionNodeTypeProp;
                        if ( ParentProp != null && ParentProp.QuestionNo != Int32.MinValue )
                        {
                            Prop.QuestionNo = ParentProp.QuestionNo;
                            Prop.SubQuestionNo = SubQuestionNos[ ParentProp.QuestionNo ];
                            SubQuestionNos[ ParentProp.QuestionNo ] += 1;
                        }
                    }
                }
            }
        }

        public void finalize()
        {
            bool ChangesMade = false;
            if ( NodeTypeTableUpdate != null )
                ChangesMade = NodeTypeTableUpdate.updateAll() || ChangesMade;
            if ( NodeTypeTabTableUpdate != null )
                ChangesMade = NodeTypeTabTableUpdate.updateAll() || ChangesMade;
            if ( NodeTypePropTableUpdate != null )
                ChangesMade = NodeTypePropTableUpdate.updateAll() || ChangesMade;
            if ( JctNodesPropsTableUpdate != null )
                ChangesMade = JctNodesPropsTableUpdate.updateAll() || ChangesMade;

            if ( ChangesMade )
                CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );
        }

        public ICswNbtFieldTypeRule makeFieldTypeRule( ICswNbtMetaDataProp MetaDataProp )
        {
            return CswNbtFieldTypeRuleFactory.makeRule( CswNbtFieldResources, MetaDataProp );
        }
    }
}
