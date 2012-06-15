using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.ServiceDrivers;

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
            //Grab the name of the person submitting feedback and the current time
            this.Author.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            this.DateSubmitted.DateTimeValue = System.DateTime.Now;

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
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp )
            {
                //CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
                //if( LoadUserContextPropertyName == OCP.PropName )
                //{
                //    //ButtonAction = NbtButtonAction.
                //    CswNbtSdTabsAndProps propsSD = new CswNbtSdTabsAndProps( _CswNbtResources );
                //    _CswNbtResources.EditMode = NodeEditMode.Add;

                //    JObject ActionDataObj = new JObject();
                //    ActionDataObj["action"] = OCP.PropName;
                //    ActionDataObj["properties"] = propsSD.getProps( Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true ); //gets the props for the add form
                //}

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

        public CswNbtNodePropComments Dicsussion
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

        public CswNbtNodePropViewReference View
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

        #endregion

    }//CswNbtObjClassFeedback

}//namespace ChemSW.Nbt.ObjClasses
