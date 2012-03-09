using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25426
    /// </summary>
    public class CswUpdateSchemaCase25426 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswTableSelect NodesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "25426_nodes_select", "nodes" );
            DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" } );
            foreach( DataRow NodeRow in NodesTable.Rows )
            {
                CswNbtNode Node = getNode( NodeRow["nodeid"] );
                setIsDemo( Node );
            }


        }//Update()

        private CswNbtNode getNode( object NodeIdCol )
        {
            CswNbtNode Ret = null;
            Int32 NodeId = CswConvert.ToInt32( NodeIdCol );
            if( Int32.MinValue != NodeId )
            {
                CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", NodeId );
                Ret = _CswNbtSchemaModTrnsctn.Nodes.GetNode( NodePk );
            }
            return Ret;
        }

        private void setIsDemo( CswNbtNode Node )
        {
            if( null != Node )
            {
                CswNbtMetaDataObjectClass.NbtObjectClass NodeOc = Node.ObjClass.ObjectClass.ObjectClass;
                if( NodeOc == CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass ||
                   NodeOc == CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass ||
                    NodeOc == CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ||
                    NodeOc == CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass ||
                    NodeOc == CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass
                    )
                {
                    Node.IsDemo = false;
                }
                else
                {
                    Node.IsDemo = true;
                }
                Node.postChanges( true );
            }
        }

    }//class CswUpdateSchemaCase25426

}//namespace ChemSW.Nbt.Schema