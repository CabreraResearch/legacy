using System;
using System.Collections.ObjectModel;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public class CswNbtWcfInspectionsPost
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;
        private Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> _Inspections;
        private CswNbtWcfInspectionsResponse _InspectionsResponse;

        public CswNbtWcfInspectionsPost( HttpContext Context, Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Inspections )
        {
            _Context = Context;
            _InspectionsResponse = new CswNbtWcfInspectionsResponse( _Context );
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

        string Completed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed );
        string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled );
        string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late );
        string Missed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed );
        string ActionRequired = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required );

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
                        CswNbtObjClassInspectionDesign NodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
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
                            /* We loop once to set the property values */
                            foreach( CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionAnswer Question in Inspection.Questions )
                            {
                                CswNbtMetaDataNodeTypeProp Ntp = InspectionNode.getNodeType().getNodeTypeProp( Question.QuestionId );
                                if( null != Ntp )
                                {
                                    CswNbtNodePropWrapper Prop = InspectionNode.Properties[Ntp];
                                    CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                                    PropAsQuestion.Answer = Question.Answer;
                                    PropAsQuestion.CorrectiveAction = Question.CorrectiveAction;
                                    DateTime DateAnswered = CswConvert.ToDateTime( Question.DateAnswered );
                                    DateTime DateCorrected = CswConvert.ToDateTime( Question.DateCorrected );
                                    PropAsQuestion.DateAnswered = DateAnswered;
                                    PropAsQuestion.DateCorrected = DateCorrected;
                                    PropAsQuestion.Comments = Question.Comments;
                                }
                            }
                            InspectionNode.postChanges( true );
                            Processed = true;

                            /* Reinit since state has changed. */
                            NodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );

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
                                Inspection.DueDate = NodeAsDesign.InspectionDate.DateTimeValue;
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