using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24523
    /// </summary>
    public class CswUpdateSchemaCase24523 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass feedbackOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass, "docs.gif", false, false );

            CswNbtMetaDataObjectClassProp caseNumberOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
             {
                 PropName = CswNbtObjClassFeedback.CaseNumberPropertyName,
                 FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence
             } );

            Int32 userOCID = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp authorOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
             {
                 PropName = CswNbtObjClassFeedback.AuthorPropertyName,
                 FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                 IsFk = true,
                 FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                 FkValue = userOCID,
                 ServerManaged = true,
                 SetValOnAdd = true
             } );

            CswNbtMetaDataObjectClassProp dateSubmittedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.DateSubmittedPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                Extended = CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString(),
                ServerManaged = true,

                SetValOnAdd = true
            } );

            CswNbtMetaDataObjectClassProp categoryOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.CategoryPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Bug Report,Feature Request,Question,Praise",
                SetValOnAdd = true
            } );

            CswNbtMetaDataObjectClassProp subjectOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
             {
                 PropName = CswNbtObjClassFeedback.SubjectPropertyName,
                 FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                 SetValOnAdd = true
             } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.SummaryPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Memo,
                SetValOnAdd = true
            } );

            CswNbtMetaDataObjectClassProp statusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.StatusPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Pending review,Resolved,Awaiting author response"
            } );
            statusOCP.DefaultValue.Field1 = "Pending review"; //indicates the ball is in supports/admins court
            statusOCP.DefaultValue.Gestalt = "Pending review";

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.LoadUserContextPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button
            } );

            CswNbtMetaDataObjectClassProp commentsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
             {
                 PropName = CswNbtObjClassFeedback.DiscussionPropertyName,
                 FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments
             } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.SelectedNodeIDPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.ViewPropertyName,
                //FieldType = CswNbtMetaDataFieldType.NbtFieldType.ViewPickList,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.ViewReference,
                ServerManaged = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.ActionPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( feedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                PropName = CswNbtObjClassFeedback.CurrentViewModePropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );

            //create a default node-type of feedbackobj called feedback
            CswNbtMetaDataNodeType feedbackNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( feedbackOC.ObjectClassId, "Feedback", "System" );
            feedbackNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFeedback.CaseNumberPropertyName ) );

            CswNbtMetaDataObjectClass roleObj = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );

            //set default permissions
            Collection<CswNbtNode> roles = roleObj.getNodes( false, true );
            foreach( CswNbtObjClassRole role in roles )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.Create, feedbackNT, role, true );
                _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.View, feedbackNT, role, true );
                _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.Edit, feedbackNT, role, true );
            }

            Int32 sequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "Feedback CaseNo" ), "F", "", 6, 1 );
            CswNbtMetaDataNodeTypeProp caseNumberNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.CaseNumberPropertyName );
            caseNumberNTP.setSequence( sequenceId );
            caseNumberNTP.ReadOnly = true; //default to read-only, but allow it to be changed

            CswNbtMetaDataNodeTypeProp authorNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.AuthorPropertyName );
            authorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            CswNbtMetaDataNodeTypeProp dateNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.DateSubmittedPropertyName );
            dateNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            CswNbtMetaDataNodeTypeProp viewNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.ViewPropertyName );
            viewNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );

            CswNbtMetaDataNodeTypeProp actionNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.ActionPropertyName );
            actionNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );

            CswNbtMetaDataNodeTypeProp selectedNodeIdNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.SelectedNodeIDPropertyName );
            selectedNodeIdNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );

            CswNbtMetaDataNodeTypeProp currentViewModeNTP = feedbackNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFeedback.CurrentViewModePropertyName );
            currentViewModeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
            currentViewModeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            //create the role view for admins
            CswNbtNode adminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != adminRoleNode )
            {
                CswNbtView feedbackView = _CswNbtSchemaModTrnsctn.makeView();

                feedbackView.makeNew( "Feedback", NbtViewVisibility.Role, adminRoleNode.NodeId );
                feedbackView.Category = "System";

                //add root
                CswNbtViewRelationship ParentRelationship = feedbackView.AddViewRelationship( feedbackOC, true );

                //add properties
                CswNbtViewProperty caseNumberVP = feedbackView.AddViewProperty( ParentRelationship, caseNumberOCP );
                CswNbtViewProperty dateSubmittedVp = feedbackView.AddViewProperty( ParentRelationship, dateSubmittedOCP );
                CswNbtViewProperty categoryVP = feedbackView.AddViewProperty( ParentRelationship, categoryOCP );
                CswNbtViewProperty authorVP = feedbackView.AddViewProperty( ParentRelationship, authorOCP );
                CswNbtViewProperty subjectVP = feedbackView.AddViewProperty( ParentRelationship, subjectOCP );
                CswNbtViewProperty statusVP = feedbackView.AddViewProperty( ParentRelationship, statusOCP );
                CswNbtViewProperty commentsVP = feedbackView.AddViewProperty( ParentRelationship, commentsOCP );

                caseNumberVP.Order = 1;
                dateSubmittedVp.Order = 2;
                categoryVP.Order = 3;
                authorVP.Order = 4;
                subjectVP.Order = 5;
                statusVP.Order = 6;
                commentsVP.Order = 7;

                caseNumberVP.Width = 15;
                dateSubmittedVp.Width = 20;
                categoryVP.Width = 20;
                authorVP.Width = 20;
                subjectVP.Width = 25;
                statusVP.Width = 20;
                commentsVP.Width = 100;

                feedbackView.SetViewMode( NbtViewRenderingMode.Grid );

                CswNbtViewPropertyFilter feedbackViewFilter = feedbackView.AddViewPropertyFilter( statusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: "Pending review", ShowAtRuntime: true );
                feedbackView.save();
            }

            // create the global view for all users to view their own feedback
            CswNbtView myFeedbackView = _CswNbtSchemaModTrnsctn.makeView();

            myFeedbackView.makeNew( "My Feedback", NbtViewVisibility.Global );
            myFeedbackView.Category = "System";

            //add root
            CswNbtViewRelationship myParentRelationship = myFeedbackView.AddViewRelationship( feedbackOC, true );

            //add properties
            CswNbtViewProperty myCaseNumberVP = myFeedbackView.AddViewProperty( myParentRelationship, caseNumberOCP );
            CswNbtViewProperty myDateSubmittedVp = myFeedbackView.AddViewProperty( myParentRelationship, dateSubmittedOCP );
            CswNbtViewProperty myCategoryVP = myFeedbackView.AddViewProperty( myParentRelationship, categoryOCP );
            CswNbtViewProperty myAuthorVP = myFeedbackView.AddViewProperty( myParentRelationship, authorOCP );
            CswNbtViewProperty mySubjectVP = myFeedbackView.AddViewProperty( myParentRelationship, subjectOCP );
            CswNbtViewProperty myStatusVP = myFeedbackView.AddViewProperty( myParentRelationship, statusOCP );
            CswNbtViewProperty myCommentsVP = myFeedbackView.AddViewProperty( myParentRelationship, commentsOCP );

            myCaseNumberVP.Order = 1;
            myDateSubmittedVp.Order = 2;
            myCategoryVP.Order = 3;
            myAuthorVP.Order = 4;
            mySubjectVP.Order = 5;
            myStatusVP.Order = 6;
            myCommentsVP.Order = 7;

            myCaseNumberVP.Width = 15;
            myDateSubmittedVp.Width = 20;
            myCategoryVP.Width = 20;
            myAuthorVP.Width = 20;
            mySubjectVP.Width = 25;
            myStatusVP.Width = 20;
            myCommentsVP.Width = 100;

            myFeedbackView.SetViewMode( NbtViewRenderingMode.Grid );

            //observe "me", this is voodoo (or magic...w/e) and sets it to the current user (from steve)
            CswNbtViewPropertyFilter myFeedbackViewFilter = myFeedbackView.AddViewPropertyFilter( myAuthorVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: "me", ShowAtRuntime: false );
            myFeedbackView.save();

        }//Update()

    }//class CswUpdateSchemaCase24523

}//namespace ChemSW.Nbt.Schema