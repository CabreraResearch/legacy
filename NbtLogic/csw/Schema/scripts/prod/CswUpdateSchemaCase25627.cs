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
    /// Updates the schema for case 25627
    /// </summary>
    public class CswUpdateSchemaCase25627 : CswUpdateSchemaTo
    {

        public override void update()
        {
            Dictionary<Int32, string> DoomedNodes = new Dictionary<Int32, string>();

            DoomedNodes.Add( 22976, "5" );
            DoomedNodes.Add( 22977, "6" );
            DoomedNodes.Add( 23563, "A Monthly Physical Inspection" );
            DoomedNodes.Add( 23530, "B Monthly Physical Inspection" );
            DoomedNodes.Add( 23813, "FE Inspection Point, On Edit Status OOC" );
            DoomedNodes.Add( 23811, "Physical Inspection, On Edit Cancelled true" );
            DoomedNodes.Add( 23812, "Physical Inspection, On Edit Status Missed" );
            DoomedNodes.Add( 22973, "Parameter 22973" );
            DoomedNodes.Add( 22979, "Result 22979" );
            DoomedNodes.Add( 22972, "Test 22972" );
            DoomedNodes.Add( 22947, "cells" );
            DoomedNodes.Add( 22945, "cells/mL" );
            DoomedNodes.Add( 22946, "colonies/cm2" );
            DoomedNodes.Add( 22944, "particles/mL" );
            DoomedNodes.Add( 22948, "Units" );
            DoomedNodes.Add( 22949, "Units/mL" );
            DoomedNodes.Add( 23248, "Kidde" );

            foreach( Int32 NodeId in DoomedNodes.Keys )
            {
                CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", NodeId );
                CswNbtNode Node = _CswNbtSchemaModTrnsctn.Nodes[NodePk];
                if( Node != null && Node.NodeName == DoomedNodes[NodeId] )
                {
                    Node.delete();
                }
            }

        }//Update()


    }//class CswUpdateSchemaCase25627
}//namespace ChemSW.Nbt.Schema