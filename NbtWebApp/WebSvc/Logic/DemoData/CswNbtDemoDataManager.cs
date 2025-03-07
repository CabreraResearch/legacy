using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Grid;
//using ChemSW.Nbt.NbtWebSvcSchedService;


namespace ChemSW.Nbt.WebServices
{
    public class CswNbtDemoDataManager
    {
        #region ctor

        private readonly CswNbtResources _CswNbtResources;

        public CswNbtDemoDataManager( CswNbtResources NbtManagerResources )
        {
            _CswNbtResources = NbtManagerResources;
        } //ctor

        #endregion ctor

        #region private

        /// TO DO: cover our privates

        #endregion private

        #region public


        private static Dictionary<string, Type> _ColumnTypeOverrides = new Dictionary<string, Type>();
        public static void getDemoDataGrid( ICswResources CswResources, CswNbtDemoDataReturn Return, object Request )
        {

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;



            //Build table infrastructure
            DataTable GridTable = new DataTable( "demodatatable" );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.NodeId, typeof( Int32 ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Name, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Type, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsUsedBy, typeof( int ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsRequiredBy, typeof( int ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.ConvertToNonDemo, typeof( Boolean ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Delete, typeof( Boolean ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.MenuOptions, typeof( string ) );


            _ColumnTypeOverrides.Clear();
            _ColumnTypeOverrides.Add( CswNbtDemoDataReturn.ColumnNames.IsUsedBy.ToString(), typeof( sbyte ) );
            _ColumnTypeOverrides.Add( CswNbtDemoDataReturn.ColumnNames.IsRequiredBy.ToString(), typeof( sbyte ) );

            //*****************************
            //Populate views
            string ViewQuery = @"select v.viewname, v.nodeviewid
                                  from node_views v
                                 where isdemo = '1'
                                   and visibility <> 'Property'
                                   and visibility <> 'Hidden'
                                 order by lower(viewname)";

            CswArbitrarySelect ArbitraryViewsSelect = CswNbtResources.makeCswArbitrarySelect( "select_demo_nodes", ViewQuery );
            DataTable DemoViewsTable = ArbitraryViewsSelect.getTable();
            foreach( DataRow CurrentDemoViewRow in DemoViewsTable.Rows )
            {
                DataRow NewGridRowOfDemoViews = GridTable.NewRow();
                GridTable.Rows.Add( NewGridRowOfDemoViews );
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.NodeId] = CurrentDemoViewRow["nodeviewid"].ToString();
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.Name] = CurrentDemoViewRow["viewname"].ToString();
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.Type] = "View";
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = 0;
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = 0;
            } //iterate demo views rows


            //*****************************
            //Populate views
            string NodesQuery = @"select n." + CswNbtDemoDataReturn.ColumnNames.NodeId + @",n.nodename,t.nodetypename, t.enabled
                                                from nodes n 
                                                join nodetypes t on (n.nodetypeid=t.nodetypeid )
                                                where n.isdemo = '1'
                                                order by lower( n.nodename ), lower( t.nodetypename )";




            CswArbitrarySelect ArbitraryNodesSelect = CswNbtResources.makeCswArbitrarySelect( "select_demo_nodes", NodesQuery );
            DataTable DemoNodesTable = ArbitraryNodesSelect.getTable();
            foreach( DataRow CurrentDemoNodeRow in DemoNodesTable.Rows )
            {
                bool NodeEnabled = CswConvert.ToBoolean( CurrentDemoNodeRow["enabled"].ToString() );
                String DisabledLabel = NodeEnabled ? "" : "(Disabled) ";

                DataRow NewGridRowOfNodes = GridTable.NewRow();
                GridTable.Rows.Add( NewGridRowOfNodes );
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.NodeId] = CurrentDemoNodeRow["nodeid"].ToString();
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.Name] = CurrentDemoNodeRow["nodename"].ToString();
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.Type] = DisabledLabel + CurrentDemoNodeRow["nodetypename"].ToString();

                CswDelimitedString UsedByNodeIds = new CswDelimitedString( ',' );
                Int32 UsedByCount = 0;

                CswDelimitedString RequiredByNodeIds = new CswDelimitedString( ',' );
                Int32 RequiredByCount = 0;


                string nodeid = CurrentDemoNodeRow["nodeid"].ToString();
                string node_used_by_query = @"select n." + CswNbtDemoDataReturn.ColumnNames.NodeId + @", n.nodename,t.nodetypename,n.isdemo, p.isrequired 
                                                from jct_nodes_props j 
                                                join nodetype_props p on (j.nodetypepropid=p.nodetypepropid)
                                                join field_types f on ( p.fieldtypeid = f.fieldtypeid )
                                                join nodes n on (j.nodeid=n.nodeid) 
                                                join nodetypes t on (n.nodetypeid=t.nodetypeid)
                                                where ( f.fieldtype='Relationship' or f.fieldtype='Location' )
                                                and j.field1_fk='" + nodeid + "'";

                CswArbitrarySelect ArbitraryUsedBySelect = CswNbtResources.makeCswArbitrarySelect( "select_nodesusedby_nodeid_" + nodeid, node_used_by_query );
                DataTable NodesUsedByTable = ArbitraryUsedBySelect.getTable();


                foreach( DataRow CurrentUsedByRow in NodesUsedByTable.Rows )
                {
                    string CurrentNodeId = CurrentUsedByRow["nodeid"].ToString();
                    if( true == CswConvert.ToBoolean( CurrentUsedByRow["isrequired"].ToString() ) )
                    {
                        RequiredByNodeIds.Add( CurrentNodeId );
                        RequiredByCount++;
                    }
                    else
                    {
                        UsedByNodeIds.Add( CurrentNodeId );
                        UsedByCount++;
                    } //if-else it's required

                } //iterate nodes used by rows


                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.MenuOptions] = "{ \"requiredby\" : [" + RequiredByNodeIds.ToString() + "],\"usedby\" :[" + UsedByNodeIds.ToString() + "], \"nodename\": \" " + CurrentDemoNodeRow["nodename"].ToString() + "\" }";
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = UsedByCount;
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = RequiredByCount;

            } //iterate node rows



            CswNbtGrid Grid = new CswNbtGrid( CswNbtResources );
            Return.Data.Grid = Grid.DataTableToGrid( GridTable, IncludeEditFields: false, ColumnTypeOverrides: _ColumnTypeOverrides );


        } //getDemoDataGrid()

        public static void getDemoDataNodesAsGrid( ICswResources CswResources, CswNbtDemoDataReturn Return, CswNbtDemoDataRequests.CswDemoNodesGridRequest Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            //Build table infrastructure
            DataTable GridTable = new DataTable( "depdendentnodestable" );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.NodeId, typeof( Int32 ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Name, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Type, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsDemo, typeof( string ) );
            //            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Action, typeof( sbyte ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.MenuOptions, typeof( string ) );


            string DependentNodesQuery = "select n.nodeid, t.nodetypeid, n.nodename as \"name\" ,n.isdemo \"Is Demo\", t.nodetypename as \"type\" ";
            DependentNodesQuery += "from nodes n ";
            DependentNodesQuery += "join nodetypes t on (n.nodetypeid=t.nodetypeid) ";



            //We have to cope with oracle's limit of 1000 on lists
            //as per case 29811
            DependentNodesQuery += " where ";
            List<string> AllNodeIds = Request.NodeIds;
            int NumberoFInClauses = ( AllNodeIds.Count / 1000 ) + 1; //we _only_ want the whole-number result here
            for( int idx = 0; idx < NumberoFInClauses; idx++ )
            {

                if( idx > 0 )
                {
                    DependentNodesQuery += " or ";
                }

                List<string> CurrentList = null;
                if( AllNodeIds.Count > 1000 )
                {
                    CurrentList = AllNodeIds.GetRange( 0, 1000 );
                    AllNodeIds.RemoveRange( 0, 1000 );
                }
                else
                {
                    CurrentList = AllNodeIds;
                }

                CswDelimitedString DepdendentNodeIds = new CswDelimitedString( ',' );
                DepdendentNodeIds.FromArray( CurrentList.ToArray() );
                DependentNodesQuery += " n.nodeid in (" + DepdendentNodeIds.ToString() + ") ";
            }




            DependentNodesQuery += "order by lower(n.nodename), lower(t.nodetypename)";

            CswArbitrarySelect DependentNodesSelect = CswNbtResources.makeCswArbitrarySelect( "select_depdendent_nodes", DependentNodesQuery );
            DataTable DepdendentNodesTableTable = DependentNodesSelect.getTable();
            foreach( DataRow CurrentDependentNodeRow in DepdendentNodesTableTable.Rows )
            {
                DataRow NewGridRowOfDependentNodes = GridTable.NewRow();
                GridTable.Rows.Add( NewGridRowOfDependentNodes );

                NewGridRowOfDependentNodes[CswNbtDemoDataReturn.ColumnNames.NodeId] = CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.NodeId];
                NewGridRowOfDependentNodes[CswNbtDemoDataReturn.ColumnNames.Name] = CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.Name];
                NewGridRowOfDependentNodes[CswNbtDemoDataReturn.ColumnNames.Type] = CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.Type];
                NewGridRowOfDependentNodes[CswNbtDemoDataReturn.ColumnNames.IsDemo] = ( "1" == CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.IsDemo].ToString() ) ? "yes" : "no";

                CswPrimaryKey cswPrimaryKey = new CswPrimaryKey();
                cswPrimaryKey.FromString( "nodes_" + CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.NodeId].ToString() );
                CswNbtNodeKey CswNbtNodeKey = new CswNbtNodeKey();
                CswNbtNodeKey.NodeId = cswPrimaryKey;
                CswNbtNodeKey.NodeTypeId = CswConvert.ToInt32( CurrentDependentNodeRow["nodetypeid"] );


                string menu_options = "{ ";
                menu_options += "\"nodeid\" : \"nodes_" + CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.NodeId].ToString() + "\",";
                menu_options += "\"nodename\" : \" " + CurrentDependentNodeRow[CswNbtDemoDataReturn.ColumnNames.Name].ToString() + "\",";
                menu_options += "\"nodekey\" : \" " + CswNbtNodeKey.ToString() + "\"";
                menu_options += " }";


                NewGridRowOfDependentNodes[CswNbtDemoDataReturn.ColumnNames.MenuOptions] = menu_options;

            }//iterate result rows

            CswNbtGrid Grid = new CswNbtGrid( CswNbtResources );
            Return.Data.Grid = Grid.DataTableToGrid( GridTable, IncludeEditFields: false );

        }//getDemoDataNodesAsGrid() 


        public static void updateDemoData( ICswResources CswResources, CswNbtDemoDataReturn Return, CswNbtDemoDataRequests.CswUpdateDemoNodesRequest Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( CswNbtResources );
            CswNbtResources OtherResources = wsMd.makeSystemUserResources( CswNbtResources.AccessId, false );

            CswNbtActDeleteDemoData CswNbtActDeleteDemoData = new CswNbtActDeleteDemoData( OtherResources );

            List<string> Errors = new List<string>();

            CswNbtActDeleteDemoData.updateDemoData( Request.node_ids_convert_to_non_demo, Request.view_ids_convert_to_non_demo, Request.node_ids_delete, Request.view_ids_delete, Errors );

            foreach( string CurrentError in Errors )
            {
                Return.addException( CswResources, new CswDniException( "There were errors updating the demo data: " + CurrentError ) );
            }

            OtherResources.finalize();
            OtherResources.release();
        }


        #endregion public



    } // class CswNbtDemoDataManager

} // namespace ChemSW.Nbt.WebServices
