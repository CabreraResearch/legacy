using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageTable
    {

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Types of LandingPage Page Components
        /// </summary>
        public enum LandingPageItemType
        {
            /// <summary>
            /// Link to a View, Report, or Action
            /// </summary>
            Link,
            /// <summary>
            /// Static text
            /// </summary>
            Text,
            /// <summary>
            /// Link for Add New Node dialog
            /// </summary>
            Add
        }

        public CswNbtLandingPageTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public DataTable getLandingPageTable( Int32 RoleId )
        {
            CswTableSelect LandingPageSelect = _CswNbtResources.makeCswTableSelect( "LandingPageSelect", "landingpage" );
            string WhereClause = "where for_roleid = '" + RoleId.ToString() + "'";
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "landingpageid", OrderByType.Ascending ) );
            DataTable LandingPageTable = LandingPageSelect.getTable( WhereClause, OrderBy );
            return LandingPageTable;
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

            Dictionary<CswNbtViewId, CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( false );
            foreach( CswNbtView View in Views.Values )
            {
                if( View.ViewName == "All Equipment" )
                    EquipmentByTypeViewId = View.ViewId;
                if( View.ViewName == "Tasks: Open" )
                    TasksOpenViewId = View.ViewId;
                if( View.ViewName == "Problems: Open" )
                    ProblemsOpenViewId = View.ViewId;
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
            if( EquipmentNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Add, CswNbtView.ViewType.View, string.Empty, EquipmentNodeTypeId, string.Empty, RoleId, 5, 1 );
            if( EquipmentByTypeViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Link, CswNbtView.ViewType.View, EquipmentByTypeViewId.ToString(), Int32.MinValue, "All Equipment", RoleId, 7, 1 );

            // Problems
            if( ProblemsOpenViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Link, CswNbtView.ViewType.View, ProblemsOpenViewId.ToString(), Int32.MinValue, "Problems", RoleId, 1, 3, "warning.gif" );
            if( ProblemNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Add, CswNbtView.ViewType.View, string.Empty, ProblemNodeTypeId, "Add New Problem", RoleId, 5, 3 );

            // Schedules and Tasks
            if( TasksOpenViewId.isSet() )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Link, CswNbtView.ViewType.View, TasksOpenViewId.ToString(), Int32.MinValue, "Tasks", RoleId, 1, 5, "clipboard.gif" );
            if( TaskNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Add, CswNbtView.ViewType.View, string.Empty, TaskNodeTypeId, "Add New Task", RoleId, 5, 5 );
            if( ScheduleNodeTypeId != Int32.MinValue )
                _AddLandingPageItem( LandingPageTable, LandingPageItemType.Add, CswNbtView.ViewType.View, string.Empty, ScheduleNodeTypeId, "Scheduling", RoleId, 7, 5 );

            LandingPageUpdate.update( LandingPageTable );
        } // ResetLandingPageItems()

        /// <summary>
        /// Adds a LandingPage component to the LandingPage page
        /// </summary>
        public void addLandingPageItem( string Type, string ViewType, string PkValue, Int32 NodeTypeId, string DisplayText, string strRoleId )
        {
            if( strRoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                strRoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;
            LandingPageItemType itemType;
            Enum.TryParse( Type, true, out itemType );
            CswNbtView.ViewType RealViewType = (CswNbtView.ViewType) ViewType;

            CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "AddLandingPageItem_Update", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getEmptyTable();
            _AddLandingPageItem( LandingPageTable, itemType, RealViewType, PkValue, NodeTypeId, DisplayText, RoleId );
            LandingPageUpdate.update( LandingPageTable );
        }

        private void _AddLandingPageItem( DataTable LandingPageTable, LandingPageItemType itemType, CswNbtView.ViewType ViewType, string PkValue,
                                          Int32 NodeTypeId, string DisplayText, Int32 RoleId,
                                          Int32 Row = Int32.MinValue, Int32 Column = Int32.MinValue, string ButtonIcon = "" )
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
            NewLandingPageRow["componenttype"] = itemType.ToString();
            NewLandingPageRow["display_col"] = Column;
            NewLandingPageRow["display_row"] = Row;

            switch( itemType )
            {
                case LandingPageItemType.Add:
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewLandingPageRow["to_nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                        NewLandingPageRow["buttonicon"] = ButtonIcon;
                        NewLandingPageRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add LandingPage Page Component" );
                    break;
                case LandingPageItemType.Link:
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
                case LandingPageItemType.Text:
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

        public void moveLandingPageItem( Int32 LandingPageId, Int32 NewRow, Int32 NewColumn )
        {
            if( LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "MoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    DataRow LandingPageRow = LandingPageTable.Rows[0];
                    LandingPageRow["display_row"] = CswConvert.ToDbVal( NewRow );
                    LandingPageRow["display_col"] = CswConvert.ToDbVal( NewColumn );
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }

        public void deleteLandingPageItem( Int32 LandingPageId )
        {
            if( LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "RemoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    foreach( DataRow LandingPageRow in LandingPageTable.Rows )
                    {
                        LandingPageRow.Delete();
                    }
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }
    }
}
