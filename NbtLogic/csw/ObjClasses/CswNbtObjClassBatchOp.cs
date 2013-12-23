﻿using System;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassBatchOp : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string BatchData = "Batch Data";
            public const string CreatedDate = "Created Date";
            public const string EndDate = "End Date";
            public const string Log = "Log";
            public const string OpName = "Operation Name";
            public const string PercentDone = "Percent Done";
            public const string Priority = "Priority";
            public const string StartDate = "Start Date";
            public const string Status = "Status";
            public const string User = "User";
        }
        
        public CswNbtObjClassBatchOp( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassBatchOp
        /// </summary>
        public static implicit operator CswNbtObjClassBatchOp( CswNbtNode Node )
        {
            CswNbtObjClassBatchOp ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.BatchOpClass ) )
            {
                ret = (CswNbtObjClassBatchOp) Node.ObjClass;
            }
            return ret;
        }

        #region Object Class Specific Behaviors

        /// <summary>
        /// For use by CswNbtBatchOps: add a log message
        /// </summary>
        public void appendToLog( string Message )
        {
            Log.AddComment( Message );
        }


        /// <summary>
        /// For use by CswNbtBatchOps: mark an operation started
        /// </summary>
        public void start()
        {
            if( CswEnumNbtBatchOpStatus.Pending.ToString() == Status.Value )
            {
                //appendToLog( "Operation started." );
                StartDate.DateTimeValue = DateTime.Now;
                Status.Value = CswEnumNbtBatchOpStatus.Processing.ToString();
                postChanges( false );
            }
        }

        /// <summary>
        /// For use by CswNbtBatchOps: mark an operation finished
        /// </summary>
        public void finish()
        {
            //appendToLog( "Operation Complete." );
            EndDate.DateTimeValue = DateTime.Now;
            Status.Value = CswEnumNbtBatchOpStatus.Completed.ToString();
            postChanges( false );
        }

        /// <summary>
        /// For use by CswNbtBatchOps: mark an operation errored
        /// </summary>
        public void error( Exception ex )
        {
            string Message = "Error: " + ex.Message + "; ";
            if(  _CswNbtResources.ShowFullStackTraceInUI )
            {
                Message += ex.StackTrace;
            } 
            appendToLog( Message );
            Status.Value = CswEnumNbtBatchOpStatus.Error.ToString();
            postChanges( false );
        }

        #endregion Object Class Specific Behaviors




        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforePromoteNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );

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
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo BatchData
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.BatchData] );
            }
        }
        public CswNbtNodePropDateTime CreatedDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CreatedDate] );
            }
        }
        public CswNbtNodePropDateTime EndDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.EndDate] );
            }
        }
        public CswNbtNodePropDateTime StartDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.StartDate] );
            }
        }
        public CswNbtNodePropComments Log
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Log] );
            }
        }
        public CswNbtNodePropText OpName
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.OpName] );
            }
        }
        public CswEnumNbtBatchOpName OpNameValue
        {
            get
            {
                return (CswEnumNbtBatchOpName) OpName.Text;
            }
        }
        public CswNbtNodePropNumber PercentDone
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.PercentDone] );
            }
        }
        public CswNbtNodePropNumber Priority
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Priority] );
            }
        }
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Status] );
            }
        }
        public CswNbtNodePropRelationship User
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.User] );
            }
        }

        #endregion

    }//CswNbtObjClassBatchOp

}//namespace ChemSW.Nbt.ObjClasses
