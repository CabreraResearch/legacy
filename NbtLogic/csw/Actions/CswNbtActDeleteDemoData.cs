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

        public void updateDemoData( List<string> node_ids_convert_to_non_demo, List<string> view_ids_convert_to_non_demo, List<string> node_ids_remove, List<string> view_ids_remove, List<string> Errors )
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
            if( view_ids_convert_to_non_demo.Count > 0 )
            {

                CswCommaDelimitedString ViewIdsToConvert = new CswCommaDelimitedString();
                ViewIdsToConvert.FromArray( view_ids_convert_to_non_demo.ToArray() );

                try
                {

                    CswTableUpdate ConvertViewsToNonDemoUpdate = _CswNbtResources.makeCswTableUpdate( "update_views_to_non_demo", "node_views" );
                    DataTable ViewsTable = ConvertViewsToNonDemoUpdate.getTable( " where nodeviewid in (" + ViewIdsToConvert.ToString() + ")" );
                    foreach( DataRow CurrentRow in ViewsTable.Rows )
                    {
                        CurrentRow["isdemo"] = CswConvert.ToDbVal( false );
                    }

                    ConvertViewsToNonDemoUpdate.update( ViewsTable );
                }
                catch( Exception Exception )
                {

                    Errors.Add( "Error converting demo views " + ViewIdsToConvert.ToString() + " to non-demo: " + Exception.Message );
                }

            }//if we have view to udpate


            if( view_ids_remove.Count > 0 )
            {

                CswCommaDelimitedString ViewIdsToRemove = new CswCommaDelimitedString();
                ViewIdsToRemove.FromArray( view_ids_remove.ToArray() );
                try
                {

                    CswTableUpdate ConvertViewsToNonDemoUpdate = _CswNbtResources.makeCswTableUpdate( "delete_demo_views", "node_views" );
                    string WhereClause = " where nodeviewid in (" + ViewIdsToRemove.ToString() + ")";
                    DataTable ViewsTable = ConvertViewsToNonDemoUpdate.getTable( WhereClause );
                    foreach( DataRow CurrentRow in ViewsTable.Rows )
                    {
                        CurrentRow.Delete();
                    }

                    ConvertViewsToNonDemoUpdate.update( ViewsTable );
                }
                catch( Exception Exception )
                {

                    Errors.Add( "Error removing demo views " + ViewIdsToRemove.ToString() + " : " + Exception.Message );
                }

            }//if we have view to udpate


            //***************************************************************
            // ***** Nodes
            //**********************
            if( node_ids_convert_to_non_demo.Count > 0 )
            {

                CswDelimitedString NodesIdsToconvert = new CswCommaDelimitedString();
                NodesIdsToconvert.FromArray( node_ids_convert_to_non_demo.ToArray() );

                try
                {

                    CswTableUpdate ConvertNodesToNonDemoUpdate = _CswNbtResources.makeCswTableUpdate( "update_Nodes_to_non_demo", "nodes" );
                    DataTable NodesTable = ConvertNodesToNonDemoUpdate.getTable( " where Nodeid in (" + NodesIdsToconvert + ")" );
                    foreach( DataRow CurrentRow in NodesTable.Rows )
                    {
                        CurrentRow["isdemo"] = CswConvert.ToDbVal( false );
                    }

                    ConvertNodesToNonDemoUpdate.update( NodesTable );
                }

                catch( Exception Exception )
                {

                    Errors.Add( "Error converting nodes " + NodesIdsToconvert.ToString() + " to non-demo: " + Exception.Message );
                }

            }//if we have nodes to convert


            if( node_ids_remove.Count > 0 )
            {
                foreach( string NodeIdToRemove in node_ids_remove )
                {
                    try
                    {
                        CswPrimaryKey NodePrimeKey = new CswPrimaryKey();
                        NodePrimeKey.FromString( "nodes_" + NodeIdToRemove );
                        CswNbtNode CurrentNode = _CswNbtResources.Nodes[NodePrimeKey];
                        if( null != CurrentNode )
                        {
                            CurrentNode.delete();
                        }

                    }
                    catch( Exception Exception )
                    {
                        string Error = "Error removing nodes to non-demo: " + Exception.Message;
                        Errors.Add( Error );
                    }

                }//iterate node ids to remove

            }//if we have nodes to delete

        }//updateDemoData()

    } // class   CswNbtActDeleteDemoData
}// namespace ChemSW.Nbt.Actions