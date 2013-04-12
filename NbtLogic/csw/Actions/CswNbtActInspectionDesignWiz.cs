using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActInspectionDesignWiz
    {
        #region ctor
        CswNbtResources _CswNbtResources = null;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        private bool _IsSchemaUpdater = false;
        private CswEnumNbtViewVisibility _newViewVis;
        private Int32 _VisId = Int32.MinValue;
        private bool _targetAlreadyExists;

        public CswNbtActInspectionDesignWiz( CswNbtResources CswNbtResources, CswEnumNbtViewVisibility newViewVis, ICswNbtUser newViewUser, bool isSchemaUpdater )
        {
            _CswNbtResources = CswNbtResources;
            _IsSchemaUpdater = isSchemaUpdater;
            _newViewVis = newViewVis;
            _CurrentUser = newViewUser;

            if( CswEnumNbtViewVisibility.User == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.UserId.PrimaryKey;
            if( CswEnumNbtViewVisibility.Role == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.RoleId.PrimaryKey;

            if( false == _IsSchemaUpdater && _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Only the ChemSW Admin role can access the Inspection Design wizard.", "Attempted to access the Inspection Design wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }//ctor
        #endregion ctor

        #region Private

        private const string _SectionName = "section";
        private const string _QuestionName = "question";
        private const string _AllowedAnswersName = "allowed_answers";
        private const string _CompliantAnswersName = "compliant_answers";
        private const string _PreferredAnswer = "preferred_answer";
        private const string _HelpTextName = "help_text";

        private readonly CswCommaDelimitedString _ColumnNames = new CswCommaDelimitedString
                                                           {
                                                               _SectionName,
                                                               _QuestionName,
                                                               _AllowedAnswersName,
                                                               _CompliantAnswersName,
                                                               _PreferredAnswer,
                                                               _HelpTextName
                                                           };

        private const string _DefaultSectionName = "Questions";
        private const string _DefaultAllowedAnswers = "Yes,No,N/A";
        private const string _DefaultCompliantAnswers = "Yes";

        private CswCommaDelimitedString _ProposedNodeTypeNames = new CswCommaDelimitedString();
        private Int32 _DesignNtId = 0;
        private Int32 _TargetNtId = 0;
        private Int32 _GroupNtId = 0;
        private CswNbtViewId _groupsViewId = null;
        private CswNbtViewId _inspectionsViewId = null;
        private CswNbtViewId _schedulingViewId = null;

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
                    throw new CswDniException( CswEnumErrorType.Warning, "The provided name is not unique.", "A proposed NodeType with the name " + NameToTest + " already exists in ProposedNodeTypeNames." );
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

        private void _validateNodeType( CswNbtMetaDataNodeType NodeType, CswEnumNbtObjectClass ObjectClass )
        {
            if( null == NodeType )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The expected object was not defined", "NodeType for ObjectClass " + ObjectClass + " was null." );
            }
            if( ObjectClass != NodeType.getObjectClass().ObjectClass )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot use a " + NodeType.NodeTypeName + " as an " + ObjectClass, "Attempted to use a NodeType of an unexpected ObjectClass" );
            }
        }

        private void _setNodeTypePermissions( CswNbtMetaDataNodeType NodeType )
        {
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Create, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Edit, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Delete, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.View, NodeType, _CurrentUser, true );
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
            Int32 TabCount = 0;
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
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
                    if( false == RetDict.ContainsKey( TabName ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NodeType.getNodeTypeTab( TabName );
                        if( null == ThisTab )
                        {
                            TabCount += 1;
                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, TabCount );
                        }
                        RetDict.Add( TabName, ThisTab );
                    }
                }
            }
            CswNbtMetaDataNodeTypeTab ActionTab = NodeType.getNodeTypeTab( "Action" );
            if( null != ActionTab )
            {
                TabCount += 1;
                ActionTab.TabOrder = TabCount;
            }
            CswNbtMetaDataNodeTypeTab DetailTab = NodeType.getNodeTypeTab( "Detail" );
            if( null != DetailTab )
            {
                TabCount += 1;
                DetailTab.TabOrder = TabCount;
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
                    string PreferredAnswer = CswConvert.ToString( ThisRow[_PreferredAnswer] );
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
                            ThisQuestion = _CswNbtResources.MetaData.makeNewProp( InspectionDesignNt, CswEnumNbtFieldType.Question, Question, ThisTabId );

                            if( null == ThisQuestion )
                            {
                                GridRowsSkipped.Add( Index.ToString() );
                            }
                            else
                            {
                                _validateAnswers( ref CompliantAnswers, ref AllowedAnswers, ref PreferredAnswer );
                                if( false == string.IsNullOrEmpty( HelpText ) )
                                {
                                    ThisQuestion.HelpText = HelpText;
                                }
                                ThisQuestion.ValueOptions = CompliantAnswers;
                                ThisQuestion.ListOptions = AllowedAnswers;
                                ThisQuestion.Extended = PreferredAnswer;
                                ThisQuestion.removeFromLayout( CswEnumNbtLayoutType.Add );
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
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot generate an Inspection Design without a Target name.", "InspectionTargetName was null or empty." );
            }
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );

            Category = _guaranteeCategoryName( Category, InspectionTargetNt, InspectionDesignNt, InspectionTargetName );

            //This is a New Target
            if( null == InspectionTargetNt )
            {
                InspectionTargetNt = _createNewInspectionTargetAndGroup( InspectionTargetName, Category, InspectionDesignNt );
            }
            else
            {
                _targetAlreadyExists = true;
                _updateInspectionsGridView( InspectionDesignNt, InspectionTargetNt );
            }
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );

            return InspectionTargetNt;
        }

        private CswNbtMetaDataNodeType _createNewInspectionTargetAndGroup( string InspectionTargetName, string Category, CswNbtMetaDataNodeType InspectionDesignNt )
        {
            CswNbtMetaDataNodeType RetInspectionTargetNt = null;
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot create Inspection Target without a name.", "InspectionTargetName was null or empty." );
            }
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );

            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateNodeType( GeneratorNt, CswEnumNbtObjectClass.GeneratorClass );
            _setNodeTypePermissions( GeneratorNt );

            string InspectionDesignName = InspectionDesignNt.NodeTypeName;

            //if we're here, we're validated
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass );
            //CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionRouteClass );

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
            CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
            ItInspectionGroupNtp.SetFK( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
            ItInspectionGroupNtp.PropName = InspectionGroupName;

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItDescriptionNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Description );
            RetInspectionTargetNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( RetInspectionTargetNt.getBarcodeProperty().PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName ) );
            ItDescriptionNtp.updateLayout( CswEnumNbtLayoutType.Add, ItInspectionGroupNtp, true );

            CswNbtMetaDataNodeTypeProp ItBarcodeNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Barcode );
            ItBarcodeNtp.ReadOnly = true; /* Case 25044 */
            ItBarcodeNtp.updateLayout( CswEnumNbtLayoutType.Add, ItDescriptionNtp, true );

            //Inspection Target has a tab to host a grid view of Inspections
            CswNbtMetaDataNodeTypeTab ItInspectionsTab = _CswNbtResources.MetaData.makeNewTab( RetInspectionTargetNt, InspectionDesignName, 2 );
            CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( RetInspectionTargetNt, CswEnumNbtFieldType.Grid, InspectionDesignName, ItInspectionsTab.TabId );
            CswNbtView ItInspectionsGridView = _createInspectionsGridView( InspectionDesignNt, RetInspectionTargetNt );
            ItInspectionsNtp.ViewId = ItInspectionsGridView.ViewId;
            ItInspectionsNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
            #endregion Set new InspectionTarget Props and Tabs

            #region Set InspectionTargetGroup Props and Tabs

            //NodeTypeName Template
            CswNbtMetaDataNodeTypeProp ItgNameNtp = InspectionTargetGroupNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTargetGroup.PropertyName.Name );
            InspectionTargetGroupNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName ) );

            //Description is useful.
            _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswEnumNbtFieldType.Text, "Description", InspectionTargetGroupNt.getFirstNodeTypeTab().TabId );

            //Inspection Target Group has a tab to host a grid view of Inspection Targets
            CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 2 );
            CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswEnumNbtFieldType.Grid, InspectionTargetName + " Locations", ItgLocationsTab.TabId );
            CswNbtView ItgInspectionPointsGridView = _createAllInspectionPointsGridView( InspectionTargetGroupNt, RetInspectionTargetNt, string.Empty, CswEnumNbtViewRenderingMode.Grid, InspectionTargetName + " Grid Prop View" );
            ItgLocationsNtp.ViewId = ItgInspectionPointsGridView.ViewId;
            ItgLocationsNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
            #endregion Set InspectionTargetGroup Props and Tabs

            return RetInspectionTargetNt;
        }

        private void _setInspectionDesignTabsAndProps( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );

            _DesignNtId = InspectionDesignNt.FirstVersionNodeTypeId;

            //CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );
            //IdNameNtp.updateLayout( CswEnumNbtLayoutType.Add );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, InspectionDesignNt.NodeTypeId, IdNameNtp, true, Int32.MinValue, Int32.MinValue, Int32.MinValue );
            //NodeTypeName Template
            if( string.IsNullOrEmpty( InspectionDesignNt.NameTemplateValue ) )
            {
                InspectionDesignNt.NameTemplateValue = CswNbtMetaData.MakeTemplateEntry( IdNameNtp.PropId.ToString() );
            }

            //Inspection Design Target is Inspection Target NT
            CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
            IdTargetNtp.updateLayout( CswEnumNbtLayoutType.Add, true );
            IdTargetNtp.IsRequired = true;
            IdTargetNtp.SetFK( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.NodeTypeId );

            CswNbtMetaDataNodeTypeProp ITargetLocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp IDesignLocationNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Location );
            IDesignLocationNtp.SetFK( CswEnumNbtViewPropIdType.NodeTypePropId.ToString(), IdTargetNtp.PropId, CswEnumNbtViewPropIdType.NodeTypePropId.ToString(), ITargetLocationNtp.PropId );

            //Inspection Design Generator is SI Inspection Schedule
            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateInspectionScheduleNt( GeneratorNt );

            CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Generator );
            if( IdGeneratorNtp.FKType != CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                IdGeneratorNtp.FKValue != GeneratorNt.NodeTypeId )
            {
                IdGeneratorNtp.SetFK( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );
                IdGeneratorNtp.PropName = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;
            }

            CswNbtMetaDataNodeTypeProp IdDueDateNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate );
            IdDueDateNtp.IsRequired = true;
            IdDueDateNtp.updateLayout( CswEnumNbtLayoutType.Add, true );
        }

        private void _validateInspectionScheduleNt( CswNbtMetaDataNodeType InspectionScheduleNt )
        {
            _validateNodeType( InspectionScheduleNt, CswEnumNbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionScheduleNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
            CswNbtMetaDataObjectClass GroupOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass );

            if( OwnerNtp.FKType != CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() || OwnerNtp.FKValue != GroupOC.ObjectClassId )
            {
                OwnerNtp.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
                // twice to set the view
                OwnerNtp.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
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
            _schedulingViewId = InspectionSchedulesView.ViewId;
            _getClientViewData( InspectionSchedulesView, ( (JObject) RetObj["views"] ) );

            //Inspection Target Group Assignement view
            CswNbtView InspectionTargetGroupAssignmentView = _createInspectionGroupAssignmentView( Category, InspectionTargetNt, InspectionDesignNt );
            _groupsViewId = InspectionTargetGroupAssignmentView.ViewId;
            _getClientViewData( InspectionTargetGroupAssignmentView, ( (JObject) RetObj["views"] ) );

            //Target Inspections view
            CswNbtView TargetInspectionsView = _createTargetInspectionsView( InspectionDesignNt, Category, InspectionTargetNt );
            _inspectionsViewId = TargetInspectionsView.ViewId;
            _getClientViewData( TargetInspectionsView, ( (JObject) RetObj["views"] ) );

            return RetObj;
        }

        private CswNbtView _createInspectionSchedulingView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );
            CswNbtView RetView = null;
            string InspectionSchedulesViewName = "Scheduling: " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionSchedulesViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }
            if( _targetAlreadyExists )
            {
                foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionTargetNt.NodeTypeName, true ) )
                {
                    if( SchedulingView.ViewName.Contains( "Scheduling" ) && _isVisibleToCurrentUser( SchedulingView ) )
                    {
                        RetView = SchedulingView;
                        break;
                    }
                }
            }
            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( CswEnumNbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.saveNew( InspectionSchedulesViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.saveNew( InspectionSchedulesViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }
                    RetView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
                    _validateNodeType( GeneratorNt, CswEnumNbtObjectClass.GeneratorClass );
                    CswNbtMetaDataNodeTypeProp GnOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );

                    CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                    CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
                    _validateNodeType( InspectionTargetGroupNt, CswEnumNbtObjectClass.InspectionTargetGroupClass );

                    /* View:
                        [Group]
                            [Schedule]                        
                    */

                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, CswEnumNbtViewPropOwnerType.Second, GnOwnerNtp, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + InspectionSchedulesViewName, "View creation failed: " + ex.StackTrace, ex );
                }
            }
            RetView.SaveToCache( true );
            return RetView;
        }

        private CswNbtView _createInspectionGroupAssignmentView( string Category, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionDesignNt )
        {
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
            CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
            _validateNodeType( InspectionTargetGroupNt, CswEnumNbtObjectClass.InspectionTargetGroupClass );

            CswNbtView RetView = null;
            string GroupAssignmentViewName = "Groups: " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( GroupAssignmentViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }
            if( _targetAlreadyExists )
            {
                foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionTargetNt.NodeTypeName, true ) )
                {
                    if( SchedulingView.ViewName.Contains( "Groups" ) && _isVisibleToCurrentUser( SchedulingView ) )
                    {
                        RetView = SchedulingView;
                        break;
                    }
                }
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( CswEnumNbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.saveNew( GroupAssignmentViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.saveNew( GroupAssignmentViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }
                    RetView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    /* View:
                        [Group]
                            [Target]
                    */
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    RetView.AddViewRelationship( IpGroupRelationship, CswEnumNbtViewPropOwnerType.Second, ItTargetGroupNtp, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + GroupAssignmentViewName, "View creation failed: " + ex.StackTrace, ex );
                }
            }
            RetView.SaveToCache( true );
            return RetView;
        }

        private bool _isVisibleToCurrentUser( CswNbtView View )
        {
            return View.Visibility == NbtViewVisibility.Global ||
                     ( View.Visibility == _newViewVis && ( View.VisibilityRoleId.PrimaryKey == _VisId ||
                                                            View.VisibilityUserId.PrimaryKey == _VisId ) );
        }

        private CswNbtView _createInspectionsGridView( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType RetInspectionTargetNt )
        {
            String GridViewName = RetInspectionTargetNt.NodeTypeName + " Inspections Grid Prop View";
            CswNbtView RetView = new CswNbtView( _CswNbtResources );
            try
            {
                RetView.saveNew( GridViewName, CswEnumNbtViewVisibility.Property, null, null, null );
                RetView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship TargetVr = RetView.AddViewRelationship( RetInspectionTargetNt, true );
                CswNbtViewRelationship InspectionVr = RetView.AddViewRelationship( TargetVr, CswEnumNbtViewPropOwnerType.Second, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target ), true );
                CswNbtViewProperty DueDateVp = RetView.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate ) );
                CswNbtViewProperty StatusVp = RetView.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status ) );
                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + GridViewName, "View creation failed", ex );
            }
            return RetView;
        }

        private void _updateInspectionsGridView( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType RetInspectionTargetNt )
        {
            String GridViewName = RetInspectionTargetNt.NodeTypeName + " Inspections Grid Prop View";
            foreach( CswNbtView View in _CswNbtResources.ViewSelect.restoreViews( GridViewName ) )
            {
                CswNbtViewRelationship TargetVr = View.Root.ChildRelationships[0];
                if( null != TargetVr )
                {
                    CswNbtMetaDataNodeTypeProp DesignTargetNTP = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                    bool AlreadyExists = TargetVr.ChildRelationships.Any( DesignNTRel => DesignNTRel.PropId == DesignTargetNTP.PropId );
                    if( false == AlreadyExists )
                    {
                        CswNbtViewRelationship InspectionVr = View.AddViewRelationship( TargetVr, CswEnumNbtViewPropOwnerType.Second, DesignTargetNTP, true );
                        CswNbtViewProperty DueDateVp = View.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate ) );
                        CswNbtViewProperty StatusVp = View.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status ) );
                        View.save();
                    }
                }
            }
        }

        private CswNbtView _createTargetInspectionsView( CswNbtMetaDataNodeType InspectionDesignNt, string Category, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );

            CswNbtView RetView = null;
            string InspectionTargetViewName = InspectionDesignNt.NodeTypeName + " Inspections: " + InspectionTargetNt.NodeTypeName;
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
                    if( CswEnumNbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.saveNew( InspectionTargetViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.saveNew( InspectionTargetViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }

                    RetView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    CswNbtMetaDataNodeTypeProp ItTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );

                    /* View:
                        [Target]
                            [Inspection]                        
                    */

                    CswNbtViewRelationship TargetRelationship = RetView.AddViewRelationship( InspectionTargetNt, false );
                    RetView.AddViewRelationship( TargetRelationship, CswEnumNbtViewPropOwnerType.Second, ItTargetNtp, false );
                    RetView.save();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + InspectionTargetViewName, "View creation failed", ex );
                }
            }
            RetView.SaveToCache( true );
            return RetView;
        }

        private CswNbtView _createAllInspectionPointsGridView( CswNbtMetaDataNodeType InspectionGroupNt, CswNbtMetaDataNodeType InspectionTargetNt, string Category, CswEnumNbtViewRenderingMode ViewMode,
             string AllInspectionPointsViewName )
        {
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtView RetView = new CswNbtView( _CswNbtResources );

            try
            {
                RetView.saveNew( AllInspectionPointsViewName, CswEnumNbtViewVisibility.Property, null, null, null );
                RetView.Category = Category;
                RetView.ViewMode = ViewMode;

                CswNbtViewRelationship InspectionGroupVr = RetView.AddViewRelationship( InspectionGroupNt, true );

                CswNbtMetaDataNodeTypeProp InspectionGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                CswNbtViewRelationship InspectionTargetVr = RetView.AddViewRelationship( InspectionGroupVr, CswEnumNbtViewPropOwnerType.Second, InspectionGroupNtp, true );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = (CswNbtMetaDataNodeTypeProp) InspectionTargetNt.getBarcodeProperty();
                RetView.AddViewProperty( InspectionTargetVr, BarcodeNtp ).Order = 0;

                CswNbtMetaDataNodeTypeProp DescriptionNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Description );
                RetView.AddViewProperty( InspectionTargetVr, DescriptionNtp ).Order = 1;

                CswNbtMetaDataNodeTypeProp LocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );
                RetView.AddViewProperty( InspectionTargetVr, LocationNtp ).Order = 2;

                //CswNbtMetaDataNodeTypeProp DateNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                //RetView.AddViewProperty( InspectionTargetVr, DateNtp ).Order = 3;

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Status );
                RetView.AddViewProperty( InspectionTargetVr, StatusNtp ).Order = 4;

                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + AllInspectionPointsViewName, "View creation failed", ex );
            }
            return RetView;
        }


        #endregion Views

        /// <summary>
        /// Ensure that Allowed Answers contains all Compliant Answers and that both collections contain only unique answers.
        /// </summary>
        private void _validateAnswers( ref string CompliantAnswersString, ref string AllowedAnswersString, ref string PreferredAnswerString )
        {
            string RetCompliantAnswersString = _DefaultCompliantAnswers;
            string RetAllowedAnswersString = _DefaultAllowedAnswers;

            CswCommaDelimitedString AllowedAnswers = new CswCommaDelimitedString();
            AllowedAnswers.FromString( AllowedAnswersString );

            CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
            CompliantAnswers.FromString( CompliantAnswersString );

            if( false == CompliantAnswers.Contains( PreferredAnswerString, CaseSensitive : false ) )
            {
                PreferredAnswerString = "";
            }

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
        public CswNbtViewId GroupsViewId { get { return ( _groupsViewId ); } }
        public CswNbtViewId InspectionsViewId { get { return ( _inspectionsViewId ); } }
        public CswNbtViewId SchedulingViewId { get { return ( _schedulingViewId ); } }

        private CswCommaDelimitedString _UniqueQuestions = new CswCommaDelimitedString();

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
                    if( false == _UniqueQuestions.Contains( Question, CaseSensitive : false ) )
                    {
                        _UniqueQuestions.Add( Question );
                        if( false == string.IsNullOrEmpty( Question ) )
                        {
                            DataRow NewRow = RetDataTable.NewRow();
                            NewRow[_QuestionName] = Question;

                            string AllowedAnswers = CswConvert.ToString( Row[_AllowedAnswersName] );
                            string ComplaintAnswers = CswConvert.ToString( Row[_CompliantAnswersName] );
                            string PreferredAnswer = CswConvert.ToString( Row[_PreferredAnswer] );
                            _validateAnswers( ref ComplaintAnswers, ref AllowedAnswers, ref PreferredAnswer );

                            NewRow[_AllowedAnswersName] = AllowedAnswers;
                            NewRow[_CompliantAnswersName] = ComplaintAnswers;
                            NewRow[_PreferredAnswer] = PreferredAnswer;
                            NewRow[_HelpTextName] = CswConvert.ToString( Row[_HelpTextName] );

                            string SectionName = _standardizeName( Row[_SectionName] );
                            if( string.Empty == SectionName )
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
            }
            catch( Exception Exception )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }

            return ( RetDataTable );
        }

        public JObject copyInspectionDesign( string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( InspectionDesignName );
            if( null != InspectionDesignNt )
            {
                string CopyInspectionNameOrig = CswTools.makeUniqueCopyName( InspectionDesignName, MaxLength : 50 );
                string CopyInspectionNameFinal = CopyInspectionNameOrig;
                Int32 Iterator = 0;
                while( null != _CswNbtResources.MetaData.getNodeType( CopyInspectionNameFinal ) )
                {
                    Iterator += 1;
                    CopyInspectionNameFinal = CopyInspectionNameOrig + " " + Iterator;
                }
                CswNbtMetaDataNodeType CopiedInspectionDesignNt = _CswNbtResources.MetaData.CopyNodeType( InspectionDesignNt, CopyInspectionNameFinal );

                CswNbtMetaDataNodeType InspectionTargetNt = _confirmInspectionDesignTarget( CopiedInspectionDesignNt, InspectionTargetName, ref Category );
                _setInspectionDesignTabsAndProps( CopiedInspectionDesignNt, InspectionTargetNt );
                _TargetNtId = InspectionTargetNt.FirstVersionNodeTypeId;

                RetObj = _createInspectionDesignViews( Category, CopiedInspectionDesignNt, InspectionTargetNt );
            }
            return RetObj;
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JArray GridArray = JArray.Parse( GridArrayString );
            return ( _createInspectionDesignTabsAndProps( GridArray, InspectionDesignName, InspectionTargetName, Category ) );
        }

        private JObject _createInspectionDesignTabsAndProps( JArray GridArray, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();

            InspectionDesignName = _validateNodeTypeName( InspectionDesignName );

            if( null == GridArray || GridArray.Count == 0 )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot create Inspection Design " + InspectionDesignName + ", because the import contained no questions.", "GridArray was null or empty." );
            }

            Int32 TotalRows = GridArray.Count;

            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.makeNewNodeType( CswEnumNbtObjectClass.InspectionDesignClass.ToString(), InspectionDesignName, string.Empty );
            _setNodeTypePermissions( InspectionDesignNt );

            //Get distinct tabs
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = _getTabsForInspection( GridArray, InspectionDesignNt );

            //Create the props
            Int32 PropsWithoutError = _createInspectionProps( GridArray, InspectionDesignNt, Tabs, GridRowsSkipped );

            //Build the MetaData
            CswNbtMetaDataNodeType InspectionTargetNt = _confirmInspectionDesignTarget( InspectionDesignNt, InspectionTargetName, ref Category );
            _setInspectionDesignTabsAndProps( InspectionDesignNt, InspectionTargetNt );
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
