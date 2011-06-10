using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// A version of CswNbtMetaData that includes operations not normally available to the application
    /// </summary>
    public class CswNbtMetaDataForSchemaUpdater : CswNbtMetaData
    {
        // IMPORTANT NOTE
        // This class should use the same CswNbtMetaDataResources instance as 
        // the CswNbtResources.MetaData collection does.  This ensures that the 
        // two collections will be using the same source data and will 
        // remain in synch when changes are made.

        /// <summary>
        /// Meta data collection with extra functionality for schema updater
        /// </summary>
        public CswNbtMetaDataForSchemaUpdater( CswNbtResources Resources, CswNbtMetaDataResources MetaDataResources, bool ExcludeDisabledModules )
            : base( Resources, MetaDataResources, ExcludeDisabledModules )
        {
        }

        /// <summary>
        /// Detects whether a nodetype is missing an object class prop, and creates the missing ones
        /// Mostly for use in Schema Update scripts
        /// </summary>
        public void makeMissingNodeTypeProps()
        {
            // First refresh our collection to catch any new changes in the database
            refreshAll();

            // BZ 7968
            // We need to prevent versioning.  Furthermore, even though we theoretically prevent any changes to locked NodeTypes, 
            // we have to apply these changes to prior versions for compatibility.

            foreach( CswNbtMetaDataNodeType NodeType in this.NodeTypes )
            {
                foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in NodeType.ObjectClass.ObjectClassProps )
                {
                    // Find exact matches
                    bool DoSynch = false;
                    CswNbtMetaDataNodeTypeProp MatchingNodeTypeProp = null;
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.NodeTypeProps )
                    {
                        if( NodeTypeProp.ObjectClassProp != null && NodeTypeProp.ObjectClassProp.PropId == ObjectClassProp.PropId )
                        {
                            MatchingNodeTypeProp = NodeTypeProp;
                            break;
                        }
                    }
                    // Find name & fieldtype matches
                    if( MatchingNodeTypeProp == null )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.NodeTypeProps )
                        {
                            if( NodeTypeProp.PropName.ToLower() == ObjectClassProp.PropName.ToLower() &&
                                NodeTypeProp.FieldType.FieldType == ObjectClassProp.FieldType.FieldType )
                            {
                                MatchingNodeTypeProp = NodeTypeProp;
                                DoSynch = true;
                                break;
                            }
                        }
                    }
                    // Make missing ones
                    if( MatchingNodeTypeProp == null )
                    {
                        //CswNbtMetaDataNodeTypeTab Tab = NodeType.getFirstNodeTypeTab();
                        MatchingNodeTypeProp = makeNewProp(NodeType, null, ObjectClassProp.FieldType.FieldTypeId, ObjectClassProp.PropName, Int32.MinValue, true, ObjectClassProp);
                        DoSynch = false;   // because makeNewProp does it for us
                    }

                    if (DoSynch)
                    {
                        CopyNodeTypePropFromObjectClassProp(ObjectClassProp, MatchingNodeTypeProp._DataRow);
                        CopyNodeTypePropDefaultValueFromObjectClassProp(ObjectClassProp, MatchingNodeTypeProp);
                    }

                } // foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in NodeType.ObjectClass.ObjectClassProps )
            } // foreach( CswNbtMetaDataNodeType NodeType in this.NodeTypes )


            // Now reset the "real" meta data collection in CswNbtResources for these changes
            CswNbtResources.MetaData.refreshAll();

        } // makeMissingNodeTypeProps()

        
        /// <summary>
        /// Deletes an object class prop and all nodetype props from the database and metadata collection
        /// </summary>
        public void DeleteObjectClassProp( CswNbtMetaDataObjectClassProp ObjectClassProp )
        {
            // Delete Nodetype Props first
            while( ObjectClassProp.NodeTypeProps.Count > 0 )
            {
                IEnumerator e = ObjectClassProp.NodeTypeProps.GetEnumerator();
                e.MoveNext();
                DeleteNodeTypeProp( (CswNbtMetaDataNodeTypeProp) e.Current, true );
            }

            // Update MetaData
            _CswNbtMetaDataResources.ObjectClassPropsCollection.Deregister( ObjectClassProp );

            // Delete the Object Class Prop
            ObjectClassProp._DataRow.Delete();
            _CswNbtMetaDataResources.ObjectClassPropTableUpdate.update( ObjectClassProp._DataRow.Table );
        }


        /// <summary>
        /// Deletes an object class and all nodetypes from the database and metadata collection
        /// </summary>
        public void DeleteObjectClass( CswNbtMetaDataObjectClass ObjectClass )
        {
            // Delete Nodetype Props first
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
            {
                DeleteNodeTypeAllVersions( NodeType );
            }

            // Update MetaData
            _CswNbtMetaDataResources.ObjectClassesCollection.Deregister( ObjectClass );

            // Delete the Object Class Prop
            ObjectClass._DataRow.Delete();
            _CswNbtMetaDataResources.ObjectClassTableUpdate.update( ObjectClass._DataRow.Table );
        }

        /// <summary>
        /// Set the default value for an object class prop, and cascade the change to all existing NodeTypeProps
        /// </summary>
        /// <param name="ObjectClassProp"></param>
        /// <param name="SubFieldName"></param>
        /// <param name="Value"></param>
        public void SetObjectClassPropDefaultValue( CswNbtMetaDataObjectClassProp ObjectClassProp, CswNbtSubField.SubFieldName SubFieldName, object Value )
        {
            ObjectClassProp.DefaultValue.SetPropRowValue( ObjectClassProp.FieldTypeRule.SubFields[SubFieldName].Column, Value );
            // We're going to regret this day
            ObjectClassProp.DefaultValue.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Value );

            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ObjectClassProp.NodeTypeProps )
            {
                NodeTypeProp.DefaultValue.SetPropRowValue( ObjectClassProp.FieldTypeRule.SubFields[SubFieldName].Column, Value );
                NodeTypeProp.DefaultValue.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Value );
            }
        }

        /// <summary>
        /// Update the attributes of an Object Class Prop, and cascade changes to existing NodeTypeProps
        /// </summary>
        public void UpdateObjectClassProp( CswNbtMetaDataObjectClassProp ObjectClassProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes Attribute, object Value )
        {
            if( Attribute != CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.Unknown )
            {
                string AttributeName = CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( Attribute );
                object DBValue = CswConvert.ToDbVal( Value );
                if( ObjectClassProp._DataRow[AttributeName] != DBValue )
                {
                    ObjectClassProp._DataRow[AttributeName] = DBValue;
                    _CswNbtMetaDataResources.ObjectClassPropTableUpdate.update( ObjectClassProp._DataRow.Table );

                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ObjectClassProp.NodeTypeProps )
                    {
                        NodeTypeProp._DataRow[AttributeName] = DBValue;
                    }
                }
            }


        } // UpdateObjectClassProp()







    }//CswNbtMetaDataForSchemaUpdater


}//ChemSW.Nbt.MetaData
