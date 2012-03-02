using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActInspectionDesignWiz
    {

        #       region ctor
        CswNbtResources _CswNbtResources = null;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        private bool _IsSchemaUpdater = false;
        private NbtViewVisibility _newViewVis;
        private Int32 _VisId = Int32.MinValue;

        public CswNbtActInspectionDesignWiz( CswNbtResources CswNbtResources, NbtViewVisibility newViewVis, ICswNbtUser newViewUser, bool isSchemaUpdater )
        {
            _CswNbtResources = CswNbtResources;
            _IsSchemaUpdater = isSchemaUpdater;
            _newViewVis = newViewVis;
            _CurrentUser = newViewUser;

            if( NbtViewVisibility.User == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.UserId.PrimaryKey;
            if( NbtViewVisibility.Role == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.RoleId.PrimaryKey;

            if( false == _IsSchemaUpdater && _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName )
            {
                throw new CswDniException( ErrorType.Error, "Only the ChemSW Admin role can access the Inspection Design wizard.", "Attempted to access the Inspection Design wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }//ctor
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

        private CswCommaDelimitedString _ProposedNodeTypeNames = new CswCommaDelimitedString();
        private Int32 _DesignNtId = 0;
        private Int32 _TargetNtId = 0;
        private Int32 _GroupNtId = 0;

        #region MetaData

        /// <summary>
        /// Verify that NodeTypeName is Unique in Database and in Session
        /// </summary>
        private void _checkUniqueNodeType( string NodeTypeName )
        {
            string NameToTest = _standardizeName( NodeTypeName );

            if( null != _CswNbtResources.MetaData.getNodeType( NameToTest ) )
            {
                if( _ProposedNodeTypeNames.Contains( NameToTest ) )
                {
                    throw new CswDniException( ErrorType.Warning, "The provided name is not unique.", "A proposed NodeType with the name " + NameToTest + " already exists in ProposedNodeTypeNames." );
                }
            }
        }

        private string _buildString( string Padding, string StringToAdd )
        {
            string Ret = "";
            if( false == string.IsNullOrEmpty( Padding ) )
            {
                Ret += Padding;
            }
            if( false == string.IsNullOrEmpty( StringToAdd ) )
            {
                Ret += StringToAdd;
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
                    CategoryName += _buildString( ": ", InspectionTargetNt.Category );
                }
                else
                {
                    CategoryName += _buildString( ": ", InspectionTargetName );
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
            if( ObjectClass != NodeType.getObjectClass().ObjectClass )
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
        }

        /// <summary>
        /// Standardize the NodeType Name, check for uniqueness, and add to cached list of new, unique nodetypenames
        /// </summary>
        private string _validateNodeTypeName( object Name, Int32 AllowedLength = Int32.MinValue )
        {
            string RetString = _standardizeName( Name, AllowedLength );
            _checkUniqueNodeType( RetString );
            _ProposedNodeTypeNames.Add( RetString );
            return RetString;
        }

        /// <summary>
        /// Convert the name into Title Case, trim spaces and optionally truncate the name to a specified length
        /// </summary>
        private string _standardizeName( object Name, Int32 AllowedLength = Int32.MinValue )
        {
            string RetString = CswConvert.ToString( Name ).Trim();
            if( 0 < AllowedLength &&
                    AllowedLength < RetString.Length )
            {
                RetString = RetString.Substring( 0, ( AllowedLength - 1 ) );
            }
            return RetString;
        }

        private Dictionary<string, CswNbtMetaDataNodeTypeTab> _getTabsForInspection( JArray Grid, CswNbtMetaDataNodeType NodeType )
        {
            int qtab_order = 0;
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = _standardizeName( ThisRow[_SectionName] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = "Section 1"; //this is objectclass voodoo, don't mess with it! we will rename it to Questions later...
                    }
                    if( false == RetDict.ContainsKey( TabName ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NodeType.getNodeTypeTab( TabName );
                        if( null == ThisTab )
                        {
                            ++qtab_order;
                            //                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, NodeType.getNodeTypeTabs().Count() );
                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, qtab_order );
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
                                ThisQuestion.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
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

            string InspectionDesignName = InspectionDesignNt.NodeTypeName;

            //if we're here, we're validated
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            //CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );

            //This will validate names and throw if not unique.
            //Case 24408: In Db, NodeTypeName == varchar(50)
            InspectionTargetName = _validateNodeTypeName( InspectionTargetName, 44 );
            //Create the new NodeTypes
            RetInspectionTargetNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetOc.ObjectClassId, InspectionTargetName, Category );
            _setNodeTypePermissions( RetInspectionTargetNt );

            string InspectionGroupName = _validateNodeTypeName( InspectionTargetName + " Group" );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetGroupOc.ObjectClassId, InspectionGroupName, Category );
            _GroupNtId = InspectionTargetGroupNt.FirstVersionNodeTypeId;

            _setNodeTypePermissions( InspectionTargetGroupNt );

            #region Set new InspectionTarget Props and Tabs

            //Inspection Target has Inspection Target Group Relationship
            CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            ItInspectionGroupNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
            ItInspectionGroupNtp.PropName = InspectionGroupName;

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItDescriptionNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            RetInspectionTargetNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( RetInspectionTargetNt.getBarcodeProperty().PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName ) );
            ItDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ItInspectionGroupNtp );

            CswNbtMetaDataNodeTypeProp ItBarcodeNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.BarcodePropertyName );
            ItBarcodeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ItDescriptionNtp );

            //Inspection Target has a tab to host a grid view of Inspections
            CswNbtMetaDataNodeTypeTab ItInspectionsTab = _CswNbtResources.MetaData.makeNewTab( RetInspectionTargetNt, InspectionDesignName, 2 );
            CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( RetInspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionDesignName, ItInspectionsTab.TabId );
            CswNbtView ItInspectionsGridView = _createInspectionsGridView( ItInspectionsNtp, InspectionDesignNt, string.Empty, NbtViewRenderingMode.Grid, true, DateTime.MinValue, InspectionTargetName + " Grid Prop View" );
            ItInspectionsNtp.ViewId = ItInspectionsGridView.ViewId;
            ItInspectionsNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            #endregion Set new InspectionTarget Props and Tabs

            #region Set InspectionTargetGroup Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItgNameNtp = InspectionTargetGroupNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTargetGroup.NamePropertyName );
            InspectionTargetGroupNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName ) );

            //Description is useful.
            _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Text, "Description", InspectionTargetGroupNt.getFirstNodeTypeTab().TabId );

            //Inspection Target Group has a tab to host a grid view of Inspection Targets
            CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 2 );
            CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName + " Locations", ItgLocationsTab.TabId );
            CswNbtView ItgInspectionPointsGridView = _createAllInspectionPointsGridView( InspectionTargetGroupNt, RetInspectionTargetNt, string.Empty, NbtViewRenderingMode.Grid, InspectionTargetName + " Grid Prop View" );
            ItgLocationsNtp.ViewId = ItgInspectionPointsGridView.ViewId;
            ItgLocationsNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            #endregion Set InspectionTargetGroup Props and Tabs

            return RetInspectionTargetNt;
        }

        private void _setInspectionDesignTabsAndProps( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            _DesignNtId = InspectionDesignNt.FirstVersionNodeTypeId;

            //CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            Int32 IdNameNtpId = InspectionDesignNt.getNodeTypePropIdByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            //IdNameNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, InspectionDesignNt.NodeTypeId, IdNameNtpId, Int32.MinValue, Int32.MinValue, Int32.MinValue );
            //NodeTypeName Template
            if( string.IsNullOrEmpty( InspectionDesignNt.NameTemplateValue ) )
            {
                InspectionDesignNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( IdNameNtpId.ToString() );
            }

            //Inspection Design Target is Inspection Target OC
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
            IdTargetNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            IdTargetNtp.IsRequired = true;
            if( IdTargetNtp.FKType != NbtViewRelatedIdType.ObjectClassId.ToString() &&
                IdTargetNtp.FKValue != InspectionTargetOc.ObjectClassId )
            {
                IdTargetNtp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), InspectionTargetOc.ObjectClassId );
            }

            CswNbtMetaDataObjectClassProp IdLocationOcp = InspectionTargetOc.getObjectClassProp( CswNbtObjClassInspectionTarget.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp IdLocationNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.LocationPropertyName );
            if(IdLocationNtp.FKType !=  NbtViewPropIdType.ObjectClassPropId.ToString() ||
               IdLocationNtp.FKValue != IdLocationOcp.ObjectClassPropId )
            {
                IdLocationNtp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), InspectionTargetOc.ObjectClassId, NbtViewPropIdType.ObjectClassPropId.ToString(), IdLocationOcp.ObjectClassPropId );
            }

            //Inspection Design Generator is SI Inspection Schedule
            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateInspectionScheduleNt( GeneratorNt );

            CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            if( IdGeneratorNtp.FKType != NbtViewRelatedIdType.NodeTypeId.ToString() &&
                IdGeneratorNtp.FKValue != GeneratorNt.NodeTypeId )
            {
                IdGeneratorNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );
                IdGeneratorNtp.PropName = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;
            }

            CswNbtMetaDataNodeTypeProp IdDueDateNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );
            IdDueDateNtp.IsRequired = true;
            IdDueDateNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
        }

        private void _pruneSectionOneTab( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataNodeTypeTab SectionOneTab = InspectionDesignNt.getNodeTypeTab( "Section 1" );
            if( null != SectionOneTab )
            {
                if( SectionOneTab.NodeTypeProps.Count() > 0 )
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
            CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionScheduleNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName );
            CswNbtMetaDataObjectClass GroupOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

            if( OwnerNtp.FKType != NbtViewRelatedIdType.ObjectClassId.ToString() || OwnerNtp.FKValue != GroupOC.ObjectClassId )
            {
                OwnerNtp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
                // twice to set the view
                OwnerNtp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
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

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionSchedulesViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }
            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( NbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.makeNew( InspectionSchedulesViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.makeNew( InspectionSchedulesViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }
                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
                    _validateNodeType( GeneratorNt, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                    CswNbtMetaDataNodeTypeProp GnOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName );

                    CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                    CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
                    _validateNodeType( InspectionTargetGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

                    /* View:
                        [Group]
                            [Schedule]                        
                    */

                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, NbtViewPropOwnerType.Second, GnOwnerNtp, false );
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
            CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
            _validateNodeType( InspectionTargetGroupNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

            CswNbtView RetView = null;
            string GroupAssignmentViewName = "Groups, " + InspectionDesignNt.NodeTypeName + ": " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( GroupAssignmentViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( NbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.makeNew( GroupAssignmentViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.makeNew( GroupAssignmentViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }
                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    /* View:
                        [Group]
                            [Target]
                        [Target]
                    */
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, NbtViewPropOwnerType.Second, ItTargetGroupNtp, false );
                    //Only show unrelated targets at the root level
                    CswNbtViewRelationship DanglingTargetRel = RetView.AddViewRelationship( InspectionTargetNt, false );
                    CswNbtViewProperty GroupVp = RetView.AddViewProperty( DanglingTargetRel, ItTargetGroupNtp );
                    RetView.AddViewPropertyFilter( GroupVp, ItTargetGroupNtp.getFieldTypeRule().SubFields[CswNbtSubField.SubFieldName.NodeID].Name, CswNbtPropFilterSql.PropertyFilterMode.Null, string.Empty, false );
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

        private CswNbtView _createInspectionsGridView( CswNbtMetaDataNodeTypeProp InspectionsGridProp, CswNbtMetaDataNodeType InspectionDesignNt, string Category, NbtViewRenderingMode ViewMode,
             bool AllInspections, DateTime DueDate, string InspectionsViewName )
        {
            _validateNodeType( InspectionDesignNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            if( string.IsNullOrEmpty( InspectionsViewName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create an Inspections view without a name.", "View name was null or empty." );
            }

            CswNbtView RetView = new CswNbtView( _CswNbtResources );
            try
            {
                RetView.makeNew( InspectionsViewName, NbtViewVisibility.Property, null, null, null );
                RetView.ViewMode = ViewMode;
                RetView.Category = Category;

                CswNbtViewRelationship TargetVr = RetView.AddViewRelationship( InspectionsGridProp.getNodeType(), true );
                CswNbtViewRelationship InspectionVr = RetView.AddViewRelationship( TargetVr, NbtViewPropOwnerType.Second, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.OwnerPropertyName ), true );
                CswNbtMetaDataNodeTypeProp DueDateNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );
                CswNbtViewProperty DueDateVp = RetView.AddViewProperty( InspectionVr, DueDateNtp );
                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty StatusVp = RetView.AddViewProperty( InspectionVr, StatusNtp );

                if( false == AllInspections )
                {
                    ICswNbtFieldTypeRule FTRule = StatusNtp.getFieldTypeRule();
                    RetView.AddViewPropertyFilter( StatusVp, FTRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled ), false );
                    RetView.AddViewPropertyFilter( StatusVp, FTRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed ), false );
                    RetView.AddViewPropertyFilter( StatusVp, FTRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late ), false );
                    RetView.AddViewPropertyFilter( StatusVp, FTRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed ), false );
                }

                if( DateTime.MinValue != DueDate )
                {
                    RetView.AddViewPropertyFilter( DueDateVp, DueDateNtp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, DateTime.Today.ToString(), false );
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
            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionTargetViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( NbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.makeNew( InspectionTargetViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.makeNew( InspectionTargetViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }

                    RetView.ViewMode = NbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeTypeProp ItTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );

                    /* View:
                        [Target]
                            [Inspection]                        
                    */

                    CswNbtViewRelationship TargetRelationship = RetView.AddViewRelationship( InspectionTargetNt, false );
                    RetView.AddViewRelationship( TargetRelationship, NbtViewPropOwnerType.Second, ItTargetNtp, false );
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

        private CswNbtView _createAllInspectionPointsGridView( CswNbtMetaDataNodeType InspectionGroupNt, CswNbtMetaDataNodeType InspectionTargetNt, string Category, NbtViewRenderingMode ViewMode,
             string AllInspectionPointsViewName )
        {
            _validateNodeType( InspectionTargetNt, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtView RetView = new CswNbtView( _CswNbtResources );

            try
            {
                RetView.makeNew( AllInspectionPointsViewName, NbtViewVisibility.Property, null, null, null );
                RetView.Category = Category;
                RetView.ViewMode = ViewMode;

                CswNbtViewRelationship InspectionGroupVr = RetView.AddViewRelationship( InspectionGroupNt, true );

                CswNbtMetaDataNodeTypeProp InspectionGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                CswNbtViewRelationship InspectionTargetVr = RetView.AddViewRelationship( InspectionGroupVr, NbtViewPropOwnerType.Second, InspectionGroupNtp, true );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = InspectionTargetNt.getBarcodeProperty();
                RetView.AddViewProperty( InspectionTargetVr, BarcodeNtp ).Order = 0;

                CswNbtMetaDataNodeTypeProp DescriptionNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                RetView.AddViewProperty( InspectionTargetVr, DescriptionNtp ).Order = 1;

                CswNbtMetaDataNodeTypeProp LocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.LocationPropertyName );
                RetView.AddViewProperty( InspectionTargetVr, LocationNtp ).Order = 2;

                CswNbtMetaDataNodeTypeProp DateNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                RetView.AddViewProperty( InspectionTargetVr, DateNtp ).Order = 3;

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.StatusPropertyName );
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

        #region public

        public Int32 DesignNtId { get { return ( _DesignNtId ); } }
        public Int32 TargetNtId { get { return ( _TargetNtId ); } }
        public Int32 GroupNtId { get { return ( _GroupNtId ); } }


        public DataTable prepareDataTable( DataTable UploadDataTable )
        {
            DataTable RetDataTable = new DataTable();
            try
            {
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
            }
            catch( Exception Exception )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }

            return ( RetDataTable );
        }

        public JObject recycleInspectionDesign( string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( InspectionDesignName );
            _setInspectionDesignTabsAndProps( InspectionDesignNt );

            CswNbtMetaDataNodeType InspectionTargetNt = _confirmInspectionDesignTarget( InspectionDesignNt, InspectionTargetName, ref Category );
            _TargetNtId = InspectionTargetNt.FirstVersionNodeTypeId;

            JObject RetObj = _createInspectionDesignViews( Category, InspectionDesignNt, InspectionTargetNt );

            return RetObj;
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JArray GridArray = JArray.Parse( GridArrayString );
            return ( _createInspectionDesignTabsAndProps( GridArray, InspectionDesignName, InspectionTargetName, Category ) );
        }

        public JObject createInspectionDesignTabsAndProps( DataTable TheQuestions, string InspectionDesignName, string InspectionTargetName, string Category )
        {

            JObject GridObj = CswConvert.DataTableToJSON( prepareDataTable( TheQuestions ) );
            JArray GridArray = (JArray) GridObj["data"];
            return ( _createInspectionDesignTabsAndProps( GridArray, InspectionDesignName, InspectionTargetName, Category ) );
        }

        private JObject _createInspectionDesignTabsAndProps( JArray GridArray, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();

            InspectionDesignName = _validateNodeTypeName( InspectionDesignName );

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
            _TargetNtId = InspectionTargetNt.FirstVersionNodeTypeId;

            //The Category name is now set
            InspectionDesignNt.Category = Category;

            //Get the views
            JObject RetObj = _createInspectionDesignViews( Category, InspectionDesignNt, InspectionTargetNt );

            //More return data
            RetObj["totalrows"] = TotalRows.ToString();
            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            return ( RetObj );
        }

        #endregion public



    }//class CswNbtActInspectionDesignWiz

}//namespace ChemSW.Actions
