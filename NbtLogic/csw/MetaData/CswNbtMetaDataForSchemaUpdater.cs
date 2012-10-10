using System;
using System.Collections.ObjectModel;
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
        public CswNbtMetaDataForSchemaUpdater( CswNbtResources Resources, CswNbtMetaDataResources MetaDataResources )
            : base( Resources, MetaDataResources, false )  // Schema updater should always see all object classes, regardless of modules
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


            //foreach( CswNbtMetaDataNodeType NodeType in this.NodeTypesByObjectClass )
            //{
            //    foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in NodeType.ObjectClass.ObjectClassProps )
            //    {
            //        // Find exact matches
            //        string PropName = ObjectClassProp.PropName;
            //        bool DoSync = false;
            //        CswNbtMetaDataNodeTypeProp MatchingNodeTypeProp = null;
            //        foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.NodeTypeProps )
            //        {
            //            if( NodeTypeProp.ObjectClassProp != null && NodeTypeProp.ObjectClassProp.PropId == ObjectClassProp.PropId )
            //            {
            //                MatchingNodeTypeProp = NodeTypeProp;
            //                break;
            //            }
            //        }
            //        // Find name & fieldtype matches
            //        if( MatchingNodeTypeProp == null )
            //        {
            //            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.NodeTypeProps )
            //            {
            //                if( false == DoSync &&
            //                    NodeTypeProp.PropName.ToLower() == PropName.ToLower() &&
            //                    NodeTypeProp.FieldType.FieldType == ObjectClassProp.FieldType.FieldType )
            //                {
            //                    MatchingNodeTypeProp = NodeTypeProp;
            //                    DoSync = true;
            //                }
            //                else if( NodeTypeProp.PropName.ToLower() == PropName.ToLower() )
            //                {
            //                    PropName += " " + NodeTypeProp.FieldType.FieldType.ToString();
            //                }
            //            }
            //            while( null != NodeType.getNodeTypeProp( PropName ) )
            //            {
            //                PropName += " " + ObjectClassProp.ObjectClassPropId;
            //            }

            //        }
            //        // Make missing ones
            //        if( MatchingNodeTypeProp == null )
            //        {
            //            //CswNbtMetaDataNodeTypeTab Tab = NodeType.getFirstNodeTypeTab();
            //            makeNewProp( NodeType, null, ObjectClassProp.FieldType.FieldTypeId, PropName, Int32.MinValue, true, ObjectClassProp );
            //            DoSync = false;
            //        }

            //        if( DoSync )
            //        {
            //            CopyNodeTypePropFromObjectClassProp( ObjectClassProp, MatchingNodeTypeProp._DataRow );
            //            CopyNodeTypePropDefaultValueFromObjectClassProp( ObjectClassProp, MatchingNodeTypeProp );
            //        }

            //    } // foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in NodeType.ObjectClass.ObjectClassProps )
            //} // foreach( CswNbtMetaDataNodeType NodeType in this.NodeTypes )


            foreach( Int32 ObjectClassId in this.getObjectClassIds().Values )
            {
                foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProps( ObjectClassId ) )
                {
                    foreach( Int32 NodeTypeId in this.getNodeTypeIds( ObjectClassId ) )
                    {
                        // Find exact matches first
                        CswNbtMetaDataNodeTypeProp MatchingNTP = getNodeTypePropByObjectClassProp( NodeTypeId, ObjectClassProp.ObjectClassPropId );

                        if( MatchingNTP == null )
                        {
                            //string PropName = ObjectClassProp.PropName;
                            bool DoSync = false;

                            // Find name & fieldtype matches
                            // Our objective is either to find a match, or to determine a unique propname
                            bool KeepSearching = true;
                            string PropName = ObjectClassProp.PropName;
                            while( KeepSearching )
                            {
                                MatchingNTP = getNodeTypeProp( NodeTypeId, PropName );
                                if( MatchingNTP == null )
                                {
                                    KeepSearching = false;
                                }
                                else if( MatchingNTP.FieldTypeId == ObjectClassProp.FieldTypeId )
                                {
                                    DoSync = true;
                                    KeepSearching = false;
                                }
                                else
                                {
                                    PropName += " " + ObjectClassProp.getFieldType().FieldType.ToString();
                                    PropName += " " + ObjectClassProp.ObjectClassPropId;
                                }
                            } // while( KeepSearching )

                            // Make missing ones
                            if( MatchingNTP == null )
                            {
                                //CswNbtMetaDataNodeTypeTab Tab = NodeType.getFirstNodeTypeTab();
                                CswNbtMetaDataNodeType NodeType = getNodeType( NodeTypeId );
                                makeNewProp( NodeType, null, ObjectClassProp.FieldTypeId, PropName, Int32.MinValue, true, ObjectClassProp );
                                DoSync = false;
                            }

                            if( DoSync )
                            {
                                CopyNodeTypePropFromObjectClassProp( ObjectClassProp, MatchingNTP._DataRow );
                                CopyNodeTypePropDefaultValueFromObjectClassProp( ObjectClassProp, MatchingNTP );
                            }

                        } // if( MatchingNodeTypeProp == null )
                    } // foreach( CswNbtMetaDataNodeType NodeType in this.getNodeTypesByObjectClass( ObjectClassId ) )
                } // foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in getObjectClassProps(ObjectClassId) )
            } // foreach( Int32 ObjectClassId in this.getObjectClassIds() )



            // Now reset the "real" meta data collection in CswNbtResources for these changes
            CswNbtResources.MetaData.refreshAll();

        } // makeMissingNodeTypeProps()


        /// <summary>
        /// Deletes an object class prop and all nodetype props from the database and metadata collection
        /// </summary>
        public Collection<CswNbtMetaDataNodeTypeProp> DeleteObjectClassProp( CswNbtMetaDataObjectClassProp ObjectClassProp, bool DeleteNodeTypeProps )
        {
            Collection<CswNbtMetaDataNodeTypeProp> Ret = new Collection<CswNbtMetaDataNodeTypeProp>();
            Collection<CswNbtMetaDataNodeTypeProp> DoomedProps = new Collection<CswNbtMetaDataNodeTypeProp>();

            foreach( CswNbtMetaDataNodeTypeProp Prop in ObjectClassProp.getNodeTypeProps() )
            {
                Prop._DataRow["objectclasspropid"] = DBNull.Value;
                _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( Prop._DataRow.Table );
                if( DeleteNodeTypeProps )
                {
                    DoomedProps.Add( Prop );
                }
                else
                {
                    Ret.Add( Prop );
                }
            }

            foreach( CswNbtMetaDataNodeTypeProp Prop in DoomedProps )
            {
                DeleteNodeTypeProp( Prop, true );
            }

            // Update MetaData
            _CswNbtMetaDataResources.ObjectClassPropsCollection.clearCache();

            // Delete the Object Class Prop
            ObjectClassProp._DataRow.Delete();
            _CswNbtMetaDataResources.ObjectClassPropTableUpdate.update( ObjectClassProp._DataRow.Table );
            return Ret;
        } // DeleteObjectClassProp()


        /// <summary>
        /// Deletes an object class and all nodetypes from the database and metadata collection
        /// </summary>
        public void DeleteObjectClass( CswNbtMetaDataObjectClass ObjectClass )
        {
            // Delete Nodetypes first
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
            {
                DeleteNodeTypeAllVersions( NodeType );
            }

            // Update MetaData
            _CswNbtMetaDataResources.ObjectClassesCollection.clearCache();
            
            // Delete the Object Class Props
            foreach( CswNbtMetaDataObjectClassProp OcProp in ObjectClass.getObjectClassProps() )
            {
                DeleteObjectClassProp( OcProp, false );
            }

            _CswNbtMetaDataResources.ObjectClassPropsCollection.clearCache();

            // Delete the Object Class
            ObjectClass._DataRow.Delete();
            _CswNbtMetaDataResources.ObjectClassTableUpdate.update( ObjectClass._DataRow.Table );
        } // DeleteObjectClass()

        /// <summary>
        /// Set the default value for an object class prop, and cascade the change to all existing NodeTypeProps
        /// </summary>
        /// <param name="ObjectClassProp"></param>
        /// <param name="SubFieldName">Optional. Use the default subfield if null.</param>
        /// <param name="Value"></param>
        public void SetObjectClassPropDefaultValue( CswNbtMetaDataObjectClassProp ObjectClassProp, CswNbtSubField.SubFieldName SubFieldName, object Value )
        {
            if( null != ObjectClassProp )
            {
                SubFieldName = SubFieldName ?? ObjectClassProp.getFieldTypeRule().SubFields.Default.Name;
                ObjectClassProp.DefaultValue.SetPropRowValue( ObjectClassProp.getFieldTypeRule().SubFields[SubFieldName].Column, Value );
                // We're going to regret this day
                ObjectClassProp.DefaultValue.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Value );

                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ObjectClassProp.getNodeTypeProps() )
                {
                    NodeTypeProp.DefaultValue.SetPropRowValue( ObjectClassProp.getFieldTypeRule().SubFields[SubFieldName].Column, Value );
                    NodeTypeProp.DefaultValue.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Value );
                }
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
                    if( Attribute == CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd )
                    {
                        ObjectClassProp._DataRow[CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_col_add )] = DBNull.Value;
                        ObjectClassProp._DataRow[CswNbtMetaDataObjectClassProp.getObjectClassPropAttributesAsString( CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_row_add )] = DBNull.Value;
                    }
                    _CswNbtMetaDataResources.ObjectClassPropTableUpdate.update( ObjectClassProp._DataRow.Table );

                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ObjectClassProp.getNodeTypeProps() )
                    {
                        CswNbtMetaDataNodeTypeProp.NodeTypePropAttributes NodeTypeAttribute;
                        Enum.TryParse( AttributeName, true, out NodeTypeAttribute );
                        if( NodeTypeAttribute != CswNbtMetaDataNodeTypeProp.NodeTypePropAttributes.unknown )
                        {
                            NodeTypeProp._DataRow[AttributeName] = DBValue;
                        }
                        else if( Attribute == CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd )
                        {
                            if( CswConvert.ToBoolean( Value ) )
                            {
                                NodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                            }
                            else
                            {
                                NodeTypeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                            }
                        }
                    }
                }
            }
        } // UpdateObjectClassProp()
    }//CswNbtMetaDataForSchemaUpdater
}//ChemSW.Nbt.MetaData
