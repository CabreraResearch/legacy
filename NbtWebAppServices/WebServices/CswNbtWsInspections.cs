using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using NbtWebAppServices.Response;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWsInspections
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtSessionResources _CswNbtSessionResources = null;
        private CswNbtMetaDataObjectClass _InspectionDesignOc = null;
        private CswNbtView _SystemView;
        private CswNbtWebServiceResponseInspectionsAndDesign _Response;
        private CswNbtActSystemViews _NbtSystemView;

        private void _initInspectionResources( CswNbtActSystemViews.SystemViewName ViewName )
        {
            _CswNbtSessionResources = _Response.CswNbtSessionResources;
            _InspectionDesignOc = _CswNbtSessionResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            _NbtSystemView = new CswNbtActSystemViews( _CswNbtSessionResources.CswNbtResources,
                                                                           ViewName,
                                                                           _InspectionDesignOc
                );
            _SystemView = _NbtSystemView.SystemView;
        }

        [OperationContract]
        [WebGet( UriTemplate = "byDateRange?StartingDate={StartingDate}&EndingDate={EndingDate}" )]
        public CswNbtWebServiceResponseInspectionsAndDesign byDateRange( string StartingDate, string EndingDate )
        {
            _Response = new CswNbtWebServiceResponseInspectionsAndDesign( _Context );
            if( _Response.Status.Success )
            {
                try
                {
                    _initInspectionResources( CswNbtActSystemViews.SystemViewName.SIInspectionsbyDate );
                    CswNbtMetaDataObjectClassProp DueDateOcp = _InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );

                    DateTime Start = CswConvert.ToDateTime( StartingDate );
                    DateTime End = CswConvert.ToDateTime( EndingDate );
                    if( Start > End )
                    {
                        End = DateTime.Now;
                    }
                    CswDateTime CswStart = new CswDateTime( _CswNbtSessionResources.CswNbtResources, Start );
                    CswDateTime CswEnd = new CswDateTime( _CswNbtSessionResources.CswNbtResources, End );

                    _NbtSystemView.addSystemViewFilter( new CswNbtActSystemViews.SystemViewPropFilterDefinition
                                                             {
                                                                 FilterMode = CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals,
                                                                 FilterValue = CswStart.ToOracleNativeDateForQuery(),
                                                                 ObjectClassProp = DueDateOcp
                                                             } );
                    _NbtSystemView.addSystemViewFilter( new CswNbtActSystemViews.SystemViewPropFilterDefinition
                                                             {
                                                                 FilterMode = CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals,
                                                                 FilterValue = CswEnd.ToOracleNativeDateForQuery(),
                                                                 ObjectClassProp = DueDateOcp
                                                             } );
                    _makeInspectionReturn();
                }
                catch( Exception ex )
                {
                    _Response.addError( ex );
                }
            }
            _Response.finalizeResponse();
            return _Response; //_AddAuthenticationStatus( SessionAuthenticationStatus.Authenticated );
        }

        [OperationContract]
        [WebGet]
        public CswNbtWebServiceResponseInspectionsAndDesign byUser()
        {
            _Response = new CswNbtWebServiceResponseInspectionsAndDesign( _Context );
            if( _Response.Status.Success )
            {
                try
                {
                    _initInspectionResources( CswNbtActSystemViews.SystemViewName.SIInspectionsbyUser );

                    _makeInspectionReturn();
                }
                catch( Exception Ex )
                {
                    _Response.addError( Ex );
                }
            }
            _Response.finalizeResponse();
            return _Response;
        } // get()

        [OperationContract]
        [WebGet]
        public CswNbtWebServiceResponseInspectionsAndDesign byLocation( string LocationName )
        {
            _Response = new CswNbtWebServiceResponseInspectionsAndDesign( _Context );
            if( _Response.Status.Success )
            {
                try
                {
                    _initInspectionResources( CswNbtActSystemViews.SystemViewName.SIInspectionsbyLocation );

                    CswNbtMetaDataObjectClassProp LocationOcp = _InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.LocationPropertyName );
                    CswNbtActSystemViews.SystemViewPropFilterDefinition Filter = _NbtSystemView.makeSystemViewFilter( LocationOcp, LocationName, CswNbtPropFilterSql.PropertyFilterMode.Contains );
                    _NbtSystemView.addSystemViewFilter( Filter );
                    _makeInspectionReturn();
                }
                catch( Exception Ex )
                {
                    _Response.addError( Ex );
                }
            }
            _Response.finalizeResponse();
            return _Response;
        } // get()

        [OperationContract]
        [WebGet]
        public CswNbtWebServiceResponseInspectionsAndDesign byBarcode( string Barcode )
        {
            _Response = new CswNbtWebServiceResponseInspectionsAndDesign( _Context );
            if( _Response.Status.Success )
            {
                try
                {
                    _initInspectionResources( CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode );

                    CswNbtMetaDataObjectClass LocationOc = _CswNbtSessionResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                    CswNbtMetaDataObjectClassProp LocationBarcodeOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.BarcodePropertyName );
                    CswNbtActSystemViews.SystemViewPropFilterDefinition LocationFilter = _NbtSystemView.makeSystemViewFilter( LocationBarcodeOcp, Barcode, CswNbtPropFilterSql.PropertyFilterMode.Contains );
                    _NbtSystemView.addSystemViewFilter( LocationFilter, LocationOc );

                    CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtSessionResources.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                    CswNbtMetaDataObjectClassProp TargetBarcodeOcp = InspectionTargetOc.getObjectClassProp( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                    CswNbtActSystemViews.SystemViewPropFilterDefinition TargetFilter = _NbtSystemView.makeSystemViewFilter( TargetBarcodeOcp, Barcode, CswNbtPropFilterSql.PropertyFilterMode.Contains );
                    _NbtSystemView.addSystemViewFilter( TargetFilter, InspectionTargetOc );

                    _makeInspectionReturn();
                }
                catch( Exception Ex )
                {
                    _Response.addError( Ex );
                }
            }
            _Response.finalizeResponse();
            return _Response;
        } // get()

        [OperationContract]
        [WebInvoke(Method = "POST")]
        public CswNbtWebServiceResponseInspections updateInspection(CswNbtInspectionsResponseModel.CswNbtInspection Inspection )
        {
            CswNbtWebServiceResponseInspections Ret = new CswNbtWebServiceResponseInspections( _Context );
            if( Ret.Status.Success )
            {
                try
                {
                    _initInspectionResources( CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode );
                    
                }
                catch( Exception Ex )
                {
                    Ret.addError( Ex );
                }
            }
            Ret.finalizeResponse();
            return Ret;
        }
        
        private Collection<Int32> InspectionDesignTypeIds = new Collection<Int32>();
        private Collection<CswPrimaryKey> InspectionDesignNodeIds = new Collection<CswPrimaryKey>();

        private void _addNodeTypeInspectionDesignToResponse( CswNbtNode InspectionNode )
        {
            if( false == InspectionDesignTypeIds.Contains( InspectionNode.NodeTypeId ) )
            {
                CswNbtMetaDataNodeType NewInspectionNodeType = InspectionNode.getNodeType();
                InspectionDesignTypeIds.Add( NewInspectionNodeType.NodeTypeId );
                var ResponseDesign = new CswNbtInspectionsResponseModel.CswNbtInspectionDesign
                                         {
                                             DesignId = NewInspectionNodeType.NodeTypeId,
                                             Name = NewInspectionNodeType.NodeTypeName
                                         };

                foreach ( CswNbtMetaDataNodeTypeTab NodeTypeTab in NewInspectionNodeType.getNodeTypeTabs() )
                {
                    var ResponseSection = new CswNbtInspectionsResponseModel.CswNbtInspectionDesign.CswNbtInspectionDesignSection
                                              {
                                                  Name = NodeTypeTab.TabName,
                                                  Order = NodeTypeTab.TabOrder,
                                                  SectionId = NodeTypeTab.TabId
                                              };

                    foreach ( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeTab.getNodeTypePropsByDisplayOrder() )
                    {
                        var ResponseProperty = new CswNbtInspectionsResponseModel.CswNbtInspectionDesign.CswNbtInspectionDesignSectionProperty
                                                   {
                                                       HelpText = NodeTypeProp.HelpText,
                                                       Text = NodeTypeProp.PropName
                                                   };
                        CswNbtMetaDataFieldType.NbtFieldType FieldType = NodeTypeProp.getFieldType().FieldType;
                        ResponseProperty.Type = FieldType.ToString();

                        if ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                        {
                            ResponseProperty.QuestionId = NodeTypeProp.PropId;
                            CswCommaDelimitedString Answers = new CswCommaDelimitedString();
                            Answers.FromString( NodeTypeProp.ListOptions );
                            foreach ( string Answer in Answers )
                            {
                                ResponseProperty.Choices.Add( Answer );
                            }
                        }
                        ResponseSection.Properties.Add( ResponseProperty );
                    }
                    ResponseDesign.Sections.Add( ResponseSection );
                    _Response.Data.Designs.Add( ResponseDesign );
                }
            }
        }

        private void _addInspectionDesignNodeNodeToResponse( CswNbtNode InspectionNode )
        {
            if( false == InspectionDesignNodeIds.Contains( InspectionNode.NodeId ) )
            {
                InspectionDesignNodeIds.Add( InspectionNode.NodeId );
                CswNbtObjClassInspectionDesign NodeAsInspectionDesign = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
                var ResponseInspection = new CswNbtInspectionsResponseModel.CswNbtInspection
                                             {
                                                 DesignId = InspectionNode.NodeTypeId,
                                                 DueDate = NodeAsInspectionDesign.Date.DateTimeValue,
                                                 InspectionId = NodeAsInspectionDesign.NodeId.PrimaryKey,
                                                 InspectionPointName = NodeAsInspectionDesign.Target.CachedNodeName,
                                                 LocationPath = NodeAsInspectionDesign.Location.Gestalt,
                                                 RouteName = default( string ),
                                                 Status = NodeAsInspectionDesign.Status.Value
                                             };

                foreach ( CswNbtNodePropWrapper Prop in InspectionNode.Properties )
                {
                    if ( Prop.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question )
                    {
                        CswNbtNodePropQuestion PropAsQuestion = Prop.AsQuestion;
                        var ResponseQuestion = new CswNbtInspectionsResponseModel.CswNbtInspection.CswNbtInspectionQuestion
                                                   {
                                                       Answer = PropAsQuestion.Answer,
                                                       AnswerId = PropAsQuestion.NodeTypePropId,
                                                       Comments = PropAsQuestion.Comments,
                                                       CorrectiveAction = PropAsQuestion.CorrectiveAction,
                                                       LastModifyDate = PropAsQuestion.LastEditDate,
                                                       QuestionId = PropAsQuestion.NodeTypePropId,
                                                       Status = NodeAsInspectionDesign.Status.Value,
                                                       LastModifyUserName = PropAsQuestion.LastEditUser
                                                   };

                        ResponseInspection.Questions.Add( ResponseQuestion );
                    }
                }
                _Response.Data.Inspections.Add( ResponseInspection );
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
                ICswNbtTree Tree = _CswNbtSessionResources.CswNbtResources.Trees.getTreeFromView( _SystemView, true, false );
                _iterateTree( Tree );
            }
        }
    }
}