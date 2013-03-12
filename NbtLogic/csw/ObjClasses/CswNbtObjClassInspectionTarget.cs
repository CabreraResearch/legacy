using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTarget : CswNbtObjClass, ICswNbtPropertySetInspectionParent
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            //public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
            public const string Status = "Status";
            public const string Location = "Location";
            public const string Description = "Description";
            public const string Barcode = "Barcode";
            public const string InspectionTargetGroup = "Inspection Target Group";
        }

        public string InspectionParentStatusPropertyName { get { return PropertyName.Status; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionTarget( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionTargetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTarget
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTarget( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTarget ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.InspectionTargetClass ) )
            {
                ret = (CswNbtObjClassInspectionTarget) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

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
            CswNbtObjClassInspectionTarget CopiedInspectionTargetNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedInspectionTargetNode.Node.copyPropertyValues( Node );
            CopiedInspectionTargetNode.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );
            CopiedInspectionTargetNode.postChanges( true );
            return CopiedInspectionTargetNode.Node;
        }

        #endregion

        #region Object class specific properties

        /// <summary>
        /// Inspection Target Inspection Status (OK, Deficient)
        /// </summary>
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }

        /// <summary>
        /// Location of Inspection Target
        /// </summary>
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropText Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropRelationship InspectionTargetGroup
        {
            get { return ( _CswNbtNode.Properties[PropertyName.InspectionTargetGroup] ); }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
