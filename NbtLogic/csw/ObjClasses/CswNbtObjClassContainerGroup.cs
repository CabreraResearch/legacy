using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainerGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Barcode = "Barcode";
            public const string SyncLocation = "Sync Location";
            public const string Location = "Location";
        }

        public CswNbtObjClassContainerGroup( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerGroup
        /// </summary>
        public static implicit operator CswNbtObjClassContainerGroup( CswNbtNode Node )
        {
            CswNbtObjClassContainerGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ContainerGroupClass ) )
            {
                ret = (CswNbtObjClassContainerGroup) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( CswEnumTristate.True == this.SyncLocation.Checked && ( this.Location.wasAnySubFieldModified() || this.SyncLocation.wasAnySubFieldModified() ) )
            {
                _setContainerLocations();
            }
        }//beforeWriteNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropBarcode Barcode { get { return _CswNbtNode.Properties[PropertyName.Barcode]; } }
        public CswNbtNodePropLogical SyncLocation { get { return _CswNbtNode.Properties[PropertyName.SyncLocation]; } }
        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }

        #endregion

        private void _setContainerLocations()
        {
            IEnumerable<CswPrimaryKey> ContainerNodePks = this.getContainersInGroup();
            if( ContainerNodePks.Count() > 0 )
            {
                int BatchThreshold = CswNbtBatchManager.getBatchThreshold( _CswNbtResources );
                if( ContainerNodePks.Count() > BatchThreshold )
                {
                    // Shelve this to a batch operation
                    CswNbtBatchOpSyncLocation op = new CswNbtBatchOpSyncLocation( _CswNbtResources );
                    CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( ContainerNodePks, this.Location.SelectedNodeId );
                }
                else
                {
                    foreach( CswPrimaryKey CurrentContainerNodePk in ContainerNodePks )
                    {
                        CswNbtObjClassContainer CurrentContainer = _CswNbtResources.Nodes[CurrentContainerNodePk];
                        if( null != CurrentContainer )
                        {
                            CurrentContainer.Location.SelectedNodeId = this.Location.SelectedNodeId;
                            CurrentContainer.postChanges( false );
                        }
                    }
                }
            }
        }

        public IEnumerable<CswPrimaryKey> getContainersInGroup()
        {
            CswNbtView ContainersInGroupView = new CswNbtView( _CswNbtResources );
            ContainersInGroupView.ViewName = "ContainersInGroup";

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtViewRelationship Rel1 = ContainersInGroupView.AddViewRelationship( ContainerOC, true );

            CswNbtMetaDataObjectClassProp ContainerGroupOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ContainerGroup );
            CswNbtViewProperty Prop2 = ContainersInGroupView.AddViewProperty( Rel1, ContainerGroupOCP );
            CswNbtViewPropertyFilter Filt3 = ContainersInGroupView.AddViewPropertyFilter( Prop2,
                                                      CswEnumNbtFilterConjunction.And,
                                                      CswEnumNbtFilterResultMode.Hide,
                                                      CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                      CswEnumNbtFilterMode.Equals,
                                                      this.NodeId.PrimaryKey.ToString(),
                                                      false,
                                                      false );

            Collection<CswPrimaryKey> _ContainerGroupNodePks = new Collection<CswPrimaryKey>();

            ICswNbtTree ContainersInGroupTree = _CswNbtResources.Trees.getTreeFromView( ContainersInGroupView, false, true, true );
            ContainersInGroupTree.goToRoot();
            for( int i = 0; i < ContainersInGroupTree.getChildNodeCount(); i++ )
            {
                ContainersInGroupTree.goToNthChild( i );
                _ContainerGroupNodePks.Add( ContainersInGroupTree.getNodeIdForCurrentPosition() );
                ContainersInGroupTree.goToParentNode();

            }

            return _ContainerGroupNodePks;
        }

    }//CswNbtObjClassContainerGroup

}//namespace ChemSW.Nbt.ObjClasses
