using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// PrintJob Object Class
    /// </summary>
    public class CswNbtObjClassPrintJob : CswNbtObjClass
    {
        /// <summary>
        /// Property names on the PrintJob class
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string JobNo = "Job No";
            public const string Printer = "Printer";
            public const string CreatedDate = "Created Date";
            public const string ProcessedDate = "Processed Date";
            public const string EndedDate = "Ended Date";
            public const string RequestedBy = "Requested By";
            public const string Label = "Label";
            public const string LabelCount = "Label Count";
            public const string LabelData = "Label Data";
            public const string JobState = "Job State";
            public const string ErrorInfo = "Error Info";
        }

        public sealed class StateOption
        {
            public const string Pending = "Pending";
            public const string Processing = "Processing";
            public const string Closed = "Closed";
            public const string Error = "Error";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPrintJob( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.PrintJobClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrintJob
        /// </summary>
        public static implicit operator CswNbtObjClassPrintJob( CswNbtNode Node )
        {
            CswNbtObjClassPrintJob ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.PrintJobClass ) )
            {
                ret = (CswNbtObjClassPrintJob) Node.ObjClass;
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
            //NodeTypes.SetOnPropChange( OnNodeTypesPropChange );
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

        public CswNbtNodePropSequence JobNo { get { return _CswNbtNode.Properties[PropertyName.JobNo]; } }
        public CswNbtNodePropRelationship Printer { get { return _CswNbtNode.Properties[PropertyName.Printer]; } }
        public CswNbtNodePropDateTime CreatedDate { get { return _CswNbtNode.Properties[PropertyName.CreatedDate]; } }
        public CswNbtNodePropDateTime ProcessedDate { get { return _CswNbtNode.Properties[PropertyName.ProcessedDate]; } }
        public CswNbtNodePropDateTime EndedDate { get { return _CswNbtNode.Properties[PropertyName.EndedDate]; } }
        public CswNbtNodePropRelationship RequestedBy { get { return _CswNbtNode.Properties[PropertyName.RequestedBy]; } }
        public CswNbtNodePropRelationship Label { get { return _CswNbtNode.Properties[PropertyName.Label]; } }
        public CswNbtNodePropNumber LabelCount { get { return _CswNbtNode.Properties[PropertyName.LabelCount]; } }
        public CswNbtNodePropMemo LabelData { get { return _CswNbtNode.Properties[PropertyName.LabelData]; } }
        public CswNbtNodePropList JobState { get { return _CswNbtNode.Properties[PropertyName.JobState]; } }
        public CswNbtNodePropMemo ErrorInfo { get { return _CswNbtNode.Properties[PropertyName.ErrorInfo]; } }

        #endregion

    }//CswNbtObjClassPrintJob

}//namespace ChemSW.Nbt.ObjClasses
