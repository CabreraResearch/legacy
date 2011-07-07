using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDefault : CswNbtObjClass
    {
        public CswNbtObjClassDefault( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
            : base( CswNbtResources, CswNbtNode )
        {
        }//ctor()

        public CswNbtObjClassDefault( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.Unknown ); }
        }

        public override void beforeCreateNode()
        {
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9609 - We must force reinit since we just added node(s)
            // actually, let's reinit everything, since any number of trees may be affected
            //_CswNbtResources.Trees.clear();

            // BZ 10094 - Notification event
            _CswNbtResources.runNotification( this.NodeTypeId, CswNbtObjClassNotification.EventOption.Create, _CswNbtNode, string.Empty, string.Empty );

        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CurrentProp.WasModified )
                {
                    // When a property changes, we need to:
                    // 1. mark composite property values which include changed properties on this node as pending update
                    foreach( CswNbtNodePropWrapper CompositeProp in _CswNbtNode.Properties[CswNbtMetaDataFieldType.NbtFieldType.Composite] )
                    {
                        if( CompositeProp.AsComposite.TemplateValue.Contains( CswNbtMetaData.MakeTemplateEntry( CurrentProp.NodeTypePropId.ToString() ) ) )
                        {
                            CompositeProp.PendingUpdate = true;
                        }
                    }

                    // 2. mark property references attached to relationships whose values changed as pending update
                    if( CurrentProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                    {
                        foreach( CswNbtNodePropWrapper PropRefPropWrapper in _CswNbtNode.Properties[CswNbtMetaDataFieldType.NbtFieldType.PropertyReference] )
                        {
                            CswNbtNodePropPropertyReference PropRefProp = PropRefPropWrapper.AsPropertyReference;
                            if( ( PropRefProp.RelationshipType == CswNbtViewRelationship.PropIdType.NodeTypePropId &&
                                 PropRefProp.RelationshipId == CurrentProp.NodeTypePropId ) ||
                                ( PropRefProp.RelationshipType == CswNbtViewRelationship.PropIdType.ObjectClassPropId &&
                                 PropRefProp.RelationshipId == CurrentProp.ObjectClassPropId ) )
                            {
                                PropRefProp.PendingUpdate = true;
                                PropRefProp.ClearCachedValue();
                            }
                        }
                    }

                    // 3. mark any property references to this property on other nodes as pending update
                    CswStaticSelect PropRefsSelect = _CswNbtResources.makeCswStaticSelect( "MetaDataOC_beforeWriteNode_proprefs_select", "getPropertyReferences" );
                    //BZ 8744 
                    if( CurrentProp.NodeId.TableName == "nodes" )
                        PropRefsSelect.S4Parameters.Add( "getnodeid", CurrentProp.NodeId.PrimaryKey );
                    else
						throw new CswDniException( ErrorType.Error, "Record could not be updated.", "Error updating property reference on node in " + CurrentProp.NodeId.TableName + " table." );
                    PropRefsSelect.S4Parameters.Add( "getnodetypepropid", CurrentProp.NodeTypePropId );
                    PropRefsSelect.S4Parameters.Add( "getobjectclasspropid", CurrentProp.ObjectClassPropId );
                    DataTable PropRefsTable = PropRefsSelect.getTable();

                    if( PropRefsTable.Rows.Count > 0 )
                    {
                        //BZ 10239
                        //Fetch the cached value field name. This works as long as jct_nodes_props is the only table we pull prop values from
                        //CISPro NG will break this
                        Int32 FirstNodeTypePropId = CswConvert.ToInt32( PropRefsTable.Rows[0]["nodetypepropid"] );
                        CswNbtMetaDataNodeTypeProp FirstNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( FirstNodeTypePropId );
                        CswNbtFieldTypeRulePropertyReference PropRefFieldTypeRule = (CswNbtFieldTypeRulePropertyReference) FirstNodeTypeProp.FieldTypeRule;
                        CswNbtSubField.PropColumn PropRefColumn = PropRefFieldTypeRule.CachedValueSubField.Column;

                        // Update the jct_nodes_props directly, to avoid having to fetch all the node info for every node with a prop ref to this prop
                        string PkString = string.Empty;
                        foreach( DataRow PropRefsRow in PropRefsTable.Rows )
                        {
                            if( PkString != string.Empty ) PkString += ",";
                            PkString += PropRefsRow["jctnodepropid"].ToString();
                        }
                        if( PkString != string.Empty )
                        {
                            CswTableUpdate JctNodesPropsUpdate = _CswNbtResources.makeCswTableUpdate( "MetaDataOC_beforeWriteNode_pendingupdate_update", "jct_nodes_props" );
                            DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( "where jctnodepropid in (" + PkString + ")" );
                            foreach( DataRow JctNodesPropsRow in JctNodesPropsTable.Rows )
                            {
                                JctNodesPropsRow["pendingupdate"] = "1";
                                JctNodesPropsRow[PropRefColumn.ToString()] = string.Empty;
                            }
                            JctNodesPropsUpdate.update( JctNodesPropsTable );
                        }
                    }
                    // 4. For locations, if this node's location changed, we need to update the pathname on the children
                    if( CurrentProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                    {
                        _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.updateRelationsToThisNode( _CswNbtNode );
                    }

                } // if(CurrentProp.WasModified)
            } // foreach (CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties)

            //_synchNodeName();
            // can't do this here, because we miss some of the onBeforeUpdateNodePropRow events
            // we do it in writer now instead

        } // beforeWriteNode()

        public override void afterWriteNode()
        {
            // BZ 10094 - Notification event
            bool SomethingModified = false;
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CurrentProp.WasModified )
                {
                    // Prop-specific notifications  
                    _CswNbtResources.runNotification( this.NodeTypeId, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, CurrentProp.PropName, string.Empty );
                    _CswNbtResources.runNotification( this.NodeTypeId, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, CurrentProp.PropName, CurrentProp.Gestalt );
                    SomethingModified = true;
                }
            }
            // Generic edit notifications  
            if( SomethingModified )
                _CswNbtResources.runNotification( this.NodeTypeId, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, string.Empty, string.Empty );

        }//afterWriteNode()

        public override void beforeDeleteNode() { }
        public override void afterDeleteNode()
        {
            // BZ 10223 - Clear all cached trees.
            //_CswNbtResources.Trees.clear();

            // BZ 10094 - Notification event
            _CswNbtResources.runNotification( this.NodeTypeId, CswNbtObjClassNotification.EventOption.Delete, _CswNbtNode, string.Empty, string.Empty );
        }



        public override void afterPopulateProps() 
        {
        }


        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
        }


        //private void _updateRelationsToThisNode()
        //{
        //    CswQueryCaddy RelatedsQueryCaddy = _CswNbtResources.makeCswQueryCaddy( "getRelationshipsToNode" ); 
        //    RelatedsQueryCaddy.S4Parameters.Add( "getnodeid", _CswNbtNode.NodeId );
        //    DataTable RelatedsTable = RelatedsQueryCaddy.Table;

        //    // Update the jct_nodes_props directly, to avoid having to fetch all the node info for every node with a relationship to this node
        //    string PkString = string.Empty;
        //    foreach( DataRow RelatedsRow in RelatedsTable.Rows )
        //    {
        //        if( PkString != string.Empty ) PkString += ",";
        //        PkString += RelatedsRow["jctnodepropid"].ToString();
        //    }
        //    if( PkString != string.Empty )
        //    {
        //        CswTableCaddy JctNodesPropsCaddy = _CswNbtResources.makeCswTableCaddy( "jct_nodes_props" );
        //        JctNodesPropsCaddy.WhereClause = "where jctnodepropid in (" + PkString + ")";
        //        DataTable JctNodesPropsTable = JctNodesPropsCaddy.Table;
        //        foreach( DataRow JctNodesPropsRow in JctNodesPropsTable.Rows )
        //        {
        //            JctNodesPropsRow["pendingupdate"] = "1";
        //        }
        //        JctNodesPropsCaddy.update( JctNodesPropsTable );
        //    }
        //}

    }//CswNbtObjClassDefault

}//namespace ChemSW.Nbt.ObjClasses
