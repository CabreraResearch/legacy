using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using System;
using System.Data;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFeedback : CswNbtObjClass
    {
        //public static string ChildFeedbackTypePropertyName { get { return "Child Feedback Type"; } }
        public const string AuthorPropertyName = "Author";
        public const string DateSubmittedPropertyName = "Date Submitted";
        public const string SubjectPropertyName = "Subject";
        public const string SummaryPropertyName = "Summary";
        public const string DiscussionPropertyName = "Discussion";
        public const string LoadUserContextPropertyName = "Load User Context";
        public const string SelectedNodeIDPropertyName = "Selected Node ID";
        public const string ActionPropertyName = "Action";
        public const string ViewPropertyName = "View";
        public const string StatusPropertyName = "Status";
        public const string CategoryPropertyName = "Category";
        public const string CaseNumberPropertyName = "Case Number";
        public const string CurrentViewModePropertyName = "Current View Mode";
        public const string LastCommentPropertyName = "Last Comment";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassFeedback( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFeedback
        /// </summary>
        public static implicit operator CswNbtObjClassFeedback( CswNbtNode Node )
        {
            CswNbtObjClassFeedback ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass ) )
            {
                ret = (CswNbtObjClassFeedback) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            //Grab the name of the person submitting feedback and the current time
            this.Author.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            this.DateSubmitted.DateTimeValue = System.DateTime.Now;

            if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentviewid" ) && false == String.IsNullOrEmpty( _CswNbtResources.CurrentNbtUser.Cookies["csw_currentviewid"] ) )
            {
                CswNbtViewId CurrentViewId = new CswNbtViewId( _CswNbtResources.CurrentNbtUser.Cookies["csw_currentviewid"] );
                View.SelectedViewIds = new Core.CswCommaDelimitedString() { CurrentViewId.get().ToString() };
            }

            if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentactionname" ) )
            {
                Action.Text = _CswNbtResources.CurrentNbtUser.Cookies["csw_currentactionname"];
            }

            if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentnodeid" ) )
            {
                SelectedNodeId.Text = _CswNbtResources.CurrentNbtUser.Cookies["csw_currentnodeid"];
            }

            if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentviewmode" ) )
            {
                CurrentViewMode.Text = _CswNbtResources.CurrentNbtUser.Cookies["csw_currentviewmode"];
            }

            LastComment.Text = ""; //give LastComment an empty string so we don't get an ORNY

            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            if( Discussion.WasModified )
            {
                LastComment.Text = Discussion.Last["message"].ToString();
            }
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
            //might cause a problem here, case 26683
            if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                LoadUserContext.Hidden = true;
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Summary.Text = Discussion.CommentsJson[0]["message"].ToString();
            this.postChanges( false );//---------------------------------------------------------------------------------------------------------for testing lsat comment
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp )
            {
                CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
                if( LoadUserContextPropertyName == OCP.PropName )
                {
                    JObject ActionDataObj = new JObject();
                    ActionDataObj["action"] = OCP.PropName;

                    ActionDataObj["type"] = "view"; //assume it's a view unless it's an action
                    CswNbtActionName ActionName = CswNbtActionName.Unknown;
                    Enum.TryParse<CswNbtActionName>( Action.Text, out ActionName );
                    if( CswNbtActionName.Unknown != ActionName )
                    {
                        if( null != _CswNbtResources.Actions[ActionName] )
                        {
                            CswNbtAction action = _CswNbtResources.Actions[ActionName];
                            ActionDataObj["type"] = "action";
                            ActionDataObj["actionname"] = action.Name.ToString();
                            ActionDataObj["actionid"] = action.ActionId.ToString();
                            ActionDataObj["actionurl"] = action.Url.ToString();
                        }
                    }

                    ActionDataObj["selectedNodeId"] = SelectedNodeId.Text;
                    if( null != CurrentViewMode )
                    {
                        ActionDataObj["viewmode"] = CurrentViewMode.Text;
                    }
                    CswNbtViewId delimitedViewId = new CswNbtViewId( CswConvert.ToInt32( View.SelectedViewIds.ToString() ) );
                    if( null != delimitedViewId )
                    {
                        ActionDataObj["viewid"] = delimitedViewId.ToString();
                    }
                    if( null != Author.RelatedNodeId )
                    {
                        if( _CswNbtResources.CurrentNbtUser.UserId != Author.RelatedNodeId )
                        {
                            ActionDataObj["userid"] = Author.RelatedNodeId.ToString();
                            CswNbtObjClassUser userNode = _CswNbtResources.Nodes[Author.RelatedNodeId];
                            if( null != userNode )
                            {
                                ActionDataObj["username"] = userNode.Username;
                            }
                        }
                    }
                    ActionData = ActionDataObj.ToString();
                    ButtonAction = NbtButtonAction.loadView;
                }
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Author
        {
            get
            {
                return ( _CswNbtNode.Properties[AuthorPropertyName] );
            }
        }

        public CswNbtNodePropDateTime DateSubmitted
        {
            get
            {
                return ( _CswNbtNode.Properties[DateSubmittedPropertyName] );
            }
        }

        public CswNbtNodePropText Subject
        {
            get
            {
                return ( _CswNbtNode.Properties[SubjectPropertyName] );
            }
        }

        public CswNbtNodePropMemo Summary
        {
            get
            {
                return ( _CswNbtNode.Properties[SummaryPropertyName] );
            }
        }

        public CswNbtNodePropComments Discussion
        {
            get
            {
                return ( _CswNbtNode.Properties[DiscussionPropertyName] );
            }
        }

        public CswNbtNodePropButton LoadUserContext
        {
            get
            {
                return ( _CswNbtNode.Properties[LoadUserContextPropertyName] );
            }
        }

        public CswNbtNodePropText SelectedNodeId
        {
            get
            {
                return ( _CswNbtNode.Properties[SelectedNodeIDPropertyName] );
            }
        }

        public CswNbtNodePropText Action
        {
            get
            {
                return ( _CswNbtNode.Properties[ActionPropertyName] );
            }
        }

        public CswNbtNodePropViewPickList View
        {
            get
            {
                return ( _CswNbtNode.Properties[ViewPropertyName] );
            }
        }

        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName] );
            }
        }

        public CswNbtNodePropList Category
        {
            get
            {
                return ( _CswNbtNode.Properties[CategoryPropertyName] );
            }
        }

        public CswNbtNodePropSequence CaseNumber
        {
            get
            {
                return ( _CswNbtNode.Properties[CaseNumberPropertyName] );
            }
        }

        public CswNbtNodePropText CurrentViewMode
        {
            get
            {
                return ( _CswNbtNode.Properties[CurrentViewModePropertyName] );
            }
        }

        public CswNbtNodePropText LastComment
        {
            get
            {
                return ( _CswNbtNode.Properties[LastCommentPropertyName] );
            }
        }

        #endregion

    }//CswNbtObjClassFeedback

}//namespace ChemSW.Nbt.ObjClasses
