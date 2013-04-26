using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Grid.ExtJs;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Tree;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.Actions
{




    /// <summary>
    /// Holds logic for deleting demo data
    /// </summary>
    public class CswNbtActDeleteDemoData
    {

        private CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtActDeleteDemoData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void updateDemoData( List<string> node_ids_convert_to_non_demo, List<string> view_ids_convert_to_non_demo, List<string> node_ids_remove, List<string> view_ids_remove )
        {
            //*****************
            //just in case: converto-to-non-demo takes precendence over delete?
            foreach( string CurrentViewId in view_ids_convert_to_non_demo )
            {
                if( view_ids_remove.Contains( CurrentViewId ) )
                {
                    view_ids_remove.Remove( CurrentViewId );
                }
            }

            foreach( string CurrentnodeId in node_ids_convert_to_non_demo )
            {
                if( node_ids_remove.Contains( CurrentnodeId ) )
                {
                    node_ids_remove.Remove( CurrentnodeId );
                }
            }



            //**********************
            // Views 
            CswDelimitedString ViewIdsToconvert = new CswCommaDelimitedString();
            ViewIdsToconvert.FromArray( view_ids_convert_to_non_demo.ToArray() );

            CswDelimitedString ViewIdsToDelete = new CswCommaDelimitedString();
            ViewIdsToDelete.FromArray( view_ids_remove.ToArray() );

            string ViewIds = ViewIdsToDelete.ToString() + ViewIdsToconvert.ToString();

            if( ViewIds.Length > 0 )
            {

                CswTableUpdate ConvertViewsToNonDemoUpdate = _CswNbtResources.makeCswTableUpdate( "update_views_to_non_demo", "node_views" );
                DataTable ViewsTable = ConvertViewsToNonDemoUpdate.getTable( " where nodeviewid in (" + ViewIds + ")" );
                foreach( DataRow CurrentRow in ViewsTable.Rows )
                {
                    string CurrentViewId = CurrentRow["nodeviewid"].ToString();
                    if( ViewIdsToconvert.Contains( CurrentViewId ) )
                    {
                        CurrentRow["isdemo"] = CswConvert.ToDbVal( false );
                    }
                    else if( ViewIdsToDelete.Contains( CurrentViewId ) )
                    {
                        CurrentRow.Delete();
                    }
                }

                ConvertViewsToNonDemoUpdate.update( ViewsTable );
            }

            //***************************************************************
            // ***** Nodes
            //**********************
            if( node_ids_convert_to_non_demo.Count > 0 )
            {
                CswDelimitedString NodesIdsToconvert = new CswCommaDelimitedString();
                NodesIdsToconvert.FromArray( node_ids_convert_to_non_demo.ToArray() );

                CswTableUpdate ConvertNodesToNonDemoUpdate = _CswNbtResources.makeCswTableUpdate( "update_Nodes_to_non_demo", "nodes" );
                DataTable NodesTable = ConvertNodesToNonDemoUpdate.getTable( " where Nodeid in (" + NodesIdsToconvert + ")" );
                foreach( DataRow CurrentRow in NodesTable.Rows )
                {
                    CurrentRow["isdemo"] = CswConvert.ToDbVal( false );
                }

                ConvertNodesToNonDemoUpdate.update( NodesTable );

            }//if we have nodes to convert

            if( node_ids_remove.Count > 0 )
            {
                foreach( string NodeIdToRemove in node_ids_remove )
                {
                    CswPrimaryKey NodePrimeKey = new CswPrimaryKey();
                    NodePrimeKey.FromString( "nodes_" + NodeIdToRemove );
                    CswNbtNode CurrentNode = _CswNbtResources.Nodes[NodePrimeKey];
                    if( null != CurrentNode )
                    {
                        CurrentNode.delete();
                    }

                }//iterate node ids to remove

            }//if we have nodes to delete

        }//updateDemoData()

    } // class   CswNbtActDeleteDemoData
}// namespace ChemSW.Nbt.Actions