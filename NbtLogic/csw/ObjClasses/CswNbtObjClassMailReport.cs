using System;
using System.Collections.Generic;
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
            public const string DueDateInterval = "Due Date Interval";
            public const string Enabled = "Enabled";
            public const string Event = "Event";
            public const string FinalDueDate = "Final Due Date";
            public const string LastProcessed = "Last Processed";
            public const string Message = "Message";
            public const string NextDueDate = "Next Due Date";
            public const string NodesToReport = "Nodes To Report";
            public const string OutputFormat = "Output Format";
            public const string Report = "Report";
            public const string ReportView = "Report View";
            public const string Recipients = "Recipients";
            public const string RunNow = "Run Now";
            public const string RunStatus = "Run Status";
            public const string RunTime = "Run Time";
            public const string TargetType = "Target Type";
            public const string Type = "Type";
            public const string WarningDays = "Warning Days";
        }

        /// <summary>
        /// Event Options
        /// </summary>
        public sealed class EventOption : CswEnum<EventOption>
        {
            private EventOption( string Name ) : base( Name ) { }
            public static IEnumerable<EventOption> _All { get { return All; } }
            public static implicit operator EventOption( string str )
            {
                EventOption ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly EventOption Unknown = new EventOption( "Unknown" );

            public static readonly EventOption Exists = new EventOption( "Exists" );
            //public static readonly EventOption Create = new EventOption( "Create" );
            public static readonly EventOption Edit = new EventOption( "Edit" );
            //public static readonly EventOption Delete = new EventOption( "Delete" );
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

            // Set default value for target type to root of view
            if( TargetType.Empty && Type.Value == TypeOptionView )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ReportView.ViewId );
                if( null != View && 
                    View.Root.ChildRelationships.Count > 0 &&
                    View.Root.ChildRelationships[0].SecondType == NbtViewRelatedIdType.NodeTypeId )
                {
                    TargetType.SelectedNodeTypeIds.Add( View.Root.ChildRelationships[0].SecondId.ToString() );
                }
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
            Type.SetOnPropChange( OnTypePropChange );

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

        public CswNbtNodePropTimeInterval DueDateInterval { get { return ( _CswNbtNode.Properties[PropertyName.DueDateInterval] ); } }
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }
        public CswNbtNodePropList Event { get { return ( _CswNbtNode.Properties[PropertyName.Event] ); } }
        public CswNbtNodePropDateTime FinalDueDate { get { return ( _CswNbtNode.Properties[PropertyName.FinalDueDate] ); } }
        public CswNbtNodePropDateTime LastProcessed { get { return ( _CswNbtNode.Properties[PropertyName.LastProcessed] ); } }
        public CswNbtNodePropMemo Message { get { return ( _CswNbtNode.Properties[PropertyName.Message] ); } }
        public CswNbtNodePropDateTime NextDueDate { get { return ( _CswNbtNode.Properties[PropertyName.NextDueDate] ); } }
        public CswNbtNodePropMemo NodesToReport { get { return ( _CswNbtNode.Properties[PropertyName.NodesToReport] ); } }
        public CswNbtNodePropList OutputFormat { get { return ( _CswNbtNode.Properties[PropertyName.OutputFormat] ); } }
        public CswNbtNodePropUserSelect Recipients { get { return ( _CswNbtNode.Properties[PropertyName.Recipients] ); } }
        public CswNbtNodePropRelationship Report { get { return ( _CswNbtNode.Properties[PropertyName.Report] ); } }
        public CswNbtNodePropViewReference ReportView { get { return ( _CswNbtNode.Properties[PropertyName.ReportView] ); } }
        public CswNbtNodePropButton RunNow { get { return ( _CswNbtNode.Properties[PropertyName.RunNow] ); } }
        public CswNbtNodePropComments RunStatus { get { return ( _CswNbtNode.Properties[PropertyName.RunStatus] ); } }
        public CswNbtNodePropDateTime RunTime { get { return ( _CswNbtNode.Properties[PropertyName.RunTime] ); } }
        public CswNbtNodePropNodeTypeSelect TargetType { get { return ( _CswNbtNode.Properties[PropertyName.TargetType] ); } }
        public CswNbtNodePropList Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public void OnTypePropChange( CswNbtNodeProp Prop )
        {
            if( Type.Value == TypeOptionView )
            {
                OutputFormat.Value = MailRptFormatOptions.Link.ToString();
            }
        } // OnTypePropChange()
        public CswNbtNodePropNumber WarningDays { get { return ( _CswNbtNode.Properties[PropertyName.WarningDays] ); } }

        #endregion

        public CswCommaDelimitedString GetNodesToReport()
        {
            CswCommaDelimitedString NodesStr = new CswCommaDelimitedString();
            NodesStr.FromString( NodesToReport.Text );
            return NodesStr;
        } // GetNodesToReport()

        public void AddNodeToReport( CswNbtNode Node )
        {
            CswCommaDelimitedString NodesStr = new CswCommaDelimitedString();
            NodesStr.FromString( NodesToReport.Text );

            NodesStr.Add( Node.NodeId.PrimaryKey.ToString(), AllowNullOrEmpty: false, IsUnique: true );

            NodesToReport.Text = NodesStr.ToString();
        } // AddNodeToReport()

        public void ClearNodesToReport()
        {
            NodesToReport.Text = string.Empty;
        } // ClearNodesToReport()

    }//CswNbtObjClassMailReport

}//namespace ChemSW.Nbt.ObjClasses
