using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Configuration;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Session;
using System.Text.RegularExpressions;
using ChemSW.Security;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Statistics
{
    //Sergei: This class is completely unimplemented because we need
    //to have a generic class called CswStatisticsRecorder that 
    //provides a mechanism for recording arbitrary statistics in a long
    //table, and that then uses ICswSessionStorage. I'm leaving
    //in the commented-out old implementation so we can remember 
    //what we used to do. 
    //--Dimitri
    public class CswNbtStatisticsStorageDb
    {

        CswNbtResources _CswNbtResources = null;
        public CswNbtStatisticsStorageDb( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        //bool load( string SessionId )
        //{
        //    return ( false ); //deliberate policy: we don't _want_ to read stats with this class
        //}//load()

        //DateTime LastSave { get; }


        //public void release()
        //{
        //}//release()


        public void save( CswNbtStatisticsEntry CswNbtStatisticsEntry )
        {
            if ( _CswNbtResources != null )
            {
                if ( CswNbtStatisticsEntry.Stats_servertime_count > 0 ) // protects against repeats
                {
                    CswTableUpdate StatisticsTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics" );
                    DataTable StatisticsTable = StatisticsTableCaddy.getEmptyTable();

                    DataRow NewStatisticsRow = StatisticsTable.NewRow();
                    NewStatisticsRow["userid"] = CswNbtStatisticsEntry.UserId.PrimaryKey;
                    NewStatisticsRow["username"] = CswNbtStatisticsEntry.UserName;
                    NewStatisticsRow["logindate"] = CswNbtStatisticsEntry.LoginDate.ToString();
                    NewStatisticsRow["logoutdate"] = DateTime.Now.ToString();
                    NewStatisticsRow["userloggedout"] = CswConvert.ToDbVal( CswNbtStatisticsEntry.Stats_LoggedOut );

                    // Statistics on Actions
                    NewStatisticsRow["count_actionloads"] = CswNbtStatisticsEntry.Stats_count_actionloads.ToString();
                    NewStatisticsRow["count_multiedit"] = CswNbtStatisticsEntry.Stats_count_multiedit.ToString();
                    NewStatisticsRow["count_reportruns"] = CswNbtStatisticsEntry.Stats_count_reportruns.ToString();

                    // Statistics on Views
                    NewStatisticsRow["count_viewloads"] = CswNbtStatisticsEntry.Stats_count_viewloads.ToString();
                    NewStatisticsRow["count_viewsedited"] = CswNbtStatisticsEntry.Stats_count_viewsedited.ToString();
                    NewStatisticsRow["count_searches"] = CswNbtStatisticsEntry.Stats_count_searches.ToString();
                    NewStatisticsRow["count_viewfiltermods"] = CswNbtStatisticsEntry.Stats_count_viewfiltermod.ToString();

                    // Statistics on Nodes
                    NewStatisticsRow["count_nodessaved"] = CswNbtStatisticsEntry.Stats_count_nodessaved.ToString();
                    NewStatisticsRow["count_nodesadded"] = CswNbtStatisticsEntry.Stats_count_nodesadded.ToString();
                    NewStatisticsRow["count_nodescopied"] = CswNbtStatisticsEntry.Stats_count_nodescopied.ToString();
                    NewStatisticsRow["count_nodesdeleted"] = CswNbtStatisticsEntry.Stats_count_nodesdeleted.ToString();

                    // Statistics on Page Lifecycle
                    NewStatisticsRow["count_lifecycles"] = CswNbtStatisticsEntry.Stats_servertime_count.ToString();
                    NewStatisticsRow["average_servertime"] = ( CswNbtStatisticsEntry.Stats_servertime_total / CswNbtStatisticsEntry.Stats_servertime_count ).ToString();
                    NewStatisticsRow["count_errors"] = CswNbtStatisticsEntry.Stats_errors.ToString();

                    StatisticsTable.Rows.Add( NewStatisticsRow );
                    Int32 StatisticsId = CswConvert.ToInt32( NewStatisticsRow["statisticsid"] );
                    StatisticsTableCaddy.update( StatisticsTable );

                    // Store views used:
                    CswTableUpdate StatisticsViewsTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics_Views" );
                    DataTable StatisticsViewsTable = StatisticsViewsTableCaddy.getEmptyTable();
                    foreach ( string ViewId in CswNbtStatisticsEntry.ViewsEdited.Keys )
                    {
						_makeNewStatisticsViewsRow( StatisticsViewsTable, StatisticsId, new CswNbtViewId( CswConvert.ToInt32( ViewId ) ), CswConvert.ToInt32( CswNbtStatisticsEntry.ViewsEdited[ViewId] ), "edit" );
                    }
                    foreach ( string ViewId in CswNbtStatisticsEntry.ViewsMultiEdited.Keys )
                    {
						_makeNewStatisticsViewsRow( StatisticsViewsTable, StatisticsId, new CswNbtViewId( CswConvert.ToInt32( ViewId ) ), CswConvert.ToInt32( CswNbtStatisticsEntry.ViewsMultiEdited[ViewId] ), "multiedit" );
                    }
                    foreach ( string ViewId in CswNbtStatisticsEntry.ViewsLoaded.Keys )
                    {
						_makeNewStatisticsViewsRow( StatisticsViewsTable, StatisticsId, new CswNbtViewId( CswConvert.ToInt32( ViewId ) ), CswConvert.ToInt32( CswNbtStatisticsEntry.ViewsLoaded[ViewId] ), "load" );
                    }
                    StatisticsViewsTableCaddy.update( StatisticsViewsTable );


                    // Store search properties and View Filter Modifications
                    CswTableUpdate StatisticsSearchesTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics_Searches" );
                    DataTable StatisticsSearchesTable = StatisticsSearchesTableCaddy.getEmptyTable();
                    foreach ( string NodeTypePropId in CswNbtStatisticsEntry.NodeTypePropsSearched.Keys )
                    {
                        _makeNewStatisticsSearchesRow( StatisticsSearchesTable, StatisticsId, CswConvert.ToInt32( NodeTypePropId ), Int32.MinValue, CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypePropsSearched[NodeTypePropId] ), "load" );
                    }
                    foreach ( string ObjectClassPropId in CswNbtStatisticsEntry.ObjectClassPropsSearched.Keys )
                    {
                        _makeNewStatisticsSearchesRow( StatisticsSearchesTable, StatisticsId, Int32.MinValue, CswConvert.ToInt32( ObjectClassPropId ), CswConvert.ToInt32( CswNbtStatisticsEntry.ObjectClassPropsSearched[ObjectClassPropId] ), "load" );
                    }
                    foreach ( string NodeTypePropId in CswNbtStatisticsEntry.NodeTypePropsFilterMod.Keys )
                    {
                        _makeNewStatisticsSearchesRow( StatisticsSearchesTable, StatisticsId, CswConvert.ToInt32( NodeTypePropId ), Int32.MinValue, CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypePropsFilterMod[NodeTypePropId] ), "modify" );
                    }
                    foreach ( string ObjectClassPropId in CswNbtStatisticsEntry.ObjectClassPropsFilterMod.Keys )
                    {
                        _makeNewStatisticsSearchesRow( StatisticsSearchesTable, StatisticsId, Int32.MinValue, CswConvert.ToInt32( ObjectClassPropId ), CswConvert.ToInt32( CswNbtStatisticsEntry.ObjectClassPropsFilterMod[ObjectClassPropId] ), "modify" );
                    }
                    StatisticsSearchesTableCaddy.update( StatisticsSearchesTable );


                    // Store node object classes:
                    CswTableUpdate StatisticsNodeTypesTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics_NodeTypes" );
                    DataTable StatisticsNodeTypesTable = StatisticsNodeTypesTableCaddy.getEmptyTable();
                    foreach ( string NodeTypeId in CswNbtStatisticsEntry.NodeTypesSaved.Keys )
                    {
                        _makeNewStatisticsNodeTypesRow( StatisticsNodeTypesTable, StatisticsId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypesSaved[NodeTypeId] ), "save" );
                    }
                    foreach ( string NodeTypeId in CswNbtStatisticsEntry.NodeTypesCopied.Keys )
                    {
                        _makeNewStatisticsNodeTypesRow( StatisticsNodeTypesTable, StatisticsId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypesCopied[NodeTypeId] ), "copy" );
                    }
                    foreach ( string NodeTypeId in CswNbtStatisticsEntry.NodeTypesDeleted.Keys )
                    {
                        _makeNewStatisticsNodeTypesRow( StatisticsNodeTypesTable, StatisticsId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypesDeleted[NodeTypeId] ), "delete" );
                    }
                    foreach ( string NodeTypeId in CswNbtStatisticsEntry.NodeTypesAdded.Keys )
                    {
                        _makeNewStatisticsNodeTypesRow( StatisticsNodeTypesTable, StatisticsId, CswConvert.ToInt32( NodeTypeId ), CswConvert.ToInt32( CswNbtStatisticsEntry.NodeTypesAdded[NodeTypeId] ), "add" );
                    }
                    StatisticsNodeTypesTableCaddy.update( StatisticsNodeTypesTable );


                    // Store reports:
                    CswTableUpdate StatisticsReportsTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics_Reports" );
                    DataTable StatisticsReportsTable = StatisticsReportsTableCaddy.getEmptyTable();
                    foreach ( string ReportId in CswNbtStatisticsEntry.ReportsLoaded.Keys )
                    {
                        _makeNewStatisticsReportsRow( StatisticsReportsTable, StatisticsId, CswConvert.ToInt32( ReportId ), CswConvert.ToInt32( CswNbtStatisticsEntry.ReportsLoaded[ReportId] ), "load" );
                    }
                    StatisticsReportsTableCaddy.update( StatisticsReportsTable );

                    // Store actions:
                    CswTableUpdate StatisticsActionsTableCaddy = _CswNbtResources.makeCswTableUpdate( "CswNbtStatisticsEntryNbt", "Statistics_Actions" );
                    DataTable StatisticsActionsTable = StatisticsActionsTableCaddy.getEmptyTable();
                    foreach ( string ActionId in CswNbtStatisticsEntry.ActionsLoaded.Keys )
                    {
                        _makeNewStatisticsActionsRow( StatisticsActionsTable, StatisticsId, CswConvert.ToInt32( ActionId ), CswConvert.ToInt32( CswNbtStatisticsEntry.ActionsLoaded[ActionId] ), "load" );
                    }
                    StatisticsActionsTableCaddy.update( StatisticsActionsTable );

                    CswNbtStatisticsEntry.ClearStatistics();

                }
            }
        }

        private void _makeNewStatisticsViewsRow( DataTable Table, Int32 StatisticsId, CswNbtViewId ViewId, Int32 HitCount, string Action )
        {
            DataRow Row = Table.NewRow();
            Row["statisticsid"] = CswConvert.ToDbVal( StatisticsId );
            Row["nodeviewid"] = CswConvert.ToDbVal( ViewId.get() );
			CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( ViewId );
            if ( ThisView != null )
                Row["viewname"] = ThisView.ViewName;
            Row["hitcount"] = CswConvert.ToDbVal( HitCount );
            Row["action"] = Action;
            Table.Rows.Add( Row );
        }

        private void _makeNewStatisticsNodeTypesRow( DataTable Table, Int32 StatisticsId, Int32 NodeTypeId, Int32 HitCount, string Action )
        {
            DataRow Row = Table.NewRow();
            Row["statisticsid"] = CswConvert.ToDbVal( StatisticsId );
            Row["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
            Row["nodetypename"] = _CswNbtResources.MetaData.getNodeType( NodeTypeId ).NodeTypeName;
            Row["hitcount"] = CswConvert.ToDbVal( HitCount );
            Row["action"] = Action;
            Table.Rows.Add( Row );
        }

        private void _makeNewStatisticsReportsRow( DataTable Table, Int32 StatisticsId, Int32 ReportId, Int32 HitCount, string Action )
        {
            DataRow Row = Table.NewRow();
            Row["statisticsid"] = CswConvert.ToDbVal( StatisticsId );
            Row["reportid"] = CswConvert.ToDbVal( ReportId );

            //Sergei:
            //This old statement
            //
            //Row["reportname"] = _CswNbtResources.Nodes[ReportId].NodeName; 
            //
            //does not compile because nodes is keyed on CswPrimaryKey. I don't know
            //whether the way I'm constructing the CswPrimeKey is ok
            //--Dimitri
            Row["reportname"] = _CswNbtResources.Nodes[new CswPrimaryKey( "", ReportId )].NodeName;
            Row["hitcount"] = CswConvert.ToDbVal( HitCount );
            Row["action"] = Action;
            Table.Rows.Add( Row );
        }

        private void _makeNewStatisticsActionsRow( DataTable Table, Int32 StatisticsId, Int32 ActionId, Int32 HitCount, string Action )
        {
            DataRow Row = Table.NewRow();
            Row["statisticsid"] = CswConvert.ToDbVal( StatisticsId );
            Row["actionid"] = ActionId.ToString();
            Row["actionname"] = _CswNbtResources.Actions[ActionId].Name;
            Row["hitcount"] = CswConvert.ToDbVal( HitCount );
            Row["action"] = Action;
            Table.Rows.Add( Row );
        }

        private void _makeNewStatisticsSearchesRow( DataTable Table, Int32 StatisticsId, Int32 NodeTypePropId, Int32 ObjectClassPropId, Int32 HitCount, string Action )
        {
            DataRow Row = Table.NewRow();
            Row["statisticsid"] = CswConvert.ToDbVal( StatisticsId );
            if ( NodeTypePropId > 0 )
            {
                Row["nodetypepropid"] = CswConvert.ToDbVal( NodeTypePropId );
                Row["propname"] = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId ).PropName;
            }
            else
            {
                Row["objectclasspropid"] = CswConvert.ToDbVal( ObjectClassPropId );
                Row["propname"] = _CswNbtResources.MetaData.getObjectClassProp( ObjectClassPropId ).PropName;
            }
            Row["hitcount"] = CswConvert.ToDbVal( HitCount );
            Row["action"] = Action;
            Table.Rows.Add( Row );
        }


    }//CswStatisticsNbt

}//ChemSW.Nbt
