using System;
using System.Collections.ObjectModel;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public class CswNbtWcfInspectionsPost
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;
        private Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> _Inspections;
        private CswNbtWcfInspectionsResponse _InspectionsResponse;
        private CswNbtMetaDataObjectClass _InspectionDesignOc = null;

        public CswNbtWcfInspectionsPost( HttpContext Context, Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Inspections, bool IsMobile = true )
        {
            _Context = Context;

            _InspectionsResponse = new CswNbtWcfInspectionsResponse( _Context, IsMobile );
            if( _InspectionsResponse.Status.Success )
            {
                try
                {
                    _Inspections = Inspections;
                    if( null == _Inspections || _Inspections.Count < 1 )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot post Inspection updates if the Inspection collection is empty.", "The provided Inspections collection was null or empty." );
                    }
                    _CswNbtWcfSessionResources = _InspectionsResponse.CswNbtWcfSessionResources;
                }
                catch( Exception ex )
                {
                    _InspectionsResponse.addError( ex );
                }
            }
        }

        string Completed = CswNbtObjClassInspectionDesign.InspectionStatus.Completed;
        string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled;
        string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatus.CompletedLate;
        string Missed = CswNbtObjClassInspectionDesign.InspectionStatus.Missed;
        string ActionRequired = CswNbtObjClassInspectionDesign.InspectionStatus.ActionRequired;

        private bool _updateInspectionNode( CswNbtWcfInspectionsDataModel.CswNbtInspection Inspection )
        {
            bool Processed = false;
            try
            {
                if( Int32.MinValue != Inspection.InspectionId )
                {
                    CswPrimaryKey InspectionPk = new CswPrimaryKey( "nodes", Inspection.InspectionId );
                    CswNbtNode InspectionNode = _CswNbtWcfSessionResources.CswNbtResources.Nodes.GetNode( InspectionPk, Inspection.DesignId );
                    if( null != InspectionNode )
                    {
                        CswNbtObjClassInspectionDesign NodeAsDesign = (CswNbtObjClassInspectionDesign) InspectionNode;
                        if( NodeAsDesign.Status.Value == Completed || NodeAsDesign.Status.Value == CompletedLate )
                        {
                            Processed = true;
                            _InspectionsResponse.Completed.Add( Inspection );
                        }
                        else if( NodeAsDesign.Status.Value == Cancelled )
                        {
                            Processed = true;
                            _InspectionsResponse.Cancelled.Add( Inspection );
                        }
                        else if( NodeAsDesign.Status.Value == Missed )
                        {
                            Processed = true;
                            _InspectionsResponse.Missed.Add( Inspection );
                        }
                        else
                        {
                            Inspection.Counts = new CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionCounts();
                            /* We loop once to set the property values */
                            CswNbtMetaDataNodeType InspectionNt = InspectionNode.getNodeType();
                            if( null != InspectionNt )
                            {
                                //Can edit the nodetype
                                if( _CswNbtWcfSessionResources.CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, InspectionNt ) )
                                {
                                    foreach( CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionAnswer Question in Inspection.Questions )
                                    {
                                        CswNbtMetaDataNodeTypeProp Ntp = InspectionNt.getNodeTypeProp( Question.QuestionId );
                                        if( null != Ntp )
                                        {
                                            CswNbtMetaDataNodeTypeTab Tab = InspectionNt.getNodeTypeTab( Ntp.FirstEditLayout.TabId );
                                            if( null != Tab )
                                            {
                                                bool CanEdit = (
                                                                    _CswNbtWcfSessionResources.CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, InspectionNt, Tab ) ||
                                                                    _CswNbtWcfSessionResources.CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.Edit, Ntp, Tab )
                                                                );

                                                CswNbtNodePropQuestion PropAsQuestion = InspectionNode.Properties[Ntp];
                                                if( CanEdit )
                                                {
                                                    PropAsQuestion.Answer = Question.Answer;
                                                    PropAsQuestion.CorrectiveAction = Question.CorrectiveAction;
                                                    DateTime DateAnswered = CswConvert.ToDateTime( Question.DateAnswered );
                                                    if( DateTime.MinValue != DateAnswered )
                                                    {
                                                        PropAsQuestion.DateAnswered = DateAnswered;
                                                    }
                                                    DateTime DateCorrected = CswConvert.ToDateTime( Question.DateCorrected );
                                                    if( DateTime.MinValue != DateCorrected )
                                                    {
                                                        PropAsQuestion.DateCorrected = DateCorrected;
                                                    }
                                                    PropAsQuestion.Comments = Question.Comments;
                                                }
                                                if( false == string.IsNullOrEmpty( Question.Answer ) )
                                                {
                                                    Inspection.Counts.Answered += 1;
                                                }
                                                else
                                                {
                                                    Inspection.Counts.UnAnswered += 1;
                                                }
                                                if( false == PropAsQuestion.IsCompliant )
                                                {
                                                    Inspection.Counts.Ooc += 1;
                                                }
                                                Inspection.Counts.Total += 1;
                                            }
                                        }
                                    }
                                    InspectionNode.postChanges( true );
                                    if( false == string.IsNullOrEmpty( Inspection.Action ) && ( Inspection.Action.ToLower() == "finish" || Inspection.Action.ToLower() == "cancel" ) )
                                    {
                                        CswNbtMetaDataNodeTypeProp ButtonNtp = null;
                                        if( Inspection.Action.ToLower() == "finish" )
                                        {
                                            ButtonNtp = InspectionNode.getNodeType().getNodeTypeProp( CswNbtObjClassInspectionDesign.PropertyName.Finish );
                                        }
                                        else if( Inspection.Action.ToLower() == "cancel" )
                                        {
                                            ButtonNtp = InspectionNode.getNodeType().getNodeTypeProp( CswNbtObjClassInspectionDesign.PropertyName.Cancel );
                                        }

                                        if( null != ButtonNtp )
                                        {
                                            CswNbtMetaDataNodeTypeTab ButtonTab = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getNodeTypeTab( ButtonNtp.FirstEditLayout.TabId );
                                            if( null != ButtonTab &&
                                                    (
                                                        _CswNbtWcfSessionResources.CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, InspectionNt, NodeTypeTab: ButtonTab ) ||
                                                        _CswNbtWcfSessionResources.CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.Edit, ButtonNtp, ButtonTab )
                                                    )
                                               )
                                            {
                                                _InspectionDesignOc = _InspectionDesignOc ?? _CswNbtWcfSessionResources.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
                                                CswNbtObjClass NbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtWcfSessionResources.CswNbtResources, _InspectionDesignOc, InspectionNode );
                                                CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( ButtonNtp );
                                                NbtObjClass.onButtonClick( ButtonData );
                                            }
                                        }
                                    }
                                    Processed = true;
                                }
                            }
                            /* Reinit since state has changed. */
                            NodeAsDesign = InspectionNode;

                            if( NodeAsDesign.Status.Value == Completed || NodeAsDesign.Status.Value == CompletedLate )
                            {
                                /* Nothing to do */
                            }
                            else if( NodeAsDesign.Status.Value == ActionRequired )
                            {
                                Inspection.Status = NodeAsDesign.Status.Value;
                                /* We loop again to modify the return with the status of the Inspection per QuestionAnswer */
                                foreach( CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionAnswer Question in Inspection.Questions )
                                {
                                    Question.Status = NodeAsDesign.Status.Value;
                                }
                                /* In case the Inspection has been modified by someone else */
                                Inspection.DueDateAsDate = NodeAsDesign.InspectionDate.DateTimeValue;
                                Inspection.InspectionPointName = NodeAsDesign.Target.CachedNodeName;
                                Inspection.LocationPath = NodeAsDesign.Location.CachedValue;
                                _InspectionsResponse.ActionRequired.Add( Inspection );
                            }
                            else
                            {
                                _InspectionsResponse.InComplete.Add( Inspection );
                            }
                        }
                    }
                }
            }
            catch( Exception Ex )
            {
                _InspectionsResponse.addError( Ex );
            }
            return Processed;
        }

        private void _updateInspections()
        {

            foreach( CswNbtWcfInspectionsDataModel.CswNbtInspection Inspection in _Inspections )
            {
                bool Processed = _updateInspectionNode( Inspection );
                if( false == Processed )
                {
                    _InspectionsResponse.Failed.Add( Inspection );
                }
            }
        }

        public CswNbtWcfInspectionsResponse finalize()
        {
            try
            {
                _updateInspections();
                _InspectionsResponse.finalizeResponse();
            }
            catch( Exception ex )
            {
                _InspectionsResponse.addError( ex );
            }
            return _InspectionsResponse;
        }

    }

}