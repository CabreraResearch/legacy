
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodeUpdateEvents
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtNode _CswNbtNode;

        /// <summary>
        /// Contains functions to perform related to the given node being updated
        /// Note - these functions do not change the node; rather, they notify other nodes that this node has updated
        /// </summary>
        public CswNbtNodeUpdateEvents( CswNbtResources CswNbtResources, CswNbtNode Node )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNode = Node;
        }

        /// <summary>
        /// Executes functions to perform related to the given node being updated
        /// Note - these functions do not change the node; rather, they notify other nodes that this node has updated
        /// </summary>
        public void triggerUpdateEvents()
        {
            Collection<CswNbtNodePropWrapper> ModifiedProps = new Collection<CswNbtNodePropWrapper>();
            foreach( CswNbtNodePropWrapper CurrentProp in _CswNbtNode.Properties )
            {
                if( CswTools.IsPrimaryKey( CurrentProp.NodeId ) &&
                    //If we run this function outside the scope of updating the node, we need to check every property
                    ( _CswNbtNode.ModificationState == CswEnumNbtNodeModificationState.Modified && 
                      CurrentProp.wasAnySubFieldModified()) )
                {
                    _markExternalPropRefsDirty( CurrentProp );
                    if( CurrentProp.getFieldTypeValue() == CswEnumNbtFieldType.Location )
                    {
                        _CswNbtNode.updateRelationsToThisNode();
                    }
                    ModifiedProps.Add( CurrentProp );
                }
            }
            if( ModifiedProps.Count > 0 )
            {
                _CswNbtResources.runMailReportEvents( _CswNbtNode.NodeTypeId, CswEnumNbtMailReportEventOption.Edit, _CswNbtNode, ModifiedProps );
            }
        }

        //mark any property references to this property on other nodes as pending update
        private void _markExternalPropRefsDirty( CswNbtNodePropWrapper CurrentProp )
        {
            if( CswTools.IsPrimaryKey( CurrentProp.NodeId ) )
            {
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
        }
    }
}
