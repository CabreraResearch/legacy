using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using NbtWebAppServices.Response;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdInspections
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtMetaDataObjectClass _InspectionDesignOc = null;
        private CswNbtView _SystemView;
        private CswNbtActSystemViews _NbtSystemView;


        private Collection<Int32> InspectionDesignTypeIds = new Collection<Int32>();
        private Collection<CswPrimaryKey> InspectionDesignNodeIds = new Collection<CswPrimaryKey>();
        private Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection> _Inspections;
        //private CswNbtSdInspectionsDataModels.InspectionData _InspectionResponse;

        public CswNbtSdInspections( CswNbtResources Resources, CswEnumNbtSystemViewName ViewName )
        {
            _CswNbtResources = Resources;
            _InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            _NbtSystemView = new CswNbtActSystemViews( _CswNbtResources,
                                                       ViewName,
                                                       _InspectionDesignOc
                );
            _SystemView = _NbtSystemView.SystemView;

        }
        public CswNbtSdInspections( CswNbtResources Resources, CswEnumNbtSystemViewName ViewName, Collection<CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection> Inspections )
        {
            _CswNbtResources = Resources;
            _InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            _NbtSystemView = new CswNbtActSystemViews( _CswNbtResources,
                                                       ViewName,
                                                       _InspectionDesignOc
                );
            _SystemView = _NbtSystemView.SystemView;
            _Inspections = Inspections;
        }

        #region Get


        private CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign _addNodeTypeInspectionDesignToResponse( CswNbtNode InspectionNode )
        {
            CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign Ret = null;
            if( false == InspectionDesignTypeIds.Contains( InspectionNode.NodeTypeId ) )
            {
                CswNbtMetaDataNodeType NewInspectionNodeType = InspectionNode.getNodeType();
                InspectionDesignTypeIds.Add( NewInspectionNodeType.NodeTypeId );
                Ret = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign
                {
                    DesignId = NewInspectionNodeType.NodeTypeId,
                    Name = NewInspectionNodeType.NodeTypeName
                };

                foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in from CswNbtMetaDataNodeTypeTab _NodeTypeTab
                                                                      in NewInspectionNodeType.getVisibleNodeTypeTabs()
                                                                  orderby _NodeTypeTab.TabOrder
                                                                  select _NodeTypeTab )
                {
                    bool canPropOnAnyOtherTab = ( false == _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, NewInspectionNodeType, NodeTypeTab : NodeTypeTab ) );
                    var ResponseSection = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign.Section
                    {
                        Name = NodeTypeTab.TabName,
                        Order = NodeTypeTab.TabOrder,
                        SectionId = NodeTypeTab.TabId,
                        ReadOnly = ( ( false == _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, NewInspectionNodeType ) ) && ( false == _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, NewInspectionNodeType, NodeTypeTab : NodeTypeTab ) ) )
                    };

                    IEnumerable<CswNbtMetaDataNodeTypeProp> NodeTypeProps = NodeTypeTab.getNodeTypePropsByDisplayOrder();
                    //Debug.Assert( NodeTypeProps != null, "NodeTypeProps != null" );

                    IEnumerable<CswNbtMetaDataNodeTypeProp> TypeProps = NodeTypeProps as CswNbtMetaDataNodeTypeProp[] ?? NodeTypeProps.ToArray();
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp
                                                                            in TypeProps
                                                                        where _NodeTypeProp.getFieldTypeValue() != CswEnumNbtFieldType.Question &&
                                                                              _propIsSupportedInMobile( _NodeTypeProp.getFieldTypeValue() )
                                                                        select _NodeTypeProp )
                    {

                        ResponseSection.Properties.Add( new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign.SectionProperty
                        {
                            HelpText = NodeTypeProp.HelpText,
                            Type = NodeTypeProp.getFieldTypeValue().ToString(),
                            QuestionId = NodeTypeProp.PropId,
                            Text = NodeTypeProp.PropName,
                            Choices = null,
                            ReadOnly = canPropOnAnyOtherTab
                        } );
                    }

                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp
                                                                            in TypeProps
                                                                        orderby _NodeTypeProp.QuestionNo
                                                                        where _NodeTypeProp.getFieldTypeValue() == CswEnumNbtFieldType.Question &&
                                                                              false == _NodeTypeProp.ReadOnly &&
                                                                              _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, _NodeTypeProp, null )
                                                                        select _NodeTypeProp )
                    {
                        var ResponseProperty = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign.SectionProperty
                        {
                            HelpText = NodeTypeProp.HelpText,
                            Type = CswEnumNbtFieldType.Question,
                            QuestionId = NodeTypeProp.PropId,
                            PreferredAnswer = NodeTypeProp.Extended,
                            Text = "Question " + NodeTypeProp.QuestionNo + ": " + NodeTypeProp.PropName,
                            ReadOnly = false
                        };

                        CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                        PossibleAnswers.FromString( NodeTypeProp.ListOptions );
                        CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                        CompliantAnswers.FromString( NodeTypeProp.ValueOptions );
                        foreach( string Answer in PossibleAnswers )
                        {
                            ResponseProperty.Choices.Add( new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign.AnswerChoice
                            {
                                Text = Answer,
                                IsCompliant = CompliantAnswers.Contains( Answer, false )
                            } );
                        }
                        ResponseSection.Properties.Add( ResponseProperty );
                    }

                    if( ResponseSection.Properties.Count > 0 )
                    {
                        Ret.Sections.Add( ResponseSection );
                    }
                }

            }
            return Ret;
        }

        private bool _propIsSupportedInMobile( CswEnumNbtFieldType FieldType )
        {
            return ( FieldType != CswNbtResources.UnknownEnum &&
                    FieldType != CswEnumNbtFieldType.Button &&
                    FieldType != CswEnumNbtFieldType.Composite &&
                    FieldType != CswEnumNbtFieldType.Grid &&
                    FieldType != CswEnumNbtFieldType.File &&
                    FieldType != CswEnumNbtFieldType.Image &&
                    FieldType != CswEnumNbtFieldType.ImageList &&
                    FieldType != CswEnumNbtFieldType.LocationContents &&
                    FieldType != CswEnumNbtFieldType.LogicalSet &&
                    FieldType != CswEnumNbtFieldType.MOL &&
                    FieldType != CswEnumNbtFieldType.MTBF &&
                    FieldType != CswEnumNbtFieldType.MultiList &&
                    FieldType != CswEnumNbtFieldType.NFPA &&
                    FieldType != CswEnumNbtFieldType.NodeTypeSelect &&
                    FieldType != CswEnumNbtFieldType.Quantity &&
                    FieldType != CswEnumNbtFieldType.Scientific &&
                    FieldType != CswEnumNbtFieldType.TimeInterval &&
                    FieldType != CswEnumNbtFieldType.UserSelect &&
                    FieldType != CswEnumNbtFieldType.ViewPickList &&
                    FieldType != CswEnumNbtFieldType.ViewReference
                   );
        }

        private CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection _addChecklistNodesToResponse( CswNbtNode InspectionNode )
        {
            CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection Ret = null;
            if( false == InspectionDesignNodeIds.Contains( InspectionNode.NodeId ) &&
                false == InspectionNode.ReadOnly )
            {
                InspectionDesignNodeIds.Add( InspectionNode.NodeId );
                CswNbtObjClassInspectionDesign NodeAsInspectionDesign = InspectionNode;
                Ret = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection
                {
                    DesignId = InspectionNode.NodeTypeId,

                    DueDateAsDate = NodeAsInspectionDesign.DueDate.DateTimeValue,
                    InspectionId = NodeAsInspectionDesign.NodeId.PrimaryKey,
                    InspectionPointName = NodeAsInspectionDesign.Target.CachedNodeName,
                    LocationPath = NodeAsInspectionDesign.Location.Gestalt,
                    RouteName = default( string ),
                    Status = NodeAsInspectionDesign.Status.Value,
                    Counts = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection.QuestionCounts(),
                    ReadOnly = InspectionNode.ReadOnly
                };

                foreach( CswNbtNodePropWrapper Prop in InspectionNode.Properties )
                {
                    if( Prop.getFieldTypeValue() == CswEnumNbtFieldType.Question &&
                        false == Prop.ReadOnly &&
                        _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, Prop.NodeTypeProp, null ) )
                    {
                        CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                        Ret.Counts.Total += 1;
                        if( false == string.IsNullOrEmpty( PropAsQuestion.Answer ) || PropAsQuestion.DateAnswered != DateTime.MinValue )
                        {
                            if( PropAsQuestion.IsCompliant )
                            {
                                Ret.Counts.Answered += 1;
                            }
                            else
                            {
                                Ret.Counts.Ooc += 1;
                            }
                        }
                        else
                        {
                            Ret.Counts.UnAnswered += 1;
                        }

                        var ResponseQuestion = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection.QuestionAnswer
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

                        Ret.Questions.Add( ResponseQuestion );
                    }
                    else if( Prop.getFieldTypeValue() == CswEnumNbtFieldType.Image &&
                        _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, Prop.NodeTypeProp, null ) )
                    {
                        CswNbtNodePropImage PropAsImage = Prop.AsImage;
                        CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                        Ret.Images = sdBlobData.GetImages( NodeAsInspectionDesign.NodeId, PropAsImage.JctNodePropId );
                        Ret.MaxImages = PropAsImage.MaxFiles >= 1 ? PropAsImage.MaxFiles : 10; //if no value set for MaxFiles, default to 10
                        Ret.ImagePropId = new CswPropIdAttr( InspectionNode, PropAsImage.NodeTypeProp ).ToString();
                    }
                }
            }
            return Ret;
        }

        private CswNbtSdInspectionsDataModels.InspectionData _iterateTree( ICswNbtTree Tree, CswNbtSdInspectionsDataModels.InspectionData Ret = null )
        {
            Ret = Ret ?? new CswNbtSdInspectionsDataModels.InspectionData();
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
                            CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspectionDesign Design = _addNodeTypeInspectionDesignToResponse( NodeForCurrentPosition );
                            if( null != Design )
                            {
                                Ret.Designs.Add( Design );
                            }
                            CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection Inspection = _addChecklistNodesToResponse( NodeForCurrentPosition );
                            if( null != Inspection )
                            {
                                Ret.Inspections.Add( Inspection );
                            }
                        }
                    }
                    if( Tree.getChildNodeCount() > 0 )
                    {
                        _iterateTree( Tree, Ret );
                    }
                    Tree.goToParentNode();
                }
            }
            return Ret;
        }

        private CswNbtSdInspectionsDataModels.InspectionData _makeInspectionReturn()
        {
            CswNbtSdInspectionsDataModels.InspectionData Ret = new CswNbtSdInspectionsDataModels.InspectionData();
            if( null != _SystemView )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _SystemView, true, false, false );
                Ret = _iterateTree( Tree );
            }
            return Ret;
        }

        public CswDateTime getCswDate( DateTime Date )
        {
            return new CswDateTime( _CswNbtResources, Date );
        }

        private void _addSystemViewPropFilter( CswEnumNbtObjectClass ObjectClass, string PropertyName, object FilterValue, CswEnumNbtFilterMode FilterMode = null, CswEnumNbtFieldType FieldType = null )
        {
            if( ObjectClass != CswNbtResources.UnknownEnum )
            {
                FilterMode = FilterMode ?? CswEnumNbtFilterMode.Contains;
                CswNbtMetaDataObjectClass InstanceOc = _CswNbtResources.MetaData.getObjectClass( ObjectClass );
                if( null != InstanceOc )
                {
                    CswNbtMetaDataObjectClassProp InstancePropertyOcp = InstanceOc.getObjectClassProp( PropertyName );
                    if( null != InstancePropertyOcp )
                    {
                        string FilterValueString = CswConvert.ToString( FilterValue );
                        CswNbtActSystemViews.SystemViewPropFilterDefinition ViewPropertyFilter = _NbtSystemView.makeSystemViewFilter( InstancePropertyOcp, FilterValueString, FilterMode, FieldType : FieldType );
                        _NbtSystemView.addSystemViewFilter( ViewPropertyFilter, InstanceOc );
                    }
                }
            }
        } // _addSystemViewPropFilter()

        private void _addSystemViewBarcodeFilter( object FilterValue, CswEnumNbtFilterMode FilterMode = null, CswEnumNbtFieldType FieldType = null )
        {

            FilterMode = FilterMode ?? CswEnumNbtFilterMode.Contains;
            foreach( CswNbtViewRelationship RootLevelRelationship in _NbtSystemView.SystemView.Root.ChildRelationships )
            {
                //CswNbtMetaDataObjectClass InstanceOc = null;
                //CswNbtMetaDataObjectClassProp BarcodeOcp = null;

                //if( NbtViewRelatedIdType.ObjectClassId == RootLevelRelationship.SecondType )
                //{
                //    InstanceOc = _CswNbtResources.MetaData.getObjectClass( RootLevelRelationship.SecondId );
                //    if( null != InstanceOc )
                //    {
                //        BarcodeOcp = (CswNbtMetaDataObjectClassProp) InstanceOc.getBarcodeProperty();

                //    }
                //}
                //else if( NbtViewRelatedIdType.NodeTypeId == RootLevelRelationship.SecondType )
                //{
                //    CswNbtMetaDataNodeType InstanceNt = _CswNbtResources.MetaData.getNodeType( RootLevelRelationship.SecondId );
                //    if( null != InstanceNt )
                //    {
                //        InstanceOc = InstanceNt.getObjectClass();
                //        CswNbtMetaDataNodeTypeProp BarcodeNtp = (CswNbtMetaDataNodeTypeProp) InstanceNt.getBarcodeProperty();
                //        if( null != BarcodeNtp )
                //        {
                //            BarcodeOcp = BarcodeNtp.getObjectClassProp();
                //        }
                //    }
                //}
                //else if( NbtViewRelatedIdType.PropertySetId == RootLevelRelationship.SecondType )
                //{
                //    // Not much we can do...
                //}

                ICswNbtMetaDataDefinitionObject secondObj = _CswNbtResources.MetaData.getDefinitionObject( RootLevelRelationship.SecondType, RootLevelRelationship.SecondId );
                ICswNbtMetaDataProp BarcodeProp = secondObj.getBarcodeProperty();

                if( null != BarcodeProp && null != secondObj )
                {
                    string FilterValueString = CswConvert.ToString( FilterValue );
                    CswNbtActSystemViews.SystemViewPropFilterDefinition ViewPropertyFilter = _NbtSystemView.makeSystemViewFilter( BarcodeProp, FilterValueString, FilterMode, FieldType : FieldType );
                    _NbtSystemView.addSystemViewFilter( ViewPropertyFilter, secondObj );
                }
            }
        } // _addSystemViewBarcodeFilter()

        public CswNbtSdInspectionsDataModels.InspectionData getInspectionsAndDesigns()
        {
            return _makeInspectionReturn();
        }

        public CswNbtSdInspectionsDataModels.InspectionData byDateRange( string StartingDate, string EndingDate )
        {
            if( _SystemView.ViewName != CswEnumNbtSystemViewName.SIInspectionsbyDate.ToString() )
            {
                _NbtSystemView.reInitSystemView( CswEnumNbtSystemViewName.SIInspectionsbyDate );
            }

            DateTime Start = CswConvert.ToDateTime( StartingDate );
            DateTime Today = DateTime.Today; //Today's time is 00:00:00 vs Now's time which is.. now
            if( DateTime.MinValue == Start )
            {
                Start = Today;
            }
            DateTime End = CswConvert.ToDateTime( EndingDate );
            if( DateTime.MinValue == End )
            {
                if( Start >= Today )
                {
                    End = Start.AddDays( 2 );
                }
                else
                {
                    End = Today.AddDays( 2 );
                }
            }
            if( Start > End )
            {
                End = Start;
                End = End.AddDays( 2 );
            }
            //In case we were provided valid dates, grab just the Day @midnight
            Start = Start.Date;
            End = End.Date;
            _addSystemViewPropFilter( CswEnumNbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.PropertyName.DueDate, Start.ToShortDateString(), CswEnumNbtFilterMode.GreaterThanOrEquals );
            _addSystemViewPropFilter( CswEnumNbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.PropertyName.DueDate, End.ToShortDateString(), CswEnumNbtFilterMode.LessThanOrEquals );


            return getInspectionsAndDesigns();
        }

        public CswNbtSdInspectionsDataModels.InspectionData byUser()
        {
            if( _SystemView.ViewName != CswEnumNbtSystemViewName.SIInspectionsbyUser.ToString() )
            {
                _NbtSystemView.reInitSystemView( CswEnumNbtSystemViewName.SIInspectionsbyUser );
            }
            return getInspectionsAndDesigns();
        }

        public CswNbtSdInspectionsDataModels.InspectionData byBarcode( string Barcode )
        {
            if( _SystemView.ViewName != CswEnumNbtSystemViewName.SIInspectionsbyBarcode.ToString() )
            {
                _NbtSystemView.reInitSystemView( CswEnumNbtSystemViewName.SIInspectionsbyBarcode );
            }
            _addSystemViewBarcodeFilter( Barcode, CswEnumNbtFilterMode.Begins, CswEnumNbtFieldType.Barcode );
            return getInspectionsAndDesigns();
        }

        public CswNbtSdInspectionsDataModels.InspectionData byLocation( string LocationName )
        {
            if( _SystemView.ViewName != CswEnumNbtSystemViewName.SIInspectionsbyLocation.ToString() )
            {
                _NbtSystemView.reInitSystemView( CswEnumNbtSystemViewName.SIInspectionsbyLocation );
            }
            _addSystemViewPropFilter( CswEnumNbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.PropertyName.Location, LocationName, CswEnumNbtFilterMode.Begins );
            return getInspectionsAndDesigns();
        }

        #endregion Get


        #region Set

        string Completed = CswEnumNbtInspectionStatus.Completed;
        string Cancelled = CswEnumNbtInspectionStatus.Cancelled;
        string CompletedLate = CswEnumNbtInspectionStatus.CompletedLate;
        string Missed = CswEnumNbtInspectionStatus.Missed;
        string ActionRequired = CswEnumNbtInspectionStatus.ActionRequired;

        private void _updateInspectionNode( CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection Inspection, CswNbtSdInspectionsDataModels.InspectionUpdateData UpdateCollection )
        {


            if( Int32.MinValue != Inspection.InspectionId )
            {
                CswPrimaryKey InspectionPk = new CswPrimaryKey( "nodes", Inspection.InspectionId );
                CswNbtNode InspectionNode = _CswNbtResources.Nodes.GetNode( InspectionPk, Inspection.DesignId );
                if( null != InspectionNode )
                {
                    CswNbtObjClassInspectionDesign NodeAsDesign = (CswNbtObjClassInspectionDesign) InspectionNode;
                    if( NodeAsDesign.Status.Value == Completed || NodeAsDesign.Status.Value == CompletedLate )
                    {
                        UpdateCollection.Completed.Add( Inspection );
                    }
                    else if( NodeAsDesign.Status.Value == Cancelled )
                    {
                        UpdateCollection.Cancelled.Add( Inspection );
                    }
                    else if( NodeAsDesign.Status.Value == Missed )
                    {
                        UpdateCollection.Missed.Add( Inspection );
                    }
                    else
                    {
                        Inspection.Counts = new CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection.QuestionCounts();
                        /* We loop once to set the property values */
                        CswNbtMetaDataNodeType InspectionNt = InspectionNode.getNodeType();
                        if( null != InspectionNt )
                        {
                            //Can edit the nodetype
                            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, InspectionNt ) )
                            {
                                foreach( CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection.QuestionAnswer Question in Inspection.Questions )
                                {
                                    CswNbtMetaDataNodeTypeProp Ntp = InspectionNt.getNodeTypeProp( Question.QuestionId );
                                    if( null != Ntp && null != Ntp.FirstEditLayout )
                                    {
                                        CswNbtMetaDataNodeTypeTab Tab = InspectionNt.getNodeTypeTab( Ntp.FirstEditLayout.TabId );
                                        if( null != Tab )
                                        {
                                            bool CanEdit = (
                                                                _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, InspectionNt, Tab ) ||
                                                                _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, Ntp, Tab )
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

                                    if( null != ButtonNtp && null != ButtonNtp.FirstEditLayout )
                                    {
                                        CswNbtMetaDataNodeTypeTab ButtonTab = _CswNbtResources.MetaData.getNodeTypeTab( ButtonNtp.FirstEditLayout.TabId );
                                        if( null != ButtonTab &&
                                                (
                                                    _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, InspectionNt, NodeTypeTab : ButtonTab ) ||
                                                    _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, ButtonNtp, ButtonTab )
                                                )
                                           )
                                        {
                                            _InspectionDesignOc = _InspectionDesignOc ?? _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
                                            CswNbtObjClass NbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, _InspectionDesignOc, InspectionNode );
                                            CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( ButtonNtp );
                                            NbtObjClass.triggerOnButtonClick( ButtonData );
                                        }
                                    }
                                }

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
                            foreach( CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection.QuestionAnswer Question in Inspection.Questions )
                            {
                                Question.Status = NodeAsDesign.Status.Value;
                            }
                            /* In case the Inspection has been modified by someone else */
                            Inspection.DueDateAsDate = NodeAsDesign.InspectionDate.DateTimeValue;
                            Inspection.InspectionPointName = NodeAsDesign.Target.CachedNodeName;
                            Inspection.LocationPath = NodeAsDesign.Location.CachedValue;
                            UpdateCollection.ActionRequired.Add( Inspection );
                        }
                        else
                        {
                            UpdateCollection.InComplete.Add( Inspection );
                        }
                    }
                }
            }


        }

        public CswNbtSdInspectionsDataModels.InspectionUpdateData update()
        {
            CswNbtSdInspectionsDataModels.InspectionUpdateData Ret = new CswNbtSdInspectionsDataModels.InspectionUpdateData();
            foreach( CswNbtSdInspectionsDataModels.InspectionData.CswNbtInspection Inspection in _Inspections )
            {
                _updateInspectionNode( Inspection, Ret );
            }
            return Ret;
        }


        #endregion Set


    } // public class CswNbtSdInspections

} // namespace ChemSW.Nbt.ServiceDrivers
