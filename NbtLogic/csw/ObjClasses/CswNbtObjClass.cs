using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public abstract class CswNbtObjClass
    {
        protected CswNbtNode _CswNbtNode = null;
        protected CswNbtResources _CswNbtResources = null;

        protected CswDateTime _Date
        {
            get { return _CswNbtNode._Date; }
        }

        private bool canSave( string TabId )
        {
            Int32 TabIdInt = CswConvert.ToInt32( TabId );
            bool Ret = false;
            if( null != this.Node )
            {
                switch( _CswNbtResources.EditMode )
                {
                    case CswEnumNbtNodeEditMode.Temp:
                    case CswEnumNbtNodeEditMode.Add:
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, this.NodeType ) )
                        {
                            Ret = true;
                        }
                        break;
                    case CswEnumNbtNodeEditMode.Edit:
                        if( TabIdInt > 0 )
                        {
                            CswNbtMetaDataNodeTypeTab Tab = this.NodeType.getNodeTypeTab( TabIdInt );
                            if( null != Tab )
                            {
                                Ret = _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, this.NodeType, Tab, NodeId : NodeId );
                            }
                        }
                        else
                        {
                            Ret = _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.Edit, this.NodeType, NodeId: NodeId );
                        }
                        break;
                }
            }
            return Ret;
        }

        /// <summary>
        /// Constructor for when we have a node instance
        /// </summary>
        public CswNbtObjClass( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
        {
            _CswNbtNode = CswNbtNode;
            _CswNbtResources = CswNbtResources;
        }//ctor()

        /// <summary>
        /// Post node property changes to the database
        /// </summary>
        /// <param name="ForceUpdate">If true, an update will happen whether properties have been modified or not (case 5446)</param>
        /// <param name="SkipEvents">Prevent calling node or property events (for use when you are inside such an event)</param>
        public void postChanges( bool ForceUpdate, bool SkipEvents = false )
        {
            _CswNbtNode.postChanges( ForceUpdate, SkipEvents: SkipEvents );
        }//postChanges()

        /// <summary>
        /// Post node property changes to the database.  
        /// Does NOT execute base event logic (for performance).
        /// TODO - Case 31708: fix performance issues on writeNode event logic and remove this function
        /// </summary>
        /// <param name="ForceUpdate">If true, an update will happen whether properties have been modified or not</param>
        public void postOnlyChanges( bool ForceUpdate )
        {
            _CswNbtNode.postOnlyChanges( ForceUpdate );
        }//postChanges()

        /// <summary>
        /// Converts a temp node to a real one.  
        /// Creates INSERT audit records for current values of all properties.
        /// Thus, make sure all other property modifications have been posted before calling this.
        /// </summary>
        public void PromoteTempToReal()
        {
            _CswNbtNode.PromoteTempToReal();
        }

        #region ObjectClass-Specific Logic

        public abstract CswNbtMetaDataObjectClass ObjectClass { get; }
        /// <summary>
        /// ObjectClass-specific logic to execute before persisting a new real node (from temp or create)
        /// </summary>
        protected virtual void beforePromoteNodeLogic( bool OverrideUniqueValidation = false ) { }
        /// <summary>
        /// ObjectClass-specific logic to execute after persisting a new real node (from temp or create)
        /// </summary>
        protected virtual void afterPromoteNodeLogic() { }
        /// <summary>
        /// ObjectClass-specific logic to execute before updating a new or existing node
        /// </summary>
        protected virtual void beforeWriteNodeLogic( bool Creating ) { }
        /// <summary>
        /// ObjectClass-specific logic to execute after updating a new or existing node
        /// </summary>
        protected virtual void afterWriteNodeLogic() { }
        /// <summary>
        /// ObjectClass-specific logic to execute before deleting a node
        /// </summary>
        protected virtual void beforeDeleteNodeLogic() { }
        /// <summary>
        /// ObjectClass-specific logic to execute after deleting a node
        /// </summary>
        protected virtual void afterDeleteNodeLogic() { }
        /// <summary>
        /// ObjectClass-specific logic to execute after clicking an object-class button (including the Save button)
        /// </summary>
        protected virtual bool onButtonClick( NbtButtonData ButtonData ) { return true; }
        /// <summary>
        /// ObjectClass-specific View Filters to add
        /// </summary>
        public virtual void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        #endregion ObjectClass-Specific Logic

        #region Base Node Event Logic

        public void beforePromoteNode( bool OverrideUniqueValidation = false )
        {
            beforePromoteNodeLogic( OverrideUniqueValidation: OverrideUniqueValidation );
        }

        public void afterPromoteNode()
        {
            afterPromoteNodeLogic();
        }

        #region BeforeWriteNode

        public void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            beforeWriteNodeLogic( Creating );
            if( false == Creating )
            {
                foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
                {
                    if( CurrentProp.wasAnySubFieldModified() )
                    {
                        _updateExternalRelatedProps( CurrentProp );
                    }
                }
            }
            if( false == OverrideUniqueValidation )
            {
                _validateCompoundUniqueProps( IsCopy );
            }
        } // beforeWriteNode()

        //Updates related composite, propertyreference, relationship, and location
        private void _updateExternalRelatedProps( CswNbtNodePropWrapper CurrentProp )
        {
            // When a property changes, we need to:
            // 1. recalculate composite property values which include changed properties on this node
            foreach( CswNbtNodePropWrapper CompositeProp in _CswNbtNode.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.Composite] )
            {
                if( CompositeProp.AsComposite.TemplateValue.Contains( CswNbtMetaData.MakeTemplateEntry( CurrentProp.NodeTypePropId.ToString() ) ) )
                {
                    CompositeProp.AsComposite.RecalculateCompositeValue();
                }
            }

            // 2. recalculate property references attached to relationships whose values changed
            if( CurrentProp.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
            {
                foreach( CswNbtNodePropWrapper PropRefPropWrapper in _CswNbtNode.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.PropertyReference] )
                {
                    CswNbtNodePropPropertyReference PropRefProp = PropRefPropWrapper.AsPropertyReference;
                    if( ( PropRefProp.RelationshipType == CswEnumNbtViewPropIdType.NodeTypePropId &&
                            PropRefProp.RelationshipId == CurrentProp.NodeTypePropId ) ||
                        ( PropRefProp.RelationshipType == CswEnumNbtViewPropIdType.ObjectClassPropId &&
                            PropRefProp.RelationshipId == CurrentProp.ObjectClassPropId ) )
                    {
                        PropRefProp.RecalculateReferenceValue();
                    }
                }
            }

            // 3. mark any property references to this property on other nodes as pending update
            if( CswTools.IsPrimaryKey( CurrentProp.NodeId ) )
            {
                //BZ 10239 - Fetch the cached value field name.
                CswNbtFieldTypeRulePropertyReference PropRefFTR = (CswNbtFieldTypeRulePropertyReference) _CswNbtResources.MetaData.getFieldTypeRule( CswEnumNbtFieldType.PropertyReference );
                CswEnumNbtPropColumn PropRefColumn = PropRefFTR.CachedValueSubField.Column;

                string SQL = @"update jct_nodes_props 
                            set pendingupdate = '" + CswConvert.ToDbVal( true ) + @"',
                                " + PropRefColumn.ToString() + @" = ''
                        where jctnodepropid in (select j.jctnodepropid
                                                    from jct_nodes_props j
                                                    join nodes n on n.nodeid = j.nodeid
                                                    join nodetype_props p on p.nodetypepropid = j.nodetypepropid
                                                    join field_types f on p.fieldtypeid = f.fieldtypeid
                                                    left outer join jct_nodes_props jntp on (jntp.nodetypepropid = p.fkvalue
                                                                                        and jntp.nodeid = n.nodeid
                                                                                        and jntp.field1_fk = " + CurrentProp.NodeId.PrimaryKey.ToString() + @")
                                                    left outer join (select jx.jctnodepropid, ox.objectclasspropid, jx.nodeid
                                                                        from jct_nodes_props jx
                                                                        join nodetype_props px on jx.nodetypepropid = px.nodetypepropid
                                                                        join object_class_props ox on px.objectclasspropid = ox.objectclasspropid
                                                                    where jx.field1_fk = " + CurrentProp.NodeId.PrimaryKey.ToString() + @") jocp 
                                                                                        on (jocp.objectclasspropid = p.fkvalue 
                                                                                        and jocp.nodeid = n.nodeid)
                                                    where f.fieldtype = 'PropertyReference'
                                                    and ((lower(p.fktype) = 'nodetypepropid' and jntp.jctnodepropid is not null)
                                                        or (lower(p.fktype) = 'objectclasspropid' and jocp.jctnodepropid is not null))
                                                    and ((lower(p.valueproptype) = 'nodetypepropid' and p.valuepropid = " + CurrentProp.NodeTypePropId.ToString() + @") 
                                                        or (lower(p.valueproptype) = 'objectclasspropid' and p.valuepropid = " + CurrentProp.ObjectClassPropId + @")))";

                // We're not doing this in a CswTableUpdate because it might be a large operation, 
                // and we don't care about auditing for this change.
                _CswNbtResources.execArbitraryPlatformNeutralSql( SQL );
            }

            // 4. For locations, if this node's location changed, we need to update the pathname on the children
            if( CurrentProp.getFieldTypeValue() == CswEnumNbtFieldType.Location &&
                CswTools.IsPrimaryKey( _CswNbtNode.NodeId ) )
            {
                _CswNbtNode.updateRelationsToThisNode();
            }
        }

        private void _validateCompoundUniqueProps( bool IsCopy )
        {
            List<CswNbtNodePropWrapper> CompoundUniqueProps = new List<CswNbtNodePropWrapper>();
            if( false == IsCopy )
            {
                //check for other compound unique props that were _not_ modififed
                foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
                {
                    if( CurrentProp.NodeTypeProp.IsCompoundUnique() )
                    {
                        CompoundUniqueProps.Add( CurrentProp );
                    }
                }
                if( CompoundUniqueProps.Count > 0 &&  NodeId != null )
                {
                    CswNbtView CswNbtView = this.NodeType.CreateDefaultView();
                    CswNbtView.ViewName = "For compound unique";

                    CswNbtViewRelationship ViewRelationship = CswNbtView.Root.ChildRelationships[0];

                    if( CswTools.IsPrimaryKey( NodeId ) )
                    {
                        ViewRelationship.NodeIdsToFilterOut.Add( NodeId );
                    }

                    foreach( CswNbtNodePropWrapper CurrentCompoundUniqueProp in CompoundUniqueProps )
                    {
                        //case 27670 - in order to reserve the right for compound unique props to be empty, it has to be explicitly stated when creating the ForCompundUnique view
                        CswNbtViewProperty CswNbtViewProperty = CswNbtView.AddViewProperty( ViewRelationship, CurrentCompoundUniqueProp.NodeTypeProp );
                        ICswNbtFieldTypeRule ftRule = CurrentCompoundUniqueProp.NodeTypeProp.getFieldTypeRule();
                        ftRule.AddUniqueFilterToView( CswNbtView, CswNbtViewProperty, CurrentCompoundUniqueProp, true );
                    }

                    ICswNbtTree NodeTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, CswNbtView, true, false, false );

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
                        string EsotericMessage = "The " + CompoundUniquePropNames.ToString() +
                                                    " of node " + NodeId.ToString() + " are the same as for node " + DuplicateValueNode.NodeId.ToString() + ": " + CompoundUniquePropValues.ToString();

                        if( false == _CswNbtNode.IsTemp && false == DuplicateValueNode.IsTemp ) //only throw an error if we're comparing two REAL nodes
                        {
                            throw ( new CswDniException( CswEnumErrorType.Warning, ExotericMessage, EsotericMessage ) );
                        }
                    }//we have a duplicate value situation
                } 
            }
            else//[9:55:20 AM 11/21/2013] Steven Salter: I can't think of any other situation [that unique props should be blanked]
            {
                foreach( CswNbtNodePropWrapper CurrentPropWrapper in CompoundUniqueProps )
                {
                    CurrentPropWrapper.ClearValue();
                    CurrentPropWrapper.clearSubFieldModifiedFlags();
                }
            } //if-else we're not a copy and not overridding
        }

        #endregion BeforeWriteNode

        #region AfterWriteNode

        public void afterWriteNode( bool OverrideMailReportEvents )
        {
            afterWriteNodeLogic();
            if( false == OverrideMailReportEvents )
            {
                _runMailReportEvents();
            }
        }//afterWriteNode()

        private void _runMailReportEvents()
        {
            Collection<CswNbtNodePropWrapper> ModifiedProps = new Collection<CswNbtNodePropWrapper>();
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CurrentProp.wasAnySubFieldModified( IncludePendingUpdate: false ) )
                {
                    ModifiedProps.Add( CurrentProp );
                }
            }
            if( ModifiedProps.Count > 0 )
            {
                _CswNbtResources.runMailReportEvents( this.NodeType, CswEnumNbtMailReportEventOption.Edit, _CswNbtNode, ModifiedProps );
            }
        }

        #endregion AfterWriteNode

        public void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes, bool ValidateRequiredRelationships )
        {
            beforeDeleteNodeLogic();
            if( ValidateRequiredRelationships )
            {
                // case 22486 - Don't allow deleting targets of required relationships
                CswTableSelect JctSelect = _CswNbtResources.makeCswTableSelect( "defaultBeforeDeleteNode_jnp_select", "jct_nodes_props" );
                string WhereClause = " where nodetypepropid in (select nodetypepropid from nodetype_props where isrequired = '1') and field1_fk = " + _CswNbtNode.NodeId.PrimaryKey.ToString();
                CswCommaDelimitedString SelectClause = new CswCommaDelimitedString() { "nodeid" };
                DataTable MatchTable = JctSelect.getTable( SelectClause, WhereClause );

                if( MatchTable.Rows.Count > 0 )
                {
                    CswCommaDelimitedString InUseStr = new CswCommaDelimitedString();
                    foreach( DataRow MatchRow in MatchTable.Rows )
                    {
                        CswPrimaryKey MatchNodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( MatchRow["nodeid"] ) );
                        if( DeleteAllRequiredRelatedNodes )
                        {
                            CswNbtNode NodeToDelete = _CswNbtResources.Nodes.GetNode( MatchNodePk );
                            if( null != NodeToDelete )
                            {
                                NodeToDelete.delete( DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
                            }
                        }
                        else
                        {
                            CswNbtNode RelatedNode = _CswNbtResources.Nodes[MatchNodePk];
                            if( null != RelatedNode )
                            {
                                InUseStr.Add( RelatedNode.NodeLink );
                            }

                        }
                    } // foreach( DataRow MatchRow in MatchTable.Rows )

                    if( false == DeleteAllRequiredRelatedNodes )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning,
                                                   "This " + _CswNbtNode.getNodeType().NodeTypeName +
                                                   " cannot be deleted because it is in use by: " + InUseStr,
                                                   "Current user (" + _CswNbtResources.CurrentUser.Username +
                                                   ") tried to delete a " + _CswNbtNode.getNodeType().NodeTypeName +
                                                   " that is in use by: " + InUseStr );
                    }
                } // if( MatchTable.Rows.Count > 0 )
            } // if( ValidateRequiredRelationships )
        } // baseBeforeDeleteNode()

        public void afterDeleteNode()
        {
            afterDeleteNodeLogic();
        }

        public void triggerAfterPopulateProps()
        {
            //We don't have a context for which Tab is going to render, but we can eliminate the base conditions for displaying the Save button here.
            //if( null != this.Node && false == canSave( TabId : Int32.MinValue ) )
            //{
            //    Save.setHidden( value : true, SaveToDb : false );
            //}
            afterPopulateProps();
        }

        protected virtual void afterPopulateProps() {}

        

        /// <summary>
        /// Save any properties before the Object Class Button Click Event is triggered
        /// </summary>
        private void _onBeforeButtonClickSaveProps( string TabId, NbtButtonData ButtonData )
        {
            Int32 TabIdAsInt = CswConvert.ToInt32( TabId );
            JObject SelectedTab = null;
            if( null != ButtonData.PropsToSave && ButtonData.PropsToSave.HasValues )
            {
                SelectedTab = CswConvert.ToJObject( ButtonData.PropsToSave[TabId] );
            }
            if( TabIdAsInt > 0 || ( null != SelectedTab && SelectedTab.HasValues ) )
            {
                if( canSave( TabId ) )
                {
                    CswNbtSdTabsAndProps Sd = new CswNbtSdTabsAndProps( _CswNbtResources );
                    ButtonData.Action = CswEnumNbtButtonAction.refresh;
                    Sd.saveProps( this.NodeId, TabIdAsInt, SelectedTab, this.NodeTypeId, null, false );
                }
            }
        }

        /// <summary>
        /// After the Object Class Button Click Event is triggered, determine whether any properties were part of the request. If so, return the updated properties for the provided tabs, if any.
        /// </summary>
        private void _onAfterButtonClickSaveProps( string TabId, NbtButtonData ButtonData )
        {
            Int32 TabIdAsInt = CswConvert.ToInt32( TabId );
            JObject SelectedTab = null;
            if( null != ButtonData.PropsToSave && ButtonData.PropsToSave.HasValues )
            {
                SelectedTab = CswConvert.ToJObject( ButtonData.PropsToSave[TabId] );
            }
            if( TabIdAsInt > 0 || ( null != SelectedTab && SelectedTab.HasValues ) )
            {
                CswNbtSdTabsAndProps Sd = new CswNbtSdTabsAndProps( _CswNbtResources );
                ButtonData.PropsToReturn = Sd.getProps( NodeId.ToString(), null, TabId, NodeTypeId, null, null, ForceReadOnly : false );
            }
        }

        public bool triggerOnButtonClick( NbtButtonData ButtonData )
        {
            bool Ret = false;
            //1: We have Button Data
            if( null != ButtonData )
            {
                //2: Before the Button Click, Save the Node if any tabs and properties have been provided
                Collection<Int32> TabIds = new Collection<int>();
                if( null != ButtonData.TabIds )
                {
                    TabIds = ButtonData.TabIds.ToIntCollection( ExcludeMinVal : true, ExcludeDuplicates : true );
                }

                if( TabIds.Count > 0 )
                {
                    foreach( Int32 TabId in TabIds )
                    {
                        _onBeforeButtonClickSaveProps( CswConvert.ToString( TabId ), ButtonData );
                    }
                }
                else
                {
                    if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                    {
                        //Client-side, we are defining a tabid as EditMode + '_tab'. This isn't great, but it's what we've got right now.
                        _onBeforeButtonClickSaveProps( CswEnumNbtNodeEditMode.Add + "_tab", ButtonData );
                    }
                }

                //3: If we're adding, we're always refreshing on add as the default option
                if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.refreshonadd;
                }

                //4: If this is the Save property, we're done; else execute the click event
                if( null != ButtonData.NodeTypeProp && ButtonData.NodeTypeProp.IsSaveProp )
                {
                    Ret = true;
                }
                else
                {
                    Ret = onButtonClick( ButtonData );
                }

                //5: If we had tabs or props coming in, we should get the current values to send back (they probably changed after the button click)
                if( TabIds.Count > 0 )
                {
                    foreach( Int32 TabId in TabIds )
                    {
                        _onAfterButtonClickSaveProps( CswConvert.ToString( TabId ), ButtonData );
                    }
                    if( ButtonData.NodeIds.Count > 1 )
                    {
                        Collection<CswPrimaryKey> MultiNodePks = new Collection<CswPrimaryKey>();
                        foreach( string CopyToNodeId in ButtonData.NodeIds )
                        {
                            CswPrimaryKey MultiNodePk = CswConvert.ToPrimaryKey( CopyToNodeId );
                            if( null != MultiNodePk && MultiNodePk != NodeId )
                            {
                                MultiNodePks.Add( MultiNodePk );
                            }
                        }
                        if( ButtonData.NodeIds.Count >= CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                        {
                            if( ButtonData.PropIds.Count > 0 )
                            {
                                Collection<Int32> NodeTypePropIds = new Collection<Int32>();
                                foreach( string PropIdAttrStr in ButtonData.PropIds )
                                {
                                    CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdAttrStr );
                                    NodeTypePropIds.Add( PropIdAttr.NodeTypePropId );
                                }
                                CswNbtBatchOpMultiEdit op = new CswNbtBatchOpMultiEdit( _CswNbtResources );
                                CswNbtObjClassBatchOp Batch = op.makeBatchOp( Node, MultiNodePks, NodeTypePropIds );
                                ButtonData.Action = CswEnumNbtButtonAction.batchop;
                                ButtonData.Data["batch"] = Batch.Node.NodeLink;
                            }
                            if( ButtonData.MultiClick && null != ButtonData.NodeTypeProp )
                            {
                                CswNbtBatchOpMultiButtonClick op = new CswNbtBatchOpMultiButtonClick( _CswNbtResources );
                                CswNbtObjClassBatchOp Batch = op.makeBatchOp( MultiNodePks, ButtonData.NodeTypeProp.PropId );
                                ButtonData.Action = CswEnumNbtButtonAction.batchop;
                                ButtonData.Data["batch"] = Batch.Node.NodeLink;
                            }
                        }
                        else
                        {
                            if( ButtonData.PropIds.Count > 0 )
                            {
                                CswNbtSdTabsAndProps Sd = new CswNbtSdTabsAndProps( _CswNbtResources );
                                Sd.copyPropValues( Node, ButtonData.NodeIds, ButtonData.PropIds );
                            }
                            if( ButtonData.MultiClick )
                            {
                                foreach( CswPrimaryKey MultiNodeId in MultiNodePks )
                                {
                                    CswNbtNode MultiNode = _CswNbtResources.Nodes[MultiNodeId];
                                    if( null != MultiNode )
                                    {
                                        MultiNode.ObjClass.onButtonClick( ButtonData );
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                    {
                        _onAfterButtonClickSaveProps( CswEnumNbtNodeEditMode.Add + "_tab", ButtonData );
                    }
                }
            }
            return Ret;
        }

        public virtual CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            return CopyNodeImpl( IsNodeTemp, OnCopy );
        }
        protected CswNbtNode CopyNodeImpl( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, IsTemp : IsNodeTemp, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    NewNode.copyPropertyValues( Node );
                    if( null != OnCopy )
                    {
                        OnCopy( NewNode );
                    }
                }, IsCopy: true );
            return CopiedNode;
        }

        #endregion Base Node Event Logic

        public abstract class PropertyName
        {
            public const string Save = "Save";
            public const string LegacyId = "Legacy Id";
        }

        public virtual CswNbtNodePropButton Save
        {
            get
            {
                CswNbtNodePropButton Ret = Node.Properties[PropertyName.Save];

                return Ret;
            }
        }

        public virtual CswNbtNodePropNumber LegacyId
        {
            get
            {
                CswNbtNodePropNumber Ret = Node.Properties[PropertyName.LegacyId];
                return Ret;
            }
        }

        public Int32 NodeTypeId { get { return _CswNbtNode.NodeTypeId; } }
        public CswNbtMetaDataNodeType NodeType { get { return _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId ); } }
        public string NodeName { get { return _CswNbtNode.NodeName; } }
        public CswPrimaryKey NodeId { get { return _CswNbtNode.NodeId; } }
        public CswNbtNode Node { get { return _CswNbtNode; } }
        public bool IsDemo { get { return _CswNbtNode.IsDemo; } set { _CswNbtNode.IsDemo = value; } }
        public bool IsTemp { get { return _CswNbtNode.IsTemp; } } //set { _CswNbtNode.IsTemp = value; } }

        public CswPrimaryKey RelationalId
        {
            get { return _CswNbtNode.RelationalId; }
            set { _CswNbtNode.RelationalId = value; }
        }

        public class NbtButtonData
        {
            public NbtButtonData( CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            {
                Data = new JObject();
                Action = CswEnumNbtButtonAction.Unknown;

                Debug.Assert( null != CswNbtMetaDataNodeTypeProp, "CswNbtMetaDataNodeTypeProp is null." );
                if( null == CswNbtMetaDataNodeTypeProp )
                {
                    throw new CswDniException( "Property is unknown." );
                }
                NodeTypeProp = CswNbtMetaDataNodeTypeProp;
            }
            public void clone( NbtButtonData DataToCopy )
            {
                if( null != DataToCopy )
                {
                    if( null != DataToCopy.Action )
                    {
                        Action = DataToCopy.Action;
                    }
                    if( null != DataToCopy.SelectedText )
                    {
                        SelectedText = DataToCopy.SelectedText;
                    }
                    if( null != DataToCopy.Data )
                    {
                        Data = DataToCopy.Data;
                    }
                    if( null != DataToCopy.Message )
                    {
                        Message = DataToCopy.Message;
                    }
                }
            }

            public CswEnumNbtButtonAction Action = CswEnumNbtButtonAction.nothing;
            public string SelectedText = string.Empty;
            public CswNbtMetaDataNodeTypeProp NodeTypeProp = null;
            public JObject Data = new JObject();
            public JObject PropsToSave = new JObject();
            public JObject PropsToReturn = new JObject();
            public CswCommaDelimitedString TabIds = new CswCommaDelimitedString();
            public string Message = string.Empty;
            public CswCommaDelimitedString NodeIds = new CswCommaDelimitedString();
            public CswCommaDelimitedString PropIds = new CswCommaDelimitedString();
            public bool MultiClick = false;
        }

        // For validating object class casting
        protected static bool _Validate( CswNbtNode Node, CswEnumNbtObjectClass TargetObjectClass )
        {
            if( Node == null )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid node", "CswNbtObjClass._Validate was given a null node as a parameter" );
            }

            if( !( Node.getObjectClass().ObjectClass == TargetObjectClass ) )
            {
                throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as " + TargetObjectClass.ToString() + "; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
            }
            return true;
        }

    }//CswNbtObjClass

}//namespace ChemSW.Nbt.ObjClasses
