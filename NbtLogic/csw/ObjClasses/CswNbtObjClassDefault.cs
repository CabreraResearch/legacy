using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtResources.UnknownEnum ); }
        }

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            List<CswNbtNodePropWrapper> CompoundUniqueProps = new List<CswNbtNodePropWrapper>();
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CurrentProp.getAnySubFieldModified() )
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

                    // case 30350 - this is very expensive for multiple nodes, and unnecessary on create.  So skip it.
                    if( false == Creating )
                    {
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
                            _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.updateRelationsToThisNode( _CswNbtNode );
                        }
                    } // if( false == _IsCreate )

                    // 5. Prepare for compound unique validation
                    if( CurrentProp.NodeTypeProp.IsCompoundUnique() )
                    {
                        CompoundUniqueProps.Add( CurrentProp );
                    }
                } // if(CurrentProp.WasModified)
            } // foreach (CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties)

            if( CompoundUniqueProps.Count > 0 && NodeId != null )
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

                    } //we have a duplicate value situation
                }

                else
                {
                    foreach( CswNbtNodePropWrapper CurrentPropWrapper in CompoundUniqueProps )
                    {
                        CurrentPropWrapper.ClearValue();
                        CurrentPropWrapper.clearSubFieldModifiedFlags();
                    }

                } //if-else we're not a copy and not overridding

            } //if we have at leaste one modified compound unique prop

            //_synchNodeName();
            // can't do this here, because we miss some of the onBeforeUpdateNodePropRow events
            // we do it in writer now instead
        } // beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            // BZ 10094 - Notification event
            Collection<CswNbtNodePropWrapper> ModifiedProps = new Collection<CswNbtNodePropWrapper>();
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CurrentProp.getAnySubFieldModified( IncludePendingUpdate: false ) )
                {
                    ModifiedProps.Add( CurrentProp );
                }
            }
            if( ModifiedProps.Count > 0 )
            {
                _CswNbtResources.runMailReportEvents( this.NodeType, CswEnumNbtMailReportEventOption.Edit, _CswNbtNode, ModifiedProps );
            }
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
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
                }
                if( false == DeleteAllRequiredRelatedNodes )
                {
                    throw new CswDniException( CswEnumErrorType.Warning,
                                              "This " + _CswNbtNode.getNodeType().NodeTypeName +
                                              " cannot be deleted because it is in use by: " + InUseStr,
                                              "Current user (" + _CswNbtResources.CurrentUser.Username +
                                              ") tried to delete a " + _CswNbtNode.getNodeType().NodeTypeName +
                                              " that is in use by: " + InUseStr );
                }
            }
        } // beforeDeleteNode()

        public override void afterDeleteNode()
        {
        }

        protected override void afterPopulateProps()
        {
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
    }//CswNbtObjClassDefault

}//namespace ChemSW.Nbt.ObjClasses
