using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Grid.ExtJs;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.NbtSchedSvcRef;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;
using Newtonsoft.Json.Linq;
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



        public static void getDemoDataGrid( ICswResources CswResources, CswNbtDemoDataReturn Return, object Request )
        {

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;



            //Build table infrastructure
            DataTable GridTable = new DataTable( "demodatatable" );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.NodeId, typeof( Int32 ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Name, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Type, typeof( string ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsUsedBy, typeof( sbyte ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.IsRequiredBy, typeof( sbyte ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.Delete, typeof( Boolean ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.ConvertToDemo, typeof( Boolean ) );
            GridTable.Columns.Add( CswNbtDemoDataReturn.ColumnNames.MenuOptions, typeof( string ) );

            //*****************************
            //Populate views
            string ViewQuery = @"select v.viewname, v.nodeviewid
                                  from node_views v
                                 where isdemo = '1'
                                   and visibility <> 'Property'
                                   and visibility <> 'Hidden'
                                 order by viewname";

            CswArbitrarySelect ArbitraryViewsSelect = CswNbtResources.makeCswArbitrarySelect( "select_demo_nodes", ViewQuery );
            DataTable DemoViewsTable = ArbitraryViewsSelect.getTable();
            foreach( DataRow CurrentDemoViewRow in DemoViewsTable.Rows )
            {
                DataRow NewGridRowOfDemoViews = GridTable.NewRow();
                GridTable.Rows.Add( NewGridRowOfDemoViews );
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.Name] = CurrentDemoViewRow["viewname"].ToString();
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.Type] = "View";
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = 0;
                NewGridRowOfDemoViews[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = 0;
                //NewGridRow[CswNbtDemoDataReturn.ColumnNames.Delete] = false ;
                //NewGridRow[CswNbtDemoDataReturn.ColumnNames.ConvertToDemo] = false ;
            }//iterate demo views rows


            //*****************************
            //Populate views
            string NodesQuery = @"select n." + CswNbtDemoDataReturn.ColumnNames.NodeId + @",n.nodename,t.nodetypename
                                    from nodes n 
                                    join nodetypes t on (n.nodetypeid=t.nodetypeid )
                                    where n.isdemo = '1'
                                    order by t.nodetypename, lower( n.nodename ) ";

            CswArbitrarySelect ArbitraryNodesSelect = CswNbtResources.makeCswArbitrarySelect( "select_demo_nodes", NodesQuery );
            DataTable DemoNodesTable = ArbitraryNodesSelect.getTable();
            foreach( DataRow CurrentDemoNodeRow in DemoNodesTable.Rows )
            {
                DataRow NewGridRowOfNodes = GridTable.NewRow();
                GridTable.Rows.Add( NewGridRowOfNodes );
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.NodeId] = CurrentDemoNodeRow["nodeid"].ToString();
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.Name] = CurrentDemoNodeRow["nodename"].ToString();
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.Type] = CurrentDemoNodeRow["nodetypename"].ToString();

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
                                                where f.fieldtype='Relationship'
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
                    }//if-else it's required

                }//iterate nodes used by rows


                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.MenuOptions] = "Required By " + RequiredByNodeIds.ToString() + "; Used By " + UsedByNodeIds.ToString();
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = UsedByCount;
                NewGridRowOfNodes[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = RequiredByCount;

            }//iterate node rows



            //Test data to populate grid
            //DataRow NewRow = GridTable.NewRow();
            //GridTable.Rows.Add( NewRow );
            //NewRow[CswNbtDemoDataReturn.ColumnNames.Name] = "Container";
            //NewRow[CswNbtDemoDataReturn.ColumnNames.Type] = "Node";
            //NewRow[CswNbtDemoDataReturn.ColumnNames.IsUsedBy] = "5";
            //NewRow[CswNbtDemoDataReturn.ColumnNames.IsRequiredBy] = "1";
            //NewRow[CswNbtDemoDataReturn.ColumnNames.Delete] = CswConvert.ToDbVal( true );
            //NewRow[CswNbtDemoDataReturn.ColumnNames.ConvertToDemo] = CswConvert.ToDbVal( false );


            CswNbtGrid Grid = new CswNbtGrid( CswNbtResources );
            Return.Data.Grid = Grid.DataTableToGrid( GridTable, IncludeEditFields: false );


        }//getDemoDataGrid()

        #endregion public



    } // class CswNbtDemoDataManager

} // namespace ChemSW.Nbt.WebServices
