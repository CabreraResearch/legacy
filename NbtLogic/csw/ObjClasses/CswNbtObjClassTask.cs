using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassTask : CswNbtObjClass, ICswNbtPropertySetGeneratorTarget
    {
        public static string DueDatePropertyName { get { return "Due Date"; } }
        public static string CompletedPropertyName { get { return "Completed"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string SummaryPropertyName { get { return "Summary"; } }
        public static string DoneOnPropertyName { get { return "Done On"; } }
        public static string IsFuturePropertyName { get { return "IsFuture"; } }
        public static string GeneratorPropertyName { get { return "Generator"; } }

        //ICswNbtPropertySetRuleGeneratorTarget
        public string GeneratorTargetGeneratedDatePropertyName { get { return DueDatePropertyName; } }
        public string GeneratorTargetIsFuturePropertyName { get { return IsFuturePropertyName; } }
        public string GeneratorTargetGeneratorPropertyName { get { return GeneratorPropertyName; } }
        public string GeneratorTargetParentPropertyName { get { return OwnerPropertyName; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassTask( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassTask( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass ); }
        }

        //BZ 10247
        private void setDoneOnDate()
        {
			if( Completed.Checked == Tristate.True && DoneOn.DateTimeValue == DateTime.MinValue )
				DoneOn.DateTimeValue = DateTime.Today;

			if( Completed.Checked == Tristate.False && DoneOn.DateTimeValue != DateTime.MinValue )
				DoneOn.DateTimeValue = DateTime.MinValue;
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
            setDoneOnDate();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
            setDoneOnDate();
            // Set the IsFuture flag = false if the node is modified
            if( !_CswNbtNode.New && ( NodeModificationState.Modified == _CswNbtNode.ModificationState ) )
            {
                //if someone set the flag deliberately don't mess with it
                if( !_CswNbtNode.Properties[CswNbtObjClassTask.IsFuturePropertyName].WasModified )
                {
                    _CswNbtNode.Properties[CswNbtObjClassTask.IsFuturePropertyName].AsLogical.Checked = Tristate.False;
                }

            }//If one of the main properties is modified        

        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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

        #endregion

        #region Object class specific properties

        public CswNbtNodePropDateTime GeneratedDate
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName].AsDateTime );
            }
        }
		public CswNbtNodePropDateTime DoneOn
        {
            get
            {
                return ( _CswNbtNode.Properties[DoneOnPropertyName].AsDateTime );
            }
        }
		public CswNbtNodePropDateTime DueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[DueDatePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropLogical IsFuture
        {
            get
            {
                return ( _CswNbtNode.Properties[IsFuturePropertyName].AsLogical );
            }
        }

        public CswNbtNodePropLogical Completed
        {
            get
            {
                return ( _CswNbtNode.Properties[CompletedPropertyName].AsLogical );
            }
        }

        public CswNbtNodePropMemo Description
        {
            get
            {
                return ( _CswNbtNode.Properties[DescriptionPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropRelationship Generator
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropRelationship Owner
        {
            get
            {
                return ( _CswNbtNode.Properties[OwnerPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropText Summary
        {
            get
            {
                return ( _CswNbtNode.Properties[SummaryPropertyName].AsText );
            }
        }
        
        public CswNbtNodePropRelationship Parent
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetParentPropertyName].AsRelationship );
            }
        }
        #endregion



    }//CswNbtObjClassTask

}//namespace ChemSW.Nbt.ObjClasses
