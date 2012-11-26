using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainerGroup : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Name = "Name";
            public const string Barcode = "Barcode";
            public const string SyncLocation = "Sync Location";
            public const string Location = "Location";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainerGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerGroup
        /// </summary>
        public static implicit operator CswNbtObjClassContainerGroup( CswNbtNode Node )
        {
            CswNbtObjClassContainerGroup ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.ContainerGroupClass ) )
            {
                ret = (CswNbtObjClassContainerGroup) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( Tristate.True == this.SyncLocation.Checked && ( this.Location.WasModified || this.SyncLocation.WasModified ) )
            {
                _setContainerLocations();
            }


            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {

            _CswNbtObjClassDefault.afterPopulateProps();

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
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
                    Collection<CswPrimaryKey> ContainerPkCollection = new Collection<CswPrimaryKey>();
                    foreach( CswPrimaryKey ContainerPk in ContainerNodePks )
                    {
                        ContainerPkCollection.Add( ContainerPk );
                    }
                    CswNbtBatchOpSyncLocation op = new CswNbtBatchOpSyncLocation( _CswNbtResources );
                    CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( ContainerPkCollection, this.Location.SelectedNodeId );
                }
                else
                {
                    foreach( CswPrimaryKey CurrentContainerNodePk in ContainerNodePks )
                    {
                        CswNbtObjClassContainer CurrentContainer = _CswNbtResources.Nodes[CurrentContainerNodePk];
                        CurrentContainer.Location.SelectedNodeId = this.Location.SelectedNodeId;
                        CurrentContainer.postChanges( false );
                    }
                }
            }
        }

        public IEnumerable<CswPrimaryKey> getContainersInGroup()
        {
            CswNbtView ContainersInGroupView = new CswNbtView( _CswNbtResources );
            ContainersInGroupView.ViewName = "ContainersInGroup";

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtViewRelationship Rel1 = ContainersInGroupView.AddViewRelationship( ContainerOC, true );

            CswNbtMetaDataObjectClassProp ContainerGroupOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ContainerGroup );
            CswNbtViewProperty Prop2 = ContainersInGroupView.AddViewProperty( Rel1, ContainerGroupOCP );
            CswNbtViewPropertyFilter Filt3 = ContainersInGroupView.AddViewPropertyFilter( Prop2,
                                                      CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                      CswNbtPropFilterSql.FilterResultMode.Hide,
                                                      CswNbtSubField.SubFieldName.NodeID,
                                                      CswNbtPropFilterSql.PropertyFilterMode.Equals,
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
