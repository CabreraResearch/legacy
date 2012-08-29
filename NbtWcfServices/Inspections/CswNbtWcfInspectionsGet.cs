using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
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

                foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in from CswNbtMetaDataNodeTypeTab _NodeTypeTab
                                                                      in NewInspectionNodeType.getVisibleNodeTypeTabs()
                                                                  orderby _NodeTypeTab.TabOrder
                                                                  select _NodeTypeTab )
                {
                    var ResponseSection = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.Section
                    {
                        Name = NodeTypeTab.TabName,
                        Order = NodeTypeTab.TabOrder,
                        SectionId = NodeTypeTab.TabId,
                        ReadOnly = ( false == _CswNbtWcfSessionResources.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NewInspectionNodeType, CheckAllTabPermissions: false, NodeTypeTab: NodeTypeTab ) )
                    };

                    IEnumerable<CswNbtMetaDataNodeTypeProp> NodeTypeProps = NodeTypeTab.getNodeTypePropsByDisplayOrder();
                    //Debug.Assert( NodeTypeProps != null, "NodeTypeProps != null" );
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp
                                                                            in NodeTypeProps
                                                                        where true != _NodeTypeProp.HideInMobile &&
                                                                              _NodeTypeProp.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Question &&
                                                                              _propIsSupportedInMobile( _NodeTypeProp.getFieldType().FieldType )
                                                                        select _NodeTypeProp )
                    {

                        var ResponseProperty = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.SectionProperty
                                                   {
                                                       HelpText = NodeTypeProp.HelpText,
                                                       Type = NodeTypeProp.getFieldType().FieldType.ToString(),
                                                       QuestionId = NodeTypeProp.PropId,
                                                       Text = NodeTypeProp.PropName,
                                                       Choices = null
                                                   };
                        ResponseSection.Properties.Add( ResponseProperty );
                    }

                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp
                                                                            in NodeTypeProps
                                                                        orderby _NodeTypeProp.PropNameWithQuestionNo
                                                                        where true != _NodeTypeProp.HideInMobile &&
                                                                              _NodeTypeProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question
                                                                        select _NodeTypeProp )
                    {
                        var ResponseProperty = new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.SectionProperty
                        {
                            HelpText = NodeTypeProp.HelpText,
                            Type = CswNbtMetaDataFieldType.NbtFieldType.Question,
                            QuestionId = NodeTypeProp.PropId,
                            PreferredAnswer = NodeTypeProp.Extended,
                            Text = "Question " + NodeTypeProp.QuestionNo + ": " + NodeTypeProp.PropName
                        };

                        CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                        PossibleAnswers.FromString( NodeTypeProp.ListOptions );
                        CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                        CompliantAnswers.FromString( NodeTypeProp.ValueOptions );
                        foreach( string Answer in PossibleAnswers )
                        {
                            var Choice =
                                new CswNbtWcfInspectionsDataModel.CswNbtInspectionDesign.AnswerChoice
                                {
                                    Text = Answer,
                                    IsCompliant = CompliantAnswers.Contains( Answer, false )
                                };
                            ResponseProperty.Choices.Add( Choice );
                        }
                        ResponseSection.Properties.Add( ResponseProperty );
                    }

                    if( ResponseSection.Properties.Count > 0 )
                    {
                        ResponseDesign.Sections.Add( ResponseSection );
                    }
                }
                _InspectionsResponse.Data.Designs.Add( ResponseDesign );
            }
        }

        private bool _propIsSupportedInMobile( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return ( FieldType != CswNbtResources.UnknownEnum &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Button &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Composite &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.File &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Image &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.ImageList &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.LogicalSet &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.MOL &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.MTBF &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.MultiList &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.NFPA &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Quantity &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.Scientific &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.TimeInterval &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.UserSelect &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewPickList &&
                    FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewReference
                   );
        }

        private void _addInspectionDesignNodeNodeToResponse( CswNbtNode InspectionNode )
        {
            if( false == InspectionDesignNodeIds.Contains( InspectionNode.NodeId ) )
            {
                InspectionDesignNodeIds.Add( InspectionNode.NodeId );
                CswNbtObjClassInspectionDesign NodeAsInspectionDesign = InspectionNode;
                var ResponseInspection = new CswNbtWcfInspectionsDataModel.CswNbtInspection
                {
                    DesignId = InspectionNode.NodeTypeId,

                    DueDate = NodeAsInspectionDesign.Date.DateTimeValue,
                    InspectionId = NodeAsInspectionDesign.NodeId.PrimaryKey,
                    InspectionPointName = NodeAsInspectionDesign.Target.CachedNodeName,
                    LocationPath = NodeAsInspectionDesign.Location.Gestalt,
                    RouteName = default( string ),
                    Status = NodeAsInspectionDesign.Status.Value,
                    Counts = new CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionCounts()

                };

                foreach( CswNbtNodePropWrapper Prop in InspectionNode.Properties )
                {
                    if( Prop.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                    {
                        CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                        ResponseInspection.Counts.Total += 1;
                        if( false == string.IsNullOrEmpty( PropAsQuestion.Answer ) || PropAsQuestion.DateAnswered != DateTime.MinValue )
                        {
                            if( PropAsQuestion.IsCompliant )
                            {
                                ResponseInspection.Counts.Answered += 1;
                            }
                            else
                            {
                                ResponseInspection.Counts.Ooc += 1;
                            }
                        }
                        else
                        {
                            ResponseInspection.Counts.UnAnswered += 1;
                        }

                        var ResponseQuestion = new CswNbtWcfInspectionsDataModel.CswNbtInspection.QuestionAnswer
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
                    if( false == Tree.getNodeLockedForCurrentPosition() )
                    {
                        CswNbtNode NodeForCurrentPosition = Tree.getNodeForCurrentPosition();
                        if( NodeForCurrentPosition.ObjClass.ObjectClass.ObjectClass == _InspectionDesignOc.ObjectClass )
                        {
                            _addNodeTypeInspectionDesignToResponse( NodeForCurrentPosition );
                            _addInspectionDesignNodeNodeToResponse( NodeForCurrentPosition );
                        }
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

        public CswNbtWcfInspectionsGet( HttpContext Context, CswNbtActSystemViews.SystemViewName ViewName, bool IsMobile = true )
        {
            _Context = Context;
            _InspectionsResponse = new CswNbtWcfInspectionsResponseWithDesigns( _Context, IsMobile );
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

        public void addSystemViewPropFilter( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass, string PropertyName, object FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode = null, CswNbtMetaDataFieldType.NbtFieldType FieldType = null )
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
                            CswNbtActSystemViews.SystemViewPropFilterDefinition ViewPropertyFilter = _NbtSystemView.makeSystemViewFilter( InstancePropertyOcp, FilterValueString, FilterMode, FieldType: FieldType );
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

        public void addSystemViewBarcodeFilter( object FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode = null, CswNbtMetaDataFieldType.NbtFieldType FieldType = null )
        {
            try
            {
                FilterMode = FilterMode ?? CswNbtPropFilterSql.PropertyFilterMode.Contains;
                foreach( CswNbtViewRelationship RootLevelRelationship in _NbtSystemView.SystemView.Root.ChildRelationships )
                {
                    CswNbtMetaDataObjectClass InstanceOc = null;
                    CswNbtMetaDataObjectClassProp BarcodeOcp = null;
                    if( NbtViewRelatedIdType.ObjectClassId == RootLevelRelationship.SecondType )
                    {
                        InstanceOc = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getObjectClass( RootLevelRelationship.SecondId );
                        if( null != InstanceOc )
                        {
                            BarcodeOcp = InstanceOc.getBarcodeProp();

                        }
                    }
                    else if( NbtViewRelatedIdType.NodeTypeId == RootLevelRelationship.SecondType )
                    {
                        CswNbtMetaDataNodeType InstanceNt = _CswNbtWcfSessionResources.CswNbtResources.MetaData.getNodeType( RootLevelRelationship.SecondId );
                        if( null != InstanceNt )
                        {
                            InstanceOc = InstanceNt.getObjectClass();
                            CswNbtMetaDataNodeTypeProp BarcodeNtp = InstanceNt.getBarcodeProperty();
                            if( null != BarcodeNtp )
                            {
                                BarcodeOcp = BarcodeNtp.getObjectClassProp();
                            }
                        }
                    }

                    if( null != BarcodeOcp && null != InstanceOc )
                    {
                        string FilterValueString = CswConvert.ToString( FilterValue );
                        CswNbtActSystemViews.SystemViewPropFilterDefinition ViewPropertyFilter = _NbtSystemView.makeSystemViewFilter( BarcodeOcp, FilterValueString, FilterMode, FieldType: FieldType );
                        _NbtSystemView.addSystemViewFilter( ViewPropertyFilter, InstanceOc );
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