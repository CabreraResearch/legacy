using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Print Label Object Class
    /// </summary>
    public class CswNbtObjClassPrintLabel: CswNbtObjClass
    {
        /// <summary>
        /// Property names on the Print Label class
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string EplText = "epltext";
            public const string Params = "params";
            public const string NodeTypes = "NodeTypes";
            public const string LabelName = "Label Name";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPrintLabel( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrintLabel
        /// </summary>
        public static implicit operator CswNbtObjClassPrintLabel( CswNbtNode Node )
        {
            CswNbtObjClassPrintLabel ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.PrintLabelClass ) )
            {
                ret = (CswNbtObjClassPrintLabel) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

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
            NodeTypes.SetOnPropChange( OnNodeTypesPropChange );
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
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo EplText { get { return _CswNbtNode.Properties[PropertyName.EplText]; } }
        public CswNbtNodePropMemo Params { get { return _CswNbtNode.Properties[PropertyName.Params]; } }
        public CswNbtNodePropNodeTypeSelect NodeTypes { get { return _CswNbtNode.Properties[PropertyName.NodeTypes]; } }
        /// <summary>
        /// When a Print Label's NodeTypeSelect's property value has changed, update the HasLabel value for each NodeType in the system.
        /// Aside: You either have to eat the cost here on Print Label save or on the HasValue get; I think it's cheaper to do it here.
        /// </summary>
        public static void updateLabels( CswNbtResources Resources )
        {
            CswNbtMetaDataObjectClass PrintLabelOc = Resources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            Collection<Int32> SelectedNodeTypeIds = new Collection<Int32>();
            foreach( CswNbtObjClassPrintLabel Node in PrintLabelOc.getNodes( forceReInit : true, includeSystemNodes : false ) )
            {
                if( null != Node &&
                    null != Node.NodeTypes.SelectedNodeTypeIds &&
                    Node.NodeTypes.SelectedNodeTypeIds.Count > 0 )
                {
                    foreach( Int32 SelectedNodeTypeid in Node.NodeTypes.SelectedNodeTypeIds.ToIntCollection() )
                    {
                        if( false == SelectedNodeTypeIds.Contains( SelectedNodeTypeid ) )
                        {
                            SelectedNodeTypeIds.Add( SelectedNodeTypeid );
                        }
                    }
                }
            }

            foreach( CswNbtMetaDataNodeType NodeType in Resources.MetaData.getNodeTypes() )
            {
                NodeType.HasLabel = ( SelectedNodeTypeIds.Contains( NodeType.FirstVersionNodeTypeId ) ||
                    SelectedNodeTypeIds.Contains( NodeType.NodeTypeId ) ||
                    SelectedNodeTypeIds.Contains( NodeType.getNodeTypeLatestVersion().NodeTypeId ) );
            }
        }
        private void OnNodeTypesPropChange( CswNbtNodeProp NodeProp )
        {
            updateLabels( _CswNbtResources );
        }

        public CswNbtNodePropText LabelName { get { return _CswNbtNode.Properties[PropertyName.LabelName]; } }

        #endregion

    }//CswNbtObjClassPrintLabel

}//namespace ChemSW.Nbt.ObjClasses
