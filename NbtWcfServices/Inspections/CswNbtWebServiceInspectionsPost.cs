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
    public class CswNbtWebServiceInspectionsPost
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtSessionResources _CswNbtSessionResources = null;
        private Collection<CswNbtInspectionsDataModel.CswNbtInspection> _Inspections;
        private CswNbtWebServiceResponseInspections _Response;

        public CswNbtWebServiceInspectionsPost( HttpContext Context, Collection<CswNbtInspectionsDataModel.CswNbtInspection> Inspections )
        {
            _Context = Context;
            _Response = new CswNbtWebServiceResponseInspections( _Context );
            if( _Response.Status.Success )
            {
                try
                {
                    _Inspections = Inspections;
                    if( null == _Inspections || _Inspections.Count < 1 )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot post Inspection updates if the Inspection collection is empty.", "The provided Inspections collection was null or empty." );
                    }
                    _CswNbtSessionResources = _Response.CswNbtSessionResources;
                }
                catch( Exception ex )
                {
                    _Response.addError( ex );
                }
            }
        }

        string Completed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed );
        string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled );
        string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late );
        string Missed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed );
        string ActionRequired = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required );

        private bool _updateInspectionNode( CswNbtInspectionsDataModel.CswNbtInspection Inspection )
        {
            bool Processed = false;
            try
            {
                if( Int32.MinValue != Inspection.InspectionId )
                {
                    CswPrimaryKey InspectionPk = new CswPrimaryKey( "nodes", Inspection.InspectionId );
                    CswNbtNode InspectionNode = _CswNbtSessionResources.CswNbtResources.Nodes.GetNode( InspectionPk, Inspection.DesignId );
                    if( null != InspectionNode )
                    {
                        CswNbtObjClassInspectionDesign NodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
                        if( NodeAsDesign.Status.Value == Completed || NodeAsDesign.Status.Value == CompletedLate )
                        {
                            Processed = true;
                            _Response.Completed.Add( Inspection );
                        }
                        else if( NodeAsDesign.Status.Value == Cancelled )
                        {
                            Processed = true;
                            _Response.Cancelled.Add( Inspection );
                        }
                        else if( NodeAsDesign.Status.Value == Missed )
                        {
                            Processed = true;
                            _Response.Missed.Add( Inspection );
                        }
                        else
                        {
                            /* We loop once to set the property values */
                            foreach( CswNbtInspectionsDataModel.CswNbtInspection.CswNbtInspectionQuestion Question in Inspection.Questions )
                            {
                                CswNbtMetaDataNodeTypeProp Ntp = InspectionNode.getNodeType().getNodeTypeProp( Question.QuestionId );
                                if( null != Ntp )
                                {
                                    CswNbtNodePropWrapper Prop = InspectionNode.Properties[Ntp];
                                    CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                                    PropAsQuestion.Answer = Question.Answer;
                                    PropAsQuestion.CorrectiveAction = Question.CorrectiveAction;
                                    PropAsQuestion.DateAnswered = Question.DateAnswered;
                                    PropAsQuestion.DateCorrected = Question.DateCorrected;
                                    PropAsQuestion.Comments = Question.Comments;
                                }
                            }
                            InspectionNode.postChanges( true );
                            Processed = true;

                            /* Reinit since state has changed. */
                            NodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );

                            if( NodeAsDesign.Status.Value == Completed || NodeAsDesign.Status.Value == CompletedLate )
                            {
                                /* Nothing to so */
                            }
                            else if( NodeAsDesign.Status.Value == ActionRequired )
                            {
                                Inspection.Status = NodeAsDesign.Status.Value;
                                /* We loop again to modify the return with the status of the Inspection per Question */
                                foreach( CswNbtInspectionsDataModel.CswNbtInspection.CswNbtInspectionQuestion Question in Inspection.Questions )
                                {
                                    Question.Status = NodeAsDesign.Status.Value;
                                }
                                /* In case the Inspection has been modified by someone else */
                                Inspection.DueDate = NodeAsDesign.InspectionDate.DateTimeValue;
                                Inspection.InspectionPointName = NodeAsDesign.Target.CachedNodeName;
                                Inspection.LocationPath = NodeAsDesign.Location.CachedValue;
                                _Response.ActionRequired.Add( Inspection );
                            }
                            else
                            {
                                _Response.InComplete.Add( Inspection );
                            }
                        }
                    }
                }
            }
            catch( Exception Ex )
            {
                _Response.addError( Ex );
            }
            return Processed;
        }

        private void _updateInspections()
        {
            foreach( CswNbtInspectionsDataModel.CswNbtInspection Inspection in _Inspections )
            {
                bool Processed = _updateInspectionNode( Inspection );
                if( false == Processed )
                {
                    _Response.Failed.Add( Inspection );
                }
            }
        }

        public CswNbtWebServiceResponseInspections finalize()
        {
            try
            {
                _updateInspections();
                _Response.finalizeResponse();
            }
            catch( Exception ex )
            {
                _Response.addError( ex );
            }
            return _Response;
        }

    }

}