using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDefault : CswNbtObjClass
    {
        public CswNbtObjClassDefault( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
            : base( CswNbtResources, CswNbtNode )
        {
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.Unknown ); }
        }

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // BZ 9609 - We must force reinit since we just added node(s)
            // actually, let's reinit everything, since any number of trees may be affected
            //_CswNbtResources.Trees.clear();

            // BZ 10094 - Notification event
            _CswNbtResources.runNotification( this.NodeType, CswNbtObjClassNotification.EventOption.Create, _CswNbtNode, string.Empty, string.Empty );

        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            List<CswNbtNodePropWrapper> CompoundUniqueProps = new List<CswNbtNodePropWrapper>();
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
                    if( CurrentProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                    {
                        foreach( CswNbtNodePropWrapper PropRefPropWrapper in _CswNbtNode.Properties[CswNbtMetaDataFieldType.NbtFieldType.PropertyReference] )
                        {
                            CswNbtNodePropPropertyReference PropRefProp = PropRefPropWrapper.AsPropertyReference;
                            if( ( PropRefProp.RelationshipType == NbtViewPropIdType.NodeTypePropId &&
                                 PropRefProp.RelationshipId == CurrentProp.NodeTypePropId ) ||
                                ( PropRefProp.RelationshipType == NbtViewPropIdType.ObjectClassPropId &&
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
                    {
                        CswStaticParam StaticParam = new CswStaticParam( "getnodeid", CurrentProp.NodeId.PrimaryKey );
                        PropRefsSelect.S4Parameters.Add( "getnodeid", StaticParam );
                    }
                    else
                    { throw new CswDniException( ErrorType.Error, "Record could not be updated.", "Error updating property reference on node in " + CurrentProp.NodeId.TableName + " table." ); }
                    PropRefsSelect.S4Parameters.Add( "getnodetypepropid", new CswStaticParam( "getnodetypepropid", CurrentProp.NodeTypePropId ) );
                    PropRefsSelect.S4Parameters.Add( "getobjectclasspropid", new CswStaticParam( "getobjectclasspropid", CurrentProp.ObjectClassPropId ) );
                    DataTable PropRefsTable = PropRefsSelect.getTable();

                    if( PropRefsTable.Rows.Count > 0 )
                    {
                        //BZ 10239
                        //Fetch the cached value field name. This works as long as jct_nodes_props is the only table we pull prop values from
                        //CISPro NG will break this
                        Int32 FirstNodeTypePropId = CswConvert.ToInt32( PropRefsTable.Rows[0]["nodetypepropid"] );
                        CswNbtMetaDataNodeTypeProp FirstNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( FirstNodeTypePropId );
                        CswNbtFieldTypeRulePropertyReference PropRefFieldTypeRule = (CswNbtFieldTypeRulePropertyReference) FirstNodeTypeProp.getFieldTypeRule();
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
                    if( CurrentProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                    {
                        _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.updateRelationsToThisNode( _CswNbtNode );
                    }

                    // 5. Prepare for compound unique validation
                    if( CurrentProp.NodeTypeProp.IsCompoundUnique() )
                    {
                        CompoundUniqueProps.Add( CurrentProp );
                    }
                } // if(CurrentProp.WasModified)
            } // foreach (CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties)

            if( CompoundUniqueProps.Count > 0 )
            {

                if( false == IsCopy && false == OverrideUniqueValidation )
                {

                    //check for other compound unique props that were _not_ modififed
                    foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
                    {
                        if( CurrentProp.NodeTypeProp.IsCompoundUnique() && ( false == CompoundUniqueProps.Contains( CurrentProp ) ) )
                        {
                            CompoundUniqueProps.Add( CurrentProp );
                        }
                    }

                    //CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                    CswNbtView CswNbtView = this.NodeType.CreateDefaultView();
                    CswNbtView.ViewName = "For compound unique";

                    CswNbtViewRelationship ViewRelationship = CswNbtView.Root.ChildRelationships[0];

                    if( NodeId != null )
                    {
                        ViewRelationship.NodeIdsToFilterOut.Add( NodeId );
                    }


                    foreach( CswNbtNodePropWrapper CurrentCompoundUniuqeProp in CompoundUniqueProps )
                    {
                        CswNbtViewProperty CswNbtViewProperty = CswNbtView.AddViewProperty( ViewRelationship, CurrentCompoundUniuqeProp.NodeTypeProp );
                        CurrentCompoundUniuqeProp.NodeTypeProp.getFieldTypeRule().AddUniqueFilterToView( CswNbtView, CswNbtViewProperty, CurrentCompoundUniuqeProp );
                    }

                    ICswNbtTree NodeTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, true, true, false, false );

                    if( NodeTree.getChildNodeCount() > 0 )
                    {
                        NodeTree.goToNthChild( 0 );
                        CswNbtNode DuplicateValueNode = NodeTree.getNodeForCurrentPosition();

                        CswCommaDelimitedString CompoundUniquePropNames = new CswCommaDelimitedString();
                        CswCommaDelimitedString CompoundUniquePropValues = new CswCommaDelimitedString();
                        foreach( CswNbtNodePropWrapper CurrentUniqueProp in CompoundUniqueProps )
                        {
                            CompoundUniquePropNames.Add( CurrentUniqueProp.PropName );
                            CompoundUniquePropValues.Add( CurrentUniqueProp.Gestalt );
                        }

                        string ExotericMessage = "The following properties must have unique values:  " + CompoundUniquePropNames.ToString();
                        string EsotericMessage = "The " + CompoundUniquePropNames.ToString() + " of node " + NodeId.ToString() + " are the same as for node " + DuplicateValueNode.NodeId.ToString() + ": " + CompoundUniquePropValues.ToString();

                        throw ( new CswDniException( ErrorType.Warning, ExotericMessage, EsotericMessage ) );


                    }//we have a duplicate value situation
                }

                else
                {
                    foreach( CswNbtNodePropWrapper CurrentPropWrapper in CompoundUniqueProps )
                    {
                        CurrentPropWrapper.ClearValue();
                        CurrentPropWrapper.clearModifiedFlag();
                    }

                }//if-else we're not a copy and not overridding

            }//if we have at leaste one modified compound unique prop

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
                    _CswNbtResources.runNotification( this.NodeType, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, CurrentProp.PropName, string.Empty );
                    _CswNbtResources.runNotification( this.NodeType, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, CurrentProp.PropName, CurrentProp.Gestalt );
                    SomethingModified = true;
                }
            }
            // Generic edit notifications  
            if( SomethingModified )
                _CswNbtResources.runNotification( this.NodeType, CswNbtObjClassNotification.EventOption.Edit, _CswNbtNode, string.Empty, string.Empty );

        }//afterWriteNode()

        public override void beforeDeleteNode() 
        { 
            // case 22486 - Don't allow deleting targets of required relationships
            CswTableSelect JctSelect = _CswNbtResources.makeCswTableSelect( "defaultBeforeDeleteNode_jnp_select", "jct_nodes_props" );
            string WhereClause = " where nodetypepropid in (select nodetypepropid from nodetype_props where isrequired = '1') and field1_fk = " + _CswNbtNode.NodeId.PrimaryKey.ToString();
            CswCommaDelimitedString SelectClause = new CswCommaDelimitedString() { "nodeid" };
            DataTable MatchTable = JctSelect.getTable( SelectClause, WhereClause );

            if( MatchTable.Rows.Count > 0 )
            {
                CswCommaDelimitedString InUseStr = new CswCommaDelimitedString();
                foreach(DataRow MatchRow in MatchTable.Rows)
                {
                    CswPrimaryKey MatchNodePk = new CswPrimaryKey("nodes", CswConvert.ToInt32(MatchRow["nodeid"]));
                    InUseStr.Add( _CswNbtResources.makeClientNodeReference( _CswNbtResources.Nodes[MatchNodePk] ) );
                }
                throw new CswDniException( ErrorType.Warning,
                                           "This " + _CswNbtNode.getNodeType().NodeTypeName + " cannot be deleted because it is in use by: " + InUseStr,
                                           "Current user (" + _CswNbtResources.CurrentUser.Username + ") tried to delete a " + _CswNbtNode.getNodeType().NodeTypeName + " that is in use by: " + InUseStr );
            }
        } // beforeDeleteNode()

        public override void afterDeleteNode()
        {
            // BZ 10223 - Clear all cached trees.
            //_CswNbtResources.Trees.clear();

            // BZ 10094 - Notification event
            _CswNbtResources.runNotification( this.NodeType, CswNbtObjClassNotification.EventOption.Delete, _CswNbtNode, string.Empty, string.Empty );
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

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
    }//CswNbtObjClassDefault

}//namespace ChemSW.Nbt.ObjClasses
