using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFeedback : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Author = "Author";
            public const string DateSubmitted = "Date Submitted";
            public const string Subject = "Subject";
            public const string Summary = "Summary";
            public const string Discussion = "Discussion";
            public const string LoadUserContext = "Load User Context";
            public const string SelectedNodeID = "Selected Node ID";
            public const string Action = "Action";
            public const string View = "View";
            public const string Status = "Status";
            public const string Category = "Category";
            public const string CaseNumber = "Case Number";
            public const string CurrentViewMode = "Current View Mode";
        }

        public sealed class Statuses
        {
            public const string PendingReview = "Pending review";
            public const string Resolved = "Resolved";
            public const string AwaitingAuthorResponse = "Awaiting author response";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    PendingReview,
                    Resolved,
                    AwaitingAuthorResponse
                };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassFeedback( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FeedbackClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFeedback
        /// </summary>
        public static implicit operator CswNbtObjClassFeedback( CswNbtNode Node )
        {
            CswNbtObjClassFeedback ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.FeedbackClass ) )
            {
                ret = (CswNbtObjClassFeedback) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            Author.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            DateSubmitted.DateTimeValue = DateTime.Now;

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
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
            if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                LoadUserContext.setHidden( value: true, SaveToDb: false );
            }
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
                if( PropertyName.LoadUserContext == OCP.PropName )
                {
                    ButtonData.Data["action"] = OCP.PropName;

                    ButtonData.Data["type"] = "view"; //assume it's a view unless it's an action
                    CswNbtActionName ActionName = CswNbtAction.ActionNameStringToEnum( Action.Text );
                    if( CswNbtActionName.Unknown != ActionName )
                    {
                        if( null != _CswNbtResources.Actions[ActionName] )
                        {
                            CswNbtAction action = _CswNbtResources.Actions[ActionName];
                            ButtonData.Data["type"] = "action";
                            ButtonData.Data["actionname"] = action.Name.ToString();
                            ButtonData.Data["actionid"] = action.ActionId.ToString();
                            ButtonData.Data["actionurl"] = action.Url.ToString();
                        }
                    }
                    else
                    {
                        ButtonData.Data["selectedNodeId"] = SelectedNodeId.Text;
                        if( null != CurrentViewMode )
                        {
                            ButtonData.Data["viewmode"] = CurrentViewMode.Text;
                        }
                        //CswNbtViewId delimitedViewId = new CswNbtViewId( CswConvert.ToInt32( View.SelectedViewIds.ToString() ) );
                        CswNbtViewId delimitedViewId = View.ViewId;
                        if( null != delimitedViewId )
                        {
                            ButtonData.Data["viewid"] = delimitedViewId.ToString();
                        }
                        if( null != Author.RelatedNodeId )
                        {
                            if( _CswNbtResources.CurrentNbtUser.UserId != Author.RelatedNodeId )
                            {
                                ButtonData.Data["userid"] = Author.RelatedNodeId.ToString();
                                CswNbtObjClassUser userNode = _CswNbtResources.Nodes[Author.RelatedNodeId];
                                if( null != userNode )
                                {
                                    ButtonData.Data["username"] = userNode.Username;
                                }
                            }
                        }
                    }
                    ButtonData.Action = NbtButtonAction.loadView;
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
                return ( _CswNbtNode.Properties[PropertyName.Author] );
            }
        }

        public CswNbtNodePropDateTime DateSubmitted
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.DateSubmitted] );
            }
        }

        public CswNbtNodePropText Subject
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Subject] );
            }
        }

        public CswNbtNodePropMemo Summary
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Summary] );
            }
        }

        public CswNbtNodePropComments Discussion
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Discussion] );
            }
        }

        public CswNbtNodePropButton LoadUserContext
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.LoadUserContext] );
            }
        }

        public CswNbtNodePropText SelectedNodeId
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SelectedNodeID] );
            }
        }

        public CswNbtNodePropText Action
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Action] );
            }
        }

        public CswNbtNodePropViewReference View //formerly CswNbtNodePropViewPickList
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.View] );
            }
        }

        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Status] );
            }
        }

        public CswNbtNodePropList Category
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Category] );
            }
        }

        public CswNbtNodePropSequence CaseNumber
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CaseNumber] );
            }
        }

        public CswNbtNodePropText CurrentViewMode
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CurrentViewMode] );
            }
        }

        #endregion

        private CswNbtView _getView( string ViewId )
        {
            CswNbtView View = null;
            if( CswNbtViewId.isViewIdString( ViewId ) )
            {
                CswNbtViewId realViewid = new CswNbtViewId( ViewId );
                View = _CswNbtResources.ViewSelect.restoreView( realViewid );
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
            {
                CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
                View = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
            }
            return View;
        } // _getView()

    }//CswNbtObjClassFeedback

}//namespace ChemSW.Nbt.ObjClasses
