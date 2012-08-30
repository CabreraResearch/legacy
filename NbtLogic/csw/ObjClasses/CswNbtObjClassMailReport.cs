using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

enum MailRptFormatOptions { Link, CSV };

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
        public static string OutputFormatPropertyName { get { return "Output Format"; } }

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

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMailReport
        /// </summary>
        public static implicit operator CswNbtObjClassMailReport( CswNbtNode Node )
        {
            CswNbtObjClassMailReport ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass ) )
            {
                ret = (CswNbtObjClassMailReport) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate();

            _assertMailReportIsValid();

            if( Type.Value == TypeOptionView )
            {
                OutputFormat.Value = MailRptFormatOptions.Link.ToString();
                OutputFormat.setReadOnly( value: true, SaveToDb: true );
            }
            else if( Type.Value == TypeOptionReport )
            {
                OutputFormat.setReadOnly( value: false, SaveToDb: true );
            }
        }

        private void _assertMailReportIsValid()
        {
            string mailReportError = _getMailReportError();
            if( Enabled.Checked == Tristate.True && false == String.IsNullOrEmpty( mailReportError ) )
            {
                Enabled.Checked = Tristate.False;
                throw new CswDniException( ErrorType.Warning, "Cannot Enable Mail Report: No " + mailReportError + " Selected.", "No " + mailReportError + " Selected." );
            }
        }

        private string _getMailReportError()
        {
            string mailReportError = String.Empty;
            if( Type.Empty )
            {
                mailReportError = "Type";
            }
            else
            {
                if( Type.Value == TypeOptionReport && Report.Empty )
                {
                    mailReportError = "Report";
                }
                else if( Type.Value == TypeOptionView && ReportView.Empty )
                {
                    mailReportError = "View";
                }
            }
            return mailReportError;
        }

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
            //_CswNbtPropertySetSchedulerImpl.setLastFutureDate();
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
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( RunNowPropertyName == OCP.PropName )
                {
                    NextDueDate.DateTimeValue = DateTime.Now.AddDays( this.WarningDays.Value );
                    Node.postChanges( false );
                    ButtonData.Action = NbtButtonAction.refresh;
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

        public CswNbtNodePropList OutputFormat
        {
            get
            {
                return ( _CswNbtNode.Properties[OutputFormatPropertyName] );
            }
        }

        public CswNbtNodePropComments RunStatus
        {
            get
            {
                return ( _CswNbtNode.Properties[RunStatusPropertyName].AsComments );
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
