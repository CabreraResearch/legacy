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
        public CswTableUpdate JctNodesPropsTableUpdate;  // for prop default values

        public CswTableSelect ObjectClassTableSelect;
        public CswTableSelect ObjectClassPropTableSelect;
        public CswTableSelect NodeTypeTableSelect;
        public CswTableSelect FieldTypeTableSelect;
        public CswTableSelect NodeTypePropTableSelect;
        public CswTableSelect NodeTypeTabTableSelect;

        public CswNbtMetaDataCollectionObjectClass ObjectClassesCollection;
        public CswNbtMetaDataCollectionObjectClassProp ObjectClassPropsCollection;
        public CswNbtMetaDataCollectionFieldType FieldTypesCollection;
        public CswNbtMetaDataCollectionNodeType NodeTypesCollection;
        public CswNbtMetaDataCollectionNodeTypeProp NodeTypePropsCollection;
        public CswNbtMetaDataCollectionNodeTypeTab NodeTypeTabsCollection;

        private bool _ExcludeDisabledModules;
        public bool ExcludeDisabledModules { get { return CswNbtMetaData.ExcludeDisabledModules; } }

        //CswNbtMetaDataTableCache _CswNbtMetaDataTableCache = null;
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
            
            ObjectClassTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClass_update", "object_class" );
            ObjectClassPropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClassProp_update", "object_class_props" );
            NodeTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeType_update", "nodetypes" );
            FieldTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_FieldType_update", "field_types" );
            NodeTypePropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeProp_update", "nodetype_props" );
            NodeTypeTabTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeTab_update", "nodetype_tabset" );
            JctNodesPropsTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_JctNodesProps_update", "jct_nodes_props" );

            ObjectClassesCollection = new CswNbtMetaDataCollectionObjectClass( this );
            ObjectClassPropsCollection = new CswNbtMetaDataCollectionObjectClassProp( this );
            FieldTypesCollection = new CswNbtMetaDataCollectionFieldType( this );
            NodeTypesCollection = new CswNbtMetaDataCollectionNodeType( this );
            NodeTypePropsCollection = new CswNbtMetaDataCollectionNodeTypeProp( this );
            NodeTypeTabsCollection = new CswNbtMetaDataCollectionNodeTypeTab( this );

            //_CswNbtMetaDataTableCache = new CswNbtMetaDataTableCache( CswNbtResources );
        }

        //public void tryAddToMetaDataCollection( object Key, object Value, IDictionary Collection, string MetaDataObjectTypeName, Int32 MetaDataObjectId, string MetaDataObjectName )
        //{
        //    if( null != Key &&
        //        null != Value )
        //    {
        //        try
        //        {
        //            Collection.Add( Key, Value );
        //        }
        //        catch( ArgumentNullException ArgumentNullException )
        //        {
        //            CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Proposed " + MetaDataObjectTypeName + " was null and cannot be added to the MetaData collection.", "", ArgumentNullException ) );
        //        }
        //        catch( ArgumentException ArgumentException )
        //        {
        //            CswNbtResources.CswLogger.reportError(
        //                new CswDniException(
        //                    ErrorType.Error,
        //                    "Duplicate " + MetaDataObjectTypeName + "s exist in the database. A " + MetaDataObjectTypeName + " named: " + MetaDataObjectName + " on " + MetaDataObjectTypeName.ToLower() + "id " + MetaDataObjectId + " has already been defined.",
        //                    "",
        //                    ArgumentException )
        //                );
        //        }
        //        catch( InvalidOperationException InvalidOperationException )
        //        {
        //            CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Cannot compare the proposed " + MetaDataObjectTypeName + " against the MetaData collection.", "", InvalidOperationException ) );
        //        }
        //        catch( NotSupportedException NotSupportedException )
        //        {
        //            CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Cannot add the proposed " + MetaDataObjectTypeName + ": " + MetaDataObjectName + " to the MetaData collection.", "", NotSupportedException ) );
        //        }
        //    }
        //}

        
        public void refreshAll()// bool ExcludeDisabledModules )
        {
            ObjectClassesCollection.clearCache();
            ObjectClassPropsCollection.clearCache();
            FieldTypesCollection.clearCache();
            NodeTypesCollection.clearCache();
            NodeTypePropsCollection.clearCache();
            NodeTypeTabsCollection.clearCache();

            //CswTimer refreshAllTimer = new CswTimer();
            //// Post existing changes first, so we don't lose them
            //finalize();

            //// So we get the freshest data dictionary around (kein 'ein hara)
            //// Nah -- If schema updater needs to do this, it should call it directly
            //// CswNbtResources.refreshDataDictionary();

            //// We instance table caddies here so that they can be refreshed too

            //ObjectClassTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClass_update", "object_class" );
            //ObjectClassPropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_ObjectClassProp_update", "object_class_props" );
            //NodeTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeType_update", "nodetypes" );
            //FieldTypeTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_FieldType_update", "field_types" );
            //NodeTypePropTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeProp_update", "nodetype_props" );
            //NodeTypeTabTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_NodeTypeTab_update", "nodetype_tabset" );
            //JctNodesPropsTableUpdate = CswNbtResources.makeCswTableUpdate( "MetaData_JctNodesProps_update", "jct_nodes_props" );

            //RefreshAllFieldTypes( ExcludeDisabledModules );
            //RefreshAllObjectClasses( ExcludeDisabledModules );
            //RefreshAllObjectClassProps( ExcludeDisabledModules );
            //RefreshAllNodeTypes( ExcludeDisabledModules );
            //RefreshAllNodeTypeTabs( ExcludeDisabledModules );
            //RefreshAllNodeTypeProps( ExcludeDisabledModules );

            //CswNbtResources.logTimerResult( "Did CswNbtMetaDataResources.refreshAll()", refreshAllTimer.ElapsedDurationInSecondsAsString );

        }//refreshAll()


//        private void RefreshAllObjectClasses( bool ExcludeDisabledModules )
//        {
//            string WhereClause = string.Empty;
//            if( ExcludeDisabledModules )
//            {
//                WhereClause = @"where (exists (select j.jctmoduleobjectclassid
//                                                 from jct_modules_objectclass j
//                                                 join modules m on j.moduleid = m.moduleid
//                                                where j.objectclassid = object_class.objectclassid
//                                                  and m.enabled = '1')
//                                       or not exists (select j.jctmoduleobjectclassid
//                                                        from jct_modules_objectclass j
//                                                        join modules m on j.moduleid = m.moduleid
//                                                       where j.objectclassid = object_class.objectclassid))";
//            }


//            DataTable ObjectClassesTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.ObjectClass );
//            if( null == ObjectClassesTable )
//            {
//                ObjectClassesTable = ObjectClassTableUpdate.getTable( WhereClause, new Collection<OrderByClause> { new OrderByClause( "objectclass", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.ObjectClass, ObjectClassesTable );
//            }

//            RefreshMetaDataObject( ObjectClassesCollection, ObjectClassesTable );
//        }

//        private void RefreshAllObjectClassProps( bool ExcludeDisabledModules )
//        {
//            string WhereClause = string.Empty;
//            if( ExcludeDisabledModules )
//            {
//                WhereClause = @"where (exists (select j.jctmoduleobjectclassid
//                                                 from jct_modules_objectclass j
//                                                 join modules m on j.moduleid = m.moduleid
//                                                where j.objectclassid = object_class_props.objectclassid
//                                                  and m.enabled = '1')
//                                       or not exists (select j.jctmoduleobjectclassid
//                                                        from jct_modules_objectclass j
//                                                        join modules m on j.moduleid = m.moduleid
//                                                       where j.objectclassid = object_class_props.objectclassid))";
//            }

//            DataTable ObjectClasPropsTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.ObjectClassProp );
//            if( null == ObjectClasPropsTable )
//            {
//                ObjectClasPropsTable = ObjectClassPropTableUpdate.getTable( WhereClause, new Collection<OrderByClause> { new OrderByClause( "propname", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.ObjectClassProp, ObjectClasPropsTable );
//            }

//            RefreshMetaDataObject( ObjectClassPropsCollection, ObjectClasPropsTable );
//        }

//        private void RefreshAllNodeTypes( bool ExcludeDisabledModules )
//        {
//            string WhereClause = string.Empty;
//            if( ExcludeDisabledModules )
//            {
//                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
//                                                    from jct_modules_objectclass j
//                                                    join modules m on j.moduleid = m.moduleid
//                                                   where j.objectclassid = nodetypes.objectclassid
//                                                     and m.enabled = '1')
//                                          or not exists (select j.jctmoduleobjectclassid
//                                                           from jct_modules_objectclass j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.objectclassid = nodetypes.objectclassid) )
//                                    and ( exists (select j.jctmodulenodetypeid
//                                                  from jct_modules_nodetypes j
//                                                  join modules m on j.moduleid = m.moduleid
//                                                 where j.nodetypeid = nodetypes.nodetypeid
//                                                   and m.enabled = '1')
//                                          or not exists (select j.jctmodulenodetypeid
//                                                           from jct_modules_nodetypes j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.nodetypeid = nodetypes.nodetypeid) ) )";
//            }


//            DataTable NodeTypesTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.NodeType );
//            if( null == NodeTypesTable )
//            {
//                NodeTypesTable = NodeTypeTableUpdate.getTable( WhereClause, new Collection<OrderByClause> { new OrderByClause( "nodetypeid", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.NodeType, NodeTypesTable );
//            }

//            RefreshMetaDataObject( NodeTypesCollection, NodeTypesTable );
//        }


//        private void RefreshAllFieldTypes( bool ExcludeDisabledModules )
//        {
//            DataTable FieldTypesTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.FieldType );
//            if( null == FieldTypesTable )
//            {
//                FieldTypesTable = FieldTypeTableUpdate.getTable( string.Empty, new Collection<OrderByClause> { new OrderByClause( "fieldtype", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.FieldType, FieldTypesTable );
//            }


//            RefreshMetaDataObject( FieldTypesCollection, FieldTypesTable );
//        }

//        private void RefreshAllNodeTypeProps( bool ExcludeDisabledModules )
//        {
//            string WhereClause = string.Empty;
//            if( ExcludeDisabledModules )
//            {
//                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
//                                                    from jct_modules_objectclass j
//                                                    join modules m on j.moduleid = m.moduleid
//                                                   where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_props.nodetypeid)
//                                                     and m.enabled = '1')
//                                          or not exists (select j.jctmoduleobjectclassid
//                                                           from jct_modules_objectclass j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_props.nodetypeid)) )
//                                    and ( exists (select j.jctmodulenodetypeid
//                                                  from jct_modules_nodetypes j
//                                                  join modules m on j.moduleid = m.moduleid
//                                                 where j.nodetypeid = nodetype_props.nodetypeid
//                                                   and m.enabled = '1')
//                                          or not exists (select j.jctmodulenodetypeid
//                                                           from jct_modules_nodetypes j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.nodetypeid = nodetype_props.nodetypeid) ) )";
//            }

//            DataTable NodeTypePropTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.NodeTypeProp );
//            if( null == NodeTypePropTable )
//            {
//                NodeTypePropTable = NodeTypePropTableUpdate.getTable( WhereClause, new Collection<OrderByClause> { new OrderByClause( "propname", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.NodeTypeProp, NodeTypePropTable );
//            }

//            RefreshMetaDataObject( NodeTypePropsCollection, NodeTypePropTable );

//            // BZ 9139 - Fill in the relational TableName/ColumnName on the subfield here, so that it's cached
//            CswTableSelect JctDdNtpSelect = CswNbtResources.makeCswTableSelect( "JctDdNtp_select", "jct_dd_ntp" );
//            DataTable JctDdNtpTable = JctDdNtpSelect.getTable();
//            foreach( DataRow JctDdNtpRow in JctDdNtpTable.Rows )
//            {
//                CswNbtMetaDataNodeTypeProp ThisProp = NodeTypePropsCollection.getNodeTypeProp( CswConvert.ToInt32( JctDdNtpRow["nodetypepropid"] ) );
//                if( ThisProp != null )
//                {
//                    CswNbtSubField.SubFieldName ThisSubFieldName = (CswNbtSubField.SubFieldName) Enum.Parse( typeof( CswNbtSubField.SubFieldName ), JctDdNtpRow["subfieldname"].ToString(), true );
//                    CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( JctDdNtpRow["datadictionaryid"] ) );
//                    ThisProp.FieldTypeRule.SubFields[ThisSubFieldName].RelationalTable = CswNbtResources.DataDictionary.TableName;
//                    ThisProp.FieldTypeRule.SubFields[ThisSubFieldName].RelationalColumn = CswNbtResources.DataDictionary.ColumnName;
//                }
//            }
//        } // RefreshAllNodeTypeProps()



//        private void RefreshAllNodeTypeTabs( bool ExcludeDisabledModules )
//        {
//            string WhereClause = string.Empty;
//            if( ExcludeDisabledModules )
//            {
//                WhereClause = @"where ( ( exists (select j.jctmoduleobjectclassid
//                                                    from jct_modules_objectclass j
//                                                    join modules m on j.moduleid = m.moduleid
//                                                   where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)
//                                                     and m.enabled = '1')
//                                          or not exists (select j.jctmoduleobjectclassid
//                                                           from jct_modules_objectclass j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)) )
//                                    and ( exists (select j.jctmodulenodetypeid
//                                                  from jct_modules_nodetypes j
//                                                  join modules m on j.moduleid = m.moduleid
//                                                 where j.nodetypeid = nodetype_tabset.nodetypeid
//                                                   and m.enabled = '1')
//                                          or not exists (select j.jctmodulenodetypeid
//                                                           from jct_modules_nodetypes j
//                                                           join modules m on j.moduleid = m.moduleid
//                                                          where j.nodetypeid = nodetype_tabset.nodetypeid) ) )";
//            }

//            DataTable NodeTypeTabsTable = _CswNbtMetaDataTableCache.get( CswNbtMetaDataTableCache.MetaDataTable.NodeTypeTab );
//            if( null == NodeTypeTabsTable )
//            {
//                NodeTypeTabsTable = NodeTypeTabTableUpdate.getTable( WhereClause, new Collection<OrderByClause> { new OrderByClause( "taborder", OrderByType.Ascending ) } );
//                _CswNbtMetaDataTableCache.put( CswNbtMetaDataTableCache.MetaDataTable.NodeTypeTab, NodeTypeTabsTable );
//            }

//            RefreshMetaDataObject( NodeTypeTabsCollection, NodeTypeTabsTable );
//        }



//        // BZ 8205 - Refresh the data row assignment but leave the existing object intact
//        private void RefreshMetaDataObject( ICswNbtMetaDataObjectCollection ObjectCollection, DataTable UpdatedTable )
//        {
//            ObjectCollection.ClearKeys();

//            Collection<ICswNbtMetaDataObject> Objects = new Collection<ICswNbtMetaDataObject>();
//            foreach( ICswNbtMetaDataObject ThisObject in ObjectCollection.All )
//                Objects.Add( ThisObject );

//            foreach( DataRow ThisRow in UpdatedTable.Rows )
//            {
//                ICswNbtMetaDataObject MatchingObject = null;
//                foreach( ICswNbtMetaDataObject ThisObject in Objects )
//                {
//                    if( ThisObject.UniqueId == CswConvert.ToInt32( ThisRow[ThisObject.UniqueIdFieldName] ) )
//                    {
//                        // Reassign the match to the new DataRow
//                        ThisObject.Reassign( ThisRow );   // won't change the uniqueid, since they match
//                        ObjectCollection.RegisterExisting( ThisObject );
//                        MatchingObject = ThisObject;
//                        break;
//                    }
//                }
//                if( MatchingObject == null )
//                {
//                    // Make New
//                    ObjectCollection.RegisterNew( ThisRow );
//                }
//                else
//                {
//                    // Take match out of the collection
//                    Objects.Remove( MatchingObject );
//                }
//            }

//            // Get rid of the leftovers
//            foreach( ICswNbtMetaDataObject ThisObject in Objects )
//            {
//                ObjectCollection.Remove( ThisObject );
//            }
//        } // RefreshMetaDataObject()


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

        public ICswNbtFieldTypeRule makeFieldTypeRule(CswNbtMetaDataFieldType.NbtFieldType FieldType)
        {
            return CswNbtFieldTypeRuleFactory.makeRule( CswNbtFieldResources, FieldType );
        }
    }
}
