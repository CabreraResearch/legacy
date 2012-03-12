using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMailReport : CswNbtObjClass, ICswNbtPropertySetScheduler
    {
        public static string ReportViewPropertyName { get { return "Report View"; } }
        public static string ReportPropertyName { get { return "Report"; } }
        //public static string StatusPropertyName { get { return "Status"; } }
        public static string MessagePropertyName { get { return "Message"; } }
        public static string NoDataNotificationPropertyName { get { return "No Data Notification"; } }
        public static string RecipientsPropertyName { get { return "Recipients"; } }
        public static string TypePropertyName { get { return "Type"; } }
        public static string LastProcessedPropertyName { get { return "Last Processed"; } }
        public static string FinalDueDatePropertyName { get { return "Final Due Date"; } }
        public static string NextDueDatePropertyName { get { return "Next Due Date"; } }
        public static string RunStatusPropertyName { get { return "Run Status"; } }
        public static string WarningDaysPropertyName { get { return "Warning Days"; } }
        public static string DueDateIntervalPropertyName { get { return "Due Date Interval"; } }
        public static string RunTimePropertyName { get { return "Run Time"; } }
        public static string EnabledPropertyName { get { return "Enabled"; } }
        public static string RunNowPropertyName { get { return "Run Now"; } }

        public static string TypeOptionReport = "Report";
        public static string TypeOptionView = "View";

        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return FinalDueDatePropertyName; } }
        public string SchedulerNextDueDatePropertyName { get { return NextDueDatePropertyName; } }
        public string SchedulerRunStatusPropertyName { get { return RunStatusPropertyName; } }
        public string SchedulerWarningDaysPropertyName { get { return WarningDaysPropertyName; } }
        public string SchedulerDueDateIntervalPropertyName { get { return DueDateIntervalPropertyName; } }
        public string SchedulerRunTimePropertyName { get { return RunTimePropertyName; } }
        public string SchedulerRunNowPropertyName { get { return RunNowPropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;

        public CswNbtObjClassMailReport( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }//ctor()

        public CswNbtObjClassMailReport( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate();

            _checkView();

        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
            //_CswNbtPropertySetSchedulerImpl.setLastFutureDate();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate();

            _checkView();
        }

        private void _checkView()
        {
            // BZ 7845
            if( Type.Empty )
            {
                RunStatus.StaticText = "Cannot Enable Mail Report: No Type Selected";
                Enabled.Checked = Tristate.False;
            }
            else
            {
                if( Type.Value == TypeOptionReport && Report.Empty )
                {
                    RunStatus.StaticText = "Cannot Enable Mail Report: No Report Selected";
                    Enabled.Checked = Tristate.False;
                }
                else if( Type.Value == TypeOptionView && ReportView.Empty )
                {
                    RunStatus.StaticText = "Cannot Enable Mail Report: No View Selected";
                    Enabled.Checked = Tristate.False;
                }
                else
                {
                    if( RunStatus.StaticText.StartsWith( "Cannot Enable Mail Report" ) )
                        RunStatus.StaticText = string.Empty;
                }
            }

        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
            //_CswNbtPropertySetSchedulerImpl.setLastFutureDate();
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
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                if( RunNowPropertyName == OCP.PropName )
                {
                    NextDueDate.DateTimeValue = DateTime.Now;
                    RunStatus.StaticText = string.Empty;
                    Node.postChanges( false );
                    ButtonAction = NbtButtonAction.refresh;
                }
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropViewPickList ReportView
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportViewPropertyName].AsViewPickList );
            }
        }

        public CswNbtNodePropRelationship Report
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportPropertyName].AsRelationship );
            }
        }

        //public CswNbtNodePropMemo Status
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[StatusPropertyName].AsMemo );
        //    }

        //}

        public CswNbtNodePropMemo Message
        {
            get
            {
                return ( _CswNbtNode.Properties[MessagePropertyName].AsMemo );
            }
        }

        public CswNbtNodePropMemo NoDataNotification
        {
            get
            {
                return ( _CswNbtNode.Properties[NoDataNotificationPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropUserSelect Recipients
        {
            get
            {
                return ( _CswNbtNode.Properties[RecipientsPropertyName].AsUserSelect );
            }
        }

        public CswNbtNodePropList Type
        {
            get
            {
                return ( _CswNbtNode.Properties[TypePropertyName].AsList );
            }
        }

        public CswNbtNodePropDateTime LastProcessed
        {
            get
            {
                return ( _CswNbtNode.Properties[LastProcessedPropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropDateTime FinalDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[FinalDueDatePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropDateTime NextDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[NextDueDatePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropButton RunNow
        {
            get
            {
                return ( _CswNbtNode.Properties[RunNowPropertyName].AsButton );
            }
        }

        public CswNbtNodePropStatic RunStatus
        {
            get
            {
                return ( _CswNbtNode.Properties[RunStatusPropertyName].AsStatic );
            }
        }

        public CswNbtNodePropNumber WarningDays
        {
            get
            {
                return ( _CswNbtNode.Properties[WarningDaysPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropTimeInterval DueDateInterval
        {
            get
            {
                return ( _CswNbtNode.Properties[DueDateIntervalPropertyName].AsTimeInterval );
            }
        }

        public CswNbtNodePropDateTime RunTime
        {
            get
            {
                return ( _CswNbtNode.Properties[RunTimePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropLogical Enabled
        {
            get
            {
                return ( _CswNbtNode.Properties[EnabledPropertyName].AsLogical );
            }
        }



        #endregion

    }//CswNbtObjClassMailReport

}//namespace ChemSW.Nbt.ObjClasses
