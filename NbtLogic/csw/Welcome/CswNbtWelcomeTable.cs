using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Welcome
{
    public class CswNbtWelcomeTable
    {

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Types of Welcome Page Components
        /// </summary>
        public enum WelcomeComponentType
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

        public CswNbtWelcomeTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void ResetWelcomeItems( string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // Reset the contents for this role to factory default
            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateReset", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "roleid", RoleId );
            for( Int32 i = 0; i < WelcomeTable.Rows.Count; i++ )
            {
                WelcomeTable.Rows[i].Delete();
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
            //    _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Search, CswNbtView.ViewType.View, FindEquipmentViewId.ToString(), Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
            if( EquipmentNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswNbtView.ViewType.View, string.Empty, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
            if( EquipmentByTypeViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswNbtView.ViewType.View, EquipmentByTypeViewId.ToString(), Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

            // Problems
            if( ProblemsOpenViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswNbtView.ViewType.View, ProblemsOpenViewId.ToString(), Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
            if( ProblemNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswNbtView.ViewType.View, string.Empty, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

            // Schedules and Tasks
            if( TasksOpenViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswNbtView.ViewType.View, TasksOpenViewId.ToString(), Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
            if( TaskNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswNbtView.ViewType.View, string.Empty, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
            if( ScheduleNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswNbtView.ViewType.View, string.Empty, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // ResetWelcomeItems()

        /// <summary>
        /// Adds a welcome component to the welcome page
        /// </summary>
        public void AddWelcomeItem( WelcomeComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getEmptyTable();

            _AddWelcomeItem( WelcomeTable, ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // AddWelcomeItem()

        private void _AddWelcomeItem( DataTable WelcomeTable, WelcomeComponentType ComponentType, CswNbtView.ViewType ViewType, string PkValue,
                                      Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, Int32 RoleId )
        {
            if( Row == Int32.MinValue )
            {
                string SqlText = @"select max(display_row) maxcol
                                     from welcome
                                    where display_col = 1
                                      and (roleid = " + RoleId.ToString() + @")";
                CswArbitrarySelect WelcomeSelect = _CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_WelcomeSelect", SqlText );
                DataTable WelcomeSelectTable = WelcomeSelect.getTable();
                Int32 MaxRow = 0;
                if( WelcomeSelectTable.Rows.Count > 0 )
                {
                    MaxRow = CswConvert.ToInt32( WelcomeSelectTable.Rows[0]["maxcol"] );
                    if( MaxRow < 0 ) MaxRow = 0;
                }
                Row = MaxRow + 1;
                Column = 1;
            }

            if( ButtonIcon == "blank.gif" )
                ButtonIcon = string.Empty;

            DataRow NewWelcomeRow = WelcomeTable.NewRow();
            NewWelcomeRow["roleid"] = RoleId;
            NewWelcomeRow["componenttype"] = ComponentType.ToString();
            NewWelcomeRow["display_col"] = Column;
            NewWelcomeRow["display_row"] = Row;

            switch( ComponentType )
            {
                case WelcomeComponentType.Add:
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                        NewWelcomeRow["buttonicon"] = ButtonIcon;
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add Welcome Page Component" );
                    break;
                case WelcomeComponentType.Link:
                    if( ViewType == CswNbtView.ViewType.View )
                    {
                        NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( PkValue ).get() );
                    }
                    else if( ViewType == CswNbtView.ViewType.Action )
                    {
                        NewWelcomeRow["actionid"] = CswConvert.ToDbVal( PkValue );
                    }
                    else if( ViewType == CswNbtView.ViewType.Report )
                    {
                        CswPrimaryKey ReportPk = new CswPrimaryKey();
                        ReportPk.FromString( PkValue );
                        Int32 PkVal = ReportPk.PrimaryKey;
                        NewWelcomeRow["reportid"] = CswConvert.ToDbVal( PkVal );
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link Welcome Page Component" );
                    }
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case WelcomeComponentType.Text:
                    if( DisplayText != string.Empty )
                    {
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text Welcome Page Component" );
                    break;
            }
            WelcomeTable.Rows.Add( NewWelcomeRow );

        } // _AddWelcomeItem()


        public bool MoveWelcomeItems( string strRoleId, Int32 WelcomeId, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            if( WelcomeId != Int32.MinValue )
            {
                CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
                DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", WelcomeId );
                if( WelcomeTable.Rows.Count > 0 )
                {
                    DataRow WelcomeRow = WelcomeTable.Rows[0];
                    WelcomeRow["display_row"] = CswConvert.ToDbVal( NewRow );
                    WelcomeRow["display_col"] = CswConvert.ToDbVal( NewColumn );
                    WelcomeUpdate.update( WelcomeTable );
                    ret = true;
                }
            } // if( WelcomeId != Int32.MinValue ) 

            return ret;
        } // MoveWelcomeItems

        private bool DeleteTheWelcomeItems( Int32 Id, string colName )
        {
            bool ret = false;

            if( Id != Int32.MinValue )
            {
                CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
                DataTable WelcomeTable = WelcomeUpdate.getTable( colName, Id );
                if( WelcomeTable.Rows.Count > 0 )
                {
                    foreach( DataRow WelcomeRow in WelcomeTable.Rows )
                    {
                        WelcomeRow.Delete();
                    }
                    WelcomeUpdate.update( WelcomeTable );
                    ret = true;
                }
            } // if( Id != Int32.MinValue ) 

            return ret;
        }

        public bool DeleteAllWelcomeItemsForRole( string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            Int32 Id = Int32.MinValue;
            if( strRoleId != String.Empty )
            {
                RolePk.FromString( strRoleId );
                Id = RolePk.PrimaryKey;
            }
            return DeleteTheWelcomeItems( Id, "roleid" );
        }

        public bool DeleteWelcomeItem( string strRoleId, Int32 WelcomeId )
        {
            return DeleteTheWelcomeItems( WelcomeId, "welcomeid" );
        } // MoveWelcomeItems


    }
}
