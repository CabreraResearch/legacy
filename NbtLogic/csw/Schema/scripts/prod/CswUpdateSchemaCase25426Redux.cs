using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25426Redux
    /// </summary>
    public class CswUpdateSchemaCase25426Redux : CswUpdateSchemaTo
    {
        private Dictionary<Int32, string> DemoDataNodes = new Dictionary<Int32, string>();

        private void FillDemoNodesDictionary()
        {
            DemoDataNodes.Add( 23249, "Admissions" );
            DemoDataNodes.Add( 23250, "Second Floor" );
            DemoDataNodes.Add( 23251, "Outer Hallway" );
            DemoDataNodes.Add( 23867, "Equipment Problem 23867" );
            DemoDataNodes.Add( 23256, "Visitor Entrance" );
            DemoDataNodes.Add( 23868, "Equipment Problem 23868" );
            DemoDataNodes.Add( 23259, "Office 201" );
            DemoDataNodes.Add( 23262, "Lab 1" );
            DemoDataNodes.Add( 23265, "First Floor" );
            DemoDataNodes.Add( 23266, "Registrar" );
            DemoDataNodes.Add( 23479, "Equipment Mail Report" );
            DemoDataNodes.Add( 23327, "Library" );
            DemoDataNodes.Add( 23331, "Third Floor" );
            DemoDataNodes.Add( 23332, "Main Hall" );
            DemoDataNodes.Add( 24056, "Check Grease (1/10/2011)" );
            DemoDataNodes.Add( 24057, "Check Grease (1/17/2011)" );
            DemoDataNodes.Add( 23336, "Center Hall" );
            DemoDataNodes.Add( 24058, "Check Grease (1/24/2011)" );
            DemoDataNodes.Add( 24059, "Check Grease (1/31/2011)" );
            DemoDataNodes.Add( 23339, "Ground Floor" );
            DemoDataNodes.Add( 23340, "Reception" );
            DemoDataNodes.Add( 24060, "Check Grease (2/7/2011)" );
            DemoDataNodes.Add( 24061, "Check Grease (2/14/2011)" );
            DemoDataNodes.Add( 23343, "Checkout" );
            DemoDataNodes.Add( 24062, "Check Grease (2/21/2011)" );
            DemoDataNodes.Add( 24063, "Check Grease (2/28/2011)" );
            DemoDataNodes.Add( 23346, "Archives" );
            DemoDataNodes.Add( 23349, "Welcome Hall" );
            DemoDataNodes.Add( 23229, "Cabinet N101-0246" );
            DemoDataNodes.Add( 23230, "Cabinet N101-0255" );
            DemoDataNodes.Add( 23231, "Cabinet N101-0298" );
            DemoDataNodes.Add( 22988, "250 Series 200 LC Pump" );
            DemoDataNodes.Add( 22991, "HPLC-00972" );
            DemoDataNodes.Add( 22989, "251 Series 200a Refractive Index Detector" );
            DemoDataNodes.Add( 23226, "Room N101" );
            DemoDataNodes.Add( 23227, "Room N103" );
            DemoDataNodes.Add( 6, "Check Grease" );
            DemoDataNodes.Add( 7, "(7/16/2011)" );
            DemoDataNodes.Add( 23204, "Pump has leak" );
            DemoDataNodes.Add( 23228, "Room S206" );
            DemoDataNodes.Add( 23461, "Inner Hallway" );
            DemoDataNodes.Add( 17021, "Sample Equipment Label" );
            DemoDataNodes.Add( 19926, "North Research Lab" );
            DemoDataNodes.Add( 23225, "South Research Lab" );
            DemoDataNodes.Add( 22972, "Test 22972" );
            DemoDataNodes.Add( 22973, "Parameter 22973" );
            DemoDataNodes.Add( 22976, "5" );
            DemoDataNodes.Add( 22977, "6" );
            DemoDataNodes.Add( 22979, "Result 22979" );
            DemoDataNodes.Add( 22982, "Pump" );
            DemoDataNodes.Add( 22984, "Detector" );
            DemoDataNodes.Add( 23232, "249 Constant-Flow Gradient HPLC Piston Pump" );
            DemoDataNodes.Add( 23182, "252 Bio C18 5um; 100 mm x 2.1 mm; 300Å Carbon load 6%; end capped" );
            DemoDataNodes.Add( 22983, "Column" );
            DemoDataNodes.Add( 23162, "Clean Lens" );
            DemoDataNodes.Add( 22992, "HPLC" );
            DemoDataNodes.Add( 23400, "First Floor" );
            DemoDataNodes.Add( 23401, "Reading Room" );
            DemoDataNodes.Add( 23406, "Second Floor" );
            DemoDataNodes.Add( 23407, "Center Hall" );
            DemoDataNodes.Add( 23412, "Checkout" );
            DemoDataNodes.Add( 23530, "B Monthly Physical Inspection" );
            DemoDataNodes.Add( 23563, "A Monthly Physical Inspection" );
            DemoDataNodes.Add( 24424, "1" );
            DemoDataNodes.Add( 24704, "Site 1" );
            DemoDataNodes.Add( 24724, "Example Group" );
            DemoDataNodes.Add( 24725, "Example Group Example Schedule" );
            DemoDataNodes.Add( 24726, "382 Example Target" );
            DemoDataNodes.Add( 24727, "SI_protocol" );
        }

        public override void update()
        {

            CswTableSelect NodesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "25426Redux_nodes_select", "nodes" );
            DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" } );
            FillDemoNodesDictionary();
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
                    string NodeName;
                    DemoDataNodes.TryGetValue( Node.NodeId.PrimaryKey, out NodeName );
                    if( NodeName == Node.NodeName )
                    {
                        Node.IsDemo = true;
                    }
                    else
                    {
                        Node.IsDemo = false;
                    }
                }
                Node.postChanges( true );
            }
        }

    }//class CswUpdateSchemaCase25426Redux

}//namespace ChemSW.Nbt.Schema