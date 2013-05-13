using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassTask : CswNbtPropertySetGeneratorTarget
    {
        public new sealed class PropertyName : CswNbtPropertySetGeneratorTarget.PropertyName
        {
            public const string Completed = "Completed";
            public const string Description = "Description";
            public const string Owner = "Owner";
            public const string Summary = "Summary";
            public const string DoneOn = "Done On";
            public const string Parts = "Parts";
            public const string PartsXValue = "Service";
            public const string CalibrationDate = "Calibration Date";
            public const string CalibrationResult = "Calibration Result";
            public const string CompletionDescription = "Completion Description";
            public const string Department = "Department";
            public const string EventType = "Event Type";
            public const string FileLink = "File Link";
            public const string IgnoreCalibrationResult = "Ignore Calibration Result";
            public const string IsCriticalTest = "Is Critical Test";
            public const string LaborCost = "Labor Cost";
            public const string LowerLimit = "Lower Limit";
            public const string OtherCost = "Other Cost";
            public const string OtherCostName = "Other Cost Name";
            public const string PartsCost = "Parts Cost";
            public const string SOPRef = "SOP/Ref#";
            public const string Technician = "Technician";
            public const string TechnicianPhone = "Technician Phone";
            public const string TravelCost = "Travel Cost";
            public const string UpperLimit = "Upper Limit";
        }
        
        // for CswNbtPropertySetGeneratorTarget
        public override string ParentPropertyName { get { return PropertyName.Owner; } }

        public CswNbtObjClassTask( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.TaskClass ); }
        }

        //BZ 10247
        private void setDoneOnDate()
        {
            if( Completed.Checked == CswEnumTristate.True && DoneOn.DateTimeValue == DateTime.MinValue )
            {
                DoneOn.DateTimeValue = DateTime.Now;
            }

            // case 25838 - don't clear existing values
            //if( Completed.Checked == Tristate.False && DoneOn.DateTimeValue != DateTime.MinValue )
            //    DoneOn.DateTimeValue = DateTime.MinValue;
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassTask
        /// </summary>
        public static implicit operator CswNbtObjClassTask( CswNbtNode Node )
        {
            CswNbtObjClassTask ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.TaskClass ) )
            {
                ret = (CswNbtObjClassTask) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            setDoneOnDate();
            // Set the IsFuture flag = false if the node is modified
            if( !_CswNbtNode.New && ( CswEnumNbtNodeModificationState.Modified == _CswNbtNode.ModificationState ) )
            {
                //if someone set the flag deliberately don't mess with it
                if( !_CswNbtNode.Properties[PropertyName.IsFuture].WasModified )
                {
                    _CswNbtNode.Properties[PropertyName.IsFuture].AsLogical.Checked = CswEnumTristate.False;
                }

            }//If one of the main properties is modified        

        }//beforeWriteNode()

        public override void afterPropertySetWriteNode()
        {
        }//afterWriteNode()

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes )
        {
        }

        public override void afterPropertySetPopulateProps()
        {
            if( Owner.RelatedNodeId != null )
            {
                CswNbtNode EquipmentOrAssemblyNode = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( EquipmentOrAssemblyNode != null )
                {
                    CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                    if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.EquipmentClass )
                    {
                        CswNbtObjClassEquipment EquipmentNodeAsEquipment = (CswNbtObjClassEquipment) EquipmentOrAssemblyNode;
                        foreach( string YValue in EquipmentNodeAsEquipment.Parts.YValues )
                        {
                            if( EquipmentNodeAsEquipment.Parts.CheckValue( CswNbtObjClassEquipment.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                    }
                    else if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                    {
                        CswNbtObjClassEquipmentAssembly AssemblyNodeAsAssembly = (CswNbtObjClassEquipmentAssembly) EquipmentOrAssemblyNode;
                        foreach( string YValue in AssemblyNodeAsAssembly.AssemblyParts.YValues )
                        {
                            if( AssemblyNodeAsAssembly.AssemblyParts.CheckValue( CswNbtObjClassEquipmentAssembly.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                    }
                    this.Parts.YValues = NewYValues;
                } // if( EquipmentOrAssemblyNode != null )
            } // if( Owner.RelatedNodeId != null )
        }//afterPopulateProps()

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropDateTime DoneOn { get { return ( _CswNbtNode.Properties[PropertyName.DoneOn] ); } }
        public CswNbtNodePropLogical Completed { get { return ( _CswNbtNode.Properties[PropertyName.Completed] ); } }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        public CswNbtNodePropText Summary { get { return ( _CswNbtNode.Properties[PropertyName.Summary] ); } }
        public CswNbtNodePropLogicalSet Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }
        public CswNbtNodePropDateTime CalibrationDate { get { return ( _CswNbtNode.Properties[PropertyName.CalibrationDate] ); } }
        public CswNbtNodePropNumber CalibrationResult { get { return ( _CswNbtNode.Properties[PropertyName.CalibrationResult] ); } }
        public CswNbtNodePropMemo CompletionDescription { get { return ( _CswNbtNode.Properties[PropertyName.CompletionDescription] ); } }
        public CswNbtNodePropRelationship Department { get { return ( _CswNbtNode.Properties[PropertyName.Department] ); } }
        public CswNbtNodePropList EventType { get { return ( _CswNbtNode.Properties[PropertyName.EventType] ); } }
        public CswNbtNodePropLink FileLink { get { return ( _CswNbtNode.Properties[PropertyName.FileLink] ); } }
        public CswNbtNodePropLogical IgnoreCalibrationResult { get { return ( _CswNbtNode.Properties[PropertyName.IgnoreCalibrationResult] ); } }
        public CswNbtNodePropLogical IsCriticalTest { get { return ( _CswNbtNode.Properties[PropertyName.IsCriticalTest] ); } }
        public CswNbtNodePropText LaborCost { get { return ( _CswNbtNode.Properties[PropertyName.LaborCost] ); } }
        public CswNbtNodePropNumber LowerLimit { get { return ( _CswNbtNode.Properties[PropertyName.LowerLimit] ); } }
        public CswNbtNodePropText OtherCost { get { return ( _CswNbtNode.Properties[PropertyName.OtherCost] ); } }
        public CswNbtNodePropText OtherCostName { get { return ( _CswNbtNode.Properties[PropertyName.OtherCostName] ); } }
        public CswNbtNodePropText PartsCost { get { return ( _CswNbtNode.Properties[PropertyName.PartsCost] ); } }
        public CswNbtNodePropText SOPRef { get { return ( _CswNbtNode.Properties[PropertyName.SOPRef] ); } }
        public CswNbtNodePropRelationship Technician { get { return ( _CswNbtNode.Properties[PropertyName.Technician] ); } }
        public CswNbtNodePropPropertyReference TechnicianPhone { get { return ( _CswNbtNode.Properties[PropertyName.TechnicianPhone] ); } }
        public CswNbtNodePropText TravelCost { get { return ( _CswNbtNode.Properties[PropertyName.TravelCost] ); } }
        public CswNbtNodePropNumber UpperLimit { get { return ( _CswNbtNode.Properties[PropertyName.UpperLimit] ); } }

        public override CswNbtNodePropRelationship Parent { get { return ( _CswNbtNode.Properties[ParentPropertyName] ); } }
        
        #endregion


    }//CswNbtObjClassTask

}//namespace ChemSW.Nbt.ObjClasses
