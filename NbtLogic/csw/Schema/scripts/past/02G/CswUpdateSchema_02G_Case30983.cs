using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30983 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30983; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Remove temp nodes"; }
        }

        public override void update()
        {
            // Adapted from CswNbtSessionDataMgr.removeAllSessionData()

            CswTableSelect NodesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "30983_nodes_select", "nodes" );
            DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" }, "where istemp = '1'" );
            if( NodesTable.Rows.Count > 0 )
            {
                Collection<CswNbtNode> DoomedNodes = new Collection<CswNbtNode>();
                foreach( DataRow Row in NodesTable.Rows )
                {
                    CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["nodeid"] ) );
                    if( CswTools.IsPrimaryKey( NodeId ) )
                    {
                        CswNbtNode TempNode = _CswNbtSchemaModTrnsctn.Nodes[NodeId];
                        if( null != TempNode )
                        {
                            DoomedNodes.Add( TempNode );
                        }
                    }
                }

                foreach( CswNbtNode DoomedNode in DoomedNodes )
                {
                    DoomedNode.delete( DeleteAllRequiredRelatedNodes: true, OverridePermissions: true );
                }

            } //there are nodes rows

        } // update()

    }

}//namespace ChemSW.Nbt.Schema