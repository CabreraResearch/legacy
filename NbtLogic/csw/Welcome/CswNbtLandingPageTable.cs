using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageTable
    {

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Types of LandingPage Page Components
        /// </summary>
        public enum LandingPageComponentType
        {
            /// <summary>
            /// Link to a View, Report, or Action
            /// </summary>
            Link,
            /// <summary>
            /// Search on a View (deprecated)
            /// </summary>
            Search,
            /// <summary>
            /// Static text
            /// </summary>
            Text,
            /// <summary>
            /// Add link for new node
            /// </summary>
            Add
        }

        /// <summary>
        /// Folder Path for Button Images
        /// </summary>
        public static string IconImageRoot = "Images/biggerbuttons";

        public CswNbtLandingPageTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void ResetLandingPageItems( string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // Reset the contents for this role to factory default
            CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "LandingPageUpdateReset", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getTable( "for_roleid", RoleId );
            for( Int32 i = 0; i < LandingPageTable.Rows.Count; i++ )
            {
                LandingPageTable.Rows[i].Delete();
            }

            CswNbtViewId EquipmentByTypeViewId = new CswNbtViewId();
            CswNbtViewId TasksOpenViewId = new CswNbtViewId();
            CswNbtViewId ProblemsOpenViewId = new CswNbtViewId();
            CswNbtViewId FindEquipmentViewId = new CswNbtViewId();

            Dictionary<CswNbtViewId, CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( false );
            foreach( CswNbtView View in Views.Values )
            {
                if( View.ViewName == "All Equipment" )
                    EquipmentByTypeViewId = View.ViewId;
                if( View.ViewName == "Tasks: Open" )
                    TasksOpenViewId = View.ViewId;
                if( View.ViewName == "Problems: Open" )
                    ProblemsOpenViewId = View.ViewId;
                if( View.ViewName == "Find Equipment" )
                    FindEquipmentViewId = View.ViewId;
            }

            Int32 ProblemNodeTypeId = Int32.MinValue;
            Int32 TaskNodeTypeId = Int32.MinValue;
            Int32 ScheduleNodeTypeId = Int32.MinValue;
            Int32 EquipmentNodeTypeId = Int32.MinValue;
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
            {
                string NodeTypeName = NodeType.NodeTypeName;
                Int32 FirstVersionNTId = NodeType.FirstVersionNodeTypeId;
                if( NodeTypeName == "Equipment Problem" )
                {
                    ProblemNodeTypeId = FirstVersionNTId;
                }
                else if( NodeTypeName == "Equipment Task" )
                {
                    TaskNodeTypeId = FirstVersionNTId;
                }
                else if( NodeTypeName == "Equipment Schedule" )
                {
                    ScheduleNodeTypeId = FirstVersionNTId;
                }
                else if( NodeTypeName == "Equipment" )
                {
                    EquipmentNodeTypeId = FirstVersionNTId;
                }
            }

            // Equipment
            //if( FindEquipmentViewId.isSet() )
            //    _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Search, CswNbtView.ViewType.View, FindEquipmentViewId.ToString(), Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
            if( EquipmentNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Add, CswNbtView.ViewType.View, string.Empty, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
            if( EquipmentByTypeViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Link, CswNbtView.ViewType.View, EquipmentByTypeViewId.ToString(), Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

            // Problems
            if( ProblemsOpenViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Link, CswNbtView.ViewType.View, ProblemsOpenViewId.ToString(), Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
            if( ProblemNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Add, CswNbtView.ViewType.View, string.Empty, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

            // Schedules and Tasks
            if( TasksOpenViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Link, CswNbtView.ViewType.View, TasksOpenViewId.ToString(), Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
            if( TaskNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Add, CswNbtView.ViewType.View, string.Empty, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
            if( ScheduleNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageComponentType.Add, CswNbtView.ViewType.View, string.Empty, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

            LandingPageUpdate.update( LandingPageTable );
        } // ResetLandingPageItems()

        /// <summary>
        /// Adds a LandingPage component to the LandingPage page
        /// </summary>
        public void AddLandingPageItem( LandingPageComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getEmptyTable();

            _AddLandingPageItem( LandingPageTable, ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, RoleId );

            LandingPageUpdate.update( LandingPageTable );
        } // AddLandingPageItem()

        private void _AddLandingPageItem( DataTable LandingPageTable, LandingPageComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                      Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, Int32 RoleId )
        {
            if( Row == Int32.MinValue )
            {
                string SqlText = @"select max(display_row) maxcol
                                     from landingpage
                                    where display_col = 1
                                      and (for_roleid = " + RoleId.ToString() + @")";
                CswArbitrarySelect LandingPageSelect = _CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_LandingPageSelect", SqlText );
                DataTable LandingPageSelectTable = LandingPageSelect.getTable();
                Int32 MaxRow = 0;
                if( LandingPageSelectTable.Rows.Count > 0 )
                {
                    MaxRow = CswConvert.ToInt32( LandingPageSelectTable.Rows[0]["maxcol"] );
                    if( MaxRow < 0 ) MaxRow = 0;
                }
                Row = MaxRow + 1;
                Column = 1;
            }

            if( ButtonIcon == "blank.gif" )
                ButtonIcon = string.Empty;

            DataRow NewLandingPageRow = LandingPageTable.NewRow();
            NewLandingPageRow["for_roleid"] = RoleId;
            NewLandingPageRow["componenttype"] = ComponentType.ToString();
            NewLandingPageRow["display_col"] = Column;
            NewLandingPageRow["display_row"] = Row;

            switch( ComponentType )
            {
                case LandingPageComponentType.Add:
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewLandingPageRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                        NewLandingPageRow["buttonicon"] = ButtonIcon;
                        NewLandingPageRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add LandingPage Page Component" );
                    break;
                case LandingPageComponentType.Link:
                    if( ViewType == CswNbtView.ViewType.View )
                    {
                        NewLandingPageRow["to_nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( PkValue ).get() );
                    }
                    else if( ViewType == CswNbtView.ViewType.Action )
                    {
                        NewLandingPageRow["to_actionid"] = CswConvert.ToDbVal( PkValue );
                    }
                    else if( ViewType == CswNbtView.ViewType.Report )
                    {
                        CswPrimaryKey ReportPk = new CswPrimaryKey();
                        ReportPk.FromString( PkValue );
                        Int32 PkVal = ReportPk.PrimaryKey;
                        NewLandingPageRow["to_reportid"] = CswConvert.ToDbVal( PkVal );
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link LandingPage Page Component" );
                    }
                    NewLandingPageRow["buttonicon"] = ButtonIcon;
                    NewLandingPageRow["displaytext"] = DisplayText;
                    break;
                case LandingPageComponentType.Text:
                    if( DisplayText != string.Empty )
                    {
                        NewLandingPageRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text LandingPage Page Component" );
                    break;
            }
            LandingPageTable.Rows.Add( NewLandingPageRow );

        } // _AddLandingPageItem()


        public bool MoveLandingPageItems( string strRoleId, Int32 LandingPageId, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            if( LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    DataRow LandingPageRow = LandingPageTable.Rows[0];
                    LandingPageRow["display_row"] = CswConvert.ToDbVal( NewRow );
                    LandingPageRow["display_col"] = CswConvert.ToDbVal( NewColumn );
                    LandingPageUpdate.update( LandingPageTable );
                    ret = true;
                }
            } // if( LandingPageId != Int32.MinValue ) 

            return ret;
        } // MoveLandingPageItems

        private bool DeleteTheLandingPageItems( Int32 Id, string colName )
        {
            bool ret = false;

            if( Id != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( colName, Id );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    foreach( DataRow LandingPageRow in LandingPageTable.Rows )
                    {
                        LandingPageRow.Delete();
                    }
                    LandingPageUpdate.update( LandingPageTable );
                    ret = true;
                }
            }

            return ret;
        }

        public bool DeleteAllLandingPageItemsForRole( string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            Int32 Id = Int32.MinValue;
            if( strRoleId != String.Empty )
            {
                RolePk.FromString( strRoleId );
                Id = RolePk.PrimaryKey;
            }
            return DeleteTheLandingPageItems( Id, "for_roleid" );
        }

        public bool DeleteLandingPageItem( string strRoleId, Int32 LandingPageId )
        {
            return DeleteTheLandingPageItems( LandingPageId, "landingpageid" );
        }
    }
}
