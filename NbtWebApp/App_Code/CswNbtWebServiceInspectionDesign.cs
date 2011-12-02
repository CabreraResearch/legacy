using System;
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
                Ret = _standardizeName( NodeTypeName );
            }
            return Ret;
        }

        private string _guaranteeCategoryName( string Category, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionDesignNt, string InspectionTargetName )
        {
            string CategoryName = Category;
            if( string.IsNullOrEmpty( CategoryName ) )
            {
                if( null != InspectionDesignNt )
                {
                    CategoryName = InspectionDesignNt.Category;
                }

                if( null != InspectionTargetNt )
                {
                    CategoryName += ": " + InspectionTargetNt.Category;
                }
                else
                {
                    CategoryName += ": " + InspectionTargetName;
                }

            }
            CategoryName = _standardizeName( CategoryName );
            return CategoryName;
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

        private string _standardizeName( object Name )
        {
            return _TextInfo.ToTitleCase( CswConvert.ToString( Name ).Trim() );
        }

        private Dictionary<string, CswNbtMetaDataNodeTypeTab> _getTabsForInspection( JArray Grid, CswNbtMetaDataNodeType NodeType )
        {
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = _standardizeName( ThisRow[_SectionName] );
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
                    string TabName = _standardizeName( ThisRow[_SectionName] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = _DefaultSectionName;
                    }
                    string Question = CswConvert.ToString( ThisRow[_QuestionName] );
                    string AllowedAnswers = CswConvert.ToString( ThisRow[_AllowedAnswersName] );
                    string CompliantAnswers = CswConvert.ToString( ThisRow[_CompliantAnswersName] );
                    string HelpText = CswConvert.ToString( ThisRow[_HelpTextName] );

                    if( false == string.IsNullOrEmpty( Question ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab;
                        Tabs.TryGetValue( TabName, out ThisTab );
                        Int32 ThisTabId;
                        if( null != ThisTab )
                        {
                            ThisTabId = ThisTab.TabId;
                        }
                        else
                        {
                            ThisTabId = Tabs[_DefaultSectionName].TabId;
                        }

                        CswNbtMetaDataNodeTypeProp ThisQuestion = InspectionDesignNt.getNodeTypeProp( Question.ToLower() );
                        if( null == ThisQuestion )
                        {
                            ThisQuestion = _CswNbtResources.MetaData.makeNewProp( InspectionDesignNt, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, ThisTabId );

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
                    else
                    {
                        GridRowsSkipped.Add( Index.ToString() );
                    }
                }
            }
            return RetCount;
        }

        private CswNbtMetaDataNodeType _confirmInspectionDesignTarget( CswNbtMetaDataNodeType InspectionDesignNt, string InspectionTargetName, ref string Category )
        {
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot generate an Inspection Design without a Target name.", "InspectionTargetName was null or empty." );
            }
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );

            Category = _guaranteeCategoryName( Category, InspectionTargetNt, InspectionDesignNt, InspectionTargetName );

            //This is a New Target
            if( null == InspectionTargetNt )
            {
                InspectionTargetNt = _createNewInspectionTargetAndGroup( InspectionTargetName, Category, InspectionDesignNt );
            }
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );

            return InspectionTargetNt;
        }

        private CswNbtMetaDataNodeType _createNewInspectionTargetAndGroup( string InspectionTargetName, string Category, CswNbtMetaDataNodeType InspectionDesignNt )
        {
            CswNbtMetaDataNodeType RetInspectionTargetNt = null;
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create Inspection Target without a name.", "InspectionTargetName was null or empty." );
            }
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateNodeType( GeneratorNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            _setNodeTypePermissions( GeneratorNt );

            //This will validate names and throw if not unique.
            InspectionTargetName = _checkUniqueNodeType( InspectionTargetName.Trim() );
            string InspectionGroupName = _checkUniqueNodeType( InspectionTargetName + " Group" );
            string InspectionDesignName = InspectionDesignNt.NodeTypeName;

            //if we're here, we're validated
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            //CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );

            //Create the new NodeTypes
            RetInspectionTargetNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetOc.ObjectClassId, InspectionTargetName, Category );
            _setNodeTypePermissions( RetInspectionTargetNt );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetGroupOc.ObjectClassId, InspectionGroupName, Category );
            _setNodeTypePermissions( InspectionTargetGroupNt );

            #region Set new InspectionTarget Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItDescriptionNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            RetInspectionTargetNt.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( RetInspectionTargetNt.BarcodeProperty.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName );

            //Inspection Target has Inspection Target Group Relationship
            CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            ItInspectionGroupNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
            ItInspectionGroupNtp.PropName = InspectionGroupName;

            //Inspection Target has a tab to host a grid view of Inspections
            CswNbtMetaDataNodeTypeTab ItInspectionsTab = _CswNbtResources.MetaData.makeNewTab( RetInspectionTargetNt, InspectionDesignName, 2 );
            CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( RetInspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionDesignName, ItInspectionsTab.TabId );
            CswNbtView ItInspectionsGridView = _createInspectionsView( InspectionDesignNt, string.Empty, NbtViewRenderingMode.Grid, NbtViewVisibility.Property, true, DateTime.MinValue, InspectionTargetName + " Grid Prop View" );
            ItInspectionsNtp.ViewId = ItInspectionsGridView.ViewId;
            ItInspectionsNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            #endregion Set new InspectionTarget Props and Tabs

            #region Set InspectionTargetGroup Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItgNameNtp = InspectionTargetGroupNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTargetGroup.NamePropertyName );
            InspectionTargetGroupNt.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName );

            //Description is useful.
            _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Text, "Description", InspectionTargetGroupNt.getFirstNodeTypeTab().TabId );

            //Inspection Target Group has a tab to host a grid view of Inspection Targets
            CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 2 );
            CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName + " Locations", ItgLocationsTab.TabId );
            CswNbtView ItgInspectionPointsGridView = _createAllInspectionPointsView( RetInspectionTargetNt, string.Empty, NbtViewRenderingMode.Grid, NbtViewVisibility.Property, InspectionTargetName + " Grid Prop View" );
            ItgLocationsNtp.ViewId = ItgInspectionPointsGridView.ViewId;
            ItgLocationsNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            #endregion Set InspectionTargetGroup Props and Tabs

            return RetInspectionTargetNt;
        }

        private void _setInspectionDesignTabsAndProps( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            //NodeTypeName Template
            if( string.IsNullOrEmpty( InspectionDesignNt.NameTemplateValue ) )
            {
                CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
                InspectionDesignNt.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( IdNameNtp.PropName );
            }

            //Inspection Design Target is Inspection Target OC
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            if( IdTargetNtp.FKType != CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                IdTargetNtp.FKValue != InspectionTargetOc.ObjectClassId )
            {
                IdTargetNtp.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), InspectionTargetOc.ObjectClassId );
            }

            //Inspection Design Generator is SI Inspection Schedule
            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateInspectionScheduleNt( GeneratorNt );

            CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            if( IdGeneratorNtp.FKType != CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                IdGeneratorNtp.FKValue != GeneratorNt.NodeTypeId )
            {
                IdGeneratorNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );
                IdGeneratorNtp.PropName = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;
            }
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

        private void _validateInspectionScheduleNt( CswNbtMetaDataNodeType InspectionScheduleNt )
        {
            _validateNodeType( InspectionScheduleNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionScheduleNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );

            if( OwnerNtp.FKType != CswNbtViewProperty.CswNbtPropType.ObjectClassPropId.ToString() || OwnerNtp.FKValue != InspectionScheduleNt.ObjectClass.ObjectClassId )
            {
                OwnerNtp.SetFK( CswNbtViewProperty.CswNbtPropType.ObjectClassPropId.ToString(), InspectionScheduleNt.ObjectClass.ObjectClassId );
            }

        }

        #endregion MetaData

        #region Views

        private void _getClientViewData( CswNbtView View, JObject RetObj )
        {
            if( null != View )
            {
                RetObj[View.ViewName] = new JObject();
                RetObj[View.ViewName]["viewname"] = View.ViewName;
                if( null != View.SessionViewId &&
                    View.SessionViewId.isSet() )
                {
                    RetObj[View.ViewName]["viewid"] = View.SessionViewId.ToString();
                }
                else
                {
                    RetObj[View.ViewName]["viewid"] = View.ViewId.ToString();
                }
            }
        }

        private JObject _createInspectionDesignViews( string Category, CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            JObject RetObj = new JObject();

            RetObj["views"] = new JObject();

            //Inspection Scheduling view
            CswNbtView InspectionSchedulesView = _createInspectionSchedulingView( InspectionDesignNt, Category, InspectionTargetNt );
            _getClientViewData( InspectionSchedulesView, ( (JObject) RetObj["views"] ) );

            //Inspection Target Group Assignement view
            CswNbtView InspectionTargetGroupAssignmentView = _createInspectionGroupAssignmentView( Category, InspectionTargetNt, InspectionDesignNt );
            _getClientViewData( InspectionTargetGroupAssignmentView, ( (JObject) RetObj["views"] ) );

            //Target Inspections view
            CswNbtView TargetInspectionsView = _createTargetInspectionsView( InspectionDesignNt, Category, InspectionTargetNt );
            _getClientViewData( TargetInspectionsView, ( (JObject) RetObj["views"] ) );

            return RetObj;
        }

        private CswNbtView _createInspectionSchedulingView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtView RetView = null;
            string InspectionSchedulesViewName = "Scheduling, " + InspectionDesignNt.NodeTypeName + ": " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionSchedulesViewName, NbtViewVisibility.Role, _CurrentUser.RoleId.PrimaryKey ) )
            {
                RetView = SchedulingView;
                break;
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    RetView.makeNew( InspectionSchedulesViewName, NbtViewVisibility.Role, _CurrentUser.RoleId, null, null );
                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
                    _validateNodeType( GeneratorNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                    CswNbtMetaDataNodeTypeProp GnOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );

                    CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                    CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
                    _validateNodeType( InspectionTargetGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

                    /* View:
                        [Group]
                            [Schedule]                        
                    */

                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, GnOwnerNtp, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( ErrorType.Error, "Failed to create view: " + InspectionSchedulesViewName, "View creation failed", ex );
                }
            }
            RetView.SaveToCache( true );
            return RetView;
        }

        private CswNbtView _createInspectionGroupAssignmentView( string Category, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
            _validateNodeType( InspectionTargetGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

            CswNbtView RetView = null;
            string GroupAssignmentViewName = "Groups, " + InspectionDesignNt.NodeTypeName + ": " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( GroupAssignmentViewName, NbtViewVisibility.Role, _CurrentUser.RoleId.PrimaryKey ) )
            {
                RetView = SchedulingView;
                break;
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    RetView.makeNew( GroupAssignmentViewName, NbtViewVisibility.Role, _CurrentUser.RoleId, null, null );
                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    /* View:
                        [Group]
                            [Target]
                        [Target]
                    */
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, ItTargetGroupNtp, false );
                    //Only show unrelated targets at the root level
                    CswNbtViewRelationship DanglingTargetRel = RetView.AddViewRelationship( InspectionTargetNt, false );
                    CswNbtViewProperty GroupVp = RetView.AddViewProperty( DanglingTargetRel, ItTargetGroupNtp );
                    RetView.AddViewPropertyFilter( GroupVp, ItTargetGroupNtp.FieldTypeRule.SubFields[CswNbtSubField.SubFieldName.NodeID].Name, CswNbtPropFilterSql.PropertyFilterMode.Null, string.Empty, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( ErrorType.Error, "Failed to create view: " + GroupAssignmentViewName, "View creation failed", ex );
                }
            }
            RetView.SaveToCache( true );
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

        private CswNbtView _createTargetInspectionsView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );

            CswNbtView RetView = null;
            string InspectionTargetViewName = "Inspections, " + InspectionDesignNt.NodeTypeName + ": " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionTargetViewName, NbtViewVisibility.Role, _CurrentUser.RoleId.PrimaryKey ) )
            {
                RetView = SchedulingView;
                break;
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    RetView.makeNew( InspectionTargetViewName, NbtViewVisibility.Role, _CurrentUser.RoleId, null, null );
                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeTypeProp ItTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );

                    /* View:
                        [Target]
                            [Inspection]                        
                    */

                    CswNbtViewRelationship TargetRelationship = RetView.AddViewRelationship( InspectionTargetNt, false );
                    RetView.AddViewRelationship( TargetRelationship, CswNbtViewRelationship.PropOwnerType.Second, ItTargetNtp, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( ErrorType.Error, "Failed to create view: " + InspectionTargetViewName, "View creation failed", ex );
                }
            }
            RetView.SaveToCache( true );
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
                    if( false == string.IsNullOrEmpty( ThisAnswer ) &&
                            false == UniqueCompliantAnswers.ContainsKey( ThisAnswer ) )
                    {
                        UniqueCompliantAnswers.Add( ThisAnswer, CompliantAnswer );
                    }
                }
                Dictionary<string, string> UniqueAllowedAnswers = new Dictionary<string, string>();
                foreach( string AllowedAnswer in AllowedAnswers )
                {
                    string ThisAnswer = AllowedAnswer.ToLower().Trim();
                    if( false == string.IsNullOrEmpty( ThisAnswer ) &&
                            false == UniqueAllowedAnswers.ContainsKey( ThisAnswer ) )
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
                else //We need at least one compliant answer. If none are provided, then all allowed answers are compliant.
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
            DataTable RetDataTable = new DataTable();
            OleDbConnection ExcelConn = null;

            try
            {
                // Microsoft JET engine knows how to read Excel files as a database
                // Problem is - it is old OLE technology - not newer ADO.NET
                string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FullPathAndFileName + ";Extended Properties=Excel 8.0;";
                ExcelConn = new OleDbConnection( ConnStr );
                ExcelConn.Open();

                DataTable InspectionDt = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                if( null == InspectionDt )
                {
                    throw new CswDniException( ErrorType.Error, "Could not process the uploaded file: " + FullPathAndFileName, "GetOleDbSchemaTable failed to parse a valid XLS file." );
                }

                string FirstSheetName = InspectionDt.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                DataTable UploadDataTable = new DataTable();

                DataAdapter.Fill( UploadDataTable );

                //Normalize the incoming column names
                foreach( DataColumn Column in UploadDataTable.Columns )
                {
                    Column.ColumnName = Column.ColumnName.ToUpper().Replace( " ", "_" );
                }

                //Prep the outgoing column names
                foreach( string ColumnName in _ColumnNames )
                {
                    RetDataTable.Columns.Add( ColumnName );
                }
                RetDataTable.Columns.Add( "RowNumber" );

                Int32 RowNumber = 0;
                foreach( DataRow Row in UploadDataTable.Rows )
                {
                    string Question = _standardizeName( Row[_QuestionName] );
                    if( false == string.IsNullOrEmpty( Question ) )
                    {
                        DataRow NewRow = RetDataTable.NewRow();
                        NewRow[_QuestionName] = Question;

                        string AllowedAnswers = CswConvert.ToString( Row[_AllowedAnswersName] );
                        string ComplaintAnswers = CswConvert.ToString( Row[_CompliantAnswersName] );
                        _validateAnswers( ref ComplaintAnswers, ref AllowedAnswers );

                        NewRow[_AllowedAnswersName] = AllowedAnswers;
                        NewRow[_CompliantAnswersName] = ComplaintAnswers;
                        NewRow[_HelpTextName] = CswConvert.ToString( Row[_HelpTextName] );

                        string SectionName = _standardizeName( Row[_SectionName] );
                        if( string.Empty == SectionName ||
                            "Section 1" == SectionName )
                        {
                            SectionName = _DefaultSectionName;
                        }
                        NewRow[_SectionName] = SectionName;
                        NewRow["RowNumber"] = RowNumber;

                        RetDataTable.Rows.Add( NewRow );
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
            return RetDataTable;
        } // convertExcelFileToDataTable()

        public JObject recycleInspectionDesign( string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( InspectionDesignName );
            _setInspectionDesignTabsAndProps( InspectionDesignNt );

            CswNbtMetaDataNodeType InspectionTargetNt = _confirmInspectionDesignTarget( InspectionDesignNt, InspectionTargetName, ref Category );
            JObject RetObj = _createInspectionDesignViews( Category, InspectionDesignNt, InspectionTargetNt );

            return RetObj;
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();

            InspectionDesignName = _checkUniqueNodeType( InspectionDesignName );

            JArray GridArray = JArray.Parse( GridArrayString );
            if( null == GridArray || GridArray.Count == 0 )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create Inspection Design " + InspectionDesignName + ", because the import contained no questions.", "GridArray was null or empty." );
            }

            Int32 TotalRows = GridArray.Count;

            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass.ToString(), InspectionDesignName, string.Empty );
            _setInspectionDesignTabsAndProps( InspectionDesignNt );
            _setNodeTypePermissions( InspectionDesignNt );

            //Get distinct tabs
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = _getTabsForInspection( GridArray, InspectionDesignNt );
            //Create the props
            Int32 PropsWithoutError = _createInspectionProps( GridArray, InspectionDesignNt, Tabs, GridRowsSkipped );
            //Delete or rename the "Section 1" tab
            _pruneSectionOneTab( InspectionDesignNt );
            //Build the MetaData
            CswNbtMetaDataNodeType InspectionTargetNt = _confirmInspectionDesignTarget( InspectionDesignNt, InspectionTargetName, ref Category );
            //The Category name is now set
            InspectionDesignNt.Category = Category;

            //Get the views
            JObject RetObj = _createInspectionDesignViews( Category, InspectionDesignNt, InspectionTargetNt );

            //More return data
            RetObj["totalrows"] = TotalRows.ToString();
            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            return RetObj;
        }

        #endregion Public

    }
}