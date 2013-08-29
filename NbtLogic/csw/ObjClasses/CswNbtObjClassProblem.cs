using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassProblem : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Parts = "Parts";
            public const string Owner = "Owner";
            public const string DateOpened = "Date Opened";
            public const string DateClosed = "Date Closed";
            public const string Closed = "Closed";
            public const string ReportedBy = "Reported By";
            public const string Failure = "Failure";
            public const string Department = "Department";
            public const string LaborCost = "Labor Cost";
            public const string OtherCost = "Other Cost";
            public const string OtherCostName = "Other Cost Name";
            public const string PartsCost = "Parts Cost";
            public const string Problem = "Problem";
            public const string ReporterPhone = "Reporter Phone";
            public const string Resolution = "Resolution";
            public const string StartDate = "Start Date";
            public const string Summary = "Summary";
            public const string Technician = "Technician";
            public const string TechnicianPhone = "Technician Phone";
            public const string TravelCost = "Travel Cost";
            public const string UnderWarranty = "Under Warranty";
            public const string WorkOrderPrinted = "Work Order Printed";
        }


        public const string PartsXValueName = "Service";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassProblem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ProblemClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassProblem
        /// </summary>
        public static implicit operator CswNbtObjClassProblem( CswNbtNode Node )
        {
            CswNbtObjClassProblem ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ProblemClass ) )
            {
                ret = (CswNbtObjClassProblem) Node.ObjClass;
            }
            return ret;
        }

        private void _setDefaultValues()
        {
            if( false == CswTools.IsPrimaryKey( ReportedBy.RelatedNodeId ) &&
                CswTools.IsPrimaryKey( _CswNbtResources.CurrentNbtUser.UserId ) )
            {
                ReportedBy.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                ReportedBy.CachedNodeName = _CswNbtResources.CurrentNbtUser.Username;
            }
            if( DateOpened.DateTimeValue == DateTime.MinValue )
            {
                DateOpened.DateTimeValue = DateTime.Now;
            }
        }

        private void _checkClosed()
        {
            // BZ 10051 - If we're closing the Problem, set the Date Closed to today
            if( Closed.Checked == CswEnumTristate.True && DateClosed.DateTimeValue == DateTime.MinValue )
                DateClosed.DateTimeValue = DateTime.Today;

            // case 25838 - don't clear existing values
            //// BZ 10051 - If we're reopening the Problem, clear the Date Closed
            //if( Closed.Checked == Tristate.False && DateClosed.DateTimeValue != DateTime.MinValue )
            //    DateClosed.DateTimeValue = DateTime.MinValue;
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
            _checkClosed();
            _setDefaultValues();
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
            if( Owner.RelatedNodeId != null )
            {
                CswNbtNode EquipmentOrAssemblyNode = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( EquipmentOrAssemblyNode != null )
                {
                    if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.EquipmentClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipment EquipmentNodeAsEquipment = (CswNbtObjClassEquipment) EquipmentOrAssemblyNode;
                        //CswNbtObjClassEquipment Equipment = CswNbtObjClassFactory.Make( CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentClass ) as CswNbtObjClassEquipment;
                        foreach( string YValue in EquipmentNodeAsEquipment.Parts.YValues )
                        {
                            if( EquipmentNodeAsEquipment.Parts.CheckValue( CswNbtObjClassEquipment.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                    else if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipmentAssembly AssemblyNodeAsAssembly = (CswNbtObjClassEquipmentAssembly) EquipmentOrAssemblyNode;
                        foreach( string YValue in AssemblyNodeAsAssembly.AssemblyParts.YValues )
                        {
                            if( AssemblyNodeAsAssembly.AssemblyParts.CheckValue( CswNbtObjClassEquipmentAssembly.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                } // if( EquipmentOrAssemblyNode != null )
            } // if( Owner.RelatedNodeId != null )

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

        //public CswNbtNodePropRelationship Equipment
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[EquipmentPropertyName].AsRelationship );
        //    }
        //}
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }

        public CswNbtNodePropRelationship ReportedBy { get { return ( _CswNbtNode.Properties[PropertyName.ReportedBy] ); } }

        public CswNbtNodePropLogicalSet Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }
        public CswNbtNodePropLogical Closed { get { return ( _CswNbtNode.Properties[PropertyName.Closed] ); } }

        public CswNbtNodePropDateTime DateOpened { get { return ( _CswNbtNode.Properties[PropertyName.DateOpened] ); } }

        public CswNbtNodePropDateTime DateClosed { get { return ( _CswNbtNode.Properties[PropertyName.DateClosed] ); } }
        public CswNbtNodePropLogical Failure { get { return ( _CswNbtNode.Properties[PropertyName.Failure] ); } }

        public CswNbtNodePropRelationship Department { get { return ( _CswNbtNode.Properties[PropertyName.Department] ); } }
        public CswNbtNodePropText LaborCost { get { return ( _CswNbtNode.Properties[PropertyName.LaborCost] ); } }
        public CswNbtNodePropText OtherCost { get { return ( _CswNbtNode.Properties[PropertyName.OtherCost] ); } }
        public CswNbtNodePropText OtherCostName { get { return ( _CswNbtNode.Properties[PropertyName.OtherCostName] ); } }
        public CswNbtNodePropText PartsCost { get { return ( _CswNbtNode.Properties[PropertyName.PartsCost] ); } }
        public CswNbtNodePropMemo Problem { get { return ( _CswNbtNode.Properties[PropertyName.Problem] ); } }
        public CswNbtNodePropPropertyReference ReporterPhone { get { return ( _CswNbtNode.Properties[PropertyName.ReporterPhone] ); } }
        public CswNbtNodePropMemo Resolution { get { return ( _CswNbtNode.Properties[PropertyName.Resolution] ); } }
        public CswNbtNodePropDateTime StartDate { get { return ( _CswNbtNode.Properties[PropertyName.StartDate] ); } }
        public CswNbtNodePropText Summary { get { return ( _CswNbtNode.Properties[PropertyName.Summary] ); } }
        public CswNbtNodePropRelationship Technician { get { return ( _CswNbtNode.Properties[PropertyName.Technician] ); } }
        public CswNbtNodePropPropertyReference TechnicianPhone { get { return ( _CswNbtNode.Properties[PropertyName.TechnicianPhone] ); } }
        public CswNbtNodePropText TravelCost { get { return ( _CswNbtNode.Properties[PropertyName.TravelCost] ); } }
        public CswNbtNodePropLogical UnderWarranty { get { return ( _CswNbtNode.Properties[PropertyName.UnderWarranty] ); } }
        public CswNbtNodePropLogical WorkOrderPrinted { get { return ( _CswNbtNode.Properties[PropertyName.WorkOrderPrinted] ); } }

        #endregion

    }//CswNbtObjClassProblem

}//namespace ChemSW.Nbt.ObjClasses
