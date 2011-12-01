﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceInspectionDesign
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _CurrentUser;
        private readonly CswNbtObjClassRole _CurrentRole;
        private readonly TextInfo _TextInfo;
        public CswNbtWebServiceInspectionDesign( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CurrentRole = _CswNbtResources.CurrentNbtUser.RoleNode;

            if( _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName )
            {
                throw new CswDniException( ErrorType.Error, "Only the ChemSW Admin role can access the Inspection Design wizard.", "Attempted to access the Inspection Design wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            _CurrentUser = _CswNbtResources.CurrentNbtUser;
            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }

        #endregion ctor

        #region Private

        private static readonly string _SectionName = "SECTION";
        private static readonly string _QuestionName = "QUESTION";
        private static readonly string _AllowedAnswersName = "ALLOWED_ANSWERS";
        private static readonly string _CompliantAnswersName = "COMPLIANT_ANSWERS";
        private static readonly string _HelpTextName = "HELP_TEXT";

        private readonly CswCommaDelimitedString _ColumnNames = new CswCommaDelimitedString()
                                                           {
                                                               _SectionName,
                                                               _QuestionName,
                                                               _AllowedAnswersName,
                                                               _CompliantAnswersName,
                                                               _HelpTextName
                                                           };


        private readonly string _DefaultSectionName = "Questions";
        private readonly string _DefaultAllowedAnswers = "Yes,No,N/A";
        private readonly string _DefaultCompliantAnswers = "Yes";

        #region MetaData

        private string _checkUniqueNodeType( string NodeTypeName )
        {
            string Ret = string.Empty;
            if( wsTools.isNodeTypeNameUnique( NodeTypeName, _CswNbtResources ) )
            {
                Ret = _TextInfo.ToTitleCase( NodeTypeName.Trim() );
            }
            return Ret;
        }

        private void _validateNodeType( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            if( null == NodeType )
            {
                throw new CswDniException( ErrorType.Warning, "The expected object was not defined", "NodeType for ObjectClass " + ObjectClass + " was null." );
            }
            if( ObjectClass != NodeType.ObjectClass.ObjectClass )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot use a " + NodeType.NodeTypeName + " as an " + ObjectClass, "Attempted to use a NodeType of an unexpected ObjectClass" );
            }
        }

        private void _setNodeTypePermissions( CswNbtMetaDataNodeType NodeType )
        {
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Create, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Edit, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Delete, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.View, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Create, NodeType, _CurrentRole, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Edit, NodeType, _CurrentRole, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.Delete, NodeType, _CurrentRole, true );
            _CswNbtResources.Permit.set( CswNbtPermit.NodeTypePermission.View, NodeType, _CurrentRole, true );
        }

        private Dictionary<string, CswNbtMetaDataNodeTypeTab> _getTabsForInspection( JArray Grid, CswNbtMetaDataNodeType NodeType )
        {
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = _TextInfo.ToTitleCase( CswConvert.ToString( ThisRow[_SectionName] ) );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = "Section 1";
                    }
                    if( false == RetDict.ContainsKey( TabName ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NodeType.getNodeTypeTab( TabName );
                        if( null == ThisTab )
                        {
                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, NodeType.NodeTypeTabs.Count );
                        }
                        RetDict.Add( TabName, ThisTab );
                    }
                }
            }
            return RetDict;
        }

        private Int32 _createInspectionProps( JArray Grid, CswNbtMetaDataNodeType InspectionDesignNt,
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs, CswCommaDelimitedString GridRowsSkipped )
        {
            Int32 RetCount = 0;
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = CswConvert.ToString( ThisRow[_SectionName] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = _DefaultSectionName;
                    }
                    string Question = _TextInfo.ToTitleCase( CswConvert.ToString( ThisRow[_QuestionName] ) );
                    string AllowedAnswers = CswConvert.ToString( ThisRow[_AllowedAnswersName] );
                    string CompliantAnswers = CswConvert.ToString( ThisRow[_CompliantAnswersName] );
                    string HelpText = CswConvert.ToString( ThisRow[_HelpTextName] );

                    if( false == string.IsNullOrEmpty( Question ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab;
                        Tabs.TryGetValue( TabName, out ThisTab );
                        Int32 ThisTabId = Int32.MinValue;
                        if( null != ThisTab )
                        {
                            ThisTabId = ThisTab.TabId;
                        }
                        CswNbtMetaDataNodeTypeProp ThisQuestion = _CswNbtResources.MetaData.makeNewProp( InspectionDesignNt, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, ThisTabId );

                        if( null == ThisQuestion )
                        {
                            GridRowsSkipped.Add( Index.ToString() );
                        }
                        else
                        {
                            _validateAnswers( ref CompliantAnswers, ref AllowedAnswers );
                            if( false == string.IsNullOrEmpty( HelpText ) )
                            {
                                ThisQuestion.HelpText = HelpText;
                            }
                            ThisQuestion.ValueOptions = CompliantAnswers;
                            ThisQuestion.ListOptions = AllowedAnswers;

                            RetCount += 1;
                        }
                    }
                    else
                    {
                        GridRowsSkipped.Add( Index.ToString() );
                    }
                }
            }
            return RetCount;
        }

        private JObject _createInspectionDesignRelationships( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt, string InspectionTargetName = null, string Category = null )
        {
            JObject RetObj = new JObject();
            //This is a New Target
            if( null == InspectionTargetNt )
            {
                if( string.IsNullOrEmpty( InspectionTargetName ) )
                {
                    throw new CswDniException( ErrorType.Warning, "Cannot create an Inspection Target without a name.", "InspectionTargetName was null or empty." );
                }
                RetObj = _createInspectionCollection( InspectionTargetName, Category, InspectionDesignNt );
            }
            else //Target and therefore (presumably) Target Group and (presumably) Generator exist--we'll validate and throw if not
            {
                _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                string FkType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                Int32 FkValue = InspectionTargetNt.NodeTypeId;
                CswNbtMetaDataNodeTypeProp TargetProperty = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                TargetProperty.SetFK( FkType, FkValue );

                RetObj["views"] = new JObject();

                //Inspections Due Today view
                CswNbtView InspectionsDueTodayView = _createInspectionsView( InspectionDesignNt, Category, NbtViewRenderingMode.Tree, NbtViewVisibility.Role, false, DateTime.Today, InspectionDesignNt.NodeTypeName + " Due Today" );
                RetObj["views"][InspectionsDueTodayView.ViewName] = new JObject();
                RetObj["views"][InspectionsDueTodayView.ViewName]["viewname"] = InspectionsDueTodayView.ViewName;
                RetObj["views"][InspectionsDueTodayView.ViewName]["viewid"] = InspectionsDueTodayView.ViewId.get();

                //All Inspections view
                CswNbtView AllInspectionsView = _createInspectionsView( InspectionDesignNt, Category, NbtViewRenderingMode.Grid, NbtViewVisibility.Role, false, DateTime.MinValue, "All " + InspectionDesignNt.NodeTypeName );
                RetObj["views"][AllInspectionsView.ViewName] = new JObject();
                RetObj["views"][AllInspectionsView.ViewName]["viewname"] = AllInspectionsView.ViewName;
                RetObj["views"][AllInspectionsView.ViewName]["viewid"] = AllInspectionsView.ViewId.get();
            }
            return RetObj;
        }

        private JObject _createInspectionCollection( string InspectionTargetName, string Category, CswNbtMetaDataNodeType InspectionDesignNt )
        {
            JObject RetObj = new JObject();
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create Inspection Target without a name.", "InspectionTargetName was null or empty." );
            }
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            //This will validate names and throw if not unique.
            InspectionTargetName = _checkUniqueNodeType( InspectionTargetName.Trim() );
            string InspectionGroupName = _checkUniqueNodeType( InspectionTargetName + " Group" );
            string InspectionDesignName = InspectionDesignNt.NodeTypeName;
            string InspectionScheduleName = _checkUniqueNodeType( InspectionTargetName + " Schedule" );
            string InspectionRouteName = _checkUniqueNodeType( InspectionTargetName + " Route" );

            //if we're here, we're validated
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            //Create the new NodeTypes
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetOc.ObjectClassId, InspectionTargetName, Category );
            _setNodeTypePermissions( InspectionTargetNt );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetGroupOc.ObjectClassId, InspectionGroupName, Category );
            _setNodeTypePermissions( InspectionTargetGroupNt );
            CswNbtMetaDataNodeType InspectionRouteNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionRouteOc.ObjectClassId, InspectionRouteName, Category );
            _setNodeTypePermissions( InspectionRouteNt );
            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.makeNewNodeType( GeneratorOc.ObjectClassId, InspectionScheduleName, Category );
            _setNodeTypePermissions( GeneratorNt );

            #region Set new InspectionTarget Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItDescriptionNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            InspectionTargetNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( InspectionTargetNt.BarcodeProperty.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName );

            //Inspection Target has Inspection Target Group Relationship
            CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            ItInspectionGroupNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
            ItInspectionGroupNtp.PropName = InspectionGroupName;

            //Inspection Target has Route relationship
            CswNbtMetaDataNodeTypeProp ItRouteNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Route", InspectionTargetNt.getFirstNodeTypeTab().TabId );
            ItRouteNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionRouteNt.NodeTypeId );
            ItRouteNtp.PropName = InspectionRouteName;

            //Inspection Target has a tab to host a grid view of Inspections
            CswNbtMetaDataNodeTypeTab ItInspectionsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetNt, InspectionDesignName, 2 );
            CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionDesignName, ItInspectionsTab.TabId );
            CswNbtView ItInspectionsGridView = _createInspectionsView( InspectionDesignNt, string.Empty, NbtViewRenderingMode.Grid, NbtViewVisibility.Property, true, DateTime.MinValue, InspectionTargetName + " Grid Prop View" );
            ItInspectionsNtp.ViewId = ItInspectionsGridView.ViewId;

            #endregion Set new InspectionTarget Props and Tabs

            #region Set InspectionTargetGroup Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItgNameNtp = InspectionTargetGroupNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTargetGroup.NamePropertyName );
            InspectionTargetGroupNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName );

            //Description is useful.
            _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Text, "Description", InspectionTargetGroupNt.getFirstNodeTypeTab().TabId );

            //Inspection Target Group has a tab to host a grid view of Inspection Targets
            CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 2 );
            CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName + " Locations", ItgLocationsTab.TabId );
            CswNbtView ItgInspectionPointsGridView = _createAllInspectionPointsView( InspectionTargetNt, string.Empty, NbtViewRenderingMode.Grid, NbtViewVisibility.Property, InspectionTargetName + " Grid Prop View" );
            ItgLocationsNtp.ViewId = ItgInspectionPointsGridView.ViewId;

            #endregion Set InspectionTargetGroup Props and Tabs

            #region Set InspectionDesign Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
            InspectionDesignNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( IdNameNtp.PropName );

            //Inspection Design Target is Inspection Target
            CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            IdTargetNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.NodeTypeId );

            //Inspection Design Generator is new Inspection Schedule
            CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            IdGeneratorNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );
            IdGeneratorNtp.PropName = "Schedule";

            CswNbtMetaDataNodeTypeTab IdDetailTab = InspectionDesignNt.getNodeTypeTab( "Details" );
            if( null == IdDetailTab )
            {
                IdDetailTab = _CswNbtResources.MetaData.makeNewTab( InspectionDesignNt, "Details", InspectionDesignNt.NodeTypeTabs.Count + 1 );
            }
            //Barcode property reference is useful
            CswNbtMetaDataNodeTypeProp IdBarcodeNtp = _CswNbtResources.MetaData.makeNewProp( InspectionDesignNt, CswNbtMetaDataFieldType.NbtFieldType.PropertyReference, "Barcode", IdDetailTab.TabId );
            IdBarcodeNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.BarcodeProperty.PropId, CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.BarcodeProperty.PropId );
            IdBarcodeNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.BarcodeProperty.PropId, CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.BarcodeProperty.PropId );

            //Location property reference is useful
            CswNbtMetaDataNodeTypeProp IdLocationNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp ItLocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
            IdLocationNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), ItLocationNtp.PropId, CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), ItLocationNtp.PropId );
            IdLocationNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), ItLocationNtp.PropId, CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), ItLocationNtp.PropId );

            #endregion Set InspectionDesign Props and Tabs

            #region Set InspectionRoute Props and Tabs

            //Route name
            _CswNbtResources.MetaData.makeNewProp( InspectionRouteNt, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", InspectionRouteNt.getFirstNodeTypeTab().TabId );
            InspectionRouteNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( "Name" );

            //InspectionRoute has a relationship to a user
            CswNbtMetaDataNodeTypeProp IrInspectorNtp = _CswNbtResources.MetaData.makeNewProp( InspectionRouteNt, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Inspector", InspectionRouteNt.getFirstNodeTypeTab().TabId );
            IrInspectorNtp.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ).ObjectClassId );

            //Route has a grid view of Inspection Targets
            CswNbtMetaDataNodeTypeTab IrTargetTab = _CswNbtResources.MetaData.makeNewTab( InspectionRouteNt, InspectionTargetName, 2 );
            CswNbtMetaDataNodeTypeProp IrTargetNtp = _CswNbtResources.MetaData.makeNewProp( InspectionRouteNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName, IrTargetTab.TabId );
            CswNbtView RouteGridView = new CswNbtView( _CswNbtResources );
            RouteGridView.makeNew( InspectionTargetName + " Route Grid View", NbtViewVisibility.Property, null, null, ItInspectionsGridView );
            IrTargetNtp.ViewId = RouteGridView.ViewId;

            #endregion Set InspectionRoute Props and Tabs

            #region Set Generator Props

            CswNbtMetaDataNodeTypeTab GnSettingsTab = _CswNbtResources.MetaData.makeNewTab( GeneratorNt, "Settings", GeneratorNt.NodeTypeTabs.Count + 1 );

            //Set generator's owner to new Inspection Target Group
            CswNbtMetaDataNodeTypeProp GnOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
            GnOwnerNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
            GnOwnerNtp.PropName = InspectionGroupName;
            GnOwnerNtp.EditLayout.Tab = GnSettingsTab;

            //Set generator's parent to new Inspection Target
            CswNbtMetaDataNodeTypeProp GnParentTypeNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            CswCommaDelimitedString TargetNodeTypeIds = new CswCommaDelimitedString() { InspectionTargetNt.NodeTypeId.ToString() };
            GnParentTypeNtp.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds = TargetNodeTypeIds;
            GnParentTypeNtp.ReadOnly = true;
            GnParentTypeNtp.IsRequired = true;
            GnParentTypeNtp.PropName = "Inspection Parent";
            GnParentTypeNtp.EditLayout.Tab = GnSettingsTab;

            //Set generator's target to new Inspection Design
            CswNbtMetaDataNodeTypeProp GnTargetTypeNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            CswCommaDelimitedString DesignNodeTypeIds = new CswCommaDelimitedString() { InspectionDesignNt.NodeTypeId.ToString() };
            GnTargetTypeNtp.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds = DesignNodeTypeIds;
            GnTargetTypeNtp.IsRequired = true;
            GnTargetTypeNtp.ReadOnly = true;
            GnTargetTypeNtp.PropName = "Inspection Type";
            GnTargetTypeNtp.EditLayout.Tab = GnSettingsTab;

            //Set generator's parent view: Schedule -> Inspection Target Group -> Inspection Target
            CswNbtMetaDataNodeTypeProp GnParentViewNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
            CswNbtView GeneratorView = _CswNbtResources.ViewSelect.restoreView( GnParentViewNtp.DefaultValue.AsViewReference.ViewId );
            _createInspectionGeneratorView( GeneratorView, InspectionDesignNt, InspectionTargetNt, GeneratorNt );
            GnParentViewNtp.ViewId = GeneratorView.ViewId;
            GnParentViewNtp.ReadOnly = true;
            GnParentViewNtp.EditLayout.Tab = GnSettingsTab;

            CswNbtMetaDataNodeTypeProp GnSummaryNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.SummaryPropertyName );
            GeneratorNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( GnOwnerNtp.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( GnSummaryNtp.PropName );

            #endregion Set Generator Props

            #region Views

            RetObj["views"] = new JObject();
            //Inspection Schedules view
            CswNbtView InspectionSchedulesView = _createInspectionSchedulesView( InspectionDesignNt, Category, InspectionTargetName );
            RetObj["views"][InspectionSchedulesView.ViewName] = new JObject();
            RetObj["views"][InspectionSchedulesView.ViewName]["viewname"] = InspectionSchedulesView.ViewName;
            RetObj["views"][InspectionSchedulesView.ViewName]["viewid"] = InspectionSchedulesView.ViewId.get();

            //Inspections Due Today view
            CswNbtView InspectionsDueTodayView = _createInspectionsView( InspectionDesignNt, Category, NbtViewRenderingMode.Tree, NbtViewVisibility.Role, false, DateTime.Today, InspectionDesignNt.NodeTypeName + " Due Today" );
            RetObj["views"][InspectionsDueTodayView.ViewName] = new JObject();
            RetObj["views"][InspectionsDueTodayView.ViewName]["viewname"] = InspectionsDueTodayView.ViewName;
            RetObj["views"][InspectionsDueTodayView.ViewName]["viewid"] = InspectionsDueTodayView.ViewId.get();

            //All Inspection Points view
            CswNbtView AllInspectionPointsView = _createAllInspectionPointsView( InspectionTargetNt, Category, NbtViewRenderingMode.Tree, NbtViewVisibility.Role, "All " + InspectionTargetNt.NodeTypeName );
            RetObj["views"][AllInspectionPointsView.ViewName] = new JObject();
            RetObj["views"][AllInspectionPointsView.ViewName]["viewname"] = AllInspectionPointsView.ViewName;
            RetObj["views"][AllInspectionPointsView.ViewName]["viewid"] = AllInspectionPointsView.ViewId.get();

            //All Inspections view
            CswNbtView AllInspectionsView = _createInspectionsView( InspectionDesignNt, Category, NbtViewRenderingMode.Grid, NbtViewVisibility.Role, false, DateTime.MinValue, "All " + InspectionDesignNt.NodeTypeName );
            RetObj["views"][AllInspectionsView.ViewName] = new JObject();
            RetObj["views"][AllInspectionsView.ViewName]["viewname"] = AllInspectionsView.ViewName;
            RetObj["views"][AllInspectionsView.ViewName]["viewid"] = AllInspectionsView.ViewId.get();

            #endregion

            return RetObj;
        }

        private void _pruneSectionOneTab( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataNodeTypeTab SectionOneTab = InspectionDesignNt.getNodeTypeTab( "Section 1" );
            if( null != SectionOneTab )
            {
                if( SectionOneTab.NodeTypeProps.Count > 0 )
                {
                    SectionOneTab.TabName = "Questions";
                }
                else
                {
                    _CswNbtResources.MetaData.DeleteNodeTypeTab( SectionOneTab );
                }
            }
        }

        #endregion MetaData

        #region Views

        private CswNbtView _getSchedulesViewFromInspectionDesign( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            string InspectionGroupName = string.Empty;
            return _getSchedulesViewFromInspectionDesign( InspectionDesignNt, ref InspectionGroupName );
        }

        private CswNbtView _getSchedulesViewFromInspectionDesign( CswNbtMetaDataNodeType InspectionDesignNt, ref string InspectionGroupName )
        {
            CswNbtView RetView = null;

            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            CswNbtMetaDataNodeTypeProp GeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            if( GeneratorNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( GeneratorNtp.FKValue );
                _validateNodeType( GeneratorNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

                CswNbtMetaDataNodeTypeProp GenOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                if( GenOwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                {
                    CswNbtMetaDataNodeType InspectionGroupNt = _CswNbtResources.MetaData.getNodeType( GenOwnerNtp.FKValue );
                    _validateNodeType( InspectionGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

                    InspectionGroupName = InspectionGroupNt.NodeTypeName;

                    RetView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, GenOwnerNtp, false );

                }

            }
            else if( GeneratorNtp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create a Schedule view", "Cannot use Object Class relationships to construct a view." );
            }
            return RetView;
        }

        private CswNbtView _getSchedulesViewFromInspectionTarget( CswNbtMetaDataNodeType InspectionTargetNt, ref string InspectionGroupName )
        {
            CswNbtView RetView = null;
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );

            CswNbtMetaDataNodeTypeProp InTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            if( InTargetGroupNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType InTargetGroupNt = _CswNbtResources.MetaData.getNodeType( InTargetGroupNtp.FKValue );
                _validateNodeType( InTargetGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

                InspectionGroupName = InTargetGroupNt.NodeTypeName;

                RetView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InTargetGroupNt, false );

                //Multiple Generators could target this Target
                CswNbtMetaDataObjectClass GeneratorOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOc.NodeTypes )
                {
                    CswNbtMetaDataNodeTypeProp OwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                    if( OwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                        OwnerNtp.FKValue == InTargetGroupNt.NodeTypeId )
                    {
                        RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, OwnerNtp, false );
                    }
                }
            }
            else if( InTargetGroupNtp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create a Schedule view", "Cannot use Object Class relationships to construct a view." );
            }
            return RetView;
        }

        private CswNbtView _createInspectionSchedulesView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, string InspectionTargetName )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtView RetView = new CswNbtView( _CswNbtResources );
            string InspectionSchedulesViewName = InspectionTargetName + " Schedule";

            try
            {
                CswNbtView ScheduleView = _getSchedulesViewFromInspectionDesign( InspectionDesignNt );
                RetView.makeNew( InspectionSchedulesViewName, NbtViewVisibility.Role, _CurrentUser.RoleId, null, ScheduleView );
                RetView.ViewMode = NbtViewRenderingMode.Tree;
                RetView.Category = Category;
                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Failed to create view: " + InspectionSchedulesViewName, "View creation failed", ex );
            }
            return RetView;
        }

        private CswNbtView _createInspectionsView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, NbtViewRenderingMode ViewMode,
            NbtViewVisibility Visibility, bool AllInspections, DateTime DueDate, string InspectionsViewName )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            if( string.IsNullOrEmpty( InspectionsViewName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create an Inspections view without a name.", "View name was null or empty." );
            }

            CswNbtView RetView = new CswNbtView( _CswNbtResources );
            try
            {
                CswPrimaryKey RoleId = null;
                if( NbtViewVisibility.Role == Visibility )
                {
                    RoleId = _CurrentUser.RoleId;
                }

                RetView.makeNew( InspectionsViewName, Visibility, RoleId, null, null );
                RetView.ViewMode = ViewMode;
                RetView.Category = Category;

                CswNbtViewRelationship InspectionVr = RetView.AddViewRelationship( InspectionDesignNt, false );
                CswNbtMetaDataNodeTypeProp DueDateNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                CswNbtViewProperty DueDateVp = RetView.AddViewProperty( InspectionVr, DueDateNtp );

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty StatusVp = RetView.AddViewProperty( InspectionVr, StatusNtp );

                if( false == AllInspections )
                {
                    RetView.AddViewPropertyFilter( StatusVp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );
                    RetView.AddViewPropertyFilter( StatusVp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                    RetView.AddViewPropertyFilter( StatusVp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                    RetView.AddViewPropertyFilter( StatusVp, StatusNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );
                }

                if( DateTime.MinValue != DueDate )
                {
                    RetView.AddViewPropertyFilter( DueDateVp, DueDateNtp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, DateTime.Today.ToString(), false );
                }

                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Failed to create view: " + InspectionsViewName, "View creation failed", ex );
            }
            return RetView;
        }

        private CswNbtView _createAllInspectionPointsView( CswNbtMetaDataNodeType InspectionTargetNt, string Category, NbtViewRenderingMode ViewMode, NbtViewVisibility Visibility, string AllInspectionPointsViewName )
        {
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtView RetView = new CswNbtView( _CswNbtResources );

            try
            {
                CswPrimaryKey RoleId = null;
                if( NbtViewVisibility.Role == Visibility )
                {
                    RoleId = _CurrentUser.RoleId;
                }

                RetView.makeNew( AllInspectionPointsViewName, Visibility, RoleId, null, null );
                RetView.Category = Category;
                RetView.ViewMode = ViewMode;

                CswNbtViewRelationship InspectionTargetVr = RetView.AddViewRelationship( InspectionTargetNt, false );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = InspectionTargetNt.BarcodeProperty;
                RetView.AddViewProperty( InspectionTargetVr, BarcodeNtp ).Order = 0;

                CswNbtMetaDataNodeTypeProp DescriptionNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                RetView.AddViewProperty( InspectionTargetVr, DescriptionNtp ).Order = 1;

                CswNbtMetaDataNodeTypeProp LocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                RetView.AddViewProperty( InspectionTargetVr, LocationNtp ).Order = 2;

                CswNbtMetaDataNodeTypeProp DateNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                RetView.AddViewProperty( InspectionTargetVr, DateNtp ).Order = 3;

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
                RetView.AddViewProperty( InspectionTargetVr, StatusNtp ).Order = 4;

                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Failed to create view: " + AllInspectionPointsViewName, "View creation failed", ex );
            }
            return RetView;
        }

        private CswNbtView _createInspectionGeneratorView( CswNbtView ViewReference, CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionScheduleNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            _validateNodeType( InspectionScheduleNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            string InspectionsViewName = InspectionDesignNt.NodeTypeName + " Due Today";

            try
            {
                ViewReference.makeNew( InspectionsViewName, NbtViewVisibility.Property, null, null, null );
                ViewReference.ViewMode = NbtViewRenderingMode.Tree;

                // Schedule -> Inspection Target Group -> Inspection Target
                CswNbtViewRelationship InspectionVr = ViewReference.AddViewRelationship( InspectionScheduleNt, false );
                CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionScheduleNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                CswNbtViewRelationship InspectionPointGroupVr = ViewReference.AddViewRelationship( InspectionVr, CswNbtViewRelationship.PropOwnerType.First, OwnerNtp, false );
                CswNbtMetaDataNodeTypeProp TargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                ViewReference.AddViewRelationship( InspectionPointGroupVr, CswNbtViewRelationship.PropOwnerType.Second, TargetGroupNtp, false );

                ViewReference.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Failed to create view: " + InspectionsViewName, "View creation failed", ex );
            }
            return ViewReference;
        }

        #endregion Views

        /// <summary>
        /// Ensure that Allowed Answers contains all Compliant Answers and that both collections contain only unique answers.
        /// </summary>
        private void _validateAnswers( ref string CompliantAnswersString, ref string AllowedAnswersString )
        {
            string RetCompliantAnswersString = _DefaultCompliantAnswers;
            string RetAllowedAnswersString = _DefaultAllowedAnswers;

            CswCommaDelimitedString AllowedAnswers = new CswCommaDelimitedString();
            AllowedAnswers.FromString( AllowedAnswersString );

            CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
            CompliantAnswers.FromString( CompliantAnswersString );

            if( AllowedAnswers.Count > 0 ||
                    CompliantAnswers.Count > 0 )
            {
                Dictionary<string, string> UniqueCompliantAnswers = new Dictionary<string, string>();
                //Get the unique answers from each collection
                foreach( string CompliantAnswer in CompliantAnswers )
                {
                    string ThisAnswer = CompliantAnswer.ToLower().Trim();
                    if( false == UniqueCompliantAnswers.ContainsKey( ThisAnswer ) )
                    {
                        UniqueCompliantAnswers.Add( ThisAnswer, CompliantAnswer );
                    }
                }
                Dictionary<string, string> UniqueAllowedAnswers = new Dictionary<string, string>();
                foreach( string AllowedAnswer in AllowedAnswers )
                {
                    string ThisAnswer = AllowedAnswer.ToLower().Trim();
                    if( false == UniqueAllowedAnswers.ContainsKey( ThisAnswer ) )
                    {
                        UniqueAllowedAnswers.Add( ThisAnswer, AllowedAnswer );
                    }
                }

                //Allowed answers must contain all compliant answers
                CswCommaDelimitedString RetCompliantAnswers = new CswCommaDelimitedString();
                foreach( KeyValuePair<string, string> UniqueCompliantAnswer in UniqueCompliantAnswers )
                {
                    RetCompliantAnswers.Add( UniqueCompliantAnswer.Value );
                    if( false == UniqueAllowedAnswers.ContainsKey( UniqueCompliantAnswer.Key ) )
                    {
                        UniqueAllowedAnswers.Add( UniqueCompliantAnswer.Key, UniqueCompliantAnswer.Value );
                    }
                }

                //Get unique allowed answers
                CswCommaDelimitedString RetAllowedAnswers = new CswCommaDelimitedString();
                foreach( KeyValuePair<string, string> UniqueAllowedAnswer in UniqueAllowedAnswers )
                {
                    RetAllowedAnswers.Add( UniqueAllowedAnswer.Value );
                }

                if( CompliantAnswers.Count > 0 )
                {
                    RetCompliantAnswersString = RetCompliantAnswers.ToString();
                }
                else
                {
                    RetCompliantAnswersString = RetAllowedAnswers.ToString();
                }

                RetAllowedAnswersString = RetAllowedAnswers.ToString();
            }
            CompliantAnswersString = RetCompliantAnswersString;
            AllowedAnswersString = RetAllowedAnswersString;
        }

        #endregion Private

        #region Public

        /// <summary>
        /// Reads an Excel file from the file system and converts it into a ADO.NET data table
        /// </summary>
        /// <param name="FullPathAndFileName"></param>
        /// <returns></returns>
        public DataTable convertExcelFileToDataTable( string FullPathAndFileName, ref string ErrorMessage, ref string WarningMessage )
        {
            DataTable ExcelDataTable = null;
            OleDbConnection ExcelConn = null;

            try
            {
                // Microsoft JET engine knows how to read Excel files as a database
                // Problem is - it is old OLE technology - not newer ADO.NET
                string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FullPathAndFileName + ";Extended Properties=Excel 8.0;";
                ExcelConn = new OleDbConnection( ConnStr );
                ExcelConn.Open();

                DataTable ExcelSchemaDT = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                string FirstSheetName = ExcelSchemaDT.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                ExcelDataTable = new DataTable();
                DataAdapter.Fill( ExcelDataTable );

                foreach( DataColumn Column in ExcelDataTable.Columns )
                {
                    Column.ColumnName = Column.ColumnName.ToUpper().Replace( " ", "_" );
                    if( false == _ColumnNames.Contains( Column.ColumnName ) )
                    {
                        ExcelDataTable.Columns.Remove( Column );
                    }
                }

                foreach( string ColumnName in _ColumnNames )
                {
                    if( ExcelDataTable.Columns[ColumnName] == null )
                    {
                        ExcelDataTable.Columns.Add( ColumnName );
                    }
                }

                if( ExcelDataTable.Columns["RowNumber"] == null )
                {
                    ExcelDataTable.Columns.Add( "RowNumber" );
                }

                Int32 RowNumber = 0;
                foreach( DataRow Row in ExcelDataTable.Rows )
                {
                    if( string.Empty == CswConvert.ToString( Row[_QuestionName] ) )
                    {
                        ExcelDataTable.Rows.Remove( Row );
                    }
                    else
                    {
                        string AllowedAnswers = CswConvert.ToString( Row[_AllowedAnswersName] );
                        string ComplaintAnswers = CswConvert.ToString( Row[_CompliantAnswersName] );
                        _validateAnswers( ref ComplaintAnswers, ref AllowedAnswers );

                        if( string.Empty == CswConvert.ToString( Row[_SectionName] ) ||
                            "Section 1" == CswConvert.ToString( Row[_SectionName] ) )
                        {
                            Row[_SectionName] = _DefaultSectionName;
                        }
                        Row["RowNumber"] = RowNumber;
                        RowNumber += 1;
                    }
                }
            } // try
            catch( Exception Exception )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }
            finally
            {
                if( ExcelConn != null )
                {
                    ExcelConn.Close();
                    ExcelConn.Dispose();
                }
            }
            return ExcelDataTable;
        } // convertExcelFileToDataTable()

        public JObject copyInspectionDesign( string CopyFromInspectionDesign, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();

            CswNbtMetaDataNodeType CopyInspectionDesignNt = _CswNbtResources.MetaData.getNodeType( CopyFromInspectionDesign );

            _validateNodeType( CopyInspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            InspectionDesignName = _checkUniqueNodeType( InspectionDesignName );
            CswNbtMetaDataNodeType NewInspectionDesignNt = _CswNbtResources.MetaData.CopyNodeType( CopyInspectionDesignNt, InspectionDesignName );
            if( false == string.IsNullOrEmpty( Category ) )
            {
                Category = _TextInfo.ToTitleCase( Category.Trim() );
                NewInspectionDesignNt.Category = Category;
            }
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            _createInspectionDesignRelationships( NewInspectionDesignNt, InspectionTargetNt, InspectionTargetName, Category );

            return RetObj;
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();

            Int32 PropsWithoutError = 0;
            Int32 TotalRows = 0;
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();
            string CategoryName = Category;
            InspectionDesignName = _checkUniqueNodeType( InspectionDesignName );

            JArray GridArray = JArray.Parse( GridArrayString );

            if( null == GridArray || GridArray.Count == 0 )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create Inspection Design " + InspectionDesignName + ", because the import contained no questions.", "GridArray was null or empty." );
            }

            TotalRows = GridArray.Count;

            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( string.IsNullOrEmpty( CategoryName ) )
            {
                if( null != InspectionTargetNt )
                {
                    CategoryName = InspectionTargetNt.Category;
                }
                else
                {
                    CategoryName = InspectionDesignName;
                }
            }
            CategoryName = _TextInfo.ToTitleCase( CategoryName.Trim() );

            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass.ToString(), InspectionDesignName, CategoryName );
            _setNodeTypePermissions( InspectionDesignNt );

            //Get distinct tabs
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = _getTabsForInspection( GridArray, InspectionDesignNt );
            //Create the props
            PropsWithoutError = _createInspectionProps( GridArray, InspectionDesignNt, Tabs, GridRowsSkipped );
            //Delete or rename the "Section 1" tab
            _pruneSectionOneTab( InspectionDesignNt );
            //Build the MetaData
            RetObj = _createInspectionDesignRelationships( InspectionDesignNt, InspectionTargetNt, InspectionTargetName, CategoryName );
            //More return data
            RetObj["totalrows"] = TotalRows.ToString();
            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            return RetObj;
        }

        public JObject getScheduleNodesForInspection( string InspectionTargetName, string CopyInspectionDesignName )
        {
            JObject RetObj = new JObject();
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( CopyInspectionDesignName );
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( null == InspectionDesignNt && null == InspectionTargetNt )
            {
                //This is a new InspectionDesign on a new InspectionTarget
                RetObj["groupcount"] = "0";
                RetObj["succeeded"] = "true";
                RetObj["groupnodenames"] = new JArray();
                RetObj["groupnodetypename"] = string.Empty;
            }
            else
            {
                RetObj["succeeded"] = "false";
                CswNbtView InspectionScheduleView = null;
                string InspectionTargetGroupName = string.Empty;
                if( null != InspectionDesignNt && InspectionDesignNt.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                {
                    InspectionScheduleView = _getSchedulesViewFromInspectionDesign( InspectionDesignNt, ref InspectionTargetGroupName );
                }
                else
                {
                    _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                    InspectionScheduleView = _getSchedulesViewFromInspectionTarget( InspectionTargetNt, ref InspectionTargetGroupName );
                }
                if( null != InspectionScheduleView )
                {
                    RetObj["succeeded"] = "true";
                    ICswNbtTree GroupTree = _CswNbtResources.Trees.getTreeFromView( InspectionScheduleView, true, true, true, false );
                    RetObj["groupcount"] = GroupTree.getChildNodeCount().ToString();

                    JArray NodeNames = new JArray();
                    RetObj["groupnodenames"] = NodeNames;
                    if( GroupTree.getChildNodeCount() > 0 )
                    {
                        for( Int32 GroupChild = 0; GroupChild < GroupTree.getChildNodeCount(); GroupChild += 1 )
                        {
                            GroupTree.goToNthChild( GroupChild );
                            CswNbtNode GroupNode = GroupTree.getNodeForCurrentPosition();

                            if( null != GroupNode )
                            {
                                NodeNames.Add( GroupNode.NodeName );
                                JArray Schedules = new JArray();
                                RetObj[GroupNode.NodeName] = Schedules;
                                for( Int32 SchedChild = 0; SchedChild < GroupTree.getChildNodeCount(); SchedChild += 1 )
                                {
                                    GroupTree.goToNthChild( SchedChild );
                                    CswNbtNode ScheduleNode = GroupTree.getNodeForCurrentPosition();
                                    Schedules.Add( ScheduleNode.NodeName );
                                    GroupTree.goToParentNode();
                                }
                            }
                            GroupTree.goToParentNode();
                        }
                    }

                    RetObj["groupnodetypename"] = InspectionTargetGroupName;
                }
            }
            return RetObj;
        }

        #endregion Public

    }
}