using System.Data;
using ChemSW;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;

namespace NbtWebApp.Actions.Explorer
{
    public class CswNbtWebServiceNodeExplorer
    {
        public static void Initialize( ICswResources CswResources, CswNbtExplorerReturn Return, string NodeIdStr )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey NodeId = CswConvert.ToPrimaryKey( NodeIdStr );
            _recurse( NbtResources, Return, NodeId.PrimaryKey.ToString(), 1, 2 );
        }

        private static void _recurse( CswNbtResources NbtResources, CswNbtExplorerReturn Return, string NodeId, int Level, int MaxLevel )
        {
            string sql = @"select jnp.field1_fk id, n.nodename, nt.iconfilename from jct_nodes_props jnp 
                                   join nodes n on n.nodeid = jnp.field1_fk
                                   join nodetypes nt on n.nodetypeid = nt.nodetypeid
                            where jnp.nodeid = " + NodeId + @" and nodetypepropid in 
                                   (select nodetypepropid from nodetype_props ntp where ntp.fieldtypeid = 
                                           (select fieldtypeid from field_types ft where ft.fieldtype = 'Relationship'))";
            CswArbitrarySelect nodesIRelateToAS = NbtResources.makeCswArbitrarySelect( "nodesIRelateTo", sql );
            DataTable nodesIRelateToDT = nodesIRelateToAS.getTable();

            string sql2 = @"select jnp.nodeid id, n.nodename, nt.iconfilename from jct_nodes_props jnp 
                                   join nodes n on n.nodeid = jnp.nodeid
                                   join nodetypes nt on n.nodetypeid = nt.nodetypeid
                            where jnp.field1_fk = " + NodeId + @" and nodetypepropid in 
                                   (select nodetypepropid from nodetype_props ntp where ntp.fieldtypeid = 
                                           (select fieldtypeid from field_types ft where ft.fieldtype = 'Relationship'))";
            CswArbitrarySelect nodesRelatingToMeAS = NbtResources.makeCswArbitrarySelect( "nodesIRelateTo", sql2 );
            DataTable nodesRelatingToMeDT = nodesRelatingToMeAS.getTable();

            _populateArborGraph( nodesIRelateToDT, Return.Data, NodeId, true, Level );
            _populateArborGraph( nodesRelatingToMeDT, Return.Data, NodeId, false, Level );

            if( Level != MaxLevel )
            {
                foreach( DataRow Row in nodesIRelateToDT.Rows )
                {
                    _recurse( NbtResources, Return, Row["id"].ToString(), Level + 1, MaxLevel );
                }
                foreach( DataRow Row in nodesRelatingToMeDT.Rows )
                {
                    _recurse( NbtResources, Return, Row["id"].ToString(), Level + 1, MaxLevel );
                }
            }
        }

        private static void _populateArborGraph( DataTable Tbl, CswNbtArborGraph Return, string CurrentNodeId, bool IsOwner, int Level )
        {
            foreach( DataRow Row in Tbl.Rows )
            {
                string RowNodeId = Row["id"].ToString();
                Return.Nodes.Add( new CswNbtArborNode
                    {
                        NodeIdStr = RowNodeId,
                        Data = new CswNbtArborNode.CswNbtArborNodeData
                            {
                                Icon = "Images/newicons/16/" + Row["iconfilename"].ToString(),
                                Label = Row["nodename"].ToString()
                            }
                    } );

                Return.Edges.Add( new CswNbtArborEdge
                    {
                        OwnerNodeIdStr = IsOwner ? CurrentNodeId : RowNodeId,
                        TargetNodeIdStr = IsOwner ? RowNodeId : CurrentNodeId,
                        Data = new CswNbtArborEdge.CswNbtArborEdgeData
                            {
                                Length = Level * 10
                            }
                    } );
            }
        }

    }
}