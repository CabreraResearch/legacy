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
        public sealed class PropertyName
        {
            public const string ReportView = "Report View";
            public const string Report = "Report";
            //public static string StatusPropertyName { get { return "Status"; } }
            public const string Message = "Message";
            public const string NoDataNotification = "No Data Notification";
            public const string Recipients = "Recipients";
            public const string Type = "Type";
            public const string LastProcessed = "Last Processed";
            public const string FinalDueDate = "Final Due Date";
            public const string NextDueDate = "Next Due Date";
            public const string RunStatus = "Run Status";
            public const string WarningDays = "Warning Days";
            public const string DueDateInterval = "Due Date Interval";
            public const string RunTime = "Run Time";
            public const string Enabled = "Enabled";
            public const string RunNow = "Run Now";
            public const string OutputFormat = "Output Format";
        }



        public const string TypeOptionReport = "Report";
        public const string TypeOptionView = "View";

        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return PropertyName.FinalDueDate; } }
        public string SchedulerNextDueDatePropertyName { get { return PropertyName.NextDueDate; } }
        public string SchedulerRunStatusPropertyName { get { return PropertyName.RunStatus; } }
        public string SchedulerWarningDaysPropertyName { get { return PropertyName.WarningDays; } }
        public string SchedulerDueDateIntervalPropertyName { get { return PropertyName.DueDateInterval; } }
        public string SchedulerRunTimePropertyName { get { return PropertyName.RunTime; } }
        public string SchedulerRunNowPropertyName { get { return PropertyName.RunNow; } }

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
                if( PropertyName.RunNow == OCP.PropName )
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

        public CswNbtNodePropViewPickList ReportView { get { return ( _CswNbtNode.Properties[PropertyName.ReportView] ); } }
        public CswNbtNodePropRelationship Report { get { return ( _CswNbtNode.Properties[PropertyName.Report] ); } }
        public CswNbtNodePropMemo Message { get { return ( _CswNbtNode.Properties[PropertyName.Message] ); } }
        public CswNbtNodePropMemo NoDataNotification { get { return ( _CswNbtNode.Properties[PropertyName.NoDataNotification] ); } }
        public CswNbtNodePropUserSelect Recipients { get { return ( _CswNbtNode.Properties[PropertyName.Recipients] ); } }
        public CswNbtNodePropList Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropDateTime LastProcessed { get { return ( _CswNbtNode.Properties[PropertyName.LastProcessed] ); } }
        public CswNbtNodePropDateTime FinalDueDate { get { return ( _CswNbtNode.Properties[PropertyName.FinalDueDate] ); } }
        public CswNbtNodePropDateTime NextDueDate { get { return ( _CswNbtNode.Properties[PropertyName.NextDueDate] ); } }
        public CswNbtNodePropButton RunNow { get { return ( _CswNbtNode.Properties[PropertyName.RunNow] ); } }
        public CswNbtNodePropList OutputFormat { get { return ( _CswNbtNode.Properties[PropertyName.OutputFormat] ); } }
        public CswNbtNodePropComments RunStatus { get { return ( _CswNbtNode.Properties[PropertyName.RunStatus] ); } }
        public CswNbtNodePropNumber WarningDays { get { return ( _CswNbtNode.Properties[PropertyName.WarningDays] ); } }
        public CswNbtNodePropTimeInterval DueDateInterval { get { return ( _CswNbtNode.Properties[PropertyName.DueDateInterval] ); } }
        public CswNbtNodePropDateTime RunTime { get { return ( _CswNbtNode.Properties[PropertyName.RunTime] ); } }
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }

        #endregion

    }//CswNbtObjClassMailReport

}//namespace ChemSW.Nbt.ObjClasses
