using System;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassBatchOp : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public static string BatchDataPropertyName { get { return "Batch Data"; } }
        public static string EndDatePropertyName { get { return "End Date"; } }
        public static string LogPropertyName { get { return "Log"; } }
        public static string OpNamePropertyName { get { return "Operation Name"; } }
        public static string PriorityPropertyName { get { return "Priority"; } }
        public static string StartDatePropertyName { get { return "Start Date"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string UserPropertyName { get { return "User"; } }


        public CswNbtObjClassBatchOp( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassBatchOp
        /// </summary>
        public static explicit operator CswNbtObjClassBatchOp( CswNbtNode Node )
        {
            CswNbtObjClassBatchOp ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass ) )
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
            if( NbtBatchOpStatus.Pending.ToString() == Status.Value )
            {
                appendToLog( "Operation started." );
                StartDate.DateTimeValue = DateTime.Now;
                Status.Value = NbtBatchOpStatus.Processing.ToString();
                postChanges( false );
            }
        }

        /// <summary>
        /// For use by CswNbtBatchOps: mark an operation finished
        /// </summary>
        public void finish()
        {
            appendToLog( "Operation Complete." );
            EndDate.DateTimeValue = DateTime.Now;
            Status.Value = NbtBatchOpStatus.Completed.ToString();
            postChanges( false );
        }

        /// <summary>
        /// For use by CswNbtBatchOps: mark an operation errored
        /// </summary>
        public void error( Exception ex )
        {
            appendToLog( "Error: " + ex.Message + "; " + ex.StackTrace );
            Status.Value = NbtBatchOpStatus.Error.ToString();
            postChanges( false );
        }

        #endregion Object Class Specific Behaviors




        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
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

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo BatchData
        {
            get
            {
                return ( _CswNbtNode.Properties[BatchDataPropertyName].AsMemo );
            }
        }
        public CswNbtNodePropDateTime EndDate
        {
            get
            {
                return ( _CswNbtNode.Properties[EndDatePropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropDateTime StartDate
        {
            get
            {
                return ( _CswNbtNode.Properties[StartDatePropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropComments Log
        {
            get
            {
                return ( _CswNbtNode.Properties[LogPropertyName].AsComments );
            }
        }
        public CswNbtNodePropList OpName
        {
            get
            {
                return ( _CswNbtNode.Properties[OpNamePropertyName].AsList );
            }
        }
        public CswNbtNodePropNumber Priority
        {
            get
            {
                return ( _CswNbtNode.Properties[PriorityPropertyName].AsNumber );
            }
        }
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }
        }
        public CswNbtNodePropRelationship User
        {
            get
            {
                return ( _CswNbtNode.Properties[UserPropertyName].AsRelationship );
            }
        }

        #endregion

    }//CswNbtObjClassBatchOp

}//namespace ChemSW.Nbt.ObjClasses
