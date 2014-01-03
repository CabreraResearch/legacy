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

        public CswNbtObjClassPrintJob( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintJobClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrintJob
        /// </summary>
        public static implicit operator CswNbtObjClassPrintJob( CswNbtNode Node )
        {
            CswNbtObjClassPrintJob ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.PrintJobClass ) )
            {
                ret = (CswNbtObjClassPrintJob) Node.ObjClass;
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

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp JobStateOcp = ObjectClass.getObjectClassProp( PropertyName.JobState );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, JobStateOcp, Value: StateOption.Closed, FilterMode: CswEnumNbtFilterMode.NotEquals, ShowAtRuntime: true );
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
