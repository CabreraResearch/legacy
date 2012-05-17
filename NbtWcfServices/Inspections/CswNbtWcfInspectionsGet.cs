using System;
using System.Collections.ObjectModel;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.Response
{
    public class CswNbtWcfInspectionsGet
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;
        private CswNbtMetaDataObjectClass _InspectionDesignOc = null;
        private CswNbtView _SystemView;
        private CswNbtActSystemViews _NbtSystemView;
        private CswNbtWcfInspectionsResponseWithDesigns _InspectionsResponse;

        private void _initInspectionResources( CswNbtActSystemViews.SystemViewName ViewName )
        {
            _CswNbtWcfSessionResources = _InspectionsResponse.CswNbtWcfSessionResources;
            _InspectionDesignOc = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            _NbtSystemView = new CswNbtActSystemViews( _CswNbtWcfSessionResources.CswNbtResources,
                                                       ViewName,
                                                       _InspectionDesignOc
                );
            _SystemView = _NbtSystemView.SystemView;
        }

        private Collection<Int32> InspectionDesignTypeIds = new Collection<Int32>();
        private Collection<CswPrimaryKey> InspectionDesignNodeIds = new Collection<CswPrimaryKey>();

        private void _addNodeTypeInspectionDesignToResponse( CswNbtNode InspectionNode )
        {
            if( false == InspectionDesignTypeIds.Contains( InspectionNode.NodeTypeId ) )
            {
                CswNbtMetaDataNodeType NewInspectionNodeType = InspectionNode.getNodeType();
                InspectionDesignTypeIds.Add( NewInspectionNodeType.NodeTypeId );
                var ResponseDesign = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign
                {
                    DesignId = NewInspectionNodeType.NodeTypeId,
                    Name = NewInspectionNodeType.NodeTypeName
                };

                foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in NewInspectionNodeType.getNodeTypeTabs() )
                {
                    var ResponseSection = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.CswNbtInspectionDesignSection
                    {
                        Name = NodeTypeTab.TabName,
                        Order = NodeTypeTab.TabOrder,
                        SectionId = NodeTypeTab.TabId
                    };

                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeTab.getNodeTypePropsByDisplayOrder() )
                    {
                        var ResponseProperty = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.CswNbtInspectionDesignSectionProperty
                        {
                            HelpText = NodeTypeProp.HelpText
                        };
                        CswNbtMetaDataFieldType.NbtFieldType FieldType = NodeTypeProp.getFieldType().FieldType;
                        ResponseProperty.Type = FieldType.ToString();

                        if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                        {
                            ResponseProperty.Text = "Question " + NodeTypeProp.QuestionNo + ": " + NodeTypeProp.PropName;
                            ResponseProperty.QuestionId = NodeTypeProp.PropId;
                            CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                            PossibleAnswers.FromString( NodeTypeProp.ListOptions );
                            CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                            CompliantAnswers.FromString( NodeTypeProp.ValueOptions );
                            foreach( string Answer in PossibleAnswers )
                            {
                                var Choice =
                                    new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.CswNbtInspectionChoice
                                        {
                                            Text = Answer,
                                            IsCompliant = CompliantAnswers.Contains( Answer, false )
                                        };
                                ResponseProperty.Choices.Add( Choice );
                            }
                        }
                        ResponseSection.Properties.Add( ResponseProperty );
                    }
                    ResponseDesign.Sections.Add( ResponseSection );
                    _InspectionsResponse.Data.Designs.Add( ResponseDesign );
                }
            }
        }

        private void _addInspectionDesignNodeNodeToResponse( CswNbtNode InspectionNode )
        {
            if( false == InspectionDesignNodeIds.Contains( InspectionNode.NodeId ) )
            {
                InspectionDesignNodeIds.Add( InspectionNode.NodeId );
                CswNbtObjClassInspectionDesign NodeAsInspectionDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
                var ResponseInspection = new CswNbtWcfInspectionsDataModel.CswNbtInspection
                {
                    DesignId = InspectionNode.NodeTypeId,

                    DueDate = NodeAsInspectionDesign.Date.DateTimeValue,
                    InspectionId = NodeAsInspectionDesign.NodeId.PrimaryKey,
                    InspectionPointName = NodeAsInspectionDesign.Target.CachedNodeName,
                    LocationPath = NodeAsInspectionDesign.Location.Gestalt,
                    RouteName = default( string ),
                    Status = NodeAsInspectionDesign.Status.Value
                };

                foreach( CswNbtNodePropWrapper Prop in InspectionNode.Properties )
                {
                    if( Prop.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                    {
                        CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                        var ResponseQuestion = new CswNbtWcfInspectionsDataModel.CswNbtInspection.CswNbtInspectionQuestion
                        {
                            Answer = PropAsQuestion.Answer,
                            AnswerId = PropAsQuestion.JctNodePropId,
                            Comments = PropAsQuestion.Comments,
                            CorrectiveAction = PropAsQuestion.CorrectiveAction,
                            DateAnswered = PropAsQuestion.DateAnswered,
                            QuestionId = PropAsQuestion.NodeTypePropId,
                            Status = NodeAsInspectionDesign.Status.Value,
                            DateCorrected = PropAsQuestion.DateCorrected
                        };

                        ResponseInspection.Questions.Add( ResponseQuestion );
                    }
                }
                _InspectionsResponse.Data.Inspections.Add( ResponseInspection );
            }
        }

        private void _iterateTree( ICswNbtTree Tree )
        {
            Int32 ChildNodeCount = Tree.getChildNodeCount();
            if( ChildNodeCount > 0 )
            {
                for( Int32 I = 0; I < ChildNodeCount; I += 1 )
                {
                    Tree.goToNthChild( I );
                    CswNbtNode NodeForCurrentPosition = Tree.getNodeForCurrentPosition();
                    if( NodeForCurrentPosition.ObjClass.ObjectClass.ObjectClass == _InspectionDesignOc.ObjectClass )
                    {
                        _addNodeTypeInspectionDesignToResponse( NodeForCurrentPosition );
                        _addInspectionDesignNodeNodeToResponse( NodeForCurrentPosition );
                    }
                    if( Tree.getChildNodeCount() > 0 )
                    {
                        _iterateTree( Tree );
                    }
                    Tree.goToParentNode();
                }
            }
        }

        private void _makeInspectionReturn()
        {
            if( null != _SystemView )
            {
                ICswNbtTree Tree = _CswNbtWcfSessionResources.CswNbtResources.Trees.getTreeFromView( _SystemView, true, false );
                _iterateTree( Tree );
            }
        }

        public CswNbtWcfInspectionsGet( HttpContext Context, CswNbtActSystemViews.SystemViewName ViewName )
        {
            _Context = Context;
            _InspectionsResponse = new CswNbtWcfInspectionsResponseWithDesigns( _Context );
            if( _InspectionsResponse.Status.Success )
            {
                try
                {
                    _initInspectionResources( ViewName );
                }
                catch( Exception ex )
                {
                    _InspectionsResponse.addError( ex );
                }
            }
        }

        public CswDateTime getCswDate( DateTime Date )
        {
            return new CswDateTime( _CswNbtWcfSessionResources.CswNbtResources, Date );
        }

        public void addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass, string PropertyName, object FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode = null )
        {
            try
            {
                if( ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown )
                {
                    FilterMode = FilterMode ?? CswNbtPropFilterSql.PropertyFilterMode.Contains;
                    CswNbtMetaDataObjectClass InstanceOc = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getObjectClass( ObjectClass );
                    if( null != InstanceOc )
                    {
                        CswNbtMetaDataObjectClassProp InstancePropertyOcp = InstanceOc.getObjectClassProp( PropertyName );
                        if( null != InstancePropertyOcp )
                        {
                            string FilterValueString = CswConvert.ToString( FilterValue );
                            CswNbtActSystemViews.SystemViewPropFilterDefinition ViewPropertyFilter = _NbtSystemView.makeSystemViewFilter( InstancePropertyOcp, FilterValueString, FilterMode );
                            _NbtSystemView.addSystemViewFilter( ViewPropertyFilter, InstanceOc );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                _InspectionsResponse.addError( ex );
            }
        }



        public CswNbtWcfInspectionsResponseWithDesigns finalize()
        {
            try
            {
                _makeInspectionReturn();
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