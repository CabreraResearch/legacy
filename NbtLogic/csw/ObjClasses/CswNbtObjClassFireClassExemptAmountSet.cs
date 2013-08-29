using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFireClassExemptAmountSet : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string SetName = "Set Name";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;

        public CswNbtObjClassFireClassExemptAmountSet( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFireClassExemptAmountSet
        /// </summary>
        public static implicit operator CswNbtObjClassFireClassExemptAmountSet( CswNbtNode Node )
        {
            CswNbtObjClassFireClassExemptAmountSet ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.FireClassExemptAmountSetClass ) )
            {
                ret = (CswNbtObjClassFireClassExemptAmountSet) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override CswNbtNode CopyNode()
        {
            CswNbtNode CopiedFireClassExemptAmountSetNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    NewNode.copyPropertyValues( Node );
                    // CopiedFireClassExemptAmountSetNode.postChanges( true, true );
                } );

            // Copy all Related FireClassExemptAmount Nodes
            CswNbtMetaDataObjectClass FireClassExemptAmountObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountClass );
            CswNbtView FCEAView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship FCEARelationship = FCEAView.AddViewRelationship( FireClassExemptAmountObjectClass, false );
            CswNbtViewProperty SetNameProperty = FCEAView.AddViewProperty( FCEARelationship, FireClassExemptAmountObjectClass.getObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.SetName ) );
            FCEAView.AddViewPropertyFilter(
                SetNameProperty,
                CswEnumNbtSubFieldName.NodeID,
                CswEnumNbtFilterMode.Equals,
                NodeId.PrimaryKey.ToString() );

            ICswNbtTree FCEATree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, FCEAView, true, false, false );
            FCEATree.goToRoot();
            Int32 ChildrenCopied = 0;
            while( ChildrenCopied < FCEATree.getChildNodeCount() )
            {
                FCEATree.goToNthChild( ChildrenCopied );
                CswNbtObjClassFireClassExemptAmount OriginalFCEANode = FCEATree.getNodeForCurrentPosition();
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalFCEANode.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        NewNode.copyPropertyValues( OriginalFCEANode.Node );
                        ( (CswNbtObjClassFireClassExemptAmount) NewNode ).SetName.RelatedNodeId = CopiedFireClassExemptAmountSetNode.NodeId;
                        //CopiedFCEANode.postChanges( true );
                    } );
                FCEATree.goToParentNode();
                ChildrenCopied++;
            }

            return CopiedFireClassExemptAmountSetNode;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText SetName { get { return _CswNbtNode.Properties[PropertyName.SetName]; } }

        #endregion

    }//CswNbtObjClassFireClassExemptAmountSet

}//namespace ChemSW.Nbt.ObjClasses
