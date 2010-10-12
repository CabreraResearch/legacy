using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Node_KeepPropCollectionUpToDate : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Node_KeepPropCollectionUpToDate( )
            : base( "Keep prop collection updated" )
        {
            _AppVenue = AppVenue.NBT;
        }//ctor

        public override void runTest()
        {

            //Update node through nodeprop collection
            _CswNbtSchemaModTrnsctn.beginTransaction();


            string RoleDescriptionValInNode = "node-update-value";
            string RoleDescriptionValInTable = "table-update-value";

            CswNbtView RolesView = _CswNbtSchemaModTrnsctn.getTreeViewOfNodeType( _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" ).NodeTypeId );
            ICswNbtTree RolesTree = _CswNbtSchemaModTrnsctn.getTreeFromView( RolesView, true );
            bool AdministratorFound = false;
            CswNbtNode AdministratorRoleNode = null;
            for ( int i = 0; !AdministratorFound && i < RolesTree.getChildNodeCount(); i++ )
            {
                RolesTree.goToNthChild( i );
                CswNbtNode RoleNode = RolesTree.getNodeForCurrentPosition();
                if ( "Administrator" == RoleNode.NodeName )
                {
                    AdministratorFound = true;
                    AdministratorRoleNode = RoleNode;
                }

                RolesTree.goToParentNode();
            }//iterate roles
            
            CswPrimaryKey AdministratorRoleNodeId = AdministratorRoleNode.NodeId;
                
            if ( null == AdministratorRoleNode )
                throw ( new CswScmUpdt_Exception( "Test case cannot proceed: administrator role node not found" ) );

            AdministratorRoleNode.Properties[ _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" ).getNodeTypeProp( "Description" ) ].AsMemo.Text = RoleDescriptionValInNode;
            AdministratorRoleNode.postChanges( true );

            _CswNbtSchemaModTrnsctn.commitTransaction();


            //Update node through table
            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswTableUpdate JctNodesPropsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("CswScmUpdt_TstCse_Node_KeepPropCollectionUpToDate_update1", "jct_nodes_props" );
            string WhereClause = @"where jctnodepropid = (select p.jctnodepropid 
                                                            from jct_nodes_props p 
                                                            join nodes n on (p.nodeid = n.nodeid) 
                                                            join nodetypes t on (n.nodetypeid = t.nodetypeid ) 
                                                            join nodetype_props ntp on (p.nodetypepropid=ntp.nodetypepropid) 
                                                           where t.nodetypename = 'Role' 
                                                             and n.nodename = 'Administrator' 
                                                             and ntp.propname = 'Description')";
            DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( WhereClause, true );
            JctNodesPropsTable.Rows[ 0 ][ "gestalt" ] = RoleDescriptionValInTable;
            JctNodesPropsUpdate.update( JctNodesPropsTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            //Update node through table

            //See which value is in the node now
            _CswNbtSchemaModTrnsctn.beginTransaction();
            //RolesView = _CswNbtSchemaModTrnsctn.getTreeViewOfNodeType( _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" ).NodeTypeId );
            //RolesTree = _CswNbtSchemaModTrnsctn.getTreeFromView( RolesView, true );
            //AdministratorFound = false;
            //AdministratorRoleNode = null;
            //for ( int i = 0; !AdministratorFound && i < RolesTree.getChildNodeCount(); i++ )
            //{
            //    RolesTree.goToNthChild( i );
            //    CswNbtNode RoleNode = RolesTree.getNodeForCurrentPosition();
            //    if ( "Administrator" == RoleNode.NodeName )
            //    {
            //        AdministratorFound = true;
            //        AdministratorRoleNode = RoleNode;
            //    }

            //    RolesTree.goToParentNode();
            //}//iterate roles

            AdministratorRoleNode = _CswNbtSchemaModTrnsctn.Nodes[AdministratorRoleNodeId];

            if ( null == AdministratorRoleNode )
                throw ( new CswScmUpdt_Exception( "Test case cannot proceed: administrator role node not found" ) );

            string PropVal = AdministratorRoleNode.Properties[ _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" ).getNodeTypeProp( "Description" ) ].AsMemo.Text;

            if ( RoleDescriptionValInTable != PropVal )
                throw ( new CswScmUpdt_Exception( "The node prop collection has stale values" ) );

            _CswNbtSchemaModTrnsctn.commitTransaction();


            //clean up after ourselves:
            _CswNbtSchemaModTrnsctn.beginTransaction();

            JctNodesPropsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Node_KeepPropCollectionUpToDate_update2","jct_nodes_props" );
            WhereClause = @"where jctnodepropid = (select p.jctnodepropid 
                                                     from jct_nodes_props p 
                                                     join nodes n on (p.nodeid = n.nodeid) 
                                                     join nodetypes t on (n.nodetypeid = t.nodetypeid ) 
                                                     join nodetype_props ntp on (p.nodetypepropid=ntp.nodetypepropid) 
                                                    where t.nodetypename = 'Role' 
                                                      and n.nodename = 'Administrator' 
                                                      and ntp.propname = 'Description')";
            JctNodesPropsTable = JctNodesPropsUpdate.getTable( WhereClause, true );
            JctNodesPropsTable.Rows[ 0 ].Delete();
            JctNodesPropsUpdate.update( JctNodesPropsTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();


        }//runTest()

    }//CswSchemaUpdaterTestCaseRollbackViewUpdates

}//ChemSW.Nbt.Schema
