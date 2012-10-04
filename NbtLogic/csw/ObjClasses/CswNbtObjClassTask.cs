using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassTask : CswNbtObjClass, ICswNbtPropertySetGeneratorTarget
    {
        public sealed class PropertyName
        {
            public const string DueDate = "Due Date";
            public const string Completed = "Completed";
            public const string Description = "Description";
            public const string Owner = "Owner";
            public const string Summary = "Summary";
            public const string DoneOn = "Done On";
            public const string IsFuture = "IsFuture";
            public const string Generator = "Generator";
            public const string Parts = "Parts";
            public const string PartsXValue = "Service";
        }


        //ICswNbtPropertySetRuleGeneratorTarget
        public string GeneratorTargetGeneratedDatePropertyName { get { return PropertyName.DueDate; } }
        public string GeneratorTargetIsFuturePropertyName { get { return PropertyName.IsFuture; } }
        public string GeneratorTargetGeneratorPropertyName { get { return PropertyName.Generator; } }
        public string GeneratorTargetParentPropertyName { get { return PropertyName.Owner; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassTask( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.TaskClass ); }
        }

        //BZ 10247
        private void setDoneOnDate()
        {
            if( Completed.Checked == Tristate.True && DoneOn.DateTimeValue == DateTime.MinValue )
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
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClassName.NbtObjectClass.TaskClass ) )
            {
                ret = (CswNbtObjClassTask) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            setDoneOnDate();
            // Set the IsFuture flag = false if the node is modified
            if( !_CswNbtNode.New && ( NodeModificationState.Modified == _CswNbtNode.ModificationState ) )
            {
                //if someone set the flag deliberately don't mess with it
                if( !_CswNbtNode.Properties[PropertyName.IsFuture].WasModified )
                {
                    _CswNbtNode.Properties[PropertyName.IsFuture].AsLogical.Checked = Tristate.False;
                }

            }//If one of the main properties is modified        

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
            if( Owner.RelatedNodeId != null )
            {
                CswNbtNode EquipmentOrAssemblyNode = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( EquipmentOrAssemblyNode != null )
                {
                    CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                    if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentClass )
                    {
                        CswNbtObjClassEquipment EquipmentNodeAsEquipment = (CswNbtObjClassEquipment) EquipmentOrAssemblyNode;
                        foreach( string YValue in EquipmentNodeAsEquipment.Parts.YValues )
                        {
                            if( EquipmentNodeAsEquipment.Parts.CheckValue( CswNbtObjClassEquipment.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                    }
                    else if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentAssemblyClass )
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

        public CswNbtNodePropDateTime GeneratedDate { get { return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName] ); } }
        public CswNbtNodePropDateTime DoneOn { get { return ( _CswNbtNode.Properties[PropertyName.DoneOn] ); } }
        public CswNbtNodePropDateTime DueDate { get { return ( _CswNbtNode.Properties[PropertyName.DueDate] ); } }
        public CswNbtNodePropLogical IsFuture { get { return ( _CswNbtNode.Properties[PropertyName.IsFuture] ); } }
        public CswNbtNodePropLogical Completed { get { return ( _CswNbtNode.Properties[PropertyName.Completed] ); } }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropRelationship Generator { get { return ( _CswNbtNode.Properties[PropertyName.Generator] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        public CswNbtNodePropText Summary { get { return ( _CswNbtNode.Properties[PropertyName.Summary] ); } }
        public CswNbtNodePropRelationship Parent { get { return ( _CswNbtNode.Properties[GeneratorTargetParentPropertyName] ); } }
        public CswNbtNodePropLogicalSet Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }

        #endregion


    }//CswNbtObjClassTask

}//namespace ChemSW.Nbt.ObjClasses
